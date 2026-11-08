# Workshop RSVP - Lab 4 (building on Lab 3)

The original ASP.NET Core MVC application now reads registrations from SQL Server using Entity Framework Core. The existing `net10.0` target, workshop choices, Razor views, form validation, and confirmation flow are preserved. EF Core packages and the repository-local CLI tool are pinned to 8.0.22; CodeGeneration.Design is 8.0.6, satisfying the Lab 4 PDF.

## Local setup

Requirements: .NET 10 SDK (the original project target), Windows SQL Server LocalDB, and NuGet access. Visual Studio SQL Server Object Explorer is used for the assignment's database screenshot.

From the repository root:

```powershell
dotnet restore WorkshopRsvp.sln
dotnet tool restore
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet ef database update --project WorkshopRsvp
dotnet build WorkshopRsvp.sln --no-restore
dotnet run --project WorkshopRsvp --launch-profile http
```

Open `http://localhost:5080/Workshops/Registrations`. Development startup applies existing migrations and seeds the empty table with Alice Martin, Bilal Khan, and Chloe Tremblay. Restarts do not duplicate the seed records. Database: `WorkshopRsvpLab4` on `(localdb)\MSSQLLocalDB`, table `dbo.Rsvps`.

## Pages and behavior

| URL | Behavior |
| --- | --- |
| `/` | Original Home page |
| `/Home/Privacy` | Original Privacy page |
| `/Workshops` | Existing workshop descriptions and database registration count |
| `/Workshops/RsvpForm` | Existing form with validation and accommodation checkbox |
| `/Workshops/Confirm` (POST) | Validates and displays the submitted model and confirmation message |
| `/Workshops/Registrations` | Strongly typed view showing asynchronously loaded database records |

As explicitly allowed by Lab 4, form submissions are not saved. The Registrations page displays the seeded database records. There is no static in-memory registration list and no UI insertion, update, or delete action.

## Database implementation

- `Models/Rsvp.cs`: existing model plus an integer primary key.
- `Data/ApplicationDbContext.cs`: options constructor and `DbSet<Rsvp> Rsvps`.
- `Data/DbInitializer.cs`: static asynchronous seed method, invoked from Program.cs.
- `Data/Migrations/`: CLI-generated SQL Server migration and model snapshot.
- `Program.cs`: SQL Server context registration, local migration startup, seed initialization, and production connection guard.
- `Controllers/WorkshopsController.cs`: injected context, `CountAsync`, and read-only `AsNoTracking().ToListAsync()` queries.

Both SQL Server and SQLite packages are included because the PDF asks for both. SQL Server is selected to meet the SQL Server Object Explorer screenshot requirement and to support Azure SQL deployment. Compatible dependency patches remove the vulnerabilities initially reported by the required older tooling packages.

## Verification

With the app running:

```powershell
./scripts/Test-Lab4.ps1
sqlcmd -S '(localdb)\MSSQLLocalDB' -d WorkshopRsvpLab4 -E -C -i scripts/Verify-Database.sql -W -w 240
dotnet ef migrations has-pending-model-changes --project WorkshopRsvp --no-build
dotnet list WorkshopRsvp package --vulnerable --include-transitive
```

If script execution is disabled, run the reviewed test script without changing system policy:

```powershell
& ([scriptblock]::Create((Get-Content scripts/Test-Lab4.ps1 -Raw)))
```

The smoke checks expect the lab's three sample records. [The verification report](docs/LAB4-REPORT.md) records actual builds, tests, database contents, migration state, modified files, and remaining work.

## Azure and submission

[Azure configuration, deployment, screenshot, and GitHub instructions](docs/AZURE-AND-SUBMISSION.md) explain the remaining approval-dependent steps. Use Azure SQL and override `DefaultConnection` in App Service; LocalDB is only for this computer. The default Production behavior expects an already migrated database and then seeds it if empty.

No deployment, GitHub push, paid resource creation, or credential changes have been performed. Local build output, publish profiles, tools, and database files are ignored by Git. Work is on branch `lab4-ef`.
