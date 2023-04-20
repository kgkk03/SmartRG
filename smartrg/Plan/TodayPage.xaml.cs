using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Plan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodayPage : ContentPage
    {
        string Mypage = "Plan.TodayPage";
        bool IsEdit = false;
        List<Models.PlanData> listplan = new List<Models.PlanData>();

        public TodayPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Showdata();
        }
        void Showdata()
        {
            AidWaitingRun(true, "กำลังดำเนินการเรียกดูข้อมูลแผนการทำงาน...");
            if(App.Servertime==new DateTime()) { LblTodayHeader.Text = " (" + Helpers.Controls.Date2ThaiString(DateTime.Now, "d-MMM-yyyy") + ")"; }
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
        async void RefreshListdata()
        {
            try
            {
                AidWaitingRun(true, "กำลังดำเนินการเรียกดูข้อมูลการทำงาน...");
                DateTime today = Helpers.Controls.GetToday();
                listplan = await App.Ws.GetPlanList(today, today.AddDays(1));
                ListData.ItemsSource = listplan;
                LblNodata.IsVisible = (listplan == null || listplan.Count == 0);
            }
            catch { }
            IsEdit = false;
            AidWaitingRun(false);
        }


        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<TodayPage, bool>(this, "OpenMenu", true);
        }

      
        private void BtnMapview_Clicked(object sender, EventArgs e)
        {
            RefreshListdata();
        }
        private async void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (await DisplayAlert("ยืนยัน","คุณต้องการเข้าร้านนี้ใช่หรือไม่","ใช่","ไม่"))
                {
                    Models.PlanData item = (Models.PlanData)e.CurrentSelection.FirstOrDefault();
                    if (item == null) { return; }
                    if (IsEdit) { return; }
                    if (await CheckLastVisit()) { return; }

                    if (item.Planstatus < 3)
                    {
                        // Visit OnPlan
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
                        Models.CustomerData cust = await App.Ws.GetCustomer(item.Custid);
                        CreateNewvisit(cust, item);
                        IsEdit = false;
                    }
                    else
                    {
                        // View Visit
                        AidWaitingRun(true, "กำลังดำเนินการเรียกดูข้อมูลการทำงาน...");
                        IsEdit = true;
                        Models.CustomerData cust = await App.Ws.GetCustomer(item.Custid);
                        Models.VisitShowpageData data = await Helpers.Controls.GetVisitShowpage(cust, null, item.Key);
                        if (data == null)
                        {
                            await DisplayAlert("แจ้งเตือน", "ไม่พบข้อมูลการทำงานที่เลือก", "ตกลง");
                            IsEdit = false;
                        }
                        else
                        {
                            var page = new Visit.VisitTabPage();
                            page.Setdata(Mypage, data);
                            foreach (var dr in data.VisitPage) { page.SetListPage(dr); }
                            page.ShowPage();
                            IsEdit = false;
                            await Navigation.PushModalAsync(page);
                            VisitTabPageMessage();
                        }
                    }
                }
            }
            catch
            {
                IsEdit = false;
            }
            ListData.SelectedItem = null;
            AidWaitingRun(false);
        }


        // =========== Check in (Unplan visit) ===========
        #region Unplan visit
        private async void BtnCheckin_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                if (await CheckLastVisit()) { return; }
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
        void GetCustomerPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Customer.GetCustomerPage, Models.CustomerData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { CreateNewvisit(arg,null); } catch { }
                        MessagingCenter.Unsubscribe<Customer.GetCustomerPage, Models.CustomerData>(this, Mypage);
                    });
                });

            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        async void CreateNewvisit(Models.CustomerData visitcust,Models.PlanData plan)
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
                if (plan != null) { plan.Planstatus = 4; } //เข้างาน  (ยังไม่เลิก)
                Models.VisitShowpageData data = await Helpers.Controls.GetVisitShowpage(visitcust, plan, "");
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
            if (data.VisitPage == null || data.VisitPage.Count == 0)
            {
                await DisplayAlert("แจ้งเตือน", "ไม่สามารถเปิดหน้าเข้าพบได้ เนื่องจากมีข้อมูลไม่เพียงพอ", "ตกลง");
                AidWaitingRun(false);
                return;
            }
            string msg = await App.Ws.SaveVisit(data.Visitdata);
            if (msg.Equals(""))
            {
                if (data.Visitdata.Planid.Equals("")) { data.Visitdata.Planid = data.Visitdata.Key; }
                var page = new Visit.VisitTabPage();
                page.Setdata(Mypage, data);
                foreach (var dr in data.VisitPage) { page.SetListPage(dr); }
                page.ShowPage();
                await Helpers.Controls.SaveVisit(data);
                IsEdit = false;
                AidWaitingRun(false);
                await Navigation.PushModalAsync(page);
                VisitTabPageMessage();
            }
            else
            {
                IsEdit = false;
                AidWaitingRun(false);
                msg = "ไม่สามารถส่งข้อมูลเริ่มงานได้โปรดตรวจสอบอินเตอร์เน็ตของท่าน \n" + msg;
                await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
            }
        }
        void VisitTabPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Visit.VisitTabPage, Models.VisitShowpageData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { RefreshListdata(); } catch { }
                        MessagingCenter.Unsubscribe<Visit.VisitTabPage, Models.VisitShowpageData>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        async Task<bool> CheckLastVisit()
        {
            try
            {
                Models.VisitShowpageData lastvisit = await Helpers.Controls.CheckLastVisit();
                //lastvisit.Detail.VisitStockSuccess = false;
                if (lastvisit != null)
                {
                    
                    if (!await DisplayAlert("แจ้งเตือน", "คุณมีการเข้างานค้างไว้ยังไม่ได้ส่ง\nต้องการคีย์ข้อมูลต่อหรือลบข้อมูลทิ้ง", "ยกเลิก", "คีย์ข้อมูล"))
                    {
                        var page = new Visit.VisitTabPage();
                        page.Setdata("MasterMenuPage", lastvisit);
                        foreach (var dr in lastvisit.VisitPage) {
                            page.SetListPage(dr); 
                        }
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
            catch (Exception ex){ 
                var a = ex.Message; 
            }
            return await Task.FromResult(false);

        }
        #endregion


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

        private void ViewCell_BindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                ViewCell ViewCellListView = ((ViewCell)sender);
                Label lblVisitDate = ViewCellListView.FindByName<Label>("lblVisitDate");
                Image imgIcon = ViewCellListView.FindByName<Image>("imgIcon");
                var item = ViewCellListView.BindingContext as Models.PlanData;
                if (listplan != null || listplan.Count != 0)
                {
                    lblVisitDate.Text = Helpers.Controls.Date2String(item.Visitdate.AddHours(7),"HH:mm");
                    imgIcon.Source = item.Planstatus == 12 ? "ic_vworking" : "ic_onplan";
                }
            }
            catch
            {

            }
        }

        private async void ListData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                Models.PlanData item = (Models.PlanData)ListData.SelectedItem;
                if (item == null) { return; }
                if (IsEdit) { return; }
                if (await CheckLastVisit()) { return; }

                if (item.Planstatus < 3)
                {
                    // Visit OnPlan
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
                    Models.CustomerData cust = await App.Ws.GetCustomer(item.Custid);
                    CreateNewvisit(cust, item);
                    IsEdit = false;
                }
                else
                {
                    // View Visit
                    AidWaitingRun(true, "กำลังดำเนินการเรียกดูข้อมูลการทำงาน...");
                    IsEdit = true;
                    Models.CustomerData cust = await App.Ws.GetCustomer(item.Custid);
                    Models.VisitShowpageData data = await Helpers.Controls.GetVisitShowpage(cust, null, item.Key);
                    if (data == null)
                    {
                        await DisplayAlert("แจ้งเตือน", "ไม่พบข้อมูลการทำงานที่เลือก", "ตกลง");
                        IsEdit = false;
                    }
                    else
                    {
                        var page = new Visit.VisitTabPage();
                        page.Setdata(Mypage, data);
                        foreach (var dr in data.VisitPage) { page.SetListPage(dr); }
                        page.ShowPage();
                        IsEdit = false;
                        await Navigation.PushModalAsync(page);
                        VisitTabPageMessage();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
                IsEdit = false;
            }
            ListData.SelectedItem = null;
            AidWaitingRun(false);
        }
    }
}