-- Fresh SQLite databases only. For repeatable updates, use dotnet ef database update.
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Rsvps" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Rsvps" PRIMARY KEY AUTOINCREMENT,
    "FullName" TEXT NOT NULL,
    "NeedsAccommodation" INTEGER NOT NULL,
    "WorkshopTitle" TEXT NOT NULL,
    "RegistrationDate" TEXT NOT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261108165306_InitialSqlite', '8.0.22');

COMMIT;
