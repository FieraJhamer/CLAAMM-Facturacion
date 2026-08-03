using System.Windows;
using ClammApp.Application;
using ClammApp.Domain.Entities;

namespace ClammApp.UI.Views;

public partial class ItemEditorWindow : Window
{
    private readonly Item _item;

    public ItemEditorWindow(Item item)
    {
        InitializeComponent();
        Ventanas.AjustarAlAreaTrabajo(this);

        _item = item;

        TituloVentana.Text = item.Id == 0 ? "Nuevo ítem" : "Editar ítem";

        CboUnidad.ItemsSource = Composicion.Unidades.ObtenerTodos().Select(u => u.Nombre).ToList();
        CboRubro.ItemsSource = Composicion.Rubros.ObtenerTodos().Select(r => r.Nombre).ToList();

        TxtDescripcion.Text = item.Descripcion;
        CboUnidad.Text = item.Unidad;
        TxtPrecio.Text = item.PrecioUnitario == 0 ? string.Empty : item.PrecioUnitario.ToString("#,##0.00");
        CboRubro.Text = item.Rubro;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        _item.Descripcion = TxtDescripcion.Text.Trim();
        _item.Unidad = string.IsNullOrWhiteSpace(CboUnidad.Text) ? "un" : CboUnidad.Text.Trim();
        _item.Rubro = CboRubro.Text?.Trim() ?? string.Empty;

        if (!decimal.TryParse(TxtPrecio.Text, out var precio))
        {
            MessageBox.Show(this, "El precio unitario no es válido.", "CLAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtPrecio.Focus();
            return;
        }

        _item.PrecioUnitario = precio;

        try
        {
            Composicion.Items.Guardar(_item);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "CLAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
