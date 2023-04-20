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
    public partial class EditSelectPage : ContentPage
    {

        string OwnerPage = "EditSelectPage";
        Models.SelectObj<object> ReturnValue = new Models.SelectObj<object>();
        public EditSelectPage()
        {
            InitializeComponent();
        }
        public void Setdata(string ownerpage, string Title, List<Models.SelectObj<object>> datas,string displayvalue = "")
        {
            OwnerPage = ownerpage;
            LblHeader.Text = Title;
            ListData.ItemsSource = datas;
            if (!displayvalue.Equals(""))
            {
                int index = datas.FindIndex(x => x.Display.Equals(displayvalue));
                if (index != -1) {
                    ReturnValue = datas[index];
                    //ListData.SelectedItem = ReturnValue;
                }
            }
        }

        
        
        private void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Models.SelectObj<object> data = (Models.SelectObj<object>)e.CurrentSelection.FirstOrDefault();
            ReturnValue = data;
            GotoOwnerPage(true);
            //ListData.SelectedItem = null;
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
            Models.SelectObj<object> result = null;
            if (success) { result = ReturnValue; }
            MessagingCenter.Send<EditSelectPage, Models.SelectObj<object>>(this, OwnerPage, result);
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