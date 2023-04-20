using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Picking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReturnPickingPage : ContentPage
    {
        string OwnerPage = "ReturnPickingPage";
        List<Models.PickingLineData> ListPicking ;
        Models.PickingData ActiveData;
        public ReturnPickingPage()
        {
            InitializeComponent();
        }
        public async void ShowData(string ownerpage, TabReturnPage MT)
        {
            try
            {
                AidWaitingRun(true);
                OwnerPage = ownerpage;
                if (ActiveData == null)
                {
                    ActiveData = MT.ActiveData;
                    LblCustname.Text = ActiveData.Agentname;
                    ImgCustomer.Source = ActiveData.Icon;
                    LblAddress.Text = ActiveData.Agentid;
                    LblPickingSumary.Text = "";
                    if (MT.AllPickingLine == null){ MT.AllPickingLine = await App.Ws.GetPickingDetail(ActiveData.Key); }
                    if (MT.AllPickingLine != null && MT.AllPickingLine.Count > 0)
                    {
                        ListPicking = MT.AllPickingLine.Where(x => x.Pickingtype.Equals("เบิก")).OrderBy(X => X.Item).ToList();
                    }
                    ListData.ItemsSource = ListPicking;
                    LblPickingdate.Text = Helpers.Controls.Date2ThaiString(ActiveData.Pickingdate, "d-MMMM-yyyy");
                    ShowHeader();
                }
            }
            catch { }
            AidWaitingRun(false);
        }

        void ShowHeader()
        {
            if (ListPicking != null && ListPicking.Count > 0)
            {
                ActiveData.Totalline = ListPicking.Count;
                ActiveData.Totalunit = ListPicking.Sum(x => x.Qty);
                double amount = ListPicking.Sum(x => (x.Price * x.Qty));
                string detail = "จำนวน " + ActiveData.Totalline + "รายการ";
                detail += ", เบิก " + ListPicking.Count.ToString() + " ขวด";
                detail += ", รวมเป็นเงิน " + amount.ToString("#,##0.00") + " บาท";
                LblPickingSumary.Text = detail;
                //BtnSend.IsVisible = (ActiveData.Transtatus == -1);
                ListData.ItemsSource = null;
                ListData.ItemsSource = ListPicking;
            }
            LblNodata.IsVisible = (ListPicking == null || ListPicking.Count == 0);

        }


        void AidWaitingRun(bool running, string msg = "")
        {
            try
            {
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