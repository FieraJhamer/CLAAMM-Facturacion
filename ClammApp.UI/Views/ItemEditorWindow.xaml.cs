using System.Windows;
using ClammApp.Application;
using ClammApp.Domain.Entities;
using ClammApp.Domain.Enums;

namespace ClammApp.UI.Views;

public partial class ItemEditorWindow : Window
{
    private readonly Item _item;

    public ItemEditorWindow(Item item)
    {
        InitializeComponent();

        _item = item;

        TituloVentana.Text = item.Id == 0 ? "Nuevo ítem" : "Editar ítem";

        CboUnidad.ItemsSource = Enum.GetValues<Unidad>();
        CboRubro.ItemsSource = Composicion.Rubros.ObtenerTodos().Select(r => r.Nombre).ToList();

        TxtDescripcion.Text = item.Descripcion;
        CboUnidad.SelectedItem = item.Unidad;
        TxtPrecio.Text = item.PrecioUnitario == 0 ? string.Empty : item.PrecioUnitario.ToString("#,##0.00");
        CboRubro.Text = item.Rubro;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        _item.Descripcion = TxtDescripcion.Text.Trim();
        _item.Unidad = (Unidad)(CboUnidad.SelectedItem ?? Unidad.un);
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
