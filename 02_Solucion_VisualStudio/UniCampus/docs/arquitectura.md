# Diagrama de arquitectura (tres capas + MVP) - entrega final

```mermaid
flowchart LR
  subgraph Presentacion["Presentación (UniCampus.Web)"]
    PB["PaginaBase<br/>sesión y roles"]
    V["Vistas Web Forms<br/>Login, Actividades, Detalle,<br/>MisInscripciones, Reportes<br/>+ interfaces I...View"]
    P["Presenters<br/>coordinan cada vista"]
    PB --> V --> P
  end
  subgraph Negocio["Negocio (UniCampus.BLL)"]
    S["Servicios<br/>Actividad, Inscripcion, Resena,<br/>Usuario, Reporte<br/>reglas, validaciones y roles"]
  end
  subgraph Datos["Datos (UniCampus.DAL)"]
    R["Repositorios ADO.NET<br/>consultas parametrizadas<br/>transacción de inscripción"]
  end
  E["UniCampus.Entities<br/>DTO compartidos"]
  DB[("SQL Server<br/>UniCampusDB")]
  P --> S --> R --> DB
  V -. usa .-> E
  S -. usa .-> E
  R -. usa .-> E
```

Dependencias (hacia adentro): Web -> BLL -> DAL -> SQL Server. `UniCampus.Entities` es referenciado por todas las capas.
La BLL depende de **interfaces** de repositorio (`I...Repository`), lo que permite probarla con repositorios simulados.

## Flujo de "Inscribirse"

```mermaid
sequenceDiagram
  participant U as Estudiante
  participant V as Detalle.aspx
  participant P as DetallePresenter
  participant S as InscripcionService
  participant R as InscripcionRepository
  participant DB as SQL Server
  U->>V: clic en "Inscribirme"
  V->>P: Inscribirse()
  P->>S: Inscribir(actividadId, usuario de la sesión)
  S->>S: valida rol y datos
  S->>R: Inscribir(actividadId, usuarioId, ahora)
  R->>DB: BEGIN TRAN + bloqueo de la actividad
  R->>DB: verifica estado, fecha, duplicado y cupo
  R->>DB: INSERT/UPDATE de la inscripción + COMMIT
  R-->>S: ResultadoInscripcion
  S-->>P: ResultadoOperacion (mensaje claro)
  P-->>V: Cargar() y MostrarMensaje()
```
