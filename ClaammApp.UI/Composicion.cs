using ClaammApp.Application.Services;
using ClaammApp.Infrastructure.Pdf;
using ClaammApp.Infrastructure.Repositories;

namespace ClaammApp.UI;

public static class Composicion
{
    public static ItemService Items { get; } = new(new ItemRepository(), new UnidadRepository());

    public static RubroService Rubros { get; } = new(new RubroRepository());

    public static UnidadService Unidades { get; } = new(new UnidadRepository());

    public static PresupuestoService Presupuestos { get; } = new(new PresupuestoRepository(), new PdfGenerator());

    public static ConfiguracionService Configuracion { get; } = new(new ConfiguracionRepository());
}
