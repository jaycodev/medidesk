USE MediDesk;
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
    @status BIT = NULL,
	@new_id INT = NULL OUTPUT,
    @error_message NVARCHAR(4000) = NULL OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;

	SET @new_id = NULL;
    SET @error_message = NULL;

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

			INSERT INTO Doctors (user_id, specialty_id, status)
			VALUES (@new_id, @specialty_id, @status);

			INSERT INTO UserRoles (user_id, role)
			VALUES (@new_id, 'medico');

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