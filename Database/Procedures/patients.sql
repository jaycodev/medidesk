USE MediDesk;
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
    @blood_type VARCHAR(3) = NULL,
	@new_id INT = NULL OUTPUT,
    @error_message NVARCHAR(4000) = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

	IF @indicator = 'INSERT'
    BEGIN
        IF EXISTS (SELECT 1 FROM Users WHERE email = @email)
        BEGIN
            SET @error_message = N'El correo ya está registrado. Por favor, ingrese otro.';
            RETURN;
        END

        BEGIN TRY
            BEGIN TRANSACTION;

            INSERT INTO Users (first_name, last_name, email, password, phone)
            VALUES (@first_name, @last_name, @email, @password, @phone);

            SET @new_id = CAST(SCOPE_IDENTITY() AS INT);

            INSERT INTO Patients (user_id, birth_date, blood_type)
            VALUES (@new_id, @birth_date, @blood_type);

            INSERT INTO UserRoles (user_id, role)
            VALUES (@new_id, 'paciente');

            COMMIT TRANSACTION;

            SELECT @new_id AS new_id;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

            SET @error_message = ERROR_MESSAGE();
            RETURN;
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