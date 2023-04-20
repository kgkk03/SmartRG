using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace smartrg.Helpers
{
    class ImageConvert
    {
        public ImageConvert() { }

        public static string ImageFile2Base64(string Path)
        {

            return "";
        }
        public static string Image2Base64(byte[] imageData)
        {
            var base64String = "data:image/png;base64," + Convert.ToBase64String(imageData);
            return base64String;
        }
        public static ImageSource ImageFB64(string ImageBase64)
        {

            try
            {
                if (ImageBase64.Length > 4)
                {
                    var str = ImageBase64.Substring(0, 4).ToLower();
                    if (str.Equals("data"))
                    {
                        var ex = ImageBase64.Split(',');
                        if (ex.Length > 1) { ImageBase64 = ex[1]; }
                    }
                    
                    else if (str.Equals("http"))
                    {
                        //// ============== ไม่รู้ทำไงเหมือนกัน เก็บไว้ก่อน
                        var imageSource = new UriImageSource { Uri = new Uri(ImageBase64) };
                        imageSource.CachingEnabled = false;
                        imageSource.CacheValidity = TimeSpan.FromHours(1);
                        return imageSource;
                    }
                    var x = ImageBase64.Split(';');
                    if (x.Length > 0) { x[0] += ";base64,"; }
                    else { ImageBase64 += ";base64,"; }
                    ImageSource image = ImageSource.FromStream(
                    () => new MemoryStream(Convert.FromBase64String(ImageBase64.Replace(x[0], ""))));
                    return image;
                }
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            return null;
        }
        public static MemoryStream ByteFB64(string ImageBase64)
        {

            try
            {
                if (ImageBase64.Length > 4)
                {
                    var str = ImageBase64.Substring(0, 4).ToLower();
                    if (str.Equals("data"))
                    {
                        var ex = ImageBase64.Split(',');
                        if (ex.Length > 1) { ImageBase64 = ex[1]; }
                    }

                    else if (str.Equals("http"))
                    {
                        //// ============== ไม่รู้ทำไงเหมือนกัน เก็บไว้ก่อน
                        var imageSource = new UriImageSource { Uri = new Uri(ImageBase64) };
                        imageSource.CachingEnabled = false;
                        imageSource.CacheValidity = TimeSpan.FromHours(1);
                        return new MemoryStream(Convert.FromBase64String(ImageBase64));
                    }
                    var x = ImageBase64.Split(';');
                    if (x.Length > 0) { x[0] += ";base64,"; }
                    else { ImageBase64 += ";base64,"; }
                    ImageSource image = ImageSource.FromStream( () => new MemoryStream(Convert.FromBase64String(ImageBase64.Replace(x[0], "")))
                    );
                    return new MemoryStream(Convert.FromBase64String(ImageBase64.Replace(x[0], "")));
                }

            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            return null;
        }
        public static async Task<string> TakeCameraAsync(PhotoSize ImageSize)
        {
            try
            {
                //No camera avaialble
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                { return null; }
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    CustomPhotoSize = 7,
                    Directory = "SM",
                    Name = "temp.jpg",
                    DefaultCamera = CameraDevice.Front,
                    CompressionQuality = 80
                });

                //Canot open file Or File not avaialble
                if (file == null) { return ""; }
                var st = file.GetStream();
                var imageData = new byte[st.Length];
                st.Read(imageData, 0, (int)st.Length);
                DependencyService.Get<Helpers.ICallService>().DeleteFile(file.Path);
                return Image2Base64(imageData);

            }
            catch { return ""; }
        }
        public static async Task<Models.Imagedata> TakeCameraAsync(PhotoSize ImageSize, string destinationfile )
        {
            try
            {
                //No camera avaialble
                Models.Imagedata result = new Models.Imagedata();
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                { return null; }
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    CustomPhotoSize = 7,
                    Directory = "SM",
                    Name =  "temp.jpg",
                    DefaultCamera = CameraDevice.Front,
                    CompressionQuality = 80
                });

                //Canot open file Or File not avaialble
                if (file == null) { return null; }
                var st = file.GetStream();
                var imageData = new byte[st.Length];
                st.Read(imageData, 0, (int)st.Length);
                if (destinationfile != "")
                {
                    //var df = DependencyService.Get<Helpers.ICallService>().GetPath(destinationfile);
                    var df = Path.Combine(App.Imagepath, destinationfile);
                    DependencyService.Get<Helpers.ICallService>().DeleteFile(df);
                    DependencyService.Get<Helpers.ICallService>().CopyFile(file.Path, df);
                    result.Imagefile = df;
                }
                DependencyService.Get<Helpers.ICallService>().DeleteFile(file.Path);
                //CreateActivityPage.filespath = file.Path;
                result.ImageBase64 = Image2Base64(imageData);
                result.Image = ImageFB64(result.ImageBase64);
                return result;
            }
            catch (Exception ex) {
                var a = ex.Message;
                return null; 
            }
        }
        public static async Task<string> BrowsPhotoAsync(PhotoSize ImageSize)
        {
            //No camera avaialble
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            { return null; }

            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            { 
                PhotoSize = PhotoSize.Custom,
                CustomPhotoSize = 10,
            });
            if (file == null) { return ""; }

            byte[] tmpimg = ReadFully(file.GetStream());
            return Image2Base64(tmpimg);
        }
        public static async Task<Models.Imagedata> BrowsPhotoAsync(PhotoSize ImageSize, string destinationfile)
        {
            //No camera avaialble
            Models.Imagedata result = new Models.Imagedata();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            { return null; }

            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            { 
                PhotoSize = PhotoSize.Custom,
                CustomPhotoSize = 10,
            });
            if (file == null) { return null; }

            byte[] tmpimg = ReadFully(file.GetStream());
            if (destinationfile != "")
            {
                //var df = DependencyService.Get<Helpers.ICallService>().GetPath(destinationfile);
                var df = Path.Combine(App.Imagepath, destinationfile);
                DependencyService.Get<Helpers.ICallService>().DeleteFile(df);
                DependencyService.Get<Helpers.ICallService>().CopyFile(file.Path, df);
                result.Imagefile = df;
            }
            result.ImageBase64 = Image2Base64(tmpimg);
            result.Image = ImageFB64(result.ImageBase64);
            return result;
        }
        static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public static  bool Image2File(string ImageBase64, string destinationfile)
        {
            try
            {
                string filename = Path.Combine(App.Imagepath, destinationfile);
                filename = DependencyService.Get<Helpers.ICallService>().GetPath(filename);
                MemoryStream img = ByteFB64(ImageBase64);
                byte[] byteimg = img.ToArray();
                File.WriteAllBytes(filename, byteimg);
                return  true;
            }
            catch (Exception ex)
            {
                //var a = ex.Message;
                return false;
            }
        }
        public static async Task<ImageSource> GetImagessource(string stringimage)
        {
            if (stringimage == null || stringimage.Equals("")) { return await Task.FromResult("no_photo"); }
            else if (stringimage.Equals("no_image")) { return await Task.FromResult("no_image"); }
            else { return await Task.FromResult(ImageFB64(stringimage)); }
        }

        public static async Task<string> ResizeImage(string ImageBase64, float max)
        {
            string result = "";
            try {
                //แปลง ImageBase64 เป็น byte[]
                MemoryStream img = ByteFB64(ImageBase64);
                byte[] imageData = img.ToArray();
                byte[] thumbnailimg = DependencyService.Get<Helpers.ICallService>().ResizeImage(imageData, max);
                result = Image2Base64(thumbnailimg);
            }
            catch { }
            return  await Task.FromResult( result);
        }

        //=========== Scanner QR Code ============
        public static async Task<string> QRCodeScan()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();
            return result.ToString();
        }
        public static string GetVisitImageType(int index)
        {
            if (index == 0) { return "เข้าพบลูกค้า"; }
            else if (index == 1) { return "สต็อกสินค้า"; }
            else if (index == 2) { return "ก่อนจัดเรียงสินค้า"; }
            else if (index == 3) { return "หลังจัดเรียงสินค้า"; }
            else if (index == 4) { return "สัญญา"; }
            else if (index == 5) { return "กิจกรรมคู่แข่ง"; }
            else if (index == 6) { return "โปรโมชั่น"; }
            else if (index == 7) { return "สินค้าชงชิม"; }
            else if (index == 8) { return "สินค้าขอเบิก"; }
            else if (index == 9) { return "สินค้าส่งคืน"; }
            else if (index == 10) { return "สินค้าคู่แข่ง"; }
            else if (index == 11) { return "เริ่มขายดี"; }
            else if (index == 12) { return "บรรยากาศร้าน"; }
            else if (index == 13) { return "BP แบรนด์อื่น"; }
            else if (index == 14) { return "ยอดขายสินค้า"; }
            else if (index == 15) { return "สั่งขายสินค้า"; }
            else if (index == 16) { return "ขายสินค้าเงินสด"; }


            else if (index == 99) { return "เลิกงาน"; }
            //	0 = Login 1 = stock 2= Facing#1 3= Facing#2
            //4= Contract 5=Compet 6=Promo 7=Tester 99 = Logout


            return "";
        }


    }
   

}