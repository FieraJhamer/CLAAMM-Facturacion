using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;

namespace ClaammApp.Tests.Fakes;

public class FakeUnidadRepository : IUnidadRepository
{
    private readonly List<UnidadMedida> _unidades = new();
    private int _siguienteId = 1;

    public IEnumerable<UnidadMedida> ObtenerTodos() => _unidades.ToList();

    public int Insertar(string nombre)
    {
        var unidad = new UnidadMedida { Id = _siguienteId++, Nombre = nombre };
        _unidades.Add(unidad);
        return unidad.Id;
    }

    public void Actualizar(int id, string nombre)
    {
        var unidad = _unidades.First(u => u.Id == id);
        unidad.Nombre = nombre;
    }

    public void Eliminar(int id) => _unidades.RemoveAll(u => u.Id == id);

    public void Asegurar(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return;
        if (_unidades.Any(u => u.Nombre.Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase)))
            return;
        Insertar(nombre.Trim());
    }
}
