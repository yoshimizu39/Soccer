using Prism.Navigation;
using Xamarin.Forms;

namespace Soccer.Prism.Views
{
    public partial class TournamentTabbedPage : TabbedPage, INavigatedAware
    {
        public TournamentTabbedPage()
        {
            InitializeComponent();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        //envía los parámetros a las pestañas hijas, es mejor que enviar a persistencia,
        //sobre todo por el uso de memoria y tiempo de respuesta
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (Children.Count == 1)
                {
                    return;
                }
                for (int pageindex = 1; pageindex < Children.Count; pageindex++)
                {
                    var page = Children[pageindex];
                    (page?.BindingContext as INavigationAware)?.OnNavigatedTo(parameters);
                }
            }
        }
    }
}
