using ClammApp.Application;
using ClammApp.Domain.Entities;

namespace ClammApp.UI.ViewModels;

public class ItemListaViewModel
{
    public ItemListaViewModel(Item item)
    {
        Item = item;
    }

    public Item Item { get; }

    public string Codigo => Item.Codigo;

    public string Descripcion => Item.Descripcion;

    public string Unidad => Item.Unidad;

    public string PrecioTexto => Formatos.Moneda(Item.PrecioUnitario);

    public string Rubro => Item.Rubro;
}
