using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace smartrg
{
    public partial class App : Application
    {
        public static string Imagepath = "";
        public static ImageSource Logo = "logo";
        public static double CheckLocation = 0.5;
        public static DateTime Servertime = new DateTime() ; 
        public static Helpers.Dbmanager dbmng { get; set; }
        public static Models.UserProfile UserProfile = new Models.UserProfile();
        public static List<Models.MenuList> Listmenu = new List<Models.MenuList>();

        public static Location Checkinlocation;
        public static Helpers.Ws Ws { get; set; } = new Helpers.Ws();
        
        //public static string MqttIP = "34.87.93.175";
        //public static int MqttPort = 7901;

        public static string MqttIP = "35.240.240.96";
        public static int MqttPort = 9000;
        public static string Imei = "";
        static int Difftime = 0;

        public static string loginname = "";
        public static string password = "";
        public static string PromotionURL = "http://rg.bttracking.com/#/mobile?";


        public App()
        {
            InitializeComponent();
            SetServertime();
            MainPage = new FirstPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        void RunServertime()
        {
            try
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    Servertime = Servertime.AddSeconds(1);
                    int gap = Convert.ToInt32(Math.Floor((Servertime - DateTime.Now).TotalMinutes));
                    if (Difftime == gap) { RunServertime(); }
                    else { SetServertime(); }
                    return false;
                });
            }
            catch (Exception ex) { App.dbmng.InsertLog("runservertime", ex.Message); }
        }
        async void SetServertime()
        {
            try {
                Servertime = await App.Ws.GetServertime();
                Difftime = Convert.ToInt32(Math.Floor((Servertime - DateTime.Now).TotalMinutes));
            }
            catch { }
            RunServertime();
        }
       
    }
}
