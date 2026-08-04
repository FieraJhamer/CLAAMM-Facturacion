using ClaammApp.Domain.Contracts;
using ClaammApp.Infrastructure.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ClaammApp.Infrastructure.Repositories;

public class ConfiguracionRepository : IConfiguracionRepository
{
    private readonly string _connectionString;

    public ConfiguracionRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? Database.ConnectionString;
    }

    public string Obtener(string clave, string valorPorDefecto = "")
    {
        using var c = new SqliteConnection(_connectionString);
        var valor = c.ExecuteScalar<string?>("SELECT Valor FROM Configuracion WHERE Clave = @clave", new { clave });
        return string.IsNullOrEmpty(valor) ? valorPorDefecto : valor;
    }

    public void Guardar(string clave, string valor)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Execute(
            """
            INSERT INTO Configuracion (Clave, Valor) VALUES (@clave, @valor)
            ON CONFLICT(Clave) DO UPDATE SET Valor = @valor
            """,
            new { clave, valor });
    }
}
