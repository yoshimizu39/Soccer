using Android.App;
using Android.OS;

namespace Soccer.Prism.Droid
{
    [Activity(
            Theme = "@style/Theme.Splash", //tema a usar en el splah
            MainLauncher = true, //de aquì arranca el proyecto
            NoHistory = true)] //para que no coloque el bòn atràs
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            System.Threading.Thread.Sleep(1); //tiempo de espera 1000s
            StartActivity(typeof(MainActivity));
        }
    }
}