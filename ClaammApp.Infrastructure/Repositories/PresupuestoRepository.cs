using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;
using ClaammApp.Infrastructure.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ClaammApp.Infrastructure.Repositories;

public class PresupuestoRepository : IPresupuestoRepository
{
    private readonly string _connectionString;

    public PresupuestoRepository(string? connectionString = null)
    {
        _connectionString = connectionString ?? Database.ConnectionString;
    }

    private sealed class PresupuestoRow
    {
        public int Id { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public decimal Descuento { get; set; }
    }

    public IEnumerable<Presupuesto> ObtenerTodos()
    {
        using var c = new SqliteConnection(_connectionString);
        var filas = c.Query<PresupuestoRow>("SELECT Id, ClienteNombre, Fecha, Descuento FROM Presupuestos ORDER BY Fecha DESC, Id DESC").ToList();
        var todosLosItems = c.Query<PresupuestoItem>(
            "SELECT Id, PresupuestoId, ItemId, Descripcion, Unidad, Cantidad, PrecioUnitario, Total FROM PresupuestoItems").ToList();

        return filas.Select(f => new Presupuesto
        {
            Id = f.Id,
            ClienteNombre = f.ClienteNombre,
            Fecha = DateTime.Parse(f.Fecha),
            DescuentoPorcentaje = f.Descuento,
            Items = todosLosItems.Where(i => i.PresupuestoId == f.Id).ToList()
        }).ToList();
    }

    public Presupuesto? ObtenerPorId(int id)
    {
        using var c = new SqliteConnection(_connectionString);
        var fila = c.QueryFirstOrDefault<PresupuestoRow>(
            "SELECT Id, ClienteNombre, Fecha, Descuento FROM Presupuestos WHERE Id = @id", new { id });

        if (fila is null)
            return null;

        var items = c.Query<PresupuestoItem>(
            """
            SELECT Id, PresupuestoId, ItemId, Descripcion, Unidad, Cantidad, PrecioUnitario, Total
            FROM PresupuestoItems WHERE PresupuestoId = @id
            """, new { id }).ToList();

        return new Presupuesto
        {
            Id = fila.Id,
            ClienteNombre = fila.ClienteNombre,
            Fecha = DateTime.Parse(fila.Fecha),
            DescuentoPorcentaje = fila.Descuento,
            Items = items
        };
    }

    public int Insertar(Presupuesto presupuesto)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Open();
        using var tx = c.BeginTransaction();

        var id = c.ExecuteScalar<int>(
            "INSERT INTO Presupuestos (ClienteNombre, Fecha, Descuento) VALUES (@ClienteNombre, @Fecha, @Descuento); SELECT last_insert_rowid();",
            new { presupuesto.ClienteNombre, Fecha = presupuesto.Fecha.ToString("o"), Descuento = presupuesto.DescuentoPorcentaje }, tx);

        InsertarItems(c, id, presupuesto.Items, tx);

        tx.Commit();
        presupuesto.Id = id;
        return id;
    }

    public void Actualizar(Presupuesto presupuesto)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Open();
        using var tx = c.BeginTransaction();

        c.Execute(
            "UPDATE Presupuestos SET ClienteNombre = @ClienteNombre, Fecha = @Fecha, Descuento = @Descuento WHERE Id = @Id",
            new { presupuesto.ClienteNombre, Fecha = presupuesto.Fecha.ToString("o"), Descuento = presupuesto.DescuentoPorcentaje, presupuesto.Id }, tx);

        c.Execute("DELETE FROM PresupuestoItems WHERE PresupuestoId = @id", new { id = presupuesto.Id }, tx);
        InsertarItems(c, presupuesto.Id, presupuesto.Items, tx);

        tx.Commit();
    }

    public void Eliminar(int id)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Open();
        using var tx = c.BeginTransaction();

        c.Execute("DELETE FROM PresupuestoItems WHERE PresupuestoId = @id", new { id }, tx);
        c.Execute("DELETE FROM Presupuestos WHERE Id = @id", new { id }, tx);

        tx.Commit();
    }

    public void ReemplazarItems(int presupuestoId, IEnumerable<PresupuestoItem> items)
    {
        using var c = new SqliteConnection(_connectionString);
        c.Open();
        using var tx = c.BeginTransaction();

        c.Execute("DELETE FROM PresupuestoItems WHERE PresupuestoId = @id", new { id = presupuestoId }, tx);
        InsertarItems(c, presupuestoId, items, tx);

        tx.Commit();
    }

    private static void InsertarItems(SqliteConnection c, int presupuestoId, IEnumerable<PresupuestoItem> items, SqliteTransaction tx)
    {
        foreach (var i in items)
        {
            c.Execute(
                """
                INSERT INTO PresupuestoItems (PresupuestoId, ItemId, Descripcion, Unidad, Cantidad, PrecioUnitario, Total)
                VALUES (@PresupuestoId, @ItemId, @Descripcion, @Unidad, @Cantidad, @PrecioUnitario, @Total)
                """,
                new { PresupuestoId = presupuestoId, i.ItemId, i.Descripcion, i.Unidad, i.Cantidad, i.PrecioUnitario, Total = i.Total }, tx);
        }
    }
}
