using ClaammApp.Domain.Entities;
using ClaammApp.Infrastructure.Repositories;

namespace ClaammApp.Tests.Infrastructure;

public class PresupuestoRepositoryTests : RepositorioTestBase
{
    private readonly PresupuestoRepository _repositorio;

    public PresupuestoRepositoryTests()
    {
        _repositorio = new PresupuestoRepository(ConnectionString);
    }

    private static Presupuesto ConItems(string cliente, DateTime fecha, params (string Descripcion, decimal Cantidad, decimal Precio)[] items)
    {
        var presupuesto = new Presupuesto { ClienteNombre = cliente, Fecha = fecha };
        foreach (var (descripcion, cantidad, precio) in items)
        {
            presupuesto.Items.Add(new PresupuestoItem
            {
                ItemId = 1,
                Descripcion = descripcion,
                Unidad = "un",
                Cantidad = cantidad,
                PrecioUnitario = precio
            });
        }
        return presupuesto;
    }

    [Fact]
    public void Insertar_PersisteCabeceraYItems()
    {
        var presupuesto = ConItems("Cliente A", new DateTime(2026, 8, 4),
            ("Item 1", 2, 100), ("Item 2", 3, 50));

        var id = _repositorio.Insertar(presupuesto);

        var obtenido = _repositorio.ObtenerPorId(id);
        Assert.NotNull(obtenido);
        Assert.Equal(id, obtenido!.Id);
        Assert.Equal("Cliente A", obtenido.ClienteNombre);
        Assert.Equal(2, obtenido.Items.Count);
        Assert.Equal(350m, obtenido.Total);
    }

    [Fact]
    public void Insertar_ConservaLaFechaCompleta()
    {
        var fecha = new DateTime(2026, 8, 4, 15, 30, 0);
        var presupuesto = ConItems("Cliente A", fecha, ("Item 1", 1, 10));

        var id = _repositorio.Insertar(presupuesto);

        Assert.Equal(fecha, _repositorio.ObtenerPorId(id)!.Fecha);
    }

    [Fact]
    public void ObtenerPorId_Inexistente_DevuelveNull()
    {
        Assert.Null(_repositorio.ObtenerPorId(999));
    }

    [Fact]
    public void Actualizar_ReemplazaItems()
    {
        var presupuesto = ConItems("Cliente A", DateTime.Today, ("Item 1", 1, 10));
        var id = _repositorio.Insertar(presupuesto);

        var modificado = _repositorio.ObtenerPorId(id)!;
        modificado.ClienteNombre = "Cliente B";
        modificado.Items.Add(new PresupuestoItem { ItemId = 2, Descripcion = "Item 2", Unidad = "un", Cantidad = 5, PrecioUnitario = 20 });

        _repositorio.Actualizar(modificado);

        var obtenido = _repositorio.ObtenerPorId(id)!;
        Assert.Equal("Cliente B", obtenido.ClienteNombre);
        Assert.Equal(2, obtenido.Items.Count);
        Assert.Equal(110m, obtenido.Total);
    }

    [Fact]
    public void Eliminar_QuitaCabeceraYItems()
    {
        var presupuesto = ConItems("Cliente A", DateTime.Today, ("Item 1", 1, 10));
        var id = _repositorio.Insertar(presupuesto);

        _repositorio.Eliminar(id);

        Assert.Null(_repositorio.ObtenerPorId(id));
        Assert.Empty(_repositorio.ObtenerTodos());
    }

    [Fact]
    public void ObtenerTodos_OrdenaPorFechaDescendente()
    {
        var idViejo = _repositorio.Insertar(ConItems("Viejo", new DateTime(2026, 8, 1), ("Item", 1, 10)));
        var idNuevo = _repositorio.Insertar(ConItems("Nuevo", new DateTime(2026, 8, 10), ("Item", 1, 10)));

        var todos = _repositorio.ObtenerTodos().ToList();

        Assert.Equal(2, todos.Count);
        Assert.Equal(idNuevo, todos[0].Id);
        Assert.Equal(idViejo, todos[1].Id);
    }

    [Fact]
    public void Insertar_PersisteDescuento()
    {
        var presupuesto = ConItems("Cliente A", DateTime.Today, ("Item 1", 2, 100));
        presupuesto.DescuentoPorcentaje = 10m;

        var id = _repositorio.Insertar(presupuesto);

        var obtenido = _repositorio.ObtenerPorId(id)!;
        Assert.Equal(10m, obtenido.DescuentoPorcentaje);
        Assert.Equal(200m, obtenido.Total);
        Assert.Equal(20m, obtenido.TotalDescuento);
        Assert.Equal(180m, obtenido.Subtotal);
        Assert.Equal(37.8m, obtenido.TotalImpuesto);
        Assert.Equal(217.8m, obtenido.TotalNeto);
    }

    [Fact]
    public void ReemplazarItems_ReemplazaLineas()
    {
        var presupuesto = ConItems("Cliente A", DateTime.Today, ("Item 1", 1, 10));
        var id = _repositorio.Insertar(presupuesto);

        _repositorio.ReemplazarItems(id, new[]
        {
            new PresupuestoItem { ItemId = 9, Descripcion = "Nuevo", Unidad = "m2", Cantidad = 4, PrecioUnitario = 25 }
        });

        var obtenido = _repositorio.ObtenerPorId(id)!;
        Assert.Single(obtenido.Items);
        Assert.Equal("Nuevo", obtenido.Items[0].Descripcion);
        Assert.Equal(100m, obtenido.Total);
    }
}
