# Sistema de Gestión de Activos (SGA)  
### *Optimización, Trazabilidad y Control Financiero del Ciclo de Vida Corporativo*

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Frontend-Blazor%20Server-512BD4?logo=blazor)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-2E8B57)](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
[![Database](https://img.shields.io/badge/Database-SQL%20Server%20LocalDB-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/en-us/sql-server)
[![Styling](https://img.shields.io/badge/UI-Tailwind%20CSS%20%2B%20Flowbite-06B6D4?logo=tailwind-css)](https://tailwindcss.com/)

---
## Resumen Ejecutivo
El **Sistema de Gestión de Activos (SGA)** es una solución empresarial de grado profesional diseñada para administrar, auditar y optimizar el ciclo de vida completo de los activos corporativos. Integra en una sola plataforma el control financiero, la gestión de mantenimiento, la trazabilidad de movimientos y el análisis de rendimiento, proporcionando a las organizaciones visibilidad total para maximizar el ROI y extender la vida útil de sus recursos.

Construido bajo los rigurosos principios de **Clean Architecture**, el sistema garantiza una separación estricta de responsabilidades, alta mantenibilidad y una experiencia de usuario (UX) moderna, responsiva y adaptativa (modo claro/oscuro).

## Características Principales

### Gestión Financiera Integral
- **Registro Patrimonial Completo**: Seguimiento detallado de cada activo, incluyendo costo de adquisición, cálculo automático de depreciación y valoración en tiempo real.
- **Dashboard Ejecutivo Interactivo**: Visualización de métricas clave (estado de depreciación, valoración global) mediante **ApexCharts**, con soporte nativo para temas claro y oscuro.

### Mantenimiento Inteligente y Predictivo
- **Historial de Intervenciones**: Registro estructurado de mantenimientos preventivos y correctivos.
- **Reglas de Negocio Automatizadas**: Sistema de alertas tempranas que recomienda la sustitución del activo cuando el costo acumulado de mantenimiento supera el **30% de su valor actual**, optimizando la toma de decisiones financieras.

### Auditoría y Trazabilidad Total
- **Línea de Tiempo (Timeline) Auditable**: Registro inmutable de todas las reasignaciones y cambios de ubicación (Edificios, Departamentos, Oficinas), garantizando la cadena de custodia y el cumplimiento normativo.
- **Panel de Detalle Dinámico**: Interfaz unificada en pestañas que consolida información financiera, historial de movimientos y registros de mantenimiento en una sola vista contextual.

### Despliegue y Onboarding Cero Configuración
- **Inicialización Autónoma de Base de Datos**: El sistema detecta la ausencia de la base de datos `SgaDb_App_V2` en LocalDB, la crea y la pobla automáticamente con más de **50 registros realistas e interconectados** al primer inicio. *Sin necesidad de ejecutar comandos CLI de migraciones.*

---

## 🏗️ Stack Tecnológico y Arquitectura

El sistema está diseñado para escalar, evolucionar y mantenerse con el menor costo técnico posible:

| Capa | Tecnologías | Beneficio Comercial |
| :--- | :--- | :--- |
| **Backend** | ASP.NET Core 8.0, Entity Framework Core | Alto rendimiento, tipado fuerte y ecosistema empresarial. |
| **Frontend** | Blazor Interactive Server, Tailwind CSS, Flowbite | UX moderna, carga rápida y diseño responsivo sin sobrecarga de JavaScript. |
| **Base de Datos** | SQL Server (LocalDB) | Fiabilidad, integridad transaccional y compatibilidad corporativa. |
| **Arquitectura** | Clean Architecture (Domain, Application, Infrastructure, UI) | Código desacoplado, altamente testeable y preparado para crecimiento. |
| **Seguridad** | BCrypt Hashing | Protección de credenciales con estándares criptográficos de la industria. |

---

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
