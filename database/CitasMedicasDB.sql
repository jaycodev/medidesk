-- =============================================
-- ELIMINAR Y CREAR BASE DE DATOS
-- =============================================

-- Eliminar la base de datos si ya existe
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'CitasMedicasDB')
BEGIN
    DROP DATABASE CitasMedicasDB;
END
GO

-- Crear nueva base de datos
CREATE DATABASE CitasMedicasDB;
GO

-- Seleccionar la base de datos creada
USE CitasMedicasDB;
GO

-- =============================================
-- TABLAS DEL SISTEMA DE CITAS MÉDICAS
-- =============================================

-- Tabla: Usuarios
-- Descripción: Almacena información de todos los usuarios (admin, médicos, pacientes)
CREATE TABLE Usuarios (
    id_usuario INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    correo VARCHAR(100) NOT NULL UNIQUE,
    contraseña VARCHAR(100) NOT NULL,
    telefono VARCHAR(20),
    rol VARCHAR(20) NOT NULL CHECK (rol IN ('administrador', 'medicos', 'pacientes')),
    foto_perfil VARCHAR(255)
);
GO

-- Tabla: Especialidades
-- Descripción: Define las especialidades médicas disponibles
CREATE TABLE Especialidades (
    id_especialidad INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(255)
);
GO

-- Tabla: Médicos
-- Descripción: Relación entre médicos y sus especialidades
CREATE TABLE Medicos (
    id_usuario INT PRIMARY KEY,
    id_especialidad INT NOT NULL,
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario),
    FOREIGN KEY (id_especialidad) REFERENCES Especialidades(id_especialidad)
);
GO

-- Tabla: Pacientes
-- Descripción: Información adicional de pacientes
CREATE TABLE Pacientes (
    id_usuario INT PRIMARY KEY,
    fecha_nacimiento DATE,
    grupo_sanguineo VARCHAR(3) CHECK (grupo_sanguineo IN ('A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-')),
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario)
);
GO

-- Tabla: Citas
-- Descripción: Almacena información sobre las citas médicas agendadas
CREATE TABLE Citas (
    id_cita INT PRIMARY KEY IDENTITY(1,1),
    id_medico INT NOT NULL,
    id_paciente INT NOT NULL,
    id_especialidad INT NOT NULL,
    fecha DATE NOT NULL,
    hora TIME NOT NULL,
    tipo_consulta VARCHAR(20) NOT NULL CHECK (tipo_consulta IN ('examen', 'consulta', 'operacion')),
    sintomas TEXT,
    estado VARCHAR(20) DEFAULT 'pendiente' CHECK (estado IN ('pendiente', 'confirmada', 'cancelada')),
    FOREIGN KEY (id_medico) REFERENCES Medicos(id_usuario),
    FOREIGN KEY (id_paciente) REFERENCES Pacientes(id_usuario),
    FOREIGN KEY (id_especialidad) REFERENCES Especialidades(id_especialidad)
);
GO

-- Tabla: HorariosDisponibles
-- Descripción: Registra los horarios disponibles de cada médico por día
CREATE TABLE HorariosDisponibles (
    id_horario INT PRIMARY KEY IDENTITY(1,1),
    id_medico INT NOT NULL,
    dia_semana VARCHAR(10) NOT NULL CHECK (dia_semana IN ('lunes', 'martes', 'miércoles', 'jueves', 'viernes', 'sábado', 'domingo')),
    hora_inicio TIME NOT NULL,
    hora_fin TIME NOT NULL,
    FOREIGN KEY (id_medico) REFERENCES Medicos(id_usuario),
    CONSTRAINT UQ_Medico_Dia UNIQUE (id_medico, dia_semana)
);
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS
-- =============================================

-- =============================================
-- PROCEDIMIENTO: usp_usuarios_crud
-- DESCRIPCIÓN : CRUD para la tabla Usuarios
-- PARÁMETROS  : indicador de acción, datos de usuario
-- =============================================
CREATE OR ALTER PROCEDURE usp_usuarios_crud
    @indicador VARCHAR(50),
    @id_usuario INT = NULL,
    @nombre VARCHAR(100) = NULL,
    @apellido VARCHAR(100) = NULL,
    @correo VARCHAR(100) = NULL,
    @contraseña VARCHAR(100) = NULL,
    @telefono VARCHAR(20) = NULL,
    @rol VARCHAR(20) = NULL,
    @foto_perfil VARCHAR(255) = NULL
AS
BEGIN
    IF @indicador = 'INSERTAR'
    BEGIN
        INSERT INTO Usuarios (nombre, apellido, correo, contraseña, telefono, rol, foto_perfil)
        VALUES (@nombre, @apellido, @correo, @contraseña, @telefono, @rol, @foto_perfil);
    END

    IF @indicador = 'ACTUALIZAR'
    BEGIN
        UPDATE Usuarios
        SET nombre = @nombre,
            apellido = @apellido,
            correo = @correo,
            contraseña = @contraseña,
            telefono = @telefono,
            rol = @rol,
            foto_perfil = @foto_perfil
        WHERE id_usuario = @id_usuario;
    END

    IF @indicador = 'ELIMINAR'
    BEGIN
        DELETE FROM Usuarios WHERE id_usuario = @id_usuario;
    END

    IF @indicador = 'CONSULTAR_X_ID'
    BEGIN
        SELECT * FROM Usuarios WHERE id_usuario = @id_usuario;
    END

    IF @indicador = 'CONSULTAR_TODO'
    BEGIN
        SELECT * FROM Usuarios;
    END

    IF @indicador = 'CONSULTAR_X_CORREO'
    BEGIN
        SELECT * FROM Usuarios WHERE correo = @correo;
    END

    IF @indicador = 'CONSULTAR_X_ROL'
    BEGIN
        SELECT * FROM Usuarios WHERE rol = @rol;
    END
END;
GO

-- =============================================
-- PROCEDIMIENTO: usp_horarios_disponibles_crud
-- DESCRIPCIÓN : CRUD para la tabla HorariosDisponibles
-- PARÁMETROS  : indicador de acción, datos de horario
-- =============================================
CREATE OR ALTER PROCEDURE usp_horarios_disponibles_crud
    @indicador VARCHAR(50),
    @id_horario INT = NULL,
    @id_medico INT = NULL,
    @dia_semana VARCHAR(10) = NULL,
    @hora_inicio TIME = NULL,
    @hora_fin TIME = NULL,
    @habilita bit = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @indicador = 'INSERTAR_O_ACTUALIZAR'
    BEGIN
        IF @habilita = 0
        BEGIN
            DELETE FROM HorariosDisponibles
            WHERE id_medico = @id_medico AND dia_semana = @dia_semana;
        END
        ELSE
        BEGIN
            MERGE INTO HorariosDisponibles AS target
            USING (SELECT @id_medico AS id_medico, @dia_semana AS dia_semana) AS source
            ON target.id_medico = source.id_medico AND target.dia_semana = source.dia_semana
            WHEN MATCHED THEN
                UPDATE SET 
                    hora_inicio = @hora_inicio,
                    hora_fin = @hora_fin
            WHEN NOT MATCHED THEN
                INSERT (id_medico, dia_semana, hora_inicio, hora_fin)
                VALUES (@id_medico, @dia_semana, @hora_inicio, @hora_fin);
        END
    END

    ELSE IF @indicador = 'ELIMINAR'
    BEGIN
        DELETE FROM HorariosDisponibles
        WHERE id_horario = @id_horario;
    END

    ELSE IF @indicador = 'CONSULTAR_TODO'
    BEGIN
        SELECT * FROM HorariosDisponibles;
    END

    ELSE IF @indicador = 'CONSULTAR_X_ID'
    BEGIN
        SELECT * FROM HorariosDisponibles
        WHERE id_horario = @id_horario;
    END

    ELSE IF @indicador = 'CONSULTAR_X_MEDICO'
    BEGIN
        SELECT * FROM HorariosDisponibles
        WHERE id_medico = @id_medico;
    END

    ELSE IF @indicador = 'CONSULTAR_X_DIA'
    BEGIN
        SELECT * FROM HorariosDisponibles
        WHERE dia_semana = @dia_semana;
    END
END;
GO

-- =============================================
-- PROCEDIMIENTO: usp_medico_crud
-- DESCRIPCIÓN : CRUD para la tabla Medicos
-- PARÁMETROS  : indicador de acción, datos de Medicos
-- =============================================
CREATE OR ALTER PROC usp_medico_crud
    @indicador VARCHAR(20),
    @id_usuario INT = NULL,
    @nombre VARCHAR(100) = NULL,
    @apellido VARCHAR(100) = NULL,
    @correo VARCHAR(100) = NULL,
    @contraseña VARCHAR(100) = NULL,
    @telefono VARCHAR(20) = NULL,
    @id_especialidad INT = NULL,
	@foto_perfil VARCHAR(255) = NULL,
	@rol varchar(20) = 'medicos'
AS
BEGIN
	IF @indicador = 'INSERTAR'
    BEGIN
        INSERT INTO Usuarios (nombre, apellido, correo, contraseña, telefono, foto_perfil, rol )
        VALUES (@nombre, @apellido, @correo, @contraseña, @telefono, @foto_perfil, @rol);

		SET @id_usuario = SCOPE_IDENTITY();

        INSERT INTO Medicos (id_usuario, id_especialidad)
        VALUES (@id_usuario, @id_especialidad);
    END
	IF @indicador = 'ACTUALIZAR'
	BEGIN
		UPDATE Usuarios
		SET nombre = @nombre,
            apellido = @apellido,
            correo = @correo,
            contraseña = @contraseña,
            telefono = @telefono,
			foto_perfil = @foto_perfil
        WHERE id_usuario = @id_usuario and
			rol = @rol
		UPDATE Medicos
        SET id_especialidad = @id_especialidad
        WHERE id_usuario = @id_usuario;

	END
	IF @indicador = 'ELIMINAR'
    BEGIN
		DELETE FROM Citas WHERE id_medico = @id_usuario;
        DELETE FROM Medicos WHERE id_usuario = @id_usuario
		DELETE FROM Usuarios WHERE id_usuario = @id_usuario
    END

    IF @indicador = 'CONSULTAR_TODO'
    BEGIN
        SELECT 
            M.id_usuario,
            U.nombre, 
            U.apellido, 
            U.correo, 
			U.contraseña,
            U.telefono,
			U.foto_perfil,
			U.rol,
			M.id_especialidad,
            E.nombre as especialidad
        FROM Medicos M
        INNER JOIN Usuarios U ON M.id_usuario = U.id_usuario
        INNER JOIN Especialidades E ON M.id_especialidad = E.id_especialidad
        WHERE @nombre = '' or U.nombre like concat(@nombre, '%')
    END
	IF @indicador = 'CONSULTAR_X_ID'
    BEGIN
        SELECT 
            M.id_usuario,
            U.nombre, 
            U.apellido, 
            U.correo, 
			U.contraseña,
            U.telefono,
			U.foto_perfil,
			U.rol,
			M.id_especialidad,
            E.nombre as especialidad
        FROM Medicos M
        INNER JOIN Usuarios U ON M.id_usuario = U.id_usuario
        INNER JOIN Especialidades E ON M.id_especialidad = E.id_especialidad
		WHERE M.id_usuario = @id_usuario
    END
END
GO

-- =============================================
-- PROCEDIMIENTO: usp_especialidad_crud
-- DESCRIPCIÓN : CRUD para la tabla Especialidades
-- PARÁMETROS  : indicador de acción, datos de Especialidades
-- =============================================
CREATE OR ALTER PROC usp_especialidad_crud
@indicador VARCHAR(20),
@id_especialidad INT = NULL,
@nombre VARCHAR(100) = NULL,
@descripcion VARCHAR(255) = NULL
AS
BEGIN
	IF @indicador = 'CONSULTAR_TODO'
    BEGIN
        SELECT * FROM  Especialidades
	END
	IF @indicador = 'CONSULTAR_X_ID'
    BEGIN
        SELECT * FROM  Especialidades
		WHERE id_especialidad = @id_especialidad
	END
	IF @indicador = 'INSERTAR'
    BEGIN
        INSERT INTO Especialidades (nombre, descripcion)
        VALUES (@nombre, @descripcion)
    END
	IF @indicador = 'ACTUALIZAR'
    BEGIN
        UPDATE Especialidades
        SET nombre = @nombre,
            descripcion = @descripcion
        WHERE id_especialidad = @id_especialidad;
    END
	IF @indicador = 'ELIMINAR'
    BEGIN
        DELETE FROM Especialidades WHERE id_especialidad = @id_especialidad;
    END
END
GO

-- =============================================
-- PROCEDIMIENTO: usp_citas_crud
-- DESCRIPCIÓN : CRUD para la tabla Citas
-- PARÁMETROS  : Indicador, Datos de las citas
-- =============================================
CREATE OR ALTER PROCEDURE usp_citas_crud
    @indicador VARCHAR(50),
    @id_cita INT = NULL,
    @id_medico INT = NULL,
	@id_paciente INT = NULL,
	@id_especialidad INT = NULL,
    @fecha Date = NULL,
    @hora TIME(7) = NULL,
	@tipo_consulta VARCHAR(20) = NULL,
    @sintomas TEXT = NULL,
    @estado VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @indicador = 'INSERTAR'
    BEGIN
        INSERT INTO Citas(id_medico, id_paciente, id_especialidad, fecha, hora, tipo_consulta,sintomas, estado)
        VALUES (@id_medico, @id_paciente, @id_especialidad, @fecha, @hora, @tipo_consulta,@sintomas, @estado);
    END

    IF @indicador = 'ACTUALIZAR'
    BEGIN
        UPDATE Citas
        SET id_medico = @id_medico,
            id_paciente = @id_paciente,
            id_especialidad = @id_especialidad,
            fecha = @fecha,
            hora = @hora,
            tipo_consulta = @tipo_consulta,
            sintomas = @sintomas,
			estado = @estado
        WHERE id_cita = @id_cita;
    END

    ELSE IF @indicador = 'ELIMINAR'
    BEGIN
        UPDATE Citas
		SET estado = 'cancelada'
        WHERE id_cita = @id_cita
    END
    ELSE IF @indicador = 'CONSULTAR_TODO'
	BEGIN
    SELECT 
        c.id_cita,
        c.id_medico,
        u_m.nombre + ' ' + u_m.apellido,
        c.id_paciente,
        u_p.nombre + ' ' + u_p.apellido,
        c.id_especialidad,
        e.nombre,
        c.fecha,
        c.hora,
        c.tipo_consulta,
        c.sintomas,
        c.estado
    FROM Citas c
    INNER JOIN Usuarios u_m ON c.id_medico = u_m.id_usuario
    INNER JOIN Usuarios u_p ON c.id_paciente = u_p.id_usuario
    INNER JOIN Especialidades e ON c.id_especialidad = e.id_especialidad;
	END
	ELSE IF @indicador = 'CONSULTAR_TODO_PENDIENTE'
    BEGIN
        SELECT 
        c.id_cita,
        c.id_medico,
        u_m.nombre + ' ' + u_m.apellido,
        c.id_paciente,
        u_p.nombre + ' ' + u_p.apellido,
        c.id_especialidad,
        e.nombre,
        c.fecha,
        c.hora,
        c.tipo_consulta,
        c.sintomas,
        c.estado
    FROM Citas c
    INNER JOIN Usuarios u_m ON c.id_medico = u_m.id_usuario
    INNER JOIN Usuarios u_p ON c.id_paciente = u_p.id_usuario
    INNER JOIN Especialidades e ON c.id_especialidad = e.id_especialidad
        WHERE c.estado = 'pendiente';
    END
	ELSE IF @indicador = 'CONSULTAR_TODO_CONFIRMADA'
    BEGIN
        SELECT 
        c.id_cita,
        c.id_medico,
        u_m.nombre + ' ' + u_m.apellido,
        c.id_paciente,
        u_p.nombre + ' ' + u_p.apellido,
        c.id_especialidad,
        e.nombre,
        c.fecha,
        c.hora,
        c.tipo_consulta,
        c.sintomas,
        c.estado
    FROM Citas c
    INNER JOIN Usuarios u_m ON c.id_medico = u_m.id_usuario
    INNER JOIN Usuarios u_p ON c.id_paciente = u_p.id_usuario
    INNER JOIN Especialidades e ON c.id_especialidad = e.id_especialidad
        WHERE c.estado = 'confirmada';
    END
	ELSE IF @indicador = 'CONSULTAR_TODOXID'
	BEGIN
    SELECT 
        c.id_cita,
        c.id_medico,
        u_m.nombre + ' ' + u_m.apellido,
        c.id_paciente,
        u_p.nombre + ' ' + u_p.apellido,
        c.id_especialidad,
        e.nombre,
        c.fecha,
        c.hora,
        c.tipo_consulta,
        c.sintomas,
        c.estado
    FROM Citas c
    INNER JOIN Usuarios u_m ON c.id_medico = u_m.id_usuario
    INNER JOIN Usuarios u_p ON c.id_paciente = u_p.id_usuario
    INNER JOIN Especialidades e ON c.id_especialidad = e.id_especialidad
		WHERE c.id_cita = @id_cita
	END
	ELSE IF @indicador = 'COMBO_TIPO_CONSULTA'
    BEGIN
        SELECT * FROM Citas
        WHERE tipo_consulta = @tipo_consulta;
    END
END;
GO
CREATE or ALTER PROCEDURE usp_RegistrarPaciente
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100),
    @Correo VARCHAR(100),
    @Contraseña VARCHAR(100),
    @Telefono VARCHAR(20) = NULL,
    @FotoPerfil VARCHAR(255) = NULL,
    @FechaNacimiento DATE,
    @GrupoSanguineo VARCHAR(3),
    @IdUsuario INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Insertar en Usuarios
        INSERT INTO Usuarios (nombre, apellido, correo, contraseña, telefono, rol, foto_perfil)
        VALUES (@Nombre, @Apellido, @Correo, @Contraseña, @Telefono, 'pacientes', @FotoPerfil);

        -- Obtener el ID generado
        SET @IdUsuario = SCOPE_IDENTITY();

        -- 2. Insertar en Pacientes
        INSERT INTO Pacientes (id_usuario, fecha_nacimiento, grupo_sanguineo)
        VALUES (@IdUsuario, @FechaNacimiento, @GrupoSanguineo);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE usp_ListarPacientes
AS
BEGIN
    SELECT 
        u.id_usuario,
        u.nombre,
        u.apellido,
        u.correo,
        u.contraseña,
        u.telefono,
        u.foto_perfil,
        p.fecha_nacimiento,
        p.grupo_sanguineo
    FROM Usuarios u
    INNER JOIN Pacientes p ON u.id_usuario = p.id_usuario
    WHERE u.rol = 'pacientes'
END;
GO