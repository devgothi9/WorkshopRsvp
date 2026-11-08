-- Run in SQL Server Object Explorer or sqlcmd against WorkshopRsvpLab4.
SELECT DB_NAME() AS DatabaseName;
SELECT Id, FullName, Email, WorkshopTitle, NeedsAccommodation, RegisteredOn
FROM dbo.Rsvps
ORDER BY RegisteredOn, Id;
SELECT COUNT(*) AS RegistrationCount FROM dbo.Rsvps;
SELECT MigrationId, ProductVersion FROM dbo.__EFMigrationsHistory;
