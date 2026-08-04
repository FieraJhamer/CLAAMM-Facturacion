using ClaammApp.Domain.Entities;

namespace ClaammApp.Domain.Contracts;

public interface IItemRepository
{
    IEnumerable<Item> GetAll();

    IEnumerable<Item> Buscar(string texto);

    Item? ObtenerPorId(int id);

    int Insertar(Item item);

    void Actualizar(Item item);

    void Eliminar(int id);

    string ObtenerProximoCodigo();
}
