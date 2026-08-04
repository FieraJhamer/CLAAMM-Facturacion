using ClaammApp.Domain.Entities;

namespace ClaammApp.Tests.Domain;

public class PresupuestoTests
{
    [Fact]
    public void PresupuestoItem_Total_EsCantidadPorPrecio()
    {
        var item = new PresupuestoItem { Cantidad = 2.5m, PrecioUnitario = 100m };

        Assert.Equal(250m, item.Total);
    }

    [Fact]
    public void Presupuesto_Total_EsSumaDeItems()
    {
        var presupuesto = new Presupuesto
        {
            Items =
            {
                new PresupuestoItem { Cantidad = 2, PrecioUnitario = 100m },
                new PresupuestoItem { Cantidad = 3, PrecioUnitario = 50m }
            }
        };

        Assert.Equal(350m, presupuesto.Total);
    }

    [Fact]
    public void Presupuesto_Total_SinItems_EsCero()
    {
        var presupuesto = new Presupuesto();

        Assert.Equal(0m, presupuesto.Total);
    }
}
