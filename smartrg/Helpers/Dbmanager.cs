using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace smartrg.Helpers
{
    public class Dbmanager : ContentPage
    {
        public SQLite.SQLiteConnection sqlite;
        public Dbmanager() { }
        public void CreateTable(string Path)
        {
            try
            {
                sqlite = new SQLite.SQLiteConnection(Path);
                //DeleteAlldata();
                sqlite.CreateTable<Models.logerror>();
                sqlite.CreateTable<Models.Sync>();
                sqlite.CreateTable<Models.UserProfile>();
                sqlite.CreateTable<Models.CustomerPage>();
                sqlite.CreateTable<Models.MenuList>();
                sqlite.CreateTable<Models.CustomerType>();
                sqlite.CreateTable<Models.CustomerFillter>();
                sqlite.CreateTable<Models.VisitData>();
                sqlite.CreateTable<Models.VisitDetailData>();
                sqlite.CreateTable<Models.VisitPage>();
                sqlite.CreateTable<Models.CustomerData>();
                sqlite.CreateTable<Models.CustImage>();
                sqlite.CreateTable<Models.VisitImage>();
                sqlite.CreateTable<Models.VisitImageType>();
                sqlite.CreateTable<Models.VisitStockData>();
                sqlite.CreateTable<Models.Questionnaire>();
                sqlite.CreateTable<Models.Answer>();
                sqlite.CreateTable<Models.VisitBillSale>();
                sqlite.CreateTable<Models.SaleorderData>();
                sqlite.CreateTable<Models.SOlineData>();
                sqlite.CreateTable<Models.CashSaleData>();
                sqlite.CreateTable<Models.CashSalelineData>();
                sqlite.CreateTable<Models.PickingLineData>();
                sqlite.CreateTable<Models.PaymentBank>();
                sqlite.CreateTable<Models.BillCompany>();



                ClearLog();
            }
            catch (Exception ex) { DisplayAlert("Dbmanager CreateTable Error", ex.Message, "OK"); }
        }
        public async Task<bool> DeleteAlldata()
        {
            try
            {
                sqlite.Table<Models.logerror>().Delete(x => !x.Msg.Equals(""));
                sqlite.Table<Models.Sync>().Delete(x => !x.Tbname.Equals(""));
                sqlite.Table<Models.UserProfile>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.CustomerPage>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.CustomerType>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.MenuList>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.CustomerFillter>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.VisitBillSale>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.VisitData>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.VisitPage>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.VisitDetailData>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.CustomerData>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.CustImage>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.VisitImage>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.VisitImageType>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.VisitStockData>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.Questionnaire>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.Answer>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.VisitBillSale>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.SaleorderData>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.SOlineData>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.CashSaleData>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.CashSalelineData>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.PickingLineData>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.PaymentBank>().Delete(x => !x.Key.Equals(""));
                sqlite.Table<Models.BillCompany>().Delete(x => !x.Key.Equals(""));
                Settings.ProfileImage = "";
                Settings.QRPayment = "";
                string imagefile = DependencyService.Get<Helpers.ICallService>().GetPath("UserProfile.jpg");
                DependencyService.Get<Helpers.ICallService>().DeleteFile(imagefile);
                string qrfile = DependencyService.Get<Helpers.ICallService>().GetPath("QRPayment.jpg");
                DependencyService.Get<Helpers.ICallService>().DeleteFile(qrfile);
                return await Task.FromResult(true);

            }
            catch (Exception ex) {await DisplayAlert("Dbmanager DeleteAlldata Error", ex.Message, "OK"); }
            return await Task.FromResult(false);
        }
        public int ClearTable(string tbname)
        {
            try
            {
                if (tbname.Equals("logerror")) { return sqlite.Table<Models.logerror>().Delete(x => !x.Msg.Equals("")); }
                else if (tbname.Equals("sync")) { return sqlite.Table<Models.Sync>().Delete(x => !x.Tbname.Equals("")); }
                else if (tbname.Equals("profile")) { return sqlite.Table<Models.UserProfile>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("visitpage")) { return sqlite.Table<Models.VisitPage>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("customerpage")) { return sqlite.Table<Models.CustomerPage>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("customertype")) { return sqlite.Table<Models.CustomerType>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("custfilter")) { return sqlite.Table<Models.CustomerFillter>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("menulist")) { return sqlite.Table<Models.MenuList>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("custtypelist")) { return sqlite.Table<Models.CustomerType>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("custpic")) { return sqlite.Table<Models.CustImage>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("visit")) { return sqlite.Table<Models.VisitData>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("visitdetail")) { return sqlite.Table<Models.VisitDetailData>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("visitimage")) { return sqlite.Table<Models.VisitImage>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("imagetype")) { return sqlite.Table<Models.VisitImageType>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("visitstock")) { return sqlite.Table<Models.VisitStockData>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("questionnaire")) { return sqlite.Table<Models.Questionnaire>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("answer")) { return sqlite.Table<Models.Answer>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("visitbillSale")) { return sqlite.Table<Models.VisitBillSale>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("customer")) { return sqlite.Table<Models.CustomerData>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("so")) { return sqlite.Table<Models.SaleorderData>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("soline")) { return sqlite.Table<Models.SOlineData>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("cashsale")) { return sqlite.Table<Models.CashSaleData>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("cashsaleline")) { return sqlite.Table<Models.CashSalelineData>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("pickingline")) { return sqlite.Table<Models.PickingLineData>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("paymentbank")) { return sqlite.Table<Models.PaymentBank>().Delete(x => !x.Key.Equals("")); }
                else if (tbname.Equals("billcompany")) { return sqlite.Table<Models.BillCompany>().Delete(x => !x.Key.Equals("")); }

            }
            catch (Exception ex) { DisplayAlert("Dbmanager Setsynctime Error", ex.Message, "OK"); }
            return -1;
        }

        public async Task<bool> SenddataInTable(string tbname)
        {
            try {
                List<object> Data = await App.dbmng.GetdataInTable(tbname);
                foreach (var dr in Data)
                {
                    //await App.Ws.SendataInTable(tbname, dr);
                }
            }
            catch (Exception ex) {
                var msg = ex.Message;
            }
            return await Task.FromResult(true);
        }
        public async Task<List<object>> GetdataInTable(string tbname)
        {
            List<object> result = new List<object>();
            if (tbname.Equals("logerror")) { var data = sqlite.Table<Models.logerror>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("sync")) { var data = sqlite.Table<Models.Sync>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("profile")) { var data = sqlite.Table<Models.UserProfile>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("visitpage")) { var data = sqlite.Table<Models.VisitPage>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("customerpage")) { var data = sqlite.Table<Models.CustomerPage>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("customertype")) { var data = sqlite.Table<Models.CustomerType>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("custfilter")) { var data = sqlite.Table<Models.CustomerFillter>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("menulist")) { var data = sqlite.Table<Models.MenuList>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("custtypelist")) { var data = sqlite.Table<Models.CustomerType>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("custpic")) { var data = sqlite.Table<Models.CustImage>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("visit")) { var data = sqlite.Table<Models.VisitData>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("visitdetail")) { var data = sqlite.Table<Models.VisitDetailData>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("visitimage")) { var data = sqlite.Table<Models.VisitImage>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("imagetype")) { var data = sqlite.Table<Models.VisitImageType>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("visitstock")) { var data = sqlite.Table<Models.VisitStockData>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("questionnaire")) { var data = sqlite.Table<Models.Questionnaire>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("answer")) { var data = sqlite.Table<Models.Answer>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("visitbillSale")) { var data = sqlite.Table<Models.VisitBillSale>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("customer")) { var data = sqlite.Table<Models.CustomerData>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("so")) { var data = sqlite.Table<Models.SaleorderData>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("soline")) { var data = sqlite.Table<Models.SOlineData>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("cashsale")) { var data = sqlite.Table<Models.CashSaleData>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("cashsaleline")) { var data = sqlite.Table<Models.CashSalelineData>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("pickingline")) { var data = sqlite.Table<Models.PickingLineData>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("paymentbank")) { var data = sqlite.Table<Models.PaymentBank>().ToList(); foreach (var dr in data) { result.Add(dr); } }
            else if (tbname.Equals("billcompany")) { var data = sqlite.Table<Models.BillCompany>().ToList(); foreach (var dr in data) { result.Add(dr); } }


            return await Task.FromResult(result);
        }



        /////////////// inser table /////////////////
        public int InsetData(object data)
        {
            try
            {
                int resint = sqlite.InsertOrReplace(data);
                return resint;
            }
            catch (Exception ex) { DisplayAlert("Dbmanager InsetData Error", ex.Message, "OK"); return 0; }
        }

        public int InsetDataSO(object data)
        {
            try
            {
                int resint = sqlite.Insert(data);
                return resint;
            }
            catch (Exception ex) { DisplayAlert("Dbmanager InsetData Error", ex.Message, "OK"); return 0; }
        }

        public int ExecuteDB(string sqlcmd)
        {
            try
            {
                var res = sqlite.Execute(sqlcmd);
                return res;
            }
            catch (Exception ex) { DisplayAlert("Dbmanager ExecuteDB Error", ex.Message, "OK"); return 0; }
        }
        public int InsertLog(string Func, string Msg)
        {
            try
            {
                var logdata = new Models.logerror();
                logdata.Logtime = App.Servertime;
                logdata.Msg = Msg;
                logdata.Func = Func;
                return InsetData(logdata);
            }
            catch (Exception ex)
            {
                DisplayAlert("Dbmanager  Error", ex.Message, "OK"); return 0;
            }

        }
        public int ClearLog()
        {
            try
            {
                DateTime clearlogdate = App.Servertime.AddDays(-7);
                sqlite.Table<Models.logerror>().Delete(x => x.Logtime < clearlogdate);
            }
            catch (Exception ex) { DisplayAlert("Dbmanager ClearLog Error", ex.Message, "OK"); }
            return 0;
        }
        public DateTime Getsynctime(string tbname)
        {
            Models.Sync syn = null;
            try
            {
                syn = App.dbmng.sqlite.Table<Models.Sync>().Where(x => x.Tbname.Equals(tbname)).FirstOrDefault();
                if (syn == null)
                {
                    syn = new Models.Sync() { Tbname = tbname, Lastsync = new DateTime() };
                    App.dbmng.InsetData(syn);
                }
                return syn.Lastsync;
            }
            catch (Exception ex) { DisplayAlert("Dbmanager Getsynctime Error", ex.Message, "OK"); }
            return new DateTime();
        }
        public bool Setsynctime(string tbname, DateTime synctime)
        {
            Models.Sync syn = null;
            try
            {
                syn = App.dbmng.sqlite.Table<Models.Sync>().Where(x => x.Tbname.Equals(tbname)).FirstOrDefault();

                if (syn == null) { syn = new Models.Sync() { Tbname = tbname, Lastsync = synctime }; }
                else { syn.Lastsync = synctime; }

                App.dbmng.InsetData(syn);
                return true;
            }
            catch (Exception ex) { DisplayAlert("Dbmanager Setsynctime Error", ex.Message, "OK"); }
            return false;
        }
        public int ClearSynctime(string tbname)
        {
            try
            {
                return App.dbmng.sqlite.Table<Models.Sync>().Delete(x => x.Tbname.Equals(tbname));
            }
            catch (Exception ex) { DisplayAlert("Dbmanager Setsynctime Error", ex.Message, "OK"); }
            return -1;
        }


    }
}
