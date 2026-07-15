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

namespace Sistema_de_Gestion_de_Activos.Services
{
	public class ReporteMantenimientoAnalisisService : IReporteMantenimientoAnalisisService
	{
		private readonly SgaDbContext _context;

		// RF-15: umbral configurable para identificar activos con costo de mantenimiento excesivo.
		private const string ClaveUmbral = "UmbralCostoMantenimientoExcesivo";
		private const decimal UmbralPorDefecto = 30m; // 30% del valor actual, según RF-29 del módulo de depreciación.

		public ReporteMantenimientoAnalisisService(SgaDbContext context)
		{
			_context = context;
		}

		// RF-13: Reporte de Mantenimientos
		public async Task<IEnumerable<ReporteMantenimientoItemDto>> GenerarReporteMantenimientosAsync(ReporteMantenimientoFiltroDto filtro)
		{
			var query = _context.Mantenimientos
				.Include(m => m.Activo).ThenInclude(a => a.Categoria)
				.Include(m => m.Activo).ThenInclude(a => a.Depreciacion)
				.Include(m => m.Tecnico)
				.Where(m => m.FechaInicio >= filtro.Desde && m.FechaInicio <= filtro.Hasta)
				.AsQueryable();

			if (filtro.CategoriaId.HasValue)
				query = query.Where(m => m.Activo.CategoriaId == filtro.CategoriaId.Value);

			if (filtro.TecnicoId.HasValue)
				query = query.Where(m => m.TecnicoId == filtro.TecnicoId.Value);

			var mantenimientos = await query.OrderByDescending(m => m.FechaInicio).ToListAsync();

			return mantenimientos.Select(m =>
			{
				var valorActual = m.Activo.Depreciacion?.ValorActual ?? m.Activo.CostoInicial;
				var baseComparacion = valorActual > 0 ? valorActual : m.Activo.CostoInicial;
				if (baseComparacion <= 0) baseComparacion = 1m;

				return new ReporteMantenimientoItemDto
				{
					MantenimientoId = m.Id,
					CodigoActivo = m.Activo.Codigo,
					DescripcionActivo = m.Activo.Descripcion,
					Categoria = m.Activo.Categoria.Nombre,
					Tipo = m.Tipo.ToString(),
					Tecnico = m.Tecnico.Nombre,
					FechaInicio = m.FechaInicio,
					FechaFin = m.FechaFin,
					Costo = m.Costo,
					Estado = m.Estado.ToString(),
					ValorActualActivo = valorActual,
					RelacionCostoValor = Math.Round((m.Costo / baseComparacion) * 100m, 2)
				};
			}).ToList();
		}

		// RF-13: exportación a CSV (compatible con Excel)
		public async Task<byte[]> ExportarMantenimientosCsvAsync(ReporteMantenimientoFiltroDto filtro)
		{
			var reporte = await GenerarReporteMantenimientosAsync(filtro);

			using var memoryStream = new MemoryStream();
			using var streamWriter = new StreamWriter(memoryStream, new System.Text.UTF8Encoding(true));
			using var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

			var registros = reporte.Select(r => new
			{
				r.CodigoActivo,
				r.DescripcionActivo,
				r.Categoria,
				r.Tipo,
				r.Tecnico,
				FechaInicio = r.FechaInicio.ToString("yyyy-MM-dd"),
				FechaFin = r.FechaFin?.ToString("yyyy-MM-dd") ?? "",
				r.Costo,
				r.Estado,
				r.ValorActualActivo,
				r.RelacionCostoValor
			});

			await csv.WriteRecordsAsync(registros);
			await streamWriter.FlushAsync();
			return memoryStream.ToArray();
		}

		// RF-13: exportación a Excel real (.xlsx)
		public async Task<byte[]> ExportarMantenimientosExcelAsync(ReporteMantenimientoFiltroDto filtro)
		{
			var reporte = await GenerarReporteMantenimientosAsync(filtro);

			using var workbook = new XLWorkbook();
			var ws = workbook.Worksheets.Add("Mantenimientos");

			var headers = new[] { "Activo", "Descripción", "Categoría", "Tipo", "Técnico", "Fecha Inicio", "Fecha Fin", "Costo", "Estado", "Valor Actual", "% Costo/Valor" };
			for (int i = 0; i < headers.Length; i++)
			{
				ws.Cell(1, i + 1).Value = headers[i];
			}
			ws.Row(1).Style.Font.Bold = true;
			ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#E5E7EB");

			int fila = 2;
			foreach (var r in reporte)
			{
				ws.Cell(fila, 1).Value = r.CodigoActivo;
				ws.Cell(fila, 2).Value = r.DescripcionActivo;
				ws.Cell(fila, 3).Value = r.Categoria;
				ws.Cell(fila, 4).Value = r.Tipo;
				ws.Cell(fila, 5).Value = r.Tecnico;
				ws.Cell(fila, 6).Value = r.FechaInicio;
				if (r.FechaFin.HasValue) ws.Cell(fila, 7).Value = r.FechaFin.Value;
				ws.Cell(fila, 8).Value = r.Costo;
				ws.Cell(fila, 9).Value = r.Estado;
				ws.Cell(fila, 10).Value = r.ValorActualActivo;
				ws.Cell(fila, 11).Value = r.RelacionCostoValor;
				fila++;
			}

			ws.Columns().AdjustToContents();

			using var ms = new MemoryStream();
			workbook.SaveAs(ms);
			return ms.ToArray();
		}

		// RF-13: exportación a PDF
		public async Task<byte[]> ExportarMantenimientosPdfAsync(ReporteMantenimientoFiltroDto filtro)
		{
			var reporte = await GenerarReporteMantenimientosAsync(filtro);

			var documento = QuestPDF.Fluent.Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4.Landscape());
					page.Margin(25);
					page.DefaultTextStyle(x => x.FontSize(9));

					page.Header().Column(col =>
					{
						col.Item().Text("Reporte de Mantenimientos").FontSize(16).Bold();
						col.Item().Text($"Período: {filtro.Desde:dd/MM/yyyy} — {filtro.Hasta:dd/MM/yyyy}").FontSize(9);
					});

					page.Content().PaddingTop(10).Table(table =>
					{
						table.ColumnsDefinition(columns =>
						{
							columns.RelativeColumn(2);
							columns.RelativeColumn();
							columns.RelativeColumn(1.3f);
							columns.RelativeColumn();
							columns.RelativeColumn();
							columns.RelativeColumn();
							columns.RelativeColumn();
						});

						table.Header(header =>
						{
							header.Cell().Text("Activo").Bold();
							header.Cell().Text("Tipo").Bold();
							header.Cell().Text("Técnico").Bold();
							header.Cell().Text("Fecha").Bold();
							header.Cell().Text("Estado").Bold();
							header.Cell().Text("Costo").Bold();
							header.Cell().Text("Costo/Valor").Bold();
						});

						foreach (var r in reporte)
						{
							table.Cell().Text($"{r.CodigoActivo} - {r.DescripcionActivo}");
							table.Cell().Text(r.Tipo);
							table.Cell().Text(r.Tecnico);
							table.Cell().Text(r.FechaInicio.ToString("dd/MM/yyyy"));
							table.Cell().Text(r.Estado);
							table.Cell().Text(r.Costo.ToString("C"));
							table.Cell().Text(r.RelacionCostoValor.ToString("0.0") + "%");
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

		// RF-15: Análisis de Costos de Mantenimiento
		public async Task<AnalisisCostoMantenimientoResultDto> GenerarAnalisisCostosAsync(Guid? categoriaId, DateTime? desde, DateTime? hasta)
		{
			var query = _context.Mantenimientos
				.Include(m => m.Activo).ThenInclude(a => a.Categoria)
				.Include(m => m.Activo).ThenInclude(a => a.Depreciacion)
				.AsQueryable();

			if (categoriaId.HasValue)
				query = query.Where(m => m.Activo.CategoriaId == categoriaId.Value);

			if (desde.HasValue)
				query = query.Where(m => m.FechaInicio >= desde.Value);

			if (hasta.HasValue)
				query = query.Where(m => m.FechaInicio <= hasta.Value);

			var mantenimientos = await query.ToListAsync();
			var umbral = await ObtenerUmbralAsync();

			var porActivo = mantenimientos
				.GroupBy(m => m.Activo)
				.Select(g =>
				{
					var activo = g.Key;
					var costoTotal = g.Sum(m => m.Costo);
					var valorActual = activo.Depreciacion?.ValorActual ?? activo.CostoInicial;
					var baseComparacion = valorActual > 0 ? valorActual : activo.CostoInicial;
					if (baseComparacion <= 0) baseComparacion = 1m;
					var relacion = Math.Round((costoTotal / baseComparacion) * 100m, 2);

					return new AnalisisCostoActivoDto
					{
						ActivoId = activo.Id,
						Codigo = activo.Codigo,
						Descripcion = activo.Descripcion,
						Categoria = activo.Categoria.Nombre,
						CantidadMantenimientos = g.Count(),
						CostoTotalMantenimiento = costoTotal,
						ValorActual = valorActual,
						RelacionCostoValor = relacion,
						CostoExcesivo = relacion > umbral
					};
				})
				.OrderByDescending(a => a.RelacionCostoValor)
				.ToList();

			var porCategoria = mantenimientos
				.GroupBy(m => m.Activo.Categoria)
				.Select(g => new AnalisisCostoCategoriaDto
				{
					CategoriaId = g.Key.Id,
					Categoria = g.Key.Nombre,
					CantidadActivos = g.Select(m => m.ActivoId).Distinct().Count(),
					CostoTotalMantenimiento = g.Sum(m => m.Costo),
					CostoPromedioPorActivo = Math.Round(
						g.Sum(m => m.Costo) / Math.Max(1, g.Select(m => m.ActivoId).Distinct().Count()), 2)
				})
				.OrderByDescending(c => c.CostoTotalMantenimiento)
				.ToList();

			return new AnalisisCostoMantenimientoResultDto
			{
				PorActivo = porActivo,
				PorCategoria = porCategoria,
				UmbralCostoExcesivo = umbral
			};
		}

		private async Task<decimal> ObtenerUmbralAsync()
		{
			var parametro = await _context.ParametrosSistema
				.FirstOrDefaultAsync(p => p.Clave == ClaveUmbral);

			if (parametro != null &&
				decimal.TryParse(parametro.Valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var valor))
			{
				return valor;
			}

			return UmbralPorDefecto;
		}
	}
}
