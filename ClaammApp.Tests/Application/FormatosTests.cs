using ClaammApp.Application;

namespace ClaammApp.Tests.Application;

public class FormatosTests
{
    [Fact]
    public void Moneda_FormatoArgentina()
    {
        Assert.Equal("$ 1.234,50", Formatos.Moneda(1234.5m));
    }

    [Fact]
    public void Moneda_Cero()
    {
        Assert.Equal("$ 0,00", Formatos.Moneda(0m));
    }

    [Fact]
    public void Cantidad_DecimalesConComa()
    {
        Assert.Equal("1.234,567", Formatos.Cantidad(1234.567m));
    }

    [Fact]
    public void Cantidad_EnteroConMillar()
    {
        Assert.Equal("1.000", Formatos.Cantidad(1000m));
    }

    [Fact]
    public void FechaCorta_FormatoDiaMesAnio()
    {
        Assert.Equal("04/08/2026", Formatos.FechaCorta(new DateTime(2026, 8, 4)));
    }
}
