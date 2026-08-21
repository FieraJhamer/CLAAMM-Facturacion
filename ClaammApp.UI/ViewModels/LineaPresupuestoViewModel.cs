using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using ClaammApp.Application;
using ClaammApp.Domain.Entities;

namespace ClaammApp.UI.ViewModels;

public class LineaPresupuestoViewModel : INotifyPropertyChanged
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-AR");

    private readonly PresupuestoItem _item;

    public LineaPresupuestoViewModel(PresupuestoItem item)
    {
        _item = item;
    }

    public PresupuestoItem Item => _item;

    public string Descripcion => _item.Descripcion;

    public string Unidad => _item.Unidad;

    public decimal Cantidad
    {
        get => _item.Cantidad;
        set
        {
            _item.Cantidad = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalTexto));
        }
    }

    public string CantidadTexto
    {
        get => _item.Cantidad.ToString("#,##0.###", Cultura);
        set
        {
            if (!decimal.TryParse(value, NumberStyles.Number, Cultura, out var n))
                return;
            _item.Cantidad = n;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalTexto));
        }
    }

    public string PrecioUnitarioTexto => Formatos.Moneda(_item.PrecioUnitario);

    public string TotalTexto => Formatos.Moneda(_item.Total);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? nombre = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
}
