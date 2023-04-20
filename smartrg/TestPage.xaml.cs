using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestPage : ContentPage
    {
        public TestPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
             LblResult.Text = Helpers.Controls.BathText(double.Parse(TxtInput.Text));
            //DependencyService.Get<Helpers.ICallService>().SaveAssetsFile("logocomp.png");
            //Helpers.PrintPDF.ExamplePdf();
            //var pdf = new Helpers.PrintPDF();
            //pdf.PrintCashSale("CSO001");
        }

        
    }
}