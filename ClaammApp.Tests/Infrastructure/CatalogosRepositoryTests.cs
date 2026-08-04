using ClaammApp.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;

namespace ClaammApp.Tests.Infrastructure;

public class CatalogosRepositoryTests : RepositorioTestBase
{
    private readonly RubroRepository _rubros;
    private readonly UnidadRepository _unidades;

    public CatalogosRepositoryTests()
    {
        _rubros = new RubroRepository(ConnectionString);
        _unidades = new UnidadRepository(ConnectionString);
    }

    [Fact]
    public void Rubro_CrudCompleto()
    {
        var id = _rubros.Insertar("Estructura");

        Assert.True(id > 0);
        Assert.Equal("Estructura", _rubros.ObtenerTodos().Single().Nombre);

        _rubros.Actualizar(id, "Estructura y base");

        Assert.Equal("Estructura y base", _rubros.ObtenerTodos().Single().Nombre);

        _rubros.Eliminar(id);

        Assert.Empty(_rubros.ObtenerTodos());
    }

    [Fact]
    public void Rubro_Duplicado_LanzaSqliteException()
    {
        _rubros.Insertar("Estructura");

        Assert.Throws<SqliteException>(() => _rubros.Insertar("Estructura"));
    }

    [Fact]
    public void Unidad_InsertarNueva_DevuelveIdValido()
    {
        var id = _unidades.Insertar("ml");

        Assert.True(id > 0);
        Assert.Contains(_unidades.ObtenerTodos(), u => u.Nombre == "ml");
    }

    [Fact]
    public void Unidad_InsertarExistente_DevuelveIdDeLaExistente()
    {
        var idOriginal = _unidades.Insertar("ml");
        var idRepetida = _unidades.Insertar("ml");

        Assert.Equal(idOriginal, idRepetida);
        Assert.Single(_unidades.ObtenerTodos(), u => u.Nombre == "ml");
    }

    [Fact]
    public void Unidad_InsertarUnidadPrecargada_DevuelveIdDeLaPrecargada()
    {
        var id = _unidades.Insertar("m2");

        Assert.True(id > 0);
        Assert.Single(_unidades.ObtenerTodos(), u => u.Nombre == "m2");
    }

    [Fact]
    public void Unidad_Asegurar_NoDuplica()
    {
        _unidades.Asegurar("ml");
        _unidades.Asegurar(" ml ");

        Assert.Single(_unidades.ObtenerTodos(), u => u.Nombre == "ml");
    }

    [Fact]
    public void Unidad_Asegurar_VacioNoAgrega()
    {
        _unidades.Asegurar("   ");

        Assert.DoesNotContain(_unidades.ObtenerTodos(), u => string.IsNullOrWhiteSpace(u.Nombre));
    }

    [Fact]
    public void Unidad_ActualizarYEliminar()
    {
        var id = _unidades.Insertar("ml");

        _unidades.Actualizar(id, "mlineal");
        Assert.Contains(_unidades.ObtenerTodos(), u => u.Nombre == "mlineal");

        _unidades.Eliminar(id);
        Assert.DoesNotContain(_unidades.ObtenerTodos(), u => u.Nombre == "mlineal");
    }
}
