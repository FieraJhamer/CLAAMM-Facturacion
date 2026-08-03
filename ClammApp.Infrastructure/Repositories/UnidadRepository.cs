using ClammApp.Domain.Contracts;
using ClammApp.Domain.Entities;
using ClammApp.Infrastructure.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ClammApp.Infrastructure.Repositories;

public class UnidadRepository : IUnidadRepository
{
    public IEnumerable<UnidadMedida> ObtenerTodos()
    {
        using var c = new SqliteConnection(Database.ConnectionString);
        return c.Query<UnidadMedida>("SELECT Id, Nombre FROM Unidades ORDER BY Nombre COLLATE NOCASE");
    }

    public int Insertar(string nombre)
    {
        using var c = new SqliteConnection(Database.ConnectionString);
        c.Execute("INSERT OR IGNORE INTO Unidades (Nombre) VALUES (@nombre)", new { nombre });
        return c.ExecuteScalar<int>("SELECT last_insert_rowid()");
    }

    public void Actualizar(int id, string nombre)
    {
        using var c = new SqliteConnection(Database.ConnectionString);
        c.Execute("UPDATE Unidades SET Nombre = @nombre WHERE Id = @id", new { nombre, id });
    }

    public void Eliminar(int id)
    {
        using var c = new SqliteConnection(Database.ConnectionString);
        c.Execute("DELETE FROM Unidades WHERE Id = @id", new { id });
    }

    public void Asegurar(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return;

        using var c = new SqliteConnection(Database.ConnectionString);
        c.Execute("INSERT OR IGNORE INTO Unidades (Nombre) VALUES (@nombre)", new { nombre = nombre.Trim() });
    }
}
