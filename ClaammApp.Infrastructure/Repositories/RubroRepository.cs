using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;
using ClaammApp.Infrastructure.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ClaammApp.Infrastructure.Repositories;

public class RubroRepository : IRubroRepository
{
    private readonly string _connectionString;

    public RubroRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? Database.ConnectionString;
    }

    public IEnumerable<Rubro> ObtenerTodos()
    {
        using var c = new SqliteConnection(_connectionString);
        return c.Query<Rubro>("SELECT Id, Nombre FROM Rubros ORDER BY Nombre COLLATE NOCASE");
    }

    public int Insertar(string nombre)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Execute("INSERT INTO Rubros (Nombre) VALUES (@nombre)", new { nombre });
        return c.ExecuteScalar<int>("SELECT last_insert_rowid()");
    }

    public void Actualizar(int id, string nombre)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Execute("UPDATE Rubros SET Nombre = @nombre WHERE Id = @id", new { nombre, id });
    }

    public void Eliminar(int id)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Execute("DELETE FROM Rubros WHERE Id = @id", new { id });
    }
}
