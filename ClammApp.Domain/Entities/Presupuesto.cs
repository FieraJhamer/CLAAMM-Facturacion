namespace ClammApp.Domain.Entities;

public class Presupuesto
{
    public int Id { get; set; }

    public string ClienteNombre { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    public List<PresupuestoItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.Total);
}
