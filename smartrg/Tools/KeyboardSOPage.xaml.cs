using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KeyboardSOPage : ContentPage
    {
        Models.SOlineData Activedata = null;
        List<Models.ProductData> AllProduct = new List<Models.ProductData>();
        Models.ProductData ActiveProduct = null;
        string Ownerpage = "";
        int ProductIndex = 0;
        int Step = 0;
        public KeyboardSOPage()
        {
            InitializeComponent();
        }
        public bool Setdata(string ownerpage, Models.SOlineData data, List<Models.ProductData> listproduct)
        {
            Activedata = data;
            Ownerpage = ownerpage;
            AllProduct = listproduct;
            return ShowSaleline();
        }
        bool ShowSaleline()
        {
            try
            {
                if (Activedata.Productid==0) { ProductIndex = 0; }
                else { ProductIndex = AllProduct.FindIndex(x => x.Productid.Equals(Activedata.Productid)); }
                if (ProductIndex < 0) { return false; }
                Setstep(0);
                ShowProduct();
                return true;
            }
            catch { return false; }

        }

        //================= Number Click ============================
        #region "Number Click"

        //========= จำนวน =========
        private void calNumber_Clicked(object sender, System.EventArgs e)
        {
            Button item = (Button)sender;
            if (Step == 0)
            {
                Models.ProductData ActiveProduct = AllProduct[ProductIndex];
                Activedata.Qty = int.Parse(txtQty.Text += item.Text);
                txtQty.Text = Activedata.Qty.ToString("0");
                Activedata.Amount = (ActiveProduct.Price * ((double)Activedata.Qty));
                txtAmount.Text = Activedata.Amount.ToString();
            }
            else
            {
                Activedata.Amount = double.Parse(txtAmount.Text += item.Text);
                if (Activedata.Qty == 0) { Activedata.Price = Activedata.Price; }
                else { Activedata.Price = (Activedata.Amount / Activedata.Qty); }
                txtAmount.Text = Activedata.Amount.ToString();
            }
        }
        private void calcls_Clicked(object sender, System.EventArgs e)
        {
            if (Step == 0)
            {
                txtQty.Text = "0";
                txtAmount.Text = "0";
                Activedata.Qty = 0;
                Activedata.Amount = 0;
            }
            else
            {
                txtAmount.Text = "0";
                Activedata.Amount = 0;
            }
        }
        private void caldat_Clicked(object sender, EventArgs e)
        {
            if (Step != 0)
            {
                if (Activedata.Amount > 0)
                {
                    Activedata.Amount = Activedata.Amount * (-1);
                    txtAmount.Text = Activedata.Amount.ToString();
                }
            }
        }
        private void caldot_Clicked(object sender, EventArgs e)
        {
            if (Step != 0)
            {
                if (!txtAmount.Text.Contains("."))
                {
                    txtAmount.Text += ".";
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
            txtAmount.BackgroundColor = (step == 0) ? Color.Silver : Color.White;
            Step = step;
        }

        private void calOk_Clicked(object sender, System.EventArgs e)
        {
            CheckSave();
        }

        //========= Next Baack Product =========



        #endregion


        //================= Search Product & Agency ============================

        #region "Search Product & Agency"

        //========= สินค้า =========
        private void calbackPro_Clicked(object sender, EventArgs e)
        {
            if (Activedata.Item != 0) { return; }
            if (ProductIndex > 0)
            {
                SetActiveProduct(ProductIndex - 1);
                Setstep(0);
            }
        }
        private void calNextPro_Clicked(object sender, EventArgs e)
        {
            if (Activedata.Item != 0) { return; }
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
            TxtSearch.Focus();
        }
        private void TxtSearch_SearchButtonPressed(object sender, EventArgs e)
        {
            SearchProduct();
        }
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchProduct();
        }
        void SearchProduct()
        {
            if (Activedata.Item != 0) { return; }
            else
            {
                string keyword = TxtSearch.Text;
                lblSearchHeader.Text = "ข้อมูลสินค้า";
                var result = AllProduct.FindAll(x => x.Productname.ToUpper().Contains(keyword.ToUpper()));
                if (result != null && result.Count > 0)
                {
                    ListSearshData.ItemsSource = result;
                }
            }
        }

        private void ListSearshData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Models.ProductData item = (Models.ProductData)ListSearshData.SelectedItem;
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
            ActiveProduct = AllProduct[ProductIndex];
            Activedata.Productcode = ActiveProduct.Productcode;
            Activedata.Productid = ActiveProduct.Productid;
            Activedata.Productname = ActiveProduct.Productname;
            Activedata.Qtyperpack = 1;
            Activedata.UnitID = 1;
            Activedata.UnitName = ActiveProduct.Unitname;
            Activedata.Price = ActiveProduct.Price;
            if (Activedata.Qty == 0) { Activedata.Price = ActiveProduct.Price; }
            else { Activedata.Price = Activedata.Amount / Activedata.Qty; }

            //if(Activedata.Item != 0)
            //{
            //    Activedata.Price = pd.Price;
            //    Activedata.Amount = Activedata.Price * Activedata.Qty;
            //}
            lblProductname.Text = Activedata.Productname;
            lblProductunit.Text = Activedata.Price.ToString("0.00") + " บาท/" + Activedata.UnitName;
            txtQty.Text = Activedata.Qty.ToString("0");
            txtAmount.Text = Activedata.Amount.ToString("0.00");
            TxtSearch.Text = "";
        }
        void SetActiveProduct(int index)
        {
            if (Activedata.Item != 0) { return; }
            if (index < 0 || index > (AllProduct.Count - 1)) { return; }
            ProductIndex = index;
            ShowProduct();
        }


        #endregion



        //========= Save SaleData =========

        #region "Save SaleData"
        async void CheckSave()
        {
            var IsSave = true;
            if (Activedata.Qty == 0)
            {
                if (await DisplayAlert("ยืนยัน", "จำนวนสินค้าเป็นศูนย์ \n ต้องการลบหรือออกจากหน้าจอ", "แก้ไข", "ลบรายการ")) { return; }


                //IsSave = false;
            }
            else
            {
                if (Activedata.Amount == 0)
                {
                    if (await DisplayAlert("ยืนยัน", "ราคาสินค้ามีค่าเป็นศูนย์ \n คุณต้องการดำเนินการต่อหรือไม่", "ยกเลิก", "ตกลง")) { return; }
                    //IsSave = false;
                }
            }
            GotoOwnerPage(IsSave);
        }

        #endregion
        void GotoOwnerPage(bool IsSave)
        {
            Models.SOlineData result = null;
            if (IsSave) { result = Activedata; }
            MessagingCenter.Send<KeyboardSOPage, Models.SOlineData>(this, Ownerpage, result);
            Navigation.PopModalAsync();
        }

        private void calCancel_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }


    }
}