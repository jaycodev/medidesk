USE MediDesk;
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

			INSERT INTO Notifications (DoctorId, PatientId, AppointmentId, Message, CreatedAt)
			VALUES (
				@doctor_id,
				@patient_id,
				@appointment_id,
				@message,
				GETDATE()
			);

			COMMIT TRANSACTION;

			SELECT @appointment_id AS new_id;
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

			INSERT INTO Notifications (DoctorId, PatientId, AppointmentId, Message, CreatedAt)
			VALUES (
				@notif_doctor_id,
				@notif_patient_id,
				@appointment_id,
				@message,
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

			INSERT INTO Notifications (DoctorId, PatientId, AppointmentId, Message, CreatedAt)
			VALUES (
				@notif_doctor_id,
				@notif_patient_id,
				@appointment_id,
				@message,
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

	ELSE IF @indicator = 'ATTEND'
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION;

			UPDATE Appointments
			SET status = 'atendida'
			WHERE appointment_id = @appointment_id
			  AND status = 'confirmada';

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
				'Tu cita del ',
				@formatted_date, ' - ',
				@formatted_time, ' ', @ampm,
				' fue atendida. ¡Gracias por asistir!'
			);

			INSERT INTO Notifications (DoctorId, PatientId, AppointmentId, Message, CreatedAt)
			VALUES (
				@notif_doctor_id,
				@notif_patient_id,
				@appointment_id,
				@message,
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
            a.status
        FROM Appointments a
        INNER JOIN Users u_d ON a.doctor_id = u_d.user_id
        INNER JOIN Users u_p ON a.patient_id = u_p.user_id
        INNER JOIN Specialties s ON a.specialty_id = s.specialty_id
        WHERE 
            (
				@user_rol = 'administrador'
				OR
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