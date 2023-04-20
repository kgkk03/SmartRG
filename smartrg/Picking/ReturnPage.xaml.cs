using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Picking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReturnPage : ContentPage
    {
        string OwnerPage = "ReturnPage";
        Models.PickingData ActiveData ;
        List<Models.PickingLineData> Listpicking = new List<Models.PickingLineData>();
        List<Models.PickingLineData> ListReturn = new List<Models.PickingLineData>();
        Models.CustomerData Customer = new Models.CustomerData();
        bool IsEdit = false;
        public ReturnPage()
        {
            InitializeComponent();
        }

        void ShowHeader()
        {
            if (ListReturn != null && ListReturn.Count > 0)
            {
                ActiveData.Totalline = ListReturn.Count;
                ActiveData.Totalunit = ListReturn.Sum(x => x.Qty);
                LblReturnSumary.Text = GetHeader(ListReturn,"คืน");
                BtnSend.IsVisible = (ActiveData.Transtatus == -1);
                BtnPrint.IsVisible = !BtnSend.IsVisible;
                ListData.ItemsSource = null;
                ListData.ItemsSource = ListReturn;
            }
            LblNodata.IsVisible = (ListReturn == null || ListReturn.Count == 0);
        }
        string GetHeader(List<Models.PickingLineData> data,string header)
        {
            string result = header + ": ====== ไม่พบรายการ" + header + " ======";
            if (ListReturn != null && ListReturn.Count > 0)
            {
                double amount = data.Sum(x => (x.Price * x.Qty));
                int totalline = data.Count;
                int totalunit = data.Sum(x => x.Qty);
                //เบิก: ...รายการ, ...ขวด, รวม...บาท
                result = header + ": " + totalline.ToString() + " รายการ";
                result += "   จำนวน : " + totalunit.ToString() + " ขวด";
                result += "   รวมเป็นเงิน : " + amount.ToString("#,##0.00") + " บาท";
            }
            return result;
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
                    if (MT.AllPickingLine == null) { MT.AllPickingLine = await App.Ws.GetPickingDetail(ActiveData.Key); }
                    if (MT.AllPickingLine!=null && MT.AllPickingLine.Count > 0)
                    {
                        Listpicking = MT.AllPickingLine;
                        ListReturn = Listpicking.Where(x=>x.Pickingtype.Equals("คืน")).OrderBy(X=>X.Item).ToList();
                        var pickingdata = Listpicking.Where(x => x.Pickingtype.Equals("เบิก")).OrderBy(X => X.Item).ToList();
                        var saledata = Listpicking.Where(x => x.Pickingtype.Equals("ขาย")).OrderBy(X => X.Item).ToList();
                        LblPickingSumary.Text = GetHeader(pickingdata, "เบิก");
                        LblSaleSumary.Text = GetHeader(saledata, "ขาย");

                    }
                    ListData.ItemsSource = ListReturn;
                    LblPickingdate.Text = Helpers.Controls.Date2ThaiString(ActiveData.Pickingdate, "d-MMMM-yyyy");
                    LblReturnSumary.Text = GetHeader(ListReturn, "คืน");
                    ShowHeader();
                }
            }
            catch { }
            AidWaitingRun(false);
        }

        #region New Picking
        public async void SetNewData(string ownerpage, TabReturnPage MT)
        {
            try
            {
                AidWaitingRun(true);
                OwnerPage = ownerpage;
                Customer = MT.Customer;
                if (Customer != null)
                {
                    LblCustname.Text = Customer.Custname + " (" + Customer.Custgroupname + ")";
                    ImgCustomer.Source = Customer.Icon;
                    LblAddress.Text = Customer.Custaddress;
                    LblPickingSumary.Text = "";
                    MT.ActiveData = GetNewData(Customer);
                    ActiveData = MT.ActiveData;
                    MT.AllPickingLine = await GetListProduct();
                    if(MT.AllPickingLine!=null&& MT.AllPickingLine.Count > 0)
                    {
                        ListReturn = await GetReturnProduct(MT.AllPickingLine);
                        ListData.ItemsSource = ListReturn;
                        LblPickingdate.Text = Helpers.Controls.Date2ThaiString(ActiveData.Modified, "d-MMMM-yyyy");
                        var pickingdata = MT.AllPickingLine.Where(x => x.Pickingtype.Equals("เบิก")).OrderBy(X => X.Item).ToList();
                        var saledata = MT.AllPickingLine.Where(x => x.Pickingtype.Equals("ขาย")).OrderBy(X => X.Item).ToList();
                        LblPickingSumary.Text = GetHeader(pickingdata, "เบิก");
                        LblSaleSumary.Text = GetHeader(saledata, "ขาย");
                    }
                }
                ShowHeader();
            }
            catch { }
            AidWaitingRun(false);
        }
        Models.PickingData GetNewData(Models.CustomerData customer)
        {
            try
            {
                DateTime req = App.Servertime.AddDays(2);
                var result = new Models.PickingData()
                {
                    Visitid = Helpers.Controls.GetID() + App.UserProfile.Empid.ToString().PadLeft(3, '0'),
                    Agentcode = customer.Custcode,
                    Agentid = customer.Key,
                    Agentname = customer.Custname,
                    Empfullname = App.UserProfile.Fullname,
                    Empid = App.UserProfile.Empid,
                    Pickingdate = App.Servertime,
                    Pickingtype = "คืน",
                    Typeid = -1,
                };
                result.Key = result.Visitid + "-0";
                return result;
            }
            catch (Exception ex) { DisplayAlert("GetNew PickingData Error", ex.Message, "OK"); }
            return null;
        }
        async Task<List<Models.PickingLineData>> GetListProduct()
        {
            try
            {
                string agenid = Customer.Key;
                List<Models.PickingLineData> result = await App.Ws.GetReturnStock(agenid);
                return await Task.FromResult(result);
            }
            catch (Exception ex) { await DisplayAlert("VisitAgentPickingPage GetListProduct Error", ex.Message, "OK"); return null; }
        }
        async Task<List<Models.PickingLineData>> GetReturnProduct(List<Models.PickingLineData> data)
        {
            List<Models.PickingLineData> result = new List<Models.PickingLineData>();
            try
            {
                if (data != null && data.Count > 0)
                {
                    result = data.Where(x => x.Pickingtype.Equals("คืน")).OrderBy(X => X.Item).ToList();
                    if (result != null && result.Count > 0)
                    {
                        int i = 1;
                        foreach (var dr in result)
                        {
                            dr.Pickingid = ActiveData.Key;
                            dr.Item = i;
                            dr.Key = ActiveData.Key + "-" + i.ToString();
                            i++;
                        }
                    }
                }
            }
            catch (Exception ex) { await DisplayAlert("VisitAgentPickingPage GetListProduct Error", ex.Message, "OK"); return null; }
            return await Task.FromResult(result);
        }
    
        #endregion




        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            // Save Data
            if (IsEdit || ActiveData.Transtatus != -1) { return; }
            IsEdit = true;
            AidWaitingRun(true, "กำลังส่งข้อมูลการคืนสินค้า...");
            int success = 0;
            bool error = false;
            string msg = "ข้อมูลที่ส่งไปแล้วจะไม่สามารถแก้ไขได้\n" + "คุณต้องการกรอกข้อมูลต่อหรือส่งรายการคืนสินค้า";
            if (await DisplayAlert("แจ้งเตือน", msg, "ส่งรายการคืนสินค้า", "ยังไม่ส่ง"))
            {
                foreach (var dr in ListReturn)
                {
                    AidWaitingRun(true, "กำลังส่งข้อมูลรายการคืนสินค้า...\n" + dr.Productname);
                    //dr.Transtatus = 2;
                    msg = await App.Ws.SaveClearstock(dr);
                    if (msg.Equals("")) { 
                        success += 1;
                    }
                    else { 
                        error = true; 
                    }
                }

                if (error)
                {
                    msg = "ไม่สามารถส่งข้อมูลรายการคืนสินค้าได้จำนวน " + (ListReturn.Count - success).ToString()
                        + "รายการ\nโปรดตรวจสอบอินเตอร์เน็ตแล้วส่งอีกครั้ง";
                    await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                    IsEdit = false;
                    AidWaitingRun(false);
                    return;
                }
                else
                {
                    ActiveData.Transtatus = 0;
                    ActiveData.Showtime = Helpers.Controls.Date2String(App.Servertime, "HH:mm");
                    msg = await App.Ws.SavePicking(ActiveData);
                    if (!msg.Equals(""))
                    {
                        ActiveData.Transtatus = -1;
                        msg = "ไม่สามารถส่งข้อมูลสรุปการคืนสินค้าได้\nโปรดตรวจสอบอินเตอร์เน็ตแล้วส่งอีกครั้ง";
                        await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                        IsEdit = false;
                        AidWaitingRun(false);
                        return;
                    }
                    else
                    {
                        await DisplayAlert("ส่งข้อมูลเรียบร้อย", msg, "ตกลง");
                        ActiveData.Transtatus = -1;
                        GotoOwnerPage(true);
                        AidWaitingRun(false);
                        IsEdit = false;
                    }
                }
            }
            IsEdit = false;
            AidWaitingRun(false);
        }
        private async void BtnPrint_Clicked(object sender, EventArgs e)
        {
            try
            {
                Models.BillCompany company = App.dbmng.sqlite.Table<Models.BillCompany>().Where(x => x.Transtatus == 0).FirstOrDefault();
                if (company == null) { company = new Models.BillCompany(); }
                var pdf = new Helpers.PrintPDF();
                await pdf.PrintClearstock(company, ActiveData, Listpicking);
            }
            catch { }
        }

            private async void BtnExit_Clicked(object sender, EventArgs e)
        {
            if (ActiveData.Transtatus == -1)
            {
                if (!await DisplayAlert("", "รายการคืนสินค้ายังไม่ได้บันทึก \n ต้องการออกจากหน้านี้โดยไม่บันทึกใช่หรือไม่", "ออกโดยไม่บันทึก", "ไม่ออก"))
                { return; }
            }
            GotoOwnerPage(false);
        }
        async void GotoOwnerPage(bool success = false)
        {
            Models.PickingData result = null;
            if (ActiveData.Transtatus == -1)
            {
                if (success) { ActiveData.Transtatus = 0; result = ActiveData; }
                MessagingCenter.Send<ReturnPage, Models.PickingData>(this, OwnerPage, result);
            }
            //await Navigation.PopModalAsync();
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