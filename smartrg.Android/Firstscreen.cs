using Android.App;
using Android.OS;

namespace smartrg.Droid
{
    [Activity(Label = "SmartRG", Icon = "@drawable/icon", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class Firstscreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
        }
    }
}