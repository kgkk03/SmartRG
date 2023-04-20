using Android.Content;
using Android.OS;
using Android.Print;
using Java.Lang;
using Java.IO;
using Android.Util;

namespace smartrg.Droid
{
    public class PrintPDFAdapter : PrintDocumentAdapter
    {
        Context context;
        string Pdfile;
        public string Filename { get; set; } = "Order.Pdf";
        public PrintPDFAdapter(Context context, string filename, string savename)
        {
            this.context = context;
            this.Pdfile = filename;
            Filename = savename;
        }

        public override void OnLayout(PrintAttributes oldAttributes, PrintAttributes newAttributes, CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
        {
            if (cancellationSignal.IsCanceled)
            {
                callback.OnLayoutCancelled();
            }
            else
            {
                PrintDocumentInfo.Builder builder = new PrintDocumentInfo.Builder(Filename);
                builder.SetContentType(PrintContentType.Document)
                    .SetPageCount(PrintDocumentInfo.PageCountUnknown)
                    .Build();
                callback.OnLayoutFinished(builder.Build(), !newAttributes.Equals(oldAttributes));
            }
        }

        public override void OnWrite(PageRange[] pages, ParcelFileDescriptor destination, CancellationSignal cancellationSignal, WriteResultCallback callback)
        {
            InputStream input = null;
            OutputStream output = null;
            try
            {
                File file = new File(Pdfile);
                input = new FileInputStream(file);
                output = new FileOutputStream(destination.FileDescriptor);

                byte[] buff = new byte[8 * 1024];
                int length;
                while ((length = input.Read(buff)) >= 0 && !cancellationSignal.IsCanceled)
                    output.Write(buff, 0, length);
                if (cancellationSignal.IsCanceled)
                {
                    callback.OnWriteCancelled();
                }
                else
                {
                    callback.OnWriteFinished(new PageRange[] { PageRange.AllPages });
                }
            }
            catch (Exception e)
            {
                callback.OnWriteFailed(e.Message);
            }
            finally
            {
                try
                {
                    input.Close();
                    output.Close();
                }
                catch (IOException ex)
                {
                    Log.Error("SM Team", "" + ex.Message);
                }
            }

        }


    }
}