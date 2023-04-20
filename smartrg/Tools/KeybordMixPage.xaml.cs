using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class KeybordMixPage : ContentPage
    {
        List<Models.VisitStockData> ProductStock = new List<Models.VisitStockData>();
        Models.VisitStockData Activedata = new Models.VisitStockData();
        string Ownerpage = "";
        int Index = 0;
        int Step = 0;
        bool Wattingclose = false;

        public KeybordMixPage()
        {
            InitializeComponent();

        }

        public void Setproduct(List<Models.VisitStockData> data, int index, string ownerpage)
        {
            Ownerpage = ownerpage;
            ProductStock = data;
            ShowData(index);
        }
        void ShowData(int index)
        {
            Activedata = ProductStock[index];
            Activedata.Check = true;
            Activedata.Icon = "ic_check";
            LblProductname.Text = Activedata.Productname;
            LblProductcode.Text = Activedata.Productcode;
            LblProductprice.Text = " (" + Activedata.Price.ToString("#,##0.00") + ") บาท";
            txtQtyitem.Text = "";
            txtQty.BackgroundColor = Color.White;
            txtQty.Text = "";
            Setstock();
            ShowSelect(Step == 0);
            // มีขาย มีของ หรือ ของหมด 
            if (Activedata.Sale)
            {
              
                Activedata.Total = Activedata.Qty + Activedata.Facing;
                if (Activedata.Total > 0)
                {
                    Activedata.Display = "สินค้ามีจำหน่าย" + ", Facing " + Activedata.Facing.ToString() + ", Tier " + Activedata.Tier.ToString();
                }
                else
                {
                    // สินค้านี้มีขายแต่ของหมด
                    Activedata.Display = "สินค้าหมด";
                    SetLostProduct();
                }

                // แสดง Stock Facing tire
                if (Step == 0) { txtQtyitem.Text = "Stock"; txtQty.Text = Activedata.Qty.ToString(); }
                else if (Step == 1) { txtQtyitem.Text = "Facing"; txtQty.Text = Activedata.Facing.ToString(); }
                else { txtQtyitem.Text = "Tier"; txtQty.Text = Activedata.Tier.ToString(); }
            }
            else
            {
                // สินค้านี้ไม่มีขายในร้าน
                Activedata.Display = "สินค้าไม่มีจำหน่าย";
                SetLostProduct();
            }
            LblDisplay.Text = Activedata.Display;
            Index = index;
        }

        //================= Number Click ============================
        #region "Number Click"

        //========= จำนวน =========
        private void calNumber_Clicked(object sender, System.EventArgs e)
        {
            Button item = (Button)sender;
            if (Activedata.Sale)
            {
                if (Step == 0) { return; }
                else
                {
                    int qty = int.Parse(txtQty.Text += item.Text);
                    txtQty.Text = qty.ToString();
                    if (Step == 1) { Activedata.Display = "สินค้ามีจำหน่าย : Facing : " + qty.ToString(); }
                    else if (Step == 2) { Activedata.Display = "สินค้ามีจำหน่าย : Facing : " + Activedata.Facing.ToString() + ", Tier : " + qty.ToString(); }
                    LblDisplay.Text = Activedata.Display;
                }

            }
        }
        private void calcls_Clicked(object sender, System.EventArgs e)
        {
            txtQty.Text = "0";
            if (Activedata.Sale)
            {
                int qty = 0;
                if (Step == 0) { }
                else if (Step == 1) { Activedata.Display = "สินค้ามีจำหน่าย : Facing : "  + qty.ToString(); }
                else if (Step == 2) { Activedata.Display = "สินค้ามีจำหน่าย : Facing : "  + Activedata.Facing.ToString() + ", Tier : " + qty.ToString(); }
                LblDisplay.Text = Activedata.Display;

            }
        }
      
        void ShowSelect(bool isshow )
        {
            stkSale.IsVisible = isshow;
            calSale.IsVisible = isshow;
            stkLost.IsVisible = isshow;
            calLost.IsVisible = isshow;
            stkStock.IsVisible = isshow;
            calStock.IsVisible = isshow;
            //calCheck.IsVisible = isshow;
            stkSelect.IsVisible = isshow;
           
        }


        //========= มีจำหน่ายหรือไม่ =========
        private void calSale_Clicked(object sender, System.EventArgs e)
        {
            Activedata.Sale= false;
            Activedata.Stock = false;
            Activedata.Qty = -1;
            Activedata.Facing = 0;
            Activedata.Tier = 0;
            ShowData(Index);
        }
        private void calLost_Clicked(object sender, System.EventArgs e)
        {
            Activedata.Sale = true;
            Activedata.Stock = false;
            Activedata.Qty = 0;
            Activedata.Facing = 0;
            Activedata.Tier = 0;
            ShowData(Index);
        }
        private void calStock_Clicked(object sender, EventArgs e)
        {
            Activedata.Sale = true;
            Activedata.Stock = true;
            Activedata.Qty = 1;
            Activedata.Facing = 0;
            Activedata.Tier = 0;
            ShowData(Index);
        }
        void Setstock()
        {
            stkSale.BackgroundColor = Color.White;
            stkLost.BackgroundColor = Color.White;
            stkStock.BackgroundColor = Color.White;
            imgSale.IsVisible = false;
            imgLost.IsVisible = false;
            imgStock.IsVisible = false;

            if (!Activedata.Sale) { stkSale.BackgroundColor = Color.LightGreen; imgSale.IsVisible =true; return; }
            if (!Activedata.Stock) { stkLost.BackgroundColor = Color.LightGreen; imgLost.IsVisible = true; return; }
            stkStock.BackgroundColor = Color.LightGreen; imgStock.IsVisible = true; 
        }

        void SetLostProduct()
        {
            Activedata.Stock = false;
            Activedata.Qty = Activedata.Sale ? 0 : -1;
            Activedata.Facing = 0;
            Activedata.Tier = 0;
            Activedata.Total = 0;
            txtQty.Text = "0";
        }

        //========= Next Baack OK =========
        private void calNext_Clicked(object sender, System.EventArgs e)
        {
            if (txtQty.Text == null) { txtQty.Text = "0"; }
            if (!Activedata.Sale|| !Activedata.Stock) { SaveProduct(1); }
            else
            {
                SaveStep();
                if (Step == 2) { SaveProduct(1); }
                else { Step += 1; ShowData(Index);  }
            }
        }
        private void calback_Clicked(object sender, System.EventArgs e)
        {
            if (txtQty.Text == null) { txtQty.Text = "0"; }
            if (!Activedata.Sale) { SaveProduct(-1); }
            else
            {
                SaveStep();
                if (Step == 0) { SaveProduct(-1); }
                else { Step -= 1; ShowData(Index); }
            }
        }
        private void calOk_Clicked(object sender, System.EventArgs e)
        {
            SaveStep();
            GotoOwnerPage();
        }
        void SaveStep()
        {
            if (Step == 0)
            {
                // save stock
            }
            else if (Step == 1)
            {
                // save facing
                Activedata.Facing = int.Parse(txtQty.Text);
            }
            else if (Step == 2)
            {
                // save Tier
                Activedata.Tier = int.Parse(txtQty.Text);
            }

            Activedata.Total = Activedata.Qty + Activedata.Facing;

        }
        void SaveProduct(int nextupdate)
        {
            try
            {

                App.dbmng.InsetData(Activedata);
                Step = 0;
                Index = Index + nextupdate;
                if (Index > ProductStock.Count - 1 || (Index < 0)) { GotoOwnerPage(); }
                else { ShowSelect(true); ShowData(Index); }
            }
            catch { }
        }

        #endregion

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
                        int index = ProductStock.FindIndex(x => x.Productid.Equals(result.Productid));
                        ShowData(index);
                        return;
                    }
                }
                await DisplayAlert("แจ้งเตือน", "ไม่พบรายการสินค้าที่เลือก", "ตกลง");
            }
            catch (Exception ex) { await DisplayAlert("SaleKeyboardBP btnBarcod_Clicked Error", ex.Message, "OK"); }
        }
        void GotoOwnerPage()
        {
            if (Wattingclose) { return; }
            Wattingclose = true;
            MessagingCenter.Send<KeybordMixPage, List<Models.VisitStockData>>(this, Ownerpage, ProductStock);
            Navigation.PopModalAsync();
        }

       
    }
}