using ClammApp.Domain.Enums;

namespace ClammApp.Domain.Entities;

public class Item
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public Unidad Unidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public string Rubro { get; set; } = string.Empty;

    public void AplicarIncremento(decimal porcentaje)
    {
        PrecioUnitario = Math.Round(PrecioUnitario * (1 + porcentaje / 100m), 2);
    }
}
