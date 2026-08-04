using ClaammApp.Domain.Contracts;

namespace ClaammApp.Tests.Fakes;

public class FakeConfiguracionRepository : IConfiguracionRepository
{
    private readonly Dictionary<string, string> _valores = new();

    public string Obtener(string clave, string valorPorDefecto = "")
        => _valores.TryGetValue(clave, out var valor) ? valor : valorPorDefecto;

    public void Guardar(string clave, string valor) => _valores[clave] = valor;
}
