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
            var logoBytes = File.Exists(configuracion.LogoRuta) ? File.ReadAllBytes(configuracion.LogoRuta) : null;

            col.Item().PaddingBottom(4).Row(row =>
            {
                if (logoBytes != null)
                    row.ConstantItem(150).Image(logoBytes).FitWidth();
                else
                    row.ConstantItem(150).Height(90).Element(LogoPlaceholder);
            });

            col.Item().Text(configuracion.RazonSocial).FontSize(14).Bold();

            col.Item().PaddingTop(2).Text(LineaDatos(configuracion)).FontSize(9).FontColor(Colors.Grey.Darken2);

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

            col.Item().PaddingTop(14).AlignRight().Text(t =>
            {
                t.Span("TOTAL:  ").FontSize(14).Bold();
                t.Span(FormatoMoneda(presupuesto.Total)).FontSize(14).Bold().FontColor(Dorado);
            });

            col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            col.Item().PaddingTop(2).Text("Observaciones:").FontSize(8).Bold().FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(2).Text("• Las formas de pagos son pactados al comenzar la obra.").FontSize(8).FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(2).Text("• No incluye traslados de resagos").FontSize(8).FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(2).Text("• Las modificaciones de trabajos serán relevadas con precios alternativos").FontSize(8).FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(2).Text("• Con factura A o B se increneta 21% mas a la factura").FontSize(8).FontColor(Colors.Grey.Darken2);
            col.Item().PaddingTop(2).Text("• Ver los itens correspondientes para la aprobacion").FontSize(8).FontColor(Colors.Grey.Darken2);
        });
    }

    private static void LogoPlaceholder(IContainer container)
    {
        container.Background(Grafito)
            .AlignMiddle().AlignCenter().Text("CLAAMM").FontColor(Colors.White).Bold().FontSize(16);
    }

    private static string LineaDatos(ConfiguracionEmpresa c)
    {
        var partes = new List<string>();
        if (!string.IsNullOrWhiteSpace(c.Cuit)) partes.Add("CUIT: " + c.Cuit);
        if (!string.IsNullOrWhiteSpace(c.Direccion)) partes.Add(c.Direccion);
        if (!string.IsNullOrWhiteSpace(c.Telefono)) partes.Add("Tel: " + c.Telefono);
        if (!string.IsNullOrWhiteSpace(c.Email)) partes.Add(c.Email);
        return string.Join("  |  ", partes);
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
