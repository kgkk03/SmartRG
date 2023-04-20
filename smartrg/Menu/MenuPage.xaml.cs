using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;

namespace smartrg.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public ListView ListView;
        public MenuPage()
        {
            try
            {
                InitializeComponent();
                if (Device.RuntimePlatform == Device.iOS) { AblMain.Margin = new Thickness(0, 30, 0, 0); }
                ShowEmployee();
                BindingContext = new MasterPageMasterViewModel();
                ListView = MenuItemsListView;
                IniMessage();
            }
            catch (Exception ex) { DisplayAlert("Open MenuPage  Error", ex.Message, "OK"); }
        }
        void IniMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Profile.ProfilePage, Models.Imagedata>(this, "ProfilePage", (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { EditImage(arg); } catch { }
                    });
                });

                MessagingCenter.Subscribe<Tools.EditImagePage, Models.Imagedata>(this, "ProfilePage", (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { EditImage(arg); } catch { }
                    });
                });

            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }
        void EditImage(Models.Imagedata value)
        {
            try
            {
                 ImgUser.Source = value.Image;
            }
            catch { }
          
        }
        public async void ShowEmployee()
        {
            try
            {
                ImgUser.Source = await Helpers.Controls.GetProfileImage();
                Lblfullname.Text = App.UserProfile.Fullname;
                LblTeam.Text = App.UserProfile.Authen + " (" + App.UserProfile.TeamName + ")";

            }
            catch (Exception ex) { await DisplayAlert("MenuPage ShowEmployee Error", ex.Message, "OK"); }
        }
        class MasterPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MenuItemClass> MenuItems { get; set; }
            public MasterPageMasterViewModel()
            {
                SetMenuItems();
            }
            void  SetMenuItems()
            {
                List<MenuItemClass> items = new List<MenuItemClass>();
                foreach(var dr in App.Listmenu)
                {
                    items.Add(new MenuItemClass() {Id=dr.Id,Title=dr.Title,Image=dr.Icon});
                }
                MenuItems = new ObservableCollection<MenuItemClass>(items);
                
            }
           

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion
        }

        private void btnMainmenu_Clicked(object sender, EventArgs e)
        {
            MasterMenuPage mypage = (MasterMenuPage)this.Parent;
            mypage.IsPresented = false;
        }
    }
}