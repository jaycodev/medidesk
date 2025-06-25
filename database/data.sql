USE MedicalAppointmentsDB;

-- 1. Insertar especialidad
INSERT INTO Specialties (name, description)
VALUES ('Medicina General', 'Atención médica general');

-- 2. Insertar usuarios

-- Admin
DECLARE @admin_id INT;
INSERT INTO Users (first_name, last_name, email, password, phone)
VALUES ('Admin', 'Test', 'admin@test.com', 'admin123', '900000001');
SET @admin_id = SCOPE_IDENTITY();
INSERT INTO UserRoles (user_id, role) VALUES (@admin_id, 'administrador');

-- Médico
DECLARE @medico_id INT;
INSERT INTO Users (first_name, last_name, email, password, phone)
VALUES ('Carlos', 'Médico', 'medico@test.com', 'medico123', '900000002');
SET @medico_id = SCOPE_IDENTITY();
INSERT INTO UserRoles (user_id, role) VALUES (@medico_id, 'medico');
INSERT INTO Doctors (user_id, specialty_id) VALUES (@medico_id, 1);

-- Paciente
DECLARE @paciente_id INT;
INSERT INTO Users (first_name, last_name, email, password, phone)
VALUES ('Lucía', 'Paciente', 'paciente@test.com', 'paciente123', '900000003');
SET @paciente_id = SCOPE_IDENTITY();
INSERT INTO UserRoles (user_id, role) VALUES (@paciente_id, 'paciente');
INSERT INTO Patients (user_id, birth_date, blood_type)
VALUES (@paciente_id, '1995-04-20', 'O+');

-- Médico y Paciente
DECLARE @dual_id INT;
INSERT INTO Users (first_name, last_name, email, password, phone)
VALUES ('Ana', 'Lopez', 'dual@test.com', 'dual123', '900000004');
SET @dual_id = SCOPE_IDENTITY();
INSERT INTO UserRoles (user_id, role) VALUES (@dual_id, 'medico');
INSERT INTO UserRoles (user_id, role) VALUES (@dual_id, 'paciente');
INSERT INTO Doctors (user_id, specialty_id) VALUES (@dual_id, 1);
INSERT INTO Patients (user_id, birth_date, blood_type)
VALUES (@dual_id, '1990-07-15', 'A+');

-- 3. Insertar citas
INSERT INTO Appointments (doctor_id, patient_id, specialty_id, date, time, consultation_type, symptoms, status)
VALUES 
-- Citas pendientes
(@medico_id, @paciente_id, 1, '2025-06-25', '09:00:00', 'consulta', 'Dolor de cabeza', 'pendiente'),
(@medico_id, @dual_id, 1, '2025-06-26', '10:00:00', 'examen', 'Revisión anual', 'pendiente'),

-- Citas confirmadas
(@medico_id, @paciente_id, 1, '2025-06-20', '11:00:00', 'operacion', 'Cirugía menor', 'confirmada'),
(@dual_id, @paciente_id, 1, '2025-06-22', '15:00:00', 'consulta', 'Consulta especializada', 'confirmada'),

-- Cita cancelada
(@medico_id, @paciente_id, 1, '2025-06-10', '12:00:00', 'examen', 'Análisis de sangre', 'cancelada'),

-- Cita atendida
(@medico_id, @dual_id, 1, '2025-06-15', '13:00:00', 'consulta', 'Dolor muscular', 'atendida');

-- 4. Insertar horarios
INSERT INTO Schedules (day_work_shift, doctor_id, weekday, start_time, end_time)
VALUES 
('mañana', @medico_id, 'lunes', '08:00:00', '12:00:00'),
('tarde', @medico_id, 'lunes', '14:00:00', '18:00:00'),
('mañana', @dual_id, 'martes', '08:00:00', '12:00:00'),
('tarde', @dual_id, 'miércoles', '14:00:00', '18:00:00');