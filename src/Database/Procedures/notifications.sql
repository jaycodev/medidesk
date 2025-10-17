USE MediDesk;
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
    @patient_id INT = NULL,
    @appointment_id INT = NULL,
    @message VARCHAR(500) = NULL
AS
BEGIN
SET NOCOUNT ON;

	IF @indicator = 'GET_ALL_DOC_NOTIFICATIONS'
	BEGIN
		SELECT NotificationId, DoctorId, PatientId, AppointmentId, Message, CreatedAt
		FROM Notifications
		WHERE 
			DoctorId = @doctor_id AND
			Message LIKE 'Nueva cita%'
		ORDER BY CreatedAt DESC;

		RETURN;
	END

	IF @indicator = 'GET_ALL_PA_NOTIFICATIONS'
	BEGIN
		SELECT NotificationId, DoctorId, PatientId, AppointmentId, Message, CreatedAt
		FROM Notifications
		WHERE 
			PatientId = @patient_id AND
			(
				Message LIKE '%confirmada%' OR
				Message LIKE '%cancelada%' OR
				Message LIKE '%atendida%'
			)
		ORDER BY CreatedAt DESC;

		RETURN;
	END

	ELSE IF @indicator = 'DELETE_BY_ID'
	BEGIN
		DELETE FROM Notifications WHERE NotificationId = @notification_id;
		RETURN;
	END

	ELSE
	BEGIN
		RAISERROR('Acción no válida: %s', 16, 1, @indicator);
		RETURN;
	END
END
GO