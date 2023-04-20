using Android.App;
using smartrg.Droid;
using smartrg.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(PackageNameDroid))]
namespace smartrg.Droid
{

    public class PackageNameDroid : IPackageName
    {
        public PackageNameDroid()
        {
        }

        public string PackageName
        {
            get { return Application.Context.PackageName; }
        }

    }
}