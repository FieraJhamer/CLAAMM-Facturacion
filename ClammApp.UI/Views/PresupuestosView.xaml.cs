using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClammApp.UI.ViewModels;
using Microsoft.Win32;

namespace ClammApp.UI.Views;

public partial class PresupuestosView : UserControl
{
    public PresupuestosView()
    {
        InitializeComponent();
        Cargar();
    }

    private void Cargar()
    {
        var presupuestos = Composicion.Presupuestos.ObtenerTodos();
        GridPresupuestos.ItemsSource = presupuestos.Select(p => new PresupuestoListaViewModel(p)).ToList();
    }

    private void BtnNuevo_Click(object sender, RoutedEventArgs e)
    {
        var editor = new PresupuestoEditorWindow(Composicion.Presupuestos.CrearNuevo()) { Owner = Window.GetWindow(this) };
        if (editor.ShowDialog() == true)
            Cargar();
    }

    private void BtnAbrir_Click(object sender, RoutedEventArgs e) => AbrirSeleccionado();

    private void GridPresupuestos_MouseDoubleClick(object sender, MouseButtonEventArgs e) => AbrirSeleccionado();

    private void AbrirSeleccionado()
    {
        if (GridPresupuestos.SelectedItem is not PresupuestoListaViewModel seleccion)
        {
            MessageBox.Show(Window.GetWindow(this) ?? System.Windows.Application.Current.MainWindow, "Seleccioná un presupuesto de la lista.", "CLAMM", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var editor = new PresupuestoEditorWindow(seleccion.Presupuesto) { Owner = Window.GetWindow(this) };
        if (editor.ShowDialog() == true)
            Cargar();
    }

    private void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (GridPresupuestos.SelectedItem is not PresupuestoListaViewModel seleccion)
        {
            MessageBox.Show(Window.GetWindow(this) ?? System.Windows.Application.Current.MainWindow, "Seleccioná un presupuesto de la lista.", "CLAMM", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var confirmar = MessageBox.Show(
            Window.GetWindow(this) ?? System.Windows.Application.Current.MainWindow,
            $"¿Eliminar el presupuesto de \"{seleccion.Cliente}\"?",
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmar == MessageBoxResult.Yes)
        {
            Composicion.Presupuestos.Eliminar(seleccion.Presupuesto.Id);
            Cargar();
        }
    }

    private void BtnExportar_Click(object sender, RoutedEventArgs e)
    {
        if (GridPresupuestos.SelectedItem is not PresupuestoListaViewModel seleccion)
        {
            MessageBox.Show(Window.GetWindow(this) ?? System.Windows.Application.Current.MainWindow, "Seleccioná un presupuesto de la lista.", "CLAMM", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialogo = new SaveFileDialog
        {
            Title = "Guardar presupuesto PDF",
            Filter = "PDF (*.pdf)|*.pdf",
            FileName = $"Presupuesto_{seleccion.Presupuesto.Id:D6}_{seleccion.Cliente}.pdf"
        };

        if (dialogo.ShowDialog(Window.GetWindow(this) ?? System.Windows.Application.Current.MainWindow) != true)
            return;

        try
        {
            var config = Composicion.Configuracion.ObtenerEmpresa();
            Composicion.Presupuestos.ExportarPdf(seleccion.Presupuesto, config, dialogo.FileName);
            MessageBox.Show(Window.GetWindow(this) ?? System.Windows.Application.Current.MainWindow, "PDF generado correctamente.", "CLAMM", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(Window.GetWindow(this) ?? System.Windows.Application.Current.MainWindow, "No se pudo generar el PDF: " + ex.Message, "CLAMM", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
