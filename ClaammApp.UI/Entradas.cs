using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClaammApp.UI;

public static class Entradas
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-AR");

    public static void SoloDecimal(object remitente, TextCompositionEventArgs e)
    {
        if (remitente is not TextBox caja)
        {
            e.Handled = true;
            return;
        }

        foreach (var caracter in e.Text)
        {
            if (char.IsDigit(caracter))
                continue;

            if (caracter is ',' or '.')
            {
                var textoBase = caja.Text.Remove(caja.SelectionStart, caja.SelectionLength);
                var separador = textoBase.IndexOfAny([',', '.']);
                if (separador >= 0)
                {
                    e.Handled = true;
                    return;
                }
                continue;
            }

            e.Handled = true;
            return;
        }
    }

    public static bool TryDecimal(string? texto, out decimal valor)
    {
        valor = 0m;
        if (string.IsNullOrWhiteSpace(texto))
            return false;

        var normalizado = texto.Trim().Replace(" ", string.Empty);

        if (normalizado.Contains(',') && normalizado.Contains('.'))
            normalizado = normalizado.Replace(".", string.Empty);
        else if (normalizado.Count(c => c == '.') > 1)
            normalizado = normalizado.Replace(".", string.Empty);
        else if (normalizado.Contains('.'))
        {
            var parteDecimal = normalizado.Split('.')[^1];
            normalizado = parteDecimal.Length == 3
                ? normalizado.Replace(".", string.Empty)
                : normalizado.Replace('.', ',');
        }

        return decimal.TryParse(normalizado, NumberStyles.Number, Cultura, out valor);
    }

    public static decimal Redondear(decimal valor, int decimales)
        => Math.Round(valor, decimales, MidpointRounding.AwayFromZero);

    public static decimal Limitar(decimal valor, decimal minimo, decimal maximo)
        => Math.Clamp(valor, minimo, maximo);
}
