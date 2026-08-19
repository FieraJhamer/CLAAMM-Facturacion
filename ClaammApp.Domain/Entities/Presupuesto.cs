namespace ClaammApp.Domain.Entities;

public class Presupuesto
{
    public int Id { get; set; }

    public string ClienteNombre { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    public decimal DescuentoPorcentaje { get; set; }

    public List<PresupuestoItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.Total);

    public decimal TotalDescuento => Math.Round(Total * DescuentoPorcentaje / 100m, 2);

    public decimal Subtotal => Total - TotalDescuento;

    public decimal TotalImpuesto => Math.Round(Subtotal * 21m / 100m, 2);

    public decimal TotalNeto => Subtotal + TotalImpuesto;
}
