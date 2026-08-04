using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClaammApp.UI.ViewModels;

namespace ClaammApp.UI.Views;

public partial class ItemsView : UserControl
{
    public ItemsView()
    {
        InitializeComponent();
        Cargar();
    }

    private void Cargar()
    {
        var filtro = TxtBuscar.Text.Trim();
        var items = Composicion.Items.Buscar(filtro);
        GridItems.ItemsSource = items.Select(i => new ItemListaViewModel(i)).ToList();
    }

    private void TxtBuscar_KeyUp(object sender, KeyEventArgs e) => Cargar();

    private void BtnNuevo_Click(object sender, RoutedEventArgs e)
    {
        var editor = new ItemEditorWindow(Composicion.Items.CrearNuevo()) { Owner = Window.GetWindow(this) };
        if (editor.ShowDialog() == true)
            Cargar();
    }

    private void BtnEditar_Click(object sender, RoutedEventArgs e) => EditarSeleccionado();

    private void GridItems_MouseDoubleClick(object sender, MouseButtonEventArgs e) => EditarSeleccionado();

    private void EditarSeleccionado()
    {
        if (GridItems.SelectedItem is not ItemListaViewModel seleccion)
        {
            MessageBox.Show(Window.GetWindow(this) ?? System.Windows.Application.Current.MainWindow, "Seleccioná un ítem de la lista.", "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var editor = new ItemEditorWindow(seleccion.Item) { Owner = Window.GetWindow(this) };
        if (editor.ShowDialog() == true)
            Cargar();
    }

    private void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (GridItems.SelectedItem is not ItemListaViewModel seleccion)
        {
            MessageBox.Show(Window.GetWindow(this) ?? System.Windows.Application.Current.MainWindow, "Seleccioná un ítem de la lista.", "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var confirmar = MessageBox.Show(
            Window.GetWindow(this) ?? System.Windows.Application.Current.MainWindow,
            $"¿Eliminar el ítem \"{seleccion.Descripcion}\"?",
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmar == MessageBoxResult.Yes)
        {
            Composicion.Items.Eliminar(seleccion.Item.Id);
            Cargar();
        }
    }

    private void BtnIncrementar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new IncrementoPreciosWindow { Owner = Window.GetWindow(this) };
        if (ventana.ShowDialog() == true)
            Cargar();
    }

    private void BtnUnidades_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new UnidadesWindow { Owner = Window.GetWindow(this) };
        ventana.ShowDialog();
    }
}
