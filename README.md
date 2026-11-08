# Workshop RSVP - Lab 4 with SQLite

This is the original Lab 3 ASP.NET Core MVC project, extended in place on branch `lab4-ef`. The existing .NET 10 target, workshop descriptions, navigation, form validation, accommodation checkbox, and confirmation flow are preserved. Registrations now come from SQLite through EF Core.

## Run locally

```powershell
dotnet restore WorkshopRsvp.sln
dotnet tool restore
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet ef database update --project WorkshopRsvp
dotnet build WorkshopRsvp.sln --no-restore
dotnet run --project WorkshopRsvp --launch-profile http
```

Open `http://localhost:5080/Workshops/Registrations`. The database is **WorkshopRsvp/WorkshopRsvp.db**. Startup applies migrations and `SeedData.InitializeAsync` inserts Alice Martin, Bilal Khan, and Chloe Tremblay only when the table is empty. Restarting does not duplicate these records.

EF Core, SqlServer, Sqlite, Tools, Design, and local dotnet-ef are 8.0.22; CodeGeneration.Design is 8.0.6. These meet the PDF's package limits. Both providers are installed as requested by the PDF; **UseSqlite** selects SQLite. Compatible dependency patches keep the package audit clean.

## Schema and Lab 3 behavior

`Rsvps` has exactly these columns, matching the reference structure:

| Column | SQLite type | Constraint |
| --- | --- | --- |
| Id | INTEGER | Primary key, autoincrement, not null |
| FullName | TEXT | Not null |
| NeedsAccommodation | INTEGER | Not null; 0 or 1 |
| WorkshopTitle | TEXT | Not null |
| RegistrationDate | TEXT | Not null; EF Core DateTime value |

Email remains on the Lab 3 form and confirmation, with its original required/email validation. It is marked `[NotMapped]`, so it is not stored or shown as a blank database-list column. `RegisteredOn` was renamed to `RegistrationDate` in the model, controller and views. The original name-length validation is retained.

Home, Privacy, Workshops, RSVP Form and confirmation are preserved. As specified for Lab 4, submitting a form **does not insert a registration**; All Registrations displays the seeded database records. There is no static registration list.

## Open the database in VS Code

SQLite Viewer (`qwtel.sqlite-viewer`) has been installed, and the database file was opened in the project window. In the Explorer, open **WorkshopRsvp -> WorkshopRsvp.db**, then select **Rsvps** in the viewer's table list. If VS Code shows binary-file text instead, right-click the tab and choose **Reopen Editor With... -> SQLite Viewer**. Select `__EFMigrationsHistory` to inspect the applied migration.

See [the viewer instructions](docs/SQLITE-IN-VSCODE.md), including the distinction between VS Code and the reference's Visual Studio screenshot.

## Verify

With the app running:

```powershell
./scripts/Test-Lab4.ps1
python scripts/verify_database.py
dotnet ef migrations list --project WorkshopRsvp --no-build
dotnet ef migrations has-pending-model-changes --project WorkshopRsvp --no-build
dotnet list WorkshopRsvp package --vulnerable --include-transitive
```

The Python check uses only the standard library and opens the database read-only. Use your installed Python interpreter if `python` is unavailable. If PowerShell blocks script files, run the reviewed smoke checks without changing system policy:

```powershell
& ([scriptblock]::Create((Get-Content scripts/Test-Lab4.ps1 -Raw)))
```

[scripts/Verify-Database.sql](scripts/Verify-Database.sql) contains SQLite queries for a query-capable database tool. The generated [schema script](scripts/Deploy-Lab4.sql) is for a **fresh** SQLite database, not an idempotent SQL Server script. Use `dotnet ef database update` for repeatable updates.

## Deployment and submission

[Full verification report and requirement checklist](docs/LAB4-REPORT.md).

[Azure preparation and Brightspace checklist](docs/AZURE-AND-SUBMISSION.md).

The local `.db`, reference inspection files, tools, publish output, and publish profiles are ignored by Git. The database is recreated from migrations plus SeedData. The previous SQL Server implementation remains in commit `79dfd5c`; its LocalDB database has not been dropped or modified by the SQLite conversion. Nothing has been deployed or pushed.
