USE UniCampusDB;
-- 1) Últimas actividades
SELECT TOP (10) a.ActividadId, a.Titulo, t.Nombre AS Tipo, a.FechaInicio, a.Cupos, a.Estado
FROM dbo.ActividadAcademica a LEFT JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId
ORDER BY a.ActividadId DESC;
-- 2) Tablas existentes (deben seguir estando después de la prueba de inyección SQL)
SELECT name FROM sys.tables ORDER BY name;
-- 3) Cupos inválidos (debe ser 0)
SELECT COUNT(*) AS CuposInvalidos FROM dbo.ActividadAcademica WHERE Cupos < 0;
-- 4) Inscripciones por actividad (activas) frente al cupo
SELECT a.Titulo, a.Cupos,
       (SELECT COUNT(1) FROM dbo.Inscripcion i WHERE i.ActividadId = a.ActividadId AND i.Estado = N'Activa') AS Inscritos
FROM dbo.ActividadAcademica a ORDER BY a.ActividadId;
-- 5) Reseñas
SELECT a.Titulo, u.Nombre, r.Calificacion, r.Comentario
FROM dbo.Resena r JOIN dbo.ActividadAcademica a ON a.ActividadId = r.ActividadId JOIN dbo.Usuario u ON u.UsuarioId = r.UsuarioId;
-- 6) Usuarios y roles (las contraseñas se guardan como hash, nunca en texto plano)
SELECT UsuarioId, Nombre, Correo, Rol, Activo, CASE WHEN PasswordHash IS NULL THEN 'sin clave' ELSE 'hash PBKDF2' END AS Clave FROM dbo.Usuario;
