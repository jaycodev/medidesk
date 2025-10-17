-- ⚠️ Run this file in SSMS with SQLCMD Mode enabled ⚠️

-- Change this variable to the path of your project if you open it from another directory
:setvar DatabasePath "C:\Repositories\medidesk\Database"

:r $(DatabasePath)\schema.sql
:r $(DatabasePath)\data.sql

:r $(DatabasePath)\Procedures\appointments.sql
:r $(DatabasePath)\Procedures\doctors.sql
:r $(DatabasePath)\Procedures\notifications.sql
:r $(DatabasePath)\Procedures\patients.sql
:r $(DatabasePath)\Procedures\schedules.sql
:r $(DatabasePath)\Procedures\specialties.sql
:r $(DatabasePath)\Procedures\users.sql
