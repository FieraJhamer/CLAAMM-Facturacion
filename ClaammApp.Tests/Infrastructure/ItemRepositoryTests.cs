using ClaammApp.Domain.Entities;
using ClaammApp.Infrastructure.Repositories;

namespace ClaammApp.Tests.Infrastructure;

public class ItemRepositoryTests : RepositorioTestBase
{
    private readonly ItemRepository _repositorio;

    public ItemRepositoryTests()
    {
        _repositorio = new ItemRepository(ConnectionString);
    }

    private int InsertarItem(string descripcion, string unidad = "un", decimal precio = 100, string rubro = "", string codigo = "")
    {
        return _repositorio.Insertar(new Item
        {
            Codigo = codigo,
            Descripcion = descripcion,
            Unidad = unidad,
            PrecioUnitario = precio,
            Rubro = rubro
        });
    }

    [Fact]
    public void Insertar_DevuelveIdYPersiste()
    {
        var id = InsertarItem("Revoque fino");

        Assert.True(id > 0);
        var item = _repositorio.ObtenerPorId(id);
        Assert.NotNull(item);
        Assert.Equal("Revoque fino", item!.Descripcion);
    }

    [Fact]
    public void GetAll_OrdenaPorDescripcion()
    {
        InsertarItem("Zanja");
        InsertarItem("Aislación");

        var descripciones = _repositorio.GetAll().Select(i => i.Descripcion).ToList();

        Assert.Equal(new[] { "Aislación", "Zanja" }, descripciones);
    }

    [Fact]
    public void Buscar_IgnoraAcentosYMayusculas()
    {
        InsertarItem("Hormigón elaborado", rubro: "Estructura");

        var resultado = _repositorio.Buscar("HORMIGON");

        Assert.Single(resultado);
        Assert.Equal("Hormigón elaborado", resultado.Single().Descripcion);
    }

    [Fact]
    public void Buscar_EncuentraPorRubro()
    {
        InsertarItem("Contrapiso", rubro: "Estructura");

        Assert.Single(_repositorio.Buscar("estruct"));
    }

    [Fact]
    public void Buscar_EncuentraPorCodigo()
    {
        InsertarItem("Algo", codigo: "0000042");

        Assert.Single(_repositorio.Buscar("0000042"));
    }

    [Fact]
    public void Buscar_SinCoincidencias_DevuelveVacio()
    {
        InsertarItem("Revoque");

        Assert.Empty(_repositorio.Buscar("noexiste"));
    }

    [Fact]
    public void ObtenerProximoCodigo_SecuencialYActualizaContador()
    {
        Assert.Equal("0000001", _repositorio.ObtenerProximoCodigo());
        Assert.Equal("0000002", _repositorio.ObtenerProximoCodigo());
        Assert.Equal("0000003", _repositorio.ObtenerProximoCodigo());

        var otroRepositorio = new ItemRepository(ConnectionString);
        Assert.Equal("0000004", otroRepositorio.ObtenerProximoCodigo());
    }

    [Fact]
    public void Actualizar_ModificaCamposConservaCodigo()
    {
        var id = InsertarItem("Original", unidad: "un", precio: 100, rubro: "R1", codigo: "0000010");

        _repositorio.Actualizar(new Item
        {
            Id = id,
            Codigo = "X", // no debería tomarse en cuenta
            Descripcion = "Nueva",
            Unidad = "m2",
            PrecioUnitario = 150,
            Rubro = "R2"
        });

        var item = _repositorio.ObtenerPorId(id)!;
        Assert.Equal("Nueva", item.Descripcion);
        Assert.Equal("m2", item.Unidad);
        Assert.Equal(150m, item.PrecioUnitario);
        Assert.Equal("R2", item.Rubro);
        Assert.Equal("0000010", item.Codigo);
    }

    [Fact]
    public void Eliminar_QuitaItem()
    {
        var id = InsertarItem("Temporal");

        _repositorio.Eliminar(id);

        Assert.Null(_repositorio.ObtenerPorId(id));
        Assert.Empty(_repositorio.GetAll());
    }

    [Fact]
    public void IncrementarPrecios_AplicaPorcentajeATodos()
    {
        InsertarItem("A", precio: 100);
        InsertarItem("B", precio: 200);

        _repositorio.IncrementarPrecios(10);

        var precios = _repositorio.GetAll().Select(i => i.PrecioUnitario).OrderBy(p => p).ToList();
        Assert.Equal(new[] { 110m, 220m }, precios);
    }

    [Fact]
    public void IncrementarPrecios_RedondeaADosDecimales()
    {
        InsertarItem("A", precio: 33.33m);

        _repositorio.IncrementarPrecios(5);

        Assert.Equal(35m, _repositorio.GetAll().Single().PrecioUnitario);
    }
}
