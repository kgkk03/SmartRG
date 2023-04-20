using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace smartrg.Visit
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodayMapPage : ContentPage
    {
        string OwnerPage = "TodayMapPage";
        Models.VisitData ActiveVisit = null;
        public TodayMapPage()
        {
            InitializeComponent();
        }

        public async void Setdata(string ownerpage, List<Models.VisitData> datas)
        {
            OwnerPage = ownerpage;
            await SetMap(datas);
            CvCustomer.ItemsSource = datas;
        }
        async Task<bool> SetMap(List<Models.VisitData> datas)
        {
            try
            {
                double Lat = 0;
                double Lng = 0;
                foreach (var dr in datas)
                {
                    
                    if (!(dr.Lat == 0 || dr.Lng == 0) )
                    {
                        await PlotMap(dr.Lat, dr.Lng, dr.Custname, dr.Showtime);
                        if (Lat == 0 || Lng == 0) { Lat = dr.Lat; Lng = dr.Lng; }
                    }
                    else
                    {
                        return await Task.FromResult(false);
                    }
                }
                if (!(Lat == 0 || Lng == 0))
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Lat, Lng), Distance.FromKilometers(10)));
                }
                else
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(13.745262, 100.551273), Distance.FromKilometers(10)));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Create CandidateBP SetMap Error", ex.Message, "OK");
            }
            return await Task.FromResult(false);
        }
        async Task<bool> PlotMap(double lat, double lng, string label, string address)
        {
            try
            {
                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = new Position(lat, lng),
                    Label = label,
                    Address = address,
                };
                map.Pins.Add(pin);

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("PlotMap Error", ex.Message, "OK");
                return await Task.FromResult(false);
            }
        }

        private void CvCustomer_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            //Models.CustomerData previousItem = (Models.CustomerData)e.PreviousItem ;
            ActiveVisit = (Models.VisitData)e.CurrentItem;
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(ActiveVisit.Lat, ActiveVisit.Lng), Distance.FromMeters(100)));

        }
        private async void Navigator_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Xamarin.Essentials.Map.OpenAsync(ActiveVisit.Custlat , ActiveVisit.Custlng, new Xamarin.Essentials.MapLaunchOptions
                {
                    Name = ActiveVisit.Custname,
                    NavigationMode = Xamarin.Essentials.NavigationMode.Driving,
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
            MessagingCenter.Send<TodayMapPage, bool>(this, OwnerPage, success);
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