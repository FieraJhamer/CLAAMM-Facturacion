using Dapper;
using Microsoft.Data.Sqlite;

namespace ClammApp.Infrastructure.Data;

public static class Database
{
    public static string ConnectionString => $"Data Source={AppPaths.RutaBaseDatos}";

    public static void Inicializar()
    {
        AppPaths.AsegurarDirectorio();

        using var conexion = new SqliteConnection(ConnectionString);
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
                Fecha TEXT NOT NULL
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
}
