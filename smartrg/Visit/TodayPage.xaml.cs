using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Visit
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodayPage : ContentPage
    {
        string Mypage = "Visit.TodayPage";
        bool IsEdit = false;
        public TodayPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Showdata();
        }

        void TodayMapPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<TodayMapPage, bool>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { AidWaitingRun(false); IsEdit = false; } catch { }
                        MessagingCenter.Unsubscribe<TodayMapPage, bool>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }

        void GetCustomerPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Customer.GetCustomerPage, Models.CustomerData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { CreateNewvisit(arg); } catch { }
                        MessagingCenter.Unsubscribe<Customer.GetCustomerPage, Models.CustomerData>(this, Mypage);
                    });
                });

            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        void VisitTabPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<VisitTabPage, Models.VisitShowpageData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { RefreshListdata(); } catch { }
                        MessagingCenter.Unsubscribe<VisitTabPage, Models.VisitShowpageData>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<TodayPage, bool>(this, "OpenMenu", true);
        }
        void Showdata()
        {
            AidWaitingRun(true, "กำลังดำเนินการเรียกดูข้อมูลการทำงาน...");
            if (App.Servertime == new DateTime()) { LblTodayHeader.Text = " (" + Helpers.Controls.Date2ThaiString(DateTime.Now, "d-MMM-yyyy") + ")"; }
            else { LblTodayHeader.Text = " (" + Helpers.Controls.Date2ThaiString(App.Servertime, "d-MMM-yyyy") + ")"; }
            ShowEmployee();
            RefreshListdata();
        }
        async void ShowEmployee()
        {
            ImgUser.Source = await Helpers.Controls.GetProfileImage();
            LblTeamname.Text = App.UserProfile.Authen + " (" + App.UserProfile.TeamName + ")";
            LblUserID.Text = App.UserProfile.Empcode;
            LblFullname.Text = App.UserProfile.Fullname;
        }
        private async void BtnCheckin_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                if(await CheckLastVisit()) { return; }
                IsEdit = true;
                AidWaitingRun(true, "ตรวจสอบข้อมูลตำแหน่ง...");
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
                App.Checkinlocation = await Geolocation.GetLocationAsync(request);
                AidWaitingRun(true, "ได้ตำแหน่งจีพีเอสแล้ว");
                if (App.Checkinlocation == null && App.Checkinlocation.Latitude <= 0)
                {
                    await DisplayAlert("แจ้งเตือน", "ไม่พบสัญญาณจีพีเอส กรุณาตรวจสอบ", "ยกเลิก");
                    AidWaitingRun(false);
                    IsEdit = false;
                    return;
                }
                var page = new Customer.GetCustomerPage();
                page.Setdata(Mypage);
                await Navigation.PushModalAsync(page);
                GetCustomerPageMessage();
            }
            catch { IsEdit = false; }

        }
        async Task<bool> CheckLastVisit()
        {
            try {
                Models.VisitShowpageData lastvisit = await Helpers.Controls.CheckLastVisit();
                lastvisit.Detail.VisitStockSuccess = false;
                if (lastvisit != null)
                {
                    if (!await DisplayAlert("แจ้งเตือน", "คุณมีการเข้างานค้างไว้ยังไม่ได้ส่ง\nต้องการคีย์ข้อมูลต่อหรือลบข้อมูลทิ้ง", "ยกเลิก", "คีย์ข้อมูล"))
                    {
                        var page = new VisitTabPage();
                        page.Setdata("MasterMenuPage", lastvisit);
                        foreach (var dr in lastvisit.VisitPage) { page.SetListPage(dr); }
                        page.ShowPage();
                        await Navigation.PushModalAsync(page);
                        return await Task.FromResult(true);
                    }
                    else
                    {
                        //await Helpers.Controls.ClearLastvisit(lastvisit.Visitdata.Key);
                        return await Task.FromResult(true);
                    }
                }
            }
            catch { }
            return await Task.FromResult(false);

        }
        async void CreateNewvisit(Models.CustomerData visitcust)
        {
            if (visitcust == null)
            {
                string msg = "คุณยังไม่ได้เลือกลูกค้าที่ต้องการเข้าพบ";
                await DisplayAlert("แจ้งเตือน", msg, "ยกเลิก");
                AidWaitingRun(false);
                IsEdit = false;
                return;
            }
            else
            {
                Models.VisitShowpageData data = await Helpers.Controls.GetVisitShowpage(visitcust, null, "");
                if (data.Visitdata.Errortype == -1)
                {
                    string msg = "คุณลงชื่อเข้างานห่างจากระยะที่กำหนด \n" + " ถ้ายืนยันดำเนินการต่อระบบจะรายงานความผิดพลาดนี้";
                    if (await DisplayAlert("แจ้งเตือน", msg, "ลงชื่อเข้างาน", "ยกเลิก"))
                    {
                        OpenVisitPage(data);
                    }
                    else
                    {
                        AidWaitingRun(false);
                        IsEdit = false;
                    }
                }
                else { OpenVisitPage(data); }
            }

        }
        async void OpenVisitPage(Models.VisitShowpageData data)
        {
            if(data.VisitPage==null || data.VisitPage.Count == 0)
            {
                await DisplayAlert("แจ้งเตือน", "ไม่สามารถเปิดหน้าเข้าพบได้ เนื่องจากมีข้อมูลไม่เพียงพอ", "ตกลง");
                AidWaitingRun(false);
                return;
            }
            string msg = await App.Ws.SaveVisit(data.Visitdata);
            if (msg.Equals("")) {
                var page = new VisitTabPage();
                page.Setdata(Mypage, data);
                foreach (var dr in data.VisitPage) { page.SetListPage(dr); }
                page.ShowPage();
                await Helpers.Controls.SaveVisit(data);
                IsEdit = false;
                AidWaitingRun(false);
                await Navigation.PushModalAsync(page);
                VisitTabPageMessage();
            }
            else {
                IsEdit = false;
                AidWaitingRun(false);
                msg = "ไม่สามารถส่งข้อมูลเริ่มงานได้โปรดตรวจสอบอินเตอร์เน็ตของท่าน \n" + msg;
                await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
            }
        }
        private async void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Models.VisitData item = (Models.VisitData)e.CurrentSelection.FirstOrDefault();
                if (item == null) { return; }
                if (IsEdit) { return; }
                if (await CheckLastVisit()) { return; }
                AidWaitingRun(true , "กำลังดำเนินการเรียกดูข้อมูลการทำงาน...");
                IsEdit = true;
                Models.CustomerData cust = await App.Ws.GetCustomer(item.Custid) ;
                Models.VisitShowpageData data = await Helpers.Controls.GetVisitShowpage(cust,null, item.Key);
                if(data == null)
                {
                    await DisplayAlert("แจ้งเตือน", "ไม่พบข้อมูลการทำงานที่เลืก", "ตกลง");
                    IsEdit = false;
                }
                else
                {
                    var page = new VisitTabPage();
                    page.Setdata(Mypage, data);
                    foreach (var dr in data.VisitPage) { page.SetListPage(dr); }
                    page.ShowPage();
                    IsEdit = false;
                    await Navigation.PushModalAsync(page);
                    VisitTabPageMessage();
                }
            }
            catch
            {
                IsEdit = false; 
            }
            ListData.SelectedItem = null;
            AidWaitingRun(false);
        }
        async void RefreshListdata()
        {
            try
            {
                AidWaitingRun(true, "กำลังดำเนินการเรียกดูข้อมูลการทำงาน...");
                DateTime today = Helpers.Controls.GetToday();
                List<Models.VisitData> listvisit = await App.Ws.GetVisitList(today, today.AddDays(1));
                ListData.ItemsSource = listvisit;
                LblNodata.IsVisible = (listvisit == null || listvisit.Count == 0);
            }
            catch { }
            IsEdit = false;
            AidWaitingRun(false);
        }
        private async void BtnMapview_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                IsEdit = true;
                List<Models.VisitData> listvisit =  (List<Models.VisitData>)ListData.ItemsSource;
                if ((listvisit == null) || (listvisit.Count == 0))
                {
                    string msg = "ไม่สามารถเรียกดูข้อมูลแผนที่ทำงานได้... \nเนื่องจากไม่มีข้อมูลการทำงานในวันนี้";
                    await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                    IsEdit = false;
                }
                else
                {
                    AidWaitingRun(true, "ตรวจสอบข้อมูลตำแหน่ง...");
                    var page = new TodayMapPage();
                    page.Setdata(Mypage, listvisit);
                    await Navigation.PushModalAsync(page);
                    TodayMapPageMessage();
                }
            }
            catch(Exception ex) {
                var a = ex.Message;
                IsEdit = false;
                AidWaitingRun(false);
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