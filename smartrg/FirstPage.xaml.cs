using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FirstPage : ContentPage
    {
        public FirstPage()
        {
            InitializeComponent();
            lblVersion.Text = "Ver. " + VersionTracking.CurrentVersion;
            App.dbmng = new Helpers.Dbmanager();
            CheckPemiison();
        }
        async public void CheckPemiison()
        {
            try
            {
                var res = await Helpers.Permission.CheckPemiison<Permissions.StorageWrite>();
                if (res)
                {
                    string packageName = DependencyService.Get<Helpers.IPackageName>().PackageName;
                    bool result = DependencyService.Get<Helpers.ICallService>().StoragerageService();
                    if (result)
                    {
                        //DependencyService.Get<Helpers.ICallService>().SetDirectory(App.Folder);
                        var gps = await Helpers.Permission.CheckPemiison<Permissions.LocationAlways>();
                        StartPage();
                    }
                    else
                    {
                        await DisplayAlert("แจ้งเตือน !", "คุณยังไม่อนุญาตการใช้งานพื้นที่จัดเก็บข้อมูล \nกรุณาตรวจสอบารอนุญาตสิทธิ์การใช้งานพื้นที่จัดเก็บ", "ตกลง");
                        App.Current.MainPage = new Profile.LoginPage();
                    }
                }
                else
                {
                    await DisplayAlert("แจ้งเตือน !", "คุณยังไม่อนุญาตการใช้งานพื้นที่จัดเก็บข้อมูล \nกรุณาตรวจสอบารอนุญาตสิทธิ์การใช้งานพื้นที่จัดเก็บ", "ตกลง");
                    App.Current.MainPage = new Profile.LoginPage();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("FirstPage CheckPemiison  Error", ex.Message, "OK");
            }
        }
        void StartPage()
        {
            try
            {
                Device.StartTimer(TimeSpan.FromMilliseconds(500), () => { GotoStartPage(); return false; });
            }
            catch (Exception ex) { DisplayAlert("FirstPage StartPage Error", ex.Message, "OK"); }
        }
        async void GotoStartPage()
        {
            try
            {
                App.Current.MainPage = new Profile.LoginPage();
                //App.Current.MainPage = new Product.PromotionImgPage();

            }
            catch (Exception ex) { await DisplayAlert("FirstPage GotoStartPage Error", ex.Message, "OK"); }
        }


    }
}