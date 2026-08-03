using ClammApp.Application.Exceptions;
using ClammApp.Domain.Contracts;
using ClammApp.Domain.Entities;
using ClammApp.Domain.Enums;

namespace ClammApp.Application.Services;

public class ItemService
{
    private readonly IItemRepository _repositorio;

    public ItemService(IItemRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public IEnumerable<Item> ObtenerTodos() => _repositorio.GetAll();

    public IEnumerable<Item> Buscar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return ObtenerTodos();

        return _repositorio.Buscar(texto.Trim());
    }

    public Item CrearNuevo() => new() { Unidad = Unidad.un };

    public Item? Obtener(int id) => _repositorio.ObtenerPorId(id);

    public void Guardar(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.Descripcion))
            throw new ValidacionException("La descripción es obligatoria.");
        if (item.PrecioUnitario < 0)
            throw new ValidacionException("El precio unitario no puede ser negativo.");

        if (item.Id == 0)
        {
            item.Codigo = _repositorio.ObtenerProximoCodigo();
            item.Id = _repositorio.Insertar(item);
        }
        else
        {
            _repositorio.Actualizar(item);
        }
    }

    public void Eliminar(int id) => _repositorio.Eliminar(id);

    public void IncrementarPrecios(decimal porcentaje)
    {
        if (porcentaje <= 0)
            throw new ValidacionException("El porcentaje debe ser mayor a cero.");

        _repositorio.IncrementarPrecios(porcentaje);
    }
}
