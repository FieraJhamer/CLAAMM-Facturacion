namespace ClaammApp.Tests.Infrastructure;

public abstract class RepositorioTestBase : IDisposable
{
    protected readonly string Ruta;
    protected readonly string ConnectionString;

    protected RepositorioTestBase()
    {
        Ruta = TestDatabase.CrearRuta();
        ConnectionString = $"Data Source={Ruta}";
    }

    public void Dispose()
    {
        try { File.Delete(Ruta); } catch { }
        try { Directory.Delete(Path.GetDirectoryName(Ruta)!, true); } catch { }
    }
}
