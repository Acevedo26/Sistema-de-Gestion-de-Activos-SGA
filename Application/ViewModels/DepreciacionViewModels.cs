using System;
using System.Collections.Generic;

namespace Sistema_de_Gestion_de_Activos.Application.ViewModels
{
    public class CategoriaConfigViewModel
    {
        public List<CategoriaItemViewModel> Categorias { get; set; } = new();
    }

    public class CategoriaItemViewModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int VidaUtil { get; set; }
    }

    public class ActivoDetalleViewModel
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime FechaAdquisicion { get; set; }
        public decimal CostoInicial { get; set; }
        public DepreciacionDetalleViewModel Depreciacion { get; set; } = new();
        public List<HistorialDepreciacionViewModel> Historial { get; set; } = new();
        public AnalisisMantenimientoViewModel AnalisisMantenimiento { get; set; } = new();
        public List<AuditoriaItemViewModel> Auditoria { get; set; } = new();
    }

    public class DepreciacionDetalleViewModel
    {
        public decimal ValorActual { get; set; }
        public decimal ValorResidual { get; set; }
        public decimal DepreciacionAcumulada { get; set; }
        public decimal PorcentajeConsumido { get; set; }
        public int VidaUtilAsignada { get; set; }
        public int AntiguedadAnios { get; set; }
        public int AntiguedadMeses { get; set; }
        public DateTime FechaUltimoCalculo { get; set; }
    }

    public class HistorialDepreciacionViewModel
    {
        public DateTime FechaConsulta { get; set; }
        public decimal ValorActual { get; set; }
        public decimal ValorResidual { get; set; }
        public decimal PorcentajeConsumido { get; set; }
        public decimal DepreciacionAcumulada { get; set; }
    }

    public class AnalisisMantenimientoViewModel
    {
        public decimal CostoMantenimientoAcumulado { get; set; }
        public decimal RelacionCostoValor { get; set; }
        public decimal UmbralAlerta { get; set; }
    }

    public class AuditoriaItemViewModel
    {
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Cambios { get; set; } = string.Empty;
    }

    public class ReporteDepreciacionViewModel
    {
        public List<ReporteDepreciacionItemViewModel> Items { get; set; } = new();
        public ReporteDepreciacionFiltroViewModel Filtro { get; set; } = new();
    }

    public class ReporteDepreciacionItemViewModel
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal CostoOriginal { get; set; }
        public decimal ValorActual { get; set; }
        public decimal ValorResidual { get; set; }
        public decimal DepreciacionAcumulada { get; set; }
        public decimal PorcentajeConsumido { get; set; }
        public decimal AniosVidaUtilRestantes { get; set; }
        public DateTime FechaEstimadaFinDepreciacion { get; set; }
        public string EstadoDepreciacion { get; set; } = string.Empty;
    }

    public class ReporteDepreciacionFiltroViewModel
    {
        public Guid? CategoriaId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? EstadoDepreciacion { get; set; }
        public Guid? UbicacionId { get; set; }
        public string? Formato { get; set; }
    }

    public class AnalisisValorDepartamentoViewModel
    {
        public List<DepartamentoValorViewModel> Departamentos { get; set; } = new();
    }

    public class DepartamentoValorViewModel
    {
        public Guid UbicacionId { get; set; }
        public string NombreUbicacion { get; set; } = string.Empty;
        public decimal ValorOriginalTotal { get; set; }
        public decimal ValorActualTotal { get; set; }
        public decimal DepreciacionTotal { get; set; }
        public decimal PromedioAntiguedad { get; set; }
        public int CantidadActivos { get; set; }
    }

    public class AlertasDepreciacionViewModel
    {
        public List<AlertaDepreciacionItemViewModel> Alertas { get; set; } = new();
    }

    public class AlertaDepreciacionItemViewModel
    {
        public Guid ActivoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PorcentajeConsumido { get; set; }
    }
}
