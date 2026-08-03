namespace ClammApp.Domain.Entities;

public class PresupuestoItem
{
    public int Id { get; set; }

    public int PresupuestoId { get; set; }

    public int ItemId { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public string Unidad { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Total => Cantidad * PrecioUnitario;
}
