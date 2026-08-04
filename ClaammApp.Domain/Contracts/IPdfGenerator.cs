using ClaammApp.Domain.Entities;

namespace ClaammApp.Domain.Contracts;

public interface IPdfGenerator
{
    byte[] GenerarPresupuesto(Presupuesto presupuesto, ConfiguracionEmpresa configuracion);
}
