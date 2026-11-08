# Lab 4 final verification report

Verified locally on November 7, 2026 (America/Toronto). Work is inside the original WorkshopRsvp solution on branch `lab4-ef`.

## Requirements

| Lab requirement | Status and evidence |
| --- | --- |
| Read the entire PDF | Complete: both pages of Downloads/Lab 4.pdf were extracted, rendered, and inspected, including deliverables, steps, and grading table. |
| Inspect existing Lab 3 | Complete: model, both controllers, all Razor views, Program.cs, settings, project, assets, and publish metadata inspected. No AGENTS.md found in the project or immediate parent. |
| Determine .NET/dependencies | Complete: existing target net10.0 retained; installed SDKs 8.0.425 and 10.0.300; original project had no package references. |
| Create lab4-ef | Complete. Original master branch preserved. |
| Required packages | Complete: EF Core, SqlServer, Sqlite, Tools, and additional CLI Design package 8.0.22; CodeGeneration.Design 8.0.6. All comply with the PDF's limits. |
| ApplicationDbContext | Complete: DbContextOptions constructor and DbSet<Rsvp> Rsvps. |
| Existing Rsvp model | Preserved; added int Id for EF's primary key. Existing validation and display fields retained. |
| Connection string | Complete locally: DefaultConnection targets WorkshopRsvpLab4 on LocalDB, using Windows authentication and no stored password. Azure override documented. |
| Context registered | Complete: AddDbContext and UseSqlServer, with transient-failure retries. |
| Static seed method called from Program.cs | Complete: DbInitializer.InitializeAsync inserts three records only when the table is empty. |
| Migration created/applied | Complete locally: 20261108004420_InitialCreate creates dbo.Rsvps and is recorded in __EFMigrationsHistory. |
| Remove static registration list | Complete: removed registration list and lock; workshop title options remain. |
| Async database reads | Complete: Registrations uses AsNoTracking, deterministic ordering, and ToListAsync; landing count uses CountAsync. |
| Strongly typed Registrations view | Complete: existing IEnumerable<Rsvp> view already met the requirement and was retained. Three rows verified through HTTP. |
| Other Lab 3 behavior | Home, Privacy, workshop landing, form, validation, confirmation, accommodation details, links, and assets preserved. As allowed by Lab 4, submissions only show confirmation and do not persist. |
| Restore/build/test | Complete locally; results below. |
| Azure deployment preparation | Complete: Release output, idempotent SQL script, production connection guard, and deployment/configuration guide prepared. |
| Deploy to Azure and verify deployed page | Pending explicit deployment approval and Azure SQL/App Service configuration. No deployment or resource/credential changes performed. |
| Required screenshots | Pending: Azure Registrations browser screenshot and SQL Server Object Explorer -> View Data screenshot. Exact capture instructions in deployment guide. |
| GitHub submission | Local implementation committed on lab4-ef with submission documentation; push requires approval. |

## Files created or modified

Created:

- `WorkshopRsvp/Data/ApplicationDbContext.cs`
- `WorkshopRsvp/Data/DbInitializer.cs`
- `WorkshopRsvp/Data/Migrations/20261108004420_InitialCreate.cs`
- `WorkshopRsvp/Data/Migrations/20261108004420_InitialCreate.Designer.cs`
- `WorkshopRsvp/Data/Migrations/ApplicationDbContextModelSnapshot.cs`
- `dotnet-tools.json` (repository-local EF CLI tool)
- `scripts/Test-Lab4.ps1` (HTTP regression/smoke checks)
- `scripts/Verify-Database.sql` (database evidence query)
- `scripts/Deploy-Lab4.sql` (generated, idempotent SQL Server/Azure SQL schema script)
- `docs/AZURE-AND-SUBMISSION.md`
- `docs/LAB4-REPORT.md`

Modified:

- `WorkshopRsvp/WorkshopRsvp.csproj`: required packages and patched transitive dependencies.
- `WorkshopRsvp/Models/Rsvp.cs`: primary key only.
- `WorkshopRsvp/Controllers/WorkshopsController.cs`: constructor injection and async reads; removed in-memory registration storage.
- `WorkshopRsvp/Program.cs`: database registration, local migrations, startup seed, production connection guard.
- `WorkshopRsvp/appsettings.json`: local connection string and production migration opt-in default.
- `WorkshopRsvp/Views/Workshops/Confirm.cshtml`: explains that this lab's form preview does not save registrations.
- `README.md`: current setup, behavior, verification and submission links.
- `.gitignore`: local tools, evidence, output, database files, environment files and publish profiles.

Local publish output is in ignored `artifacts/publish`. Existing publish profiles were preserved and excluded from submission.

## Actual verification results

| Check executed | Result |
| --- | --- |
| dotnet restore WorkshopRsvp.sln | Passed. |
| dotnet build WorkshopRsvp.sln --no-restore | Passed: 0 warnings, 0 errors on final build. |
| dotnet build WorkshopRsvp.sln -c Release --no-restore | Passed: 0 warnings, 0 errors. |
| dotnet publish ... -c Release --no-restore -o artifacts/publish | Passed; generated IIS web.config inspected. |
| dotnet list WorkshopRsvp package --vulnerable --include-transitive | No vulnerable packages reported by the configured sources. |
| dotnet ef migrations add InitialCreate | Passed; migration and snapshot generated by CLI. |
| dotnet ef database update | Passed; database, table and migration history created. |
| dotnet ef migrations list | InitialCreate listed as applied (not pending). |
| dotnet ef migrations has-pending-model-changes | No changes since last migration. |
| Generated idempotent script executed against initialized LocalDB | Passed without changing existing rows. |
| Development HTTP regression checks | All 19 passed, then all 19 passed again after restart. |
| Published Release in Production mode against local SQL named pipe | Executed locally with a connection override; all 19 HTTP checks passed. This verifies the publish output, not Azure networking or hosting. |
| Direct SQL query after form tests and restart | Exactly 3 rows, Ids 1/2/3, expected data and migration history. |
| Native browser visual review | Unverified: browser host unavailable; Chrome app approval timed out. |
| Azure URL and Azure SQL verification | Not run: deployment not authorized/performed. |

The 19 HTTP checks cover Home, Privacy, Workshops, RSVP Form, Registrations, CSS, Bootstrap, validation JavaScript, each seeded name, three rows, Yes/No accommodation, database count, antiforgery field, server-side invalid-form validation, successful confirmation, no insertion from the form, and HTTP 400 for missing antiforgery token.

Database evidence:

| Id | FullName | Email | WorkshopTitle | NeedsAccommodation | RegisteredOn |
| --- | --- | --- | --- | --- | --- |
| 1 | Alice Martin | alice@example.com | Routing Basics | 0 | 2026-10-01 09:30:00 |
| 2 | Bilal Khan | bilal@example.com | EF Core Intro | 1 | 2026-10-02 14:15:00 |
| 3 | Chloe Tremblay | chloe@example.com | Razor Essentials | 0 | 2026-10-03 11:00:00 |

Migration: `20261108004420_InitialCreate`, product version `8.0.22`. Its timestamp was generated by the CLI in UTC.

## Resolved issues and remaining limits

Initial sandbox restrictions prevented network restore, Git metadata writes and LocalDB access; approved command escalation allowed those operations. Required older scaffolding dependencies initially triggered NuGet advisories. Direct compatible patches (Microsoft.Build 17.8.43, NuGet.Packaging/Protocol 6.11.2, SQLitePCLRaw.bundle_e_sqlite3 2.1.14) removed all reported advisories while keeping the required EF and scaffolding versions. An intermediate build encountered an executable lock from the test app; stopping that process and rebuilding resolved it. PowerShell's script execution policy was left unchanged; checks ran as a reviewed in-memory script block.

No unresolved compilation, migration, package-audit, or tested HTTP errors remain. HTTP-only local testing logged the expected inability to determine an HTTPS redirect port; Azure HTTPS still needs verification after deployment. Seeding is designed for a single-instance lab startup; concurrent multi-instance initialization was not tested. A real browser visual check, cloud deployment, Azure verification, screenshots, and GitHub push remain outstanding.

Follow [Azure and submission instructions](AZURE-AND-SUBMISSION.md) for the exact settings, verification command, two screenshots, two submission links and approval-dependent steps.
