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

        Assert.Equal(string.Empty, empresa.RazonSocial);
        Assert.Equal(string.Empty, empresa.Cuit);
        Assert.Equal(string.Empty, empresa.LogoRuta);
    }

    [Fact]
    public void GuardarYObtener_RoundTripConPrefijo()
    {
        var servicio = new ConfiguracionService(new FakeConfiguracionRepository());
        var empresa = new ConfiguracionEmpresa
        {
            RazonSocial = "CLAAMM SA",
            Cuit = "30-12345678-9",
            Direccion = "Calle 1",
            Telefono = "123",
            Email = "a@b.com",
            LogoRuta = @"C:\logo.png"
        };

        servicio.GuardarEmpresa(empresa);
        var obtenida = servicio.ObtenerEmpresa();

        Assert.Equal("CLAAMM SA", obtenida.RazonSocial);
        Assert.Equal("30-12345678-9", obtenida.Cuit);
        Assert.Equal(@"C:\logo.png", obtenida.LogoRuta);
    }
}
