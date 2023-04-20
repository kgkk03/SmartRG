using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Picking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
   
    public partial class PickingPage : ContentPage
    {
        string OwnerPage = "PickingPage";
        string Mypage = "PickingPage";
        Models.PickingData ActiveData = new Models.PickingData();
        List<Models.PickingLineData> ListPicking = new List<Models.PickingLineData>();
        List<Models.PickingLineData> ListProduct = new List<Models.PickingLineData>();
        Models.CustomerData Customer = new Models.CustomerData();
        bool IsEdit = false;
        public PickingPage()
        {
            InitializeComponent();
        }

        void ShowHeader()
        {
            if (ListPicking != null && ListPicking.Count > 0)
            {
                ActiveData.Totalline = ListPicking.Count;
                ActiveData.Totalunit = ListPicking.Sum(x => x.Qty);
                double amount = ListPicking.Sum(x =>(x.Price * x.Qty));
                string detail = "จำนวน " + ActiveData.Totalline + " รายการ";
                detail += ", เบิก " + ListPicking.Count .ToString() + " ขวด";
                detail += ", รวมเป็นเงิน " + amount.ToString("#,##0.00") + " บาท";
                LblPickingSumary.Text = detail;
                BtnSend.IsVisible = (ActiveData.Transtatus == -1);
                BtnAdd.IsVisible = (ActiveData.Transtatus == -1);
                ListData.ItemsSource = null;
                ListData.ItemsSource = ListPicking;
            }
        }
        public async void ShowData(string ownerpage, Models.PickingData data)
        {
            try
            {
                AidWaitingRun(true);
                OwnerPage = ownerpage;
                ActiveData = data;
                if (ActiveData != null)
                {
                    ActiveData = data;
                    LblCustname.Text = ActiveData.Agentname;
                    ImgCustomer.Source = ActiveData.Icon;
                    LblAddress.Text = ActiveData.Agentid;
                    LblPickingSumary.Text = "";
                    ListPicking = await App.Ws.GetPickingDetail(ActiveData.Key);
                    ListData.ItemsSource = ListPicking;
                }
                LblPickingdate.Text = Helpers.Controls.Date2ThaiString(ActiveData.Modified, "d-MMMM-yyyy");
                ShowHeader();
            }
            catch { }
            AidWaitingRun(false);
        }

        #region New Picking
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
                    LblPickingSumary.Text = "";
                    ActiveData = GetNewData(Customer);
                    ListProduct = await GetListProduct();
                    ListData.ItemsSource = ListPicking;
                    LblPickingdate.Text = Helpers.Controls.Date2ThaiString(ActiveData.Modified, "d-MMMM-yyyy");
                }
                LblPickingSumary.Text = "จำนวน 0 รายการ, เบิก 0 ขวด, รวมเป็นเงิน 0.00 บาท";
                ShowHeader();
            }
            catch { }
            AidWaitingRun(false);
        }
        Models.PickingData GetNewData(Models.CustomerData customer)
        {
            try
            {
                DateTime req = App.Servertime.AddDays(2);
                var result = new Models.PickingData()
                {
                    Visitid = Helpers.Controls.GetID() + App.UserProfile.Empid.ToString().PadLeft(3, '0'),
                    Agentcode = customer.Custcode,
                    Agentid = customer.Key,
                    Agentname = customer.Custname,
                    Empfullname=App.UserProfile.Fullname,
                    Empid = App.UserProfile.Empid,
                    Pickingdate = App.Servertime,
                    Pickingtype = "เบิก",
                    Typeid = 1,
                };
                result.Key = result.Visitid + "-1";
                return result;
            }
            catch (Exception ex) { DisplayAlert("Set New SO Error", ex.Message, "OK"); }
            return null;
        }
        async Task<Models.PickingLineData> GetNewDetaildata(Models.ProductData dr)
        {
            try
            {
                Models.PickingLineData result = new Models.PickingLineData()
                {
                    Pickingid = ActiveData.Key,
                    Pickingtype = "เบิก",
                    Typeid = 1,
                    Item = 0,
                    Productid = dr.Productid,
                    Productcode = dr.Productcode,
                    Productname = dr.Productname,
                    Unitname = dr.Unitname,
                    Sizename = dr.Size,
                    Price = dr.Price,
                    Agentid = ActiveData.Agentid,
                    Agentname = ActiveData.Agentname,
                    Custid = App.UserProfile.Empid.ToString(),
                    Custname = App.UserProfile.Empname + " " + App.UserProfile.Empsurname
                };
                return await Task.FromResult(result);
            }
            catch (Exception ex) { await DisplayAlert("PickingPage GetNewDetaildata Error", ex.Message, "OK");  }
            return null;
        }
        async Task<List<Models.PickingLineData>> GetListProduct()
        {
            try
            {
                List<Models.PickingLineData> result = new List<Models.PickingLineData>();
                string agenid = Customer.Key;
                var stock = await App.Ws.GetPickingStock(agenid);
                List<Models.ProductData> prd = await App.Ws.GetSaleProduct(Customer.Custgroupid, Customer.Key);
                if (prd != null && prd.Count > 0)
                {
                    foreach (var dr in prd)
                    {
                        var temp = await GetNewDetaildata(dr);
                        if (stock != null && stock.Count > 0)
                        {
                            Models.PickingLineData laststock = stock.Find(x => x.Productid.Equals(dr.Productid));
                            if (laststock != null) { temp.Stock = laststock.Balance; }
                        }
                        result.Add(temp);
                    }
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex) { await DisplayAlert("VisitAgentPickingPage GetListProduct Error", ex.Message, "OK"); return null; }
        }

        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                IsEdit = true;
                var kb = new Tools.KeyboardPickingPage();
                if (ListProduct == null) { ListProduct = await GetListProduct(); }
                kb.Setproduct(Mypage, ListProduct);
                await Navigation.PushModalAsync(kb);
                KeyboardPickingPageMessage();
            }
            catch (Exception ex) { await DisplayAlert("VisitAgentPickingPage btnAdddata_Clicked Error", ex.Message, "OK"); }
        }
        void KeyboardPickingPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.KeyboardPickingPage, Models.PickingLineData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try
                        {
                            IsEdit = false; AidWaitingRun(false);
                            UpdateDetailData(arg);
                        }
                        catch { }
                        MessagingCenter.Unsubscribe<Tools.KeyboardSOPage, Models.SOlineData>(this, Mypage);
                    });
                });

            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        private void UpdateDetailData(Models.PickingLineData data)
        {
            try
            {
                if (data != null)
                {
                    var prd = ListPicking.Find(x => x.Productid.Equals(data.Productid));
                    if (prd == null)
                    {
                        if (data.Qty > 0)
                        {
                            data.Item = ListPicking.Count + 1;
                            data.Key = (data.Pickingid + "-" + data.Item.ToString());
                            ListPicking.Add(data);
                            //App.dbmng.InsetData(data);
                        }
                    }
                    else
                    {
                        if (data.Qty == 0) { ListPicking.Remove(prd); }
                        else
                        {
                            prd.Qty = data.Qty;
                            prd.Total =  data.Total;
                            prd.Balance = data.Balance;
                            prd.Modified = App.Servertime;
                            prd.Amount = data.Qty * data.Price;
                        }
                        //App.dbmng.InsetData(prd);
                    }
                    ShowHeader();
                }
            }
            catch (Exception ex) { DisplayAlert("PickingPage SetPickingData Error", ex.Message, "OK"); }
            IsEdit = false;
        }
        private async void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Models.PickingLineData item = (Models.PickingLineData)e.CurrentSelection.FirstOrDefault();
            ListData.SelectedItem = null;
            if (item == null) { return; }
            if (IsEdit || ActiveData.Transtatus != -1) { return; }
            try
            {
                IsEdit = true;
                if (ListProduct == null) { ListProduct = await GetListProduct(); }
                int index = ListProduct.FindIndex(x => x.Productid.Equals(item.Productid));
                if (index < 0) { index = 0; }
                var kb = new Tools.KeyboardPickingPage();
                kb.Setproduct(Mypage, ListProduct, index);
                await Navigation.PushModalAsync(kb);
                KeyboardPickingPageMessage();
            }
            catch { IsEdit = false; }
        }

        #endregion




        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            // Save Data
            if (IsEdit || ActiveData.Transtatus != -1) { return; }
            IsEdit = true;
            AidWaitingRun(true, "กำลังส่งข้อมูลการเบิกสินค้า...");
            int success = 0;
            bool error = false;
            string msg = "ข้อมูลที่ส่งไปแล้วจะไม่สามารถแก้ไขได้\n" + "คุณต้องการกรอกข้อมูลต่อหรือส่งรายการเบิกสินค้า";
            if (await DisplayAlert("แจ้งเตือน", msg, "ส่งรายการเบิกสินค้า", "ยังไม่ส่ง"))
            {
                foreach (var dr in ListPicking)
                {
                    AidWaitingRun(true, "กำลังส่งข้อมูลรายการเบิกสินค้า...\n" + dr.Productname);
                    msg = await App.Ws.SavePickingline(dr);
                    if (msg.Equals("")) { success += 1; }
                    else { error = true; }
                }

                if (error)
                {
                    msg = "ไม่สามารถส่งข้อมูลรายการเบิกสินค้าได้จำนวน " + (ListPicking.Count - success).ToString()
                        + "รายการ\nโปรดตรวจสอบอินเตอร์เน็ตแล้วส่งอีกครั้ง";
                    await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                    IsEdit = false;
                    AidWaitingRun(false);
                    return;
                }
                else
                {
                    ActiveData.Transtatus = 0;
                    ActiveData.Showtime = Helpers.Controls.Date2String(App.Servertime, "HH:mm");
                    msg = await App.Ws.SavePicking(ActiveData);
                    if (!msg.Equals(""))
                    {
                        ActiveData.Transtatus = -1;
                        msg = "ไม่สามารถส่งข้อมูลสรุปการสั่งขายสินค้าได้\nโปรดตรวจสอบอินเตอร์เน็ตแล้วส่งอีกครั้ง";
                        await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                        IsEdit = false;
                        AidWaitingRun(false);
                        return;
                    }
                    else
                    {
                        await DisplayAlert("ส่งข้อมูลเรียบร้อย", msg, "ตกลง");
                        ActiveData.Transtatus = -1;
                        GotoOwnerPage(true);
                        AidWaitingRun(false);
                        IsEdit = false;
                    }
                }
            }
            IsEdit = false;
            AidWaitingRun(false);
        }
        private async void BtnExit_Clicked(object sender, EventArgs e)
        {
            if (ActiveData.Transtatus == -1)
            {
                if (!await DisplayAlert("แจ้งเตือน", "รายการเบิกสินค้ายังไม่ได้บันทึก \n ต้องการออกจากหน้านี้โดยไม่บันทึกใช่หรือไม่", "ออกโดยไม่บันทึก", "ไม่ออก"))
                { return; }
            }
            GotoOwnerPage(false);
        }
        async void GotoOwnerPage(bool success = false)
        {
            Models.PickingData result = null;
            if (ActiveData.Transtatus == -1)
            {
                if (success) { ActiveData.Transtatus = 0;  result = ActiveData; }
                MessagingCenter.Send<PickingPage, Models.PickingData>(this, OwnerPage, result);
            }
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