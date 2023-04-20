using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.Extensions;

namespace smartrg.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerMapPage : ContentPage
    {
        string OwnerPage = "CustomerPage";
        List<Models.CustinlistData> CustomerList = new List<Models.CustinlistData>();
        Models.CustinlistData ActiveCustomer = null;
        public CustomerMapPage()
        {
            InitializeComponent();
        }

        public async void Setdata(string ownerpage, List<Models.CustinlistData> datas)
        {
            OwnerPage = ownerpage;
            CustomerList = datas;
            await SetMap(CustomerList);
            CvCustomer.ItemsSource = CustomerList;
        }
        async Task<bool> SetMap(List<Models.CustinlistData> datas)
        {
            try
            {
                Position fistposition = new Position(0,0);
                foreach (var dr in datas)
                {

                    if (!(dr.Lat == 0 || dr.Lng == 0))
                    {
                        await PlotMap(dr);
                        if (fistposition ==  new Position(0,0) ) { fistposition = new Position(dr.Lat, dr.Lng); }
                    }
                }
                if (fistposition != new Position(0, 0)) {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(fistposition, Distance.FromKilometers(10)));
                }
                map.PinClicked += (s, e) => { ShowCustomerPin(s, e); };
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Create CandidateBP SetMap Error", ex.Message, "OK");
            }
            return await Task.FromResult(false);
        }
        async Task<bool> PlotMap(Models.CustinlistData data, Pin selectedpin = null)
        {
            try
            {
                bool newpin = (selectedpin == null);
                if (newpin) { selectedpin = new Pin(); }
                selectedpin.Type = PinType.Place;
                selectedpin.Position = new Position(data.Lat, data.Lng);
                selectedpin.Label = data.Custname;
                selectedpin.Address = data.Custaddress;
                selectedpin.BindingContext = data;
                if (newpin) { map.Pins.Add(selectedpin); }
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("PlotMap Error", ex.Message, "OK");
                return await Task.FromResult(false);
            }
        }

        void ShowCustomerPin(object sender, PinClickedEventArgs e)
        {
            Pin item = e.Pin;
            Models.CustinlistData data = (Models.CustinlistData)item.BindingContext;
            List<Models.CustinlistData> datas =(List<Models.CustinlistData>) CvCustomer.ItemsSource;
            int index = datas.FindIndex(x => x.Custid.Equals(data.Custid));
            if (index >= 0) { CvCustomer.Position = index; }
        }

        private void CvCustomer_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            //Models.CustomerData previousItem = (Models.CustomerData)e.PreviousItem ;
            ActiveCustomer = (Models.CustinlistData) e.CurrentItem ;
            double Lat = ActiveCustomer.Lat;
            double Lng = ActiveCustomer.Lng;
            if (!(Lat == 0 || Lng == 0))
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Lat, Lng), Distance.FromMeters(500)));
            }
            else
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(13.745262, 100.551273), Distance.FromMeters(500)));
                this.DisplayToastAsync("ไม่มีข้อมูลตำแหน่ง"+ ActiveCustomer.Custname, 5000);
            }
        }

        private async void BtnNavigator_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (ActiveCustomer == null) { return; }
                await Xamarin.Essentials.Map.OpenAsync(ActiveCustomer.Lat, ActiveCustomer.Lng, new MapLaunchOptions
                {
                    Name = ActiveCustomer.Custname,
                    NavigationMode = NavigationMode.Driving,
                });
            }
            catch
            {
                await DisplayAlert("แจ้งเตือน", "ไม่สามารถเปิดใช้งานแผนที่ได้ \n โปรดตรวจสอบการใช้งานตำแหน่ง", "ตกลง");
            }
        }

        private void Btnback_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }

        async void GotoOwnerPage(bool success = false)
        {
            MessagingCenter.Send<CustomerMapPage, bool>(this, OwnerPage, success);
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