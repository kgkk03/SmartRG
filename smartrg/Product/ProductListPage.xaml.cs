using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Product
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
  
    public partial class ProductListPage : ContentPage
    {
        bool Searching = false;
        public ProductListPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Showdata("");
        }
        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<ProductListPage, bool>(this, "OpenMenu", true);
        }
        async void Showdata(string keyword)
        {
            if (Searching) { return; }
            Searching = true;
            AidWaitingRun(true, "กำลังค้นหาข้อมูลสินค้า");
            List<Models.ProductData> listofdata = await App.Ws.GetProductList(keyword) ;
            ListData.ItemsSource = listofdata;
            AidWaitingRun(false, "กำลังค้นหาข้อมูลสินค้า");
            Searching = false;
        }
        private void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Models.ProductData item = (Models.ProductData)e.CurrentSelection.FirstOrDefault();
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
        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            Showdata(TxtSearch.Text);
        }

        async void GotoOwnerPage(bool success = false)
        {
            //Models.UserData result = null;
            //if (success) { result = ActiveUser; }
            //MessagingCenter.Send<ProductListPage, Models.UserData>(this, OwnerPage, result);
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

        private async void BtnImageview_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PromotionImgPage());
        }
    }
}