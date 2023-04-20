using iTextSharp.text.pdf;
using System.Threading.Tasks;


namespace smartrg.Helpers
{
    public interface ICallService
    {
        string BntMoveToBack();
        string GetPath(string filename = "");
        bool StoragerageService();
        string GetDirectory(string foldername);
        void DeleteFile(string source);
        void CopyFile(string source, string destination);
        void ReplaceFile(string source, string destination);
        string DrawImage(string ImgVisitFile, int iximg, string sMemberName, string VisitDate, string Empname, string Locations);
        string DrawImageFram(string ImgVisitFile, int iximg, string sMemberName, string VisitDate, string Empname, string Locations);
        Task<bool> Share(string imgpath);
        byte[] ResizeImage(byte[] imageData, float max);
        void PrintPDF(string fileName, string SaveName);
        void SaveAssetsFile(string fontname);
    }

    public interface IPackageName
    {
        string PackageName { get; }
    }
}
