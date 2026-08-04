using ClaammApp.Domain.Entities;

namespace ClaammApp.Tests.Domain;

public class ItemTests
{
    [Fact]
    public void AplicarIncremento_10PorCiento_AumentaPrecio()
    {
        var item = new Item { PrecioUnitario = 100m };

        item.AplicarIncremento(10);

        Assert.Equal(110m, item.PrecioUnitario);
    }

    [Fact]
    public void AplicarIncremento_RedondeaADosDecimales()
    {
        var item = new Item { PrecioUnitario = 33.33m };

        item.AplicarIncremento(5);

        Assert.Equal(35.00m, item.PrecioUnitario);
    }

    [Fact]
    public void AplicarIncremento_CeroPorCiento_NoCambiaPrecio()
    {
        var item = new Item { PrecioUnitario = 50m };

        item.AplicarIncremento(0);

        Assert.Equal(50m, item.PrecioUnitario);
    }

    [Fact]
    public void AplicarIncremento_PorcentajeNegativo_ReducePrecio()
    {
        var item = new Item { PrecioUnitario = 100m };

        item.AplicarIncremento(-10);

        Assert.Equal(90m, item.PrecioUnitario);
    }
}
