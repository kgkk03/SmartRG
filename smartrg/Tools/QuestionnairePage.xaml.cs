using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class QuestionnairePage : ContentPage
    {

        string Ownerpage = "QuestionnairePage";
        public List<Models.Questionnaire> QuestionList = new List<Models.Questionnaire>();
        public List<Models.Answer> AnswerList = new List<Models.Answer>();
        int Index = 0;
        public bool CanSkip { get; set; }
        public QuestionnairePage()
        {
            InitializeComponent();
            CanSkip = false;
        }



        public void SetQuestion(string ownerpage, List<Models.Questionnaire> quest, List<Models.Answer> ans, int id = 0)
        {
            Index = id;
            Ownerpage = ownerpage;
            QuestionList = quest;
            AnswerList = ans;
            ShowQuestion(id);
        }


        //============== Next Back Enter OK Cancel ======================
        #region "Next Back Enter OK Cancel"
        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            if (await KeepAnswer(Index)) { ShowQuestion(Index + 1); }
        }
        private async void btnBack_Clicked(object sender, EventArgs e)
        {
            if (await KeepAnswer(Index)) { ShowQuestion(Index - 1); }
        }
        private async void txtInput_Completed(object sender, EventArgs e)
        {
            if (await KeepAnswer(Index)) { ShowQuestion(Index + 1); }
        }
        private async void btnOk_Clicked(object sender, EventArgs e)
        {
            await KeepAnswer(Index);
            GotoOwnerPage();
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {

        }

        #endregion

        //============== Keep Answer ======================
        #region "Keep Answer"
        async Task<bool> KeepAnswer(int id)
        {
            var ans = QuestionList[id];
            bool result = false;
            if (ans.Qtype == 0) { result = await await Task.FromResult(KeepSingleAnswer(ans)); }
            else if (ans.Qtype == 1) { result = await await Task.FromResult(KeepMultiAnswer(ans)); }
            else if (ans.Qtype == 2) { result = await await Task.FromResult(KeepTimeAnswer(ans)); }
            else if (ans.Qtype == 3) { result = await await Task.FromResult(KeepPeriodAnswer(ans)); }
            else { result = await await Task.FromResult(KeepTextAnswer(ans)); }
            ans.Icon = (result && !ans.Answer.Equals("")) ? "ic_check" : "ic_uncheck";
            ans.Check = (result && !ans.Answer.Equals("")) ;
            App.dbmng.InsetData(ans);
            return await Task.FromResult(result);
        }
        async Task<bool> KeepSingleAnswer(Models.Questionnaire Quest)
        {
            try
            {
                Models.Answer SelectedItem = (Models.Answer)listAnswer.SelectedItem;
                if (SelectedItem == null)
                {
                    if (await DisplayAlert("แจ้งเตือน", "คุณยังไม่ได้เลือกคำตอบ", "ตกลง", "ข้ามคำตอบ"))
                    { return await Task.FromResult(false); }
                    else
                    { return await Task.FromResult(true); }
                }
                Quest.AnswerID = SelectedItem.Item;
                Quest.Answer = SelectedItem.Description;
                Quest.AnswerAll = SelectedItem.Description;
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        async Task<bool> KeepMultiAnswer(Models.Questionnaire Quest)
        {
            try
            {
                Quest.AnswerID = 0;
                Quest.Answer = "";
                List<Models.SelectObj<Models.Answer>> result = (List<Models.SelectObj<Models.Answer>>)listMultiAnswer.ItemsSource;
                if (result != null & result.Count > 0)
                {
                    foreach (var dr in result)
                    {
                        if (dr.Check) { 
                            Quest.Answer = (Quest.Answer + (Quest.Answer.Equals("") ? "" : ",") + dr.Obj.Item.ToString());
                            Quest.AnswerAll = (Quest.AnswerAll + (Quest.AnswerAll.Equals("") ? "" : ", ") + dr.Obj.Description);
                        }
                    }
                }
                if (Quest.Answer.Equals(""))
                {
                    if (await DisplayAlert("แจ้งเตือน", "คุณยังไม่ได้เลือกคำตอบ", "ตกลง", "ข้ามคำตอบ"))
                    { return await Task.FromResult(false); }
                    else
                    { return await Task.FromResult(true); }
                }
                else { return await Task.FromResult(true); }

            }
            catch { }
            return await Task.FromResult(false);
        }
        async Task<bool> KeepTimeAnswer(Models.Questionnaire Quest)
        {
            try
            {
                Quest.AnswerID = 0;
                Quest.Answer = "";
                Quest.AnswerAll = Quest.Answer;
                Models.Answer starttime = (Models.Answer)PickerTime.SelectedItem;
                if (starttime == null)
                {
                    if (await DisplayAlert("แจ้งเตือน", "คุณยังไม่ได้กรอกข้อมูลเวลาเริ่มต้น", "ตกลง", "ข้ามคำตอบ"))
                    { return await Task.FromResult(false); }
                    else
                    { return await Task.FromResult(true); }
                }
                Quest.AnswerID = starttime.Item;
                Quest.Answer = starttime.Description;
                Quest.AnswerAll = Quest.Answer;
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        async Task<bool> KeepPeriodAnswer(Models.Questionnaire Quest)
        {
            try
            {
                Quest.AnswerID = 0;
                Quest.Answer = "";
                Quest.AnswerAll = Quest.Answer;
                Models.Answer starttime = (Models.Answer)PickerStart.SelectedItem;
                Models.Answer endtime = (Models.Answer)PickerFinish.SelectedItem;
                if (starttime == null)
                {
                    if (await DisplayAlert("แจ้งเตือน", "คุณยังไม่ได้กรอกข้อมูลเวลาเริ่มต้น", "ตกลง", "ข้ามคำตอบ"))
                    { return await Task.FromResult(false); }
                    else
                    { return await Task.FromResult(true); }
                }

                if (endtime == null)
                {
                    if (await DisplayAlert("แจ้งเตือน", "คุณยังไม่ได้กรอกข้อมูลเวลาสิ้นสุด", "ตกลง", "ข้ามคำตอบ"))
                    { return await Task.FromResult(false); }
                    else
                    { return await Task.FromResult(true); }
                }
                Quest.Answer = starttime.Description + "-" + endtime.Description;
                Quest.AnswerAll = Quest.Answer;
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        async Task<bool> KeepTextAnswer(Models.Questionnaire Quest)
        {
            try
            {
                Quest.AnswerID = 0;
                if (!(TxtAnswerInput.Text.Trim().Equals("")))
                {
                    Quest.Answer = TxtAnswerInput.Text;
                    Quest.AnswerAll = Quest.Answer;
                    if (Quest.Qtype == 5 || Quest.Qtype == 10) { Quest.AnswerID = int.Parse(TxtAnswerInput.Text); }
                    return await Task.FromResult(true);
                }
                else
                {
                    if (await DisplayAlert("แจ้งเตือน", "คุณยังไม่ได้กรอกข้อมูลในการตอบคำถามนี้", "ตกลง", "ข้ามคำตอบ"))
                    { return await Task.FromResult(false); }
                    else
                    { return await Task.FromResult(true); }
                }
            }
            catch { }
            return await Task.FromResult(false);
            //if (CanSkip) { return await Task.FromResult(true); }
            //else
            //{
            //    if (await DisplayAlert("แจ้งเตือน", "คุณยังไม่ได้กรอกข้อมูลในการตอบคำถามนี้", "ตกลง", "ข้ามคำตอบ"))
            //    { return await Task.FromResult(false); }
            //    else
            //    { return await Task.FromResult(true); }
            //}
        }


        #endregion

        //============== Show Question & Answer======================
        #region "Show Question & Answer"

        void ShowQuestion(int id)
        {
            if ((id < 0) || (id >= QuestionList.Count)) { GotoOwnerPage(); }
            else
            {
                Index = id;
                var result = QuestionList[id];
                lblQuestion.Text = result.Question;
                ShowAnswer(result);
            }
        }
        void ShowAnswer(Models.Questionnaire Quest)
        {
            lblQtype.Text = GetQuestionType(Quest.Qtype);
            StkAnswerSelect.IsVisible = false;
            StkAnswerMultiSelect.IsVisible = false;
            StkAnswerTime.IsVisible = false;
            StkAnswerPeriod.IsVisible = false;
            StkAnswerText.IsVisible = false;
            if (Quest.Qtype == 0) { SetSingleAnswer(Quest); }
            else if (Quest.Qtype == 1) { SetMultiAnswer(Quest); }
            else if (Quest.Qtype == 2) { SetTimeAnswer(Quest); }
            else if (Quest.Qtype == 3) { SetPeriodAnswer(Quest); }
            else { SetTextAnswer(Quest); }
        }
        string GetQuestionType(int type)
        {
            string[] listqtype = { "Single Choice Question", "Multiple Choice Question", "Select Time Question", "Select Period Question" };
            if (type > 3) { type = 3; }
            return listqtype[type];
            //var qt = App.dbmng.sqlite.Table<Models.StatusText>().Where(x => x.Tbname.Equals("question") && x.ID == type).FirstOrDefault();
            //if (qt == null) { return "เลือกคำตอบ"; }
            //else { return qt.Descript; }

        }
        void SetSingleAnswer(Models.Questionnaire Quest)
        {
            try
            {
                StkAnswerSelect.IsVisible = true;
                var result = AnswerList.Where(x=>x.ID== Quest.QuestionID).OrderBy(x=>x.Item).ToList();
                listAnswer.ItemsSource = result;
                if (Quest.AnswerID != 0)
                {
                    var SelecItem = result.Find(x => x.Item == Quest.AnswerID);
                    if (SelecItem != null) { listAnswer.SelectedItem = SelecItem; }
                }
                else
                {
                    listAnswer.SelectedItem = null;
                }
            }
            catch { }
        }
        void SetMultiAnswer(Models.Questionnaire Quest)
        {
            StkAnswerMultiSelect.IsVisible = true;
            List<Models.SelectObj<Models.Answer>> result = new List<Models.SelectObj<Models.Answer>>();
            var data = AnswerList.Where(x => x.ID == Quest.QuestionID).OrderBy(x => x.Item).ToList();
            if (data != null)
            {
                foreach (var dr in data)
                {
                    Models.SelectObj<Models.Answer> temp = new Models.SelectObj<Models.Answer>() { Check = false, Obj = dr };
                    result.Add(temp);
                }
                if (!Quest.Answer.Equals(""))
                {
                    var listans = Quest.Answer.Split(',');
                    foreach (var dr in listans)
                    {
                        var ans = result.Find(x => x.Obj.Item == int.Parse(dr));
                        if (ans != null) { ans.Check = true; }
                    }
                }

            }
            listMultiAnswer.ItemsSource = result;

        }
        async void SetTimeAnswer(Models.Questionnaire Quest)
        {
            StkAnswerTime.IsVisible = true;
            var listTime = await Helpers.Controls.GetListofAnswerTime();
            PickerTime.ItemsSource = listTime;
            if (Quest.AnswerID != 0)
            {
                var Ans = listTime.Find(x => x.Item == Quest.AnswerID);
                PickerTime.SelectedItem = Ans;
            }

        }
        async void SetPeriodAnswer(Models.Questionnaire Quest)
        {
            StkAnswerPeriod.IsVisible = true;
            var listStart = await Helpers.Controls.GetListofAnswerPeriod();
            var listEnd = await Helpers.Controls.GetListofAnswerPeriod();
            PickerStart.ItemsSource = listStart;
            PickerFinish.ItemsSource = listEnd;
            if (!Quest.Answer.Equals(""))
            {
                var listans = Quest.Answer.Split('-');
                if (listans.Length == 2)
                {
                    var AnsStart = listStart.Find(x => x.Description.Equals(listans[0]));
                    var AnsEnd = listEnd.Find(x => x.Description.Equals(listans[1]));
                    PickerStart.SelectedItem = AnsStart;
                    PickerFinish.SelectedItem = AnsEnd;
                }

            }

        }
        void SetTextAnswer(Models.Questionnaire Quest)
        {
            StkAnswerText.IsVisible = true;
            if (Quest.Qtype == 5 || Quest.Qtype == 10) { TxtAnswerInput.Keyboard = Keyboard.Numeric; }
            else if (Quest.Qtype == 6) { TxtAnswerInput.Keyboard = Keyboard.Telephone; }
            else if (Quest.Qtype == 7) { TxtAnswerInput.Keyboard = Keyboard.Email; }
            else { TxtAnswerInput.Keyboard = Keyboard.Default; }
            TxtAnswerInput.Text = Quest.Answer;
            TxtAnswerInput.Focus();
        }

        #endregion








        void GotoOwnerPage()
        {
            MessagingCenter.Send<QuestionnairePage, List<Models.Questionnaire>>(this, Ownerpage, QuestionList);
            Navigation.PopModalAsync();
        }

    }
}