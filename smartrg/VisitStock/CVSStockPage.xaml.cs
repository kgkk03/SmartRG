using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.VisitStock
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CVSStockPage : ContentPage
    {
        string Mypage = "CVSStockPage";
        Models.VisitShowpageData Activedata = null;
        bool IsEdit = false;

        public CVSStockPage()
        {
            InitializeComponent();
        }
        void StartKeybordMixPageMessage()
        {
            MessagingCenter.Subscribe<Tools.KeybordMixPage, List<Models.VisitStockData>>(this, Mypage, (sender, arg) =>
            {
                Device.BeginInvokeOnMainThread(() => {
                    try { ShowData(arg); } catch { }
                    MessagingCenter.Unsubscribe<Tools.KeybordMixPage, List<Models.VisitStockData>>(this, Mypage);
                });
            });
        }
        public async void Setdata(Models.VisitShowpageData data)
        {
            AidWaitingRun(true);
            if (Activedata == null)
            {
                Activedata = data;
                LblCustname.Text = Activedata.Customer.Custname + " (" + Activedata.Customer.Custgroupname + ")";
                ImgCustomer.Source = Activedata.Customer.Icon;
                LblAddress.Text = Activedata.Customer.Custaddress;
                LblWorksumary.Text = Activedata.Visitdata.Key;
            }

            if (Activedata.Stock == null)
            {
                if (Activedata.Detail.Newvisit) { await GetNewVisitstok(); }
                else { await GetVisitstokLog(); }
            }

            if (Activedata.Stock != null)
            {
                ListData.ItemsSource = Activedata.Stock;
            }
            if (!Activedata.Detail.Newvisit) { Activedata.Detail.VisitStockSuccess = true; }
            ShowHeader();
            AidWaitingRun(false);
        }
        public async Task<bool> GetNewVisitstok()
        {
            try
            {
                Activedata.Stock = App.dbmng.sqlite.Table<Models.VisitStockData>()
                                        .Where(x => x.Visitid.Equals(Activedata.Visitdata.Key))
                                        .OrderBy(x => x.Piority).ThenBy(x => x.Productname).ToList();

                if (Activedata.Stock == null || Activedata.Stock.Count == 0)
                {
                    Activedata.Stock = await App.Ws.GetVisitStock(Activedata.Customer.Custgroupid, Activedata.Customer.Key);
                    if (Activedata.Stock != null && Activedata.Stock.Count > 0)
                    {
                        foreach (var dr in Activedata.Stock)
                        {
                            if (!dr.Sale) { dr.Stock = false; }
                            dr.Visitid = Activedata.Visitdata.Key;
                            dr.Custid = Activedata.Customer.Key;
                            dr.Custtype = Activedata.Customer.Typeid;
                            dr.Grouptype = Activedata.Customer.Custgroupid;
                            dr.Key = Activedata.Visitdata.Key + "-" + dr.Productid;
                            App.dbmng.InsetData(dr);
                        }
                    }
                }
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        public async Task<bool> GetVisitstokLog()
        {
            try
            {
                Activedata.Stock = await App.Ws.GetVisitStocklog(Activedata.Customer.Custgroupid, Activedata.Visitdata.Key);
                if (Activedata.Stock != null && Activedata.Stock.Count > 0)
                {
                    foreach (var dr in Activedata.Stock)
                    {
                        dr.Display = "Stock : " + dr.Qty.ToString() +
                                            ", Facing : " + dr.Facing.ToString() +
                                            ", Tier : " + dr.Tier.ToString();
                        dr.Check = true;
                        dr.Icon = "ic_check";
                    }
                }
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }

        void ShowData(List<Models.VisitStockData> listdata)
        {
            try
            {
                Activedata.Stock = listdata;
                ListData.ItemsSource = null;
                ListData.ItemsSource = listdata;
                ShowHeader();
            }
            catch { }
            IsEdit = false;
        }
        void ShowHeader()
        {
            var data = Activedata.Stock;
            if (data != null && data.Count > 0)
            {
                int total = data.Count;
                int check = data.Count(x => x.Check);
                int sale = data.Count(x => x.Check && x.Sale);
                int lost = data.Count(x => x.Check && x.Sale && !x.Stock);
                int available = data.Count(x => x.Check && x.Sale && x.Stock);
                string detail = "มีจำหน่าย " + sale.ToString();
                detail += ", ไม่มีจำหน่าย " + (check - sale).ToString();
                detail += ", ของหมด " + lost.ToString();
                detail += ", Facing " + data.Count(x => x.Facing > 0).ToString();

                Activedata.Detail.StockCount = check.ToString() + " / " + total.ToString();
                Activedata.Detail.StockHeader = detail;
            }
            LblWorksumary.Text = Activedata.Detail.StockCount;
            LblWorkdetails.Text = Activedata.Detail.StockHeader;
            BtnSend.IsVisible =  !Activedata.Detail.VisitStockSuccess;
        }
        private void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Activedata.Detail.VisitStockSuccess || IsEdit) { return; }
                IsEdit = true;
                Models.VisitStockData item = (Models.VisitStockData)e.CurrentSelection.FirstOrDefault();
                if (item == null) { return; }
                int index = Activedata.Stock.FindIndex(x => x.Productid.Equals(item.Productid));
                EditStockData(index);
                ListData.SelectedItem = null;
            }
            catch { IsEdit = false; }
        }
        async void EditStockData(int index)
        {
            var page = new Tools.KeybordMixPage();
            page.Setproduct(Activedata.Stock, index, Mypage);
            await Navigation.PushModalAsync(page);
            StartKeybordMixPageMessage();
        }
        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            // Save Data
            if (IsEdit) { return; }
            IsEdit = true;
            bool reqpage = false;
            Models.VisitPage  page = Activedata.VisitPage.Where(x => x.Pageid == 1102).FirstOrDefault();
            if (page != null) { reqpage = page.Reqpage; }
            int uncheck = Activedata.Stock.Count(x => !x.Check);

            if(reqpage && uncheck > 0)
            {
                await DisplayAlert("แจ้งเตือน","คุณต้องเช็คสต็อกให้ครบก่อนส่งข้อมูล","ตกลง");
                IsEdit = false;
                return;
            }

            AidWaitingRun(true, "กำลังส่งข้อมูลการเช็คสต็อก...");
            int success = 0;
            bool error = false;
            string msg = "";
            if (!reqpage && uncheck > 0)
            {
                msg = "คุณมีรายการสินค้าที่ไม่ได้ตรวจสอบจำนวน " + uncheck + " รายการ\n" + "คุณต้องการกรอกข้อมูลต่อหรือส่งตามข้อมูลที่มี";
                if (!await DisplayAlert("แจ้งเตือน", msg, "ส่งตามที่มี", "ยังไม่ส่ง")) { IsEdit = false; return; }
            }
            var senddata = Activedata.Stock.FindAll(x => x.Check);
            foreach (var dr in senddata)
            {
                AidWaitingRun(true, "กำลังส่งข้อมูลการเช็คสต็อก...\n" + dr.Productname);
                msg = await App.Ws.SaveVisitStock(dr);
                if (msg.Equals("")) { success += 1; }
                else { error = true; }
            }
            if (error)
            {
                msg = "ไม่สามารถส่งข้อมูลรายการเช็คสต็อกได้จำนวน " + (senddata.Count - success).ToString()
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
            }
            IsEdit = false;
            AidWaitingRun(false);
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