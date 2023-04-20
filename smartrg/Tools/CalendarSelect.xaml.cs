using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarSelect : ContentPage
    {
        string OwnerPage = "CalendarSelect";
        public bool IsBusy = false;
        DateTime SelectedMonth = App.Servertime;
        ResuleCalandra Resultselect = new ResuleCalandra() {Type=2 };
        List<CalandraShow> MonthData = new List<CalandraShow>();
        bool Selectstart = true;
        

        public CalendarSelect()
        {
            InitializeComponent();
            Showcalendar(Resultselect, "");
        }
        public void Showcalendar(ResuleCalandra selected, string ownerpage)
        {
            AidWaitingRun(true);
            OwnerPage = ownerpage;
            Resultselect = selected;
            if (Resultselect.Type<2) { StkConfirmselect.IsVisible = false; }
            SetMountcalendar();
        }
        void SetMountcalendar()
        {
            try
            {
                AidWaitingRun(true);
                MonthData = new List<CalandraShow>();
                DateTime Firstday = new DateTime(SelectedMonth.Year, SelectedMonth.Month, 1);
                DateTime Lastday = Firstday.AddMonths(1).AddDays(-1);
                int wd = (int)Firstday.DayOfWeek;
                int totalday = DateTime.DaysInMonth(Firstday.Year, Firstday.Month);
                int ed = (int)Lastday.DayOfWeek;
                int calendardays = totalday + wd + (ed == 6 ? 0 : 6 - ed);
                for (int i = 0;i< calendardays; i++)
                {
                    var temp = new CalandraShow()
                    {
                        Id = i,
                        Calendardate = Firstday.AddDays(i-wd),
                        Calendartext = Firstday.AddDays(i-wd).Day.ToString(),
                    };
                    if(i<wd || i > (totalday+ wd-1))
                    {
                        temp.Calendarcolor = Color.LightGray;
                        temp.Fontcolor = Color.Gray;
                        temp.CanSelect = false;
                    }
                    MonthData.Add(temp);
                }
                ClnView.ItemsSource = MonthData;
                lblCalendarHeader.Text = Helpers.Controls.GetThaiMonth(Firstday.Month, true);

            }
            catch { }
            AidWaitingRun(false);
        }

        public void GetCalendar()
        {
            var a = 1;
        }

        private void CalendaRefresh_Refreshing(object sender, EventArgs e)
        {
            List<CalandraShow> listcalendar = (List<CalandraShow>)ClnView.ItemsSource ;
            CalandraShow fistcalendar = listcalendar[0];
            DateTime Firstday = fistcalendar.Calendardate;
            List<CalandraShow> newcalendar = new List<CalandraShow>();
            for(int i = 28; i > 0; i--)
            {
                var temp = new CalandraShow()
                {
                    Id = i,
                    Calendardate = Firstday.AddDays(i),
                    Calendartext = Firstday.AddDays(i).Day.ToString(),
                };
                //if (i < wd || i > (totalday + wd - 1))
                //{
                //    temp.Calendarcolor = Color.LightGray;
                //    temp.Fontcolor = Color.Gray;
                //    temp.CanSelect = false;
                //}
                newcalendar.Add(temp);
            }
            foreach(var dr in listcalendar) { newcalendar.Add(dr); }
            ClnView.ItemsSource = newcalendar;
            CalendaRefresh.IsRefreshing = false;

        }
        private void btnnextMonths_Clicked(object sender, EventArgs e)
        {
            AidWaitingRun(true);
            SelectedMonth = SelectedMonth.AddMonths(1);
            SetMountcalendar();
            AidWaitingRun(false);
        }
        private void btnbackMonths_Clicked(object sender, EventArgs e)
        {
            AidWaitingRun(true);
            SelectedMonth = SelectedMonth.AddMonths(-1);
            SetMountcalendar();
            AidWaitingRun(false);
        }

        private void ClnView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalandraShow item = (CalandraShow)e.CurrentSelection.FirstOrDefault();
            ClnView.SelectedItem = null;
            if (item == null || IsBusy) { return; }
            if(!item.CanSelect) { return; }
            IsBusy = true;
            if (Resultselect.Type == 1)
            {
                //Singleselect
                var lastitem = MonthData.Find(x => x.Calendardate == Resultselect.Start);
                if (lastitem != null) { lastitem.Calendarcolor = Color.White; lastitem.Fontcolor = Color.Gray; }
                Resultselect.Start = item.Calendardate;
                Resultselect.End = item.Calendardate;
                item.Calendarcolor = Color.LightPink;
                item.Fontcolor = Color.White;
                LblSelectDate.Text = "วันที่เลือก : " + Helpers.Controls.Date2ThaiString(Resultselect.Start, "d-MMMM-yyyy");
                GotoOwnerpage(Resultselect);
            }
            else
            {
                if(item.Calendardate <= Resultselect.Start) {Resultselect.Start = item.Calendardate; Selectstart = false; }
                else
                {
                    if (item.Calendardate >= Resultselect.End) { Resultselect.End = item.Calendardate; Selectstart = true;}
                    else {
                        if (Selectstart) { Resultselect.Start = item.Calendardate; Selectstart = false; }
                        else { Resultselect.End = item.Calendardate; Selectstart = true; }
                    }
                }
                var listunselect = MonthData.FindAll(x => x.Calendarcolor == Color.LightPink);
                if(listunselect!=null && listunselect.Count > 0)
                {
                    foreach (var lastitem in listunselect) { lastitem.Calendarcolor = Color.White; lastitem.Fontcolor = Color.Black; }
                }
                var selectedperiod = MonthData.FindAll(x => x.Calendardate>= Resultselect.Start && x.Calendardate <= Resultselect.End);
                if (selectedperiod != null && selectedperiod.Count > 0)
                {
                    foreach (var selecttem in selectedperiod) { 
                        selecttem.Calendarcolor = Color.LightPink; selecttem.Fontcolor = Color.White; 
                    }
                }

                LblSelectDate.Text = "วันที่เลือก : " + Helpers.Controls.Date2ThaiString(Resultselect.Start, "d-MMMM-yyyy") +" ถึง " +Helpers.Controls.Date2ThaiString(Resultselect.End, "d-MMMM-yyyy");

                //multiselect
            }
            IsBusy = false;
            ClnView.ItemsSource = null;
            ClnView.ItemsSource = MonthData;
        }



        private void btnSubmit_Clicked(object sender, EventArgs e)
        {
            Resultselect.End = Resultselect.End.AddDays(1).AddSeconds(-1);
            GotoOwnerpage(Resultselect);
        }
        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            GotoOwnerpage(null);
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
        void GotoOwnerpage(ResuleCalandra data)
        {
            try
            {
                MessagingCenter.Send<CalendarSelect, ResuleCalandra>(this, OwnerPage, data);
                Navigation.PopModalAsync();
            }
            catch (Exception ex) { DisplayAlert("SelectPeriod GotoOwnerpage Error", ex.Message, "OK"); }
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

    public class CalandraShow
    {
        public int Id { get; set; } = 0;
        public DateTime Calendardate { get; set; } = new DateTime();
        public string Calendartext { get; set; } = "0";
        public bool CanSelect { get; set; } =true; 
        public Color Fontcolor { get; set; } = Color.Black;
        public Color Calendarcolor { get; set; } = Color.White;
    }

    public class ResuleCalandra
    {
        public DateTime Start { get; set; } = Helpers.Controls.GetToday(App.Servertime);
        public DateTime End { get; set; } =  App.Servertime.AddDays(1);
        public DateTime Min { get; set; } = App.Servertime.AddDays(-30);
        public DateTime Max { get; set; } = App.Servertime.AddDays(30);
        public int Type { get; set; }
        //---0:เลือกวันเดียว 1:เลือกวันเดียว ห้ามเกินช่วงวันที่กำหนด 2:เลือกช่วงเวลา 3:เลือกช่วงเวลา ห้ามเกินช่วงวันที่กำหนด 
    }


}