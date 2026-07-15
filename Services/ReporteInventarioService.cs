using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Services
{
	public class ReporteInventarioService : IReporteInventarioService
	{
		private readonly SgaDbContext _context;

		public ReporteInventarioService(SgaDbContext context)
		{
			_context = context;
		}

		// RF-12: Reporte de Inventario
		public async Task<IEnumerable<ReporteInventarioItemDto>> GenerarReporteInventarioAsync(ReporteInventarioFiltroDto filtro)
		{
			var activos = await ObtenerActivosFiltradosAsync(filtro);

			return activos.Select(a => new ReporteInventarioItemDto
			{
				Id = a.Id,
				Codigo = a.Codigo,
				Descripcion = a.Descripcion,
				Categoria = a.Categoria.Nombre,
				Estado = a.Estado.ToString(),
				Ubicacion = FormatearUbicacion(a.Ubicacion),
				Responsable = "Sin asignar",
				FechaAdquisicion = a.FechaAdquisicion,
				Valor = a.CostoInicial,
				ValorActual = filtro.IncluirDepreciacion ? (a.Depreciacion?.ValorActual ?? a.CostoInicial) : null,
				PorcentajeConsumido = filtro.IncluirDepreciacion ? (a.Depreciacion?.PorcentajeConsumido ?? 0m) : null
			}).ToList();
		}

		// RF-12: exportación a CSV (compatible con Excel)
		public async Task<byte[]> ExportarInventarioCsvAsync(ReporteInventarioFiltroDto filtro)
		{
			var reporte = await GenerarReporteInventarioAsync(filtro);

			using var memoryStream = new MemoryStream();
			using var streamWriter = new StreamWriter(memoryStream, new System.Text.UTF8Encoding(true)); // UTF8 con BOM
			using var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

			if (filtro.IncluirDepreciacion)
			{
				var registros = reporte.Select(r => new
				{
					r.Codigo,
					r.Descripcion,
					r.Categoria,
					r.Estado,
					r.Ubicacion,
					r.Responsable,
					FechaAdquisicion = r.FechaAdquisicion.ToString("yyyy-MM-dd"),
					Valor = r.Valor,
					ValorActual = r.ValorActual,
					PorcentajeConsumido = r.PorcentajeConsumido
				});
				await csv.WriteRecordsAsync(registros);
			}
			else
			{
				var registros = reporte.Select(r => new
				{
					r.Codigo,
					r.Descripcion,
					r.Categoria,
					r.Estado,
					r.Ubicacion,
					r.Responsable,
					FechaAdquisicion = r.FechaAdquisicion.ToString("yyyy-MM-dd"),
					Valor = r.Valor
				});
				await csv.WriteRecordsAsync(registros);
			}

			await streamWriter.FlushAsync();
			return memoryStream.ToArray();
		}

		// RF-12: exportación a Excel real (.xlsx)
		public async Task<byte[]> ExportarInventarioExcelAsync(ReporteInventarioFiltroDto filtro)
		{
			var reporte = await GenerarReporteInventarioAsync(filtro);

			using var workbook = new XLWorkbook();
			var ws = workbook.Worksheets.Add("Inventario");

			var headers = new List<string> { "Código", "Descripción", "Categoría", "Estado", "Ubicación", "Responsable", "Valor" };
			if (filtro.IncluirDepreciacion)
			{
				headers.Add("Valor Actual");
				headers.Add("% Consumido");
			}

			for (int i = 0; i < headers.Count; i++)
			{
				ws.Cell(1, i + 1).Value = headers[i];
			}
			ws.Row(1).Style.Font.Bold = true;
			ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#E5E7EB");

			int fila = 2;
			foreach (var r in reporte)
			{
				int col = 1;
				ws.Cell(fila, col++).Value = r.Codigo;
				ws.Cell(fila, col++).Value = r.Descripcion;
				ws.Cell(fila, col++).Value = r.Categoria;
				ws.Cell(fila, col++).Value = r.Estado;
				ws.Cell(fila, col++).Value = r.Ubicacion;
				ws.Cell(fila, col++).Value = r.Responsable;
				ws.Cell(fila, col++).Value = r.Valor;

				if (filtro.IncluirDepreciacion)
				{
					ws.Cell(fila, col++).Value = r.ValorActual ?? 0;
					ws.Cell(fila, col++).Value = r.PorcentajeConsumido ?? 0;
				}
				fila++;
			}

			ws.Columns().AdjustToContents();

			using var ms = new MemoryStream();
			workbook.SaveAs(ms);
			return ms.ToArray();
		}

		// RF-12: exportación a PDF
		public async Task<byte[]> ExportarInventarioPdfAsync(ReporteInventarioFiltroDto filtro)
		{
			var reporte = await GenerarReporteInventarioAsync(filtro);
			var incluirDep = filtro.IncluirDepreciacion;

			var documento = QuestPDF.Fluent.Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4.Landscape());
					page.Margin(25);
					page.DefaultTextStyle(x => x.FontSize(9));

					page.Header().Text("Reporte de Inventario").FontSize(16).Bold();

					page.Content().PaddingTop(10).Table(table =>
					{
						table.ColumnsDefinition(columns =>
						{
							columns.RelativeColumn();
							columns.RelativeColumn(2);
							columns.RelativeColumn();
							columns.RelativeColumn();
							columns.RelativeColumn(1.5f);
							columns.RelativeColumn();
							if (incluirDep)
							{
								columns.RelativeColumn();
								columns.RelativeColumn();
							}
						});

						table.Header(header =>
						{
							header.Cell().Text("Código").Bold();
							header.Cell().Text("Descripción").Bold();
							header.Cell().Text("Categoría").Bold();
							header.Cell().Text("Estado").Bold();
							header.Cell().Text("Ubicación").Bold();
							header.Cell().Text("Valor").Bold();
							if (incluirDep)
							{
								header.Cell().Text("Valor Actual").Bold();
								header.Cell().Text("% Consumido").Bold();
							}
						});

						foreach (var r in reporte)
						{
							table.Cell().Text(r.Codigo);
							table.Cell().Text(r.Descripcion);
							table.Cell().Text(r.Categoria);
							table.Cell().Text(r.Estado);
							table.Cell().Text(r.Ubicacion);
							table.Cell().Text(r.Valor.ToString("C"));
							if (incluirDep)
							{
								table.Cell().Text(r.ValorActual?.ToString("C") ?? "-");
								table.Cell().Text(r.PorcentajeConsumido?.ToString("0.0") + "%");
							}
						}
					});

					page.Footer().AlignCenter().Text(x =>
					{
						x.Span("Generado el ");
						x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
					});
				});
			});

			return documento.GeneratePdf();
		}

		// RF-14: Reporte de Activos por Ubicación
		public async Task<IEnumerable<ReporteUbicacionDto>> GenerarReportePorUbicacionAsync()
		{
			var activos = await _context.Activos
				.Include(a => a.Ubicacion)
				.Where(a => a.Estado != EstadoActivo.Baja && a.Estado != EstadoActivo.Descartado)
				.ToListAsync();

			var agrupado = activos
				.GroupBy(a => a.Ubicacion)
				.Select(g => new ReporteUbicacionDto
				{
					UbicacionId = g.Key.Id,
					Edificio = g.Key.Edificio,
					Departamento = g.Key.Departamento,
					Oficina = g.Key.Oficina,
					CantidadActivos = g.Count(),
					ValorTotal = g.Sum(a => a.CostoInicial),
					Activos = g.Select(a => new ActivoResumenDto
					{
						Id = a.Id,
						Codigo = a.Codigo,
						Descripcion = a.Descripcion,
						Estado = a.Estado.ToString()
					}).ToList()
				})
				.OrderBy(r => r.Departamento)
				.ThenBy(r => r.Oficina)
				.ToList();

			return agrupado;
		}

		private async Task<List<Domain.Entities.Activo>> ObtenerActivosFiltradosAsync(ReporteInventarioFiltroDto filtro)
		{
			var query = _context.Activos
				.Include(a => a.Categoria)
				.Include(a => a.Ubicacion)
				.Include(a => a.Depreciacion)
				.AsQueryable();

			if (filtro.CategoriaId.HasValue)
				query = query.Where(a => a.CategoriaId == filtro.CategoriaId.Value);

			if (filtro.UbicacionId.HasValue)
				query = query.Where(a => a.UbicacionId == filtro.UbicacionId.Value);

			if (!string.IsNullOrEmpty(filtro.Estado) && Enum.TryParse<EstadoActivo>(filtro.Estado, true, out var estado))
				query = query.Where(a => a.Estado == estado);

			if (filtro.FechaDesde.HasValue)
				query = query.Where(a => a.FechaAdquisicion >= filtro.FechaDesde.Value);

			if (filtro.FechaHasta.HasValue)
				query = query.Where(a => a.FechaAdquisicion <= filtro.FechaHasta.Value);

			return await query.OrderBy(a => a.Codigo).ToListAsync();
		}

		private static string FormatearUbicacion(Domain.Entities.Ubicacion ubicacion)
		{
			var partes = new List<string>();
			if (!string.IsNullOrWhiteSpace(ubicacion.Departamento)) partes.Add(ubicacion.Departamento);
			if (!string.IsNullOrWhiteSpace(ubicacion.Oficina)) partes.Add(ubicacion.Oficina);
			if (!string.IsNullOrWhiteSpace(ubicacion.Edificio)) partes.Add(ubicacion.Edificio);
			return partes.Count > 0 ? string.Join(" - ", partes) : "Sin ubicación";
		}
	}
}
