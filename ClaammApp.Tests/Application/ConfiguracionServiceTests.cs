using ClaammApp.Application.Services;
using ClaammApp.Domain.Entities;
using ClaammApp.Tests.Fakes;

namespace ClaammApp.Tests.Application;

public class ConfiguracionServiceTests
{
    [Fact]
    public void ObtenerEmpresa_Vacia_DevuelveValoresVacios()
    {
        var servicio = new ConfiguracionService(new FakeConfiguracionRepository());

        var empresa = servicio.ObtenerEmpresa();

        Assert.Equal(string.Empty, empresa.Responsable);
        Assert.Equal(string.Empty, empresa.Cuit);
        Assert.Equal(string.Empty, empresa.Ubicacion);
        Assert.Equal(string.Empty, empresa.Email);
    }

    [Fact]
    public void GuardarYObtener_RoundTripConPrefijo()
    {
        var servicio = new ConfiguracionService(new FakeConfiguracionRepository());
        var empresa = new ConfiguracionEmpresa
        {
            Responsable = "Juan Perez",
            Cuit = "30-12345678-9",
            Direccion = "Calle 1",
            Ubicacion = "CABA",
            Telefono = "123",
            Email = "a@b.com"
        };

        servicio.GuardarEmpresa(empresa);
        var obtenida = servicio.ObtenerEmpresa();

        Assert.Equal("Juan Perez", obtenida.Responsable);
        Assert.Equal("30-12345678-9", obtenida.Cuit);
        Assert.Equal("CABA", obtenida.Ubicacion);
        Assert.Equal("a@b.com", obtenida.Email);
    }
}
