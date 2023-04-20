using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KeyboardPaymentPage : ContentPage
    {
        string Ownerpage = "KeyboardPaymentPage";
        double TotalAmount = 0;
        double LastBalance = 0;
        double LastPayment = 0;
        double TotalPayment = 0;
        Models.PaymentData Activepayment = new Models.PaymentData();
        string QRCode = "";
        bool isbusey = false;
        public KeyboardPaymentPage()
        {
            InitializeComponent();
        }
        public bool Setdata(string ownerpage, double totalamount, double lastpayment)
        {
            Ownerpage = ownerpage;
            TotalAmount = totalamount;
            LastPayment = lastpayment;
            return ShowPayment();
        }
        bool ShowPayment()
        {
            try
            {
                LastBalance = TotalAmount - LastPayment;
                lblTotalAmount.Text = "ยอดชำระ : " + TotalAmount.ToString("#,##0.00") + " บาท";
                lblLastPayment.Text = "ชำระแล้ว : " + LastPayment.ToString("#,##0.00") + " บาท";
                lblBalance.Text = "คงเหลือ : " + LastBalance.ToString("#,##0.00") + " บาท";
                txtPayment.Text = LastBalance.ToString("0.00");
                TotalPayment = double.Parse(txtPayment.Text);
                SetPaymenttype(Activepayment.paytypeid);
                ListBank.ItemsSource = App.dbmng.sqlite.Table<Models.PaymentBank>().OrderBy(x => x.Priority).ThenBy(x => x.Bankname).ToList();
                lblPayBank.Text = "";
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
            TotalPayment = double.Parse(txtPayment.Text += item.Text);
            if (!txtPayment.Text.Contains(".")) { txtPayment.Text = TotalPayment.ToString(); }
        }
        private void calcls_Clicked(object sender, System.EventArgs e)
        {
            txtPayment.Text = "0";
            TotalPayment = 0;
        }
        private void caldot_Clicked(object sender, EventArgs e)
        {
            if (!txtPayment.Text.Contains("."))
            {
                txtPayment.Text += ".";
            }
        }


        //========= Next Baack Product =========
        #endregion

        //================= Payment Type (เงื่อนไขการชำระเงิน) ============================
        #region "PaymentType"
        private void calPaytype_Clicked(object sender, EventArgs e)
        {
            Button item = (Button)sender;
            Activepayment.paytypeid = int.Parse(item.StyleId);
            SetPaymenttype(Activepayment.paytypeid);
        }
        async void SetPaymenttype(int paymentid)
        {
            calChash.BackgroundColor = (paymentid == 0) ? Color.LightGreen : Color.LightPink;
            calTransfer.BackgroundColor = (paymentid == 1) ? Color.LightGreen : Color.LightPink;
            calCheque.BackgroundColor = (paymentid == 2) ? Color.LightGreen : Color.LightPink;
            calCredit.BackgroundColor = (paymentid == 3) ? Color.LightGreen : Color.LightPink;
            calqr.BackgroundColor = (paymentid == 4) ? Color.LightGreen : Color.LightPink;
            Activepayment.Paytypename = GetPaymenttype(Activepayment.paytypeid);
            lblPayType.Text = Activepayment.Paytypename + " :";
            if(paymentid == 4) {await ShowQRCode(); }
        }
        private string GetPaymenttype(int paymentid)
        {
            if (paymentid == 0) { return "เงินสด"; }
            else if (paymentid == 1) { return "โอนเงิน"; }
            else if (paymentid == 2) { return "เช็ค"; }
            else if (paymentid == 3) { return "เครดิต"; }
            else if (paymentid == 4) { return "QR Code"; }
            else { return "อื่น ๆ"; }
        }

        #endregion


        //================= QR Code (QR Payment) ============================
        #region QR Code
        async Task<bool> ShowQRCode()
        {
            try {
                return false; 
                if (isbusey) { return false; }
                isbusey = true;
                ImgQRCode.Source = await Helpers.Controls.GetQRPayment();
                StkQRCode.IsVisible = true;
            }
            catch { }
            isbusey = false;
            return await Task.FromResult(true);
        }

        private void BtnCloseQR_Clicked(object sender, EventArgs e)
        {
            StkQRCode.IsVisible = false;
        }

        #endregion
        //================= Bank (ธนาคารที่รับ ชำระเงิน) ============================
        #region "Bank data"
        private void calBank_Clicked(object sender, EventArgs e)
        {
            StkBank.IsVisible = true;
        }
        private void ListBank_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Models.PaymentBank item = (Models.PaymentBank)ListBank.SelectedItem;
            lblPayBank.Text = item.Bankname;
            Activepayment.Bank = item.Bankname;
        }
        private void btnCancelBank_Clicked(object sender, EventArgs e)
        {
            lblPayBank.Text = "";
            Activepayment.Bank = "";
            StkBank.IsVisible = false;
        }
        private void btnAddBank_Clicked(object sender, EventArgs e)
        {
            StkBank.IsVisible = false;
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

            if (TotalPayment == 0)
            { await DisplayAlert("แจ้งเตือน", "กรุณาระบุจำนวนเงินที่ชำระ", "แก้ไข"); return; }
            else if (TotalPayment > LastBalance)
            { if (await DisplayAlert("ยืนยัน", "จำนวนเงินที่ชำระ เกินกว่า มูลค่าสินค้า", "แก้ไข", "ดำเนินการต่อ")) { return; } }
            else if (TotalPayment > LastBalance)
            { if (await DisplayAlert("ยืนยัน", "จำนวนเงินที่ชำระ น้อยกว่า มูลค่าสินค้า", "แก้ไข", "ดำเนินการต่อ")) { return; } }
            if (Activepayment.paytypeid != 0 && Activepayment.Bank.Trim().Equals(""))
            { if (await DisplayAlert("ยืนยัน", "คุณยังไม่ระบุ ธนาคาร", "แก้ไข", "ดำเนินการต่อ")) { return; } }
            Activepayment.Total = TotalPayment;
            GotoOwnerPage(true);
        }

        private void calCancel_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }

        #endregion

        void GotoOwnerPage(bool IsSave)
        {
            Models.PaymentData result = null;
            if (IsSave) { result = Activepayment; }
            MessagingCenter.Send<KeyboardPaymentPage, Models.PaymentData>(this, Ownerpage, result);
            Navigation.PopModalAsync();
        }

       
    }
}