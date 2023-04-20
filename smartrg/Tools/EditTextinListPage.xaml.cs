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
    public partial class EditTextinListPage : ContentPage
    {

        string OwnerPage = "EditTextinListPage";
        string ReturnValue = "";
        public EditTextinListPage()
        {
            InitializeComponent();
        }
        public void Setdata(string ownerpage, string Title, List<string> data, string value = "")
        {
            OwnerPage = ownerpage;
            LblHeader.Text = Title;
            PikInput.Title = Title;
            PikInput.ItemsSource = data;
            ReturnValue = value;
        }

        private void PikInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                ReturnValue = (string)picker.ItemsSource[selectedIndex];
            }
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
            if (success) { result = ReturnValue; }
            MessagingCenter.Send<EditTextinListPage, string>(this, OwnerPage, result);
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