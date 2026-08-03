using ClammApp.Domain.Contracts;
using ClammApp.Domain.Entities;

namespace ClammApp.Application.Services;

public class ConfiguracionService
{
    private const string Prefijo = "empresa.";
    private const string ClaveRazonSocial = Prefijo + "RazonSocial";
    private const string ClaveCuit = Prefijo + "Cuit";
    private const string ClaveDireccion = Prefijo + "Direccion";
    private const string ClaveTelefono = Prefijo + "Telefono";
    private const string ClaveEmail = Prefijo + "Email";
    private const string ClaveLogoRuta = Prefijo + "LogoRuta";

    private readonly IConfiguracionRepository _repositorio;

    public ConfiguracionService(IConfiguracionRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public ConfiguracionEmpresa ObtenerEmpresa()
    {
        return new ConfiguracionEmpresa
        {
            RazonSocial = _repositorio.Obtener(ClaveRazonSocial),
            Cuit = _repositorio.Obtener(ClaveCuit),
            Direccion = _repositorio.Obtener(ClaveDireccion),
            Telefono = _repositorio.Obtener(ClaveTelefono),
            Email = _repositorio.Obtener(ClaveEmail),
            LogoRuta = _repositorio.Obtener(ClaveLogoRuta)
        };
    }

    public void GuardarEmpresa(ConfiguracionEmpresa empresa)
    {
        _repositorio.Guardar(ClaveRazonSocial, empresa.RazonSocial ?? string.Empty);
        _repositorio.Guardar(ClaveCuit, empresa.Cuit ?? string.Empty);
        _repositorio.Guardar(ClaveDireccion, empresa.Direccion ?? string.Empty);
        _repositorio.Guardar(ClaveTelefono, empresa.Telefono ?? string.Empty);
        _repositorio.Guardar(ClaveEmail, empresa.Email ?? string.Empty);
        _repositorio.Guardar(ClaveLogoRuta, empresa.LogoRuta ?? string.Empty);
    }
}
