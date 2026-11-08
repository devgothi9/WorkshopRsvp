# Lab 4 verification report - SQLite version

Verified November 8, 2026 (America/Toronto) inside the original WorkshopRsvp solution, branch `lab4-ef`.

## Inspection and reference comparison

Both pages of the Lab 4 PDF were previously read, rendered and inspected. The current source was inspected again before this change. The reference ZIP's model, context, seed initializer, controller, Program.cs, project file, settings, migration and snapshot were reviewed without executing its application. Only the database and database screenshot were extracted to ignored `artifacts/reference` for inspection. The reference's Git metadata, publish credentials and personal records were not copied into this project's source or database.

The reference is a .NET 8 project using SQLite and a five-column Rsvps table. This project keeps its original .NET 10 target and existing MVC implementation. A direct comparison of `PRAGMA table_info(Rsvps)` in both databases passed: column order, names, types, nullability and primary-key metadata match exactly. The reference screenshot shows Visual Studio's SQLite/SQL Server Compact Toolbox, not VS Code or SQL Server Object Explorer.

## Requirement checklist

| Requirement | Status / evidence |
| --- | --- |
| Work directly in existing Lab 3 project | Complete; same solution, controllers, views and model reused. |
| Branch lab4-ef | Complete; original master preserved. |
| PDF-compatible packages | Complete; EF Core, SqlServer, Sqlite, Tools, Design and local dotnet-ef 8.0.22; CodeGeneration.Design 8.0.6. |
| ApplicationDbContext | Complete; existing options constructor and DbSet<Rsvp> retained. |
| SQLite connection in appsettings.json | Complete; Data Source=WorkshopRsvp.db;Pooling=False. Relative filenames resolve against the project's content root. |
| Register context in Program.cs | Complete; UseSqlite replaces UseSqlServer. |
| SeedData static method invoked at startup | Complete; SeedData.InitializeAsync checks for an empty table before inserting. |
| Own WorkshopRsvp.db | Complete; generated locally by EF migrations, then seeded by startup; 20,480-byte file at WorkshopRsvp/WorkshopRsvp.db. |
| Required five-column Rsvps schema | Complete; Id, FullName, NeedsAccommodation, WorkshopTitle, RegistrationDate. Types and constraints verified directly. |
| Three original sample records | Complete; Alice Martin, Bilal Khan, Chloe Tremblay retained from the user's original project. None of the reference's people copied. |
| EF migrations created and applied | Complete; 20261108165306_InitialSqlite, product version 8.0.22; no pending model changes. |
| Remove static registration list | Complete; no static list or registration lock in the controller. |
| Async EF registrations query | Complete; AsNoTracking, ordering by RegistrationDate/Id, ToListAsync; landing count uses CountAsync. |
| Strongly typed registrations view | Complete; IEnumerable<Rsvp> renders the three database rows. |
| Preserve Lab 3 form and confirmation | Complete; workshop choices, email validation, name validation, accommodation details, antiforgery protection, and confirmation preserved. |
| No database writes from RSVP UI | Complete; valid submissions show confirmation only and do not change database rows, as requested for Lab 4. |
| VS Code table viewer | SQLite Viewer installed and the exact database file opened by command. Final table-grid visual inspection remains unverified because app access approval timed out. Instructions provided. |
| Deployment preparation | Complete locally; published app, fresh schema script, reproducible read-only SQLite deployment folder and configuration guide prepared and tested. |
| Azure deployment and deployed Registrations verification | Incomplete; requires approval and an intended App Service target. No upload/deployment performed. |
| Azure Registrations screenshot | Incomplete; the new code has not been deployed. |
| PDF SQL Server Object Explorer -> View Data screenshot | Incomplete; PDF allows SQLite but names a SQL Server tool. Reference instead uses a SQLite toolbox. Instructor acceptance of an equivalent SQLite screenshot remains unresolved. |
| GitHub / Brightspace links | Existing remote identified and local implementation prepared on lab4-ef. Push still requires approval; Azure link remains unavailable. |

## Schema and preserved email behavior

The reference schema has exactly five columns. To preserve the original Lab 3 form, `Rsvp.Email` remains required and email-validated but is `[NotMapped]`. It appears on the form and confirmation, not in SQLite or the registrations table. `RegisteredOn` was renamed to `RegistrationDate` throughout the model, controller, and date displays. This is an intentional adaptation to match the requested structure, not a loss of form validation.

| Id | FullName | NeedsAccommodation | WorkshopTitle | RegistrationDate |
| --- | --- | --- | --- | --- |
| 1 | Alice Martin | 0 | Routing Basics | 2026-10-01 09:30:00 |
| 2 | Bilal Khan | 1 | EF Core Intro | 2026-10-02 14:15:00 |
| 3 | Chloe Tremblay | 0 | Razor Essentials | 2026-10-03 11:00:00 |

## Actual test results

| Command / check actually run | Result |
| --- | --- |
| dotnet restore WorkshopRsvp.sln | Passed. |
| dotnet tool restore | Passed. |
| dotnet build WorkshopRsvp.sln --no-restore | Passed: zero warnings, zero errors. |
| dotnet build WorkshopRsvp.sln -c Release --no-restore | Passed: zero warnings, zero errors. |
| dotnet publish ... -c Release --no-restore -o artifacts/publish-sqlite | Passed. Development database excluded from default publish output. |
| dotnet ef migrations add InitialSqlite | Passed; SQLite migration generated by CLI. |
| dotnet ef database update | Passed; SQLite database, Rsvps and migration history created. |
| dotnet ef migrations list / has-pending-model-changes | InitialSqlite applied; no pending model changes. |
| dotnet list ... package --vulnerable --include-transitive | No vulnerable packages reported by the configured sources. |
| Development app, full HTTP suite | All 19 checks passed. |
| Restarted development app, same DB | All 19 checks passed again; exactly three records remained. |
| Published Production app, fresh separate SQLite DB | Automatically migrated/seeded; all 19 HTTP checks and read-only database checks passed. |
| Prepared read-only deployment folder in Production | All 19 HTTP checks passed, including form confirmation without DB writes. Its database integrity/schema/records check passed. |
| Direct SQLite verification | PRAGMA integrity_check = ok; exact five-column schema, original records and migration history verified. |
| Comparison with reference DB metadata | Exact PRAGMA table_info match; only metadata compared, not personal records copied. |
| Generated SQLite script against a fresh in-memory DB | Passed; expected five-column table created. |
| Native app visual check | Blocked by app-access approval timeout; no browser/grid visual pass claimed. |
| Real Azure hosting / Azure URL | Not run; not deployed. |

The 19 HTTP checks cover five pages, three static assets, each seeded name, row count, Yes/No accommodation values, landing count, antiforgery token, invalid-form server validation, valid confirmation and email/accommodation details, unchanged registration rows after submission, and HTTP 400 without an antiforgery token. They do not prove client-side JavaScript behavior or visual layout.

## Files created / modified in the SQLite conversion

Created/renamed:

- `WorkshopRsvp/Data/SeedData.cs` (replaces DbInitializer.cs).
- `WorkshopRsvp/Data/Migrations/20261108165306_InitialSqlite.cs` and its Designer.cs.
- `scripts/verify_database.py` (standard-library, read-only DB integration check).
- `scripts/prepare_sqlite_deployment.py` (consistent DB backup and read-only Production package).
- `docs/SQLITE-IN-VSCODE.md`.
- Generated local `WorkshopRsvp/WorkshopRsvp.db` (ignored; recreated by migrations and startup seed).

Modified:

- `WorkshopRsvp/Models/Rsvp.cs`: exact mapped fields, RegistrationDate, form-only Email.
- `WorkshopRsvp/Program.cs`: stable SQLite path, context provider and seed call.
- `WorkshopRsvp/appsettings.json`: SQLite connection and startup migrations.
- `WorkshopRsvp/Controllers/WorkshopsController.cs`: RegistrationDate query/confirmation usage.
- `WorkshopRsvp/Views/Workshops/Registrations.cshtml`: mapped record columns/date.
- `WorkshopRsvp/Views/Workshops/Confirm.cshtml`: renamed date property.
- `WorkshopRsvp/WorkshopRsvp.csproj`: exclude local DB files from ordinary publish.
- `WorkshopRsvp/Data/Migrations/ApplicationDbContextModelSnapshot.cs`: regenerated for SQLite.
- `scripts/Deploy-Lab4.sql` and `scripts/Verify-Database.sql`: SQLite syntax.
- `.gitignore`, README, this report and Azure/submission guide.

The old SQL Server migration files were replaced because migrations are provider-specific. The earlier implementation is preserved in Git commit `79dfd5c`; its SQL Server database was left untouched. ApplicationDbContext itself and the existing HTTP test script needed no change. Local artifacts contain published output and the read-only package; none of the reference files or generated databases are staged for Git.

## Remaining work and limitations

No unresolved compilation, migration, dependency-audit, database-integrity or tested HTTP errors remain. Local HTTP-only testing logs the expected HTTPS-port warning; real Azure HTTPS still requires verification. The seed initializer is intended for a single-instance lab startup; concurrent initialization was not tested.

A VS Code database editor tab was opened and the extension installed, but the final visual grid remains unverified. Follow [SQLite viewing instructions](SQLITE-IN-VSCODE.md). Actual Azure deployment/verification, Azure screenshot, acceptable database screenshot, GitHub push and Brightspace submission remain pending. See [Azure and submission instructions](AZURE-AND-SUBMISSION.md). No credentials or Azure resources were changed.
