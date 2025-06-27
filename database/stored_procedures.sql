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
	@current_password VARCHAR(100) = NULL,
    @phone VARCHAR(20) = NULL,
    @roles VARCHAR(100) = NULL,
    @profile_picture VARCHAR(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @indicator = 'INSERT'
	BEGIN
		IF EXISTS (SELECT 1 FROM Users WHERE email = @email)
		BEGIN
			RAISERROR('El correo ya está registrado. Por favor, ingrese otro.', 16, 1);
			RETURN;
		END

		BEGIN TRY
			BEGIN TRANSACTION;

			INSERT INTO Users (first_name, last_name, email, password, phone, profile_picture)
			VALUES (@first_name, @last_name, @email, @password, @phone, @profile_picture);

			SET @user_id = SCOPE_IDENTITY();

			INSERT INTO UserRoles (user_id, role)
			VALUES (@user_id, 'administrador');

			COMMIT TRANSACTION;

			SELECT 1 AS affected_rows;
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;

			THROW;
		END CATCH

		RETURN;
	END

	ELSE IF @indicator = 'UPDATE'
	BEGIN
		IF EXISTS (
			SELECT 1 FROM Users 
			WHERE email = @email AND user_id <> @user_id
		)
		BEGIN
			RAISERROR('El correo ya está registrado. Por favor, ingrese otro.', 16, 1);
			RETURN;
		END

		BEGIN TRY
			BEGIN TRANSACTION;

			UPDATE Users
			SET first_name = @first_name,
				last_name = @last_name,
				email = @email,
				phone = @phone,
				profile_picture = @profile_picture
			WHERE user_id = @user_id;

			DECLARE @rows INT = @@ROWCOUNT;

			DELETE FROM UserRoles WHERE user_id = @user_id;

			DECLARE @role_cleaned NVARCHAR(100) = REPLACE(@roles, ' ', '');
			DECLARE @next_role NVARCHAR(20);
			DECLARE @pos INT = 1;
			DECLARE @len INT = LEN(@role_cleaned) + 1;
			DECLARE @comma INT;

			WHILE @pos < @len
			BEGIN
				SET @comma = CHARINDEX(',', @role_cleaned, @pos);
				IF @comma = 0 SET @comma = @len;

				SET @next_role = SUBSTRING(@role_cleaned, @pos, @comma - @pos);

				IF @next_role IN ('administrador', 'medico', 'paciente')
				BEGIN
					INSERT INTO UserRoles (user_id, role)
					VALUES (@user_id, @next_role);
				END

				SET @pos = @comma + 1;
			END

			COMMIT TRANSACTION;

			SELECT @rows AS affected_rows;
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
			THROW;
		END CATCH

		RETURN;
	END

	ELSE IF @indicator = 'DELETE'
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION;

			DELETE FROM UserRoles WHERE user_id = @user_id;
			DELETE FROM Users WHERE user_id = @user_id;

			COMMIT TRANSACTION;

			SELECT @@ROWCOUNT AS affected_rows;
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
			THROW;
		END CATCH

		RETURN;
	END

	ELSE IF @indicator = 'GET_BY_ID'
	BEGIN
		SELECT 
			u.user_id,
			u.first_name,
			u.last_name,
			u.email,
			u.phone,
			STRING_AGG(ur.role, ',') AS roles,
			u.profile_picture
		FROM Users u
		LEFT JOIN UserRoles ur ON u.user_id = ur.user_id
		WHERE u.user_id = @user_id
		GROUP BY 
			u.user_id, u.first_name, u.last_name, u.email, u.phone, u.profile_picture;

		RETURN;
	END

	ELSE IF @indicator = 'GET_ALL'
	BEGIN
		SELECT 
			u.user_id,
			u.first_name,
			u.last_name,
			u.email,
			u.phone,
			STRING_AGG(ur.role, ',') AS roles,
			u.profile_picture,

			CASE 
				WHEN EXISTS (SELECT 1 FROM Doctors d WHERE d.user_id = u.user_id)
					OR EXISTS (SELECT 1 FROM Patients p WHERE p.user_id = u.user_id)
					THEN 0
				ELSE 1
			END AS can_delete

		FROM Users u
		LEFT JOIN UserRoles ur ON u.user_id = ur.user_id
		WHERE u.user_id <> @user_id
		GROUP BY 
			u.user_id, u.first_name, u.last_name, u.email, u.phone, u.profile_picture;

		RETURN;
	END

	ELSE IF @indicator = 'LOGIN'
	BEGIN
		SELECT 
			u.user_id,
			u.first_name,
			u.last_name,
			u.email,
			u.phone,
			STRING_AGG(ur.role, ',') AS roles,
			u.profile_picture
		FROM Users u
		LEFT JOIN UserRoles ur ON u.user_id = ur.user_id
		WHERE 
			u.email COLLATE Latin1_General_CS_AS = @email COLLATE Latin1_General_CS_AS AND
			u.password COLLATE Latin1_General_CS_AS = @password COLLATE Latin1_General_CS_AS
		GROUP BY 
			u.user_id, u.first_name, u.last_name, u.email, u.phone, u.profile_picture;

		RETURN;
	END

	ELSE IF @indicator = 'UPDATE_PROFILE'
	BEGIN
		IF EXISTS (
			SELECT 1 FROM Users 
			WHERE email = @email AND user_id <> @user_id
		)
		BEGIN
			RAISERROR('El correo ya está registrado. Por favor, ingrese otro.', 16, 1);
			RETURN;
		END

		BEGIN TRY
			UPDATE Users
			SET first_name = @first_name,
				last_name = @last_name,
				email = @email,
				phone = @phone
			WHERE user_id = @user_id;

			SELECT @@ROWCOUNT AS affected_rows;
		END TRY
		BEGIN CATCH
			THROW;
		END CATCH

		RETURN;
	END

	ELSE IF @indicator = 'UPDATE_PROFILE_PICTURE'
	BEGIN
		BEGIN TRY
			UPDATE Users
			SET profile_picture = @profile_picture
			WHERE user_id = @user_id;

			SELECT @@ROWCOUNT AS affected_rows;
		END TRY
		BEGIN CATCH
			THROW;
		END CATCH

		RETURN;
	END


	ELSE IF @indicator = 'UPDATE_PASSWORD'
	BEGIN
		BEGIN TRY
			IF EXISTS (
				SELECT 1
				FROM Users
				WHERE user_id = @user_id AND password = @current_password
			)
			BEGIN
				UPDATE Users
				SET password = @password
				WHERE user_id = @user_id;

				SELECT @@ROWCOUNT AS affected_rows;
			END
			ELSE
			BEGIN
				RAISERROR('La contraseña actual no es correcta.', 16, 1);
			END
		END TRY
		BEGIN CATCH
			THROW;
		END CATCH

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
	SET NOCOUNT ON;

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

		SELECT @@ROWCOUNT AS affected_rows;

		RETURN;
	END

	ELSE IF @indicator = 'UPDATE'
	BEGIN
		UPDATE Specialties
		SET name = @name,
			description = @description
		WHERE specialty_id = @specialty_id;

		SELECT @@ROWCOUNT AS affected_rows;

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
    @status BIT = 1
AS
BEGIN
	SET NOCOUNT ON;

	IF @indicator = 'INSERT'
	BEGIN
		IF EXISTS (SELECT 1 FROM Users WHERE email = @email)
		BEGIN
			RAISERROR('El correo ya está registrado. Por favor, ingrese otro.', 16, 1);
			RETURN;
		END

		BEGIN TRY
			BEGIN TRANSACTION;

			INSERT INTO Users (first_name, last_name, email, password, phone, profile_picture)
			VALUES (@first_name, @last_name, @email, @password, @phone, @profile_picture);

			SET @user_id = SCOPE_IDENTITY();

			INSERT INTO Doctors (user_id, specialty_id, status)
			VALUES (@user_id, @specialty_id, @status);

			INSERT INTO UserRoles (user_id, role)
			VALUES (@user_id, 'medico');

			COMMIT TRANSACTION;

			SELECT 1 AS affected_rows;
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
			THROW;
		END CATCH

		RETURN;
	END

	ELSE IF @indicator = 'UPDATE'
	BEGIN
		IF EXISTS (SELECT 1 FROM Doctors WHERE user_id = @user_id)
		BEGIN
			UPDATE Doctors
			SET specialty_id = @specialty_id,
				status = @status
			WHERE user_id = @user_id;
		END
		ELSE
		BEGIN
			INSERT INTO Doctors (user_id, specialty_id, status)
			VALUES (@user_id, @specialty_id, @status);
		END

		SELECT 1 AS affected_rows;
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
		INNER JOIN UserRoles UR ON U.user_id = UR.user_id
		WHERE UR.role = 'medico';

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
		INNER JOIN UserRoles UR ON U.user_id = UR.user_id
		WHERE D.user_id = @user_id AND UR.role = 'medico';

		RETURN;
	END

	ELSE IF @indicator = 'GET_DETAILS_BY_ID'
	BEGIN
		SELECT 
			D.specialty_id,
			S.name AS specialty_name,
			D.status
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
		INNER JOIN UserRoles ur ON u.user_id = ur.user_id
		WHERE d.specialty_id = @specialty_id
		  AND ur.role = 'medico'
		  AND d.status = 1
		  AND d.user_id <> @user_id;

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
    @profile_picture VARCHAR(255) = NULL,
    @birth_date DATE = NULL,
    @blood_type VARCHAR(3) = NULL
AS
BEGIN
    SET NOCOUNT ON;

	IF @indicator = 'INSERT'
	BEGIN
		IF EXISTS (SELECT 1 FROM Users WHERE email = @email)
		BEGIN
			RAISERROR('El correo ya está registrado. Por favor, ingrese otro.', 16, 1);
			RETURN;
		END

		BEGIN TRY
			BEGIN TRANSACTION;

			INSERT INTO Users (first_name, last_name, email, password, phone, profile_picture)
			VALUES (@first_name, @last_name, @email, @password, @phone, @profile_picture);

			SET @user_id = SCOPE_IDENTITY();

			INSERT INTO Patients (user_id, birth_date, blood_type)
			VALUES (@user_id, @birth_date, @blood_type);

			INSERT INTO UserRoles (user_id, role)
			VALUES (@user_id, 'paciente');

			COMMIT TRANSACTION;

			SELECT 1 AS affected_rows;
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
			THROW;
		END CATCH

		RETURN;
	END

	ELSE IF @indicator = 'UPDATE'
	BEGIN
		IF EXISTS (SELECT 1 FROM Patients WHERE user_id = @user_id)
		BEGIN
			UPDATE Patients
			SET birth_date = @birth_date,
				blood_type = @blood_type
			WHERE user_id = @user_id;
		END
		ELSE
		BEGIN
			INSERT INTO Patients (user_id, birth_date, blood_type)
			VALUES (@user_id, @birth_date, @blood_type);
		END

		SELECT 1 AS affected_rows;
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
		INNER JOIN UserRoles UR ON U.user_id = UR.user_id
		WHERE UR.role = 'paciente';

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
		INNER JOIN UserRoles UR ON U.user_id = UR.user_id
		WHERE P.user_id = @user_id AND UR.role = 'paciente';

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
    @user_rol VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE
        @notif_patient_id INT,
        @notif_doctor_id INT,
        @notif_date DATE,
        @notif_time TIME,
        @ampm NVARCHAR(5),
        @formatted_time NVARCHAR(10),
        @formatted_date NVARCHAR(20),
        @message NVARCHAR(300);

	IF @indicator = 'INSERT'
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION;

			INSERT INTO Appointments(doctor_id, patient_id, specialty_id, date, time, consultation_type, symptoms, status)
			VALUES (@doctor_id, @patient_id, @specialty_id, @date, @time, @consultation_type, @symptoms, @status);

			SET @appointment_id = SCOPE_IDENTITY();

			SELECT @notif_time = time FROM Appointments WHERE appointment_id = @appointment_id;

			SET LANGUAGE Spanish;

			SET @ampm = CASE 
				WHEN DATEPART(HOUR, @notif_time) < 12 THEN 'a. m.' 
				ELSE 'p. m.' 
			END;

			SET @formatted_time = FORMAT(@notif_time, 'hh\:mm');
			SET @formatted_date = LOWER(FORMAT(@date, 'dd MMM yyyy', 'es-ES'));

			SET @message = CONCAT(
				'Nueva cita solicitada para el ',
				@formatted_date, ' - ',
				@formatted_time, ' ', @ampm
			);

			INSERT INTO Notifications (DoctorId, PatientId, AppointmentId, Message, IsRead, CreatedAt)
			VALUES (
				@doctor_id,
				@patient_id,
				@appointment_id,
				@message,
				0,
				GETDATE()
			);

			COMMIT TRANSACTION;

			SELECT @appointment_id AS appointment_id, 1 AS affected_rows;
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;

			THROW;
		END CATCH

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

		SELECT day_work_shift, start_time, end_time
		FROM Schedules
		WHERE doctor_id = @doctor_id AND weekday = @weekday;

		RETURN;
	END

	ELSE IF @indicator = 'CANCEL'
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION;

			UPDATE Appointments
			SET status = 'cancelada'
			WHERE appointment_id = @appointment_id
				AND status NOT IN ('cancelada', 'atendida');

			IF @@ROWCOUNT = 0
			BEGIN
				ROLLBACK TRANSACTION;
				SELECT 0 AS affected_rows;
				RETURN;
			END

			SELECT 
				@notif_patient_id = patient_id,
				@notif_doctor_id = doctor_id,
				@notif_date = date,
				@notif_time = time
			FROM Appointments
			WHERE appointment_id = @appointment_id;

			SET LANGUAGE Spanish;

			SET @ampm = CASE 
				WHEN DATEPART(HOUR, @notif_time) < 12 THEN 'a. m.' 
				ELSE 'p. m.' 
			END;

			SET @formatted_time = FORMAT(@notif_time, 'hh\:mm');
			SET @formatted_date = LOWER(FORMAT(@notif_date, 'dd MMM yyyy', 'es-ES'));

			SET @message = CONCAT(
				'Tu cita para el ',
				@formatted_date, ' - ',
				@formatted_time, ' ', @ampm,
				' ha sido cancelada.'
			);

			INSERT INTO Notifications (DoctorId, PatientId, AppointmentId, Message, IsRead, CreatedAt)
			VALUES (
				@notif_doctor_id,
				@notif_patient_id,
				@appointment_id,
				@message,
				0,
				GETDATE()
			);

			COMMIT TRANSACTION;

			SELECT 1 AS affected_rows;
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;
			THROW;
		END CATCH

		RETURN;
	END

	ELSE IF @indicator = 'CONFIRM'
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION;

			UPDATE Appointments
			SET status = 'confirmada'
			WHERE appointment_id = @appointment_id
			  AND status = 'pendiente';

			IF @@ROWCOUNT = 0
			BEGIN
				ROLLBACK TRANSACTION;
				SELECT 0 AS affected_rows;
				RETURN;
			END
			SELECT 
				@notif_patient_id = patient_id,
				@notif_doctor_id = doctor_id,
				@notif_date = date,
				@notif_time = time
			FROM Appointments
			WHERE appointment_id = @appointment_id;

			SET LANGUAGE Spanish;

			SET @ampm = CASE 
				WHEN DATEPART(HOUR, @notif_time) < 12 THEN 'a. m.' 
				ELSE 'p. m.' 
			END;

			SET @formatted_time = FORMAT(@notif_time, 'hh\:mm');
			SET @formatted_date = LOWER(FORMAT(@notif_date, 'dd MMM yyyy', 'es-ES'));

			SET @message = CONCAT(
				'Tu cita para el ',
				@formatted_date, ' - ',
				@formatted_time, ' ', @ampm,
				' ha sido confirmada.'
			);

			INSERT INTO Notifications (DoctorId, PatientId, AppointmentId, Message, IsRead, CreatedAt)
			VALUES (
				@notif_doctor_id,
				@notif_patient_id,
				@appointment_id,
				@message,
				0,
				GETDATE()
			);

			COMMIT TRANSACTION;

			SELECT 1 AS affected_rows;
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;
			THROW;
		END CATCH

		RETURN;
	END

    ELSE IF @indicator = 'GET_BY_ID'
    BEGIN
        SELECT 
            a.appointment_id,
            s.name AS specialty_name,
            u_d.first_name + ' ' + u_d.last_name AS doctor_name,
            u_p.first_name + ' ' + u_p.last_name AS patient_name,
            a.consultation_type,
            a.date,
            a.time,
            a.symptoms,
            a.status
        FROM Appointments a
        INNER JOIN Users u_d ON a.doctor_id = u_d.user_id
        INNER JOIN Users u_p ON a.patient_id = u_p.user_id
        INNER JOIN Specialties s ON a.specialty_id = s.specialty_id
        WHERE a.appointment_id = @appointment_id;

		RETURN;
    END

	ELSE IF @indicator = 'GET_IDS_BY_ID'
	BEGIN
		SELECT 
			appointment_id,
			doctor_id,
			patient_id,
			specialty_id
		FROM Appointments
		WHERE appointment_id = @appointment_id;

		RETURN;
	END

	ELSE IF @indicator = 'GET_ALL'
    BEGIN
		SELECT 
            a.appointment_id,
            s.name AS specialty_name,
            u_d.first_name + ' ' + u_d.last_name AS doctor_name,
            u_p.first_name + ' ' + u_p.last_name AS patient_name,
            a.consultation_type,
            a.date,
            a.time,
            a.symptoms,
            a.status
        FROM Appointments a
        INNER JOIN Users u_d ON a.doctor_id = u_d.user_id
        INNER JOIN Users u_p ON a.patient_id = u_p.user_id
        INNER JOIN Specialties s ON a.specialty_id = s.specialty_id

		RETURN;
    END

    ELSE IF @indicator = 'GET_BY_USER_AND_STATUS'
    BEGIN
		SELECT 
            a.appointment_id,
            s.name AS specialty_name,
            u_d.first_name + ' ' + u_d.last_name AS doctor_name,
            u_p.first_name + ' ' + u_p.last_name AS patient_name,
            a.consultation_type,
            a.date,
            a.time,
            a.symptoms,
            a.status
        FROM Appointments a
        INNER JOIN Users u_d ON a.doctor_id = u_d.user_id
        INNER JOIN Users u_p ON a.patient_id = u_p.user_id
        INNER JOIN Specialties s ON a.specialty_id = s.specialty_id
        WHERE 
            (
                (@user_rol = 'paciente' AND a.patient_id = @user_id)
                OR
                (@user_rol = 'medico' AND a.doctor_id = @user_id)
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
            s.name AS specialty_name,
            u_d.first_name + ' ' + u_d.last_name AS doctor_name,
            u_p.first_name + ' ' + u_p.last_name AS patient_name,
            a.consultation_type,
            a.date,
            a.time,
            a.symptoms,
            a.status
        FROM Appointments a
        INNER JOIN Users u_d ON a.doctor_id = u_d.user_id
        INNER JOIN Users u_p ON a.patient_id = u_p.user_id
        INNER JOIN Specialties s ON a.specialty_id = s.specialty_id
        WHERE 
            (
                (@user_rol = 'paciente' AND a.patient_id = @user_id)
                OR
                (@user_rol = 'medico' AND a.doctor_id = @user_id)
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
		DECLARE @rows INT = 0;

		IF @enabled = 0
		BEGIN
			DELETE FROM Schedules
			WHERE doctor_id = @doctor_id 
			  AND weekday = @weekday 
			  AND day_work_shift = @day_work_shift;

			SET @rows = @@ROWCOUNT;
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

			SET @rows = @@ROWCOUNT;
		END

		SELECT @rows AS affected_rows;

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

-- =============================================
-- PROCEDURE: Notification_CRUD
-- DESCRIPTION: CRUD for the updated Notification table
-- PARAMETERS : action indicator, Notification data
-- =============================================
CREATE OR ALTER PROCEDURE Notification_CRUD
    @indicator VARCHAR(50),
    @notification_id INT = NULL,
    @doctor_id INT = NULL,
    @pacient_id INT = NULL,
    @appointment_id INT = NULL,
    @message VARCHAR(500) = NULL,
    @is_read BIT = NULL,
    @role VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @indicator = 'GET_UNREAD_DOC'
    BEGIN
        SELECT *
        FROM Notifications
        WHERE 
            IsRead = 0 AND
            DoctorId = @doctor_id AND
            Message LIKE 'Nueva cita%'
        ORDER BY CreatedAt DESC;

        RETURN;
    END

    IF @indicator = 'GET_UNREAD_PA'
    BEGIN
        SELECT *
        FROM Notifications
        WHERE 
            IsRead = 0 AND
            PatientId = @pacient_id AND
            (
                Message LIKE '%confirmada%' OR
                Message LIKE '%cancelada%'
            )
        ORDER BY CreatedAt DESC;

        RETURN;
    END

    ELSE IF @indicator = 'MARK_AS_READ'
    BEGIN
        UPDATE Notifications
        SET IsRead = 1
        WHERE NotificationId = @notification_id;

        RETURN;
    END

    ELSE
    BEGIN
        RAISERROR('Acción no válida: %s', 16, 1, @indicator);
        RETURN;
    END
END
GO