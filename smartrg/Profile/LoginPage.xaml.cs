using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class LoginPage : ContentPage
    {
        string Mypage = "LoginPage";
        bool IsEdit = false;
        string Password = "";
        
        public LoginPage()
        {
            InitializeComponent();
            txtLogin.Text = Helpers.Settings.Loginname;
            lblVersion.Text = "Ver. " + VersionTracking.CurrentVersion;

            //TestRun();
        }
        void TestRun()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Autologin();
                //return true; // return true to repeat counting, false to stop timer
                return false;
            });
        }
        async void Autologin()
        {
            txtLogin.Text = "bwon";
            Password = "123456";
            //Helpers.Settings.TeamID = 0;
            //Helpers.Settings.ProfileImage = "";
            await CheckLogin();
        }
        private void BtnRegister_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new RegisterPage();
        }
        private void BtnLogin_Clicked(object sender, EventArgs e)
        {
            GotoPassword();
        }
        private void txtLogin_Completed(object sender, EventArgs e)
        {
            GotoPassword();
        }
        async void GotoPassword()
        {
            if (txtLogin.Text != null && !txtLogin.Text.Equals(""))
            {
                if (IsEdit) { return; }
                await AidWaitingRun(true, "กำลังตรวจสอบข้อมูล...");
                var page = new Tools.PasswordPage();
                page.Setdata(Mypage, false);
                await Navigation.PushModalAsync(page);
                StartPasswordMessage();
                await AidWaitingRun(false);
            }
            else
            {
                await DisplayAlert("Email Error", "กรุณาระบุ E-Mail หรือ Login User", "OK");
            }
        }
        void StartPasswordMessage()
        {
            MessagingCenter.Subscribe<Tools.PasswordPage, string>(this, Mypage, (sender, arg) =>
            {
                Device.BeginInvokeOnMainThread(() => {
                    try
                    {
                        MessagingCenter.Unsubscribe<Tools.PasswordPage, string>(this, Mypage);
                        var s = sender;
                        var a = arg;
                        EditPassword(arg);
                    }
                    catch { }
                });
            });

        }
        async void EditPassword(string value)
        {
            try
            {
                if (value.Equals(""))
                {
                    await DisplayAlert("Password Error", "กรุณาใส่ข้อมูลรหัสผ่านให้ถูกต้อง", "OK");
                    IsEdit = false;
                }
                else { 
                    Password = value;
                    await AidWaitingRun(true, "กำลังตรวจสอบข้อมูล...");
                    await CheckLogin();
                }
            }
            catch { }
            await  AidWaitingRun(false);
        }
        async Task<bool> CheckLogin()
        {
            try
            {
                await AidWaitingRun(true, "กำลังตรวจสอบข้อมูล...");
                //Check Login & Get User
                string md5pwd =  Helpers.Controls.GetMd5Password(Password).ToUpper();
                App.loginname = txtLogin.Text;
                App.password = md5pwd;
                var login = await App.Ws.GetLogin(txtLogin.Text, md5pwd);
                if (login.Code.Equals("000"))
                {
                    if (login.Team == null|| login.Team.Count==0)   {
                        await DisplayAlert("แจ้งเตือน", "ผู้ใช้งานไม่มีสิทธิ์เข้าใช้งาน", "ตกลง");
                    }
                    else
                    {
                        if (login.Team != null && login.Team.Count == 1)
                        {
                            App.UserProfile = login.Data;
                            await Setteam(login.Team[0]);
                            await CheckTeam();
                            return await Task.FromResult(true);
                        }
                        else
                        {
                            App.UserProfile = login.Data;
                            List<Models.SelectObj<object>> result = new List<Models.SelectObj<object>>();
                            foreach (var data in login.Team)
                            {
                                var temp = new Models.SelectObj<object>();
                                temp.Display = data.TeamName + " (" + data.Authen + ")";
                                temp.Obj = data;
                                result.Add(temp);
                            }
                            var page = new Tools.EditSelectPage();
                            page.Setdata(Mypage, "เลือกทีมที่ต้องการทำงาน", result, "");
                            await Navigation.PushModalAsync(page);
                            StartEditSelectPageMessage();
                            return await Task.FromResult(true);
                        }

                    }
                }
                else { await DisplayAlert("แจ้งเตือน", login.Message, "ตกลง"); }
            }
            catch { await DisplayAlert("แจ้งเตือน", "ติดต่อเซิฟเวอร์ไม่ได้", "ตกลง"); }
            await AidWaitingRun(false);
            return await  Task.FromResult(false);
        }
        void StartEditSelectPageMessage()
        {
            MessagingCenter.Subscribe<Tools.EditSelectPage, Models.SelectObj<Object>>(this, Mypage, (sender, arg) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    MessagingCenter.Unsubscribe<Tools.EditSelectPage, Models.SelectObj<Object>>(this, Mypage);
                    try { UpdateTeam(arg); } catch { }
                });

            });
        }
        async void UpdateTeam(Models.SelectObj<Object> value)
        {
            if (value == null) {  
                await  DisplayAlert("Select Team Error", "กรุณาระบุทีมที่ต้องการทำงาน", "OK");
                await CheckLogin();
            }
            else
            {
                Models.UserTeam selectedteam = (Models.UserTeam)value.Obj;
                await Setteam(selectedteam);
                await CheckTeam();
            }
        }
       async Task<bool> Setteam(Models.UserTeam selectedteam)
        {
            try {
                App.UserProfile.Authen = selectedteam.Authen;
                App.UserProfile.Teamid = selectedteam.ID;
                App.UserProfile.TeamName = selectedteam.TeamName;
                App.UserProfile.Icon = selectedteam.Image;
                App.UserProfile.Role = selectedteam.Role;
            }
            catch { }
            return await Task.FromResult(true);
        }
        async Task<bool> CheckTeam()
        {
            int TeamID = -1;
            string Loginname = "";
            int Role = -1;

            Models.UserProfile lastprofile = App.dbmng.sqlite.Table<Models.UserProfile>().FirstOrDefault();
            if (lastprofile != null) { TeamID = lastprofile.Teamid; Loginname = lastprofile.Key; Role = lastprofile.Role; }
            bool olduser = (App.UserProfile.Teamid == TeamID && App.UserProfile.Key.Equals(Loginname) && App.UserProfile.Role== Role);
            if (olduser) { GotoStartPage(); }
            else {
                await App.dbmng.DeleteAlldata();
                App.dbmng.InsetData(App.UserProfile);
                GotoUpdatePage(); 
            }
            return await Task.FromResult(true);
        }
        async void GotoStartPage()
        {
            App.dbmng.ClearTable("profile");
            App.dbmng.InsetData(App.UserProfile);
            Helpers.Settings.Loginname = App.UserProfile.Key;
            App.Listmenu = await Helpers.Controls.GetListMenu(App.UserProfile.Role, App.UserProfile.Teamid);
            App.Current.MainPage = new Menu.MasterMenuPage(); 
        }
        async void GotoUpdatePage()
        {
            Helpers.Settings.ProfileImage = "";
            App.dbmng.DeleteAlldata();
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
                        if (arg) { GotoStartPage(); };
                    }
                    catch { }
                });
            });
        }
        async Task<List<Models.MenuList>> GetListMenu(int roleid, int teamid)
        {
            List<Models.MenuList> result = App.dbmng.sqlite.Table<Models.MenuList>().Where(x => x.roleid == roleid && x.Teamid == teamid).OrderBy(x => x.Piority).ToList();
            result.Add(new Models.MenuList() { Id = 999, Title = "ออกจากระบบ", Icon = "mnu_logout", Piority = 999 });
            return await Task.FromResult(result);
        }
        async  Task<bool> AidWaitingRun(bool running, string msg = "")
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
                await  DisplayAlert("AidWaitingRun Error", ex.Message, "OK");
                return await Task.FromResult(false);

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