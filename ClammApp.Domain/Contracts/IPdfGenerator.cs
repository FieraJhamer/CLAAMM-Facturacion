using ClammApp.Domain.Entities;

namespace ClammApp.Domain.Contracts;

public interface IPdfGenerator
{
    byte[] GenerarPresupuesto(Presupuesto presupuesto, ConfiguracionEmpresa configuracion);
}
