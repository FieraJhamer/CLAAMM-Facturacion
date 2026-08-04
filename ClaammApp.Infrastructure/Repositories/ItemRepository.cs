using System.Globalization;
using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;
using ClaammApp.Infrastructure.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ClaammApp.Infrastructure.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly string _connectionString;

    public ItemRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? Database.ConnectionString;
    }

    public IEnumerable<Item> GetAll()
    {
        using var c = new SqliteConnection(_connectionString);
        return c.Query<Item>(
            "SELECT Id, Codigo, Descripcion, Unidad, PrecioUnitario, Rubro FROM Items ORDER BY Descripcion COLLATE NOCASE");
    }

    public IEnumerable<Item> Buscar(string texto)
    {
        using var c = new SqliteConnection(_connectionString);
        var items = c.Query<Item>(
            "SELECT Id, Codigo, Descripcion, Unidad, PrecioUnitario, Rubro FROM Items").ToList();

        var normalizado = Normalizar(texto);
        return items
            .Where(i => Normalizar(i.Descripcion).Contains(normalizado)
                     || Normalizar(i.Rubro).Contains(normalizado)
                     || Normalizar(i.Codigo).Contains(normalizado))
            .OrderBy(i => i.Descripcion)
            .ToList();
    }

    public Item? ObtenerPorId(int id)
    {
        using var c = new SqliteConnection(_connectionString);
        return c.QueryFirstOrDefault<Item>(
            "SELECT Id, Codigo, Descripcion, Unidad, PrecioUnitario, Rubro FROM Items WHERE Id = @id",
            new { id });
    }

    public int Insertar(Item item)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Execute(
            """
            INSERT INTO Items (Codigo, Descripcion, Unidad, PrecioUnitario, Rubro)
            VALUES (@Codigo, @Descripcion, @Unidad, @PrecioUnitario, @Rubro)
            """,
            new { item.Codigo, item.Descripcion, item.Unidad, item.PrecioUnitario, item.Rubro });
        return c.ExecuteScalar<int>("SELECT last_insert_rowid()");
    }

    public void Actualizar(Item item)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Execute(
            """
            UPDATE Items
            SET Descripcion = @Descripcion, Unidad = @Unidad, PrecioUnitario = @PrecioUnitario, Rubro = @Rubro
            WHERE Id = @Id
            """,
            new { item.Descripcion, item.Unidad, item.PrecioUnitario, item.Rubro, item.Id });
    }

    public void Eliminar(int id)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Execute("DELETE FROM Items WHERE Id = @id", new { id });
    }

    public string ObtenerProximoCodigo()
    {
        using var c = new SqliteConnection(_connectionString);
        var n = c.ExecuteScalar<int?>(
            "SELECT CAST(Valor AS INTEGER) FROM Configuracion WHERE Clave = 'ProximoCodigoItem'") ?? 1;

        c.Execute(
            """
            INSERT INTO Configuracion (Clave, Valor) VALUES ('ProximoCodigoItem', @v)
            ON CONFLICT(Clave) DO UPDATE SET Valor = @v
            """,
            new { v = (n + 1).ToString() });

        return $"{n:D7}";
    }

    private static string Normalizar(string texto)
    {
        var sinDiacriticos = new string(texto
            .Normalize(System.Text.NormalizationForm.FormD)
            .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            .ToArray());
        return sinDiacriticos.ToLowerInvariant();
    }
}
