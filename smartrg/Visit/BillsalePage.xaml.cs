using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Visit
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BillsalePage : ContentPage
    {
        string OwnerPage = "BillsalePage";
        string Mypage = "BillsalePage";
        Models.VisitShowpageData Activedata = null;

        public BillsalePage()
        {
            InitializeComponent();
        }
    
        public  void Setdata(Models.VisitShowpageData data, string ownerpage)
        {
            AidWaitingRun(true);
            OwnerPage = ownerpage;
            if (Activedata == null)
            {
                Activedata = data;
                if (Activedata.BillSale == null)
                {
                    //Activedata.VisitBillSale = await Helpers.SampleData.GetListofVisitBillSale(Activedata.Visitdata.Key);
                }
                ListData.ItemsSource = Activedata.BillSale;
            }
            Activedata = data;
            LblCustname.Text = Activedata.Customer.Custname + " (" + Activedata.Customer.Custgroupname + ")";
            ImgCustomer.Source = Activedata.Customer.Icon;
            LblAddress.Text = Activedata.Customer.Custaddress;
            LblWorksumary.Text = Activedata.Visitdata.Key;
            ShowHeader();
            AidWaitingRun(false);
        }
        private void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Models.VisitBillSale item = (Models.VisitBillSale)e.CurrentSelection.FirstOrDefault();
                if (item == null) { return; }
                int index = Activedata.BillSale.FindIndex(x => x.Productid.Equals(item.Productid));
                EditBillSaleData(index);
            }
            catch { }
        }
        async void EditBillSaleData(int index)
        {
            var page = new Tools.KeybordBillsalePage();
            page.Setproduct(Activedata.BillSale, index, Mypage);
            await Navigation.PushModalAsync(page);
            KeybordBillsalePageMessage();
        }
        void KeybordBillsalePageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.KeybordBillsalePage, List<Models.ShowVisitBillSale>>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { ShowData(arg); } catch { }
                        MessagingCenter.Unsubscribe<Tools.KeybordBillsalePage, List<Models.ShowVisitBillSale>>(this, Mypage);
                    });
                });


            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        void UpdateData(Models.ShowVisitBillSale saledata)
        {
            try
            {
                Activedata.BillSale.Add(saledata);
                ListData.ItemsSource = null;
                ListData.ItemsSource = Activedata.BillSale;
                ShowHeader();
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }

        }

        void ShowData(List<Models.ShowVisitBillSale> listdata)
        {
            try
            {
                Activedata.BillSale = listdata;
                ListData.ItemsSource = null;
                ListData.ItemsSource = listdata;
                ShowHeader();
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }

        }
        void ShowHeader()
        {
            try
            {
                if (Activedata.BillSale != null && Activedata.BillSale.Count > 0)
                {
                    var data = Activedata.BillSale;
                    int total = data.Where(x => x.Amount > 0).Count();
                    int sale = data.Where(x => x.Amount > 0).Sum(x => x.Total); ;
                    double amount = data.Sum(x => x.Amount);

                    LblWorksumary.Text = amount.ToString("#,##0.00") + " บาท";
                    string detail = "จำนวน " + data.Count().ToString() + " รายการ, ";
                    detail += "ขาย " + total.ToString() + " รายการ, ";
                    detail += "รวม " + sale.ToString() + " ขวด";
                    LblWorkdetails.Text = detail;
                    return;
                }
            }
            catch { }
            LblWorksumary.Text = "0.00 บาท";
            LblWorkdetails.Text = "";

        }

        void AidWaitingRun(bool running, string msg = "")
        {
            try
            {
                if (running && msg.Equals("")) { msg = "กำลังตรวจสอบข้อมูล..."; }
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