using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;

namespace ClaammApp.Tests.Fakes;

public class FakePresupuestoRepository : IPresupuestoRepository
{
    private readonly List<Presupuesto> _presupuestos = new();
    private int _siguienteId = 1;

    public int Inserts { get; private set; }
    public int Updates { get; private set; }

    public IEnumerable<Presupuesto> ObtenerTodos() => _presupuestos.ToList();

    public Presupuesto? ObtenerPorId(int id) => _presupuestos.FirstOrDefault(p => p.Id == id);

    public int Insertar(Presupuesto presupuesto)
    {
        presupuesto.Id = _siguienteId++;
        _presupuestos.Add(presupuesto);
        Inserts++;
        return presupuesto.Id;
    }

    public void Actualizar(Presupuesto presupuesto)
    {
        var existente = _presupuestos.First(p => p.Id == presupuesto.Id);
        existente.ClienteNombre = presupuesto.ClienteNombre;
        existente.Fecha = presupuesto.Fecha;
        existente.Items = presupuesto.Items.ToList();
        Updates++;
    }

    public void Eliminar(int id) => _presupuestos.RemoveAll(p => p.Id == id);

    public void ReemplazarItems(int presupuestoId, IEnumerable<PresupuestoItem> items)
    {
        var presupuesto = _presupuestos.First(p => p.Id == presupuestoId);
        presupuesto.Items = items.ToList();
    }
}
