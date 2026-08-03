using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClammApp.Application;
using ClammApp.UI.ViewModels;
using Microsoft.Win32;

namespace ClammApp.UI.Views;

public partial class PresupuestoEditorWindow : Window
{
    private readonly Domain.Entities.Presupuesto _presupuesto;
    private readonly ObservableCollection<LineaPresupuestoViewModel> _lineas;

    public PresupuestoEditorWindow(Domain.Entities.Presupuesto presupuesto)
    {
        InitializeComponent();
        Ventanas.AjustarAlAreaTrabajo(this);

        _presupuesto = presupuesto;

        TituloVentana.Text = presupuesto.Id == 0
            ? "Nuevo presupuesto"
            : $"Presupuesto Nº {presupuesto.Id:D6}";

        TxtCliente.Text = presupuesto.ClienteNombre;
        FechaPicker.SelectedDate = presupuesto.Fecha;

        _lineas = new ObservableCollection<LineaPresupuestoViewModel>();
        foreach (var item in presupuesto.Items)
            _lineas.Add(CrearLinea(item));
        GridLineas.ItemsSource = _lineas;

        ActualizarTotal();
    }

    private LineaPresupuestoViewModel CrearLinea(Domain.Entities.PresupuestoItem item)
    {
        var linea = new LineaPresupuestoViewModel(item);
        linea.PropertyChanged += Linea_PropertyChanged;
        return linea;
    }

    private void Linea_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LineaPresupuestoViewModel.TotalTexto))
            ActualizarTotal();
    }

    private void ActualizarTotal()
    {
        TxtTotal.Text = "TOTAL:  " + Formatos.Moneda(_presupuesto.Total);
    }

    private void TxtBuscarItem_KeyUp(object sender, KeyEventArgs e) => BuscarItems();

    private void BuscarItems()
    {
        var texto = TxtBuscarItem.Text.Trim();
        if (string.IsNullOrWhiteSpace(texto))
        {
            ListaResultados.ItemsSource = null;
            return;
        }

        var items = Composicion.Items.Buscar(texto);
        ListaResultados.ItemsSource = items.Select(i => new ItemListaViewModel(i)).ToList();
    }

    private void BtnAgregar_Click(object sender, RoutedEventArgs e) => AgregarSeleccionado();

    private void ListaResultados_MouseDoubleClick(object sender, MouseButtonEventArgs e) => AgregarSeleccionado();

    private void AgregarSeleccionado()
    {
        if (ListaResultados.SelectedItem is not ItemListaViewModel seleccion)
        {
            MessageBox.Show(this, "Buscá y seleccioná un ítem para agregar.", "CLAMM", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (!decimal.TryParse(TxtCantidad.Text, out var cantidad) || cantidad <= 0)
        {
            MessageBox.Show(this, "Ingresá una cantidad mayor a cero.", "CLAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtCantidad.Focus();
            return;
        }

        Composicion.Presupuestos.AgregarItem(_presupuesto, seleccion.Item, cantidad);
        _lineas.Add(CrearLinea(_presupuesto.Items[^1]));
        ActualizarTotal();
    }

    private void BtnQuitarLinea_Click(object sender, RoutedEventArgs e)
    {
        if (GridLineas.SelectedItem is not LineaPresupuestoViewModel linea)
        {
            MessageBox.Show(this, "Seleccioná una línea para quitar.", "CLAMM", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _lineas.Remove(linea);
        _presupuesto.Items.Remove(linea.Item);
        ActualizarTotal();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void BtnGuardar_Click(object sender, RoutedEventArgs e) => Guardar(exportar: false);

    private void BtnGuardarYExportar_Click(object sender, RoutedEventArgs e) => Guardar(exportar: true);

    private void Guardar(bool exportar)
    {
        _presupuesto.ClienteNombre = TxtCliente.Text.Trim();
        _presupuesto.Fecha = FechaPicker.SelectedDate ?? DateTime.Today;

        try
        {
            Composicion.Presupuestos.Guardar(_presupuesto);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "CLAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (exportar)
        {
            var dialogo = new SaveFileDialog
            {
                Title = "Guardar presupuesto PDF",
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = $"Presupuesto_{_presupuesto.Id:D6}_{_presupuesto.ClienteNombre}.pdf"
            };

            if (dialogo.ShowDialog(this) == true)
            {
                try
                {
                    var config = Composicion.Configuracion.ObtenerEmpresa();
                    Composicion.Presupuestos.ExportarPdf(_presupuesto, config, dialogo.FileName);
                    MessageBox.Show(this, "PDF generado correctamente.", "CLAMM", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "No se pudo generar el PDF: " + ex.Message, "CLAMM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        DialogResult = true;
    }
}
