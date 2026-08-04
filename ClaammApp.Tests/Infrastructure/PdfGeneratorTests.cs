using ClaammApp.Domain.Entities;
using ClaammApp.Infrastructure.Pdf;

namespace ClaammApp.Tests.Infrastructure;

public class PdfGeneratorTests
{
    private static Presupuesto CrearPresupuesto()
    {
        return new Presupuesto
        {
            Id = 42,
            ClienteNombre = "Cliente de prueba",
            Fecha = new DateTime(2026, 8, 4),
            Items =
            {
                new PresupuestoItem { Descripcion = "Revoque fino", Unidad = "m2", Cantidad = 12.5m, PrecioUnitario = 8500m },
                new PresupuestoItem { Descripcion = "Pintura látex", Unidad = "m2", Cantidad = 50m, PrecioUnitario = 3200m }
            }
        };
    }

    private static ConfiguracionEmpresa CrearConfiguracion()
    {
        return new ConfiguracionEmpresa
        {
            RazonSocial = "CLAAMM Construcciones",
            Cuit = "30-12345678-9",
            Direccion = "Calle Falsa 123",
            Telefono = "011-5555-1234",
            Email = "contacto@claamm.com"
        };
    }

    [Fact]
    public void GenerarPresupuesto_DevuelveUnPdfValido()
    {
        var generador = new PdfGenerator();

        var bytes = generador.GenerarPresupuesto(CrearPresupuesto(), CrearConfiguracion());

        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 1000);
        Assert.Equal("%PDF", System.Text.Encoding.ASCII.GetString(bytes, 0, 4));
    }

    [Fact]
    public void GenerarPresupuesto_SinLogoNiDatos_NoFalla()
    {
        var generador = new PdfGenerator();
        var presupuesto = new Presupuesto
        {
            ClienteNombre = "Sin datos",
            Fecha = DateTime.Today,
            Items = { new PresupuestoItem { Descripcion = "Solo", Unidad = "un", Cantidad = 1, PrecioUnitario = 1 } }
        };
        var configuracion = new ConfiguracionEmpresa();

        var bytes = generador.GenerarPresupuesto(presupuesto, configuracion);

        Assert.StartsWith("%PDF", System.Text.Encoding.ASCII.GetString(bytes, 0, 4));
    }
}
