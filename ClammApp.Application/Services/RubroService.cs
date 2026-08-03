using ClammApp.Application.Exceptions;
using ClammApp.Domain.Contracts;
using ClammApp.Domain.Entities;

namespace ClammApp.Application.Services;

public class RubroService
{
    private readonly IRubroRepository _repositorio;

    public RubroService(IRubroRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public IEnumerable<Rubro> ObtenerTodos() => _repositorio.ObtenerTodos();

    public void Agregar(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidacionException("El nombre del rubro es obligatorio.");

        var limpio = nombre.Trim();
        if (ObtenerTodos().Any(r => r.Nombre.Equals(limpio, StringComparison.OrdinalIgnoreCase)))
            throw new ValidacionException("Ya existe un rubro con ese nombre.");

        _repositorio.Insertar(limpio);
    }

    public void Actualizar(int id, string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidacionException("El nombre del rubro es obligatorio.");

        var limpio = nombre.Trim();
        if (ObtenerTodos().Any(r => r.Id != id && r.Nombre.Equals(limpio, StringComparison.OrdinalIgnoreCase)))
            throw new ValidacionException("Ya existe un rubro con ese nombre.");

        _repositorio.Actualizar(id, limpio);
    }

    public void Eliminar(int id) => _repositorio.Eliminar(id);
}
