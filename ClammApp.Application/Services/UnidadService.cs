using ClammApp.Application.Exceptions;
using ClammApp.Domain.Contracts;
using ClammApp.Domain.Entities;

namespace ClammApp.Application.Services;

public class UnidadService
{
    private readonly IUnidadRepository _repositorio;

    public UnidadService(IUnidadRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public IEnumerable<UnidadMedida> ObtenerTodos() => _repositorio.ObtenerTodos();

    public void Agregar(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidacionException("El nombre de la unidad es obligatorio.");

        var limpio = nombre.Trim();
        if (ObtenerTodos().Any(u => u.Nombre.Equals(limpio, StringComparison.OrdinalIgnoreCase)))
            throw new ValidacionException("Ya existe una unidad con ese nombre.");

        _repositorio.Insertar(limpio);
    }

    public void Actualizar(int id, string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ValidacionException("El nombre de la unidad es obligatorio.");

        var limpio = nombre.Trim();
        if (ObtenerTodos().Any(u => u.Id != id && u.Nombre.Equals(limpio, StringComparison.OrdinalIgnoreCase)))
            throw new ValidacionException("Ya existe una unidad con ese nombre.");

        _repositorio.Actualizar(id, limpio);
    }

    public void Eliminar(int id) => _repositorio.Eliminar(id);
}
