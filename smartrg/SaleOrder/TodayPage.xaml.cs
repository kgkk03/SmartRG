using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.SaleOrder
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
   
    
    public partial class TodayPage : ContentPage
    {
        string Mypage = "SO.TodayPage";
        bool IsEdit = false;
        List <Models.SaleorderData> ActiveWork = new List<Models.SaleorderData>();
        Tools.ResuleCalandra SelectPeriod = new Tools.ResuleCalandra() { Type = 1 };
        public TodayPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            ShowToday();
        }
        #region Get Daialy SO

        private async void ShowToday()
        {
            await Showdata();
        }
        private async void BtnSelectnext_Clicked(object sender, EventArgs e)
        {
            SelectPeriod.Start = SelectPeriod.Start.AddDays(1);
            SelectPeriod.End = SelectPeriod.Start;
            await Showdata();
        }
        private async  void BtnSelectback_Clicked(object sender, EventArgs e)
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
                        try { 
                            IsEdit = false; 
                            AidWaitingRun(false);
                            if (arg != null) { SelectPeriod = arg; }
                            else { SelectPeriod = new Tools.ResuleCalandra() { Type=1}; }
                            await Showdata();
                        }
                        catch { }
                        MessagingCenter.Unsubscribe<Tools.CalendarSelect, Tools.ResuleCalandra>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        async Task<bool> Showdata()
        {
            if (!IsEdit) {
                try
                {
                    IsEdit = true;
                    AidWaitingRun(true,"กำลังเรียกดูข้อมูลการสั่งขาย...");
                    DateTime selecteddate = SelectPeriod.Start;
                    LblTodayHeader.Text = Helpers.Controls.Date2ThaiString(selecteddate, "d-MMM-yyyy");
                    selecteddate = Helpers.Controls.GetToday(selecteddate);
                    ActiveWork = await App.Ws.GetSOList(selecteddate, selecteddate.AddDays(1));
                    StkNodata.IsVisible = (ActiveWork == null || ActiveWork.Count == 0);
                    ListData.ItemsSource = ActiveWork;
                    ShowEmployee();
                }
                catch { }
            }
            IsEdit = false;
            AidWaitingRun(false);
            return await Task.FromResult(true);
        }

        async void ShowEmployee()
        {
            ImgUser.Source = await Helpers.Controls.GetProfileImage();
            LblTeamname.Text = App.UserProfile.Authen + " (" + App.UserProfile.TeamName + ")";
            LblUserID.Text = App.UserProfile.Empcode;
            LblFullname.Text = App.UserProfile.Fullname;
        }


        #endregion

        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<TodayPage, bool>(this, "OpenMenu", true);
        }
        private async void BtnCheckin_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                IsEdit = true;
                AidWaitingRun(true, "ค้นหาข้อมูลลูกค้า...");
                var page = new Customer.GetCustomerPage();
                page.Setdata(Mypage,false);
                await Navigation.PushModalAsync(page);
                GetCustomerPageMessage();
            }
            catch { IsEdit = false; AidWaitingRun(false);
            }
        }
        void GetCustomerPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe< Customer.GetCustomerPage, Models.CustomerData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try {
                            
                            IsEdit = false; AidWaitingRun(false); CreateNewSO(arg); } catch { }
                        MessagingCenter.Unsubscribe<Customer.GetCustomerPage, Models.CustomerData>(this, Mypage);
                    });
                });

            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }

        async void CreateNewSO(Models.CustomerData customer)
        {
            if (IsEdit) { return; }
            try
            {
                if (customer == null) { return; }
                IsEdit = true;
                AidWaitingRun(true, "ค้นหาข้อมูลลูกค้า...");
                var page = new SOPage();
                page.SetNewSO(Mypage, customer);
                await Navigation.PushModalAsync(page);
                SOPageMessage();
            }
            catch
            {
                IsEdit = false; AidWaitingRun(false);
            }
        }
        void SOPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<SOPage, Models.SaleorderData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { IsEdit = false; AidWaitingRun(false); RefreshSO(arg); } catch { }
                        MessagingCenter.Unsubscribe<SOPage, Models.SaleorderData>(this, Mypage);
                    });
                });

            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }

        private async void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Models.SaleorderData item = (Models.SaleorderData)e.CurrentSelection.FirstOrDefault();
                ListData.SelectedItem = null;
                if (item == null) { return; }
                if (IsEdit) { return; }
                IsEdit = true;
                var page = new SOPage();
                page.ShowSO(Mypage, item);
                await Navigation.PushModalAsync(page);
            }
            catch { }
            IsEdit = false;
            AidWaitingRun(false);
        }

        async void RefreshSO(Models.SaleorderData data)
        {
            DateTime today = Helpers.Controls.GetToday();
            if (SelectPeriod.Start == today)
            {
                if (data == null) { return; }
                ActiveWork.Add(data);
                ListData.ItemsSource = null;
                ListData.ItemsSource = ActiveWork;
            }
            else
            {
                SelectPeriod.Start = today;
                await Showdata();
            }

        }

        async void GotoOwnerPage(bool success = false)
        {
            //Models.UserData result = null;
            //if (success) { result = ActiveUser; }
            //MessagingCenter.Send<TodayPage, Models.UserData>(this, OwnerPage, result);
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