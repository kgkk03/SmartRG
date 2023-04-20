using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace smartrg.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustTabPage : Xamarin.Forms.TabbedPage
    {
        public int Pageindex = 1;
        public Models.CustomerData Activedata = new Models.CustomerData();
        string OwnerPage = "CustTabPage";
        string Mypage = "CustTabPage";
        public string CustID = "";
        public CustTabPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            InitializeComponent();
            this.CurrentPageChanged += CurrentPageHasChanged;
        }
        public void Setdata(string ownerpage, Models.CustomerData data)
        {
            OwnerPage = ownerpage;
            Activedata = data;
        }
        private void CurrentPageHasChanged(object sender, EventArgs e)
        {
            try
            {
                var MT = sender as Xamarin.Forms.TabbedPage;
                var indexpage = MT.Children.IndexOf(MT.CurrentPage);
                Setpage(indexpage, MT);
            }
            catch (Exception ex) { DisplayAlert("VisitTabPage CurrentPageHasChanged Error", ex.Message, "OK"); }
        }
        void Setpage(int index, Xamarin.Forms.TabbedPage MT)
        {
            try
            {
                if (index == 0)
                {
                    GotoOwnerPage(null);
                }
                else if (index == 1)
                {
                    CustomerPage tab = (CustomerPage)MT.CurrentPage;
                    tab.Setdata(Mypage, Activedata);
                    CustomerPageMessage();
                }
                else if (index == 2)
                {
                    //CustomervisitPage tab = (CustomervisitPage)MT.CurrentPage;
                    //tab.Setdata(Mypage, Activedata);
                }
                else if (index == 3)
                {
                    //CustomerStockPage tab = (CustomerStockPage)MT.CurrentPage;
                    //tab.Setdata(Mypage, Activedata);
                }

                Pageindex = index;
            }
            catch (Exception ex) { DisplayAlert("VisitTabPage Setpage Error", ex.Message, "OK"); }
        }
        void CustomerPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<CustomerPage, Models.CustomerData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try
                        {
                            MessagingCenter.Unsubscribe<CustomerPage, Models.CustomerData>(this, Mypage);
                            if (arg != null) { GotoOwnerPage(arg); }
                        }
                        catch { }
                    });
                   
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        public void SetListPage(Models.CustomerPage selectedpage)
        {
            //Visittab.Children.Clear();
            var page = Helpers.Controls.GetSubPage(selectedpage.Pageid);
            int index = MainTabpage.Children.Count;
            MainTabpage.Children.Add(page);
            MainTabpage.Children[index].Title = selectedpage.Title;
            MainTabpage.Children[index].IconImageSource = selectedpage.Icon;
        }
        public void ShowPage()
        {
            this.CurrentPage = this.Children[1];
            //Setpage(0, this);
        }
        public async void GotoOwnerPage(Models.CustomerData cust)
        {
            try
            {
                MessagingCenter.Send<CustTabPage, Models.CustomerData>(this, OwnerPage, cust);
                await Navigation.PopModalAsync();
            }
            catch (Exception ex) { await DisplayAlert("VisitTabPage GotoOwnerPage Error", ex.Message, "OK"); }
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
            catch (Exception ex) { DisplayAlert("BookingTabPage OnBackButtonPressed Error", ex.Message, "OK"); }
            return true;
        }

    }
}