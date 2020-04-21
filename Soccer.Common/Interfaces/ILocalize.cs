using System.Globalization;

namespace Soccer.Common.Interfaces
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();

        //CultureInfo muestra info del telèfono, SetLocale, muestra la aplicaciòn en el idioma que le diga CultureInfo
        void SetLocale(CultureInfo ci);
    }
}
