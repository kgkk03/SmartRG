using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Tools
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditImagePage : ContentPage
    {

        string OwnerPage = "ShowImagePage";
        Models.Imagedata ActiveImage = new Models.Imagedata();
        public bool CanUseimageRow = true;
        public EditImagePage()
        {
            InitializeComponent();
        }
        public void Setdata(string ownerpage, string Title, Models.Imagedata value, List<Models.SelectObj<object>> listimagetype = null)
        {
            OwnerPage = ownerpage;
            LblHeader.Text = Title;
            ActiveImage = value;
            //BtnSave.IsVisible = ActiveImage.canedit;
            //แก้ไขให้หน้า ถ่ายภาพรูปโปรไฟล์สามารถเปลี่ยนรูปภาพได้
            BtnSave.IsVisible = true;
            if (value != null) { MyImage.Source = ActiveImage.Image; }
            if(listimagetype==null|| listimagetype.Count == 0) {
                PikImagetype.IsVisible = false;
            }
            else {
                PikImagetype.IsVisible = true;
                PikImagetype.ItemsSource = listimagetype;
                if (ActiveImage.Type != null)
                {
                    int index = listimagetype.FindIndex(x => x.Display.Equals(ActiveImage.Type.Display));
                    if (index != -1) { PikImagetype.SelectedIndex = index; }
                }
                PikImagetype.IsEnabled = ActiveImage.canedit;
            }
            if (value.ImageBase64.Equals("")) { TakePhoto(); }
        }

        async void TakePhoto()
        {
            AidWaitingRun(true,"เตรียมข้อมูลรูปภาพ");
            Models.Imagedata Imgdata = null;
            if (CanUseimageRow)
            {
                if (await DisplayAlert("ระบุรูปแบบ", "เลือกรูปแบบการใช้รูปภาพ", "ถ่ายภาพ", "เลือกจากคลังภาพ"))
                {
                    Imgdata = await Helpers.ImageConvert.TakeCameraAsync(PhotoSize.Medium, ActiveImage.Imagefile);
                }
                else
                {
                    Imgdata = await Helpers.ImageConvert.BrowsPhotoAsync(PhotoSize.Medium, ActiveImage.Imagefile); ;
                }

            }
            else
            {
                Imgdata = await Helpers.ImageConvert.TakeCameraAsync(PhotoSize.Medium, ActiveImage.Imagefile);
            }

            if (Imgdata == null)
            {
                await DisplayAlert("แจ้งเตือน", "กรุณาเลือกรูปที่ต้องการ", "ตกลง");
            }
            else
            {
                ActiveImage.Image = Imgdata.Image;
                ActiveImage.ImageBase64 = Imgdata.ImageBase64;
                ActiveImage.Imagefile = Imgdata.Imagefile;
                MyImage.Source = ActiveImage.Image;
            }
            AidWaitingRun(false);
        }
        private void PikImagetype_SelectedIndexChanged(object sender, EventArgs e)
        {
            Models.SelectObj<object> item = (Models.SelectObj<object>) PikImagetype.SelectedItem;
            if(item!=null) { ActiveImage.Type = item; }
        }
        private void MyImage_Clicked(object sender, EventArgs e)
        {
            TakePhoto();
        }
        private void Btnback_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(false);
        }

        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            GotoOwnerPage(true);
        }
        async void GotoOwnerPage(bool success = false)
        {
            Models.Imagedata result = null;
            if (success) { result = ActiveImage; }
            MessagingCenter.Send<EditImagePage, Models.Imagedata>(this, OwnerPage, result);
            await Navigation.PopModalAsync();
        }

        void AidWaitingRun(bool running, string msg = "")
        {
            try
            {
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