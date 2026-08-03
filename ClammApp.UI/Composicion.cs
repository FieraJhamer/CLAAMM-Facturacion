using ClammApp.Application.Services;
using ClammApp.Infrastructure.Pdf;
using ClammApp.Infrastructure.Repositories;

namespace ClammApp.UI;

public static class Composicion
{
    public static ItemService Items { get; } = new(new ItemRepository());

    public static RubroService Rubros { get; } = new(new RubroRepository());

    public static PresupuestoService Presupuestos { get; } = new(new PresupuestoRepository(), new PdfGenerator());

    public static ConfiguracionService Configuracion { get; } = new(new ConfiguracionRepository());
}
