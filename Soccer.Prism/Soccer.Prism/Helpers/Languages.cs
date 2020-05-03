using Soccer.Common.Interfaces;
//using Soccer.Prism.Interfaces;
using Soccer.Prism.Resources;
using System.Globalization;
using Xamarin.Forms;

namespace Soccer.Prism.Helpers
{
    public static class Languages
    {
        static Languages()
        {
            //DependencyService.Get pide la dependencia de ILocalize (iOS/Android) y ejecuta GetCurrentCultureInfo
            CultureInfo ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();

            //al archivo de resource le asigna GetCurrentCultureInfo, es decir que muestre el idioma en inglès o españil
            Resource.Culture = ci;
            Culture = ci.ToString(); //obtiene el lenguaje del sistema            
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }

        public static string Culture { get; set; }

        public static string Accept => Resource.Accept;

        public static string ConnectionError => Resource.ConnectionError;

        public static string Error => Resource.Error;

        public static string Name => Resource.Name;

        public static string MP => Resource.MP;

        public static string MW => Resource.MW;

        public static string MT => Resource.MT;

        public static string ML => Resource.ML;

        public static string PO => Resource.PO;

        public static string GD => Resource.GD;

        public static string Loading => Resource.Loading;

        public static string Tournaments => Resource.Tournaments;

        public static string MyPredictions => Resource.MyPredictions;

        public static string MyPositions => Resource.MyPositions;

        public static string ModifyUser => Resource.ModifyUser;

        public static string Login => Resource.Login;

        public static string PasswordPlaceHolder => Resource.PasswordPlaceHolder;

        public static string PasswordError => Resource.PasswordError;

        public static string Register => Resource.Register;

        public static string Email => Resource.Email;

        public static string EmailPlaceHolder => Resource.EmailPlaceHolder;

        public static string EmailError => Resource.EmailError;

        public static string Password => Resource.Password;

        public static string LoginError => Resource.LoginError;

        public static string Logout => Resource.Logout;
    }
}
