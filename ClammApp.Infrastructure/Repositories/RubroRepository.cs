using ClammApp.Domain.Contracts;
using ClammApp.Domain.Entities;
using ClammApp.Infrastructure.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ClammApp.Infrastructure.Repositories;

public class RubroRepository : IRubroRepository
{
    public IEnumerable<Rubro> ObtenerTodos()
    {
        using var c = new SqliteConnection(Database.ConnectionString);
        return c.Query<Rubro>("SELECT Id, Nombre FROM Rubros ORDER BY Nombre COLLATE NOCASE");
    }

    public int Insertar(string nombre)
    {
        using var c = new SqliteConnection(Database.ConnectionString);
        c.Execute("INSERT INTO Rubros (Nombre) VALUES (@nombre)", new { nombre });
        return c.ExecuteScalar<int>("SELECT last_insert_rowid()");
    }

    public void Actualizar(int id, string nombre)
    {
        using var c = new SqliteConnection(Database.ConnectionString);
        c.Execute("UPDATE Rubros SET Nombre = @nombre WHERE Id = @id", new { nombre, id });
    }

    public void Eliminar(int id)
    {
        using var c = new SqliteConnection(Database.ConnectionString);
        c.Execute("DELETE FROM Rubros WHERE Id = @id", new { id });
    }
}
