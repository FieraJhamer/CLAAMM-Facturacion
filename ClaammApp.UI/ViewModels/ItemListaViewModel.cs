using ClaammApp.Application;
using ClaammApp.Domain.Entities;

namespace ClaammApp.UI.ViewModels;

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
