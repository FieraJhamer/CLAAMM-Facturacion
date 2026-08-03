using System.Globalization;

namespace ClammApp.Application;

public static class Formatos
{
    private static readonly CultureInfo CulturaAr = CultureInfo.GetCultureInfo("es-AR");

    public static string Moneda(decimal valor) => valor.ToString("C2", CulturaAr);

    public static string Cantidad(decimal valor) => valor.ToString("#,##0.###", CulturaAr);

    public static string FechaCorta(DateTime fecha) => fecha.ToString("dd/MM/yyyy");
}
