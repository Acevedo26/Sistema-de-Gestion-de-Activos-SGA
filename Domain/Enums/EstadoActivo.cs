namespace Sistema_de_Gestion_de_Activos.Domain.Enums
{
    /// <summary>
    /// Representa el estado actual de un activo (RF-01, RF-02).
    /// Se persiste como string para facilitar su lectura en la base de datos.
    /// </summary>
    public enum EstadoActivo
    {
        Activo,
        Mantenimiento,
        Inactivo,
        Baja,
        Descartado
    }
}
