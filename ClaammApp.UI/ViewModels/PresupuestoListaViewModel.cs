using ClaammApp.Application;
using ClaammApp.Domain.Entities;

namespace ClaammApp.UI.ViewModels;

public class PresupuestoListaViewModel
{
    public PresupuestoListaViewModel(Presupuesto presupuesto)
    {
        Presupuesto = presupuesto;
    }

    public Presupuesto Presupuesto { get; }

    public string Numero => Presupuesto.Id.ToString("D6");

    public string Cliente => Presupuesto.ClienteNombre;

    public string FechaTexto => Formatos.FechaCorta(Presupuesto.Fecha);

    public string TotalTexto => Formatos.Moneda(Presupuesto.Total);
}
