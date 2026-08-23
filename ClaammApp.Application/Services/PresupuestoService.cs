using ClaammApp.Application.Exceptions;
using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;

namespace ClaammApp.Application.Services;

public class PresupuestoService
{
    private readonly IPresupuestoRepository _repositorio;
    private readonly IPdfGenerator _pdfGenerator;

    public PresupuestoService(IPresupuestoRepository repositorio, IPdfGenerator pdfGenerator)
    {
        _repositorio = repositorio;
        _pdfGenerator = pdfGenerator;
    }

    public IEnumerable<Presupuesto> ObtenerTodos() => _repositorio.ObtenerTodos();

    public Presupuesto? Obtener(int id) => _repositorio.ObtenerPorId(id);

    public Presupuesto CrearNuevo() => new() { Fecha = DateTime.Today };

    public void Guardar(Presupuesto presupuesto)
    {
        if (string.IsNullOrWhiteSpace(presupuesto.ClienteNombre))
            throw new ValidacionException("El nombre del cliente es obligatorio.");
        if (presupuesto.DescuentoPorcentaje < 0 || presupuesto.DescuentoPorcentaje > 100)
            throw new ValidacionException("El descuento debe estar entre 0 y 100.");
        if (presupuesto.Items.Count == 0)
            throw new ValidacionException("El presupuesto debe tener al menos un ítem.");
        if (presupuesto.Items.Any(i => i.Cantidad <= 0))
            throw new ValidacionException("Las cantidades deben ser mayores a cero.");

        if (presupuesto.Id == 0)
            _repositorio.Insertar(presupuesto);
        else
            _repositorio.Actualizar(presupuesto);
    }

    public void Eliminar(int id) => _repositorio.Eliminar(id);

    public void AgregarItem(Presupuesto presupuesto, Item item, decimal cantidad)
    {
        presupuesto.Items.Add(new PresupuestoItem
        {
            ItemId = item.Id,
            Descripcion = item.Descripcion,
            Unidad = item.Unidad,
            Cantidad = cantidad,
            PrecioUnitario = item.PrecioUnitario
        });
    }

    public void ExportarPdf(Presupuesto presupuesto, ConfiguracionEmpresa configuracion, string rutaDestino)
    {
        var bytes = _pdfGenerator.GenerarPresupuesto(presupuesto, configuracion);
        File.WriteAllBytes(rutaDestino, bytes);
    }
}
