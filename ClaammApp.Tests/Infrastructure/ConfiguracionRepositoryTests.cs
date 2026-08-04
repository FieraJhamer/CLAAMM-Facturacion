using ClaammApp.Infrastructure.Repositories;

namespace ClaammApp.Tests.Infrastructure;

public class ConfiguracionRepositoryTests : RepositorioTestBase
{
    private readonly ConfiguracionRepository _repositorio;

    public ConfiguracionRepositoryTests()
    {
        _repositorio = new ConfiguracionRepository(ConnectionString);
    }

    [Fact]
    public void GuardarObtener_RoundTrip()
    {
        _repositorio.Guardar("empresa.RazonSocial", "CLAAMM SA");

        Assert.Equal("CLAAMM SA", _repositorio.Obtener("empresa.RazonSocial"));
    }

    [Fact]
    public void Guardar_Duplicado_ActualizaValor()
    {
        _repositorio.Guardar("clave", "uno");
        _repositorio.Guardar("clave", "dos");

        Assert.Equal("dos", _repositorio.Obtener("clave"));
    }

    [Fact]
    public void Obtener_Inexistente_DevuelveValorPorDefecto()
    {
        Assert.Equal("", _repositorio.Obtener("noexiste"));
        Assert.Equal("default", _repositorio.Obtener("noexiste", "default"));
    }
}
