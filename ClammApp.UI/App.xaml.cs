using System.Globalization;
using System.Windows;
using ClammApp.Infrastructure.Data;

namespace ClammApp.UI;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-AR");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-AR");

        Database.Inicializar();

        new MainWindow().Show();
    }
}
