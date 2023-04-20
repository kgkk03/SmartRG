using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KeybordCVSPage : ContentPage
    {
        List<Models.VisitStockData> ProductStock = new List<Models.VisitStockData>();
        Models.VisitStockData Activedata = new Models.VisitStockData();
        string Ownerpage = "";
        int Index = 0;
        int Step = 0;
        bool Wattingclose = false;

        public KeybordCVSPage()
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
            LblProductprice.Text = " (" +Activedata.Price.ToString("#,##0.00") + ") บาท";
            txtQtyitem.Text = "";
            txtQty.BackgroundColor = Color.White;
            txtQty.Text = "";
            // มีขาย มีของ หรือ ของหมด 
            if (Activedata.Sale)
            {
                imgSale.Source = "ic_check";
                lblSale.Text =  "มีขาย";
                stkSale.BackgroundColor = Color.LightGreen;
                Activedata.Total = Activedata.Qty + Activedata.Facing;
                if (Activedata.Total == 0) {
                    // สินค้านี้มีขายแต่ของหมด
                    Activedata.Display = "สินค้าหมด";
                    stkLost.BackgroundColor =  Color.LightPink;
                    imgLost.Source = "ic_uncheck";
                    lblLost.Text = "ของขาด";
                    SetLostProduct();
                }
                else {
                    // สินค้านี้มีขายและมีของอยู่ในร้าน
                    Activedata.Display = "Stock : " + Activedata.Qty.ToString() +
                                            ", Facing : " + Activedata.Facing.ToString() +
                                            ", Tier : " + Activedata.Tier.ToString();
                    stkLost.BackgroundColor = Color.LightGreen;
                    imgLost.Source = "ic_check";
                    lblLost.Text = "มีของ";
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
                imgSale.Source = "ic_uncheck";
                lblSale.Text = "ไม่ขาย";
                stkSale.BackgroundColor = Color.LightPink;
                stkLost.BackgroundColor = Color.LightPink;
                imgLost.Source = "ic_uncheck";
                lblLost.Text = "ของขาด";
                txtQty.BackgroundColor = Color.LightGray;
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
                int qty = int.Parse(txtQty.Text += item.Text);
                txtQty.Text = qty.ToString();
                if (Step == 0) { ShowStock(qty); }
                else if (Step == 1) { Activedata.Display = "Stock : " + Activedata.Qty.ToString() + ", Facing : " + qty.ToString(); }
                else if (Step == 2) { Activedata.Display = "Stock : " + Activedata.Qty.ToString() + ", Facing : " + qty.ToString() + ", Tier : " + qty.ToString(); }
                LblDisplay.Text = Activedata.Display;

            }
        }
        private void calcls_Clicked(object sender, System.EventArgs e)
        {
            txtQty.Text = "0";
            if (Activedata.Sale)
            {
                int qty = 0;
                if (Step == 0) { ShowStock(qty); }
                else if (Step == 1) { Activedata.Display = "Stock : " + Activedata.Qty.ToString() + ", Facing : " + qty.ToString(); }
                else if (Step == 2) { Activedata.Display = "Stock : " + Activedata.Qty.ToString() + ", Facing : " + qty.ToString() + ", Tier : " + qty.ToString(); }
                LblDisplay.Text = Activedata.Display;

            }
        }
        void ShowStock(int qty)
        {
            if (qty > 0)
            {
                // สินค้านี้มีขายและมีของอยู่ในร้าน
                Activedata.Display = "Stock : " + qty.ToString();
                stkLost.BackgroundColor = Color.LightGreen;
                imgLost.Source = "ic_check";
                lblLost.Text = "มีของ";
                LblDisplay.Text = Activedata.Display;
            }
            else
            {
                // สินค้านี้มีขายแต่ของหมด
                Activedata.Display = "สินค้าหมด";
                stkLost.BackgroundColor = Color.LightPink;
                imgLost.Source = "ic_uncheck";
                lblLost.Text = "ของขาด";
                LblDisplay.Text = Activedata.Display;
                SetLostProduct();
            }
        }


        //========= มีจำหน่ายหรือไม่ =========
        private void calSale_Clicked(object sender, System.EventArgs e)
        {
            Activedata.Sale = !Activedata.Sale;
            if (Activedata.Sale) { Step = 0; Activedata.Qty = 0; }
            else { Step = 2; Activedata.Qty =-1; }
            ShowData(Index);
        }
        private void calLost_Clicked(object sender, System.EventArgs e)
        {
            if (!Activedata.Sale) { return; }
            Step = 2;
            Activedata.Qty = 0;
            Activedata.Facing = 0;
            ShowData(Index);
        }
        void SetLostProduct()
        {
            Activedata.Stock = false;
            Activedata.Qty = Activedata.Sale?0:-1;
            Activedata.Facing = 0;
            Activedata.Tier = 0;
            Activedata.Total = 0;
            txtQty.Text = "0";
        }
      
        //========= Next Baack OK =========
        private  void calNext_Clicked(object sender, System.EventArgs e)
        {
            if (txtQty.Text == null) { txtQty.Text = "0"; }
            if (!Activedata.Sale) { SaveProduct(1); }
            else
            {
                SaveStep();
                if (Step == 2) { SaveProduct(1); }
                else { Step += 1; ShowData(Index); }
            }
        }
        private  void calback_Clicked(object sender, System.EventArgs e)
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
                int qty = int.Parse(txtQty.Text);
                Activedata.Stock = (qty > 0);
                Activedata.Qty = (Activedata.Stock)? int.Parse(txtQty.Text) : 0;
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
            try {
                
                App.dbmng.InsetData(Activedata);
                Step = 0;
                Index = Index + nextupdate;
                if (Index > ProductStock.Count - 1 || (Index < 0)) {  GotoOwnerPage(); }
                else { ShowData(Index); }
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
            MessagingCenter.Send<KeybordCVSPage, List<Models.VisitStockData>>(this, Ownerpage, ProductStock);
            Navigation.PopModalAsync();
        }





    }

}