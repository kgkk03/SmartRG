using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoPage : ContentPage
    {
        public NoPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }
        public NoPage(string header)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            LblHeader.Text = header;
        }
        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<NoPage, bool>(this, "OpenMenu", true);
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