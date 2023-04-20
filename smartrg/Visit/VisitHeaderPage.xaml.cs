using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Visit
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisitHeaderPage : ContentPage
    {
        Models.VisitShowpageData Activedata = null;
        bool IsEdit = false;
        public VisitHeaderPage()
        {
            InitializeComponent();
        }
     

        public async void Setdata(Models.VisitShowpageData data)
        {
            AidWaitingRun(true);
            try
            {
                if (Activedata == null)
                {
                    Activedata = data;
                    LblCustname.Text = Activedata.Customer.Custname + " (" + Activedata.Customer.Custgroupname + ")";
                    ImgCustomer.Source = Activedata.Customer.Icon;
                    LblAddress.Text = Activedata.Customer.Custaddress;
                    LblWorksumary.Text = Activedata.Visitdata.Key;
                    //if (!Activedata.Detail.Newvisit) Activedata.Detail.QuestionSuccess = false;
                }
                //if (Activedata.VisitQuestionnaire == null)
                //{
                //    if (Activedata.Detail.Newvisit) { await GetNewData(); }
                //    else { await GetLogData(); }
                //}

                //if (Activedata.VisitQuestionnaire != null)
                //{
                //    ListData.ItemsSource = Activedata.VisitQuestionnaire;
                //}
                ShowHeader();
            }
            catch { }
            AidWaitingRun(false);
        }
        public async Task<bool> GetNewData()
        {
            try
            {
                //Activedata.VisitQuestionnaire = App.dbmng.sqlite.Table<Models.Questionnaire>()
                //                       .Where(x => x.Visitid.Equals(Activedata.Visitdata.Key))
                //                       .OrderBy(x => x.Piority).ThenBy(x => x.QuestionID).ToList();

                //if (Activedata.VisitQuestionnaire != null && Activedata.VisitQuestionnaire.Count > 0)
                //{
                //    Activedata.VisitAnswer = App.dbmng.sqlite.Table<Models.Answer>().ToList();
                //}
                //else
                //{
                //    List<Models.VisitQuestion> result = await App.Ws.GetVisitQuestionByCust(Activedata.Customer.Custgroupid);
                //    if (result != null)
                //    {
                //        if (result.Count > 0)
                //        {
                //            string qlist = "";
                //            Activedata.VisitQuestionnaire = new List<Models.Questionnaire>();
                //            foreach (var dr in result)
                //            {
                //                Models.Questionnaire drq = new Models.Questionnaire()
                //                {
                //                    Key = Activedata.Visitdata.Key + "-" + dr.ID,
                //                    Visitid = Activedata.Visitdata.Key,
                //                    QuestionID = dr.ID,
                //                    Qtype = dr.Qtype,
                //                    Question = dr.Question,
                //                    Piority = dr.Piority,
                //                };
                //                Activedata.VisitQuestionnaire.Add(drq);
                //                qlist += (qlist.Equals("") ? "" : ",") + dr.ID.ToString();
                //                App.dbmng.InsetData(drq);
                //            }
                //            Activedata.VisitAnswer = await App.Ws.GetVisitAnswer(qlist);
                //            if (Activedata.VisitAnswer != null && Activedata.VisitAnswer.Count > 0)
                //            {
                //                foreach (var ans in Activedata.VisitAnswer) { App.dbmng.InsetData(ans); }
                //            }
                //        }
                //        else
                //        {
                //            Activedata.VisitQuestionnaire = new List<Models.Questionnaire>();
                //            Activedata.VisitAnswer = new List<Models.Answer>();
                //        }
                //    }
                //    else
                //    {
                //        // ไม่มีข้อมูลคำถาม
                //        Activedata.VisitQuestionnaire = null;
                //        Activedata.VisitAnswer = null;
                //    }
                //}
                //return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        public async Task<bool> GetLogData()
        {
            try
            {
                //Activedata.VisitQuestionnaire = await App.Ws.GetVisitQuestionLog(Activedata.Visitdata.Key);
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        void ShowData(List<Models.Questionnaire> listdata)
        {
            try
            {
                //Activedata.VisitQuestionnaire = listdata;
                ListData.ItemsSource = null;
                ListData.ItemsSource = listdata;
                ShowHeader();
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }

        }
        void ShowHeader()
        {
            var data = Activedata.Questionnaire;
            if (data != null && data.Count > 0)
            {
                Activedata.Detail.QuestionCount = data.Count(x => !x.AnswerAll.Equals("")).ToString() + " / " + data.Count.ToString();
                string detail = "";
                foreach (var dr in data)
                {
                    if (!dr.AnswerAll.Equals(""))
                    {
                        detail += (detail.Equals("") ? "" : ",") + dr.AnswerAll;
                    }
                }
                Activedata.Detail.QuestionHeader = detail;
            }
            LblWorksumary.Text = Activedata.Detail.QuestionCount;
            LblWorkdetails.Text = Activedata.Detail.QuestionHeader;
            //BtnSend.IsVisible =   !Activedata.Detail.QuestionSuccess;

        }
        private async void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (IsEdit || Activedata.Detail.QuestionSuccess) { return; }

                //Models.Questionnaire item = (Models.Questionnaire)e.CurrentSelection.FirstOrDefault();
                //int index = Activedata.VisitQuestionnaire.FindIndex(x => x.Key.Equals(item.Key));
                //if (index < 0) { index = 0; }
                //var kb = new Tools.QuestionnairePage();
                //kb.SetQuestion(Mypage, Activedata.VisitQuestionnaire, Activedata.VisitAnswer, index);
                //await Navigation.PushModalAsync(kb);
                //ListData.SelectedItem = null;
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
        }
        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            // Save Data
            if (IsEdit) { return; }
            //IsEdit = true;
            //AidWaitingRun(true, "กำลังส่งข้อมูลการตอบคำถาม...");
            //int success = 0;
            //bool error = false;
            //string msg = "ข้อมูลที่ส่งไปแล้วจะไม่สามารถแก้ไขได้\n" + "คุณต้องการกรอกข้อมูลต่อหรือส่งตามข้อมูลที่มี";
            //if (await DisplayAlert("แจ้งเตือน", msg, "ส่งตามที่มี", "ยังไม่ส่ง"))
            //{
            //    foreach (var dr in Activedata.VisitQuestionnaire)
            //    {
            //        AidWaitingRun(true, "กำลังส่งข้อมูลการเช็คสต็อก...\n" + dr.Question);
            //        msg = await App.Ws.SaveQuestionnaire(dr);
            //        if (msg.Equals("")) { success += 1; }
            //        else { error = true; }
            //        dr.Check = false;
            //        App.dbmng.InsetData(dr);
            //    }

            //    if (error)
            //    {
            //        msg = "ไม่สามารถส่งข้อมูลรายการเช็คสต็อกได้จำนวน " + (Activedata.VisitQuestionnaire.Count - success).ToString()
            //            + "รายการ\nโปรดตรวจสอบอินเตอร์เน็ตแล้วส่งอีกครั้ง";
            //        await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
            //    }
            //    else
            //    {
            //        Activedata.Detail.QuestionSuccess = true;
            //        Activedata.Detail.Questionicon = "ic_check";
            //        ShowHeader();
            //        App.dbmng.InsetData(Activedata.Detail);
            //        Activedata.Visitdata.Transtatus = 3;
            //        Activedata.Visitdata.Modifieddate = App.Servertime;
            //        App.dbmng.InsetData(Activedata.Visitdata);
            //        await App.Ws.SaveVisit(Activedata.Visitdata);

            //    }
            //}
            //IsEdit = false;
            AidWaitingRun(false);
        }
        void AidWaitingRun(bool running, string msg = "")
        {
            try
            {
                if (running && msg.Equals("")) { msg = "กำลังตรวจสอบข้อมูล..."; }
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