using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterMenuPage : MasterDetailPage
    {
        

        public MasterMenuPage()
        {
            InitializeComponent();
            IniMessage();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            if (App.Listmenu!=null && App.Listmenu.Count>0) { 
                SetPage(App.Listmenu[0].Id); 
            }
            else { 
                Detail = new NavigationPage(new Menu.NoPage()); 
            }

            // visit unsend
            //CheckLastVisit();


        }
        void IniMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Visit.TodayPage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<Plan.TodayPage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<Customer.CustomerListPage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<Product.ProductListPage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<SaleOrder.TodayPage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<Reports.ReportSaleOrderPage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<Reports.ReportVisitPage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<Profile.ProfilePage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<Profile.EmployeeToday, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<Plan.TodayPage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<Picking.ListPickingPage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

                MessagingCenter.Subscribe<CashSale.ListCashsalePage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });


                MessagingCenter.Subscribe<NoPage, bool>(this, "OpenMenu", (sender, arg) =>
                { Device.BeginInvokeOnMainThread(() => { try { ShowMenuPage(true); } catch { } }); });

            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                var item = e.SelectedItem as MenuItemClass;
                if (item == null) { return; }
                SetPage(item.Id);
            }
            catch (Exception ex) { await DisplayAlert("Master ListView_ItemSelected Error", ex.Message, "OK"); }
        }
        void SetPage(int id)
        {
            try
            {
                if (id == 999) {
                    App.Current.MainPage = new Profile.LoginPage();
                }
                else
                {
                    var page = Helpers.Controls.GetMenuPage(id);
                    Detail = new NavigationPage(page);
                    IsPresented = false;
                    MasterPage.ListView.SelectedItem = null;
                }
            }
            catch (Exception ex) { DisplayAlert("Master ListView_ItemSelected Error", ex.Message, "OK"); }
        }
        void ShowMenuPage(bool ispresented)
        {
            IsPresented = ispresented;
        }

        async void CheckLastVisit()
        {
            Models.VisitShowpageData lastvisit = await Helpers.Controls.CheckLastVisit();
            if (lastvisit != null){
                if (!await DisplayAlert("แจ้งเตือน", "คุณมีการเข้างานค้างไว้ยังไม่ได้ส่ง\nต้องการคีย์ข้อมูลต่อหรือลบข้อมูลทิ้ง","ลบทิ้ง", "คีย์ข้อมูล"))
                {
                    var page = new Visit.VisitTabPage();
                    page.Setdata("MasterMenuPage", lastvisit);
                    foreach (var dr in lastvisit.VisitPage) { page.SetListPage(dr); }
                    page.ShowPage();
                    await Navigation.PushModalAsync(page);
                }
                else
                {
                    await Helpers.Controls.ClearLastvisit(lastvisit.Visitdata.Key);
                }
            }

        }

    }
}