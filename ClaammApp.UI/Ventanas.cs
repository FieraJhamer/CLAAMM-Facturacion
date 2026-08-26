using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

    public static void ConfirmarSalidaSinGuardar(Window ventana, Func<bool> hayCambiosSinGuardar)
    {
        ventana.Closing += (_, e) =>
        {
            if (!hayCambiosSinGuardar())
                return;

            var guardando = false;
            try
            {
                guardando = ventana.DialogResult == true;
            }
            catch (InvalidOperationException)
            {
            }

            if (guardando)
                return;

            var respuesta = MessageBox.Show(
                ventana,
                "Hay cambios sin guardar.\n\n¿Querés salir sin guardarlos?",
                "Cambios sin guardar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (respuesta != MessageBoxResult.Yes)
                e.Cancel = true;
        };
    }

    public static void HabilitarCierreConEscape(Window ventana)
    {
        ventana.PreviewKeyDown += (_, e) =>
        {
            if (e.Key != Key.Escape)
                return;

            if (Keyboard.FocusedElement is not DependencyObject elemento)
                return;

            if (ObtenerAncestro<ComboBox>(elemento) is { IsDropDownOpen: true })
                return;

            if (ObtenerAncestro<DataGridCell>(elemento)?.IsEditing == true)
                return;

            e.Handled = true;
            try
            {
                ventana.DialogResult = false;
            }
            catch (InvalidOperationException)
            {
                ventana.Close();
            }
        };
    }

    private static T? ObtenerAncestro<T>(DependencyObject desde) where T : DependencyObject
    {
        var actual = desde;
        while (actual is not null and not Window)
        {
            if (actual is T coincidencia)
                return coincidencia;

            try
            {
                actual = VisualTreeHelper.GetParent(actual);
            }
            catch (InvalidOperationException)
            {
                actual = LogicalTreeHelper.GetParent(actual) as DependencyObject;
            }
        }

        return null;
    }
}
