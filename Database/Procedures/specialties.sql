USE MediDesk;
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
    @description VARCHAR(255) = NULL,
	@new_id        INT = NULL OUTPUT,
    @affected_rows INT = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	SET @new_id = NULL;
    SET @affected_rows = NULL;

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

		SET @new_id = SCOPE_IDENTITY();

		RETURN;
	END

	ELSE IF @indicator = 'UPDATE'
	BEGIN
		UPDATE Specialties
		SET name = @name,
			description = @description
		WHERE specialty_id = @specialty_id;

		SET @affected_rows = @@ROWCOUNT;

		RETURN;
	END

	ELSE
    BEGIN
        RAISERROR('Acción no válida: %s', 16, 1, @indicator);
		RETURN;
    END
END;
GO