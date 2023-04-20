using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        int CurentStep = 0;
        string Mypage = "RegisterPage";
        bool IsEdit = false;

        public RegisterPage()
        {
            InitializeComponent();
        }
      
        private async  void BtnNextStep_Clicked(object sender, EventArgs e)
        {
            BtnNextStep.IsVisible = false;
            await AidWaitingRun(true, "กำลังตรวจสอบข้อมูล...");
            var result = CheckData();
            if (result != CurentStep)
            {
                CurentStep = result;
                ShowObject();
            }
            await AidWaitingRun(false);
            BtnNextStep.IsVisible = true;

        }
        int CheckData()
        {
            try
            {

                return CurentStep + 1;
                
            }
            catch { }
            return CurentStep;

        }
        void ShowObject()
        {

            StkTeam.IsVisible = (CurentStep == 0);
            StkInsertUser.IsVisible = (CurentStep > 0 && CurentStep < 4);
            TxtEmail.IsVisible = (CurentStep == 1);
            TxtFullname.IsVisible = (CurentStep == 2);
            TxtPhone.IsVisible = (CurentStep == 3);
            StkShowUser.IsVisible = (CurentStep == 4);
            if (CurentStep > 4)
            {
                ShowPassword();
            }

        }
        async void ShowPassword()
        {
            var Page = new Tools.PasswordPage();
            Page.Setdata(Mypage,true);
            await Navigation.PushModalAsync(Page);
            PasswordPageMessage();
        }
        void PasswordPageMessage()
        {
            try
            {

                MessagingCenter.Subscribe<Tools.PasswordPage, string>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { SavePassword(arg); } catch { }
                        MessagingCenter.Unsubscribe<Tools.PasswordPage, string>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("PasswordPageMessage Error", ex.Message, "OK"); }
        }

        void SavePassword(string password)
        {
            GotoOwnerPage();
        }
        private async void BtnimgUser_Clicked(object sender, EventArgs e)
        {
            if (IsEdit ) { return; }
            await AidWaitingRun(true);
            var page = new Tools.EditImagePage();
            Models.Imagedata img = new Models.Imagedata()
            {
                canedit = true,
                Image = imgUser.Source,
            };
            page.Setdata(Mypage, "รูปภาพพนักงาน", img);
            await Navigation.PushModalAsync(page);
            EditImagePageMessage();
            await AidWaitingRun(false);
        }
        void EditImagePageMessage()
        {
            try
            {

                MessagingCenter.Subscribe<Tools.EditImagePage, string>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { EditImage(arg); } catch { }
                        MessagingCenter.Unsubscribe<Tools.EditImagePage, string>(this, Mypage);
                    });
                });


            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        async void EditImage(string value)
        {
            try
            {
               
            }
            catch { }
            IsEdit = false;
            await AidWaitingRun(false);
        }

        private void BtnClose_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage();
        }
        void GotoOwnerPage()
        {
            App.Current.MainPage = new ProfilePage();

        }
        async Task<bool> AidWaitingRun(bool running, string msg = "")
        {
            try
            {
                LblStatus.Text = msg;
                Stk_AidWaitingBk.IsVisible = running;
                Stk_AidWaiting.IsVisible = running;
                AidWaiting.IsVisible = running;
                AidWaiting.IsRunning = running;
                return await Task.FromResult(true);
            }
            catch (Exception ex) { 
                await DisplayAlert("AidWaitingRun Error", ex.Message, "OK");
                return await  Task.FromResult(false);
            }
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