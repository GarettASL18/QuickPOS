using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuickPOS.Models;

namespace QuickPOS.Services
{
    public class PdfService
    {
        public PdfService()
        {
            // Configuración obligatoria para la versión gratuita (Community)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void ExportarFactura(Factura factura, List<FacturaDetalle> detalles, string nombreEmpresa, string rutaArchivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // --- ENCABEZADO ---
                    page.Header().Row(row =>
                    {
                        // Lado Izquierdo: Datos Empresa
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(nombreEmpresa).SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                            col.Item().Text($"Fecha: {factura.Fecha:dd/MM/yyyy HH:mm}");
                            col.Item().Text($"Factura #: {factura.FacturaId}");
                        });

                        // Lado Derecho: Datos Cliente
                        row.ConstantItem(150).Column(col =>
                        {
                            col.Item().Text("CLIENTE").SemiBold();
                            col.Item().Text(factura.NombreCliente);
                        });
                    });

                    // --- CONTENIDO (TABLA) ---
                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        // Definir columnas
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Producto (más ancho)
                            columns.RelativeColumn();  // Precio
                            columns.RelativeColumn();  // Cantidad
                            columns.RelativeColumn();  // Total
                        });

                        // Encabezados de tabla
                        table.Header(header =>
                        {
                            // CORRECCIÓN: SemiBold() va aplicado al Texto, no al Elemento contenedor
                            header.Cell().Element(EstiloCeldaHeader).Text("Producto").SemiBold();
                            header.Cell().Element(EstiloCeldaHeader).AlignRight().Text("Precio").SemiBold();
                            header.Cell().Element(EstiloCeldaHeader).AlignCenter().Text("Cant").SemiBold();
                            header.Cell().Element(EstiloCeldaHeader).AlignRight().Text("Total").SemiBold();
                        });

                        // Filas de productos
                        foreach (var item in detalles)
                        {
                            table.Cell().Element(EstiloCelda).Text(item.NombreProducto);
                            table.Cell().Element(EstiloCelda).AlignRight().Text($"{item.PrecioUnitario:C2}");
                            table.Cell().Element(EstiloCelda).AlignCenter().Text(item.Cantidad.ToString());
                            table.Cell().Element(EstiloCelda).AlignRight().Text($"{item.TotalLinea:C2}");
                        }

                        // --- DEFINICIÓN DE ESTILOS LOCALES ---
                        // Aquí definimos SOLO bordes y padding, nada de fuentes
                        static IContainer EstiloCeldaHeader(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }

                        static IContainer EstiloCelda(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten4).PaddingVertical(5);
                        }
                    });

                    // --- PIE DE PÁGINA (TOTALES) ---
                    page.Footer().AlignRight().Column(col =>
                    {
                        col.Item().Text($"Subtotal: {factura.Subtotal:C2}");
                        col.Item().Text($"Impuesto: {factura.Impuesto:C2}");

                        // Total Grande y Verde
                        col.Item().Text(txt =>
                        {
                            txt.Span("TOTAL: ").SemiBold().FontSize(14);
                            txt.Span($"{factura.Total:C2}").SemiBold().FontSize(14).FontColor(Colors.Green.Medium);
                        });
                    });
                });
            })
            .GeneratePdf(rutaArchivo);
        }
    }
}