using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KeyboardSalePage : ContentPage
    {
        Models.CashSalelineData Activedata = null;
        List<Models.ProductInStock> AllProduct = new List<Models.ProductInStock>();
        List<Models.CashSalelineData> SaleData = new List<Models.CashSalelineData>();


        string Ownerpage = "";
        int ProductIndex = 0;
        int Step = 0;
        int TotalQty = 0;
        double TotalPrice = 0;
        string AgentID = "";


        public KeyboardSalePage()
        {
            InitializeComponent();
        }
        public bool Setdata(string ownerpage, Models.CashSalelineData data, List<Models.ProductInStock> listproduct, List<Models.CashSalelineData> saledata)
        {
            Activedata = data;
            Ownerpage = ownerpage;
            AllProduct = listproduct;
            SaleData = saledata;
            return ShowSaleline();
        }
        bool ShowSaleline()
        {
            try
            {
                if (Activedata.Productid==-1) { ProductIndex = 0; }
                else { ProductIndex = AllProduct.FindIndex(x => x.Productid.Equals(Activedata.Productid)); }
                if (ProductIndex < 0) { return false; }
                Setstep(0);
                ShowProduct();
                return true;
            }
            catch (Exception ex)
            { 
                var a = ex.Message;  
                return false; }
        }

        //================= Number Click ============================
        #region "Number Click"

        //========= จำนวน =========
        private async void calNumber_Clicked(object sender, System.EventArgs e)
        {
            Button item = (Button)sender;
            if (Step == 0)
            {
                Models.ProductInStock ActiveProduct = AllProduct[ProductIndex];
                if (txtQty.Text.Length < 8)
                {
                    TotalQty = int.Parse(txtQty.Text += item.Text);
                    txtQty.Text = TotalQty.ToString("0");
                    TotalPrice = (ActiveProduct.Product.Price * ((double)TotalQty));
                    txtPrice.Text = TotalPrice.ToString();
                }
                else
                {
                    await DisplayAlert("แจ้งเตือน", "กรุณาระบุจำนวนไม่เกิน \n 9,999,999", "ตกลง");
                    txtQty.Text = "0";
                    txtPrice.Text = "0.00";
                    return;
                }
            }
            else
            {
                if(txtPrice.Text.Length < 12)
                {
                    TotalPrice = double.Parse(txtPrice.Text += item.Text);
                    txtPrice.Text = TotalPrice.ToString();
                }
                else
                {
                    await DisplayAlert("แจ้งเตือน", "กรุณาระบุจำนวนไม่เกิน \n 99,999,999,999", "ตกลง");
                    txtQty.Text = "0";
                    txtPrice.Text = "0.00";
                    return;
                }

            }
        }
        private void calcls_Clicked(object sender, System.EventArgs e)
        {
            if (Step == 0)
            {
                txtQty.Text = "0";
                txtPrice.Text = "0";
                TotalQty = 0;
                TotalPrice = 0;
            }
            else
            {
                txtPrice.Text = "0";
                TotalPrice = 0;
            }
        }
        private void caldot_Clicked(object sender, EventArgs e)
        {
            if (Step != 0)
            {
                if (!txtPrice.Text.Contains("."))
                {
                    txtPrice.Text += ".";
                }
            }
        }



        //============= จำนวน ราคา ===================
        private void calQty_Clicked(object sender, EventArgs e)
        {
            Setstep(0);
        }
        private void calPrice_Clicked(object sender, EventArgs e)
        {
            Setstep(1);
        }
        void Setstep(int step)
        {
            txtQty.BackgroundColor = (step == 0) ? Color.White : Color.Silver;
            txtPrice.BackgroundColor = (step == 0) ? Color.Silver : Color.White;
            Step = step;
        }

        //========= Next Baack Product =========
        #endregion

        #region "Agent data"
        private void calagent_Clicked(object sender, EventArgs e)
        {
            StkSearchAgent.IsVisible = true;
            lblAgentProduct.Text = AllProduct[ProductIndex].Product.Productname;
            ListSearchAgent.ItemsSource = AllProduct[ProductIndex].Stockdata;
        }

        private void ListSearchAgent_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Models.PickingLineData item = (Models.PickingLineData)ListSearchAgent.SelectedItem;
            SetActivestock(AllProduct[ProductIndex], item.Agentid);
            StkSearchAgent.IsVisible = false;
            ListSearchAgent.SelectedItem = null;
        }
        private void btnCloseAgent_Clicked(object sender, EventArgs e)
        {
            StkSearchAgent.IsVisible = false;
        }

        #endregion


        //================= Search Product & Agency ============================

        #region "Search Product & Agency"

        //========= สินค้า =========
        private void calbackPro_Clicked(object sender, EventArgs e)
        {
            if (ProductIndex > 0)
            {
                SetActiveProduct(ProductIndex - 1);
                Setstep(0);
            }
        }
        private void calNextPro_Clicked(object sender, EventArgs e)
        {
            if (ProductIndex < (AllProduct.Count - 1))
            {
                SetActiveProduct(ProductIndex + 1);
                Setstep(0);
            }
        }
        private void calSearch_Clicked(object sender, EventArgs e)
        {
            //Searchtype = 0;
            if (Activedata.Item != 0) { return; }
            StkSearch.IsVisible = true;
            TxtSearsh.Focus();
        }
        private void TxtSearsh_SearchButtonPressed(object sender, EventArgs e)
        {
            SearchProduct();
        }
        private void TxtSearsh_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchProduct();
        }
        void SearchProduct()
        {
            string keyword = TxtSearsh.Text;
            if (Activedata.Item != 0) { return; }
            else
            {
                lblSearchHeader.Text = "ข้อมูลสินค้าบนรถ";
                List<Models.ProductInStock> result = new List<Models.ProductInStock>();
                if (keyword == null || keyword.Trim().Equals("")) { result = AllProduct; }
                else { result = AllProduct.FindAll(x => x.Product.Productname.ToUpper().Contains(keyword.Trim().ToUpper())); }

                if (result != null && result.Count > 0)
                {
                    ListSearshData.ItemsSource = result;
                }
            }
        }
        private void ListSearshData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Models.ProductInStock item = (Models.ProductInStock)ListSearshData.SelectedItem;
            int index = AllProduct.FindIndex(x => x.Productid.Equals(item.Productid));
            SetActiveProduct(index);
            StkSearch.IsVisible = false;
        }
        private void btnCloseSearch_Clicked(object sender, EventArgs e)
        {
            StkSearch.IsVisible = false;
        }
        private async void btnScan_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                string barcode = "";
                barcode = await Helpers.ImageConvert.QRCodeScan();
                if (!barcode.Equals(""))
                {
                    var result = App.dbmng.sqlite.Table<Models.Product>().Where(x => x.Barcode.Equals(barcode)).FirstOrDefault();
                    if (result != null)
                    {
                        int index = AllProduct.FindIndex(x => x.Productid.Equals(result.Productid));
                        SetActiveProduct(index);
                        return;
                    }
                }
                await DisplayAlert("แจ้งเตือน", "ไม่พบรายการสินค้าที่เลือก", "ตกลง");
            }
            catch (Exception ex) { await DisplayAlert("ProductList btnBarcod_Clicked Error", ex.Message, "OK"); }
        }
        void ShowProduct()
        {
            var pd = AllProduct[ProductIndex];
            Activedata.Productcode = pd.Product.Productcode;
            Activedata.Productid = pd.Product.Productid;
            Activedata.Productname = pd.Product.Productname;
            //Activedata.Qtyperpack = pd.Qtyperpack;
            //Activedata.Unitid = pd.Unitid;
            Activedata.Unitname = pd.Product.Unitname;

            lblProductname.Text = pd.Product.Productname;
            lblProductunit.Text = pd.Product.Price + " บาท คงเหลือ (รวม) : " + pd.Balance + " " + pd.Product.Unitname;
            if (pd.Stockdata != null || pd.Stockdata.Count > 0) { lblAgent.Text = pd.Stockdata[0].Agentname; }
            lblAgent.Text = "";
            txtQty.Text = Activedata.Qty.ToString("0");
            txtPrice.Text = Activedata.Amount.ToString("0.00");
            TxtSearsh.Text = "";
            TotalQty = Activedata.Qty;
            txtQty.Text = Activedata.Qty.ToString("0");

            txtQty.BackgroundColor = Color.White;
            TotalPrice = Activedata.Amount;
            txtPrice.Text = Activedata.Amount.ToString("0.00");
            txtPrice.BackgroundColor = Color.Silver;

            AgentID = Activedata.Agentid;
            if (AgentID == null || AgentID.Equals("")) { SetActivestock(pd); }
            {
                var activestock = pd.Stockdata.Find(x => x.Agentid.Equals(AgentID));
                if (activestock == null) { SetActivestock(pd); }
                else
                {
                    int balance = Getbalance();
                    lblAgent.Text = "คงเหลือ : " + balance.ToString() + " " + pd.Product.Unitname + " (" + activestock.Agentname + ")";
                }
            }
        }

        void SetActiveProduct(int index)
        {
            if (Activedata.Item != 0) { return; }
            if (index < 0 || index > (AllProduct.Count - 1)) { return; }
            ProductIndex = index;
            ShowProduct();
        }

        int Getbalance()
        {
            var activeproduct = AllProduct[ProductIndex];
            var ag = activeproduct.Stockdata.Find(x => x.Agentid.Equals(AgentID));
            int lastsale = 0;
            if (SaleData != null && SaleData.Count > 0)
            {
                lastsale = SaleData.Where(x => (x.Agentid.Equals(AgentID)) && (x.Productid.Equals(activeproduct.Productid)) && (x.Key != Activedata.Key)).Sum(x => x.Qty);
            }
            return (ag.Balance - lastsale);
        }

        void SetActivestock(Models.ProductInStock pd, string agentid = "")
        {
            Models.PickingLineData activestock = pd.Stockdata.Find(x => x.Agentid.Equals(agentid));
            if (activestock == null) { activestock = pd.Stockdata[0]; }
            Activedata.Agentid = activestock.Agentid;
            AgentID = activestock.Agentid;
            Activedata.Agentname = activestock.Agentname;
            int balance = Getbalance();
            lblAgent.Text = "คงเหลือ : " + balance.ToString() + " " + pd.Product.Unitname + " (" + activestock.Agentname + ")";

            double Allbalance = GetAllbalance();
            lblProductunit.Text = pd.Product.Price + " บาท คงเหลือ (รวม) : " + Allbalance + " " + pd.Product.Unitname;
        }

        private double GetAllbalance()
        {
            var activeproduct = AllProduct[ProductIndex];
            //var ag = activeproduct.Stockdata.Find(x => x.Agentid.Equals(AgentID));
            int lastsale = 0;
            if (SaleData != null && SaleData.Count > 0)
            {
                lastsale = SaleData.Where(x => (x.Agentid.Equals(AgentID)) && (x.Productid.Equals(activeproduct.Productid)) && (x.Key != Activedata.Key)).Sum(x => x.Qty);
            }
            return (activeproduct.Balance - lastsale);
        }

        #endregion



        //========= Save SaleData =========

        #region "Save SaleData"
        private void calOk_Clicked(object sender, System.EventArgs e)
        {
            CheckSave();
        }
        async void CheckSave()
        {

            if (TotalQty == 0)
            {
                if (await DisplayAlert("ยืนยัน", "จำนวนสินค้าเป็นศูนย์ \n ต้องการลบหรือออกจากหน้าจอ", "แก้ไข", "ลบรายการ")) { return; }
            }
            else
            {
                if (TotalPrice == 0)
                {
                    if (await DisplayAlert("ยืนยัน", "ราคาสินค้ามีค่าเป็นศูนย์ \n คุณต้องการดำเนินการต่อหรือไม่", "แก้ไข", "ตกลง")) { return; }
                }
            }
            if (CheckOverstock()) { GotoOwnerPage(true); }
            else
            {
                await DisplayAlert("แจ้งเตือน", "ไม่สามารถขายสินค้ารายการนี้ได้เนื่องจากสินค้ามีไม่พอจำหน่าย", "ตกลง");
                return;
            }


        }
        bool CheckOverstock()
        {
            try
            {
                var activeproduct = AllProduct[ProductIndex];
                var ag = activeproduct.Stockdata.Find(x => x.Agentid.Equals(AgentID));
                int laststock = Getbalance();
                int balance = laststock - TotalQty;
                if (balance < 0) { return false; }
                Activedata.Productid = ag.Productid;
                Activedata.Productcode = ag.Productcode;
                Activedata.Productname = ag.Productname;
                Activedata.Unitid = ag.Unitid;
                Activedata.Unitname = ag.Unitname;
                Activedata.Qtyperpack = ag.Qtyperpack;
                Activedata.Agentid = ag.Agentid;
                Activedata.Agentname = ag.Agentname;
                Activedata.Agentstock = laststock;
                Activedata.Qty = TotalQty;
                Activedata.Amount = TotalPrice;
                if (Activedata.Qty == 0) { Activedata.Price = 0; }
                else
                {
                    string stringprice = (Activedata.Amount / Activedata.Qty).ToString("0.00");
                    Activedata.Price = double.Parse(stringprice);
                }
                return true;
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            return false;
        }
        private void calCancel_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }

        #endregion

        void GotoOwnerPage(bool IsSave)
        {
            Models.CashSalelineData result = null;
            if (IsSave) { result = Activedata; }
            MessagingCenter.Send<KeyboardSalePage, Models.CashSalelineData>(this, Ownerpage, Activedata);
            Navigation.PopModalAsync();
        }


    }


}