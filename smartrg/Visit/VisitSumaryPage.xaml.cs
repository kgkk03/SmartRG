using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartrg.Visit
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisitSumaryPage : ContentPage
    {
        string OwnerPage = "VisitSumaryPage";
        string Mypage = "VisitSumaryPage";
        Models.VisitShowpageData Activedata = null;
        Models.ShowVisitImage ActiveImage = null;
        List<Models.SelectObj<object>> Listimagetype = null;
        bool IsEdit = false;
        public VisitSumaryPage()
        {
            InitializeComponent();
        }
       
        public void Setdata(Models.VisitShowpageData data, string ownerpage)
        {
            AidWaitingRun(true);
            OwnerPage = ownerpage;
            if (Activedata == null)
            {
                Activedata = data;
                BtnSend.IsVisible = Activedata.Detail.Newvisit;
                BtnExit.IsVisible = !Activedata.Detail.Newvisit;
            }
            ShowData();
            foreach (var dr in Activedata.VisitPage)
            {
                if (dr.Pageid == 1101) { SetQuestion(); }
                else if (dr.Pageid == 1102) { SetStock(); }
                else if (dr.Pageid == 1103) { SetStock(); }
                else if (dr.Pageid == 1110) { SetBillSale(); }
            }
            SetImage();
            AidWaitingRun(false);
        }
        void ShowData()
        {
            LblCustname.Text = Activedata.Customer.Custname + " (" + Activedata.Customer.Custgroupname + ")";
            ImgCustomer.Source = Activedata.Customer.Icon;
            LblAddress.Text = Activedata.Customer.Custaddress;
            LblWorkdate.Text = Helpers.Controls.Date2ThaiString(Activedata.Visitdata.Visittime, "dd-MMM-yyyy HH:mm");
            LblEmpdetail.Text = Activedata.Visitdata.Empfullname;
            Lblvisitid.Text = Activedata.Visitdata.Key;
            if (Activedata.Visitdata.Details.Equals("")) { LblDetails.Text = "--ไม่ระบุ--"; }
            else { LblDetails.Text = Activedata.Visitdata.Details; }
        }
        void SetQuestion()
        {
            ImgQuestion.Source = Activedata.Detail.Questionicon;
            LblSumQuestion.Text = Activedata.Detail.QuestionCount;
            LblQuestiondetails.Text = Activedata.Detail.QuestionHeader;
        }
        void SetStock()
        {
            ImgCheckStock.Source = Activedata.Detail.Stockicon;
            LblSumStock.Text = Activedata.Detail.StockCount;
            LblStockdetails.Text = Activedata.Detail.StockHeader;
        }
        void SetBillSale()
        {
            ImgBillSale.Source = Activedata.Detail.BillSaleicon;
            LblSumBillSale.Text = Activedata.Detail.BillSaleCount;
            LblBillSaledetails.Text = Activedata.Detail.BillSaleHeader;
        }

        #region Visit details
        private async void BtnVisitDetails_Clicked(object sender, EventArgs e)
        {
            if (IsEdit) { return; }
            IsEdit = true;
            AidWaitingRun(true);
            var page = new Tools.EditTextPage();
            var details = "";
            if (LblDetails.Text == null || LblDetails.Text == "--ไม่ระบุ--") { details = ""; }
            else { details = LblDetails.Text; }
            page.Setdata(Mypage, "รายละเอียดการทำงาน", Keyboard.Plain, "พิมพ์ข้อมูลการทำงาน", details);
            await Navigation.PushModalAsync(page);
            EditTextPageMessage();
            AidWaitingRun(false);
        }

        void EditTextPageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.EditTextPage, string>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { UpdateDetail(arg); } catch { }
                        MessagingCenter.Unsubscribe<Tools.EditTextPage, string>(this, Mypage);
                    });
                });

            }
            catch (Exception ex) { DisplayAlert("MasterBP MessagingCenter Error", ex.Message, "OK"); }
        }

        void UpdateDetail(string data)
        {
            if (!data.Equals("")) {
                if (data.Length > 250) { data = data.Substring(0, 250); }
                Activedata.Visitdata.Details = data;
                LblDetails.Text = Activedata.Visitdata.Details;
            }
            IsEdit = false;
        }

        #endregion

        #region Visit Image
        async void SetImage()
        {
            if (Activedata.VisitImage == null)
            {
                if (Activedata.Detail.Newvisit) { await GetNewVisitImage(); }
                else { await GetVisitimageLog(); }

            }
            if (Activedata.VisitImage != null)
            {
                CovVisitImage.ItemsSource = await GetImageList(Activedata.VisitImage);
            }
        }
        public async Task<bool> GetNewVisitImage()
        {
            try
            {
                Activedata.VisitImage = App.dbmng.sqlite.Table<Models.VisitImage>()
                                        .Where(x => x.Visitid.Equals(Activedata.Visitdata.Key))
                                        .OrderBy(x => x.Item).ToList();
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        public async Task<bool> GetVisitimageLog()
        {
            try
            {
                Activedata.VisitImage = await App.Ws.GetVisitImage(Activedata.Visitdata.Key);
                return await Task.FromResult(true);
            }
            catch { }
            return await Task.FromResult(false);
        }
        public async Task<List<Models.ShowVisitImage>> GetImageList(List<Models.VisitImage> datas)
        {
            List<Models.ShowVisitImage> result = new List<Models.ShowVisitImage>();
            try { 
                
                if(datas!=null && datas.Count > 0)
                {
                    foreach (var dr in datas)
                    {
                        Models.ShowVisitImage data = new Models.ShowVisitImage()
                        {
                            Item = dr.Item,
                            Typename = dr.Typename,
                            Display = (dr.Item>0 ? (dr.Item.ToString()+": ") :"") + dr.Typename,
                            Image = Helpers.ImageConvert.ImageFB64(dr.ImgBase64),
                            Data = dr,
                        };
                        result.Add(data);
                    }
                }

                if (Activedata.Detail.Newvisit)  { result.Add(GetAddImage()); }
            
            } catch { }
            return await Task.FromResult(result);
        }
        public Models.ShowVisitImage GetAddImage()
        {
            return new Models.ShowVisitImage()
            {
                Item = -1,
                Typename = "เพิ่มรูปใหม่",
                Image = "ic_addimage",
                Data = new Models.VisitImage(),
            };
        }
        async void CovVisitImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Models.ShowVisitImage item = (Models.ShowVisitImage)e.CurrentSelection.FirstOrDefault();
            if (item == null || IsEdit) { return; }
            IsEdit = true;
            ActiveImage = item;
            //if (Activedata.Detail.Newvisit) { await Editimage(); }
            //else { await Editimage(); }
            await Editimage();
            CovVisitImage.SelectedItem = null;
        }
        async void ImageButton_Clicked(object sender, EventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            if (ActiveImage == null || IsEdit) { return; }
            IsEdit = true;
            await Editimage();
        }
        async Task<bool> Editimage()
        {
            if (Listimagetype == null) { Listimagetype = await GetListImageType(); }
            var page = new Tools.EditImagePage();
            page.CanUseimageRow = false;
            //เพิ่ม . ใน string png บรรทัดที่ 221
            Models.Imagedata img = new Models.Imagedata()
            {
                canedit = Activedata.Detail.Newvisit ,
                ImageBase64 = ActiveImage.Data.ImgBase64,
                Image = ActiveImage.Image,
                ShareFile = true,
                Imagefile = ActiveImage.ToString() + ".png",
                Type = await Getimagetype(ActiveImage.Typename),
            };
            page.Setdata(Mypage, Title, img, Listimagetype);
            await Navigation.PushModalAsync(page);
            EditImagePageMessage();
            return await Task.FromResult(true);
        }
        void EditImagePageMessage()
        {
            try
            {
                MessagingCenter.Subscribe<Tools.EditImagePage, Models.Imagedata>(this, Mypage, (sender, arg) =>
                {
                    Device.BeginInvokeOnMainThread(() => {
                        try { UpdateImage(arg); } catch { }
                        MessagingCenter.Unsubscribe<Tools.EditImagePage, Models.Imagedata>(this, Mypage);
                    });
                });
            }
            catch (Exception ex) { DisplayAlert("EditImagePageMessage Error", ex.Message, "OK"); }
        }


        async void UpdateImage(Models.Imagedata img)
        {
            try
            {
                AidWaitingRun(true, "กำลังเตรียมข้อมูลรูปถ่าย");
                if (img != null && ActiveImage != null)
                {
                    List<Models.ShowVisitImage> listimage = (List<Models.ShowVisitImage>)CovVisitImage.ItemsSource;
                    Models.VisitImage Editimage =  ActiveImage.Data;
                    if (img.Thumbnail.Equals("")) { img.Thumbnail = await Helpers.ImageConvert.ResizeImage(img.ImageBase64, 50); }

                    // แก้ไขข้อมูล ประเภทภาพถ่าย
                    if (img.Type != null && img.Type.Obj != null)
                    {
                        Models.VisitImageType imgtype = (Models.VisitImageType)img.Type.Obj;
                        Editimage.Typeid = imgtype.Key;
                        Editimage.Typename = imgtype.Typename;
                    }
                    else
                    {
                        Editimage.Typeid = 0;
                        Editimage.Typename = "อื่น ๆ";
                    }

                    // แก้ไขข้อมูล รูปภาพ และเพิ่มจำนวนครั้งที่แก้ไข
                    Editimage.ImgBase64 = img.ImageBase64;
                    Editimage.Thumbnail = img.Thumbnail;
                    Editimage.Transtatus += 1;
                    Editimage.Modified = App.Servertime;
                    ActiveImage.Typename = Editimage.Typename;
                    ActiveImage.Image = Helpers.ImageConvert.ImageFB64(img.ImageBase64);

                    if (ActiveImage.Item == -1)
                    {
                        ActiveImage.Item = Activedata.VisitImage.Count;
                        Editimage.Item = ActiveImage.Item;
                        Editimage.Visitid = Activedata.Visitdata.Key;
                        Editimage.Key = Activedata.Visitdata.Key + "-" + ActiveImage.Item;
                        // เพิ่มรูปใหม่ (Ediimage) ใน VisitImage
                        Activedata.VisitImage.Add(Editimage);
                        
                        // เพิ่มปุ่มสร้างรูปใหม่
                        listimage.Add(GetAddImage());
                    }
                    else
                    {
                        Models.VisitImage data = Activedata.VisitImage.Find(x => x.Item == ActiveImage.Item);
                        if (data != null) {data = Editimage;}
                    }

                    ActiveImage.Display = Editimage.Typename;
                    ActiveImage.Display = (ActiveImage.Item > 0 ? (ActiveImage.Item.ToString() + ": ") : "") + ActiveImage.Typename;


                    //if (await DisplayAlert("ยืนยัน", "คุณต้องการแชร์รูปภาพนี้หรือไม่", "แชร์ไฟล์", "ไม่ต้องการ"))
                    //{
                    //    // แชร์ไฟล์

                    //}
                    AidWaitingRun(true, "กำลังส่งข้อมูลรูปถ่าย");
                    string msg = await App.Ws.SaveVisitImage(Editimage);
                    if (msg.Equals("")) { App.dbmng.InsetData(Editimage); }
                    else { await DisplayAlert("แจ้งเตือน", "ไม่สามารถส่งข้อมูลภาพได้\n"+ msg, "ตกลง"); }
                    CovVisitImage.ItemsSource = null;
                    CovVisitImage.ItemsSource = listimage;
                }
            }
            catch { }
            AidWaitingRun(false);
            IsEdit = false;
        }
        async Task<List<Models.SelectObj<object>>> GetListImageType()
        {
            List<Models.SelectObj<object>> result = new List<Models.SelectObj<object>>();
            try
            {
                List<Models.VisitImageType> datas = App.dbmng.sqlite.Table<Models.VisitImageType>().OrderBy(x => x.Typename).ToList();
                foreach (var dr in datas)
                {
                    var temp = new Models.SelectObj<object>()
                    {
                        Display = dr.Typename,
                        Check = false,
                        Obj = dr,
                    };
                    result.Add(temp);
                }
            }
            catch { }
            return await Task.FromResult(result);
        }
        async Task<Models.SelectObj<object>> Getimagetype(string imagetype)
        {
            Models.SelectObj<object> result = null;
            try
            {
                result = new Models.SelectObj<object>()
                {
                    Check = true,
                    Display = "อื่น ๆ",
                    Obj = new Models.VisitImageType() { Key = -1, Typename = "อื่น ๆ" },
                };
                if (Listimagetype != null && Listimagetype.Count > 0)
                {
                    var data = Listimagetype.Find(x => x.Display.Equals(imagetype));
                    if (data != null) { result = data; }
                }
            }
            catch { }
            return await Task.FromResult(result);
        }

        #endregion


        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            if (IsBusy == true) { return; }
            IsBusy = true;
            // Save Data
            string msg = "";

            // Check Send Complete
            List<Models.VisitPage> listReqsend = Activedata.VisitPage.Where(x => x.Reqsend).ToList();
            if(listReqsend!=null|| listReqsend.Count > 0)
            {
                foreach (var dr in listReqsend)
                {
                    if (dr.Pageid == 1101 && !Activedata.Detail.QuestionSuccess) { 
                        if(Activedata.Questionnaire!=null&& Activedata.Questionnaire.Count > 0) 
                        { msg = "กรุณาบันทึกและส่งข้อมูลแบบสอบถามก่อนจบงาน"; }
                    }
                    else if (dr.Pageid == 1102 && !Activedata.Detail.VisitStockSuccess) { msg = "กรุณาบันทึกและส่งข้อมูลการเช็คสต็อกก่อนจบงาน"; }
                    else if (dr.Pageid == 1103 && !Activedata.Detail.VisitStockSuccess) { msg = "กรุณาบันทึกและส่งข้อมูลการเช็คสต็อกก่อนจบงาน"; }
                    else if (dr.Pageid == 1110 && !Activedata.Detail.BillSaleSuccess) { msg = "กรุณาบันทึกและส่งข้อมูลตรวจสอบยอดขายก่อนจบงาน"; }

                }
                if (!msg.Equals(""))
                {
                    await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                    IsBusy = false;
                    return;
                }
            }

            msg = "คุณต้องการส่งข้อมูลเสร็จสิ้นการทำงาน";
            if (await DisplayAlert("แจ้งเตือน", msg, "ส่งจบงาน", "ยังไม่ส่ง"))
            {
                AidWaitingRun(true, "กำลังส่งข้อมูลเลิกงาน...");
                Activedata.Visitdata.Transtatus = 10;
                Activedata.Visitdata.Visitfinish = App.Servertime;
                string datails = Helpers.Controls.GetVisitDetails(Activedata);
                Activedata.Visitdata.Details += datails;
                if (Activedata.Visitdata.Planstatus == 4) { Activedata.Visitdata.Planstatus = 8; }
                msg = await App.Ws.SaveVisit(Activedata.Visitdata);
                if (msg.Equals(""))
                {
                    MessagingCenter.Send<VisitSumaryPage, bool>(this, OwnerPage, true);
                }
                else
                {
                    Activedata.Visitdata.Transtatus = 1;
                    Activedata.Visitdata.Visitfinish = Activedata.Visitdata.Visitstart;
                    msg = "ไม่สามารถส่งข้อมูลเริ่มงานได้โปรดตรวจสอบอินเตอร์เน็ตของท่าน \n" + msg;
                    await DisplayAlert("แจ้งเตือน", msg, "ตกลง");
                }
                AidWaitingRun(false);
            }
            IsBusy = false;

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
        private void BtnExit_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send<VisitSumaryPage, bool>(this, OwnerPage, false);
        }

        private async void imgShare_Clicked(object sender, EventArgs e)
        {
            try
            {
                if(IsBusy == true) { return; }
                IsBusy = true;
                AidWaitingRun(true, "กำลังแชร์รูปภาพ...");
                if(await DisplayAlert("แจ้งเตือน", "คุณต้องการแชร์ภาพถ่ายใช่หรือไม่", "ใช่", "ไม่"))
                {
                    if (ActiveImage != null)
                    {
                        await Helpers.Controls.ShareVisitImageFromDb(false, Activedata, ActiveImage.Data.Key);
                    }
                    else
                    {//shared ย้อยหลัง
                        await Helpers.Controls.ShareVisitImageFromDb(false, Activedata, Activedata.Visitdata.Key);
                    }
                }
            }
            catch ( Exception ex )
            {
                var exc = ex.Message;
            }
            AidWaitingRun(false);
            IsEdit = false;
            IsBusy = false;
        }
    }

   
}