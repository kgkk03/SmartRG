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
    public partial class GetCustomerPage : ContentPage
    {
        bool IsEdit = false;
        string OwnerPage = "GetCustomerPage";
        Models.CustomerData ActiveCustomer = null;
        int Filterid = 0;
        bool SelectLocation = true;
        int StartLimit = 0;
        public GetCustomerPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        public void Setdata(string ownerpage,bool Searchlocation = true, List<Models.CustomerFillter> filter = null )
        {
            OwnerPage = ownerpage;
            if (filter == null) { filter = Helpers.Controls.GetCustFillter(App.UserProfile.Teamid); }
            else { Filterid = filter[0].ID; }
            Pikfilter.ItemsSource = filter;
            Pikfilter.ItemDisplayBinding = new Binding("Display"); ;
            Showdata(Searchlocation);
        }
        private async void Showdata(bool Searchlocation = true)
        {
            if (IsEdit) { return; }
            if (Searchlocation) {
                AidWaitingRun(true, "กำลังค้นหาข้อมูลลูกค้าบริเวณนี้...");
                await GetListLocationCustomer();
            }
            else {
                AidWaitingRun(true, "กำลังค้นหาข้อมูลลูกค้า");
                SelectLocation = false;
                await GetListCustomer();
            }            
            AidWaitingRun(false);
        }

        #region Search Customer
        private async void BtnLocation_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            await GetListLocationCustomer();
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
            StkFilter.IsVisible = true;
            StkSearch.IsVisible = true;
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
                if (cust == null)
                {
                    await DisplayAlert("แจ้งเตือน", "ไม่สามารถเรียกข้อมูลลูกค้าที่ต้องการได้", "ตกลง");
                    IsEdit = false;
                    ActiveCustomer = null;
                }
                else
                {
                    ActiveCustomer = cust;
                    string msg = ActiveCustomer.Custname;
                    string btnok = "เข้าพบลูกค้า";
                    if (OwnerPage.Equals("Visit.TodayPage")) { msg = "คุณต้องการเข้าพบลูกค้า \n" + msg; }
                    else if (OwnerPage.Equals("SO.TodayPage")) { msg = "คุณต้องการสั่งสินค้าสำหรับ \n" + msg; btnok = "สั่งขายสินค้า"; }
                    else if (OwnerPage.Equals("ListPickingPage")) { msg = "คุณต้องการเบิกสินค้าจาก \n" + msg; btnok = "เบิกสินค้า"; }
                    else if (OwnerPage.Equals("ListCashsalePage")) { msg = "คุณต้องการขายเงินสดสำหรับ \n" + msg; btnok = "ขายเงินสด"; }

                    if (await DisplayAlert("ยืนยัน", msg, btnok, "ไม่ต้องการ"))
                    {
                        GotoOwnerPage(true);
                    }
                }

            }
            catch(Exception ex) 
            {
                var exc = ex.Message;
            }
            IsEdit = false;
            ListData.SelectedItem = null;
            AidWaitingRun(false);
        }
        private void BtnBack_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }
        async void GotoOwnerPage(bool success = false)
        {
            Models.CustomerData result = null;
            if (success) { result = ActiveCustomer; }
            MessagingCenter.Send<GetCustomerPage, Models.CustomerData>(this, OwnerPage, result);
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