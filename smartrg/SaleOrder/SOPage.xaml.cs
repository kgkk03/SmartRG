using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.SaleOrder
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SOPage : ContentPage
    {
        string OwnerPage = "SOPage";
        string Mypage = "SOPage";
        Models.SaleorderData ActiveSO = new Models.SaleorderData();
        Models.VisitData ActiveVisit = null;
        List<Models.SOlineData> ListSO = new List<Models.SOlineData>();
        List<Models.ProductData> ListProduct = null;
        Tools.ResuleCalandra SelectPeriod = null;
        bool IsEdit = false;
        public SOPage()
        {
            InitializeComponent();
        }

        void ShowHeader()
        {
            if(ActiveSO != null)
            {
                BtnSend.IsVisible = (ActiveSO.Transtatus == 0);
                BtnAdd.IsVisible = (ActiveSO.Transtatus == 0);
                LblRequest.Text = Helpers.Controls.Date2ThaiString(ActiveSO.Requestdate, "dd-MMM-yyyy");
                LblSodate.Text = Helpers.Controls.Date2ThaiString(ActiveSO.Modified, "dd-MMM-yyyy");
                if (ListSO != null && ListSO.Count > 0)
                {
                    ActiveSO.Totalline = ListSO.Count;
                    ActiveSO.Amount = ListSO.Sum(x => x.Amount);
                    ActiveSO.Vat = (ActiveSO.Amount * 0.07);
                    ActiveSO.Total = (ActiveSO.Amount + ActiveSO.Vat);
                    int sale = ListSO.Where(x => x.Amount > 0).Sum(x => x.Qty);
                    int free = ListSO.Where(x => x.Amount == 0).Sum(x => x.Qty);
                    string detail = "จำนวน " + ActiveSO.Totalline + "รายการ";
                    detail += ", ขาย " + sale.ToString() + "ขวด";
                    detail += ", แถม " + free.ToString() + "ขวด";
                    detail += ", รวม " + ActiveSO.Total.ToString("#,##0.00") + "บาท";
                    LblSOSumary.Text = detail;

                }
            }
            
        }
        public async void ShowSO(string ownerpage, Models.SaleorderData sodata)
        {
            try
            {
                AidWaitingRun(true);
                OwnerPage = ownerpage;
                ActiveSO = sodata;
                if (ActiveSO != null)
                {
                    //Activedata = data;
                    LblCustname.Text = ActiveSO.Custname;
                    ImgCustomer.Source = ActiveSO.Icon;
                    LblAddress.Text = ActiveSO.CustAddress;
                    LblSOSumary.Text = "";
                    ListSO = await App.Ws.GetSOline(ActiveSO.Key);
                    ListData.ItemsSource = ListSO;
                }
                LblRequest.Text = Helpers.Controls.Date2ThaiString(ActiveSO.Requestdate, "d-MMMM-yyyy");
                LblSodate.Text = Helpers.Controls.Date2ThaiString(ActiveSO.Modified, "d-MMMM-yyyy");
                ShowHeader();
            }
            catch { }
            AidWaitingRun(false);
        }

        #region New Sale Order
        public async void SetNewSO(string ownerpage, Models.CustomerData Customer)
        {
            try {
                AidWaitingRun(true);
                OwnerPage = ownerpage;
                if (Customer != null)
                {
                    //Activedata = data;
                    LblCustname.Text = Customer.Custname + " (" + Customer.Custgroupname + ")";
                    ImgCustomer.Source = Customer.Icon;
                    LblAddress.Text = Customer.Custaddress;
                    LblSOSumary.Text = "";
                    ActiveVisit = await Helpers.Controls.GetVisitdata(Customer, null);
                    ActiveVisit.Planstatus = -3;
                    ActiveVisit.Planid = "";
                    ActiveVisit.Transtatus = 15;
                    string msg = await App.Ws.SaveVisit(ActiveVisit);

                    ActiveSO = GetNewSO(Customer, ActiveVisit.Key);
                    if (ListProduct == null || ListProduct.Count == 0) { ListProduct = await App.Ws.GetSaleProduct(Customer.Custgroupid, Customer.Key); }
                    ListData.ItemsSource = ListSO;
                }
                ShowHeader();
            }
            catch { }
            AidWaitingRun(false);
        }
        Models.SaleorderData GetNewSO(Models.CustomerData customer,string visitid)
        {
            try
            {
                DateTime req = App.Servertime.AddDays(2);
                return new Models.SaleorderData()
                {
                    Key = visitid,
                    CustAddress = customer.Custaddress,
                    CustCode = customer.Custcode,
                    Custid = customer.Key,
                    Custname = customer.Custname,
                };
            }
            catch (Exception ex) { DisplayAlert("Set New SO Error", ex.Message, "OK"); }
            return null;
        }
        Models.SOlineData GetNewSOline()
        {
            try
            {
                return new Models.SOlineData() { Soid = ActiveSO.Key, Custid = ActiveSO .Custid};
            }
            catch (Exception ex) { DisplayAlert("Get New Saleorder Error", ex.Message, "OK"); }
            return new Models.SOlineData();
        }
        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                IsEdit = true;
                var so = GetNewSOline();
                Tools.KeyboardSOPage kb = new Tools.KeyboardSOPage();
                if (ListProduct != null && ListProduct.Count > 0)
                {
                    if (kb.Setdata(Mypage, so, ListProduct)) { 
                        await Navigation.PushModalAsync(kb);
                        KeyboardSOPageMessage();
                        return; 
                    }
                }
                await DisplayAlert("แจ้งเตือน", "ไม่พบข้อมูลสินค้าที่สามารถขายได้", "ตกลง");
            }
            catch (Exception ex) {
                IsEdit = false;
            }
        }
        private async void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Models.SOlineData item = (Models.SOlineData)e.CurrentSelection.FirstOrDefault();
            ListData.SelectedItem = null;
            if (item == null) { return; }
            if (IsEdit || ActiveSO.Transtatus != 0) { return; }
            try
            {
                IsEdit = true;
                Tools.KeyboardSOPage kb = new Tools.KeyboardSOPage();
                if (ListProduct != null && ListProduct.Count > 0)
                {
                    if (kb.Setdata(Mypage, item, ListProduct))
                    {
                        await Navigation.PushModalAsync(kb);
                        KeyboardSOPageMessage();
                        return;
                    }
                }
                await DisplayAlert("แจ้งเตือน", "ไม่พบข้อมูลสินค้าที่สามารถขายได้", "ตกลง");
            }
            catch { IsEdit = false; }
        }
        void KeyboardSOPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.KeyboardSOPage, Models.SOlineData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { IsEdit = false; AidWaitingRun(false); 
                            UpdateSoline(arg); 
                        } 
                        catch { }
                        MessagingCenter.Unsubscribe<Tools.KeyboardSOPage, Models.SOlineData>(this, Mypage);
                    });
                });

            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        void UpdateSoline(Models.SOlineData data)
        {
            if (data.Item == 0) {
                if (data.Qty > 0)
                {
                    // รายการสั่งขายใหม่
                    data.Item = (ListSO.Count == 0) ? 1 : ListSO.Max(x => x.Item) + 1;
                    data.Key = data.Soid + "-" + data.Item.ToString();
                    ListSO.Add(data);
                }
            }
            else {
                var olddata = ListSO.Find(x => x.Item == data.Item);
                if (data.Qty == 0) {
                    ListSO.Remove(olddata);
                }
                else {
                    olddata = data;
                }
            }
            ListData.ItemsSource = null;
            ListData.ItemsSource = ListSO;
            ShowHeader();
        }

        #endregion

     


        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            // Save Data
            if (IsEdit || ActiveSO.Transtatus!=0) { return; }
            IsEdit = true;
            AidWaitingRun(true, "กำลังส่งข้อมูลการสั่งขายสินค้า...");
            int success = 0;
            bool error = false;
            string msg = "ข้อมูลที่ส่งไปแล้วจะไม่สามารถแก้ไขได้\n" + "คุณต้องการกรอกข้อมูลต่อหรือส่งรายการสั่งขายสินค้า";
            if (await DisplayAlert("แจ้งเตือน", msg, "ส่งรายการสั่งขาย", "ยังไม่ส่ง"))
            {
                foreach (var dr in ListSO)
                {
                    AidWaitingRun(true, "กำลังส่งข้อมูลรายการสั่งขายสินค้า...\n" + dr.Productname);
                    msg = await App.Ws.SaveSOline(dr);
                    if (msg.Equals("")) { success += 1; }
                    else { error = true; }
                }

                if (error)
                {
                    msg = "ไม่สามารถส่งข้อมูลรายการสั่งขายสินค้าได้จำนวน " + (ListSO.Count - success).ToString()
                        + "รายการ\nโปรดตรวจสอบอินเตอร์เน็ตแล้วส่งอีกครั้ง";
                    await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                    IsEdit = false;
                    AidWaitingRun(false);
                    return;
                }
                else
                {
                    ActiveSO.Transtatus = 1;
                    ActiveSO.Showtime = Helpers.Controls.Date2String(App.Servertime, "HH:mm");
                    msg = await App.Ws.SaveSO(ActiveSO);
                    if (!msg.Equals("")) {
                        ActiveSO.Transtatus = 0;
                        msg = "ไม่สามารถส่งข้อมูลสรุปการสั่งขายสินค้าได้\nโปรดตรวจสอบอินเตอร์เน็ตแล้วส่งอีกครั้ง";
                        await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                        IsEdit = false;
                        AidWaitingRun(false);
                        return;
                    }
                    else {
                        ActiveVisit.Transtatus = 5;
                        await App.Ws.SaveVisit(ActiveVisit);
                        await DisplayAlert("ส่งข้อมูลเรียบร้อย", msg, "ตกลง");
                        GotoOwnerPage(true);
                        IsEdit = false;
                        AidWaitingRun(false);
                    }
                }
            }
            IsEdit = false;
            AidWaitingRun(false);
        }
        private async void BtnRequest_Clicked(object sender, EventArgs e)
        {
            if (!IsEdit && ActiveSO.Transtatus == 0)
            {
                try
                {
                    IsEdit = true;
                    AidWaitingRun(true, "กำลังเรียกดูข้อมูลปฎิทิน...");
                    var page = new Tools.CalendarSelect();
                    if (SelectPeriod == null) { SelectPeriod = GetSelectPeriod(); }
                    page.Showcalendar(SelectPeriod, Mypage);
                    await Navigation.PushModalAsync(page);
                    CalendarSelectMessage();
                }
                catch { }
            }
            IsEdit = false;
            AidWaitingRun(false);
        }
        void CalendarSelectMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.CalendarSelect, Tools.ResuleCalandra>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread( () => {
                        try
                        {
                            IsEdit = false;
                            AidWaitingRun(false);
                            if (arg != null) { SelectPeriod = arg; }
                            var mindate = App.Servertime.AddDays(2);
                            if (SelectPeriod.Start < mindate) {
                                string msg = "ไม่สามารถเลือกวันที่ต้องการรับสินค้าต่ำกว่า\n" + Helpers.Controls.Date2ThaiString(mindate,"dd-MMMM-yyyy");
                                DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                                return;
                            }
                            ActiveSO.Requestdate = SelectPeriod.Start;
                            LblRequest.Text = Helpers.Controls.Date2ThaiString(ActiveSO.Requestdate,"d-MMMM-yyyy");
                        }
                        catch { }
                        MessagingCenter.Unsubscribe<Tools.CalendarSelect, Tools.ResuleCalandra>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }

        Tools.ResuleCalandra GetSelectPeriod()
        {
            return  new Tools.ResuleCalandra()
            {
                Type = 1,
                Start = ActiveSO.Requestdate,
                End = ActiveSO.Requestdate,
                Min = Helpers.Controls.GetToday(App.Servertime.AddDays(2)),
                Max = Helpers.Controls.GetToday(App.Servertime.AddDays(30)),
            };
        }

        private async void BtnExit_Clicked(object sender, EventArgs e)
        {
            if (ActiveSO.Transtatus == 0)
            {
                if (!await DisplayAlert("", "รายการสั่งขายสินค้ายังไม่ได้บันทึก \n ต้องการออกจากหน้านี้โดยไม่บันทึกใช่หรือไม่", "ออกโดยไม่บันทึก", "ไม่ออก"))
                { return; }
            }
            GotoOwnerPage(false);
        }
        async void GotoOwnerPage(bool success)
        {
            try
            {
                if (success)
                {
                    MessagingCenter.Send<SOPage, Models.SaleorderData>(this, OwnerPage, ActiveSO);

                }
                else
                {
                    if (ActiveVisit != null && ActiveVisit.Transtatus == 15)
                    {
                        ActiveVisit.Transtatus = 25;
                        await App.Ws.SaveVisit(ActiveVisit);
                    }
                    MessagingCenter.Send<SOPage, Models.SaleorderData>(this, OwnerPage, null);

                }


                
            }
            catch { }
                await Navigation.PopModalAsync();
        }
        void AidWaitingRun(bool running, string msg = "")
        {
            try
            {
                LblStatus.Text = msg;
                Stk_AidWaitingBk.IsVisible = running;
                Stk_AidWaiting.IsVisible = running;
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