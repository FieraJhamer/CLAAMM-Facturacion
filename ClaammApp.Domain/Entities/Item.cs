namespace ClaammApp.Domain.Entities;

public class Item
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public string Unidad { get; set; } = string.Empty;

    public decimal PrecioUnitario { get; set; }

    public string Rubro { get; set; } = string.Empty;
}
