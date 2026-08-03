using ClammApp.Domain.Entities;

namespace ClammApp.Domain.Contracts;

public interface IUnidadRepository
{
    IEnumerable<UnidadMedida> ObtenerTodos();

    int Insertar(string nombre);

    void Actualizar(int id, string nombre);

    void Eliminar(int id);

    void Asegurar(string nombre);
}
