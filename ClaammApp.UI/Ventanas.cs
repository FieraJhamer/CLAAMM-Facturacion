using System.Windows;
using System.Windows.Media;

namespace ClaammApp.UI;

public static class Ventanas
{
    private static readonly ImageSource IconoAplicacion =
        new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/favico.ico"));

    public static void AjustarAlAreaTrabajo(Window ventana)
    {
        ventana.Icon = IconoAplicacion;

        var area = SystemParameters.WorkArea;

        ventana.Width = Math.Min(ventana.Width, area.Width * 0.95);
        ventana.Height = Math.Min(ventana.Height, area.Height * 0.95);

        ventana.Width = Math.Max(ventana.Width, ventana.MinWidth);
        ventana.Height = Math.Max(ventana.Height, ventana.MinHeight);
    }
}
