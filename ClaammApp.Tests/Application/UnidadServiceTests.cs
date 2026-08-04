using ClaammApp.Application.Exceptions;
using ClaammApp.Application.Services;
using ClaammApp.Tests.Fakes;

namespace ClaammApp.Tests.Application;

public class UnidadServiceTests
{
    private readonly FakeUnidadRepository _repositorio = new();
    private readonly UnidadService _servicio;

    public UnidadServiceTests()
    {
        _servicio = new UnidadService(_repositorio);
    }

    [Fact]
    public void Agregar_NombreVacio_LanzaValidacion()
    {
        Assert.Throws<ValidacionException>(() => _servicio.Agregar(" "));
    }

    [Fact]
    public void Agregar_RecortaEspacios()
    {
        _servicio.Agregar("  ml  ");

        Assert.Equal("ml", Assert.Single(_servicio.ObtenerTodos()).Nombre);
    }

    [Fact]
    public void Agregar_DuplicadoIgnoraMayusculas_LanzaValidacion()
    {
        _servicio.Agregar("m2");

        Assert.Throws<ValidacionException>(() => _servicio.Agregar("M2"));
    }

    [Fact]
    public void Actualizar_DuplicadoEnOtroId_LanzaValidacion()
    {
        _servicio.Agregar("m2");
        _servicio.Agregar("ml");
        var ml = _servicio.ObtenerTodos().Single(u => u.Nombre == "ml");

        Assert.Throws<ValidacionException>(() => _servicio.Actualizar(ml.Id, "m2"));
    }

    [Fact]
    public void Actualizar_MismoNombreEnMismoId_NoLanza()
    {
        _servicio.Agregar("ml");
        var id = _servicio.ObtenerTodos().Single().Id;

        _servicio.Actualizar(id, "ML");

        Assert.Single(_servicio.ObtenerTodos());
    }

    [Fact]
    public void Eliminar_QuitaUnidad()
    {
        _servicio.Agregar("ml");
        var id = _servicio.ObtenerTodos().Single().Id;

        _servicio.Eliminar(id);

        Assert.Empty(_servicio.ObtenerTodos());
    }
}
