using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;
using ClaammApp.Infrastructure.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ClaammApp.Infrastructure.Repositories;

public class UnidadRepository : IUnidadRepository
{
    private readonly string _connectionString;

    public UnidadRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? Database.ConnectionString;
    }

    public IEnumerable<UnidadMedida> ObtenerTodos()
    {
        using var c = new SqliteConnection(_connectionString);
        return c.Query<UnidadMedida>("SELECT Id, Nombre FROM Unidades ORDER BY Nombre COLLATE NOCASE");
    }

    public int Insertar(string nombre)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Execute("INSERT OR IGNORE INTO Unidades (Nombre) VALUES (@nombre)", new { nombre });
        return c.ExecuteScalar<int>("SELECT Id FROM Unidades WHERE Nombre = @nombre", new { nombre });
    }

    public void Actualizar(int id, string nombre)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Execute("UPDATE Unidades SET Nombre = @nombre WHERE Id = @id", new { nombre, id });
    }

    public void Eliminar(int id)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Execute("DELETE FROM Unidades WHERE Id = @id", new { id });
    }

    public void Asegurar(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return;

        using var c = new SqliteConnection(_connectionString);
        c.Execute("INSERT OR IGNORE INTO Unidades (Nombre) VALUES (@nombre)", new { nombre = nombre.Trim() });
    }
}
