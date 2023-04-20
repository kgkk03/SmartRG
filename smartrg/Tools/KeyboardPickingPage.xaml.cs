using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class KeyboardPickingPage : ContentPage
    {
        List<Models.PickingLineData> AllProduct = new List<Models.PickingLineData>();
        Models.PickingLineData ActiveProduct = new Models.PickingLineData();
        string Ownerpage = "";
        int ProductIndex = -1;
        int Step = 0;

        public KeyboardPickingPage()
        {
            InitializeComponent();
        }

        public void Setproduct(string ownerpage, List<Models.PickingLineData> listProduct, int index = -1)
        {
            Ownerpage = ownerpage;
            AllProduct = listProduct;
            calbackPro.IsEnabled = true;
            calNextPro.IsEnabled = true;
            calSearch.IsEnabled = true;
            btnScan.IsEnabled = true;
            if (index == -1)
            {
                AllProduct = listProduct.FindAll(x => x.Qty == 0);
                index = 0;
            }
            else
            {
                calbackPro.IsEnabled = false;
                calNextPro.IsEnabled = false;
                calSearch.IsEnabled = false;
                btnScan.IsEnabled = false;
            }
            ShowData(index);
        }


        void ShowData(int index)
        {
            if (AllProduct != null && AllProduct.Count > 0)
            {
                ActiveProduct = AllProduct[index];
                lblProductname.Text = ActiveProduct.Productname;
                lblProductunit.Text = ActiveProduct.Sizename;
                lblStockUnit.Text = ActiveProduct.Stock.ToString();
                txtPrice.Text = ActiveProduct.Price.ToString("0.00");
                txtQty.Text = ActiveProduct.Qty.ToString("0");
                ProductIndex = index;
                ListSearshData.ItemsSource = AllProduct;

            }
        }

        //=========== Get Product ===================
        #region "Product"
        private void calbackPro_Clicked(object sender, EventArgs e)
        {
            if (AllProduct != null && AllProduct.Count > 0)
            {
                int index = ProductIndex - 1;
                if (index < 0) { GotoOwnerPage(); }
                else { ShowData(index); }
            }

        }
        private void calNextPro_Clicked(object sender, EventArgs e)
        {
            if (AllProduct != null && AllProduct.Count > 0)
            {
                int index = ProductIndex + 1;
                if (index > (AllProduct.Count - 1)) { GotoOwnerPage(); }
                else { ShowData(index); }
            }

        }

        private void calSearch_Clicked(object sender, EventArgs e)
        {
            StkSearch.IsVisible = true;
        }

        private void btnCloseSearch_Clicked(object sender, EventArgs e)
        {
            StkSearch.IsVisible = false;
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
            if (TxtSearsh == null) { keyword = ""; }
            var data = AllProduct.FindAll(x => x.Productname.ToUpper().Contains(keyword.ToUpper()));
            ListSearshData.ItemsSource = data;

        }
        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            try
            {
                string barcode = "";
                barcode = await Helpers.ImageConvert.QRCodeScan();
                if (!barcode.Equals("")) { GetProductBarcode(barcode); }
                else { await DisplayAlert("แจ้งเตือน", "ไม่พบรายการสินค้าที่เลือก", "ตกลง"); }
            }
            catch (Exception ex) { await DisplayAlert("StockMerttKeyboardPage btnScan_Clicked Error", ex.Message, "OK"); }
        }
        void GetProductBarcode(string barcode = "")
        {
            try
            {
                //var data = App.dbmng.sqlite.Table<Models.Product>().Where(x => x.Barcode.Equals(barcode) || x.Barcodebox.Equals(barcode)).OrderBy(x => x.Productname).FirstOrDefault();
                //if (data == null) { DisplayAlert("แจ้งเตือน", "ไม่พบรายการสินค้าที่เลือก", "ตกลง"); }
                //else
                //{
                //    ProductIndex = AllProduct.FindIndex(x => x.Productid.Equals(data.Productid));
                //    if (ProductIndex < 0)
                //    {
                //        DisplayAlert("แจ้งเตือน", "ไม่มีข้อมูลที่เลือก", "ตกลง");
                //        ProductIndex = 0;
                //    }
                //    ShowData(ProductIndex);
                //}
            }
            catch (Exception ex) { DisplayAlert("ProductList GetProductBarcode Error", ex.Message, "OK"); }
        }

        private void ListSearshData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ActiveProduct = (Models.PickingLineData)ListSearshData.SelectedItem;
            ProductIndex = AllProduct.FindIndex(x => x.Productid.Equals(ActiveProduct.Productid));
            if (ProductIndex < 0)
            {
                DisplayAlert("แจ้งเตือน", "ไม่มีข้อมูลที่เลือก", "ตกลง");
                ProductIndex = 0;
            }
            ShowData(ProductIndex);
            StkSearch.IsVisible = false;
        }
        #endregion



        private void calNumber_Clicked(object sender, EventArgs e)
        {
            Button item = (Button)sender;
            if(txtQty.Text.Length > 7) { DisplayAlert("แจ้งเตือน", "ไม่สามารถกรอกจำนวนเกินกว่า \n10,000,000 ได้", "ตกลง");
                return;
            }
            int qty = int.Parse(txtQty.Text + item.Text);
            txtQty.Text = qty.ToString();
        }

        private void calcls_Clicked(object sender, EventArgs e)
        {
            txtQty.Text = "0";
        }




        private async void calOk_Clicked(object sender, EventArgs e)
        {
            int qty = int.Parse(txtQty.Text);
            if (qty == 0)
            {
                if (!await DisplayAlert("แจ้งเตือน", "คุณไม่ต้องการเบิกสินค้ารายการนี้ใช่หรือไม่ ", "ไม่เบิก", "แก้ไขจำนวน")) { return; }
            }
            int total = (ActiveProduct.Typeid * qty);
            int balance = (ActiveProduct.Stock + total);
            if (balance < 0)
            {
                await DisplayAlert("แจ้งเตือน", "ไม่สามารถคืนสินค้าเกินจำนวนยอดคงเหลือได้ ", "แก้ไขจำนวน");
                txtQty.Text = ActiveProduct.Stock.ToString();
                return;
            }
            ActiveProduct.Qty = qty;
            ActiveProduct.Total = total;
            ActiveProduct.Balance = balance;
            GotoOwnerPage(true);
        }
        private void btncancel_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }

        void GotoOwnerPage(bool success = false)
        {
            Models.PickingLineData result = null;
            if (success) { result = ActiveProduct; }
            MessagingCenter.Send<KeyboardPickingPage, Models.PickingLineData>(this, Ownerpage, result);
            Navigation.PopModalAsync();
        }


    }
}