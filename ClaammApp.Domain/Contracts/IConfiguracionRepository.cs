namespace ClaammApp.Domain.Contracts;

public interface IConfiguracionRepository
{
    string Obtener(string clave, string valorPorDefecto = "");

    void Guardar(string clave, string valor);
}
