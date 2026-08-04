using ClaammApp.Application.Exceptions;
using ClaammApp.Application.Services;
using ClaammApp.Domain.Entities;
using ClaammApp.Tests.Fakes;

namespace ClaammApp.Tests.Application;

public class ItemServiceTests
{
    private readonly FakeItemRepository _repositorio = new();
    private readonly FakeUnidadRepository _unidades = new();
    private readonly ItemService _servicio;

    public ItemServiceTests()
    {
        _servicio = new ItemService(_repositorio, _unidades);
    }

    [Fact]
    public void Guardar_NuevoItem_AsignaCodigoEId()
    {
        var item = new Item { Descripcion = "Revoque", Unidad = "m2", PrecioUnitario = 100 };

        _servicio.Guardar(item);

        Assert.True(item.Id > 0);
        Assert.Equal("0000001", item.Codigo);
        Assert.Single(_repositorio.GetAll());
    }

    [Fact]
    public void Guardar_DescripcionVacia_LanzaValidacion()
    {
        var item = new Item { Descripcion = "   ", Unidad = "un", PrecioUnitario = 100 };

        var ex = Assert.Throws<ValidacionException>(() => _servicio.Guardar(item));
        Assert.Contains("descripción", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Guardar_PrecioNegativo_LanzaValidacion()
    {
        var item = new Item { Descripcion = "Item", Unidad = "un", PrecioUnitario = -1 };

        Assert.Throws<ValidacionException>(() => _servicio.Guardar(item));
    }

    [Fact]
    public void Guardar_Edicion_NoCambiaCodigo()
    {
        var item = new Item { Id = 1, Codigo = "0000005", Descripcion = "Original", Unidad = "un", PrecioUnitario = 10 };
        _repositorio.Agregar(item);

        item.Descripcion = "Modificada";
        _servicio.Guardar(item);

        Assert.Equal("0000005", item.Codigo);
        Assert.Single(_repositorio.GetAll());
    }

    [Fact]
    public void Guardar_Edicion_ActualizaRegistro()
    {
        var item = new Item { Id = 1, Codigo = "0000005", Descripcion = "Original", Unidad = "un", PrecioUnitario = 10 };
        _repositorio.Agregar(item);

        item.PrecioUnitario = 999;
        _servicio.Guardar(item);

        Assert.Equal(999m, _repositorio.ObtenerPorId(1)!.PrecioUnitario);
    }

    [Fact]
    public void Guardar_NuevoItem_AseguraLaUnidad()
    {
        var item = new Item { Descripcion = "Item", Unidad = "ml", PrecioUnitario = 5 };

        _servicio.Guardar(item);

        Assert.Contains(_unidades.ObtenerTodos(), u => u.Nombre == "ml");
    }

    [Fact]
    public void CrearNuevo_UnidadPorDefecto_EsUn()
    {
        var nuevo = _servicio.CrearNuevo();

        Assert.Equal("un", nuevo.Unidad);
        Assert.Equal(0, nuevo.Id);
    }

    [Fact]
    public void Buscar_Vacio_DevuelveTodos()
    {
        _repositorio.Agregar(new Item { Id = 1, Descripcion = "A" });
        _repositorio.Agregar(new Item { Id = 2, Descripcion = "B" });

        var resultado = _servicio.Buscar("   ");

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public void IncrementarPrecios_Cero_LanzaValidacion()
    {
        Assert.Throws<ValidacionException>(() => _servicio.IncrementarPrecios(0));
    }

    [Fact]
    public void IncrementarPrecios_Negativo_LanzaValidacion()
    {
        Assert.Throws<ValidacionException>(() => _servicio.IncrementarPrecios(-5));
    }

    [Fact]
    public void IncrementarPrecios_Positivo_ActualizaPrecios()
    {
        _repositorio.Agregar(new Item { Id = 1, Descripcion = "A", PrecioUnitario = 100 });

        _servicio.IncrementarPrecios(10);

        Assert.Equal(110m, _repositorio.ObtenerPorId(1)!.PrecioUnitario);
    }

    [Fact]
    public void Eliminar_QuitaItem()
    {
        _repositorio.Agregar(new Item { Id = 1, Descripcion = "A" });

        _servicio.Eliminar(1);

        Assert.Empty(_repositorio.GetAll());
    }
}
