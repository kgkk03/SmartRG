using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerPage : ContentPage
    {
        Models.CustomerData ActiveCustomer = new Models.CustomerData();
        public Models.Imagedata ActiveImage = null;
        string OwnerPage = "CustomerPage";
        public CustomerPage()
        {
            InitializeComponent();
        }

        public  void Setdata(string ownerpage, Models.CustomerData data)
        {
            OwnerPage = ownerpage;
            ActiveCustomer = data;
            ShowData();

        }

        void ShowData()
        {
            try {
                LblName.Text = ActiveCustomer.Custname == null ? "" : ActiveCustomer.Custname;
                LblType.Text = ActiveCustomer.Typename == null ? "" : ActiveCustomer.Typename;
                LblCode.Text = ActiveCustomer.Custcode == null ? "" : ActiveCustomer.Custcode;
                LblAddress.Text = ActiveCustomer.Custaddress == null ? "" : ActiveCustomer.Custaddress;

                //LblCustdetailHeader.Text = ActiveCustomer.Custname;
                ImgCustype.Source = ActiveCustomer.Icon == null ? "avatar" : ActiveCustomer.Icon;
                LblCustType.Text = ActiveCustomer.Typename == null ? "" : ActiveCustomer.Typename;
                LblCustPhone.Text = ActiveCustomer.Custphone == null ? "" : ActiveCustomer.Custphone;
                LblCustAddress.Text = ActiveCustomer.Custaddress == null ? "" : ActiveCustomer.Custaddress;
                Lbllocation.Text = (ActiveCustomer.Lat.ToString("0.000000") + ", " + ActiveCustomer.Lng.ToString("0.000000"));

                //lblContractHeader.Text = ActiveCustomer.Contractname;

                LblContractName.Text = ActiveCustomer.Contractname == null ? "" : ActiveCustomer.Contractname;
                LblContractPhone.Text = ActiveCustomer.Contractmobile == null ? "" : ActiveCustomer.Contractmobile;

                if (ActiveImage == null) { SetCustomerImage(); }
            }
            catch { }
        }

        async void SetCustomerImage()
        {
            Models.CustImage custimg = App.dbmng.sqlite.Table<Models.CustImage>().Where(x => x.Key.Equals(ActiveCustomer.Key)).FirstOrDefault();
            if (custimg == null) {
                custimg = await App.Ws.GetCustomerImage(ActiveCustomer.Key);
                if (custimg == null)
                {
                    ActiveImage = new Models.Imagedata() { canedit = true, Image = "avatar", Imagefile = "Customer" };

                }
                else { 
                    App.dbmng.InsetData(custimg);
                    ActiveImage = ConvertImage(custimg);
                }
            }
            else { ActiveImage = ConvertImage(custimg);}
            ImgCustomer.Source = ActiveImage.Image;
        }

        Models.Imagedata ConvertImage(Models.CustImage custimg)
        {
            Models.Imagedata result = new Models.Imagedata() { canedit = true, Image = null, Imagefile = "Customer" };
            if (custimg != null) { result.Image = Helpers.ImageConvert.ImageFB64(custimg.Icon); }
            return result;
        }

        private async  void BtnCheckin_Clicked(object sender, EventArgs e)
        {
            string msg = "คุณต้องการเข้าพบลูกค้า \n" + ActiveCustomer.Custname;
            if (await DisplayAlert("ยืนยัน", msg, "เข้าพบลูกค้า", "ไม่ต้องการ"))
            {
                AidWaitingRun(true, "ตรวจสอบข้อมูลตำแหน่ง...");
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
                App.Checkinlocation = await Geolocation.GetLocationAsync(request);
                AidWaitingRun(true, "ได้ตำแหน่งจีพีเอสแล้ว");
                if (App.Checkinlocation == null && App.Checkinlocation.Latitude <= 0)
                {
                    await DisplayAlert("แจ้งเตือน", "ไม่พบสัญญาณจีพีเอส กรุณาตรวจสอบ", "ยกเลิก");
                    AidWaitingRun(false);
                    return;
                }
                MessagingCenter.Send<CustomerPage, Models.CustomerData>(this, OwnerPage, ActiveCustomer);
            }
        }

        private async void BtnNavigator_Clicked(object sender, EventArgs e)
        {

            try {
                await Map.OpenAsync(ActiveCustomer.Lat , ActiveCustomer.Lng, new MapLaunchOptions
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

        private void BtnPhone_Clicked(object sender, EventArgs e)
        {
            try { PhoneDialer.Open(ActiveCustomer.Custphone); } 
            catch {
                DisplayAlert("แจ้งเตือน", "ไม่สามารถใช้งานเบอร์โทรนี้ได้ \n โปรดตรวจสอบหมายเลข", "ตกลง");
            }
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