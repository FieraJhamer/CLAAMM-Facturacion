using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;

namespace ClaammApp.Application.Services;

public class ConfiguracionService
{
    private const string Prefijo = "empresa.";
    private const string ClaveResponsable = Prefijo + "Responsable";
    private const string ClaveCuit = Prefijo + "Cuit";
    private const string ClaveDireccion = Prefijo + "Direccion";
    private const string ClaveUbicacion = Prefijo + "Ubicacion";
    private const string ClaveTelefono = Prefijo + "Telefono";
    private const string ClaveEmail = Prefijo + "Email";

    private readonly IConfiguracionRepository _repositorio;

    public ConfiguracionService(IConfiguracionRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public ConfiguracionEmpresa ObtenerEmpresa()
    {
        return new ConfiguracionEmpresa
        {
            Responsable = _repositorio.Obtener(ClaveResponsable),
            Cuit = _repositorio.Obtener(ClaveCuit),
            Direccion = _repositorio.Obtener(ClaveDireccion),
            Ubicacion = _repositorio.Obtener(ClaveUbicacion),
            Telefono = _repositorio.Obtener(ClaveTelefono),
            Email = _repositorio.Obtener(ClaveEmail),
        };
    }

    public void GuardarEmpresa(ConfiguracionEmpresa empresa)
    {
        _repositorio.Guardar(ClaveResponsable, empresa.Responsable ?? string.Empty);
        _repositorio.Guardar(ClaveCuit, empresa.Cuit ?? string.Empty);
        _repositorio.Guardar(ClaveDireccion, empresa.Direccion ?? string.Empty);
        _repositorio.Guardar(ClaveUbicacion, empresa.Ubicacion ?? string.Empty);
        _repositorio.Guardar(ClaveTelefono, empresa.Telefono ?? string.Empty);
        _repositorio.Guardar(ClaveEmail, empresa.Email ?? string.Empty);
    }
}
