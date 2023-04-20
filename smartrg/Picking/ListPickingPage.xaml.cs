using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Picking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListPickingPage : ContentPage
    {
        bool IsEdit = false;
        string Mypage = "ListPickingPage";
        Tools.ResuleCalandra SelectPeriod = new Tools.ResuleCalandra() { Type = 1 };
        string SelectAgent = "";
        public ListPickingPage()
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

        #region ข้อมูลการเบิก-ขาย-คืนสินค้า
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
                    AidWaitingRun(true, "กำลังเรียกดูข้อมูลการเบิก-คืน...");
                    DateTime selecteddate = SelectPeriod.Start;
                    LblTodayHeader.Text = Helpers.Controls.Date2ThaiString(selecteddate, "d-MMM-yyyy");
                    selecteddate = Helpers.Controls.GetToday(selecteddate);
                    List<Models.PickingData> result = await App.Ws.GetPickingList(selecteddate, selecteddate.AddDays(1), SelectAgent);
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
                Models.PickingData item = (Models.PickingData)e.CurrentSelection.FirstOrDefault();
                ListData.SelectedItem = null;
                if (item == null) { return; }
                if (IsEdit) { return; }
                IsEdit = true;
                AidWaitingRun(true,"กำลังค้นหาข้อมูลรายการเบิก...");
                //PickingType(เบิก, ขาย, คืน)
                if (item.Pickingtype.Equals("คืน"))
                {
                    var page = new TabReturnPage();
                    page.ShowData(Mypage, item);
                    await Navigation.PushModalAsync(page);

                    //var page = new ReturnPage();
                    //page.ShowData(Mypage, item);
                    //await Navigation.PushModalAsync(page);
                }
                else if (item.Pickingtype.Equals("ขาย")){
                
                }
                else
                {
                    var page = new PickingPage();
                    page.ShowData(Mypage, item);
                    await Navigation.PushModalAsync(page);
                }
               
            }
            catch { }
            IsEdit = false;
            AidWaitingRun(false);
        }
        #endregion

        #region เบิกสินค้า 

        private async void BtnPicking_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                IsEdit = true;
                stkFillterOption.IsVisible = false;
                AidWaitingRun(true, "กำลังค้นหาข้อมูลลูกค้า...");
                var page = new Customer.GetCustomerPage();
                List<Models.CustomerFillter> filters = await GetPickingCustgroup();
                if (filters != null)
                {
                    page.Setdata(Mypage, false, filters);
                    await Navigation.PushModalAsync(page);
                    GetCustomerPageMessage();
                    return;
                }
                else
                {
                    await DisplayAlert("แจ้งเตือน", "ไม่สามารถค้นหาข้อมูลตัวแทนจำหน่ายได้", "ตกลง");
                    AidWaitingRun(false);
                }
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
                            CreatePicking(arg);
                        }
                        catch { }
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        async Task<List<Models.CustomerFillter>> GetPickingCustgroup()
        {
            try
            {
                //var result = App.dbmng.sqlite.Table<Models.CustomerFillter>().Where(x => x.Type == 3).OrderBy(x => x.Piority).ThenBy(x => x.Display).ToList();
                //var result = App.dbmng.sqlite.Table<Models.CustomerFillter>().ToList();
                var result = App.dbmng.sqlite.Table<Models.CustomerFillter>().Where(x => x.Isstore).OrderBy(x => x.Piority).ThenBy(x => x.Display).ToList();
                if (result != null && result.Count > 0) { return await Task.FromResult(result); }
            }
            catch { }
            return null;
        }
        private async void CreatePicking(Models.CustomerData cust)
        {
            try
            {
                if (cust != null)
                {
                    var page = new PickingPage();
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
                MessagingCenter.Subscribe<PickingPage, Models.PickingData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try
                        {
                            MessagingCenter.Unsubscribe<PickingPage, Models.PickingData>(this, Mypage);
                            if (arg != null) { ShowToday(); }
                        }
                        catch { }
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }

        #endregion

        #region คืนสินค้า
        private async void BtnReturn_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            IsEdit = true;
            AidWaitingRun(true, "กำลังค้นหาตัวแทนจำหน่าย...");
            stkFillterOption.IsVisible = false;
            SelectAgent = "";
            if( await GetListPickingAgent()) { Pikfilter.Focus(); }
            else { IsEdit = false; StkFilter.IsVisible = false; }
            AidWaitingRun(false);
        }
        async Task<bool> GetListPickingAgent()
        {
            try
            {
                List<Models.CustomerData> result = await App.Ws.GetAgentPicking();
                if (result != null && result.Count > 0)
                {
                    Pikfilter.ItemsSource = result;
                    Pikfilter.ItemDisplayBinding = new Binding("Custname");
                    StkFilter.IsVisible = true;
                    return true;
                }
                else
                {
                    await DisplayAlert("แจ้งเตือน","ไม่มีรายการสินค้าที่ต้องคืนเหลืออยู่","ตกลง");
                }
            }
            catch { }
            return false;

        }
        private void BtnFilterclose_Clicked(object sender, EventArgs e)
        {
            StkFilter.IsVisible = false;
            IsEdit = false;
        }
        private void Pikfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Models.CustomerData item = (Models.CustomerData)Pikfilter.SelectedItem;
            if (item != null)
            {
                Pikfilter.SelectedItem = null;
                StkFilter.IsVisible = false;
                SelectAgent = item.Key;
                ShowReturn(item);
            }
        }
        private async void ShowReturn(Models.CustomerData customer)
        {
            AidWaitingRun(true, "กำลังค้นหาข้อมูลสินค้าคงเหลือ...");
            if (customer != null)
            {
                var page = new TabReturnPage();
                page.SetNewData(Mypage, customer);
                await Navigation.PushModalAsync(page);
                TabReturnPageMessage();
            }
            else
            {
                await DisplayAlert("แจ้งเตือน", "กรุณาเลือกตัวแทนจำหน่ายที่รับคืนสินค้า", "ตกลง");
            }
            stkFillterOption.IsVisible = false;
            AidWaitingRun(false);
            IsEdit = false;
        }
        void TabReturnPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<TabReturnPage, Models.PickingData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try
                        {
                            MessagingCenter.Unsubscribe<TabReturnPage, Models.PickingData>(this, Mypage);
                            if (arg != null) { ShowToday(); }
                        }
                        catch { }
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        #endregion

        #region ข้อมูลสินค้าคงเหลือ
        private async void BtnPickStock_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                IsEdit = true;
                stkFillterOption.IsVisible = false;
                AidWaitingRun(true, "กำลังค้นหาข้อมูลสินค้าคงเหลือ...");
                List<Models.PickingLineData> stockproduct = await App.Ws.GetPickingStock("");
                if (stockproduct != null)
                {
                    var page = new PickingStockPage();
                    page.SetData( stockproduct);
                    await Navigation.PushModalAsync(page);
                }
            }
            catch { }
            AidWaitingRun(false);
            IsEdit = false;
        }

        #endregion


        #region เมนู & Option Select
        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<ListPickingPage, bool>(this, "OpenMenu", true);
        }
        private void BtnFillterOption_Clicked(object sender, EventArgs e)
        {
            stkFillterOption.IsVisible = !stkFillterOption.IsVisible;
        }
      
        #endregion


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