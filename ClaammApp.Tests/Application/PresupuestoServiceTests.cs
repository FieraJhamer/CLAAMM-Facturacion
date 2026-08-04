using ClaammApp.Application.Exceptions;
using ClaammApp.Application.Services;
using ClaammApp.Domain.Entities;
using ClaammApp.Tests.Fakes;

namespace ClaammApp.Tests.Application;

public class PresupuestoServiceTests
{
    private readonly FakePresupuestoRepository _repositorio = new();
    private readonly FakePdfGenerator _pdf = new();
    private readonly PresupuestoService _servicio;

    public PresupuestoServiceTests()
    {
        _servicio = new PresupuestoService(_repositorio, _pdf);
    }

    private static Presupuesto ConUnItem()
    {
        return new Presupuesto
        {
            ClienteNombre = "Cliente",
            Items = { new PresupuestoItem { Descripcion = "Item", Unidad = "un", Cantidad = 1, PrecioUnitario = 10 } }
        };
    }

    [Fact]
    public void Guardar_ClienteVacio_LanzaValidacion()
    {
        var presupuesto = ConUnItem();
        presupuesto.ClienteNombre = "";

        Assert.Throws<ValidacionException>(() => _servicio.Guardar(presupuesto));
    }

    [Fact]
    public void Guardar_SinItems_LanzaValidacion()
    {
        var presupuesto = new Presupuesto { ClienteNombre = "Cliente" };

        Assert.Throws<ValidacionException>(() => _servicio.Guardar(presupuesto));
    }

    [Fact]
    public void Guardar_CantidadCero_LanzaValidacion()
    {
        var presupuesto = ConUnItem();
        presupuesto.Items[0].Cantidad = 0;

        Assert.Throws<ValidacionException>(() => _servicio.Guardar(presupuesto));
    }

    [Fact]
    public void Guardar_CantidadNegativa_LanzaValidacion()
    {
        var presupuesto = ConUnItem();
        presupuesto.Items[0].Cantidad = -3;

        Assert.Throws<ValidacionException>(() => _servicio.Guardar(presupuesto));
    }

    [Fact]
    public void Guardar_NuevoPresupuesto_InsertaYAsignaId()
    {
        var presupuesto = ConUnItem();

        _servicio.Guardar(presupuesto);

        Assert.True(presupuesto.Id > 0);
        Assert.Equal(1, _repositorio.Inserts);
    }

    [Fact]
    public void Guardar_PresupuestoExistente_Actualiza()
    {
        var presupuesto = ConUnItem();
        var id = _repositorio.Insertar(presupuesto);
        presupuesto.ClienteNombre = "Cliente modificado";

        _servicio.Guardar(presupuesto);

        Assert.Equal(1, _repositorio.Updates);
        Assert.Equal(1, _repositorio.Inserts);
        Assert.Equal("Cliente modificado", _repositorio.ObtenerPorId(id)!.ClienteNombre);
    }

    [Fact]
    public void AgregarItem_AgregaLineaConSnapshotDelPrecio()
    {
        var presupuesto = new Presupuesto();
        var item = new Item { Id = 7, Descripcion = "Item X", Unidad = "m2", PrecioUnitario = 250 };

        _servicio.AgregarItem(presupuesto, item, 3);

        var linea = Assert.Single(presupuesto.Items);
        Assert.Equal(7, linea.ItemId);
        Assert.Equal("Item X", linea.Descripcion);
        Assert.Equal("m2", linea.Unidad);
        Assert.Equal(3m, linea.Cantidad);
        Assert.Equal(250m, linea.PrecioUnitario);
        Assert.Equal(750m, linea.Total);
    }

    [Fact]
    public void AgregarItem_UsaSnapshotNoElPrecioActualDelItem()
    {
        var presupuesto = new Presupuesto();
        var item = new Item { Id = 7, Descripcion = "Item X", Unidad = "un", PrecioUnitario = 100 };

        _servicio.AgregarItem(presupuesto, item, 1);
        item.PrecioUnitario = 999;

        Assert.Equal(100m, presupuesto.Items[0].PrecioUnitario);
    }

    [Fact]
    public void CrearNuevo_FechaEsHoy()
    {
        var presupuesto = _servicio.CrearNuevo();

        Assert.Equal(DateTime.Today, presupuesto.Fecha);
        Assert.Equal(0, presupuesto.Id);
    }

    [Fact]
    public void ExportarPdf_EscribeElArchivo()
    {
        var presupuesto = ConUnItem();
        var config = new ConfiguracionEmpresa { RazonSocial = "CLAAMM" };
        var ruta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pdf");
        try
        {
            _servicio.ExportarPdf(presupuesto, config, ruta);

            Assert.True(File.Exists(ruta));
            Assert.Equal(new byte[] { 37, 80, 68, 70 }, File.ReadAllBytes(ruta));
            Assert.Same(presupuesto, _pdf.UltimoPresupuesto);
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    [Fact]
    public void Obtener_PassthroughAlRepositorio()
    {
        var presupuesto = ConUnItem();
        var id = _repositorio.Insertar(presupuesto);

        Assert.NotNull(_servicio.Obtener(id));
        Assert.Null(_servicio.Obtener(999));
    }

    [Fact]
    public void Eliminar_QuitaPresupuesto()
    {
        var presupuesto = ConUnItem();
        var id = _repositorio.Insertar(presupuesto);

        _servicio.Eliminar(id);

        Assert.Empty(_repositorio.ObtenerTodos());
    }
}
