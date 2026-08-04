using ClaammApp.Domain.Entities;

namespace ClaammApp.Domain.Contracts;

public interface IPresupuestoRepository
{
    IEnumerable<Presupuesto> ObtenerTodos();

    Presupuesto? ObtenerPorId(int id);

    int Insertar(Presupuesto presupuesto);

    void Actualizar(Presupuesto presupuesto);

    void Eliminar(int id);

    void ReemplazarItems(int presupuestoId, IEnumerable<PresupuestoItem> items);
}
