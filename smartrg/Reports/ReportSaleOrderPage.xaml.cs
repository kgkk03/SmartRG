using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Reports
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    
    public partial class ReportSaleOrderPage : ContentPage
    {
        public ReportSaleOrderPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Showdata();
        }

        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<ReportSaleOrderPage, bool>(this, "OpenMenu", true);
        }

        void Showdata()
        {

            LblReportHeader.Text = Helpers.Controls.Date2ThaiString(App.Servertime.AddDays(-7), "d-M-yyyy") + " ถึง " + Helpers.Controls.Date2ThaiString(App.Servertime, "d-M-yyyy");
            //List<Models.ReportSOsumary> listofdata = await Helpers.SampleData.GetListofReportSOsumary();
            //ListData.ItemsSource = listofdata;
        }
        private void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Models.ReportSOsumary item = (Models.ReportSOsumary)e.CurrentSelection.FirstOrDefault();
                if (item == null) { return; }
                //if (IsEdit) { return; }
                //IsEdit = true;
                //var page = new Booking.BookingPage();
                //page.Setdata(Mypage, new Models.BookingData());
                //await Navigation.PushModalAsync(page);

            }
            catch
            {
                //IsEdit = false; 
            }
        }

        async void GotoOwnerPage(bool success = false)
        {
            //Models.UserData result = null;
            //if (success) { result = ActiveUser; }
            //MessagingCenter.Send<ReportSaleOrderPage, Models.UserData>(this, OwnerPage, result);
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