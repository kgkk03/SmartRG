using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace smartrg.Picking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabReturnPage : Xamarin.Forms.TabbedPage
    {
        public int Pageindex = 1;
        string OwnerPage = "TabReturnPage";
        string Mypage = "TabReturnPage";
        public Models.PickingData ActiveData;
        public List<Models.PickingLineData> AllPickingLine;
        public Models.CustomerData Customer;
        public TabReturnPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            InitializeComponent();
            this.CurrentPageChanged += CurrentPageHasChanged;
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
        async void Setpage(int index, Xamarin.Forms.TabbedPage MT)
        {
            try
            {
                if (index == 0)
                {
                    if (ActiveData.Transtatus == -1)
                    {
                        bool result = false;
                        result = await DisplayAlert("", "รายการคืนสินค้ายังไม่ได้บันทึก \n ต้องการออกจากหน้านี้โดยไม่บันทึกใช่หรือไม่", "ออกโดยไม่บันทึก", "ไม่ออก");
                        if (!result)
                        {
                            this.CurrentPage = this.Children[Pageindex];
                            return;
                        }
                        
                    }
                    GotoOwnerPage(null);
                }
                else if (index == 1)
                {
                    ReturnPage tab = (ReturnPage)MT.CurrentPage;
                    tab.ShowData(Mypage, this);
                }
                else if (index == 2)
                {
                    ReturnPickingPage tab = ( ReturnPickingPage)MT.CurrentPage;
                    tab.ShowData(Mypage, this);
                }
                else if (index == 3)
                {
                    ReturnSalePage tab = (ReturnSalePage)MT.CurrentPage;
                    tab.ShowData(Mypage, this);
                }

                Pageindex = index;
            }
            catch (Exception ex) { await DisplayAlert("TabReturnPage Setpage Error", ex.Message, "OK"); }
        }
        public void ShowData(string ownerpage, Models.PickingData data)
        {
            OwnerPage = ownerpage;
            ActiveData = data;
            this.CurrentPage = this.Children[1];
        }

        public void SetNewData(string ownerpage, Models.CustomerData customer)
        {
            OwnerPage = ownerpage;
            Customer = customer;
            this.CurrentPage = this.Children[1];
            var page = (ReturnPage)this.CurrentPage ;
            page.SetNewData(Mypage, this);
            ReturnPageMessage();
        }
        void ReturnPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<ReturnPage, Models.PickingData>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try
                        {
                            MessagingCenter.Unsubscribe<ReturnPage, Models.PickingData>(this, Mypage);
                            if (arg != null) { GotoOwnerPage(arg); }
                        }
                        catch { }
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("TabReturnPage MessagingCenter Error", ex.Message, "OK"); }
        }

        public async void GotoOwnerPage(Models.PickingData data)
        {
            try
            {
                MessagingCenter.Send<TabReturnPage, Models.PickingData>(this, OwnerPage, data);
                await Navigation.PopModalAsync();
            }
            catch (Exception ex) { await DisplayAlert("TabReturnPage GotoOwnerPage Error", ex.Message, "OK"); }
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