using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ListImagePage : ContentPage
    {
        Models.CustomerData Customer = null;
        private readonly ObservableCollection<Models.ShowVisitImage> Imagelist = new ObservableCollection<Models.ShowVisitImage>();
        //List<Models.ShowVisitImage> Imagelist = new List<Models.ShowVisitImage>();


        bool IsEdit = false;
        int index = 0;
        int Limit = 6;
        bool Success = false;
        public ListImagePage()
        {
            InitializeComponent();
            Setdata(new Models.CustomerData() { Key= "20220621181506105" });
        }


        public async void Setdata(Models.CustomerData customer)
        {
            AidWaitingRun(true);
            try
            {
                if (Customer == null)
                {
                    Customer = customer;
                    LblCustname.Text = Customer.Custname + " (" + Customer.Custgroupname + ")";
                    ImgCustomer.Source = Customer.Icon;
                    LblAddress.Text = Customer.Custaddress;
                    Success = await GetData();
                    ListData.ItemsSource = Imagelist;
                }
                ShowHeader();
            }
            catch { }
            AidWaitingRun(false);
        }
        private async void ListData_RemainingItemsThresholdReached(object sender, EventArgs e)
        {
            if (IsEdit || Success) { return; }
            Success = await GetData();
            ListData.ItemsSource = Imagelist;
        }
        public async Task<bool> GetData()
        {
            try
            {
                IsEdit = true;
                AidWaitingRun(true, "กำลังโหลดรูปภาพใหม่...");
                int start = index * Limit;
                List<Models.VisitImage> result = await App.Ws.GetVisitShowImage(1, Customer.Key, start, Limit);
                if(result!=null || result.Count> 0)
                {
                    foreach (var dr in result)
                    {
                        Models.ShowVisitImage data = new Models.ShowVisitImage()
                        {
                            Item = dr.Item,
                            Typename = dr.Typename,
                            Display = Helpers.Controls.Date2ThaiString(dr.Modified,"d-MMM HH:mm") ,
                            Image = Helpers.ImageConvert.ImageFB64(dr.Thumbnail),
                            Customer =dr.Custname,
                            Location = dr.Location,
                            Employee = dr.Empfullname,
                            Data = dr,
                        };
                        Imagelist.Add(data);
                    }
                    if (result.Count == Limit)
                    {
                        index += 1;
                        AidWaitingRun(false);
                        IsEdit = false;
                        return await Task.FromResult(false);
                    }
                }
            }
            catch { }
            AidWaitingRun(false);
            IsEdit = false;
            return await Task.FromResult(true);
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

            LblWorksumary.Text = "จำนวน " + Imagelist.Count().ToString() + " ภาพ";
            //LblWorkdetails.Text = Activedata.Detail.QuestionHeader;
            //BtnSend.IsVisible =   !Activedata.Detail.QuestionSuccess;

        }
        private void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //if (IsEdit || Activedata.Detail.QuestionSuccess) { return; }

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
            Imagelist.Clear();
            index = 0;
            Success = false;
            Success = await GetData();
            ListData.ItemsSource = Imagelist;
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