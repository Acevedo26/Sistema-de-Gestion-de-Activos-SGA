# Sistema de Gestión de Activos (SGA)

Sistema completo para la gestión del ciclo de vida de activos corporativos, integrando control financiero, mantenimiento, historial de movimientos y análisis de rendimiento, construido bajo principios de **Clean Architecture** utilizando ASP.NET Core y Blazor.

## Características Principales

*   **Gestión Integral de Activos:** Registro y seguimiento de activos incluyendo datos financieros (costo original, depreciación, valor actual).
*   **Detalle Dinámico:** Panel interactivo en pestañas que unifica información financiera, historial de movimientos y registros de mantenimiento.
*   **Mantenimiento Inteligente:** Registro del historial de intervenciones (preventivas y correctivas), cálculo de costos acumulados y alertas tempranas (recomendaciones de reemplazo si el costo de mantenimiento supera el 30% del valor del activo).
*   **Auditoría y Trazabilidad:** Historial de reasignaciones y cambios de ubicación (Edificios, Departamentos y Oficinas) registrados en una línea de tiempo (timeline).
*   **Dashboard Interactivo:** Análisis visual del estado de depreciación y valoración mediante ApexCharts, reaccionando dinámicamente al modo claro / oscuro.
*   **Inicialización Autónoma de Base de Datos:** Base de datos auto-creada en el inicio de la aplicación y sembrada con datos de prueba realistas para probar todas las funcionalidades instantáneamente sin requerir comandos CLI de migraciones.

## Tecnologías

*   **Backend:** ASP.NET Core 8.0, Entity Framework Core, SQL Server (LocalDB)
*   **Frontend:** Blazor Interactive Server, Tailwind CSS, Flowbite, ApexCharts
*   **Arquitectura:** Clean Architecture (Domain, Application, Infrastructure/Data, UI/Components)

## Cómo empezar

El proyecto incluye un inicializador de base de datos automatizado (`DbInitializer`) que se encarga de crear la base de datos `SgaDb_App_V2` de LocalDB e inyectarle más de 50 registros pre-poblados interconectados si ésta se encuentra vacía.

1. Clona el repositorio.
2. Abre la solución y compila el proyecto (`dotnet build`).
3. Ejecuta la aplicación (`dotnet run` o con F5 en tu IDE). La base de datos será creada y poblada automáticamente.

### Cuentas de Prueba

El sistema se inicializa con varios roles y usuarios preconfigurados para explorar los distintos permisos. **Todos tienen la misma contraseña generada correctamente por BCrypt**:

**Contraseña para todos los usuarios:** `Password123!`

**Usuarios por Rol:**
*   **Administradores:** `ana.admin@empresa.com`, `carlos.admin@empresa.com`
*   **Gestores:** `maria.gestor@empresa.com`, `juan.gestor@empresa.com`
*   **Técnicos:** `pedro.tecnico@empresa.com`, `laura.tecnico@empresa.com`
*   **Visualizadores:** `luis.visor@empresa.com`, `sofia.visor@empresa.com`

---
*Desarrollado con un estricto enfoque en la separación de responsabilidades (UI pasiva consumiendo DTOs preprocesados) y una experiencia de usuario moderna y responsiva.*
