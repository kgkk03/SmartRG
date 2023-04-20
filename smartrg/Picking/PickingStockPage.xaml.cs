using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Picking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickingStockPage : ContentPage
    {
        bool GroupbyAgent = false;
        List<Models.PickingLineData> StockData = new List<Models.PickingLineData>();
        public PickingStockPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        public async void SetData(List<Models.PickingLineData> data)
        {
            AidWaitingRun(true, "กำลังค้นหาข้อมูลสินค้าในสต็อก...");
            StockData = data;
            await ShowData();
            AidWaitingRun(false);
        }
        private async void BtnOption_Clicked(object sender, EventArgs e)
        {
            GroupbyAgent = !GroupbyAgent;
            await ShowData();
        }
        async Task<bool> ShowData()
        {
            try {
                AidWaitingRun(true,"กำลังค้นหาข้อมูล...");
                List<ProductGroup> groupdata = new List<ProductGroup>();
                if (GroupbyAgent)
                {
                    BtnOption.Source = "ic_shop";
                    LblHeader.Text = "สินค้าคงเหลือ (รายตัวแทน)";
                    var listagent = StockData.GroupBy(x => x.Agentname)
                            .Select(group => new { Header = group.Key, Total = group.Sum(x => x.Balance), Items = group.ToList() })
                            .ToList();
                    foreach (var dr in listagent)
                    {
                        groupdata.Add(new ProductGroup(dr.Header, dr.Total, dr.Items));
                    }
                }
                else {
                    BtnOption.Source = "ic_liststock";
                    LblHeader.Text = "สินค้าคงเหลือ (รายสินค้า)";

                    var listproduct = StockData.GroupBy(x => x.Productname)
                                     .Select(group => new { Header = group.Key, Total = group.Sum(x => x.Balance), Items = group.ToList() })
                                     .ToList();
                    foreach (var dr in listproduct)
                    {
                        groupdata.Add(new ProductGroup(dr.Header, dr.Total, dr.Items));
                    }
                }
                ListData.ItemsSource = groupdata;
                ListData.IsGrouped = true;
                AidWaitingRun(false);
                return await Task.FromResult(true);
            }
            catch { }
            AidWaitingRun(false);
            return await Task.FromResult(false);
        }

        private void BtnExit_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
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

    public class ProductGroup : List<Models.PickingLineData>
    {
        public string Name { get; private set; }
        public int Total { get; private set; }


        public ProductGroup(string name, int total, List<Models.PickingLineData> data) : base(data)
        {
            Name = name;
            Total = total;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}