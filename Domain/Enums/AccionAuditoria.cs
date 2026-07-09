namespace Sistema_de_Gestion_de_Activos.Domain.Enums
{
    /// <summary>
    /// Acciones auditables en el sistema (RF-03, RF-18).
    /// Se persiste como string.
    /// </summary>
    public enum AccionAuditoria
    {
        INSERT,
        UPDATE,
        DELETE
    }
}
