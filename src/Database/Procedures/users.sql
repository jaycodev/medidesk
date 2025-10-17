USE MediDesk;
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
    @profile_picture VARCHAR(255) = NULL,
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

			INSERT INTO UserRoles (user_id, role)
			VALUES (@new_id, 'administrador');

			COMMIT TRANSACTION;

			SELECT @new_id AS new_id;
		END TRY
		BEGIN CATCH
			IF @@TRANCOUNT > 0
				ROLLBACK TRANSACTION;

			SET @error_message = ERROR_MESSAGE();
			RETURN;
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
			SET @error_message = N'El correo ya está registrado. Por favor, ingrese otro.';
			RETURN;
		END

		BEGIN TRY
			BEGIN TRANSACTION;

			UPDATE Users
			SET first_name = @first_name,
				last_name = @last_name,
				email = @email,
				phone = @phone
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
			IF @@TRANCOUNT > 0 
				ROLLBACK TRANSACTION;

			SET @error_message = ERROR_MESSAGE();
			RETURN;
		END CATCH

		RETURN;
	END

	ELSE IF @indicator = 'DELETE'
	BEGIN
		BEGIN TRY
			DELETE FROM UserRoles WHERE user_id = @user_id;
			DELETE FROM Users WHERE user_id = @user_id;
			SELECT @@ROWCOUNT AS affected_rows;
		END TRY
		BEGIN CATCH
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
			u.profile_picture,

			d.status,
			s.name AS specialty_name,

			p.birth_date,
			p.blood_type
		FROM Users u
		LEFT JOIN UserRoles ur ON u.user_id = ur.user_id
		LEFT JOIN Doctors d ON u.user_id = d.user_id
		LEFT JOIN Specialties s ON d.specialty_id = s.specialty_id
		LEFT JOIN Patients p ON u.user_id = p.user_id
		WHERE u.user_id = @user_id
		GROUP BY 
			u.user_id, 
			u.first_name, 
			u.last_name, 
			u.email, 
			u.phone, 
			u.profile_picture, 
			d.status, 
			s.name,
			p.birth_date, 
			p.blood_type;

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
			u.profile_picture,
			u.phone
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
		BEGIN TRY
			UPDATE Users
			SET phone = @phone
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