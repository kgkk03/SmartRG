using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    

    public partial class PasswordPage : ContentPage
    {

        string OwnerPage = "EditPassworePage";
        string Password = "";
        string ConfirmPassword = "";
        bool IsConfirm = false;
        bool HidePassword = false;

        public PasswordPage()
        {
            InitializeComponent();
        }
        public void Setdata(string ownerpage, bool confirm = false)
        {
            OwnerPage = ownerpage;
            IsConfirm = confirm;
            SetHeader();
            FillPassword(0);
        }

        void SetHeader()
        {
            try
            {
                if (OwnerPage.Equals("LoginPage"))
                {
                    LblHeader.Text = "กำหนดรหัสผ่าน";
                    Lblusername.IsVisible = false;
                }
                else
                {
                    LblHeader.Text = "แก้ไขรหัสผ่าน";
                    Lblusername.IsVisible = true;
                    Lblusername.Text = App.UserProfile.Fullname;
                }
            }
            catch { }
        }
        private void btncal_Clicked(object sender, EventArgs e)
        {
            var result = (ImageButton)sender;
            if (result.StyleId.Equals("hide")) { SetHidePassword(result); }
            else if (result.StyleId.Equals("back"))
            {
                if (Password.Length > 0) { Password = Password.Substring(0, Password.Length - 1); }
            }
            else { if (Password.Length < 7) { Password += result.StyleId; } }
            Showpassoword();
        }
        void SetHidePassword(ImageButton btn)
        {
            HidePassword = !HidePassword;
            Showpassoword();
            btn.Source = HidePassword? "ic_pwdshow" : "ic_pwdhide";
        }
        async void Showpassoword()
        {
            int i = 0;
            foreach (char pw in Password)
            {
                if (i == 0) { lblpw1.Text = HidePassword ? "*" : pw.ToString(); }
                else if (i == 1) { lblpw2.Text = HidePassword ? "*" : pw.ToString(); }
                else if (i == 2) { lblpw3.Text = HidePassword ? "*" : pw.ToString(); }
                else if (i == 3) { lblpw4.Text = HidePassword ? "*" : pw.ToString(); }
                else if (i == 4) { lblpw5.Text = HidePassword ? "*" : pw.ToString(); }
                else if (i == 5) { lblpw6.Text = HidePassword ? "*" : pw.ToString(); }
                i++;
            }
            if (i < 6) { FillPassword(i); }
            else if(i==6)
            {
                if (IsConfirm && ConfirmPassword.Equals("")) {
                    ConfirmPassword = Password;
                    Password = "";
                    FillPassword(0);
                }
                else
                {
                    if (IsConfirm)
                    {
                        if (ConfirmPassword.Equals(Password)) { 
                            GotoOwnerPage(true);
                        }
                        else
                        {
                            await DisplayAlert("Password Error", "รหัสผ่านไม่ตรงกันกรุณาลองอีกครั้ง", "OK");
                            ConfirmPassword = "";
                            Password = "";
                            FillPassword(0);
                        }
                    }
                    else
                    {
                        GotoOwnerPage(true);
                    }
                }
            }
        }
        void FillPassword(int start)
        {
            for (int i = start; i < 6; i++)
            {
                if (i == 0) { lblpw1.Text = ""; }
                else if (i == 1) { lblpw2.Text = ""; }
                else if (i == 2) { lblpw3.Text = ""; }
                else if (i == 3) { lblpw4.Text = ""; }
                else if (i == 4) { lblpw5.Text = ""; }
                else if (i == 5) { lblpw6.Text = ""; }
            }
        }

        private void Btnback_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }

        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(true);
        }

        async void GotoOwnerPage(bool success = false)
        {
            string result = "";
            if (success) { result = Password; }
            MessagingCenter.Send<PasswordPage, string>(this, OwnerPage, result);
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