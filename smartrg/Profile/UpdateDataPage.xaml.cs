using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateDataPage : ContentPage
    {
        string OwnerPage = "UpdateDataPage";
        bool UpdatSuccess = false;
        public UpdateDataPage()
        {
            try
            {
                InitializeComponent();
                if (Device.RuntimePlatform == Device.iOS) { AblMain.Margin = new Thickness(0, 30, 0, 0); }
            }
            catch (Exception ex) { DisplayAlert("Open UpdateBPPage  Error", ex.Message, "OK"); }
        }
      

        private async void btnUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                btnUpdate.IsVisible = false;
                if (await updatedata()) { await Task.Delay(3000); lblHeader.Text = "สำเร็จ"; await DisplayAlert("แจ้งเตือน", "อัพเดทข้อมูลเรียบร้อย", "ตกลง"); }
                else { await DisplayAlert("แจ้งเตือน", "อัพเดทข้อมูลมีปัญหาโปรดลองอีกครั้ง", "ยกเลิก"); }
                btnUpdate.IsVisible = true;
                MessagingCenter.Send<UpdateDataPage, bool>(this, "Close Page", true);
                await Navigation.PopModalAsync();
            }
            catch (Exception ex) { await DisplayAlert("UpdateBPPage btnUpdate_Clicked Error", ex.Message, "OK"); }
        }
        public void AutoUpdate(string ownerpage)
        {
            try
            {
                OwnerPage = ownerpage;
                btnUpdate.IsVisible = false;
                Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
                {
                    //App.dbmng.DeleteAlldata();
                    //App.dbmng.InsetData(App.UserProfile);
                    UpdateFirstTime();
                    return false;
                });
            }
            catch (Exception ex) { DisplayAlert("UpdateBPPage AutoUpdate Error", ex.Message, "OK"); }
        }
        async void UpdateFirstTime()
        {
            bool result = await updatedata();
            if (!result) { await DisplayAlert("แจ้งเตือน", "อัพเดทข้อมูลมีปัญหาโปรดอัพเดทข้อมูลก่อนใช้งาน", "ยกเลิก"); }
            UpdatSuccess = result;
            //WaittingUpdate();
            GotoOwnerPage(result);
        }
        async Task<bool> updatedata()
        {
            bool result = false;
            SetProcessAllValue(0);
            try
            {
                //========= อัพเดทข้อมูล พื้นฐานของระบบ ==================
                aidwitdata.IsRunning = true;

                #region "Configuration"

                lblHeader.Text = "ตรวจสอบข้อมูลระบบ";
                SetProcessAllValue(0.0);
                SetProcessValue(0);

                lblHeader.Text = "Get Command";
                if (await SetSyncData("devicecmd")) { lblUpdatedata.TextColor = Color.LimeGreen; }
                else { lblUpdatePlan.TextColor = Color.Red; }

                lblHeader.Text = "ตรวจสอบ รายการเมนูหลัก";
                if (await SetSyncData("menulist")) { lblUpdatedata.TextColor = Color.LimeGreen; }
                else { lblUpdatePlan.TextColor = Color.Red; }
                SetProcessAllValue(0.1);

                lblHeader.Text = "ตรวจสอบ รายการเมนูการเข้าพบ";
                if (await SetSyncData("visitpage")) { lblUpdatedata.TextColor = Color.LimeGreen; }
                else { lblUpdatePlan.TextColor = Color.Red; }
                SetProcessAllValue(0.2);

                lblHeader.Text = "ตรวจสอบ รายการเมนูลูกค้่า";
                if (await SetSyncData("customerpage")) { lblUpdatedata.TextColor = Color.LimeGreen; }
                else { lblUpdatePlan.TextColor = Color.Red; }
                SetProcessAllValue(0.3);

                lblHeader.Text = "ตรวจสอบ รายการประเภทลูกค้า";
                if (await SetSyncData("custtypelist")) { lblUpdatedata.TextColor = Color.LimeGreen; }
                else { lblUpdatePlan.TextColor = Color.Red; }
                SetProcessAllValue(0.4);

                lblHeader.Text = "ตรวจสอบ รายการ Customer Filter";
                if (await SetSyncData("custfilter")) { lblUpdatedata.TextColor = Color.LimeGreen; }
                else { lblUpdatePlan.TextColor = Color.Red; }
                SetProcessAllValue(0.5);

                lblHeader.Text = "ตรวจสอบ รายการ Image Type";
                if (await SetSyncData("imagetype")) { lblUpdatedata.TextColor = Color.LimeGreen; }
                else { lblUpdatePlan.TextColor = Color.Red; }
                SetProcessAllValue(0.6);

                lblHeader.Text = "ตรวจสอบ ข้อมูลรายชื่อ ธนาคาร";
                if (await SetSyncData("paymentbank")) { lblUpdatedata.TextColor = Color.LimeGreen; }
                else { lblUpdatePlan.TextColor = Color.Red; }
                SetProcessAllValue(0.7);

                lblHeader.Text = "ตรวจสอบ ข้อมูลรายชื่อ หัวใบเสร็จ";
                if (await SetSyncData("billcompany")) { lblUpdatedata.TextColor = Color.LimeGreen; }
                else { lblUpdatePlan.TextColor = Color.Red; }
                SetProcessAllValue(0.7);


                #endregion

                aidwitdata.IsRunning = false;
                aidwitSale.IsRunning = false;
                SetProcessAllValue(1);
                SetProcessValue(1);
                result = true;
            }
            catch (Exception ex) { await DisplayAlert("UpdateData Main Function Error", ex.Message, "OK"); }
            return await Task.FromResult(result);
        }

        //=================== Update Data============================
        async Task<bool> SetSyncData(string tbname)
        {
            try
            {
                DateTime lastsync = App.dbmng.Getsynctime(tbname);
                List<object> result = await App.Ws.GetSyncData(tbname);
                if (result != null)
                {
                    if (tbname.Equals("devicecmd")) { return await UpdateCommandData(result); }
                    else  { return await UpSyncData(tbname, result); }
                }
                return await Task.FromResult(true);
            }
            catch (Exception ex) { await DisplayAlert("UpdateData SetSyncData "+tbname+" Error", ex.Message, "OK"); }
            return await Task.FromResult(false);
        }
        async Task<bool> UpSyncData(string tbname, List<Object> objs)
        {
            try
            {
                try
                {
                    DateTime lastsync = App.dbmng.Getsynctime(tbname);
                    if (objs != null)
                    {
                        double i = 1;
                        foreach (var obj in objs)
                        {
                            try
                            {
                                DateTime modified = await SetUpdatedata(obj, tbname, lastsync);
                                if (lastsync < modified) { lastsync = modified; }
                                Prgvalue.ProgressColor = Color.White;
                            }
                            catch { Prgvalue.ProgressColor = Color.Red; }
                            SetProcessValue(i / objs.Count);
                            i += 1;
                        }
                        App.dbmng.Setsynctime(tbname, lastsync);
                    }
                    return await Task.FromResult(true);
                }
                catch (Exception ex) { await DisplayAlert("UpdateData UpdateSyncData " + tbname + " Error", ex.Message, "OK"); }
                return await Task.FromResult(false);
            }
            catch (Exception ex) { await DisplayAlert("UpdateData UpdateSyncData " + tbname + " Error", ex.Message, "OK"); }
            return await Task.FromResult(false);
        }
        async Task<DateTime> SetUpdatedata(object obj, string tbname, DateTime lastsynctime)
        {
            try
            {
                if (tbname.Equals("visitpage"))
                {
                    var data = JObject.Parse(obj.ToString()).ToObject<Models.VisitPage>();
                    if (data.Transtatus < 0) { App.dbmng.sqlite.Table<Models.VisitPage>().Delete(x => x.Key.Equals(data.Key)); }
                    else { App.dbmng.InsetData(data); }
                    return await Task.FromResult(data.Modified);
                }
                else if (tbname.Equals("menulist"))
                {
                    var data = JObject.Parse(obj.ToString()).ToObject<Models.MenuList>();
                    if (data.Transtatus < 0) { App.dbmng.sqlite.Table<Models.MenuList>().Delete(x => x.Key.Equals(data.Key)); }
                    else { App.dbmng.InsetData(data); }
                    return await Task.FromResult(data.Modified);
                }
                else if (tbname.Equals("custtypelist"))
                {
                    var data = JObject.Parse(obj.ToString()).ToObject<Models.CustomerType>();
                    if (data.Transtatus < 0) { App.dbmng.sqlite.Table<Models.CustomerType>().Delete(x => x.Key.Equals(data.Key)); }
                    else { App.dbmng.InsetData(data); }
                    return await Task.FromResult(data.Modified);
                }
                else if (tbname.Equals("customerpage"))
                {
                    var data = JObject.Parse(obj.ToString()).ToObject<Models.CustomerPage>();
                    if (data.Transtatus < 0) { App.dbmng.sqlite.Table<Models.CustomerPage>().Delete(x => x.Key.Equals(data.Key)); }
                    else { App.dbmng.InsetData(data); }
                    return await Task.FromResult(data.Modified);
                }
                else if (tbname.Equals("custfilter"))
                {
                    var data = JObject.Parse(obj.ToString()).ToObject<Models.CustomerFillter>();
                    if (data.Transtatus < 0) { App.dbmng.sqlite.Table<Models.CustomerFillter>().Delete(x => x.Key.Equals(data.Key)); }
                    else { App.dbmng.InsetData(data); }
                    return await Task.FromResult(data.Modified);
                }
                else if (tbname.Equals("imagetype"))
                {
                    var data = JObject.Parse(obj.ToString()).ToObject<Models.VisitImageType>();
                    if (data.Transtatus < 0) { App.dbmng.sqlite.Table<Models.VisitImageType>().Delete(x => x.Key.Equals(data.Key)); }
                    else { App.dbmng.InsetData(data); }
                    return await Task.FromResult(data.Modified);
                }
                
                else if (tbname.Equals("paymentbank"))
                {
                    var data = JObject.Parse(obj.ToString()).ToObject<Models.PaymentBank>();
                    if (data.Transtatus < 0) { App.dbmng.sqlite.Table<Models.PaymentBank>().Delete(x => x.Key.Equals(data.Key)); }
                    else { App.dbmng.InsetData(data); }
                    return await Task.FromResult(data.Modified);
                }
                else if (tbname.Equals("billcompany"))
                {
                    var data = JObject.Parse(obj.ToString()).ToObject<Models.BillCompany>();
                    if (data.Transtatus < 0) { App.dbmng.sqlite.Table<Models.BillCompany>().Delete(x => x.Key.Equals(data.Key)); }
                    else { App.dbmng.InsetData(data); }
                    return await Task.FromResult(data.Modified);
                }
                else if (tbname.Equals(""))
                {
                    //var data = JObject.Parse(obj.ToString()).ToObject<Models.CustomerFillter>();
                    //if (data.Transtatus < 0) { App.dbmng.sqlite.Table<Models.CustomerFillter>().Delete(x => x.Key.Equals(data.Key)); }
                    //else { App.dbmng.InsetData(data); }
                    //return await Task.FromResult(data.Modified);
                }

            }
            catch (Exception ex) { await DisplayAlert("UpdateData Insertdata " + tbname + " Error", ex.Message, "OK"); }
            return lastsynctime;
        }


        //=================== Update Command============================
        async Task<bool> UpdateCommandData(List<Object> objs)
        {
            try
            {
                try
                {
                    DateTime lastsync = App.dbmng.Getsynctime("devicecmd");
                    if (objs != null)
                    {
                        double i = 1;
                        foreach (var obj in objs)
                        {
                            Models.CommandData dr = (Models.CommandData)obj;
                            if (await SetCommandData(dr) < 0)
                            { Prgvalue.ProgressColor = Color.Red; }
                            else { Prgvalue.ProgressColor = Color.White; }
                            SetProcessValue(i / objs.Count);
                            i += 1;
                            if (lastsync < dr.Modified) { lastsync = dr.Modified; }
                        }
                        App.dbmng.Setsynctime("devicecmd", lastsync);
                    }
                    return await Task.FromResult(true);
                }
                catch (Exception ex) { await DisplayAlert("UpdateData CheckCommand Error", ex.Message, "OK"); }
                return await Task.FromResult(false);


            }
            catch (Exception ex) { await DisplayAlert("UpdateData SetCommandData Error", ex.Message, "OK"); }
            return await Task.FromResult(false);
        }
        async Task<int> SetCommandData(Models.CommandData data)
        {
            try
            {
                //0 = Execute Query 1 = Clear Sync 2 = Clear Table
                if (data.Cmdtype == 0)
                {
                    return await Task.FromResult(App.dbmng.sqlite.Execute(data.Sql));
                }
                else if (data.Cmdtype == 1)
                {
                    return await Task.FromResult(App.dbmng.ClearSynctime(data.Sql));
                }
                else if (data.Cmdtype == 2)
                {
                    return await Task.FromResult(App.dbmng.ClearTable(data.Sql));
                }
                else if (data.Cmdtype == 3)
                {
                    return  await App.Ws.SendataInTable(data.Sql);
                }
            }
            catch (Exception ex) { await DisplayAlert("UpdateData SetCommandData Error", ex.Message, "OK"); }
            return await Task.FromResult(-1);
        }
        void SetProcessAllValue(double value)
        {
            try
            {
                Prgdownload.AnimatedProgress = value;
                LblPrgunit.Text = "%";
                LblPrgvalue.Text = (Prgdownload.AnimatedProgress * 100).ToString("0");
            }
            catch (Exception ex) { DisplayAlert("UpdateData SetProcessAllValue Error", ex.Message, "OK"); }
        }

        void SetProcessValue(double value)
        {
            try
            {
                Prgvalue.Progress = value;
            }
            catch (Exception ex) { DisplayAlert("UpdateData SetProcessValue Error", ex.Message, "OK"); }
        }

        private void btnBack_Clicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopModalAsync();
            }
            catch (Exception ex) { DisplayAlert("UpdateData btnBack_Clicked Error", ex.Message, "OK"); }
        }

        async void GotoOwnerPage(bool success = false)
        {
            MessagingCenter.Send<UpdateDataPage,bool>(this, OwnerPage, success);
            await Navigation.PopModalAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}