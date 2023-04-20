using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
   
    public partial class ProfilePage : ContentPage
    {
        string OwnerPage = "ProfilePage";
        string Mypage = "ProfilePage";
        string EditTitle = "";
        bool IsEdit = false;
        bool CanEditProfile = true;
        bool CanEditPassword = true;
        Models.UserProfile ActiveUser = App.UserProfile;
        public ProfilePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            //CanEditProfile = true;
            //แก้ไขเพื่อให้แก้ไขโปรไฟล์ไม่ได้
            CanEditProfile = false;
            CanEditPassword = true;
            BtnMenu.IsVisible = true;
            lblVersion.Text = "Ver. " + VersionTracking.CurrentVersion;
            Setdata(App.UserProfile);
        }

        public ProfilePage(string ownerpage, Models.UserProfile value)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            CanEditProfile = false;
            CanEditPassword = false;
            BtnMenu.IsVisible = false;
            OwnerPage = ownerpage;
            Setdata(value);
        }
     
        public void Setdata(Models.UserProfile value)
        {
            ActiveUser = value;
            ShowProfile();
        }
        private async void ShowProfile()
        {
            LblTeamname.Text = ActiveUser.Authen + " (" + ActiveUser.TeamName + ")";
            LblUserID.Text = ActiveUser.Empcode;
            LblFullname.Text = ActiveUser.Fullname;
            LblPhone.Text = ActiveUser.Phon;
            LblResetLogin.Text = ActiveUser.Key;
            ImgUser.Source = await Helpers.Controls.GetProfileImage();
        }

        #region Image

        private void BtnImgUser_Clicked(object sender, EventArgs e)
        {
            ImageSource Userimg = ImgUser.Source;
            ShowEditImage("Userimg", "รูปภาพผู้ใช้งาน", CanEditProfile, Userimg);
        }
        async void ShowEditImage(string Key, string Title, bool canedit, ImageSource value)
        {

            //if (IsEdit || !CanEditProfile) { return; }
            AidWaitingRun(true);
            EditTitle = Key;
            var page = new Tools.EditImagePage();
            Models.Imagedata img = new Models.Imagedata()
            {
                canedit = canedit,
                Image = value,
                Imagefile = "UserProfile.jpg"
            };
            page.Setdata(Mypage, Title, img);
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
            catch (Exception ex) { DisplayAlert("EditImagePageMessage Error", ex.Message, "OK"); }
        }
        async void EditImage(Models.Imagedata value)
        {
            try
            {
                if (!EditTitle.Equals(""))
                {
                    AidWaitingRun(true);
                    if (EditTitle.Equals("Userimg"))
                    {
                        ImgUser.Source = value.Image;
                        Helpers.Settings.ProfileImage = value.Imagefile;
                        value.Thumbnail = await Helpers.ImageConvert.ResizeImage(value.ImageBase64, 50);
                        await App.Ws.SaveUserImage(value);
                        // ส่งข้อมูลไปเก็บที่ server ด้วย
                    }
                }
            }
            catch { }
            EditTitle = "";
            IsEdit = false;
            AidWaitingRun(false);
        }


        #endregion

        #region Text
        private void BtnFullname_Clicked(object sender, EventArgs e)
        {
            ShowEditText("FullName", "ชื่อ-นามสกุล ผู้ใช้งาน", Keyboard.Text, "ระบุชื่อ-นามสกุล ผู้ใช้งาน", LblFullname.Text);
        }
        private void BtnPhone_Clicked(object sender, EventArgs e)
        {
            ShowEditText("PhoneNumber", "เบอร์โทรศัพท์ ผู้ใช้งาน", Keyboard.Telephone, "ระบุเบอร์โทรศัพท์ ผู้ใช้งาน", LblPhone.Text);
        }
        private void BtnResetLogin_Clicked(object sender, EventArgs e)
        {
            ShowEditText("LoginName", "Login name", Keyboard.Text, "ระบุ Login name", "");
        }
        async void ShowEditText(string Key, string Title, Keyboard Keyboardtype, string Placeholder, string value = "")
        {
            if (IsEdit || !CanEditProfile) { return; }
            AidWaitingRun(true);
            EditTitle = Key;
            var page = new Tools.EditTextPage();
            page.Setdata(Mypage, Title, Keyboardtype, Placeholder, value);
            await Navigation.PushModalAsync(page);
            EditTextPageMessage();
            AidWaitingRun(false);
        }
        void EditTextPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.EditTextPage, string>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { EditProfile(arg); } catch { }
                        MessagingCenter.Unsubscribe<Tools.EditTextPage, string>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        void EditProfile(string value)
        {
            try
            {
                if (!EditTitle.Equals(""))
                {
                    AidWaitingRun(true);
                    if (EditTitle.Equals("FullName")) { LblFullname.Text = value; }
                    else if (EditTitle.Equals("PhoneNumber")) { LblPhone.Text = value; }
                    else if (EditTitle.Equals("LoginName"))
                    {
                        //Loginname = value;
                    }
                }
            }
            catch { }
            EditTitle = "";
            IsEdit = false;
            AidWaitingRun(false);
        }
        #endregion


        #region Password
        async void BtnPassword_Clicked(object sender, EventArgs e)
        {
            if (IsEdit || !CanEditPassword) { return; }
            AidWaitingRun(true);
            EditTitle = "Password";
            var page = new Tools.PasswordPage();
            page.Setdata(Mypage, true);
            await Navigation.PushModalAsync(page);
            PasswordPageMessage();
            AidWaitingRun(false);
        }
        void PasswordPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.PasswordPage, string>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { 
                            EditPassword(arg); 
                        } 
                        catch { }
                        MessagingCenter.Unsubscribe<Tools.PasswordPage, string>(this, Mypage);
                    });
                });

            }
            catch (Exception ex) { DisplayAlert("PasswordPageMessage Error", ex.Message, "OK"); }
        }
        async void EditPassword(string value)
        {
            try
            {
                //if (!EditTitle.Equals(""))
                if (!value.Equals("") && (!(value.Length != 6)))
                {
                    AidWaitingRun(true);
                    string md5pwd = Helpers.Controls.GetMd5Password(value).ToUpper();
                    if(await App.Ws.SaveUserPassword(md5pwd)) {
                        await DisplayAlert("แจ้งเตือน", "บันทึกรหัสผ่านใหม่เรียบร้อย", "ตกลง");
                    }
                    else {
                        await DisplayAlert("แจ้งเตือน", "บันทึกรหัสผ่านใหม่ไม่สำเร็จ", "ตกลง");
                    }
                    //password = value;
                }
            }
            catch { }
            EditTitle = "";
            IsEdit = false;
            AidWaitingRun(false);
        }
        #endregion


        private void BtnPrinter_Clicked(object sender, EventArgs e)
        {

        }
        private void BtnMenu_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<ProfilePage, bool>(this, "OpenMenu", true);

        }
        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(true);
        }
        private void Btnback_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }
       
        private async void BtnUpdate_Clicked(object sender, EventArgs e)
        {
            AidWaitingRun(true, "กำลังอัพเดทข้อมูล...");
            var page = new UpdateDataPage();
            page.AutoUpdate(Mypage);
            await Navigation.PushModalAsync(page);
            StartUpdateDataPageMessage();
        }
        void StartUpdateDataPageMessage()
        {
            MessagingCenter.Subscribe<UpdateDataPage, bool>(this, Mypage, (sender, arg) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        MessagingCenter.Unsubscribe<UpdateDataPage, bool>(this, Mypage);
                        AidWaitingRun(false);
                    }
                    catch { }
                });
            });
        }

        async void GotoOwnerPage(bool success = false)
        {
            Models.UserProfile result = null;
            if (success) { result = ActiveUser; }
            MessagingCenter.Send<ProfilePage, Models.UserProfile>(this, OwnerPage, result);
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