using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Product
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PromotionImgPage : ContentPage
    {
        public PromotionImgPage()
        {
            InitializeComponent();
            Setdata();
        }

       async void Setdata()
        {
            //http://192.168.1.16:4200/#/mobile?login=witooinn.2521@gmil.com&password=E10ADC3949BA59ABBE56E057F20F883E&page=mobilepromotion&team=2
            //http://rg.bttracking.com/#/mobile?login=witooinn.2521@gmil.com&password=E10ADC3949BA59ABBE56E057F20F883E&page=mobilepromotion&team=2
            // var urldata = await GetUrl();
            var urldata = App.PromotionURL + "login="+App.UserProfile.Key + "&password="+ App.password+"&page=mobilepromotion&team="+App.UserProfile.Teamid;
            wbdata.Source = urldata;
        }

        async Task<string> GetUrl()
        {
            string result = "";
            try {
                var client = new HttpClient();
                client.BaseAddress = new Uri("localhost:8080");
                string jsonData = @"{""username"" : ""myusername"", ""password"" : ""mypassword""}";
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("/xxxxxxxx/login", content);
                result = await response.Content.ReadAsStringAsync();
            }
            catch { }
            return await Task.FromResult(result);
        }


        private void Btnback_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}