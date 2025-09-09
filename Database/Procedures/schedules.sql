USE MediDesk;
GO

-- =============================================
-- PROCEDURE: Schedule_CRUD
-- DESCRIPTION: CRUD for the updated Schedules table
-- PARAMETERS : action indicator, schedule data
-- =============================================
CREATE OR ALTER PROCEDURE Schedule_CRUD
    @indicator VARCHAR(50),
    @doctor_id INT = NULL,
	@date DATE = NULL,
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

    ELSE IF @indicator = 'GET_BY_DOCTOR'
    BEGIN
        SELECT day_work_shift, doctor_id, weekday, start_time, end_time FROM Schedules
        WHERE doctor_id = @doctor_id;

        RETURN;
    END

	ELSE IF @indicator = 'GET_BY_DOCTOR_AND_DATE'
	BEGIN
		SET LANGUAGE Spanish;

		SET @weekday = LOWER(DATENAME(weekday, @date));

		SELECT day_work_shift, start_time, end_time
		FROM Schedules
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