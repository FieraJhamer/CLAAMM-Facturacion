using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;

namespace ClaammApp.Tests.Fakes;

public class FakeItemRepository : IItemRepository
{
    private readonly List<Item> _items = new();
    private int _siguienteId = 1;
    private int _siguienteCodigo = 1;

    public IEnumerable<Item> GetAll() => _items.ToList();

    public IEnumerable<Item> Buscar(string texto) => _items
        .Where(i => i.Descripcion.Contains(texto, StringComparison.OrdinalIgnoreCase)
                 || i.Rubro.Contains(texto, StringComparison.OrdinalIgnoreCase)
                 || i.Codigo.Contains(texto, StringComparison.OrdinalIgnoreCase))
        .ToList();

    public Item? ObtenerPorId(int id) => _items.FirstOrDefault(i => i.Id == id);

    public int Insertar(Item item)
    {
        item.Id = _siguienteId++;
        _items.Add(item);
        return item.Id;
    }

    public void Actualizar(Item item)
    {
        var existente = _items.First(i => i.Id == item.Id);
        existente.Descripcion = item.Descripcion;
        existente.Unidad = item.Unidad;
        existente.PrecioUnitario = item.PrecioUnitario;
        existente.Rubro = item.Rubro;
    }

    public void Eliminar(int id) => _items.RemoveAll(i => i.Id == id);

    public string ObtenerProximoCodigo() => (_siguienteCodigo++).ToString("D7");

    public void IncrementarPrecios(decimal porcentaje)
    {
        foreach (var item in _items)
            item.AplicarIncremento(porcentaje);
    }

    public void Agregar(Item item) => _items.Add(item);
}
