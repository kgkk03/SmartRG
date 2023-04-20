using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace smartrg.Visit
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisitTabPage : Xamarin.Forms.TabbedPage
    {
        public int Pageindex = 0;
        public Models.VisitShowpageData Activedata = new Models.VisitShowpageData();
        string OwnerPage = "VisitTabPage";
        string Mypage = "VisitTabPage";
        public VisitTabPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            InitializeComponent();
            VisitSumaryPageMessage();
            this.CurrentPageChanged += CurrentPageHasChanged;
        }
        void VisitSumaryPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<VisitSumaryPage, bool>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { GotoOwnerPage(true); } catch { }
                        MessagingCenter.Unsubscribe<VisitSumaryPage, bool>(this, Mypage);
                    });
                });


            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        public void Setdata(string ownerpage, Models.VisitShowpageData data)
        {
            OwnerPage = ownerpage;
            Activedata = data;
        }
        public void ShowPage()
        {
            try
            {
                this.CurrentPage = this.Children[0];

            }
            catch (Exception ex)
            {
                DisplayAlert("Error" , ex.Message , "OK");
            }
        }

        public void SetListPage(Models.VisitPage selectedpage)
        {
            var page = Helpers.Controls.GetSubPage(selectedpage.Pageid);
            int index = MainTabpage.Children.Count;
            MainTabpage.Children.Add(page);
            MainTabpage.Children[index].Title = selectedpage.Title;
            MainTabpage.Children[index].IconImageSource = selectedpage.Icon;
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
        void Setpage(int tapindex, Xamarin.Forms.TabbedPage MT)
        {
            try
            {
                int index = Activedata.VisitPage[tapindex].Pageid;

                //VisitSumaryPage
                //if (ID == 1100) { return new Visit.VisitSumaryPage(); }
                if (index == 1100)
                {
                    VisitSumaryPage tab = (VisitSumaryPage)MT.CurrentPage;
                    tab.Setdata(Activedata, Mypage);
                }

                ////QuestionPage
                //else if (ID == 1101) { return new Visit.QuestionPage(); }
                else if (index == 1101)
                {
                    QuestionPage tab = (QuestionPage)MT.CurrentPage;
                    tab.Setdata(Activedata);
                }

                ////CVSStockPage
                //else if (ID == 1102) { return new Visit.CVSStockPage(); }
                else if (index == 1102)
                {
                    VisitStock.CVSStockPage tab = (VisitStock.CVSStockPage)MT.CurrentPage;
                    tab.Setdata(Activedata);
                }

                ////HyperStockPage
                //else if (ID == 1103) { return new Visit.HyperStockPage(); }
                else if (index == 1103)
                {
                    VisitStock.HyperStockPage tab = (VisitStock.HyperStockPage)MT.CurrentPage;
                    tab.Setdata(Activedata);
                }

                ////AgentStockPage
                //else if (ID == 1104) { return new Visit.VisitSumaryPage(); }
                else if (index == 1104)
                {
                    VisitStock.ShopStockPage tab = (VisitStock.ShopStockPage)MT.CurrentPage;
                    tab.Setdata(Activedata);
                }

                ////ShopStockPage
                //else if (ID == 1105) { return new Visit.VisitSumaryPage(); }
                else if (index == 1105)
                {
                    VisitStock.ShopStockPage tab = (VisitStock.ShopStockPage)MT.CurrentPage;
                    tab.Setdata(Activedata);
                }

                ////AgentPickingPage
                //else if (ID == 1106) { return new Visit.VisitSumaryPage(); }
                else if (index == 1106)
                {
                    VisitHeaderPage tab = (VisitHeaderPage)MT.CurrentPage;
                    tab.Setdata(Activedata);
                }
                ////AgentReturnPage
                //else if (ID == 1107) { return new Visit.VisitSumaryPage(); }
                else if (index == 1107)
                {
                    VisitHeaderPage tab = (VisitHeaderPage)MT.CurrentPage;
                    tab.Setdata(Activedata);
                }
                ////CashSalePage
                //else if (ID == 1108) { return new Visit.VisitSumaryPage(); }
                else if (index == 1108)
                {
                    VisitHeaderPage tab = (VisitHeaderPage)MT.CurrentPage;
                    tab.Setdata(Activedata);
                }
                ////SOPage
                //else if (ID == 1109) { return new Visit.VisitSumaryPage(); }
                else if (index == 1109)
                {
                    VisitHeaderPage tab = (VisitHeaderPage)MT.CurrentPage;
                    tab.Setdata(Activedata);
                }
                ////BillsalePage
                //else if (ID == 1110) { return new Visit.BillsalePage(); }
                else if (index == 1110)
                {
                    BillsalePage tab = (BillsalePage)MT.CurrentPage;
                    tab.Setdata(Activedata,Mypage);
                }

                Pageindex = tapindex;
            }
            catch (Exception ex) { DisplayAlert("VisitTabPage Setpage Error", ex.Message, "OK"); }
        }
        public async void GotoOwnerPage(bool success)
        {
            try
            {
                Models.VisitShowpageData result = null;
                if (success) { 
                    result = Activedata;
                    await Helpers.Controls.ClearLastvisit(Activedata.Visitdata.Key);
                }
                MessagingCenter.Send<VisitTabPage, Models.VisitShowpageData>(this, OwnerPage, result);
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