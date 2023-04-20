using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Customer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
   
    public partial class ListVisitPage : ContentPage
    {
        List<Models.ReportVisit> VisitList = new List<Models.ReportVisit>();
        Models.CustomerData ActiveCustomer = new Models.CustomerData();
        string OwnerPage = "CustomerPage";
        public ListVisitPage()
        {
            InitializeComponent();
        }
        public void Setdata(string ownerpage, Models.CustomerData data)
        {
            OwnerPage = ownerpage;
            ActiveCustomer = data;
            Showdata();
        }
        void Showdata()
        {
            //VisitList = await Helpers.SampleData.GetListofReportVisit(0, new DateTime(), ActiveCustomer);
            ListData.ItemsSource = VisitList;
        }

        private void ListData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}