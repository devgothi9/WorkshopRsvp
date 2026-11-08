-- SQLite: open WorkshopRsvp/WorkshopRsvp.db using a SQLite viewer/query tool.
PRAGMA integrity_check;
PRAGMA table_info('Rsvps');
SELECT Id, FullName, NeedsAccommodation, WorkshopTitle, RegistrationDate
FROM Rsvps
ORDER BY RegistrationDate, Id;
SELECT COUNT(*) AS RegistrationCount FROM Rsvps;
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory;
