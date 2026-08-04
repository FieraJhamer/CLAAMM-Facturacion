using ClaammApp.Application.Exceptions;
using ClaammApp.Application.Services;
using ClaammApp.Tests.Fakes;

namespace ClaammApp.Tests.Application;

public class RubroServiceTests
{
    private readonly FakeRubroRepository _repositorio = new();
    private readonly RubroService _servicio;

    public RubroServiceTests()
    {
        _servicio = new RubroService(_repositorio);
    }

    [Fact]
    public void Agregar_NombreVacio_LanzaValidacion()
    {
        Assert.Throws<ValidacionException>(() => _servicio.Agregar("   "));
    }

    [Fact]
    public void Agregar_RecortaEspacios()
    {
        _servicio.Agregar("  Estructura  ");

        Assert.Equal("Estructura", Assert.Single(_servicio.ObtenerTodos()).Nombre);
    }

    [Fact]
    public void Agregar_DuplicadoIgnoraMayusculas_LanzaValidacion()
    {
        _servicio.Agregar("Estructura");

        Assert.Throws<ValidacionException>(() => _servicio.Agregar("estructura"));
    }

    [Fact]
    public void Actualizar_NombreVacio_LanzaValidacion()
    {
        Assert.Throws<ValidacionException>(() => _servicio.Actualizar(1, ""));
    }

    [Fact]
    public void Actualizar_DuplicadoEnOtroId_LanzaValidacion()
    {
        _servicio.Agregar("Estructura");
        _servicio.Agregar("Terminaciones");
        var terminaciones = _servicio.ObtenerTodos().Single(r => r.Nombre == "Terminaciones");

        Assert.Throws<ValidacionException>(() => _servicio.Actualizar(terminaciones.Id, "estructura"));
    }

    [Fact]
    public void Actualizar_MismoNombreEnMismoId_NoLanza()
    {
        _servicio.Agregar("Estructura");
        var id = _servicio.ObtenerTodos().Single().Id;

        _servicio.Actualizar(id, "ESTRUCTURA");

        Assert.Single(_servicio.ObtenerTodos());
    }

    [Fact]
    public void Eliminar_QuitaRubro()
    {
        _servicio.Agregar("Estructura");
        var id = _servicio.ObtenerTodos().Single().Id;

        _servicio.Eliminar(id);

        Assert.Empty(_servicio.ObtenerTodos());
    }
}
