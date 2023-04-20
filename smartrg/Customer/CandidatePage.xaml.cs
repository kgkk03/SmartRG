using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CandidatePage : ContentPage
    {
        string OwnerPage = "CandidatePage";
        string Mypage = "CandidatePage";
        int EditStep = 0;
        public Models.CustomerData ActiveCustomer = new Models.CustomerData();
        public Models.Imagedata ActiveImage = new Models.Imagedata() ;
        bool IsEdit = false;
        public CandidatePage()
        {
            InitializeComponent();
        }
      

        public async void Setdata(string ownerpage, Location mylocation)
        {
            OwnerPage = ownerpage;
            ActiveCustomer = await GetNewCandidate(mylocation);
            ActiveImage = new Models.Imagedata() { canedit = true, Image = null , Imagefile = "Candiate" }; 
            ShowEdit(0);
        }

        async Task<Models.CustomerData> GetNewCandidate(Location mylocation)
        {
            try {
                string address = await Helpers.Controls.GetLocationname(mylocation.Latitude, mylocation.Longitude);
                Models.Admindata admindata = await App.Ws.GetAdminData(mylocation.Latitude, mylocation.Longitude);
                Models.CustomerData result = new Models.CustomerData()
                {
                    Key = Helpers.Controls.GetID()+ App.UserProfile.Empid,
                    Empid = App.UserProfile.Empid,
                    Lat = mylocation.Latitude,
                    Lng = mylocation.Longitude,
                    Custaddress = address,
                    Adminname = admindata.AdminName,
                    Admincode = admindata.AdminCode,
                };
                return await Task.FromResult(result);
            }
            catch { }
            return await Task.FromResult(new Models.CustomerData());
        }

        
        void ShowData()
        {
            LblCustID.Text = ActiveCustomer.Key;

            ImgCustType.Source = ActiveCustomer.Icon.Equals("")?"ic_shop": ActiveCustomer.Icon;
            LblCustGroup.Text = ActiveCustomer.Custgroupname;
            LblCustCode.Text = " (" + ActiveCustomer.Custcode + ")";

            LblCustName.Text = ActiveCustomer.Custname;
            LblCustPhone.Text = ActiveCustomer.Custphone;
            LblAddresse.Text = ActiveCustomer.Custaddress;

            LblContractname.Text = ActiveCustomer.Contractname;
            LblContractphone.Text = ActiveCustomer.Contractmobile;

            LblRemark.Text = ActiveCustomer.Remark;

            LblProvince.Text = ActiveCustomer.Adminname;
            LblPosition.Text = ActiveCustomer.Lat.ToString("0.000000") + " " + ActiveCustomer.Lng.ToString("0.000000");

        }

        private async void BtnEditImage_Clicked(object sender, EventArgs e)
        {

            if (IsEdit) { return; }
            AidWaitingRun(true);
            IsEdit = true;
            var page = new Tools.EditImagePage();
            page.Setdata(Mypage, Title, ActiveImage);
            await Navigation.PushModalAsync(page);
            EditImagePageMessage();
            AidWaitingRun(false);
        }
        void EditImagePageMessage()
        {
            try
            {

                MessagingCenter.Subscribe<Tools.EditImagePage, Models.Imagedata>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { EditImage(arg); } catch { }
                        MessagingCenter.Unsubscribe<Tools.EditImagePage, Models.Imagedata>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        async void EditImage(Models.Imagedata value)
        {
            try
            {
                AidWaitingRun(true);
                value.Thumbnail = await Helpers.ImageConvert.ResizeImage(value.ImageBase64, 50);
                ImgCustomer.Source = value.Image;
                //Imgbk.Source = ImgCustomer.Source;
            }
            catch { }
            IsEdit = false;
            AidWaitingRun(false);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Button item = (Button)sender;
            ShowEdit(int.Parse(item.StyleId));
        }
        void SaveEdit(int nextstep)
        {
            if (Txtinput.Text==null) { Txtinput.Text=""; }
            if (EditStep == 0) {  }
            else if (EditStep == 1) { ActiveCustomer.Custcode = Txtinput.Text; }
            else if (EditStep == 2) { ActiveCustomer.Custname = Txtinput.Text; }
            else if (EditStep == 3) { ActiveCustomer.Custphone = Txtinput.Text; }
            else if (EditStep == 4) { ActiveCustomer.Custaddress = Txtinput.Text; }
            else if (EditStep == 5) { ActiveCustomer.Contractname = Txtinput.Text; }
            else if (EditStep == 6) { ActiveCustomer.Contractmobile = Txtinput.Text; }
            else if (EditStep == 7) { ActiveCustomer.Remark = Txtinput.Text; }
            int index = EditStep + nextstep;
            if (index == EditStep || index < 0 || index > 7) {
                StkEditText.IsVisible = false;
                StkEditList.IsVisible = false;
                ShowData();
            }
            else { ShowEdit(index); }
        }

        void ShowEdit(int nextstep)
        {
            EditStep = nextstep;
            if (EditStep == 0 ) { ShowEditList(); }
            else { ShowEditText(); }
        }

        void ShowEditText()
        {
            StkEditText.IsVisible = true;
            StkEditList.IsVisible = false;
            Txtinput.Keyboard = default;
            if (EditStep == 1) { LblTitleEdit.Text = "รหัสลูกค้า"; Txtinput.Text = ActiveCustomer.Custcode; Txtinput.Placeholder = "ระบุรหัสลูกค้า"; }
            else if (EditStep == 2) { LblTitleEdit.Text = "ชื่อร้านค้า"; Txtinput.Text = ActiveCustomer.Custname; Txtinput.Placeholder = "ระบุชื่อร้านค้า"; Txtinput.Keyboard = Keyboard.Default; }
            else if (EditStep == 3) { LblTitleEdit.Text = "เบอร์โทรร้านค้า"; Txtinput.Text = ActiveCustomer.Custphone; Txtinput.Placeholder = "ระบุเบอร์โทรร้านค้า"; Txtinput.Keyboard = Keyboard.Telephone; }
            else if (EditStep == 4) { LblTitleEdit.Text = "ที่อยู่ร้านค้า"; Txtinput.Text = ActiveCustomer.Custaddress; Txtinput.Placeholder = "ระบุที่อยู่ร้านค้า"; Txtinput.Keyboard = Keyboard.Default; }
            else if (EditStep == 5) { LblTitleEdit.Text = "ชื่อผู้ติดต่อ"; Txtinput.Text = ActiveCustomer.Contractname; Txtinput.Placeholder = "ระบุชื่อผู้ติดต่อ"; Txtinput.Keyboard = Keyboard.Default; }
            else if (EditStep == 6) { LblTitleEdit.Text = "เบอร์โทรผู้ติดต่อ"; Txtinput.Text = ActiveCustomer.Contractmobile; Txtinput.Placeholder = "ระบุเบอร์โทรผู้ติดต่อ"; Txtinput.Keyboard = Keyboard.Telephone; }
            else if (EditStep == 7) { LblTitleEdit.Text = "หมายเหตุ"; Txtinput.Text = ActiveCustomer.Remark; Txtinput.Placeholder = "หมายเหตุ"; Txtinput.Keyboard = Keyboard.Default; }
            else { StkEditText.IsVisible = false; return; }
            Txtinput.Focus();
        }
        void ShowEditList()
        {
            StkEditList.IsVisible = true;
            StkEditText.IsVisible = false;
            if (EditStep == 0) {  
                LblTitleList.Text = "กลุ่มร้านค้า"; PikInput.Title = "ระบุกลุ่มร้านค้า";
                List<Models.CustomerType> listdata = App.dbmng.sqlite.Table<Models.CustomerType>().OrderBy(x => x.Priority).ThenBy(x => x.Groupname).ToList();
                PikInput.ItemsSource = listdata;
                PikInput.ItemDisplayBinding = new Binding("Groupname");
                if (ActiveCustomer.Custgroupid != 0) {
                    var index = listdata.FindIndex(x => x.PriceID == ActiveCustomer.Custgroupid);
                    if (index > -1) { PikInput.SelectedIndex = index; }
                }
            }
            else if (EditStep == 4) { LblTitleList.Text = "จังหวัด"; PikInput.Title = "ระบุจังหวัด"; }
            else { StkEditList.IsVisible = false; }
        }

        private void Txtinput_Completed(object sender, EventArgs e)
        {
            SaveEdit(1);
        }
        private void PikInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditStep==0) {
                Models.CustomerType item = (Models.CustomerType)PikInput.SelectedItem;
                if(item == null) { return; }
                ActiveCustomer.Typeid = item.TypeID;
                ActiveCustomer.Typename = item.typename;
                ActiveCustomer.Custgroupid = item.PriceID;
                ActiveCustomer.Custgroupname = item.Groupname;
                ActiveCustomer.Icon = item.Icon;
            }else if (EditStep == 4)
            {

            }
        }
        private void BtnBack_Clicked(object sender, EventArgs e)
        {
            SaveEdit(-1);
        }

        private void BtnNext_Clicked(object sender, EventArgs e)
        {
            SaveEdit(1);
        }
        private void BtnOK_Clicked(object sender, EventArgs e)
        {
            SaveEdit(0);
        }




        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            AidWaitingRun(true, "กำลังบันทึกข้อมูลลูกค้าใหม่...");
            if (await SaveCandidate()) { GotoOwnerPage(true); }
            AidWaitingRun(false);
        }
        async Task<bool>  SaveCandidate()
        {
            try {
                string msg = CheckSaveCandidate();
                if (msg.Equals("")) {

                    Models.ResultUpdate result = await App.Ws.SaveCandidate(ActiveCustomer);
                    if (result.Code == "000")
                    {
                        if (!ActiveImage.Thumbnail.Equals(""))
                        {
                            //Models.CustImage img = new Models.CustImage() { Custid = ActiveCustomer.Custid, Icon = ActiveImage.Thumbnail, Image = ActiveImage.ImageBase64 };
                            var resultimg = await App.Ws.SaveCuctImage(new Models.CustImage() { Key = ActiveCustomer.Key, Icon = ActiveImage.Thumbnail, Image = ActiveImage.ImageBase64 });
                            App.dbmng.InsetData(new Models.CustImage() { Key = ActiveCustomer.Key, Icon = ActiveImage.Thumbnail });
                        }
                        return await Task.FromResult(true);
                    }
                    else
                    {
                        if(result.Message.Contains("Duplicate"))
                        {
                            await DisplayAlert("แจ้งเตือน", "ไม่สามารถบันทึกข้อมูลลูกค้าใหม่ได้ \n มีลูกค้าอยู่ในระบบแล้ว โปรดตรวจสอบอีกครั้ง", "ยกเลิก");
                            return await Task.FromResult(false); 
                        }
                    }
                }
                else{ 
                    await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                    return await Task.FromResult(false);
                }                
            }
            catch { }
            await DisplayAlert("แจ้งเตือน", "ไม่สามารถบันทึกข้อมูลลูกค้าใหม่ได้ \n โปรดตรวจสอบการเชื่อมต่ออินเตอร์เน็ต", "ตกลง"); 
            return await Task.FromResult(false);
        }

        string CheckSaveCandidate()
        {
            string msg = "";
            try
            {
                if (ActiveCustomer.Custgroupname.Trim().Equals("")) { msg = "กรุณาระบุกลุ่มลูกค้า"; EditStep = 0; return msg; }
                if (ActiveCustomer.Custcode.Trim().Equals("")) { msg = "กรุณาระบุรหัสลูกค้า"; EditStep = 1; return msg; }
                if (ActiveCustomer.Custname.Trim().Equals("")) { msg = "กรุณาระบุชื่อลูกค้า"; EditStep = 2; return msg; }
            }
            catch { }
            return msg;
        }


        private async  void BtnExit_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("", "รายการเพิ่มลูกค้าใหม่ยังไม่ได้บันทึก \n ต้องการออกจากหน้านี้โดยไม่บันทึกใช่หรือไม่", "ออกโดยไม่บันทึก", "ไม่ออก"))
            { GotoOwnerPage(false); }
        }

        async void GotoOwnerPage(bool success = false)
        {
            Models.CustomerData result = null;
            if (success) { result = ActiveCustomer; }
            MessagingCenter.Send<CandidatePage, Models.CustomerData>(this, OwnerPage, result);
            await Navigation.PopModalAsync();
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