USE MedicalAppointmentsDB;
GO

-- =============================================
-- PROCEDURE: User_CRUD
-- DESCRIPTION: CRUD for the Users table
-- PARAMETERS : action indicator, user data
-- =============================================
CREATE OR ALTER PROCEDURE User_CRUD
    @indicator VARCHAR(50),
    @user_id INT = NULL,
    @first_name VARCHAR(100) = NULL,
    @last_name VARCHAR(100) = NULL,
    @email VARCHAR(100) = NULL,
    @password VARCHAR(100) = NULL,
    @phone VARCHAR(20) = NULL,
    @role VARCHAR(20) = NULL,
    @profile_picture VARCHAR(255) = NULL
AS
BEGIN
	IF @indicator = 'INSERT'
    BEGIN
        SET NOCOUNT ON;

        IF EXISTS (SELECT 1 FROM Users WHERE email = @email)
        BEGIN
            RAISERROR('Este correo ya está registrado. Por favor, ingrese otro.', 16, 1);
            RETURN;
        END

        INSERT INTO Users (first_name, last_name, email, password, phone, role, profile_picture)
        VALUES (@first_name, @last_name, @email, @password, @phone, @role, @profile_picture);

		RETURN;
    END

    ELSE IF @indicator = 'UPDATE'
    BEGIN
        UPDATE Users
        SET first_name = @first_name,
            last_name = @last_name,
            email = @email,
            password = @password,
            phone = @phone,
            role = @role,
            profile_picture = @profile_picture
        WHERE user_id = @user_id;

		RETURN;
    END

    ELSE IF @indicator = 'DELETE'
    BEGIN
        DELETE FROM Users WHERE user_id = @user_id;

		RETURN;
    END

    ELSE IF @indicator = 'GET_BY_ID'
    BEGIN
	    SELECT 
            user_id,
            first_name,
            last_name,
            email,
            phone,
            role,
            profile_picture
        FROM Users 
		WHERE user_id = @user_id;

		RETURN;
    END

    ELSE IF @indicator = 'GET_ALL'
    BEGIN
        SELECT 
            user_id,
            first_name,
            last_name,
            email,
            phone,
            role,
            profile_picture
        FROM Users;

		RETURN;
    END

    ELSE IF @indicator = 'LOGIN'
    BEGIN
        SELECT 
            user_id,
            first_name,
            last_name,
            email,
            phone,
            role,
            profile_picture
        FROM Users 
        WHERE email = @email AND password = @password;

		RETURN;
    END

	ELSE
    BEGIN
        RAISERROR('Acción no válida: %s', 16, 1, @indicator);
		RETURN;
    END
END;
GO

-- =============================================
-- PROCEDURE: Specialty_CRUD
-- DESCRIPTION: CRUD for the Specialties table
-- PARAMETERS : action indicator, specialty data
-- =============================================
CREATE OR ALTER PROCEDURE Specialty_CRUD
    @indicator VARCHAR(20),
    @specialty_id INT = NULL,
    @name VARCHAR(100) = NULL,
    @description VARCHAR(255) = NULL
AS
BEGIN
    IF @indicator = 'GET_ALL'
    BEGIN
        SELECT specialty_id, name, description FROM Specialties;

		RETURN;
    END

    ELSE IF @indicator = 'GET_BY_ID'
    BEGIN
        SELECT specialty_id, name, description FROM Specialties 
        WHERE specialty_id = @specialty_id;

		RETURN;
    END

    ELSE IF @indicator = 'INSERT'
    BEGIN
        INSERT INTO Specialties (name, description)
        VALUES (@name, @description);

		RETURN;
    END

    ELSE IF @indicator = 'UPDATE'
    BEGIN
        UPDATE Specialties
        SET name = @name,
            description = @description
        WHERE specialty_id = @specialty_id;

		RETURN;
    END

	ELSE
    BEGIN
        RAISERROR('Acción no válida: %s', 16, 1, @indicator);
		RETURN;
    END
END;
GO

-- =============================================
-- PROCEDURE: Doctor_CRUD
-- DESCRIPTION: CRUD for the Doctors table
-- PARAMETERS : action indicator, doctor data
-- =============================================
CREATE OR ALTER PROCEDURE Doctor_CRUD
    @indicator VARCHAR(20),
    @user_id INT = NULL,
    @first_name VARCHAR(100) = NULL,
    @last_name VARCHAR(100) = NULL,
    @email VARCHAR(100) = NULL,
    @password VARCHAR(100) = NULL,
    @phone VARCHAR(20) = NULL,
    @specialty_id INT = NULL,
    @profile_picture VARCHAR(255) = NULL,
    @role VARCHAR(20) = 'medico',
    @status BIT = 1
AS
BEGIN
	IF @indicator = 'INSERT'
    BEGIN
        SET NOCOUNT ON;

        IF EXISTS (SELECT 1 FROM Users WHERE email = @email)
        BEGIN
            RAISERROR('Este correo ya está registrado. Por favor, ingrese otro.', 16, 1);
            RETURN;
        END

        BEGIN TRY
            BEGIN TRANSACTION;

            INSERT INTO Users (first_name, last_name, email, password, phone, profile_picture, role)
            VALUES (@first_name, @last_name, @email, @password, @phone, @profile_picture, @role);

            SET @user_id = SCOPE_IDENTITY();

            INSERT INTO Doctors (user_id, specialty_id, status)
            VALUES (@user_id, @specialty_id, @status);

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH
		
		RETURN;
    END

    ELSE IF @indicator = 'UPDATE'
    BEGIN
        UPDATE Doctors
        SET specialty_id = @specialty_id,
            status = @status
        WHERE user_id = @user_id;

		RETURN;
    END

    ELSE IF @indicator = 'GET_ALL'
    BEGIN
        SELECT 
            D.user_id,
            U.first_name,
            U.last_name,
            U.email,
            U.profile_picture,
            S.name AS specialty_name,
            D.status
        FROM Doctors D
        INNER JOIN Users U ON D.user_id = U.user_id
        INNER JOIN Specialties S ON D.specialty_id = S.specialty_id
        WHERE U.role = @role;

		RETURN;
    END

    ELSE IF @indicator = 'GET_BY_ID'
    BEGIN
        SELECT 
            D.user_id,
            U.first_name,
            U.last_name,
            U.email,
            U.phone,
            U.profile_picture,
			D.specialty_id,
            S.name AS specialty_name,
            D.status
        FROM Doctors D
        INNER JOIN Users U ON D.user_id = U.user_id
        INNER JOIN Specialties S ON D.specialty_id = S.specialty_id
        WHERE D.user_id = @user_id AND U.role = @role;

		RETURN;
    END

	ELSE IF @indicator = 'GET_DETAILS_BY_ID'
	BEGIN
		SELECT S.name AS specialty_name
		FROM Doctors D
		INNER JOIN Specialties S ON D.specialty_id = S.specialty_id
		WHERE D.user_id = @user_id;

		RETURN;
	END

	IF @indicator = 'GET_BY_SPECIALTY'
    BEGIN
        SELECT 
            d.user_id,
            u.first_name,
            u.last_name,
            d.specialty_id
        FROM Doctors d
        INNER JOIN Users u ON d.user_id = u.user_id
        WHERE d.specialty_id = @specialty_id AND u.role = 'medico' AND d.status = 1;

		RETURN;
    END

	ELSE
    BEGIN
        RAISERROR('Acción no válida: %s', 16, 1, @indicator);
		RETURN;
    END
END;
GO

-- =============================================
-- PROCEDURE: Patient_CRUD
-- DESCRIPTION: CRUD for the Patients table
-- PARAMETERS : action indicator, patient data
-- =============================================
CREATE OR ALTER PROCEDURE Patient_CRUD
    @indicator VARCHAR(20),
    @user_id INT = NULL,
    @first_name VARCHAR(100) = NULL,
    @last_name VARCHAR(100) = NULL,
    @email VARCHAR(100) = NULL,
    @password VARCHAR(100) = NULL,
    @phone VARCHAR(20) = NULL,
    @role VARCHAR(20) = 'paciente',
    @profile_picture VARCHAR(255) = NULL,
    @birth_date DATE = NULL,
    @blood_type VARCHAR(3) = NULL
AS
BEGIN
    IF @indicator = 'INSERT'
    BEGIN
        SET NOCOUNT ON;

        IF EXISTS (SELECT 1 FROM Users WHERE email = @email)
        BEGIN
            RAISERROR('Este correo ya está registrado. Por favor, ingrese otro.', 16, 1);
            RETURN;
        END

        BEGIN TRY
            BEGIN TRANSACTION;

            INSERT INTO Users (first_name, last_name, email, password, phone, role, profile_picture)
            VALUES (@first_name, @last_name, @email, @password, @phone, @role, @profile_picture);

            SET @user_id = SCOPE_IDENTITY();

            INSERT INTO Patients (user_id, birth_date, blood_type)
            VALUES (@user_id, @birth_date, @blood_type);

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH

		RETURN;
    END

    ELSE IF @indicator = 'UPDATE'
    BEGIN
        UPDATE Patients
        SET birth_date = @birth_date,
            blood_type = @blood_type
        WHERE user_id = @user_id;

		RETURN;
    END

    ELSE IF @indicator = 'GET_ALL'
    BEGIN
        SELECT 
            P.user_id,
            U.first_name,
            U.last_name,
            U.email,
            U.profile_picture,
            P.birth_date,
            P.blood_type
        FROM Patients P
        INNER JOIN Users U ON P.user_id = U.user_id
        WHERE role = @role;

		RETURN;
    END

    ELSE IF @indicator = 'GET_BY_ID'
    BEGIN
        SELECT 
            P.user_id,
            U.first_name,
            U.last_name,
            U.email,
            U.phone,
            U.profile_picture,
            P.birth_date,
            P.blood_type
        FROM Patients P
        INNER JOIN Users U ON P.user_id = U.user_id
        WHERE P.user_id = @user_id AND role = @role;

		RETURN;
    END

	ELSE IF @indicator = 'GET_DETAILS_BY_ID'
    BEGIN
        SELECT birth_date, blood_type FROM Patients WHERE user_id = @user_id;

		RETURN;
    END

	ELSE
    BEGIN
        RAISERROR('Acción no válida: %s', 16, 1, @indicator);
		RETURN;
    END
END;
GO

-- =============================================
-- PROCEDURE: Appointment_CRUD
-- DESCRIPTION: CRUD for the Appointments table
-- PARAMETERS : action indicator, appointment data
-- =============================================
CREATE OR ALTER PROCEDURE Appointment_CRUD
    @indicator VARCHAR(50),
    @appointment_id INT = NULL,
    @doctor_id INT = NULL,
    @patient_id INT = NULL,
    @specialty_id INT = NULL,
    @date DATE = NULL,
    @time TIME(7) = NULL,
    @consultation_type VARCHAR(20) = NULL,
    @symptoms TEXT = NULL,
    @status VARCHAR(20) = NULL,
    @user_id INT = NULL,
    @user_type VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @indicator = 'INSERT'
    BEGIN
        INSERT INTO Appointments(doctor_id, patient_id, specialty_id, date, time, consultation_type, symptoms, status)
        VALUES (@doctor_id, @patient_id, @specialty_id, @date, @time, @consultation_type, @symptoms, @status);

		SELECT SCOPE_IDENTITY() AS appointment_id;

		RETURN;
    END

	ELSE IF @indicator = 'GET_BY_DOCTOR_AND_DATE'
	BEGIN
		SELECT time
		FROM Appointments
		WHERE doctor_id = @doctor_id AND date = @date AND status != 'cancelada';

		RETURN;
	END

	ELSE IF @indicator = 'GET_SCHEDULE_BY_DOCTOR_AND_DAY'
	BEGIN
		SET LANGUAGE Spanish;

		DECLARE @weekday VARCHAR(10)
		SET @weekday = LOWER(DATENAME(weekday, @date))

		SELECT start_time, end_time
		FROM Schedules
		WHERE doctor_id = @doctor_id AND weekday = @weekday;

		RETURN;
	END

	ELSE IF @indicator = 'CANCEL'
	BEGIN
		UPDATE Appointments
		SET status = 'cancelada'
		WHERE appointment_id = @appointment_id AND status != 'cancelada';

		SELECT @@ROWCOUNT AS affected_rows;

		RETURN;
	END

    ELSE IF @indicator = 'GET_BY_ID'
    BEGIN
        SELECT 
            a.appointment_id,
            u_d.first_name + ' ' + u_d.last_name AS doctor_name,
            u_p.first_name + ' ' + u_p.last_name AS patient_name,
            s.name AS specialty_name,
            a.date,
            a.time,
            a.consultation_type,
            a.symptoms,
            a.status
        FROM Appointments a
        INNER JOIN Users u_d ON a.doctor_id = u_d.user_id
        INNER JOIN Users u_p ON a.patient_id = u_p.user_id
        INNER JOIN Specialties s ON a.specialty_id = s.specialty_id
        WHERE a.appointment_id = @appointment_id;

		RETURN;
    END

    ELSE IF @indicator = 'GET_BY_USER_AND_STATUS'
    BEGIN
        SELECT 
            a.appointment_id,
            a.doctor_id,
            u_d.first_name + ' ' + u_d.last_name AS doctor_name,
            a.patient_id,
            u_p.first_name + ' ' + u_p.last_name AS patient_name,
            a.specialty_id,
            s.name AS specialty_name,
            a.date,
            a.time,
            a.consultation_type,
            a.symptoms,
            a.status
        FROM Appointments a
        INNER JOIN Users u_d ON a.doctor_id = u_d.user_id
        INNER JOIN Users u_p ON a.patient_id = u_p.user_id
        INNER JOIN Specialties s ON a.specialty_id = s.specialty_id
        WHERE 
            (
                (@user_type = 'paciente' AND a.patient_id = @user_id)
                OR
                (@user_type = 'medico' AND a.doctor_id = @user_id)
            )
            AND
            (
                @status IS NULL OR @status = '' OR a.status = @status
            );

		RETURN;
    END

    ELSE IF @indicator = 'GET_COMPLETED_OR_CANCELLED_BY_USER'
    BEGIN
        SELECT 
            a.appointment_id,
            a.doctor_id,
            u_d.first_name + ' ' + u_d.last_name AS doctor_name,
            a.patient_id,
            u_p.first_name + ' ' + u_p.last_name AS patient_name,
            a.specialty_id,
            s.name AS specialty_name,
            a.date,
            a.time,
            a.consultation_type,
            a.symptoms,
            a.status
        FROM Appointments a
        INNER JOIN Users u_d ON a.doctor_id = u_d.user_id
        INNER JOIN Users u_p ON a.patient_id = u_p.user_id
        INNER JOIN Specialties s ON a.specialty_id = s.specialty_id
        WHERE 
            (
                (@user_type = 'paciente' AND a.patient_id = @user_id)
                OR
                (@user_type = 'medico' AND a.doctor_id = @user_id)
            )
            AND a.status IN ('cancelada', 'atendida');

		RETURN;
    END

	ELSE
    BEGIN
        RAISERROR('Acción no válida: %s', 16, 1, @indicator);
		RETURN;
    END
END;
GO
-- =============================================
-- PROCEDURE: Schedule_CRUD
-- DESCRIPTION: CRUD for the updated Schedules table
-- PARAMETERS : action indicator, schedule data
-- =============================================
CREATE OR ALTER PROCEDURE Schedule_CRUD
    @indicator VARCHAR(50),
    @doctor_id INT = NULL,
    @weekday VARCHAR(10) = NULL,
    @day_work_shift VARCHAR(10) = NULL,
    @start_time TIME = NULL,
    @end_time TIME = NULL,
    @enabled BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @indicator = 'INSERT_OR_UPDATE'
    BEGIN
        -- Si enabled = 0, se elimina
        IF @enabled = 0
        BEGIN
            DELETE FROM Schedules
            WHERE doctor_id = @doctor_id 
              AND weekday = @weekday 
              AND day_work_shift = @day_work_shift;
        END
        ELSE
        BEGIN
            MERGE INTO Schedules AS target
            USING (
                SELECT @doctor_id AS doctor_id, @weekday AS weekday, @day_work_shift AS day_work_shift
            ) AS source
            ON target.doctor_id = source.doctor_id 
               AND target.weekday = source.weekday 
               AND target.day_work_shift = source.day_work_shift
            WHEN MATCHED THEN
                UPDATE SET 
                    start_time = @start_time,
                    end_time = @end_time
            WHEN NOT MATCHED THEN
                INSERT (doctor_id, weekday, day_work_shift, start_time, end_time)
                VALUES (@doctor_id, @weekday, @day_work_shift, @start_time, @end_time);
        END

        RETURN;
    END

    ELSE IF @indicator = 'DELETE'
    BEGIN
        DELETE FROM Schedules
        WHERE doctor_id = @doctor_id AND weekday = @weekday AND day_work_shift = @day_work_shift;
        RETURN;
    END

    ELSE IF @indicator = 'GET_ALL'
    BEGIN
        SELECT * FROM Schedules;
        RETURN;
    END

    ELSE IF @indicator = 'GET_BY_DOCTOR'
    BEGIN
        SELECT * FROM Schedules
        WHERE doctor_id = @doctor_id;
        RETURN;
    END

    ELSE IF @indicator = 'GET_BY_WEEKDAY'
    BEGIN
        SELECT * FROM Schedules
        WHERE weekday = @weekday;
        RETURN;
    END

    ELSE IF @indicator = 'GET_BY_DOCTOR_AND_DAY'
    BEGIN
        SELECT * FROM Schedules
        WHERE doctor_id = @doctor_id AND weekday = @weekday;
        RETURN;
    END

    ELSE
    BEGIN
        RAISERROR('Acción no válida: %s', 16, 1, @indicator);
        RETURN;
    END
END;
GO
