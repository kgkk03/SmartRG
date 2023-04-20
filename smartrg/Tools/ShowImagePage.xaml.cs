using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowImagePage : ContentPage
    {

        string OwnerPage = "ShowImagePage";
        string ImgBase64 = "";
        public ShowImagePage()
        {
            InitializeComponent();
        }
        public void Setdata(string ownerpage, string Title, bool canedit = false, ImageSource value = null)
        {
            OwnerPage = ownerpage;
            LblHeader.Text = Title;
            BtnSave.IsVisible = canedit;
            if (value != null) { MyImage.Source = value; }
            ImgBase64 = "";
        }
        private void Btnback_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }

        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(true);
        }
        async void GotoOwnerPage(bool success = false)
        {
            string result = "";
            if (success) { result = ImgBase64; }
            MessagingCenter.Send<ShowImagePage, string>(this, OwnerPage, result);
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