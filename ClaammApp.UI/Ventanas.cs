using System.Windows;

namespace ClaammApp.UI;

public static class Ventanas
{
    public static void AjustarAlAreaTrabajo(Window ventana)
    {
        var area = SystemParameters.WorkArea;

        ventana.Width = Math.Min(ventana.Width, area.Width * 0.95);
        ventana.Height = Math.Min(ventana.Height, area.Height * 0.95);

        ventana.Width = Math.Max(ventana.Width, ventana.MinWidth);
        ventana.Height = Math.Max(ventana.Height, ventana.MinHeight);
    }
}
