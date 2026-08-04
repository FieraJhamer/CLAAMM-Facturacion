using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;

namespace ClaammApp.Tests.Fakes;

public class FakePdfGenerator : IPdfGenerator
{
    public byte[] Bytes { get; set; } = new byte[] { 37, 80, 68, 70 };

    public Presupuesto? UltimoPresupuesto { get; private set; }

    public byte[] GenerarPresupuesto(Presupuesto presupuesto, ConfiguracionEmpresa configuracion)
    {
        UltimoPresupuesto = presupuesto;
        return Bytes;
    }
}
