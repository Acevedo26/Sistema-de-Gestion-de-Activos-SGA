namespace Sistema_de_Gestion_de_Activos.Domain.Enums
{
    /// <summary>
    /// Estado del proceso de mantenimiento (RF-08).
    /// Se persiste como string.
    /// </summary>
    public enum EstadoMantenimiento
    {
        Pendiente,
        EnProgreso, // Se omite el espacio para validez en C#
        Finalizado
    }
}
