using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KeybordBillsalePage : ContentPage
    {
        List<Models.ShowVisitBillSale> ProductBill = new List<Models.ShowVisitBillSale>();
        Models.ShowVisitBillSale Activedata = new Models.ShowVisitBillSale();
        string Ownerpage = "";
        int Index = 0;
        int Step = 0;


        public KeybordBillsalePage()
        {
            InitializeComponent();
            ProductBill.Add(Activedata);
        }

        public void Setproduct(List<Models.ShowVisitBillSale> data, int index, string ownerpage)
        {
            Ownerpage = ownerpage;
            ProductBill = data;
            ShowData(index);
        }


        void ShowData(int index)
        {
            Activedata.Check = true;
            Activedata = ProductBill[index];
            ShowSale();
            LblProductname.Text = Activedata.Productname;
            LblProductcode.Text = Activedata.Productcode;
            LblProductprice.Text = " (" + Activedata.Price.ToString("#,##0.00") + ") บาท";
            SetDisplay();
            LblDisplay.Text = Activedata.Display;
            stkLost.BackgroundColor = (Activedata.Total == 0) ? Color.LightPink : Color.LightGreen;
            imgSale.Source = Activedata.Sale ? "ic_check" : "ic_uncheck";
            lblSale.Text = Activedata.Sale ? "มีขาย" : "ไม่ขาย";
            imgLost.Source = (Activedata.Total == 0) ? "ic_uncheck" : "ic_check";
            lblLost.Text = (Activedata.Total == 0) ? "ไม่มียอด" : "มียอด";
            txtQtyitem.Text = (Step == 0 ? "จำนวน" : "ราคา");
            txtQty.Text = (Step == 0 ? Activedata.Total.ToString() :  Activedata.Amount.ToString());
            Index = index;
            Activedata.Icon = "ic_check";
            Activedata.Cor = Color.LightGreen;

        }

        void SetDisplay()
        {
            if (Activedata.Sale)
            {
                if (Activedata.Total != 0)
                {
                    Activedata.Display = "จำนวน : " + Activedata.Total.ToString() + " " + Activedata.Unitname +
                        ", รวมเงิน : " + Activedata.Amount.ToString("#,##0.00") + " บาท" ;
                }
                else { Activedata.Display = "ไม่มียอดขาย"; }
            }
            else { Activedata.Display = "สินค้าไม่มีจำหน่าย"; }

        }



        //================= Number Click ============================
        #region "Number Click"

        //========= จำนวน =========
        private void calNumber_Clicked(object sender, System.EventArgs e)
        {
            Button item = (Button)sender;
            if (Activedata.Sale)
            {
                Activedata.Total = int.Parse(txtQty.Text += item.Text);
                txtQty.Text = Activedata.Total.ToString();
            }
        }
        private void calcls_Clicked(object sender, System.EventArgs e)
        {
            txtQty.Text = "0";
            Activedata.Total = 0;
        }

        //========= มีจำหน่ายหรือไม่ =========
        private void calSale_Clicked(object sender, System.EventArgs e)
        {
            Activedata.Sale = !Activedata.Sale;
            Step = Activedata.Sale ? 0 : 2;
            ShowData(Index);
        }
        private void calLost_Clicked(object sender, System.EventArgs e)
        {
            if (!Activedata.Sale) { return; }
            //Activedata.Stock = 0;
            //Activedata.Facing = 0;
            //Activedata.Tier = 0;
            Activedata.Total = 0;
            Activedata.Amount = 0;
            txtQty.Text = "0";
            Step = 0;
            SaveProduct(1);
        }
        void ShowSale()
        {
            if (Activedata.Sale)
            {
                // มีจำหน่าย
                stkSale.BackgroundColor = Color.LightGreen;
                Activedata.Sale = true;
            }
            else
            {
                // ไม่มีจำหน่าย
                stkSale.BackgroundColor = Color.LightPink;
                stkLost.BackgroundColor = Color.Silver;
                imgLost.Source = "ic_uncheck";
                Activedata.Total = 0;
                Activedata.Amount = 0;
                txtQty.Text = "0";
            }
        }


        //========= Next Baack OK =========
        private void calNext_Clicked(object sender, System.EventArgs e)
        {
            if (txtQty.Text == null) { txtQty.Text = "0"; }
            SaveStep();
            if (Step == 1) { SaveProduct(1); }
            else { Step += 1; ShowData(Index); }
            
        }
        private void calback_Clicked(object sender, System.EventArgs e)
        {
            if (txtQty.Text == null) { txtQty.Text = "0"; }
            SaveStep();
            if (Step == 0) { SaveProduct(-1); }
            else { Step -= 1; ShowData(Index); }
        }
        private void calOk_Clicked(object sender, System.EventArgs e)
        {
            //if (Activedata.Total == 0 && Activedata.Sale)
            //{
            //    if (await DisplayAlert("ยืนยัน", "จำนวนสินค้าคงเหลือมีค่าเท่ากับ 0 \n คุณต้องการดำเนินการต่อหรือไม่", "ยกเลิก", "ตกลง"))
            //    { return; }
            //}
            //Activedata.Check = true;
            SaveStep();
            GotoOwnerPage();

        }

        void SaveStep()
        {
            if (Step == 0)
            {
                // save stock
                Activedata.Total = int.Parse(txtQty.Text);
                Activedata.Amount = Activedata.Price* Activedata.Total;
            }
            else if (Step == 1)
            {
                // save facing
                Activedata.Amount = double.Parse(txtQty.Text);
            }
        }

        void SaveProduct(int nextupdate)
        {
            Step = 0;
            Index = Index + nextupdate;
            if (Index >= ProductBill.Count - 1 || (Index < 0)) { GotoOwnerPage(); }
            else { ShowData(Index); }
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
                        int index = ProductBill.FindIndex(x => x.Productid.Equals(result.Productid));
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
            MessagingCenter.Send<KeybordBillsalePage, List<Models.ShowVisitBillSale>>(this, Ownerpage, ProductBill);
            Navigation.PopModalAsync();
        }





    }

}