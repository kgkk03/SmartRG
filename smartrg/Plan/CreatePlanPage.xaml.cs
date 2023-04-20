using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Plan
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePlanPage : ContentPage
    {
        public CreatePlanPage()
        {
            InitializeComponent();
            Webview();
        }

        async void Webview()
        {
            try
            {
                AidWaitingRun(true);
                string loginname = App.loginname;
                string password = App.password;
                string page = "createplan";
                string team = App.UserProfile.Teamid.ToString();

                var url = "http://rg.bttracking.com/#/mobile?login=" + loginname + "&password=" + password + "&page=" + page + "&team=" + team;
                //var url = "http://192.168.1.79:8016/#/mobile?login=" + loginname + "&password=" + password + "&page=" + page + "&team=" + team;
                //var url = "http://192.168.1.79:8016/#/mobile?login=yenwatana@gmail.com&password=52C69E3A57331081823331C4E69D3F2E&page=createplan&team=2";
                //var url = "http://192.168.1.79:9016/";

                webCreate.Source = url;
                AidWaitingRun(false);
            }
            catch (Exception ex)
            {
                AidWaitingRun(false);
                var message = ex.Message;
                await DisplayAlert("แจ้งเตือน", message, "OK");
            }
        }
        async Task<bool> AidWaitingRun(bool running, string msg = "")
        {
            try
            {
                LblStatus.Text = msg;
                Stk_AidWaitingBk.IsVisible = running;
                Stk_AidWaiting.IsVisible = running;
                AidWaiting.IsVisible = running;
                AidWaiting.IsRunning = running;
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("AidWaitingRun Error", ex.Message, "OK");
                return await Task.FromResult(false);

            }
        }

    }
}