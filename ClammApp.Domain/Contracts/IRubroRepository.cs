using ClammApp.Domain.Entities;

namespace ClammApp.Domain.Contracts;

public interface IRubroRepository
{
    IEnumerable<Rubro> ObtenerTodos();

    int Insertar(string nombre);

    void Actualizar(int id, string nombre);

    void Eliminar(int id);
}
