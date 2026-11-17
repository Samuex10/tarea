/* =====================================================================
   UniCampus - Script de base de datos (ENTREGA FINAL)
   Reproducible e idempotente: puede ejecutarse varias veces sin duplicar
   datos y actualiza una base creada con la primera entrega.
   Si su equipo tiene un diagrama ER aprobado, documente en el informe
   que entidades se reutilizaron o ajustaron.
   Cuentas de prueba (datos ficticios):
     admin@unicampus.edu            Admin123*        (Administrador)
     estudiante.demo@ejemplo.edu    Estudiante123*   (Estudiante)
     ana.torres@unicampus.edu       Estudiante123*   (Estudiante)
     carlos.perez@unicampus.edu     Estudiante123*   (Estudiante)
   ===================================================================== */
IF DB_ID(N'UniCampusDB') IS NULL CREATE DATABASE UniCampusDB;
GO
USE UniCampusDB;
GO

/* ---------- Tablas base ---------- */
IF OBJECT_ID(N'dbo.TipoActividad', N'U') IS NULL
CREATE TABLE dbo.TipoActividad (
    TipoId      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(60)  NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL
);
GO
IF OBJECT_ID(N'dbo.ActividadAcademica', N'U') IS NULL
CREATE TABLE dbo.ActividadAcademica (
    ActividadId  INT IDENTITY(1,1) PRIMARY KEY,
    Titulo       NVARCHAR(120)  NOT NULL,
    Descripcion  NVARCHAR(1000) NULL,
    FechaInicio  DATETIME2      NOT NULL,
    FechaFin     DATETIME2      NOT NULL,
    Lugar        NVARCHAR(150)  NULL,
    Cupos        INT            NOT NULL CONSTRAINT DF_Actividad_Cupos DEFAULT (0),
    Estado       NVARCHAR(20)   NOT NULL CONSTRAINT DF_Actividad_Estado DEFAULT ('Publicada'),
    CONSTRAINT CK_Actividad_Cupos  CHECK (Cupos >= 0),
    CONSTRAINT CK_Actividad_Fechas CHECK (FechaFin >= FechaInicio),
    CONSTRAINT CK_Actividad_Estado CHECK (Estado IN (N'Borrador', N'Publicada', N'Cancelada', N'Finalizada'))
);
GO
IF COL_LENGTH(N'dbo.ActividadAcademica', N'TipoId') IS NULL
    ALTER TABLE dbo.ActividadAcademica ADD TipoId INT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Actividad_Tipo')
    ALTER TABLE dbo.ActividadAcademica
        ADD CONSTRAINT FK_Actividad_Tipo FOREIGN KEY (TipoId) REFERENCES dbo.TipoActividad(TipoId);
GO
IF OBJECT_ID(N'dbo.Usuario', N'U') IS NULL
CREATE TABLE dbo.Usuario (
    UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre    NVARCHAR(100) NOT NULL,
    Correo    NVARCHAR(150) NOT NULL UNIQUE,
    Rol       NVARCHAR(30)  NOT NULL
);
GO
IF COL_LENGTH(N'dbo.Usuario', N'PasswordHash') IS NULL ALTER TABLE dbo.Usuario ADD PasswordHash VARBINARY(32) NULL;
GO
IF COL_LENGTH(N'dbo.Usuario', N'PasswordSalt') IS NULL ALTER TABLE dbo.Usuario ADD PasswordSalt VARBINARY(16) NULL;
GO
IF COL_LENGTH(N'dbo.Usuario', N'Activo') IS NULL
    ALTER TABLE dbo.Usuario ADD Activo BIT NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1);
GO
IF OBJECT_ID(N'dbo.Inscripcion', N'U') IS NULL
CREATE TABLE dbo.Inscripcion (
    InscripcionId    INT IDENTITY(1,1) PRIMARY KEY,
    ActividadId      INT NOT NULL,
    UsuarioId        INT NOT NULL,
    FechaInscripcion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Estado           NVARCHAR(20) NOT NULL DEFAULT ('Activa'),
    CONSTRAINT FK_Inscripcion_Actividad FOREIGN KEY (ActividadId) REFERENCES dbo.ActividadAcademica(ActividadId),
    CONSTRAINT FK_Inscripcion_Usuario   FOREIGN KEY (UsuarioId)   REFERENCES dbo.Usuario(UsuarioId),
    CONSTRAINT UQ_Inscripcion_Actividad_Usuario UNIQUE (ActividadId, UsuarioId)
);
GO
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Inscripcion_Estado')
    ALTER TABLE dbo.Inscripcion
        ADD CONSTRAINT CK_Inscripcion_Estado CHECK (Estado IN (N'Activa', N'Cancelada'));
GO
IF OBJECT_ID(N'dbo.Resena', N'U') IS NULL
CREATE TABLE dbo.Resena (
    ResenaId      INT IDENTITY(1,1) PRIMARY KEY,
    ActividadId   INT NOT NULL,
    UsuarioId     INT NOT NULL,
    Calificacion  TINYINT NOT NULL,
    Comentario    NVARCHAR(1000) NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT CK_Resena_Calificacion CHECK (Calificacion BETWEEN 1 AND 5),
    CONSTRAINT FK_Resena_Actividad FOREIGN KEY (ActividadId) REFERENCES dbo.ActividadAcademica(ActividadId),
    CONSTRAINT FK_Resena_Usuario   FOREIGN KEY (UsuarioId)   REFERENCES dbo.Usuario(UsuarioId)
);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Resena_Actividad_Usuario' AND object_id = OBJECT_ID(N'dbo.Resena'))
    CREATE UNIQUE INDEX UX_Resena_Actividad_Usuario ON dbo.Resena (ActividadId, UsuarioId);
GO

/* ---------- Datos de prueba (ficticios) ---------- */
INSERT INTO dbo.TipoActividad (Nombre, Descripcion)
SELECT v.Nombre, v.Descripcion
FROM (VALUES
    (N'Taller',       N'Actividad práctica guiada'),
    (N'Conferencia',  N'Charla magistral'),
    (N'Seminario',    N'Espacio de discusión académica'),
    (N'Curso',        N'Formación de varias sesiones'),
    (N'Encuentro',    N'Encuentro o feria académica')
) AS v(Nombre, Descripcion)
WHERE NOT EXISTS (SELECT 1 FROM dbo.TipoActividad t WHERE t.Nombre = v.Nombre);
GO
IF NOT EXISTS (SELECT 1 FROM dbo.ActividadAcademica WHERE Titulo = N'Feria de proyectos tecnológicos')
    INSERT INTO dbo.ActividadAcademica (Titulo, Descripcion, FechaInicio, FechaFin, Lugar, Cupos, Estado)
    VALUES (N'Feria de proyectos tecnológicos', N'Presentación de proyectos desarrollados por estudiantes.',
            '2026-11-18T09:00:00', '2026-11-18T13:00:00', N'Auditorio principal', 40, N'Publicada');
IF NOT EXISTS (SELECT 1 FROM dbo.ActividadAcademica WHERE Titulo = N'Taller de seguridad digital')
    INSERT INTO dbo.ActividadAcademica (Titulo, Descripcion, FechaInicio, FechaFin, Lugar, Cupos, Estado)
    VALUES (N'Taller de seguridad digital', N'Buenas prácticas para proteger información y cuentas.',
            '2026-11-20T14:00:00', '2026-11-20T17:00:00', N'Laboratorio 2', 25, N'Publicada');
IF NOT EXISTS (SELECT 1 FROM dbo.ActividadAcademica WHERE Titulo = N'Conferencia de inteligencia artificial')
    INSERT INTO dbo.ActividadAcademica (Titulo, Descripcion, FechaInicio, FechaFin, Lugar, Cupos, Estado)
    VALUES (N'Conferencia de inteligencia artificial', N'Tendencias y aplicaciones de la IA en la educación.',
            '2026-12-02T09:00:00', '2026-12-02T11:00:00', N'Auditorio principal', 100, N'Publicada');
IF NOT EXISTS (SELECT 1 FROM dbo.ActividadAcademica WHERE Titulo = N'Charla con cupo limitado')
    INSERT INTO dbo.ActividadAcademica (Titulo, Descripcion, FechaInicio, FechaFin, Lugar, Cupos, Estado)
    VALUES (N'Charla con cupo limitado', N'Actividad de un solo cupo para probar el control de cupos.',
            '2026-12-10T15:00:00', '2026-12-10T16:00:00', N'Sala de juntas', 1, N'Publicada');
IF NOT EXISTS (SELECT 1 FROM dbo.ActividadAcademica WHERE Titulo = N'Seminario de bases de datos')
    INSERT INTO dbo.ActividadAcademica (Titulo, Descripcion, FechaInicio, FechaFin, Lugar, Cupos, Estado)
    VALUES (N'Seminario de bases de datos', N'Actividad ya realizada: permite probar las reseñas.',
            '2026-10-15T08:00:00', '2026-10-15T12:00:00', N'Laboratorio 1', 30, N'Finalizada');
GO
UPDATE dbo.ActividadAcademica SET TipoId = (SELECT TipoId FROM dbo.TipoActividad WHERE Nombre = N'Encuentro')
WHERE TipoId IS NULL AND Titulo LIKE N'Feria%';
UPDATE dbo.ActividadAcademica SET TipoId = (SELECT TipoId FROM dbo.TipoActividad WHERE Nombre = N'Taller')
WHERE TipoId IS NULL AND Titulo LIKE N'Taller%';
UPDATE dbo.ActividadAcademica SET TipoId = (SELECT TipoId FROM dbo.TipoActividad WHERE Nombre = N'Conferencia')
WHERE TipoId IS NULL AND (Titulo LIKE N'Conferencia%' OR Titulo LIKE N'Charla%');
UPDATE dbo.ActividadAcademica SET TipoId = (SELECT TipoId FROM dbo.TipoActividad WHERE Nombre = N'Seminario')
WHERE TipoId IS NULL AND Titulo LIKE N'Seminario%';
GO

/* ---------- Usuarios (contraseñas con PBKDF2-SHA256, nunca en texto plano) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'admin@unicampus.edu')
    INSERT INTO dbo.Usuario (Nombre, Correo, Rol) VALUES (N'Administrador UniCampus', N'admin@unicampus.edu', N'Administrador');
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'estudiante.demo@ejemplo.edu')
    INSERT INTO dbo.Usuario (Nombre, Correo, Rol) VALUES (N'Estudiante de prueba', N'estudiante.demo@ejemplo.edu', N'Estudiante');
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'ana.torres@unicampus.edu')
    INSERT INTO dbo.Usuario (Nombre, Correo, Rol) VALUES (N'Ana Torres', N'ana.torres@unicampus.edu', N'Estudiante');
IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE Correo = N'carlos.perez@unicampus.edu')
    INSERT INTO dbo.Usuario (Nombre, Correo, Rol) VALUES (N'Carlos Pérez', N'carlos.perez@unicampus.edu', N'Estudiante');
GO
UPDATE dbo.Usuario SET PasswordSalt = 0x10AA802881DEFDC306DDEC872E57CA0B, PasswordHash = 0x16F238683ED382996BD331E5AA730D1D2E886A69F6376B2AC4576894E034EB7D
WHERE Correo = N'admin@unicampus.edu' AND PasswordHash IS NULL;
UPDATE dbo.Usuario SET PasswordSalt = 0x9B4C54286CE689B062126ADABE1B4412, PasswordHash = 0x23B20E4905BDBD98532ADCDD3B69B1ACB23EA500C0D64194BA9619DAA22114E9
WHERE Correo = N'estudiante.demo@ejemplo.edu' AND PasswordHash IS NULL;
UPDATE dbo.Usuario SET PasswordSalt = 0x562C5ADA23990AA654F57F519C980EC8, PasswordHash = 0x9C7C7334AA0EA5E9CBCD3BF98B97C9D482E5B8B8EADF77B394C0CE247571B885
WHERE Correo = N'ana.torres@unicampus.edu' AND PasswordHash IS NULL;
UPDATE dbo.Usuario SET PasswordSalt = 0xDDF93EC03D98E6915D1F3AB478A1C165, PasswordHash = 0xA9DC196878EDB64F4AF4BE6041C522E0DF36655A024A843648B06453DDC1D1B6
WHERE Correo = N'carlos.perez@unicampus.edu' AND PasswordHash IS NULL;
GO

/* ---------- Inscripción y reseña de ejemplo en la actividad ya realizada ---------- */
INSERT INTO dbo.Inscripcion (ActividadId, UsuarioId, FechaInscripcion, Estado)
SELECT a.ActividadId, u.UsuarioId, '2026-10-01T10:00:00', N'Activa'
FROM dbo.ActividadAcademica a CROSS JOIN dbo.Usuario u
WHERE a.Titulo = N'Seminario de bases de datos'
  AND u.Correo IN (N'estudiante.demo@ejemplo.edu', N'ana.torres@unicampus.edu')
  AND NOT EXISTS (SELECT 1 FROM dbo.Inscripcion i WHERE i.ActividadId = a.ActividadId AND i.UsuarioId = u.UsuarioId);
GO
INSERT INTO dbo.Resena (ActividadId, UsuarioId, Calificacion, Comentario, FechaCreacion)
SELECT a.ActividadId, u.UsuarioId, 5, N'Muy buen seminario, ejemplos claros.', '2026-10-16T09:00:00'
FROM dbo.ActividadAcademica a CROSS JOIN dbo.Usuario u
WHERE a.Titulo = N'Seminario de bases de datos' AND u.Correo = N'ana.torres@unicampus.edu'
  AND NOT EXISTS (SELECT 1 FROM dbo.Resena r WHERE r.ActividadId = a.ActividadId AND r.UsuarioId = u.UsuarioId);
GO

SELECT TOP (10) ActividadId, Titulo, Estado, Cupos FROM dbo.ActividadAcademica ORDER BY ActividadId DESC;
GO
