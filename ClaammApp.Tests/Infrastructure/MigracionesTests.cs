using ClaammApp.Infrastructure.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ClaammApp.Tests.Infrastructure;

public class MigracionesTests
{
    private static string NuevaRuta()
    {
        var directorio = Path.Combine(Path.GetTempPath(), "ClaammAppTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directorio);
        return Path.Combine(directorio, "test.db");
    }

    private static void CrearItemsManualmente(string ruta, params (string Codigo, string Descripcion)[] items)
    {
        using var c = new SqliteConnection($"Data Source={ruta}");
        c.Execute(
            """
            CREATE TABLE Items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Codigo TEXT NOT NULL,
                Descripcion TEXT NOT NULL,
                Unidad TEXT NOT NULL,
                PrecioUnitario REAL NOT NULL,
                Rubro TEXT NOT NULL
            );
            """);
        foreach (var (codigo, descripcion) in items)
            c.Execute("INSERT INTO Items (Codigo, Descripcion, Unidad, PrecioUnitario, Rubro) VALUES (@codigo, @descripcion, 'un', 1, '')",
                new { codigo, descripcion });
    }

    [Fact]
    public void Inicializar_CreaTodasLasTablas()
    {
        var ruta = NuevaRuta();
        try
        {
            Database.Inicializar(ruta, migrarBaseAntigua: false);

            using var c = new SqliteConnection($"Data Source={ruta}");
            var tablas = c.Query<string>("SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name").ToList();

            Assert.Contains("Items", tablas);
            Assert.Contains("Rubros", tablas);
            Assert.Contains("Unidades", tablas);
            Assert.Contains("Presupuestos", tablas);
            Assert.Contains("PresupuestoItems", tablas);
            Assert.Contains("Configuracion", tablas);
        }
        finally
        {
            Limpiar(ruta);
        }
    }

    [Fact]
    public void Inicializar_SiembraUnidadesPredeterminadas()
    {
        var ruta = NuevaRuta();
        try
        {
            Database.Inicializar(ruta, migrarBaseAntigua: false);

            using var c = new SqliteConnection($"Data Source={ruta}");
            var nombres = c.Query<string>("SELECT Nombre FROM Unidades ORDER BY Nombre").ToList();

            Assert.Contains("m2", nombres);
            Assert.Contains("m3", nombres);
            Assert.Contains("un", nombres);
            Assert.Contains("grl", nombres);
        }
        finally
        {
            Limpiar(ruta);
        }
    }

    [Fact]
    public void MigrarCodigos_ItemsConCodigoNoNumerico_SeRenumeran()
    {
        var ruta = NuevaRuta();
        try
        {
            CrearItemsManualmente(ruta, ("ABC", "Primero"), ("XYZ", "Segundo"));

            Database.Inicializar(ruta, migrarBaseAntigua: false);

            using var c = new SqliteConnection($"Data Source={ruta}");
            var codigos = c.Query<string>("SELECT Codigo FROM Items ORDER BY Id").ToList();
            var proximo = c.ExecuteScalar<string>("SELECT Valor FROM Configuracion WHERE Clave = 'ProximoCodigoItem'");

            Assert.Equal(new[] { "0000001", "0000002" }, codigos);
            Assert.Equal("3", proximo);
        }
        finally
        {
            Limpiar(ruta);
        }
    }

    [Fact]
    public void MigrarCodigos_ItemsConCodigoNumerico_NoSeRenumeran()
    {
        var ruta = NuevaRuta();
        try
        {
            CrearItemsManualmente(ruta, ("0000005", "Quinto"), ("0000007", "Septimo"));

            Database.Inicializar(ruta, migrarBaseAntigua: false);

            using var c = new SqliteConnection($"Data Source={ruta}");
            var codigos = c.Query<string>("SELECT Codigo FROM Items ORDER BY Id").ToList();

            Assert.Equal(new[] { "0000005", "0000007" }, codigos);
        }
        finally
        {
            Limpiar(ruta);
        }
    }

    [Fact]
    public void MigrarCodigos_ItemsVacios_NoFalla()
    {
        var ruta = NuevaRuta();
        try
        {
            CrearItemsManualmente(ruta);

            Database.Inicializar(ruta, migrarBaseAntigua: false);

            using var c = new SqliteConnection($"Data Source={ruta}");
            var cantidad = c.ExecuteScalar<long>("SELECT COUNT(*) FROM Items");

            Assert.Equal(0, cantidad);
        }
        finally
        {
            Limpiar(ruta);
        }
    }

    private static void Limpiar(string ruta)
    {
        try { File.Delete(ruta); } catch { }
        try { Directory.Delete(Path.GetDirectoryName(ruta)!, true); } catch { }
    }
}
