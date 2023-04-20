using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
using System.Net.Mqtt;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using Xamarin.CommunityToolkit.Extensions;

namespace smartrg.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmployeeToday : ContentPage
    {
        string OwnerPage = "TodayMapPage";
        List<Models.VisitData> VisitList = null;
        List<Models.Empvisit> EmpList = new List<Models.Empvisit>();
        Models.Empvisit ActiveVisit = null;
        IMqttClient MqttRealtime;
        public EmployeeToday()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Setdata();
        }
        public async void Setdata()
        {
            AidWaitingRun(true, "กำลังตรวจสอบข้อมูลพนักงาน");
            try {

                EmpList = await App.Ws.GetEmpByAudit();
                VisitList = await App.Ws.GetVisitByAudit();
                await StartMqttRealtime();
                foreach (var emp in EmpList)
                {
                    var visit = VisitList.Find(x => x.Empid == emp.Empid);
                    if (visit != null) { emp.Visit = visit; }
                    await SetlisteningRelatime(emp.Empid);
                }
                await SetMap(EmpList);
                CvCustomer.ItemsSource = EmpList;
                SetImage();
            }
            catch { }
            AidWaitingRun(false);
        }
        void SetImage()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if(EmpList!=null && EmpList.Count > 0)
                {
                    foreach (var dr in EmpList)
                    { LoadImage(dr); }
                }
                //return true; // return true to repeat counting, false to stop timer
                return false;
            });
        }

        async void LoadImage(Models.Empvisit dr)
        {
            dr.Icon = await App.Ws.GetEmpThumbnail(dr.Empid);
        }


        async Task<bool> StartMqttRealtime()
        {
            try
            {
                MqttRealtime = await MqttClient.CreateAsync(App.MqttIP, App.MqttPort);
                await MqttRealtime.ConnectAsync();
                MqttRealtime.MessageStream.Subscribe(ReceiverRelatime);
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        async Task<bool> SetlisteningRelatime(int Empid)
        {
            try
            {
                //AidWaitingRun(true, "กำลังส่งคำขอตำแหน่ง " + Empid.ToString + " ไปยังเชริฟเวอร์...");
                string toppic = "smartrg-" + Empid.ToString();
                await MqttRealtime.SubscribeAsync(toppic, MqttQualityOfService.ExactlyOnce);
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        private void ReceiverRelatime(MqttApplicationMessage msg)
        {
            try
            {
                string res = Encoding.UTF8.GetString(msg.Payload);
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetMqttRelatimeData(res);
                });
            }
            catch { }
        }
        async void  SetMqttRelatimeData(string mqttdata)
        {
            try
            {
                Models.VisitData data = JsonConvert.DeserializeObject<Models.VisitData>(mqttdata);
                var Emp = EmpList.Find(x => x.Empid == data.Empid);
                if (Emp != null) {
                    DateTime today = Helpers.Controls.GetToday();
                    if (data.Modifieddate > today) { data.Showtime = Helpers.Controls.Date2String(data.Modifieddate, "HH:mm"); }
                    else { data.Showtime = Helpers.Controls.Date2ThaiString(data.Modifieddate, "d-M-yy"); }
                    if (Emp.Visit == null) {await PlotMap(data,null); }
                    else { UpdateEmpvisit(data); }
                    Emp.Visit = data;
                }
                //CvCustomer.ItemsSource = null;
                //CvCustomer.ItemsSource = EmpList;
            }
            catch (Exception ex)
            {
                var exc = ex.Message;
            }
        }
        async void UpdateEmpvisit(Models.VisitData data)
        {
            foreach (var p in map.Pins)
            {
                if (p.Label.Equals(data.Empfullname))
                {
                    await PlotMap(data, p);
                }
            }

            //pin.Position = new Position(data.Lat, data.Lng
        }
        async Task<bool> SetMap(List<Models.Empvisit> datas)
        {
            try
            {
                if(datas!=null && datas.Count > 0)
                {
                    double Lat = 0;
                    double Lng = 0;

                    foreach (var dr in datas)
                    {
                        if (dr.Visit != null)
                        {
                            if (!(dr.Visit.Lat == 0 || dr.Visit.Lng == 0))
                            {
                                await PlotMap(dr.Visit);
                                if (Lat == 0 || Lng == 0) { Lat = dr.Visit.Lat; Lng = dr.Visit.Lng; }
                            }
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
                    map.PinClicked += (s, e) => { ShowEmployeePin(s, e); };
                    return await Task.FromResult(true);
                }
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Create CandidateBP SetMap Error", ex.Message, "OK");
            }
            return await Task.FromResult(false);
        }
        async Task<bool> PlotMap(Models.VisitData data,Pin selectedpin = null)
        {
            try
            {
                bool newpin = (selectedpin == null);
                if (newpin) { selectedpin = new Pin();}
                selectedpin.Type = PinType.Place;
                selectedpin.Position = new Position(data.Lat, data.Lng);
                selectedpin.Label = data.Empfullname;
                selectedpin.Address = data.Custname;
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
        void ShowEmployeePin(object sender, PinClickedEventArgs e)
        {
            Pin item = e.Pin;
            Models.VisitData data = (Models.VisitData)item.BindingContext;
            List<Models.VisitData> datas = (List<Models.VisitData>) CvCustomer.ItemsSource;
            int index = datas.FindIndex(x => x.Empid == data.Empid);
            if (index >= 0) { CvCustomer.Position = index; }
        }
        private void CvCustomer_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            //Models.CustomerData previousItem = (Models.CustomerData)e.PreviousItem ;
            ActiveVisit = (Models.Empvisit)e.CurrentItem;
            double Lat = ActiveVisit.Visit.Lat;
            double Lng = ActiveVisit.Visit.Lng;
            if (!(Lat == 0 || Lng == 0))
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Lat, Lng), Distance.FromKilometers(10)));
            }
            else
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(13.745262, 100.551273), Distance.FromKilometers(10)));
                this.DisplayToastAsync("ไม่มีข้อมูลตำแหน่ง" + ActiveVisit.Fullname, 5000);
            }
        }

        private async void Navigator_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Xamarin.Essentials.Map.OpenAsync(ActiveVisit.Visit.Lat, ActiveVisit.Visit.Lng, new Xamarin.Essentials.MapLaunchOptions
                {
                    Name = ActiveVisit.Fullname,
                    NavigationMode = Xamarin.Essentials.NavigationMode.Driving,
                });
            }
            catch
            {
                await DisplayAlert("แจ้งเตือน", "ไม่สามารถเปิดใช้งานแผนที่ได้ \n โปรดตรวจสอบการใช้งานตำแหน่ง", "ตกลง");
            }
        }

        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<EmployeeToday, bool>(this, "OpenMenu", true);
        }
        private void Btnback_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }

        async void GotoOwnerPage(bool success = false)
        {
            MessagingCenter.Send<EmployeeToday, bool>(this, OwnerPage, success);
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