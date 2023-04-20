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
    public partial class EditTextPage : ContentPage
    {

        string OwnerPage = "EditTextPage";
        public EditTextPage()
        {
            InitializeComponent();
        }
        public void Setdata(string ownerpage, string Title, Keyboard Keyboardtype, string Placeholder, string value = "")
        {
            OwnerPage = ownerpage;
            LblHeader.Text = Title;
            TxtInput.Keyboard = Keyboardtype;
            TxtInput.Placeholder = Placeholder;
            TxtInput.Text = value;
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
            if (success) { result = TxtInput.Text; }
            MessagingCenter.Send<EditTextPage, string>(this, OwnerPage, result);
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