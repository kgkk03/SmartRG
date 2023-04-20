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
    public partial class LastStockPage : ContentPage
    {
        Models.CustomerData ActiveCustomer = new Models.CustomerData();
        string OwnerPage = "CustomerPage";
        public LastStockPage()
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

        }

    }
}