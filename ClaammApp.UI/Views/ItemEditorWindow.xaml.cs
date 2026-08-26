using System.Windows;
using ClaammApp.Application;
using ClaammApp.Domain.Entities;

namespace ClaammApp.UI.Views;

public partial class ItemEditorWindow : Window
{
    private readonly Item _item;
    private string _descripcionOriginal = string.Empty;
    private string _unidadOriginal = string.Empty;
    private string _rubroOriginal = string.Empty;
    private decimal _precioOriginal;

    public ItemEditorWindow(Item item)
    {
        InitializeComponent();
        Ventanas.AjustarAlAreaTrabajo(this);
        Ventanas.HabilitarCierreConEscape(this);

        _item = item;

        TituloVentana.Text = item.Id == 0 ? "Nuevo ítem" : "Editar ítem";

        CboUnidad.ItemsSource = Composicion.Unidades.ObtenerTodos().Select(u => u.Nombre).ToList();
        var rubros = Composicion.Rubros.ObtenerTodos().Select(r => r.Nombre).ToList();
        CboRubro.ItemsSource = rubros;

        TxtDescripcion.Text = item.Descripcion;
        CboUnidad.Text = item.Unidad;
        TxtPrecio.Text = item.PrecioUnitario == 0 ? string.Empty : item.PrecioUnitario.ToString("#,##0.00");
        CboRubro.Text = item.Rubro;
        if (string.IsNullOrWhiteSpace(item.Rubro) && rubros.Count > 0)
            CboRubro.SelectedIndex = 0;

        _descripcionOriginal = TxtDescripcion.Text;
        _unidadOriginal = CboUnidad.Text;
        _rubroOriginal = CboRubro.Text ?? string.Empty;
        _precioOriginal = Entradas.TryDecimal(TxtPrecio.Text, out var precioInicial) ? precioInicial : 0m;

        Ventanas.ConfirmarSalidaSinGuardar(this, HayCambiosSinGuardar);
    }

    private bool HayCambiosSinGuardar()
    {
        Entradas.TryDecimal(TxtPrecio.Text, out var precio);

        return TxtDescripcion.Text != _descripcionOriginal
            || CboUnidad.Text != _unidadOriginal
            || (CboRubro.Text ?? string.Empty) != _rubroOriginal
            || precio != _precioOriginal;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void TxtPrecio_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        => Entradas.SoloDecimal(sender, e);

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        _item.Descripcion = TxtDescripcion.Text.Trim();
        _item.Unidad = string.IsNullOrWhiteSpace(CboUnidad.Text) ? "un" : CboUnidad.Text.Trim();
        _item.Rubro = CboRubro.Text?.Trim() ?? string.Empty;

        if (!Entradas.TryDecimal(TxtPrecio.Text, out var precio))
        {
            MessageBox.Show(this, "El precio unitario no es válido.", "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtPrecio.Focus();
            return;
        }

        if (precio < 0)
        {
            MessageBox.Show(this, "El precio unitario no puede ser negativo.", "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtPrecio.Focus();
            return;
        }

        _item.PrecioUnitario = Entradas.Redondear(precio, 2);

        try
        {
            Composicion.Items.Guardar(_item);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
