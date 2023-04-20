using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Visit
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionPage : ContentPage
    {
        string Mypage = "QuestionPage";
        Models.VisitShowpageData Activedata = null;
        bool IsEdit = false;
        string Questionlist = "";
        public QuestionPage()
        {
            InitializeComponent();
        }
        public async void Setdata(Models.VisitShowpageData data)
        {
            AidWaitingRun(true);
            try {
                if (Activedata == null)
                {
                    Activedata = data;
                    LblCustname.Text = Activedata.Customer.Custname + " (" + Activedata.Customer.Custgroupname + ")";
                    ImgCustomer.Source = Activedata.Customer.Icon;
                    LblAddress.Text = Activedata.Customer.Custaddress;
                    LblWorksumary.Text = Activedata.Visitdata.Key;
                    if (!Activedata.Detail.Newvisit) { Activedata.Detail.QuestionSuccess = false; }
                }
                if (Activedata.Questionnaire == null)
                {
                    if (Activedata.Detail.Newvisit) {
                        Activedata.Questionnaire = await GetNewData(); }
                    else 
                    {
                        Activedata.Questionnaire = await GetLogData();
                        Activedata.Detail.QuestionSuccess = true;
                    }
                }

                if (Activedata.Questionnaire != null && Activedata.Questionnaire.Count > 0)
                {
                    ListData.ItemsSource = Activedata.Questionnaire;
                }
               
                ShowHeader();
            }
            catch { }
            AidWaitingRun(false);
        }
        public async Task<List<Models.Questionnaire>> GetNewData()
        {
            try
            {
                List<Models.Questionnaire> result = App.dbmng.sqlite.Table<Models.Questionnaire>()
                                       .Where(x => x.Visitid.Equals(Activedata.Visitdata.Key))
                                       .OrderBy(x => x.Piority).ThenBy(x => x.QuestionID).ToList();

                if (result != null && result.Count> 0)
                {
                    Activedata.Answer = App.dbmng.sqlite.Table<Models.Answer>().ToList();
                }
                else
                {
                    List<Models.VisitQuestion> question = await App.Ws.GetVisitQuestionByCust(Activedata.Customer.Custgroupid);
                    result = await GetNewQuestionnaire(question);
                }
                return await Task.FromResult(result);
            }
            catch { }
            return null;
        }
        public async Task<List<Models.Questionnaire>> GetLogData()
        {
            try
            {
                List < Models.Questionnaire > result = await App.Ws.GetVisitQuestionLog(Activedata.Customer.Custgroupid, Activedata.Visitdata.Key);
                return await Task.FromResult(result);
            }
            catch { }
            return null;
        }
        public async Task<List<Models.Questionnaire>> GetNewQuestionnaire(List<Models.VisitQuestion> data )
        {
            try {
                List<Models.Questionnaire> result = new List<Models.Questionnaire>();
                string Questionlist = "";

                if (data != null && data.Count > 0)
                {
                    foreach (var dr in data)
                    {
                        Models.Questionnaire drq = new Models.Questionnaire()
                        {
                            Key = Activedata.Visitdata.Key + "-" + dr.ID,
                            Visitid = Activedata.Visitdata.Key,
                            QuestionID = dr.ID,
                            Qtype = dr.Qtype,
                            Question = dr.Question,
                            Piority = dr.Piority,
                        };
                        result.Add(drq);
                        Questionlist += (Questionlist.Equals("") ? "" : ",") + dr.ID.ToString();
                        App.dbmng.InsetData(drq);
                    }
                    Activedata.Answer = await App.Ws.GetVisitAnswer(Questionlist);
                    if (Activedata.Answer != null && Activedata.Answer.Count > 0)
                    {
                        foreach (var ans in Activedata.Answer) { App.dbmng.InsetData(ans); }
                    }
                }
                else
                {
                   result = null;
                   Activedata.Answer = null;
                }
               
                return await Task.FromResult(result);
            } catch { }
            return  null;
        }
        void ShowData(List<Models.Questionnaire> listdata)
        {
            try
            {
                Activedata.Questionnaire = listdata;
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
            if(data!=null && data.Count > 0)
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
            BtnSend.IsVisible = false;
            if (Activedata.Questionnaire != null && Activedata.Questionnaire.Count > 0)
            {
                if (Activedata.Detail.Newvisit && !Activedata.Detail.QuestionSuccess)
                {
                    BtnSend.IsVisible = true;
                }
            }
        }
        private async void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (IsEdit || Activedata.Detail.QuestionSuccess) { return; }

                Models.Questionnaire item = (Models.Questionnaire)e.CurrentSelection.FirstOrDefault();
                int index = Activedata.Questionnaire.FindIndex(x => x.Key.Equals(item.Key));
                if (index < 0) { index = 0; }
                var kb = new Tools.QuestionnairePage();
                kb.SetQuestion(Mypage, Activedata.Questionnaire, Activedata.Answer, index);
                await Navigation.PushModalAsync(kb);
                QuestionnairePageMessage();
                ListData.SelectedItem = null;
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
        }
        void QuestionnairePageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.QuestionnairePage, List<Models.Questionnaire>>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { ShowData(arg); } catch { }
                        MessagingCenter.Unsubscribe<Tools.QuestionnairePage, List<Models.Questionnaire>>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Save Data
                if (IsEdit) { return; }
                IsEdit = true;
                AidWaitingRun(true, "กำลังส่งข้อมูลการตอบคำถาม...");
                int success = 0;
                bool error = false;
                string msg = "ข้อมูลที่ส่งไปแล้วจะไม่สามารถแก้ไขได้\n" + "คุณต้องการกรอกข้อมูลต่อหรือส่งตามข้อมูลที่มี";
                if (await DisplayAlert("แจ้งเตือน", msg, "ส่งตามที่มี", "ยังไม่ส่ง"))
                {
                    foreach (var dr in Activedata.Questionnaire)
                    {
                        AidWaitingRun(true, "กำลังส่งข้อมูลการเช็คสต็อก...\n" + dr.Question);
                        msg = await App.Ws.SaveQuestionnaire(dr);
                        if (msg.Equals("")) { success += 1; }
                        else { error = true; }
                        dr.Check = false;
                        App.dbmng.InsetData(dr);
                    }

                    if (error)
                    {
                        msg = "ไม่สามารถส่งข้อมูลรายการเช็คสต็อกได้จำนวน " + (Activedata.Questionnaire.Count - success).ToString()
                            + "รายการ\nโปรดตรวจสอบอินเตอร์เน็ตแล้วส่งอีกครั้ง";
                        await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                    }
                    else
                    {
                        Activedata.Detail.QuestionSuccess = true;
                        Activedata.Detail.Questionicon = "ic_check";
                        ShowHeader();
                        App.dbmng.InsetData(Activedata.Detail);
                        Activedata.Visitdata.Transtatus = 3;
                        Activedata.Visitdata.Modifieddate = App.Servertime;
                        App.dbmng.InsetData(Activedata.Visitdata);
                        await App.Ws.SaveVisit(Activedata.Visitdata);

                    }
                }
                IsEdit = false;
            }
            catch { }
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