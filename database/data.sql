USE MedicalAppointmentsDB;
GO

-- Insertar especialidad
INSERT INTO Specialties (name, description)
VALUES ('Medicina General', 'Atención médica general');
GO

-- Insertar usuario: Admin
DECLARE @admin_id INT;
INSERT INTO Users (first_name, last_name, email, password, phone)
VALUES ('Admin', 'Test', 'admin@test.com', 'admin123', '900000001');
SET @admin_id = SCOPE_IDENTITY();

INSERT INTO UserRoles (user_id, role) VALUES (@admin_id, 'administrador');
GO

-- Insertar usuario: Médico
DECLARE @medico_id INT;
INSERT INTO Users (first_name, last_name, email, password, phone)
VALUES ('Carlos', 'Médico', 'medico@test.com', 'medico123', '900000002');
SET @medico_id = SCOPE_IDENTITY();

INSERT INTO UserRoles (user_id, role) VALUES (@medico_id, 'medico');
INSERT INTO Doctors (user_id, specialty_id) VALUES (@medico_id, 1);
GO

-- Insertar usuario: Paciente
DECLARE @paciente_id INT;
INSERT INTO Users (first_name, last_name, email, password, phone)
VALUES ('Lucía', 'Paciente', 'paciente@test.com', 'paciente123', '900000003');
SET @paciente_id = SCOPE_IDENTITY();

INSERT INTO UserRoles (user_id, role) VALUES (@paciente_id, 'paciente');
INSERT INTO Patients (user_id, birth_date, blood_type)
VALUES (@paciente_id, '1995-04-20', 'O+');
GO

-- Insertar usuario: Medico y Paciente
DECLARE @medico_paciente_id INT;
INSERT INTO Users (first_name, last_name, email, password, phone)
VALUES ('Ana', 'Lopez', 'dual@test.com', 'dual123', '900000004');
SET @medico_paciente_id = SCOPE_IDENTITY();

INSERT INTO UserRoles (user_id, role) VALUES (@medico_paciente_id, 'medico');
INSERT INTO UserRoles (user_id, role) VALUES (@medico_paciente_id, 'paciente');

INSERT INTO Doctors (user_id, specialty_id)
VALUES (@medico_paciente_id, 1);

INSERT INTO Patients (user_id, birth_date, blood_type)
VALUES (@medico_paciente_id, '1990-07-15', 'A+');
GO