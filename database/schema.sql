CREATE DATABASE MedicalAppointmentsDB;
GO

USE MedicalAppointmentsDB;
GO

CREATE TABLE Users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    role VARCHAR(20) NOT NULL CHECK (role IN ('administrador', 'medico', 'paciente')),
    profile_picture VARCHAR(255)
);
GO

CREATE TABLE Specialties (
    specialty_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(100) NOT NULL,
    description VARCHAR(255)
);
GO

CREATE TABLE Doctors (
    user_id INT PRIMARY KEY,
    specialty_id INT NOT NULL,
    status BIT NOT NULL DEFAULT(1),
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    FOREIGN KEY (specialty_id) REFERENCES Specialties(specialty_id)
);
GO

CREATE TABLE Patients (
    user_id INT PRIMARY KEY,
    birth_date DATE NOT NULL,
    blood_type VARCHAR(3) NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    CONSTRAINT CHK_ValidBloodType CHECK (
        blood_type IS NULL OR 
        blood_type IN ('A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-')
    )
);
GO

CREATE TABLE Appointments (
    appointment_id INT PRIMARY KEY IDENTITY(1,1),
    doctor_id INT NOT NULL,
    patient_id INT NOT NULL,
    specialty_id INT NOT NULL,
    date DATE NOT NULL,
    time TIME NOT NULL,
    consultation_type VARCHAR(20) NOT NULL CHECK (consultation_type IN ('examen', 'consulta', 'operacion')),
    symptoms TEXT,
    status VARCHAR(20) DEFAULT 'pendiente' CHECK (status IN ('pendiente', 'confirmada', 'cancelada', 'atendida')),
    FOREIGN KEY (doctor_id) REFERENCES Doctors(user_id),
    FOREIGN KEY (patient_id) REFERENCES Patients(user_id),
    FOREIGN KEY (specialty_id) REFERENCES Specialties(specialty_id)
);
GO

CREATE TABLE Schedules (
    schedule_id INT PRIMARY KEY IDENTITY(1,1),
    doctor_id INT NOT NULL,
    weekday VARCHAR(10) NOT NULL CHECK (weekday IN ('lunes', 'martes', 'miércoles', 'jueves', 'viernes', 'sábado', 'domingo')),
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    FOREIGN KEY (doctor_id) REFERENCES Doctors(user_id),
    CONSTRAINT UQ_Doctor_Weekday UNIQUE (doctor_id, weekday)
);
GO