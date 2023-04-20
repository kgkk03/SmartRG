using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.VisitStock
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShopStockPage : ContentPage
    {
        Models.VisitShowpageData Activedata = null;
        bool IsEdit = false;

        public ShopStockPage()
        {
            InitializeComponent();
        }

        public async void Setdata(Models.VisitShowpageData data)
        {
            AidWaitingRun(true);
            IsEdit = true;
            if (Activedata == null)
            {
                Activedata = data;
                LblCustname.Text = Activedata.Customer.Custname + " (" + Activedata.Customer.Custgroupname + ")";
                ImgCustomer.Source = Activedata.Customer.Icon;
                LblAddress.Text = Activedata.Customer.Custaddress;
                LblWorksumary.Text = Activedata.Visitdata.Key;
                if (!Activedata.Detail.Newvisit) { Activedata.Detail.VisitStockSuccess = true; }
            }
            if (Activedata.Stock == null)
            {
                if (Activedata.Detail.Newvisit) {
                    Activedata.Stock = await Helpers.Controls.GetNewVisitstok(Activedata.Visitdata.Key, Activedata.Customer);
                }
                else
                {
                    Activedata.Stock = await Helpers.Controls.GetVisitstokLog(Activedata.Visitdata.Key, Activedata.Customer.Custgroupid);
                }
            }

            if (Activedata.Stock != null)
            {
                List<Models.VisitStockShop> listvisit = await Helpers.Controls.GetStockList(Activedata.Stock, !Activedata.Detail.VisitStockSuccess);
                ListData.ItemsSource = listvisit;
            }
            ShowHeader();
            IsEdit = false;
            AidWaitingRun(false);

        }
        
       

       

        void ShowHeader()
        {
            var data = Activedata.Stock;
            if (data != null && data.Count > 0)
            {
                int total = data.Count;
                int sale = data.Count(x => x.Sale);
                int lost = data.Count(x => x.Sale && !x.Stock);
                int available = data.Count(x => x.Sale && x.Stock);
                string detail = "มีจำหน่าย " + sale.ToString();
                detail += ", ไม่มี " + (total - sale).ToString();
                detail += ", ของหมด " + lost.ToString();

                Activedata.Detail.StockCount = total.ToString();
                Activedata.Detail.StockHeader = detail;
            }
            LblWorksumary.Text = Activedata.Detail.StockHeader;
            BtnSend.IsVisible =  !Activedata.Detail.VisitStockSuccess;
        }

        private void BtnStock_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                ImageButton item = (ImageButton)sender;
                Models.VisitStockShop data = (Models.VisitStockShop)item.BindingContext;
                Grid stk = (Grid)item.Parent;
                ImageButton BtnStock = (ImageButton)stk.FindByName("BtnStock");
                ImageButton BtnLostStock = (ImageButton)stk.FindByName("BtnLostStock");
                ImageButton BtnUnSale = (ImageButton)stk.FindByName("BtnUnSale");
                data.Stock = true;
                BtnStock.Source = "ic_check";
                data.Data.Stock = true;
                data.Data.Sale = true;

                data.Lost = false;
                data.Unsale = false;
                BtnLostStock.Source = "ic_point";
                BtnUnSale.Source = "ic_point";

                var product = Activedata.Stock.Find(x => x.Key.Equals(data.Data.Key));
                if (product != null) { product = data.Data; }
                App.dbmng.InsetData(product);

            }
            catch { }
        }

        private void BtnLostStock_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                ImageButton item = (ImageButton)sender;
                Models.VisitStockShop data = (Models.VisitStockShop)item.BindingContext;
                Grid stk = (Grid)item.Parent;
                ImageButton BtnStock = (ImageButton)stk.FindByName("BtnStock");
                ImageButton BtnLostStock = (ImageButton)stk.FindByName("BtnLostStock");
                ImageButton BtnUnSale = (ImageButton)stk.FindByName("BtnUnSale");
                data.Lost = true;
                BtnLostStock.Source = "ic_check";

                data.Unsale = false;
                data.Stock = false;
                BtnStock.Source = "ic_point";
                BtnUnSale.Source = "ic_point";

                data.Data.Sale = true;
                data.Data.Stock = false;
                var product = Activedata.Stock.Find(x => x.Key.Equals(data.Data.Key));
                if (product != null) { product = data.Data; }
                App.dbmng.InsetData(product);

            }
            catch { }
        }

        private void BtnUnSale_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                ImageButton item = (ImageButton)sender;
                Models.VisitStockShop data = (Models.VisitStockShop)item.BindingContext;
                Grid stk = (Grid)item.Parent;
                ImageButton BtnStock = (ImageButton)stk.FindByName("BtnStock");
                ImageButton BtnLostStock = (ImageButton)stk.FindByName("BtnLostStock");
                ImageButton BtnUnSale = (ImageButton)stk.FindByName("BtnUnSale");
                Entry TxtPrice = (Entry)stk.FindByName("TxtPrice");
                data.Unsale = true;
                BtnUnSale.Source = "ic_check";

                data.Lost = false;
                data.Stock = false;
                data.Price = 0;
                TxtPrice.Text = data.Price.ToString();
                BtnStock.Source = "ic_point";
                BtnLostStock.Source = "ic_point";

                data.Data.Sale = false;
                data.Data.Stock = false;
                var product = Activedata.Stock.Find(x => x.Key.Equals(data.Data.Key));
                if (product != null) { product = data.Data; }
                App.dbmng.InsetData(product);

            }
            catch { }
        }

        private void TxtPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                Entry item = (Entry)sender;
                item.Text = (item.Text.ToString().Length > 9 ? "0" : item.Text);
                if (item.Text == null) { item.Text = "0"; }
                Models.VisitStockShop databinding = (Models.VisitStockShop)item.BindingContext;
                var data = Activedata.Stock.Find(x => x.Key.Equals(databinding.Data.Key));
                if (data != null) {
                    if (!data.Sale)
                    {
                        data.Price = 0;
                        databinding.Data.Price = 0;
                        item.TextColor = Color.Red;
                    }
                    else
                    {
                        data.Price = int.Parse(item.Text.Equals("")? "0" : item.Text);
                        databinding.Data.Price = data.Price;
                    }
                    item.Text = data.Price.ToString("0");
                    App.dbmng.InsetData(data);
                }
                else
                {
                    item.Text = "";
                }
                ShowHeader();
            }
            catch (Exception ex){
                var exc = ex.Message;
            }
        }

        private void SwitchSale_Toggled(object sender, ToggledEventArgs e)
        {
            if (IsEdit) {  return; }
            try
            {
                Switch item = (Switch)sender;
                Models.VisitStockData data = (Models.VisitStockData)item.BindingContext;
                StackLayout stk = (StackLayout)item.Parent;
                Switch stockitem = (Switch)stk.FindByName("SwStock");
                if (!data.Sale)
                {
                    data.Stock = false;
                    stockitem.IsToggled = false;
                    stockitem.IsEnabled = false;
                }
                else
                {
                    stockitem.IsEnabled = true;
                }
                App.dbmng.InsetData(data);
                ShowHeader();
            }
            catch { }
        }
        private void SwitchLost_Toggled(object sender, ToggledEventArgs e)
        {
            if (IsEdit) { return; }
            try
            {
                Switch item = (Switch)sender;
                Models.VisitStockData data = (Models.VisitStockData)item.BindingContext;
                StackLayout stk = (StackLayout)item.Parent;
                Switch stockitem = (Switch)stk.FindByName("SwStock");
                if (!data.Sale)
                {
                    data.Stock = false;
                    stockitem.IsToggled = false;
                    stockitem.IsEnabled = false;
                }
                else
                {
                    stockitem.IsEnabled = true;
                }
                App.dbmng.InsetData(data);
                ShowHeader();
            }
            catch { }
        }
        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            // Save Data
            if (IsEdit) { return; }
            IsEdit = true;
            AidWaitingRun(true, "กำลังส่งข้อมูลการเช็คสต็อก...");
            int success = 0;
            bool error = false;
            string msg = "ข้อมูลที่ส่งไปแล้วจะไม่สามารถแก้ไขได้\n" + "คุณต้องการกรอกข้อมูลต่อหรือส่งตามข้อมูลที่มี";
            if (await DisplayAlert("แจ้งเตือน", msg, "ส่งตามที่มี", "ยังไม่ส่ง"))
            {
                foreach (var dr in Activedata.Stock)
                {
                    AidWaitingRun(true, "กำลังส่งข้อมูลการเช็คสต็อก...\n" + dr.Productname);
                    msg = await App.Ws.SaveVisitStock(dr);
                    if (msg.Equals("")) { success += 1; }
                    else { error = true; }
                    dr.Check = false;
                    App.dbmng.InsetData(dr);
                }

                if (error)
                {
                    msg = "ไม่สามารถส่งข้อมูลรายการเช็คสต็อกได้จำนวน " + (Activedata.Stock.Count - success).ToString()
                        + "รายการ\nโปรดตรวจสอบอินเตอร์เน็ตแล้วส่งอีกครั้ง";
                    await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                }
                else
                {
                    Activedata.Detail.VisitStockSuccess = true;
                    Activedata.Detail.Stockicon = "ic_check";
                    ShowHeader();
                    App.dbmng.InsetData(Activedata.Detail);
                    Activedata.Visitdata.Transtatus = 2;
                    Activedata.Visitdata.Modifieddate = App.Servertime;
                    App.dbmng.InsetData(Activedata.Visitdata);
                    await App.Ws.SaveVisit(Activedata.Visitdata);

                    List<Models.VisitStockShop> listvisit = await Helpers.Controls.GetStockList(Activedata.Stock, !Activedata.Detail.VisitStockSuccess);
                    ListData.ItemsSource = null;
                    ListData.ItemsSource = listvisit;
                }
            }

            IsEdit = false;
            AidWaitingRun(false);
        }
        void AidWaitingRun(bool running, string msg = "", double progress = 0)
        {
            try
            {
                if (running && msg.Equals("")) { msg = "กำลังตรวจสอบข้อมูล..."; }
                LblStatus.Text = msg;
                Stk_AidWaitingBk.IsVisible = running;
                Stk_AidWaiting.IsVisible = running;
                AidWaiting.IsVisible = running;
                AidWaiting.IsRunning = running;
                Prgvalue.Progress = progress;
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