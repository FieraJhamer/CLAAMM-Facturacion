using System.Windows;

namespace ClammApp.UI.Views;

public partial class IncrementoPreciosWindow : Window
{
    public IncrementoPreciosWindow()
    {
        InitializeComponent();
        Ventanas.AjustarAlAreaTrabajo(this);
        TxtPorcentaje.Focus();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void BtnAplicar_Click(object sender, RoutedEventArgs e)
    {
        if (!decimal.TryParse(TxtPorcentaje.Text, out var porcentaje))
        {
            MessageBox.Show(this, "Ingresá un porcentaje válido.", "CLAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtPorcentaje.Focus();
            return;
        }

        var confirmar = MessageBox.Show(
            this,
            $"¿Aplicar un aumento del {porcentaje}% a todos los precios?",
            "Confirmar",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmar != MessageBoxResult.Yes)
            return;

        try
        {
            Composicion.Items.IncrementarPrecios(porcentaje);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "CLAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
