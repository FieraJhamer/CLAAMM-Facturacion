using System.Windows;
using System.Windows.Controls;
using ClaammApp.Domain.Entities;

namespace ClaammApp.UI.Views;

public partial class RubrosWindow : Window
{
    public RubrosWindow()
    {
        InitializeComponent();
        Ventanas.AjustarAlAreaTrabajo(this);
        Ventanas.HabilitarCierreConEscape(this);
        Cargar();
    }

    private void Cargar()
    {
        ListaRubros.ItemsSource = Composicion.Rubros.ObtenerTodos().ToList();
    }

    private void ListaRubros_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var seleccionado = ListaRubros.SelectedItem as Rubro;
        BtnEditar.IsEnabled = seleccionado != null;
        BtnEliminar.IsEnabled = seleccionado != null;

        if (seleccionado != null)
            TxtNombre.Text = seleccionado.Nombre;
    }

    private void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Composicion.Rubros.Agregar(TxtNombre.Text);
            TxtNombre.Clear();
            Cargar();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (ListaRubros.SelectedItem is not Rubro rubro)
            return;

        try
        {
            Composicion.Rubros.Actualizar(rubro.Id, TxtNombre.Text);
            Cargar();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (ListaRubros.SelectedItem is not Rubro rubro)
            return;

        var confirmar = MessageBox.Show(
            this,
            $"¿Eliminar el rubro \"{rubro.Nombre}\"?\nLos ítems que lo usan conservarán el texto del rubro.",
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmar == MessageBoxResult.Yes)
        {
            Composicion.Rubros.Eliminar(rubro.Id);
            Cargar();
        }
    }
}
