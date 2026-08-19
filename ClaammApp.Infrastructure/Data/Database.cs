using Dapper;
using Microsoft.Data.Sqlite;

namespace ClaammApp.Infrastructure.Data;

public static class Database
{
    public static string ConnectionString => $"Data Source={AppPaths.RutaBaseDatos}";

    public static void Inicializar() => Inicializar(AppPaths.RutaBaseDatos);

    public static void Inicializar(string rutaBaseDatos, bool migrarBaseAntigua = true)
    {
        var directorio = Path.GetDirectoryName(rutaBaseDatos);
        if (!string.IsNullOrEmpty(directorio))
            Directory.CreateDirectory(directorio);

        if (migrarBaseAntigua)
            MigrarBaseDeDatos(rutaBaseDatos);

        MigrarCodigosItems(rutaBaseDatos);
        AgregarColumnaDescuento(rutaBaseDatos);
        CrearEsquema(rutaBaseDatos);
    }

    private static void AgregarColumnaDescuento(string rutaBaseDatos)
    {
        using var conexion = new SqliteConnection($"Data Source={rutaBaseDatos}");
        var tablas = conexion.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name='Presupuestos'").ToList();
        if (tablas.Count == 0)
            return;
        var columnas = conexion.Query<string>(
            "SELECT name FROM pragma_table_info('Presupuestos')").ToList();
        if (!columnas.Contains("Descuento", StringComparer.OrdinalIgnoreCase))
            conexion.Execute("ALTER TABLE Presupuestos ADD COLUMN Descuento REAL NOT NULL DEFAULT 0");
    }

    private static void CrearEsquema(string rutaBaseDatos)
    {
        using var conexion = new SqliteConnection($"Data Source={rutaBaseDatos}");
        conexion.Execute(
            """
            CREATE TABLE IF NOT EXISTS Items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Codigo TEXT NOT NULL,
                Descripcion TEXT NOT NULL,
                Unidad TEXT NOT NULL,
                PrecioUnitario REAL NOT NULL,
                Rubro TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Rubros (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS Unidades (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL UNIQUE
            );

            INSERT OR IGNORE INTO Unidades (Nombre) VALUES ('m2'), ('m3'), ('un'), ('grl');

            INSERT OR IGNORE INTO Unidades (Nombre)
            SELECT DISTINCT Unidad FROM Items WHERE TRIM(Unidad) <> '';

            CREATE TABLE IF NOT EXISTS Presupuestos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ClienteNombre TEXT NOT NULL,
                Fecha TEXT NOT NULL,
                Descuento REAL NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS PresupuestoItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PresupuestoId INTEGER NOT NULL,
                ItemId INTEGER NOT NULL,
                Descripcion TEXT NOT NULL,
                Unidad TEXT NOT NULL,
                Cantidad REAL NOT NULL,
                PrecioUnitario REAL NOT NULL,
                Total REAL NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Configuracion (
                Clave TEXT PRIMARY KEY,
                Valor TEXT NOT NULL
            );
            """);
    }

    private static void MigrarBaseDeDatos(string rutaNueva)
    {
        _ = rutaNueva;
    }

    private static void MigrarCodigosItems(string rutaBaseDatos)
    {
        using var conexion = new SqliteConnection($"Data Source={rutaBaseDatos}");
        conexion.Execute(
            """
            CREATE TABLE IF NOT EXISTS Items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Codigo TEXT NOT NULL,
                Descripcion TEXT NOT NULL,
                Unidad TEXT NOT NULL,
                PrecioUnitario REAL NOT NULL,
                Rubro TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Configuracion (
                Clave TEXT PRIMARY KEY,
                Valor TEXT NOT NULL
            );
            """);

        var items = conexion.Query("SELECT Id, Codigo FROM Items ORDER BY Id").ToList();
        if (items.Count == 0)
            return;

        var yaNumericos = items.All(i => ((string)i.Codigo).Length > 0 && ((string)i.Codigo).All(char.IsDigit));
        if (yaNumericos)
            return;

        for (var n = 1; n <= items.Count; n++)
        {
            var codigo = n.ToString("D7");
            var id = (long)items[n - 1].Id;
            conexion.Execute("UPDATE Items SET Codigo = @codigo WHERE Id = @id", new { codigo, id });
        }

        conexion.Execute(
            """
            INSERT INTO Configuracion (Clave, Valor) VALUES ('ProximoCodigoItem', @v)
            ON CONFLICT(Clave) DO UPDATE SET Valor = @v
            """,
            new { v = (items.Count + 1).ToString() });
    }
}
