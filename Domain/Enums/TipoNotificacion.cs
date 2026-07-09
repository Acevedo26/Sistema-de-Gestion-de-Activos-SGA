namespace Sistema_de_Gestion_de_Activos.Domain.Enums
{
    /// <summary>
    /// Tipos de alertas del sistema (RF-10, RF-28, RF-29).
    /// Se persiste como string.
    /// </summary>
    public enum TipoNotificacion
    {
        VencimientoMantenimiento,
        DepreciacionProxima,
        CostoMantenimientoExcesivo
    }
}
