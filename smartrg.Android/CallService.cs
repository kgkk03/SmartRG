using Android.App;
using System;
using smartrg.Helpers;
using Xamarin.Forms;
using System.IO;
using Android.Graphics;
using Android.Print;
using Android.Content;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using Android.OS;
using Xamarin.Essentials;

[assembly: Dependency(typeof(smartrg.Droid.CallService))]
namespace smartrg.Droid
{
    public class CallService : ICallService
    {
        public static Activity ThisActivity { get; set; }
        public string BntMoveToBack()
        {
            ThisActivity.MoveTaskToBack(true);
            return "";
        }
        public bool StoragerageService()
        {
            try
            {
                //*** ใช้เขียนพื้นที่ภายนอก 
                string packageName = DependencyService.Get<Helpers.IPackageName>().PackageName;
                string mypath ="Android/media/"+ packageName ;
                string paths = Android.OS.Environment.GetExternalStoragePublicDirectory(mypath).AbsolutePath;

                //string paths = Android.OS.Environment.ExternalStorageDirectory.Path;
                //paths = System.IO.Path.Combine(paths, "database");
                //string paths = Android.OS.Environment.GetExternalStoragePublicDirectory("database").AbsolutePath;


                //*** ใช้เขียนพื้นที่ภายใน
                //*For android 11 sdk30
                //string paths = Environment.GetFolderPath(Environment.SpecialFolder.Personal);


                // Database Path 
                string  dbpaths = System.IO.Path.Combine(paths, "database");
                if (!Directory.Exists(dbpaths)) { Directory.CreateDirectory(dbpaths); }
                dbpaths = System.IO.Path.Combine(dbpaths, "smartrg.sqlite");
                App.dbmng.CreateTable(dbpaths);

                // Image Path 
                string imgpaths = System.IO.Path.Combine(paths, "image");
                if (!Directory.Exists(imgpaths)) { Directory.CreateDirectory(imgpaths); }
                App.Imagepath = imgpaths;

                // Image Path 
                string filepaths = System.IO.Path.Combine(paths, "file");
                if (!Directory.Exists(filepaths)) { Directory.CreateDirectory(filepaths); }

                return true;
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("CallService StoragerageService", ex.Message);
                return false;
            }
        }
        public string GetDirectory(string foldername)
        {
            try
            {
                //*** ใช้เขียนพื้นที่ภายนอก 
                //string paths = Android.OS.Environment.ExternalStorageDirectory.Path;
                //*** ใช้เขียนพื้นที่ภายใน
                //string paths = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                string packageName = DependencyService.Get<Helpers.IPackageName>().PackageName;
                string mypath = "Android/media/" + packageName;
                string paths = Android.OS.Environment.GetExternalStoragePublicDirectory(mypath).AbsolutePath;
                paths = System.IO.Path.Combine(paths, foldername);
                if (!Directory.Exists(paths)) { Directory.CreateDirectory(paths); }
                return paths;

            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("SetFilePath", ex.Message);
                return "";
            }
        }
        public string GetPath(string filename = "")
        {
            try
            {
                //*** ใช้เขียนพื้นที่ภายนอก 
                //string paths = Android.OS.Environment.ExternalStorageDirectory.Path;
                string packageName = DependencyService.Get<Helpers.IPackageName>().PackageName;
                string mypath = "Android/media/" + packageName;
                string paths = Android.OS.Environment.GetExternalStoragePublicDirectory(mypath).AbsolutePath;
                string Filepaths = System.IO.Path.Combine(paths, "file");

                //*** ใช้เขียนพื้นที่ภายใน
                //string paths = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                if (filename != "") {
                    paths = System.IO.Path.Combine(paths, Filepaths);
                    paths = System.IO.Path.Combine(paths, filename);
                }
                return paths;
            }
            catch (Exception ex)
            {
                App.dbmng.InsertLog("CallService GetPath", ex.Message);
                return "";
            }
        }
        public async void SaveAssetsFile(string fontname )
        {
            var filPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fontname);
            if (!File.Exists(filPath))
            {
                try
                {
                    using (var fileAssetStream = Android.App.Application.Context.Assets.Open(fontname))
                    using (var fileStream = new FileStream(filPath, FileMode.OpenOrCreate))
                    {
                        var buffer = new byte[1024];

                        int b = buffer.Length;
                        int length;

                        while ((length = await fileAssetStream.ReadAsync(buffer, 0, b)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, length);
                        }
                        fileStream.Flush();
                        fileStream.Close();
                        fileAssetStream.Close();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }



        public void DeleteFile(string source)
        {
            try
            {
                File.Delete(source);
            }
            catch { }
        }
        public void CopyFile(string source, string destination)
        {
            try
            {
                File.Copy(source, destination);
            }
            catch(Exception ex) { 
                var a = ex.Message; 
            }
        }
        public string DrawImageFram(string ImgVisitFile, int iximg, string Customer, string VisitDate, string Empname, string Locations)
        {
            int iNameLimit = 28;

            using (Bitmap bmCard = BitmapFactory.DecodeFile(ImgVisitFile).Copy(Bitmap.Config.Argb8888, true))
            {
                // === Draw image To Canvas
                float fScale = Android.App.Application.Context.Resources.DisplayMetrics.Density;
                Canvas cCanvas = new Canvas(bmCard);
                cCanvas.Save();

                // === หมุนรูป
                //float py = bmCard.Height / 2.0f;
                //float px = bmCard.Width / 2.0f;
                //cCanvas.Rotate(90, px, py);

                //card number
                Paint pCardNumberPaint = new Paint(PaintFlags.AntiAlias);
                pCardNumberPaint.SetStyle(Paint.Style.Fill);
                //pCardNumberPaint.Color = Android.Resource.Color.White;
                pCardNumberPaint.TextSize = 8 * fScale;


                //member name
                Paint pMemberNamePaint = new Paint(PaintFlags.AntiAlias);
                pMemberNamePaint.SetStyle(Paint.Style.Fill);
                var color = new Android.Graphics.Color(96, 109, 244);
                pMemberNamePaint.Color = color;
                pMemberNamePaint.TextSize = 8 * fScale;

                try
                {
                    if (Customer.Length > (iNameLimit - 3))
                    {
                        Customer = Customer.Substring(0, iNameLimit - 3) + "...";
                    }
                }
                catch { }

                Android.Graphics.Rect CustomerBounds = new Android.Graphics.Rect();
                string CustomerText = Customer;
                pCardNumberPaint.GetTextBounds(CustomerText, 0, CustomerText.Length, CustomerBounds);
                int Draw_X = 10;
                int Draw_Y = bmCard.Height - 62;// (int)((bmCard.Height + rMemberNameBounds.Height()) * .2);

                var path = new Android.Graphics.Path();
                path.MoveTo(1, Draw_Y - 15);
                path.LineTo(230, Draw_Y - 15);

                path.LineTo(230, Draw_Y + 63);
                path.LineTo(1, Draw_Y + 63);
                path.LineTo(1, Draw_Y);

                Paint paint = new Paint(PaintFlags.AntiAlias);
                //paint.Color = new Android.Graphics.Color(245, 245, 245);
                paint.Color = Android.Graphics.Color.Transparent;
                paint.SetStyle(Paint.Style.Fill);
                paint.SetXfermode(new Android.Graphics.PorterDuffXfermode(PorterDuff.Mode.SrcOut));

                //cCanvas.DrawPath(path, paint);

                Paint shadowPaint = new Paint(PaintFlags.AntiAlias);
                shadowPaint.AntiAlias = true;
                shadowPaint.Color = new Android.Graphics.Color(255, 255, 255);
                shadowPaint.TextSize = 50.0f; // 60
                shadowPaint.StrokeWidth = 1.0f;
                shadowPaint.SetStyle(Paint.Style.Stroke);
                //shadowPaint.SetShadowLayer(5.0f, 10.0f, 10.0f, new Android.Graphics.Color(251, 252, 252));


                //Draw Customer Name 

                Paint shadowPaintf = new Paint(PaintFlags.AntiAlias);
                shadowPaintf.AntiAlias = true;
                shadowPaintf.Color = new Android.Graphics.Color(28, 40, 51);
                shadowPaintf.TextSize = 50.0f; // 60
                shadowPaintf.StrokeWidth = 2.0f;
                shadowPaintf.SetStyle(Paint.Style.Fill);
                //shadowPaintf.SetShadowLayer(5.0f, 10.0f, 10.0f, new Android.Graphics.Color(251, 252, 252));
                Draw_Y -= (80 * 20) / 25;

                cCanvas.DrawText(CustomerText, Draw_X, Draw_Y, shadowPaintf);
                cCanvas.DrawText(CustomerText, Draw_X, Draw_Y, shadowPaint);

                //Draw VisitDate 
                shadowPaint.TextSize = 40.0f;//50.0f;
                shadowPaintf.TextSize = 40.0f;
                Draw_Y += (45 * 20) / 25;
                if (VisitDate == null) VisitDate = "";
                cCanvas.DrawText(VisitDate, Draw_X, Draw_Y, shadowPaintf);
                cCanvas.DrawText(VisitDate, Draw_X, Draw_Y, shadowPaint);

                //Draw Empname 
                shadowPaint.TextSize = 35.0f;
                shadowPaintf.TextSize = 35.0f;
                Draw_Y += (40 * 20) / 25;
                if (Empname == null) Empname = "";
                cCanvas.DrawText(Empname, Draw_X, Draw_Y, shadowPaintf);
                cCanvas.DrawText(Empname, Draw_X, Draw_Y, shadowPaint);

                //Draw Locations 
                Draw_Y += (40 * 20) / 25;
                if (Locations == null) Locations = "";
                cCanvas.DrawText(Locations, Draw_X, Draw_Y, shadowPaintf);
                cCanvas.DrawText(Locations, Draw_X, Draw_Y, shadowPaint);

                var sCardFileName = "";

                //string paths = GetPath("image");

                string paths = App.Imagepath;

                if (!Directory.Exists(paths)) { Directory.CreateDirectory(paths); }
                sCardFileName = System.IO.Path.Combine(paths, "imgshare" + iximg.ToString() + ".jpg");

                if (File.Exists(sCardFileName)) { File.Delete(sCardFileName); }
                try
                {
                    using (FileStream swStreamWriter = File.Create(sCardFileName))
                    {
                        bmCard.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 80, swStreamWriter);
                        swStreamWriter.Flush();
                        var uri = Android.Net.Uri.Parse(sCardFileName);
                    }

                    bmCard.Recycle();
                    return sCardFileName;
                }
                catch (OutOfMemoryException e)
                {
                    var a = e.Message;
                    return "";
                }

            }
        }
        public string DrawImage(string ImgFile, int iximg, string Title, string SubTitle, string Details1, string Details2)
        {
            //int iNameLimit = 28;
            using (Bitmap bmCard = BitmapFactory.DecodeFile(ImgFile).Copy(Bitmap.Config.Argb8888, true))
            {
                // === Draw image To Canvas
                float fScale = Android.App.Application.Context.Resources.DisplayMetrics.Density;
                float imgHeight = bmCard.Height;
                float imgWidth = bmCard.Width;
                Canvas cCanvas = new Canvas(bmCard);
                cCanvas.Save();

                // === Create rectangle on Canvas
                int TextTitleSize = (10 * int.Parse(imgWidth.ToString())) / 208;
                int TextSubTitleSize = (8 * int.Parse(imgWidth.ToString())) / 208; 
                int TexteSize = (5 * int.Parse(imgWidth.ToString())) / 208;
                int RectHight = TextTitleSize + (TextTitleSize / 2) + (TextSubTitleSize * 3) + (TexteSize * 2);


                // === Create Title Paint
                Paint TitlePaint = new Paint();
                TitlePaint.Color = Android.Graphics.Color.Black;
                TitlePaint.TextSize = TextTitleSize;
                TitlePaint.FakeBoldText = true;

                // === Create SubTitle Paint
                Paint SubTitlePaint = new Paint();
                SubTitlePaint.Color = Android.Graphics.Color.Black;
                SubTitlePaint.TextSize = TextSubTitleSize;
                SubTitlePaint.FakeBoldText = true;

                // === Create SubTitle Paint
                Paint DetailPaint = new Paint();
                DetailPaint.Color = Android.Graphics.Color.Black;
                DetailPaint.TextSize = TexteSize;

                Paint RectPaint = new Paint();
                RectPaint.Color = Android.Graphics.Color.White;
                RectPaint.Alpha = 150;
                cCanvas.DrawRect(0, (imgHeight - RectHight), imgWidth, imgHeight, RectPaint);

                int textLine = (TexteSize / 2);
                cCanvas.DrawText(Details2, 20, (imgHeight - textLine), DetailPaint);

                textLine += ((TextSubTitleSize / 2) + TexteSize);
                cCanvas.DrawText(Details1, 20, (imgHeight - textLine), SubTitlePaint);

                textLine += ((TextSubTitleSize / 2) + TextSubTitleSize);
                cCanvas.DrawText(SubTitle, 20, (imgHeight - textLine), SubTitlePaint);

                textLine += ((TextTitleSize / 2) + TextSubTitleSize);
                cCanvas.DrawText(Title, 20, (imgHeight - textLine), TitlePaint);


                var sCardFileName = "";
                string paths =App.Imagepath;
                if (!Directory.Exists(paths)) { Directory.CreateDirectory(paths); }
                sCardFileName = System.IO.Path.Combine(paths, "imgshare" + iximg.ToString() + ".jpg");
                if (File.Exists(sCardFileName)) { File.Delete(sCardFileName); }
                try
                {
                    using (FileStream swStreamWriter = File.Create(sCardFileName))
                    {
                        bmCard.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 80, swStreamWriter);
                        swStreamWriter.Flush();
                        var uri = Android.Net.Uri.Parse(sCardFileName);
                    }
                    bmCard.Recycle();
                    return sCardFileName;
                }
                catch (OutOfMemoryException ex)
                {
                    App.dbmng.InsertLog("CallService DrawImageFram : ", ex.Message);
                    return "";
                }

            }
        }

        // ========= > Android
        public byte[] ResizeImage(byte[] imageData, float max)
        {
            // Load the bitmap
            float width = max;
            float height = max;
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            if (originalImage.Height > originalImage.Width)
            {
                // รูปแนวตั้ง
                width = (height * ((float)originalImage.Width / (float)originalImage.Height));
            }
            else
            {
                // รูปแนวนอน
                height = (width * ((float)originalImage.Height / (float)originalImage.Width));
            }

            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 90, ms);
                return ms.ToArray();
            }
        }
        public void PrintPDF(string fileName, string SaveName)
        {
            PrintManager printManager = (PrintManager)ThisActivity.GetSystemService(Context.PrintService);
            try
            {
                PrintDocumentAdapter adapter = new PrintPDFAdapter(ThisActivity, fileName, SaveName);
                printManager.Print("Document", adapter, new PrintAttributes.Builder().Build());
            }
            catch  { }
        }

        public void ReplaceFile(string source, string destination)
        {
            try
            {
                string[] img = source.Split("/");
                string bkimg = App.Imagepath + "/" + destination.ToString() + ".png";
                File.Replace(source, source, bkimg);
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
        }

        public Task<bool> Share(string imgpath)
        {
            try
            {
                string pathsimg = imgpath;
                Android.Content.Intent share = new Intent(Android.Content.Intent.ActionSendMultiple);

                share.SetType("image/*");

                List<IParcelable> uris = new List<IParcelable>();
                string[] spimg = pathsimg.Split(',');

                for (int i = 0; i < spimg.Length; i++)
                {
                    Android.Net.Uri url = Android.Net.Uri.Parse(spimg[i]);
                    uris.Add(url);
                }


                share.PutParcelableArrayListExtra(Android.Content.Intent.ExtraStream, uris);

                var chooserIntent = Android.Content.Intent.CreateChooser(share, "Share");
                chooserIntent.SetFlags(ActivityFlags.GrantReadUriPermission);
                chooserIntent.SetFlags(ActivityFlags.ClearTop);
                chooserIntent.SetFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(chooserIntent);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var exc = ex.Message;
                return Task.FromResult(false);
            }
        }

    }
}