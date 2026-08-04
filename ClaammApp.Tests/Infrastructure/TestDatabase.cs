using ClaammApp.Infrastructure.Data;

namespace ClaammApp.Tests.Infrastructure;

public static class TestDatabase
{
    public static string CrearRuta()
    {
        var directorio = Path.Combine(Path.GetTempPath(), "ClaammAppTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directorio);
        var ruta = Path.Combine(directorio, "test.db");
        Database.Inicializar(ruta, migrarBaseAntigua: false);
        return ruta;
    }
}
