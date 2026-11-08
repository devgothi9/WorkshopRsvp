# Azure and Brightspace submission for the SQLite version

No deployment, push, paid-resource creation, or credential change has been performed. The project remains `net10.0`. Azure App Service must have a compatible .NET 10 runtime, or a separately prepared self-contained publish for its actual architecture. The self-contained variant has not been tested here.

## Prepared read-only package

Lab 4 only requires reading three seeded records. To support this without needing a writable database inside a deployment package, a local folder has been prepared at **artifacts/azure-readonly** containing:

- Release publish output and IIS `web.config`.
- A consistent SQLite backup of your own database, including its applied migration and three original rows.
- `appsettings.Production.json` selecting `Data Source=WorkshopRsvp.db;Mode=ReadOnly;Pooling=False` and `Database:ApplyMigrations=false`.

This folder was tested locally in Production mode: all 19 HTTP checks passed. The database is intentionally a read-only deployment artifact; changing the seed contents requires preparing and redeploying it. This fits the Lab 4 UI, which does not save form submissions. It is not a design for later writable registration features; Assignment 2 requires Azure SQL.

Regenerate from the repository root after changes:

```powershell
dotnet restore WorkshopRsvp.sln
dotnet tool restore
dotnet build WorkshopRsvp.sln -c Release --no-restore
dotnet publish WorkshopRsvp/WorkshopRsvp.csproj -c Release --no-restore -o artifacts/publish-sqlite
python scripts/prepare_sqlite_deployment.py
```

The default publish excludes the development `.db`; the preparation script explicitly adds a consistent backup using SQLite's backup API and converts its journal mode to DELETE. It never copies the friend's database. `scripts/Deploy-Lab4.sql` is a generated fresh-SQLite schema script; it is not idempotent. The recommended repeatable schema command is `dotnet ef database update`. [Microsoft's SQLite migration limitations](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations).

## Steps requiring approval/configuration

1. Confirm the intended existing App Service, or explicitly approve new resources and any cost first.
2. Verify the App Service runtime supports this project's .NET 10 target and set `ASPNETCORE_ENVIRONMENT=Production`.
3. Review any existing connection-string/app-setting overrides. A leftover SQL Server `DefaultConnection`, `ConnectionStrings__DefaultConnection`, or `Database__ApplyMigrations=true` would override the packaged settings and must be corrected with approval. No SQL credentials are needed for this SQLite deployment.
4. Deploy the **contents of artifacts/azure-readonly** as the site's publish output after approval. Keep the database at the content root beside the DLL, not under `wwwroot`. App Service package deployment can keep that database read-only.
5. Verify the actual Azure URL, table contents, logs, and restart behavior. These cloud checks have not been run; a local Production test cannot prove Azure hosting works.

For a different deployment that needs a mutable SQLite database, writable storage and file locking must be designed and tested separately. Do not assume the application package or temporary storage is writable/durable. Microsoft cautions against SQLite on App Service custom storage mounts because it relies on file locks. [App Service filesystem behavior](https://learn.microsoft.com/en-us/azure/app-service/operating-system-functionality), [storage mount limitations](https://learn.microsoft.com/en-us/azure/app-service/configure-connect-to-azure-storage). The prepared read-only package avoids runtime database writes.

## Verify after deployment

Open `https://<actual-app-host>/Workshops/Registrations`. Expect the three original rows and No/Yes/No accommodation values. Check Home, Privacy, workshop landing, the form, validation, and confirmation. New form submissions intentionally do not appear in All Registrations.

Run read-only smoke checks against the actual host:

```powershell
./scripts/Test-Lab4.ps1 -BaseUrl 'https://<actual-app-host>' -SkipFormPosts
```

If local script execution is disabled:

```powershell
& ([scriptblock]::Create((Get-Content scripts/Test-Lab4.ps1 -Raw))) -BaseUrl 'https://<actual-app-host>' -SkipFormPosts
```

Restart App Service and check the same three rows remain. Cloud logs should not show SQLite read-only-write failures or runtime-loading errors.

## Brightspace deliverables

1. **Azure website link** and **browser screenshot** of the Azure `/Workshops/Registrations` page with its database rows and Azure address visible. Not yet available: no deployment has occurred.
2. **Database data screenshot.** The PDF specifies SQL Server Object Explorer -> View Data, but allows SQLite; the reference uses SQLite/SQL Server Compact Toolbox in Visual Studio. Your requested VS Code SQLite view has equivalent table data, but acceptance of that tool is an instructor decision. See [viewer instructions](SQLITE-IN-VSCODE.md). Do not claim the literal PDF screenshot requirement is complete.
3. **GitHub repository link** containing the `lab4-ef` implementation. Existing remote: `https://github.com/devgothi9/WorkshopRsvp.git`. The local database is generated/ignored; source, migrations and SeedData reproduce it.

No changes have been pushed. Once the local implementation is committed and you approve pushing:

```powershell
git push -u origin lab4-ef
```

After that push, the submission branch link is `https://github.com/devgothi9/WorkshopRsvp/tree/lab4-ef`. Confirm that the new SQLite migration, SeedData and setup instructions are visible before submitting.
