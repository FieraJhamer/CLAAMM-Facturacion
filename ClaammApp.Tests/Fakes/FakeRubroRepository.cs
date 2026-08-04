using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;

namespace ClaammApp.Tests.Fakes;

public class FakeRubroRepository : IRubroRepository
{
    private readonly List<Rubro> _rubros = new();
    private int _siguienteId = 1;

    public IEnumerable<Rubro> ObtenerTodos() => _rubros.ToList();

    public int Insertar(string nombre)
    {
        var rubro = new Rubro { Id = _siguienteId++, Nombre = nombre };
        _rubros.Add(rubro);
        return rubro.Id;
    }

    public void Actualizar(int id, string nombre)
    {
        var rubro = _rubros.First(r => r.Id == id);
        rubro.Nombre = nombre;
    }

    public void Eliminar(int id) => _rubros.RemoveAll(r => r.Id == id);
}
