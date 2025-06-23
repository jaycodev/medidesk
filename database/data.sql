USE MedicalAppointmentsDB;
GO

INSERT INTO Specialties (name, description)
VALUES ('Medicina General', 'Atención médica general');

INSERT INTO Users (first_name, last_name, email, password, phone, role)
VALUES ('Admin', 'Test', 'admin@test.com', 'admin123', '900000001', 'administrador');

INSERT INTO Users (first_name, last_name, email, password, phone, role)
VALUES ('Carlos', 'Médico', 'medico@test.com', 'medico123', '900000002', 'medico');
INSERT INTO Doctors (user_id, specialty_id)
VALUES (2, 1);

INSERT INTO Users (first_name, last_name, email, password, phone, role)
VALUES ('Lucía', 'Paciente', 'paciente@test.com', 'paciente123', '900000003', 'paciente');
INSERT INTO Patients (user_id, birth_date, blood_type)
VALUES (3, '1995-04-20', 'O+');