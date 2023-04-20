using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace smartrg.CashSale
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListCashsalePage : ContentPage
    {
        bool IsEdit = false;
        string Mypage = "ListCashsalePage";
        Tools.ResuleCalandra SelectPeriod = new Tools.ResuleCalandra() { Type = 1 };
        public ListCashsalePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            ShowToday();
        }
        private async void ShowToday()
        {
            SelectPeriod = new Tools.ResuleCalandra() { Type = 1 };
            await Showdata();
        }

        #region ประวัติการขายสินค้าเงินสด
        private async void BtnSelectnext_Clicked(object sender, EventArgs e)
        {
            SelectPeriod.Start = SelectPeriod.Start.AddDays(1);
            SelectPeriod.End = SelectPeriod.Start;
            await Showdata();
        }
        private async void BtnSelectback_Clicked(object sender, EventArgs e)
        {
            SelectPeriod.Start = SelectPeriod.Start.AddDays(-1);
            SelectPeriod.End = SelectPeriod.Start;
            await Showdata();
        }
        private async void BtnSelectDate_Clicked(object sender, EventArgs e)
        {
            if (!IsEdit)
            {
                try
                {
                    IsEdit = true;
                    AidWaitingRun(true, "กำลังเรียกดูข้อมูลปฎิทิน...");
                    var page = new Tools.CalendarSelect();
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
                    Device.BeginInvokeOnMainThread(async () => {
                        try
                        {
                            IsEdit = false;
                            AidWaitingRun(false);
                            if (arg != null) { SelectPeriod = arg; }
                            else { SelectPeriod = new Tools.ResuleCalandra() { Type = 1 }; }
                            await Showdata();
                        }
                        catch { }
                        MessagingCenter.Unsubscribe<Tools.CalendarSelect, Tools.ResuleCalandra>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("ListPickingPage CalendarSelectMessage  Error", ex.Message, "OK"); }
        }
        async Task<bool> Showdata()
        {
            if (!IsEdit)
            {
                try
                {
                    IsEdit = true;
                    AidWaitingRun(true, "กำลังเรียกดูข้อมูลการสั่งขาย...");
                    DateTime selecteddate = SelectPeriod.Start;
                    LblTodayHeader.Text = Helpers.Controls.Date2ThaiString(selecteddate, "d-MMM-yyyy");
                    selecteddate = Helpers.Controls.GetToday(selecteddate);
                    List<Models.CashSaleData> result = await App.Ws.GetCashsaleList(selecteddate, selecteddate.AddDays(1));
                    StkNodata.IsVisible = (result == null || result.Count == 0);
                    ListData.ItemsSource = result;
                }
                catch { }
            }
            IsEdit = false;
            AidWaitingRun(false);
            return await Task.FromResult(true);
        }
        private async void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Models.CashSaleData item = (Models.CashSaleData)e.CurrentSelection.FirstOrDefault();
                ListData.SelectedItem = null;
                if (item == null) { return; }
                if (IsEdit) { return; }
                IsEdit = true;
                AidWaitingRun(true, "กำลังค้นหาข้อมูลรายการขายเงินสด...");
                var page = new CashsalePage();
                page.ShowData(Mypage, item);
                await Navigation.PushModalAsync(page);

            }
            catch { }
            IsEdit = false;
            AidWaitingRun(false);
        }
        #endregion

        #region เปิดรายการขายสินค้าเงินสด 

        private async void BtnNewCashsale_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                IsEdit = true;
                AidWaitingRun(true, "กำลังค้นหาข้อมูลสินค้าคงเหลือ...");
                var productinstock = await App.Ws.GetPickingStock("");
                if(productinstock==null || productinstock.Count == 0)
                {
                    await DisplayAlert("แจ้งเตือน", "ไม่มีสินค้าให้จำหน่าย กรุณาเบิกสินค้า", "OK");
                    IsEdit = false;
                    AidWaitingRun(false);
                    return;
                }
                AidWaitingRun(true, "กำลังค้นหาข้อมูลลูกค้า...");
                var page = new Customer.GetCustomerPage();
                page.Setdata(Mypage, true);
                await Navigation.PushModalAsync(page);
                GetCustomerPageMessage();
                return;
            }
            catch { }
            IsEdit = false;
        }
        void GetCustomerPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Customer.GetCustomerPage, Models.CustomerData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try
                        {
                            MessagingCenter.Unsubscribe<Customer.GetCustomerPage, Models.CustomerData>(this, Mypage);
                            CreateCashsale(arg);
                        }
                        catch { }
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
       
        private async void CreateCashsale(Models.CustomerData cust)
        {
            try
            {
                if (cust != null)
                {
                    var page = new CashsalePage();
                    page.SetNewData(Mypage, cust);
                    await Navigation.PushModalAsync(page);
                    PickingPageMessage();
                }
            }
            catch { }
            AidWaitingRun(false);
            IsEdit = false;
            return;
        }
        void PickingPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<CashsalePage, Models.CashSaleData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try
                        {
                            MessagingCenter.Unsubscribe<CashsalePage, Models.CashSaleData>(this, Mypage);
                            if (arg != null) { ShowToday(); }
                        }
                        catch { }
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }

        #endregion

        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<ListCashsalePage, bool>(this, "OpenMenu", true);
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