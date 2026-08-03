using System.Windows;
using System.Windows.Controls;
using ClammApp.Domain.Entities;

namespace ClammApp.UI.Views;

public partial class UnidadesWindow : Window
{
    public UnidadesWindow()
    {
        InitializeComponent();
        Ventanas.AjustarAlAreaTrabajo(this);
        Cargar();
    }

    private void Cargar()
    {
        ListaUnidades.ItemsSource = Composicion.Unidades.ObtenerTodos().ToList();
    }

    private void ListaUnidades_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var seleccionado = ListaUnidades.SelectedItem as UnidadMedida;
        BtnEditar.IsEnabled = seleccionado != null;
        BtnEliminar.IsEnabled = seleccionado != null;

        if (seleccionado != null)
            TxtNombre.Text = seleccionado.Nombre;
    }

    private void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Composicion.Unidades.Agregar(TxtNombre.Text);
            TxtNombre.Clear();
            Cargar();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "CLAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (ListaUnidades.SelectedItem is not UnidadMedida unidad)
            return;

        try
        {
            Composicion.Unidades.Actualizar(unidad.Id, TxtNombre.Text);
            Cargar();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "CLAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (ListaUnidades.SelectedItem is not UnidadMedida unidad)
            return;

        var confirmar = MessageBox.Show(
            this,
            $"¿Eliminar la unidad \"{unidad.Nombre}\"?\nLos ítems que la usan conservarán el texto de la unidad.",
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmar == MessageBoxResult.Yes)
        {
            Composicion.Unidades.Eliminar(unidad.Id);
            Cargar();
        }
    }
}
