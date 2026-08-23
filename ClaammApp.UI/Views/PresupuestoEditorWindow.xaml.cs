using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClaammApp.Application;
using ClaammApp.UI.ViewModels;
using Microsoft.Win32;

namespace ClaammApp.UI.Views;

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
        TxtDescuento.Text = presupuesto.DescuentoPorcentaje.ToString("#,##0.##");

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

    private bool _saneandoDescuento;

    private void TxtDescuento_PreviewTextInput(object sender, TextCompositionEventArgs e)
        => Entradas.SoloDecimal(sender, e);

    private void TxtCantidad_PreviewTextInput(object sender, TextCompositionEventArgs e)
        => Entradas.SoloDecimal(sender, e);

    private void TxtDescuento_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_saneandoDescuento)
            return;

        if (!Entradas.TryDecimal(TxtDescuento.Text, out var valor))
            return;

        var saneado = Entradas.Limitar(Entradas.Redondear(valor, 2), 0m, 100m);
        _presupuesto.DescuentoPorcentaje = saneado;
        ActualizarTotal();

        if (valor == saneado)
            return;

        _saneandoDescuento = true;
        TxtDescuento.Text = saneado.ToString("#,##0.##");
        TxtDescuento.CaretIndex = TxtDescuento.Text.Length;
        _saneandoDescuento = false;
    }

    private decimal LeerDescuento()
        => Entradas.TryDecimal(TxtDescuento.Text, out var d)
            ? Entradas.Limitar(d, 0m, 100m)
            : 0m;

    private void ActualizarTotal()
    {
        var partes = new StringBuilder("TOTAL:  " + Formatos.Moneda(_presupuesto.Total));
        if (_presupuesto.DescuentoPorcentaje > 0)
            partes.Append($"   (%{_presupuesto.DescuentoPorcentaje:0.##} desc.)");
        partes.Append("   (+21% IVA)");
        TxtTotal.Text = partes.ToString();
    }

    private void TxtBuscarItem_TextChanged(object sender, TextChangedEventArgs e) => BuscarItems();

    private void TxtBuscarItem_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (ListaResultados.Visibility != Visibility.Visible)
            return;

        switch (e.Key)
        {
            case Key.Down:
                MoverSeleccion(1);
                e.Handled = true;
                break;
            case Key.Up:
                MoverSeleccion(-1);
                e.Handled = true;
                break;
            case Key.Enter:
                if (ListaResultados.SelectedItem is not null)
                {
                    AgregarSeleccionado();
                    e.Handled = true;
                }
                break;
        }
    }

    private void MoverSeleccion(int desplazamiento)
    {
        if (ListaResultados.Items.Count == 0)
            return;

        var indice = ListaResultados.SelectedIndex + desplazamiento;
        indice = Math.Clamp(indice, 0, ListaResultados.Items.Count - 1);
        ListaResultados.SelectedIndex = indice;
        ListaResultados.ScrollIntoView(ListaResultados.SelectedItem);
    }

    private void BuscarItems()
    {
        var texto = TxtBuscarItem.Text.Trim();
        if (string.IsNullOrWhiteSpace(texto))
        {
            ListaResultados.ItemsSource = null;
            ListaResultados.Visibility = Visibility.Collapsed;
            MensajeSinResultados.Visibility = Visibility.Collapsed;
            return;
        }

        var items = Composicion.Items.Buscar(texto).ToList();
        var hayResultados = items.Count > 0;

        ListaResultados.ItemsSource = hayResultados ? items.Select(i => new ItemListaViewModel(i)).ToList() : null;
        ListaResultados.Visibility = hayResultados ? Visibility.Visible : Visibility.Collapsed;
        MensajeSinResultados.Visibility = hayResultados ? Visibility.Collapsed : Visibility.Visible;
    }

    private void BtnAgregar_Click(object sender, RoutedEventArgs e) => AgregarSeleccionado();

    private void ListaResultados_MouseDoubleClick(object sender, MouseButtonEventArgs e) => AgregarSeleccionado();

    private void AgregarSeleccionado()
    {
        if (ListaResultados.SelectedItem is not ItemListaViewModel seleccion)
        {
            MessageBox.Show(this, "Buscá y seleccioná un ítem para agregar.", "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (!Entradas.TryDecimal(TxtCantidad.Text, out var cantidad) || cantidad <= 0)
        {
            MessageBox.Show(this, "Ingresá una cantidad mayor a cero.", "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtCantidad.Focus();
            return;
        }

        cantidad = Entradas.Redondear(cantidad, 3);

        Composicion.Presupuestos.AgregarItem(_presupuesto, seleccion.Item, cantidad);
        _lineas.Add(CrearLinea(_presupuesto.Items[^1]));
        ActualizarTotal();

        TxtBuscarItem.Clear();
        ListaResultados.ItemsSource = null;
        ListaResultados.Visibility = Visibility.Collapsed;
        MensajeSinResultados.Visibility = Visibility.Collapsed;
        TxtBuscarItem.Focus();
    }

    private void BtnEliminarLinea_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: LineaPresupuestoViewModel linea })
            return;

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
        _presupuesto.DescuentoPorcentaje = LeerDescuento();

        try
        {
            Composicion.Presupuestos.Guardar(_presupuesto);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "No se pudo generar el PDF: " + ex.Message, "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        DialogResult = true;
    }
}
