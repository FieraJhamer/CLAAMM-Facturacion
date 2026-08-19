using System.Globalization;
using ClaammApp.Domain.Contracts;
using ClaammApp.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClaammApp.Infrastructure.Pdf;

public class PdfGenerator : IPdfGenerator
{
    private static readonly Color Dorado = Color.FromHex("#C8A13E");
    private static readonly Color Grafito = Color.FromHex("#2E3238");

    static PdfGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerarPresupuesto(Presupuesto presupuesto, ConfiguracionEmpresa configuracion)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(t => t.FontSize(10).FontColor(Colors.Grey.Darken4));

                page.Header().Element(c => ComposeHeader(c, presupuesto, configuracion));
                page.Content().Element(c => ComposeContent(c, presupuesto));
                page.Footer().AlignCenter().Text(t =>
                {
                    t.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Medium));
                    t.Span("CLAAMM  |  ");
                    t.CurrentPageNumber();
                    t.Span(" de ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, Presupuesto presupuesto, ConfiguracionEmpresa configuracion)
    {
        container.Column(col =>
        {
            var logoRuta = Path.Combine(AppContext.BaseDirectory, "logo.png");
            var logoBytes = File.Exists(logoRuta) ? File.ReadAllBytes(logoRuta) : null;

            col.Item().PaddingBottom(4).Row(row =>
            {
                if (logoBytes != null)
                    row.ConstantItem(170).Image(logoBytes).FitWidth();
                else
                    row.ConstantItem(170).Height(90).Element(LogoPlaceholder);
            });

            col.Item().Text("Servicios y Construcciones").FontSize(14).Bold();

            col.Item().PaddingTop(2).Row(row =>
            {
                row.RelativeItem().Column(izq => IzquierdaDatos(izq, configuracion));
                row.RelativeItem().Column(der => DerechaDatos(der, configuracion));
            });

            col.Item().PaddingTop(18).LineHorizontal(2).LineColor(Dorado);

            col.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text(t =>
                {
                    t.Span("PRESUPUESTO").FontSize(15).Bold().FontColor(Dorado);
                    t.Span($"   Nº {presupuesto.Id:D6}").FontSize(11);
                });
                row.ConstantItem(180).AlignRight().Text("Fecha: " + presupuesto.Fecha.ToString("dd/MM/yyyy")).FontSize(10);
            });

            col.Item().PaddingTop(6).Text($"Cliente:  {presupuesto.ClienteNombre}").FontSize(12).Bold();

            col.Item().PaddingTop(12).Text("DETALLE").FontSize(10).Bold().FontColor(Colors.Grey.Darken2);
        });
    }

    private static void ComposeContent(IContainer container, Presupuesto presupuesto)
    {
        container.Column(col =>
        {
            col.Item().Table(tabla =>
            {
                tabla.ColumnsDefinition(cd =>
                {
                    cd.RelativeColumn(4);
                    cd.ConstantColumn(45);
                    cd.ConstantColumn(70);
                    cd.ConstantColumn(90);
                    cd.ConstantColumn(90);
                });

                tabla.Header(h =>
                {
                    h.Cell().Background(Grafito).Padding(6).Text("Descripción").FontColor(Colors.White).Bold().FontSize(9);
                    h.Cell().Background(Grafito).Padding(6).Text("Und.").FontColor(Colors.White).Bold().FontSize(9);
                    h.Cell().Background(Grafito).Padding(6).AlignRight().Text("Cantidad").FontColor(Colors.White).Bold().FontSize(9);
                    h.Cell().Background(Grafito).Padding(6).AlignRight().Text("Precio Unitario").FontColor(Colors.White).Bold().FontSize(9);
                    h.Cell().Background(Grafito).Padding(6).AlignRight().Text("Total").FontColor(Colors.White).Bold().FontSize(9);
                });

                for (var i = 0; i < presupuesto.Items.Count; i++)
                {
                    var item = presupuesto.Items[i];
                    var fondo = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten3;

                    tabla.Cell().Background(fondo).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(5).PaddingHorizontal(4).Text(item.Descripcion).FontSize(9);
                    tabla.Cell().Background(fondo).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(5).PaddingHorizontal(4).Text(item.Unidad).FontSize(9);
                    tabla.Cell().Background(fondo).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text(FormatoCantidad(item.Cantidad)).FontSize(9);
                    tabla.Cell().Background(fondo).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text(FormatoMoneda(item.PrecioUnitario)).FontSize(9);
                    tabla.Cell().Background(fondo).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(5).PaddingHorizontal(4).AlignRight().Text(FormatoMoneda(item.Total)).FontSize(9);
                }
            });

            col.Item().PaddingTop(11).AlignRight().Text("Total:  " + FormatoMoneda(presupuesto.Total)).FontSize(11).Bold();

            if (presupuesto.DescuentoPorcentaje > 0)
            {
                col.Item().PaddingTop(4).AlignRight().Text(
                    $"Descuento {presupuesto.DescuentoPorcentaje:0.##}%:  " +
                    FormatoMoneda(presupuesto.TotalDescuento)).FontSize(11).FontColor(Colors.Grey.Darken2);
            }

            col.Item().PaddingTop(4).AlignRight().Text(
                "Impuesto 21%:  " + FormatoMoneda(presupuesto.TotalImpuesto)).FontSize(11).FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(4).AlignRight().Text(
                "Importe neto total:  " + FormatoMoneda(presupuesto.TotalNeto)).FontSize(14).Bold().FontColor(Dorado);

            col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            col.Item().PaddingTop(6).Text("Observaciones:").FontSize(9).Bold().FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(2).Text("- Las formas de pagos son pactados al comenzar la obra.").FontSize(9).FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(2).Text("- No incluye traslados de resagos").FontSize(9).FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(2).Text("- Las modificaciones de trabajos serán relevadas con precios alternativos").FontSize(9).FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(2).Text("- Con factura A o B se increneta 21% mas a la factura").FontSize(9).FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(2).Text("- Ver los items correspondientes para la aprobacion").FontSize(9).FontColor(Colors.Grey.Darken2);
        });
    }

    private static void LogoPlaceholder(IContainer container)
    {
        container.Background(Grafito)
            .AlignMiddle().AlignCenter().Text("CLAAMM").FontColor(Colors.White).Bold().FontSize(16);
    }

    private static void IzquierdaDatos(ColumnDescriptor col, ConfiguracionEmpresa c)
    {
        var estilo = new TextStyle().FontSize(9).FontColor(Colors.Grey.Darken2);
        if (!string.IsNullOrWhiteSpace(c.Responsable))
            col.Item().Text("Responsable " + c.Responsable).Style(estilo);
        if (!string.IsNullOrWhiteSpace(c.Cuit))
            col.Item().Text("CUIT: " + c.Cuit).Style(estilo);
        if (!string.IsNullOrWhiteSpace(c.Email))
            col.Item().Text("Correo: " + c.Email).Style(estilo);
    }

    private static void DerechaDatos(ColumnDescriptor col, ConfiguracionEmpresa c)
    {
        var estilo = new TextStyle().FontSize(9).FontColor(Colors.Grey.Darken2);
        if (!string.IsNullOrWhiteSpace(c.Direccion))
            col.Item().Text("Dirección: " + c.Direccion).Style(estilo);
        if (!string.IsNullOrWhiteSpace(c.Ubicacion))
            col.Item().Text("Ubicación: " + c.Ubicacion).Style(estilo);
        if (!string.IsNullOrWhiteSpace(c.Telefono))
            col.Item().Text("Teléfono: " + c.Telefono).Style(estilo);
    }

    private static string FormatoMoneda(decimal valor)
    {
        return valor.ToString("C2", CultureInfo.GetCultureInfo("es-AR"));
    }

    private static string FormatoCantidad(decimal valor)
    {
        return valor.ToString("#,##0.###", CultureInfo.InvariantCulture);
    }
}
