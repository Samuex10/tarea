# UniCampus - Plataforma de registro de actividades académicas

Proyecto parcial del tercer corte de **Desarrollo de Aplicaciones Web II**
Corporación Universitaria Americana · Docente: Ingeniera Cindy Fuentes

**Entrega final - 23 de noviembre de 2026 (50%)**
Repositorio: https://github.com/Samuex10/Reto-Actividades-Academicas-DW2

## Descripción

Aplicación web para gestionar actividades académicas, organizada en **tres capas** (Presentación, Negocio, Datos)
con el patrón **Model-View-Presenter (MVP)** en la capa de presentación, sobre ASP.NET Web Forms y SQL Server.

## Integrantes y responsabilidades

| Integrante | Responsabilidad |
|---|---|
| Samuel Camelo | [completar] |

## Funcionalidades

| Función | Administrador | Estudiante |
|---|---|---|
| Iniciar y cerrar sesión (contraseñas con hash PBKDF2) | ✔ | ✔ |
| Registrar, editar, desactivar y eliminar actividades | ✔ | - |
| Listar, buscar y filtrar (nombre, fecha, tipo, estado) | ✔ (todos los estados) | ✔ (Publicada y Finalizada) |
| Ver el detalle de una actividad | ✔ (con lista de inscritos) | ✔ |
| Inscribirse y cancelar la inscripción (control de cupos en transacción) | - | ✔ |
| Ver "Mis inscripciones" | - | ✔ |
| Publicar reseñas y calificaciones (1 a 5) | - | ✔ (si estuvo inscrito) |
| Reporte-resumen por actividad (inscritos, cupos, promedio) | ✔ | - |

## Reglas de negocio principales

- **Actividad:** título obligatorio (máx. 120), tipo obligatorio, fechas válidas (fin no anterior al inicio), cupo entero mayor que cero,
  estado permitido; las actividades nuevas no pueden empezar en el pasado; el cupo no puede quedar por debajo de los inscritos.
- **Eliminar:** solo si no tiene inscripciones ni reseñas; en otro caso se usa **Desactivar** (estado Cancelada).
- **Inscripción:** solo estudiantes; solo actividades Publicadas que no han iniciado; sin duplicados; sin superar el cupo.
  La verificación se hace en una **transacción con bloqueo de fila** (`UPDLOCK, HOLDLOCK`), por lo que dos solicitudes simultáneas
  no pueden consumir el último cupo. Una inscripción cancelada se reactiva sin duplicar la fila (restricción `UNIQUE`).
- **Reseñas:** solo estudiantes inscritos, la actividad ya debe haber comenzado, calificación entera de 1 a 5,
  comentario de hasta 1000 caracteres y una sola reseña por estudiante y actividad (restricción `UNIQUE` en la base de datos).
- **Roles:** cada página y cada operación del servicio verifican el rol; el usuario sale de la **sesión del servidor**, no de un control de la página.

## Arquitectura

Flujo: `Web Forms -> Presenter (MVP) -> Servicio de negocio -> Repositorio -> SQL Server`

| Proyecto | Capa | Contenido |
|---|---|---|
| `UniCampus.Web` | Presentación | Páginas `.aspx` (Login, Actividades, Detalle, MisInscripciones, Reportes), interfaces de vista y Presenters |
| `UniCampus.BLL` | Negocio | `ActividadService`, `InscripcionService`, `ResenaService`, `UsuarioService`, `ReporteService`, `SeguridadClave` |
| `UniCampus.DAL` | Datos | Repositorios ADO.NET parametrizados e interfaces (`IActividadRepository`, ...) |
| `UniCampus.Entities` | Compartida | DTO: `ActividadDto`, `UsuarioDto`, `InscripcionDto`, `ResenaDto`, ... |
| `UniCampus.Tests` | Pruebas | Pruebas unitarias de las reglas de negocio con repositorios simulados |

Referencias: Web -> BLL, Entities; BLL -> DAL, Entities; DAL -> Entities; Tests -> BLL, DAL, Entities.
La capa de datos no depende de la capa Web. Diagrama en [`docs/arquitectura.md`](docs/arquitectura.md).

## Seguridad

- Consultas SQL **siempre parametrizadas** con tipo y longitud (`SqlParameter`); la búsqueda escapa los comodines de `LIKE`.
- Validación **en el servidor** (capa de negocio), además de la del navegador.
- Contraseñas guardadas como hash **PBKDF2-SHA256** con sal; comparación en tiempo constante; mensaje genérico de error de login.
- Salida codificada en HTML (prevención de XSS) y validación de solicitudes de ASP.NET activa.
- Cookies de sesión `HttpOnly`, `ViewStateUserKey` ligado a la sesión (CSRF), páginas privadas sin caché.
- Errores controlados: mensajes amigables al usuario y detalle técnico en `UniCampus.Web/App_Data/errores.log`.
- Sin credenciales en el repositorio: la cadena de conexión usa autenticación integrada de Windows.

## Requisitos

- Visual Studio 2022 o posterior con la carga de trabajo **ASP.NET y desarrollo web**
- .NET Framework 4.8 (o 4.8.1) Developer Pack
- SQL Server LocalDB (incluido con Visual Studio) o SQL Server Express

## Ejecución

1. Abrir `UniCampus.sln` en Visual Studio y compilar (**Ctrl+Shift+B**; la primera vez restaura los paquetes de pruebas).
2. Establecer `UniCampus.Web` como proyecto de inicio y pulsar **F5**.
3. Al arrancar, la aplicación **crea y actualiza la base `UniCampusDB`** con el script `database/03_BaseDatos.sql`
   (idempotente: tablas, restricciones y datos de prueba). Ese script también puede ejecutarse a mano en SQL Server.
4. La cadena `UniCampusDb` está en `UniCampus.Web/Web.config` (LocalDB por defecto; hay una opción comentada para SQL Server Express).
5. Iniciar sesión con una cuenta de prueba (datos ficticios):

| Correo | Contraseña | Rol |
|---|---|---|
| admin@unicampus.edu | Admin123* | Administrador |
| estudiante.demo@ejemplo.edu | Estudiante123* | Estudiante |
| ana.torres@unicampus.edu | Estudiante123* | Estudiante |
| carlos.perez@unicampus.edu | Estudiante123* | Estudiante |

Ayuda para problemas de conexión: abrir `/Diagnostico.aspx` (solo desde el mismo equipo) o revisar `App_Data/errores.log`.

## Base de datos

| Tabla | Uso | Restricciones destacadas |
|---|---|---|
| `TipoActividad` | Clasifica las actividades | PK, nombre único |
| `ActividadAcademica` | Entidad central | PK, FK a tipo, `Cupos >= 0`, `FechaFin >= FechaInicio`, estados permitidos |
| `Usuario` | Administradores y estudiantes | PK, correo único, hash y sal de la contraseña |
| `Inscripcion` | Relaciona usuarios y actividades | PK, FK, `UNIQUE (ActividadId, UsuarioId)`, estado Activa/Cancelada |
| `Resena` | Valoraciones | PK, FK, `Calificacion BETWEEN 1 AND 5`, `UNIQUE (ActividadId, UsuarioId)` |

> [Completar: indicar qué entidades del diagrama entidad-relación del equipo se reutilizaron y qué ajustes se hicieron.]

`database/verificacion.sql` contiene las consultas usadas como evidencia de las pruebas.

## Pruebas

- **Automáticas:** menú **Prueba -> Ejecutar todas las pruebas** (proyecto `UniCampus.Tests`, 42 pruebas de reglas de negocio:
  validaciones, roles, cupos, duplicados, reseñas, autenticación).
- **Funcionales:** casos documentados en [`docs/PRUEBAS_FINAL.md`](docs/PRUEBAS_FINAL.md), con capturas en `docs/capturas`.
- **Primera entrega:** [`docs/PRUEBAS.md`](docs/PRUEBAS.md).

## Estructura

```
UniCampus/
├── UniCampus.sln
├── UniCampus.Web/  UniCampus.BLL/  UniCampus.DAL/  UniCampus.Entities/  UniCampus.Tests/
├── database/    03_BaseDatos.sql, verificacion.sql
├── docs/        arquitectura, pruebas, sustentación, capturas
├── Respaldo_Proyecto_Inicial/
└── README.md
```

## Limitaciones conocidas y trabajo futuro

- La sesión usa memoria del servidor (`InProc`); para varios servidores habría que usar un almacén de sesión compartido.
- No hay recuperación de contraseña ni registro de nuevos usuarios desde la interfaz (los usuarios se crean en la base de datos).
- La publicación en Internet (milla extra) no se realizó.

