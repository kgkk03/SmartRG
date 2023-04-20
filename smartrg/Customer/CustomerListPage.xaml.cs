using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerListPage : ContentPage
    {
        bool IsEdit = false;
        string Mypage = "CustomerListPage";
        int Filterid = 0;
        bool SelectLocation = true;
        int StartLimit = 0;


        public CustomerListPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Showdata();
        }
     
        private async void Showdata() {
            AidWaitingRun(true, "กำลังค้นหาข้อมูลลูกค้าบริเวณนี้...");
            List<Models.CustomerFillter> filter = Helpers.Controls.GetCustFillter(App.UserProfile.Teamid);
            Pikfilter.ItemsSource = filter;
            Pikfilter.ItemDisplayBinding = new Binding("Display");
            await GetListLocationCustomer();
            AidWaitingRun(false);
        }

        #region Search Customer
        private async void BtnLocation_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            await GetListLocationCustomer();
            stkFillterOption.IsVisible = false;
        }
        async Task<bool> GetListLocationCustomer()
        {
            bool result = false;
            try
            {
                IsEdit = true;
                AidWaitingRun(true, "ตรวจสอบข้อมูลตำแหน่ง...");
                Txtsearch.Text = "";
                StkSearch.IsVisible = false;
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
                App.Checkinlocation = await Geolocation.GetLocationAsync(request);
                SelectLocation = true;
                StartLimit = 0;
                Filterid = 0;
                LblHeader.Text = "ข้อมูลลูกค้า (บริเวณนี้)";
                await GetListCustomer();
                result = true;
            }
            catch { }
            IsEdit = false;
            AidWaitingRun(false);
            return await Task.FromResult(result);
        }
        async Task<bool> GetListCustomer()
        {
            AidWaitingRun(true, "กำลังค้นหาข้อมูลลูกค้า...");
            double lat = 0;
            double lng = 0;
            int filter = 0;
            string keyword = "";
            if (SelectLocation)
            {
                if (App.Checkinlocation == null && App.Checkinlocation.Latitude <= 0)
                {
                    await DisplayAlert("แจ้งเตือน", "ไม่พบสัญญาณจีพีเอส กรุณาตรวจสอบ", "ยกเลิก");
                    AidWaitingRun(false);
                    IsEdit = false;
                    return await Task.FromResult(false);
                }
                lat = App.Checkinlocation.Latitude;
                lng = App.Checkinlocation.Longitude;
            }
            else
            {
                if (Txtsearch.Text == null) { Txtsearch.Text = ""; }
                filter = Filterid;
                keyword = Txtsearch.Text;
            }

            List<Models.CustinlistData> listofdata = await App.Ws.GetListCustomer(filter, keyword, lat, lng, StartLimit);
            ListData.ItemsSource = listofdata;
            AidWaitingRun(false);
            return await Task.FromResult(true);

        }

        private async void Txtsearch_SearchButtonPressed(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            await GetListCustomer();
        }
        private async void BtnSearch_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            await GetListCustomer();
        }
        private void BtnFilter_Clicked(object sender, EventArgs e)
        {
            //StkFilter.IsVisible = true;
            //StkSearch.IsVisible = true;
            stkFillterOption.IsVisible = false;
            Pikfilter.Focus();

        }
        private void BtnFilterclose_Clicked(object sender, EventArgs e)
        {
            StkFilter.IsVisible = false;
            if (LblHeader.Text.Equals("ข้อมูลลูกค้า (บริเวณนี้)")) { StkSearch.IsVisible = false; }
        }

        private async void Pikfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            Models.CustomerFillter item = (Models.CustomerFillter)Pikfilter.SelectedItem;
            if (item != null)
            {
                Filterid = item.ID;
                Pikfilter.SelectedItem = null;
                LblHeader.Text = "ข้อมูลลูกค้า (" + item.Display + ")";
                SelectLocation = false;
                StartLimit = 0;
                await GetListCustomer();
                StkFilter.IsVisible = false;
            }
        }
        #endregion

        #region Create Candidate

        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
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
                var page = new CandidatePage();
                page.Setdata(Mypage, App.Checkinlocation);
                await Navigation.PushModalAsync(page);
                CandidatePageMessage();
            }
            catch { IsEdit = false; }
        }
        void CandidatePageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<CandidatePage, Models.CustomerData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { CreateNewCandidate(arg); Showdata(); } catch { }
                        MessagingCenter.Unsubscribe<CandidatePage, Models.CustomerData>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }


        private async  void CreateNewCandidate(Models.CustomerData cust)
        {
            if (cust != null)
            {
                if(!await CheckLastVisit()) {
                    string msg = "คุณต้องการเข้าพบลูกค้า \n" + cust.Custname;
                    if (await DisplayAlert("ยืนยัน", msg, "เข้าพบลูกค้า", "ไม่ต้องการ"))
                    {
                        CreateVisit(cust);
                        return;
                    }
                }
            }
            AidWaitingRun(false);
            IsEdit = false;
            return;
        }

        #endregion

        #region Create Visit
        async Task<bool> CheckLastVisit()
        {
            try
            {
                Models.VisitShowpageData lastvisit = await Helpers.Controls.CheckLastVisit();
                if (lastvisit != null)
                {
                    if (!await DisplayAlert("แจ้งเตือน", "คุณมีการเข้างานค้างไว้ยังไม่ได้ส่ง\nต้องการคีย์ข้อมูลต่อหรือลบข้อมูลทิ้ง", "ลบทิ้ง", "คีย์ข้อมูล"))
                    {
                        var page = new Visit.VisitTabPage();
                        page.Setdata("MasterMenuPage", lastvisit);
                        foreach (var dr in lastvisit.VisitPage) { page.SetListPage(dr); }
                        page.ShowPage();
                        await Navigation.PushModalAsync(page);
                        return await Task.FromResult(true);
                    }
                    else
                    {
                        await Helpers.Controls.ClearLastvisit(lastvisit.Visitdata.Key);
                    }
                }
            }
            catch { }
            return await Task.FromResult(false);

        }

        async void CreateVisit(Models.CustomerData cust)
        {
            if (cust != null)
            {
                Models.VisitShowpageData data = await Helpers.Controls.GetVisitShowpage(cust, null, "");
                if (data.Visitdata.Errortype==-1)
                {
                    string msg = "คุณลงชื่อเข้างานห่างจากระยะที่กำหนด \n" + " ถ้ายืนยันดำเนินการต่อระบบจะรายงานความผิดพลาดนี้";
                    if (await DisplayAlert("แจ้งเตือน", msg, "ลงชื่อเข้างาน", "ยกเลิก")) {
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
            string msg = await App.Ws.SaveVisit(data.Visitdata);
            if (msg.Equals(""))
            {
                if (data.Visitdata.Planid.Equals("")) { data.Visitdata.Planid = data.Visitdata.Key; }
                if (data.Detail.Newvisit) { App.dbmng.InsetData(data.Visitdata); }
                var page = new Visit.VisitTabPage();
                page.Setdata(Mypage, data);
                foreach (var dr in data.VisitPage) { page.SetListPage(dr); }
                page.ShowPage();
                IsEdit = false;
                AidWaitingRun(false);
                await Navigation.PushModalAsync(page);
            }
            else
            {
                IsEdit = false;
                AidWaitingRun(false);
                msg = "ไม่สามารถส่งข้อมูลเริ่มงานได้โปรดตรวจสอบอินเตอร์เน็ตของท่าน \n" + msg;
                await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
            }



           
        }

        #endregion

        private async void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Models.CustinlistData item = (Models.CustinlistData)e.CurrentSelection.FirstOrDefault();
                if (item == null) { return; }
                if (IsEdit) { return; }
                IsEdit = true;
                AidWaitingRun(true, "กำลังค้นหาข้อมูลลูกค้า...");
                string keyword = Txtsearch.Text == null ? "" : Txtsearch.Text;
                Models.CustomerData cust = await App.Ws.GetCustomer(item.Custid);
                if (cust == null) { 
                    await DisplayAlert("แจ้งเตือน", "ไม่สามารถเรียกข้อมูลลูกค้าที่ต้องการได้", "ตกลง");
                    IsEdit = false;
                }
                else
                {
                    var page = new CustTabPage();
                    page.Setdata(Mypage, cust);
                    var listpage = App.dbmng.sqlite.Table<Models.CustomerPage>().Where(x => x.Custtype == cust.Typeid).OrderBy(x => x.Piority).ToList();
                    foreach (var dr in listpage) { page.SetListPage(dr); }
                    page.ShowPage();
                    await Navigation.PushModalAsync(page);
                    CustTabPageMessage();
                }
            }
            catch {
                IsEdit = false; 
            }
            ListData.SelectedItem=null;
            AidWaitingRun(false);
        }

        void CustTabPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<CustTabPage, Models.CustomerData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(async () => {
                        try
                        {
                            MessagingCenter.Unsubscribe<CustTabPage, Models.CustomerData>(this, Mypage);
                            IsEdit = false;
                            AidWaitingRun(false);
                            if (arg != null) { 
                                if(!await CheckLastVisit()) { CreateVisit(arg); }
                            }
                        }
                        catch { }
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("CustTabPageMessage Error", ex.Message, "OK"); }
        }

        #region Mapview
        private void BtnMapview_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            AidWaitingRun(true, "กำลังเปิดแผนที่...");
            IsEdit = true;
            List<Models.CustinlistData> listofdata = (List<Models.CustinlistData>)ListData.ItemsSource;
            if (listofdata == null || (listofdata.Count == 0)) {
                DisplayAlert("แจ้งเตือน", "ไม่มีข้อมูลลูกค้า กรุณาเลือกลูกค้าก่อนเปิดแผนที่", "ตกลง");
            }
            else
            {
                var page = new CustomerMapPage();
                page.Setdata(Mypage, listofdata);
                Navigation.PushModalAsync(page);
                CustomerMapPageMessage();
                stkFillterOption.IsVisible = false;
            }
            
            AidWaitingRun(false);
        }
        void CustomerMapPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<CustomerMapPage, bool>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { IsEdit = false; } catch { }
                        MessagingCenter.Unsubscribe<CustomerMapPage, bool>(this, Mypage);
                    });
                });


            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }

        #endregion

        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<CustomerListPage, bool>(this, "OpenMenu", true);
        }

        private void BtnFillterOption_Clicked(object sender, EventArgs e)
        {
            stkFillterOption.IsVisible = !stkFillterOption.IsVisible;
        }

        async void GotoOwnerPage(bool success = false)
        {
            //Models.UserData result = null;
            //if (success) { result = ActiveUser; }
            //MessagingCenter.Send<CustomerListPage, Models.UserData>(this, OwnerPage, result);
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