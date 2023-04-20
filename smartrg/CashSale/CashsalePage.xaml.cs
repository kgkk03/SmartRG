using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.CashSale
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CashsalePage : ContentPage
    {
        string OwnerPage = "CashsalePage";
        string MyPage = "CashsalePage";
        Models.CashSaleData ActiveData = null;
        Models.VisitData ActiveVisit = null;
        List<Models.CashSalelineData> ListDetails= new List<Models.CashSalelineData>();
        List<Models.PickingLineData> ListStock = new List<Models.PickingLineData>();
        List<Models.ProductInStock> ListProduct = new List<Models.ProductInStock>();
        List<Models.PaymentData> ListPaymentData = new List<Models.PaymentData>();
        Models.CustomerData Customer = new Models.CustomerData();
        bool IsEdit = false;

        public CashsalePage()
        {
            InitializeComponent();
        }
        void ShowHeader()
        {

            var data = ListDetails;
            string details = "";
            string sumary = "";

            if (data != null && data.Count > 0)
            {
                int line = ListDetails.Count;
                int qty = ListDetails.Sum(x => x.Qty);
                double amount = ListDetails.Sum(x => x.Amount);
                details = line.ToString() + " รายการ จำนวน " + qty.ToString() + " ขวด";
                sumary = "รวม : " + amount.ToString("#,##0") + "บาท";
            }
            LblSaledate.Text = Helpers.Controls.Date2ThaiString(DateTime.Now, "d-MMM-yyyy");
            LblWorksumary.Text = sumary;
            LblWorkdetails.Text = details;
            BtnSend.Source = (ActiveData.Transtatus == 0? "ic_send" : "ic_print");
            BtnAdd.IsVisible = (ActiveData.Transtatus == 0);
            ListData.ItemsSource = null;
            ListData.ItemsSource = ListDetails;
        }


        #region create cash sale
        public async void SetNewData(string ownerpage, Models.CustomerData customer)
        {
            try
            {
                AidWaitingRun(true);
                OwnerPage = ownerpage;
                Customer = customer;
                if (Customer != null)
                {
                    LblCustname.Text = Customer.Custname + " (" + Customer.Custgroupname + ")";
                    ImgCustomer.Source = Customer.Icon;
                    LblAddress.Text = Customer.Custaddress;
                    ActiveVisit = await Helpers.Controls.GetVisitdata(Customer, null);
                    ActiveVisit.Transtatus = 14;
                    ActiveVisit.Planid = "";
                    ActiveVisit.Planstatus = -3;
                    string msg = await App.Ws.SaveVisit(ActiveVisit);
                    ActiveData = GetNewData(Customer, ActiveVisit.Key);
                    ListDetails = new List<Models.CashSalelineData>();
                    ListStock = await App.Ws.GetPickingStock("");
                    ListProduct = await GetProductInStock(ListStock);
                }
                LblWorkdetails.Text = "จำนวน 0 รายการ, รวม 0 ขวด, รวมเป็นเงิน 0.00 บาท";
                ShowHeader();
            }
            catch { }
            AidWaitingRun(false);
        }
        Models.CashSaleData GetNewData(Models.CustomerData customer,string visitid)
        {
            try
            {
                DateTime req = App.Servertime.AddDays(2);
                var result = new Models.CashSaleData()
                {
                    Key = visitid,
                    Custcode = customer.Custcode,
                    Custid = customer.Key,
                    Custname = customer.Custname,
                    Custax = customer.TaxID,
                    Icon = customer.Icon,
                    Empid = App.UserProfile.Empid,
                    Salename = App.UserProfile.Fullname,
                    Custaddress = customer.Custaddress,
                    Showtime=Helpers.Controls.Date2String(App.Servertime,"HH;mm"),
                };
                return result;
            }
            catch (Exception ex) { DisplayAlert("Set New SO Error", ex.Message, "OK"); }
            return null;
        }
        async Task<List<Models.ProductInStock>> GetProductInStock(List<Models.PickingLineData> laststock)
        {
            try
            {
                List<Models.ProductInStock> result = new List<Models.ProductInStock>();
                var stock = (from itme in laststock
                             group itme by new { itme.Productid } into grp
                             select new
                             {
                                 grp.Key.Productid,
                                 Qty = grp.Sum(x => x.Balance),
                                 Totalagent = grp.Count(),
                                 Maxstock = grp.Max(x => x.Balance),
                                 Stockdata = grp,
                             }).ToList();
                string productid = "";
                foreach (var dr in stock) { productid += ((productid.Equals("") ? "" : ",") + dr.Productid); }
                List<Models.ProductData> listproduct = await App.Ws.GetProductList("", productid);

                foreach (var dr in stock)
                {
                    var stockdata = laststock.FindAll(x => x.Productid.Equals(dr.Productid));
                    var product = listproduct.Where(x => x.Productid.Equals(dr.Productid)).FirstOrDefault();

                    var temp = new Models.ProductInStock()
                    {
                        Productid = dr.Productid,
                        LastStock = dr.Qty,
                        Totalagent = dr.Totalagent,
                        Maxstock = dr.Maxstock,
                        Balance = dr.Qty,
                        Stockdata = stockdata,
                        Product = product,
                    };
                    result.Add(temp);
                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                await DisplayAlert("SalePage GetProductInStock Error", ex.Message, "OK");
                return await Task.FromResult(new List<Models.ProductInStock>());
            }
        }
        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            if (IsEdit || ActiveData.Transtatus > 0) { return; }
            try
            {
                IsEdit = true;
                var sa = new Models.CashSalelineData() { Visitid = ActiveData.Key,Custid=Customer.Key,Custname=Customer.Custname };
                Tools.KeyboardSalePage kb = new Tools.KeyboardSalePage();
                if (ListProduct == null) { ListProduct = ListProduct = await GetProductInStock(ListStock); }
                if (ListProduct != null && ListProduct.Count > 0)
                {
                    if (kb.Setdata(MyPage, sa, ListProduct, ListDetails))
                    {
                        await Navigation.PushModalAsync(kb);
                        KeyboardSaleMessage();
                        return;
                    }
                }
                await DisplayAlert("แจ้งเตือน", "ไม่พบข้อมูลสินค้าที่สามารถขายได้", "ตกลง");
            }
            catch (Exception ex) {
                IsEdit = false;
                await DisplayAlert("Add Saleorder data Error", ex.Message, "OK"); 
            }
        }
        private async void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (IsEdit || ActiveData.Transtatus > 0) { ListData.SelectedItem = null; return; }
                IsEdit = true;
                Models.CashSalelineData item = (Models.CashSalelineData)e.CurrentSelection.FirstOrDefault();
                Tools.KeyboardSalePage kb = new Tools.KeyboardSalePage();
                if (ListProduct == null) { ListProduct = ListProduct = await GetProductInStock(ListStock); }
                if (ListProduct != null && ListProduct.Count > 0)
                {
                    if (kb.Setdata(MyPage, item, ListProduct, ListDetails))
                    {
                        await Navigation.PushModalAsync(kb);
                        KeyboardSaleMessage();
                        ListData.SelectedItem = null;
                        return;
                    }
                }
                await DisplayAlert("แจ้งเตือน", "ไม่พบข้อมูลสินค้าที่สามารถขายได้", "ตกลง");
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            ListData.SelectedItem = null;
        }
        void KeyboardSaleMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.KeyboardSalePage, Models.CashSalelineData>(this, MyPage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Unsubscribe<Tools.KeyboardSalePage, Models.CashSalelineData>(this, MyPage);
                        try { 
                            if (arg != null) { SetSaleData(arg); } 
                        } catch { }
                        IsEdit = false;
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("Set MessagingCenter Error", ex.Message, "OK"); }
        }
        void SetSaleData(Models.CashSalelineData data)
        {
            try
            {
                if (data.Qty != 0)
                {
                    if (data.Item == 0)
                    {
                        data.Item = ListDetails.Count + 1;
                        data.Key = ActiveData.Key + "-" + data.Item.ToString();
                        ListDetails.Add(data);
                    }
                }
                else
                {
                    if (data.Item != 0) { RemoveSaleData(data); }
                }
                ShowHeader() ;

            }
            catch (Exception ex) { DisplayAlert("Set Sale Data Error", ex.Message, "OK"); }

        }
        void RemoveSaleData(Models.CashSalelineData data)
        {
            try
            {
              
                var item = ListDetails.Find(x => x.Key.Equals(data.Key));
                if (item != null) { item.Qty = 0; }
                // ลบข้อมูลเก่าในตารางทิ้งก่อน
                ListDetails.Remove(item);

                // เรียงลำดับข้อมูลใหม่ 
                var sortdata = ListDetails.Where(x => x.Qty > 0).OrderBy(x => x.Productname).ThenByDescending(x => x.Qty).ToList();
                ListDetails = sortdata;
                int i = 1;
                foreach (var dr in ListDetails)
                {
                    dr.Item = i;
                    dr.Key = ActiveData.Key + "-" + i.ToString();
                    i++;
                }
            }
            catch (Exception ex) { DisplayAlert("Remove Data Error", ex.Message, "OK"); }
        }

        #endregion

        #region Payment
        private void BtnOpenPayment_Clicked(object sender, EventArgs e)
        {
            //if (IsEdit || ListPaymentData==null) { return; }
            if (ListPaymentData==null) { return; }
            if (ListPaymentData.Count == 0) { OpenPaymentKeyboard(); }
            else { Stk_Payment.IsVisible=true; }
            
        }
        private void btnClosePayment_Clicked(object sender, EventArgs e)
        {
            IsEdit = false; Stk_Payment.IsVisible = false;
        }
        async void OpenPaymentKeyboard()
        {
            try
            {
                if(ActiveData.Transtatus > 0) { IsEdit = false; return; }
                IsEdit = true;
                double amount = ListDetails.Sum(x => x.Amount);
                double payment = ListPaymentData.Sum(x => x.Total);
                Tools.KeyboardPaymentPage pm = new Tools.KeyboardPaymentPage();
                pm.Setdata(MyPage, amount, payment);
                await Navigation.PushModalAsync(pm);
                KeyboardPaymentMessage();
            }
            catch { }
            IsEdit = false;
        }
        private async void BtnAddPayment_Clicked(object sender, EventArgs e)
        {
            if (IsEdit || ActiveData.Transtatus > 0) { return; }
            IsEdit = true;
            try {
                double amount = ListDetails.Sum(x => x.Amount);
                double payment = ListPaymentData.Sum(x => x.Total);
                Tools.KeyboardPaymentPage pm = new Tools.KeyboardPaymentPage();
                pm.Setdata(MyPage, amount, payment);
                await Navigation.PushModalAsync (pm);
                KeyboardPaymentMessage();
            } catch { }
            IsEdit = false;
        }
        private async void ListPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsEdit || ActiveData.Transtatus > 0) { return; }
            IsEdit = true;
            if (await DisplayAlert("แจ้งเตือน","คุณต้องการลบรายการชำระเงินนี้หรือไม่","ลบรายการชำระเงิน","ยกเลิก"))
            {
                Models.PaymentData item = (Models.PaymentData)e.CurrentSelection.FirstOrDefault();
                Models.PaymentData removedata = ListPaymentData.Find(x => x.Paymentid.Equals(item.Paymentid));
                if (removedata != null) { ListPaymentData.Remove(removedata); }
                int index = 1;
                foreach (var dr in ListPaymentData)
                {
                    dr.Item = index;
                    dr.Paymentid = dr.Visitid+"-"+dr.Item.ToString();
                }
            }
            ListPayment.SelectedItem = null;
            IsEdit = false;
            ShowPaymentData();
        }
        void KeyboardPaymentMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.KeyboardPaymentPage, Models.PaymentData>(this, MyPage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Unsubscribe<Tools.KeyboardPaymentPage, Models.PaymentData>(this, MyPage);
                        try { if (arg != null) { SetPaymentData(arg); } }
                        catch { }
                        IsEdit = false;
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("Set MessagingCenter Error", ex.Message, "OK"); }
        }
        void SetPaymentData(Models.PaymentData data)
        {
            
            if (data.Item==0) {
                data.Visitid = ActiveData.Key;
                data.Item = ListPaymentData.Count+1;
                data.Paymentid = ActiveData.Key + "-" +data.Item.ToString();
                ListPaymentData.Add(data);
            }
            else {
                var oldpayment = ListPaymentData.Find(x => x.Paymentid.Equals(data.Paymentid));
                 oldpayment = data;
            }
            ShowPaymentData();
        }
        void ShowPaymentData()
        {
            double amount = ListDetails.Sum(x => x.Amount);
            double payment = ListPaymentData.Sum(x => x.Total);
            lblTotalAmount.Text = amount.ToString("0 บาท");
            lblTotalPayment.Text = payment.ToString("0 บาท");
            LblTotalpayment.Text = payment.ToString("0 บาท");
            ListPaymentData = ListPaymentData.OrderBy(x=>x.Item).ToList();
            ListPayment.ItemsSource = null;
            ListPayment.ItemsSource = ListPaymentData;
        }

        private async void btnQRPayment_Clicked(object sender, EventArgs e)
        {
            if (ActiveData.Transtatus > 0) { return; }
            Stk_QRPayment.IsVisible = true;

            try
            {
                if (IsEdit) { return; }
                AidWaitingRun(true, "ค้นหาข้อมูล QR Payment");
                IsEdit = true;
                ImgQRCode.Source = await Helpers.Controls.GetQRPayment();
                Stk_QRPayment.IsVisible = true;
            }
            catch { }
            AidWaitingRun(false);
            IsEdit = false;
        }

        private void BtnCancelQR_Clicked(object sender, EventArgs e)
        {
            Stk_QRPayment.IsVisible = false;
        }

        private void BtnConfirmQR_Clicked(object sender, EventArgs e)
        {
            double amount = ListDetails.Sum(x => x.Amount);
            double payment = ListPaymentData.Sum(x => x.Total);
            double total = amount - payment;
            Models.PaymentData qrpayment = new Models.PaymentData()
            {
                Item = 0,
                paytypeid=4,
                Paytypename="QR Payment",
                Total = total,
            };
            SetPaymentData(qrpayment);
            Stk_QRPayment.IsVisible = false;
        }


        #endregion

        #region Save Data
        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { ListData.SelectedItem = null; return; }
            IsEdit = true;
            try {
                if (ListDetails.Count > 0)
                {
                    if (ActiveData.Transtatus > 0) { PrintSale(); }
                    else { SaveCashSale(); }

                }
                else
                {
                    await DisplayAlert("แจ้งเตือน", "ไม่มีข้อมูลการขายสินค้าเงินสด ", "ยกเลิก");
                }
            } catch { }
            ListData.SelectedItem = null;
            AidWaitingRun(false);
            IsEdit = false;
        }
        async void SaveCashSale()
        {
            try
            {

                // ========== ตรวจสอบรายการขายสินค้าเงินสด =========
                if (ListDetails == null || ListDetails.Count == 0)
                {
                    await DisplayAlert("แจ้งเตือน", "ไม่มีข้อมูลรายการขายสินค้าเงินสด", "ยกเลิก");
                    AidWaitingRun(false, "");
                    return;
                }

                // ========== ตรวจสอบเอกสารขายสินค้าเงินสด =========
                if (!await SetSaleData())
                {
                    await DisplayAlert("แจ้งเตือน", "ไม่สามารถสร้างเอกสารขายสินค้าเงินสด", "ยกเลิก");
                    AidWaitingRun(false, "");
                    return;
                }

                // ========== ตรวจสอบรายการชำระเงิน =========
                double amount = ActiveData.Amount;
                double payment = 0;
                if (ListPaymentData != null && ListPaymentData.Count > 0) { payment = ListPaymentData.Sum(x => x.Total); }
                if (payment < amount)
                {
                    await DisplayAlert("แจ้งเตือน", "กรุณาบันทึกรายการชำระเงิน", "ยกเลิก");
                    AidWaitingRun(false, "");
                    return;
                }
                string msg = "ส่งข้อมูลการขายสินค้าเงินสด...";
                AidWaitingRun(true, msg);
                string err = "";
                double i = 0;
                // Update Cash Sale line to Server
                foreach (var dr in ListDetails)
                {
                    dr.Modified = App.Servertime;
                    dr.Transtatus = 1;
                    msg = "ส่งข้อมูลรายการขายสินค้าเงินสด \n" + dr.Productname + "\n จำนวน " + dr.Qty.ToString() + " " + dr.Unitname;
                    double prgvalue = i / ListDetails.Count;
                    AidWaitingRun(true, msg, prgvalue);
                    err = await App.Ws.SaveCashSaleline(dr);
                    if (!err.Equals(""))
                    {
                        await DisplayAlert("แจ้งเตือน", "ส่งข้อมูลรายการขายสินค้าเงินสด \n" + dr.Productname + "\n จำนวน " + dr.Qty.ToString() + " " + dr.Unitname + " ไม่สำเร็จ\n" + err, "ตกลง");
                        AidWaitingRun(false, "");
                        return;
                    }
                    i += 1;
                }

                msg = "ส่งข้อมูลเอกสารการขายสินค้าเงินสด...";
                AidWaitingRun(true, msg);
                ActiveData.Transtatus = 1;
                err = await App.Ws.SaveCashSale(ActiveData);
                if (!err.Equals(""))
                {
                    await DisplayAlert("แจ้งเตือน", "ส่งข้อมูลเอกสารการขายสินค้าเงินสดไม่สำเร็จ\n" + err, "ตกลง");
                    AidWaitingRun(false, "");
                    ActiveData.Transtatus = 0;
                    return;
                }


                msg = "ส่งข้อมูลรายการชำระเงิน...";
                AidWaitingRun(true, msg);
                i = 0;
                foreach (var dr in ListPaymentData)
                {
                    dr.Transtatus = 1;
                    err = await App.Ws.SaveCashSalePayment(dr);
                    double prgvalue = i / ListDetails.Count;
                    AidWaitingRun(true, msg, prgvalue);
                    if (!err.Equals(""))
                    {
                        await DisplayAlert("แจ้งเตือน", "ส่งข้อมูลรายการชำระเงินไม่สำเร็จ\n" + err, "ตกลง");
                        AidWaitingRun(false, "");
                        return;
                    }
                    i += 1;
                }
                ActiveVisit.Transtatus = 4;

                msg = "ส่งข้อมูลจบการขายเงินสด...";
                AidWaitingRun(true, msg); await App.Ws.SaveVisit(ActiveVisit);
                MessagingCenter.Send<CashsalePage, Models.CashSaleData>(this, OwnerPage, ActiveData);
                ShowHeader();
            }
            catch (Exception ex) { await DisplayAlert("Save Cash Sale Error", ex.Message, "OK"); }
            AidWaitingRun(false, "");
        }
        async Task<bool> SetSaleData()
        {
            try {
                if (ListDetails != null && ListDetails.Count > 0)
                {
                    double amount = ListDetails.Sum(x => x.Amount);
                    double discount = ListDetails.Sum(x => x.Discount);
                    double total = amount - discount;
                    double beforvat = total / 1.07;
                    double vat = total - beforvat;
                    ActiveData.Totalline = ListDetails.Count;
                    ActiveData.Amount = amount;
                    ActiveData.Discount = discount;
                    ActiveData.Vat = vat;
                    ActiveData.Total = total;
                    //ActiveData.Transtatus = 1;
                    ActiveData.Modified = App.Servertime;
                    return await Task.FromResult(true);
                }
            }
            catch { }
             return await Task.FromResult(false);
        }

        #endregion

        #region Show Data

        public async void ShowData(string ownerpage, Models.CashSaleData data)
        {
            try
            {
                AidWaitingRun(true);
                OwnerPage = ownerpage;
                ActiveData = data;
                LblCustname.Text = ActiveData.Custname;
                ImgCustomer.Source = ActiveData.Icon;
                LblAddress.Text = ActiveData.Custaddress;
                ListDetails = await App.Ws.GetCashsaleLine(ActiveData.Key);
                ListPaymentData = await App.Ws.GetCashsalePayment(ActiveData.Key);
                ShowHeader();
                ShowPaymentData();
            }
            catch { }
            AidWaitingRun(false);
        }

        #endregion



        async void PrintSale()
        {
            try
            {
                Models.BillCompany company = App.dbmng.sqlite.Table<Models.BillCompany>().Where(x => x.Transtatus == 0).FirstOrDefault();
                if (company == null) { company = new Models.BillCompany(); }

                var pdf = new Helpers.PrintPDF();
                await pdf.PrintCashSale(company,ActiveData, ListDetails, ListPaymentData);
            }
            catch { }


          AidWaitingRun(false, "");
        }

        private async void BtnExit_Clicked(object sender, EventArgs e)
        {
            if (ActiveData.Transtatus == 0)
            {
                if (!await DisplayAlert("แจ้งเตือน", "รายการขายเงินสดยังไม่ได้บันทึก \n ต้องการออกจากหน้านี้โดยไม่บันทึกใช่หรือไม่", "ออกโดยไม่บันทึก", "ไม่ออก"))
                { return; }
            }
            GotoOwnerPage();
        }
        async void GotoOwnerPage()
        {
            if (ActiveVisit!=null && ActiveVisit.Transtatus == 14)
            {
                ActiveVisit.Transtatus = 24;
                await App.Ws.SaveVisit(ActiveVisit);
            }
            MessagingCenter.Send<CashsalePage, Models.CashSaleData>(this, OwnerPage, null);
            await Navigation.PopModalAsync();
        }
        void AidWaitingRun(bool running, string msg = "", double val=0)
        {
            try
            {
                if (running && msg.Equals("")) { msg = "กำลังตรวจสอบข้อมูล..."; }
                LblStatus.Text = msg;
                Stk_AidWaitingBk.IsVisible = running;
                Stk_AidWaiting.IsVisible = running;
                Prgvalue.Progress = val;
                AidWaiting.IsVisible = running;
                AidWaiting.IsRunning = running;
            }
            catch (Exception ex) { DisplayAlert("AidWaitingRun Error", ex.Message, "OK"); }
        }
        protected override bool OnBackButtonPressed()
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DependencyService.Get<Helpers.ICallService>().BntMoveToBack();
                });
            }
            catch (Exception ex) { DisplayAlert("OnBackButtonPressed Error", ex.Message, "OK"); }
            return true;
        }

        
    }
}