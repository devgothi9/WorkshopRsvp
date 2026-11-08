# Lab 4 deployment and submission

The existing application remains ASP.NET Core MVC targeting **net10.0**. All EF Core packages and the local `dotnet-ef` tool are **8.0.22**, within the lab's >= 8.0.10 and < 9.x limits. CodeGeneration.Design is **8.0.6**. EF 8 supports .NET 8 and higher; changing the application's target framework was unnecessary.

## Database choice and screenshot conflict

The PDF allows Azure SQL or SQLite for deployment, but explicitly asks for a **SQL Server Object Explorer -> View Data** screenshot. SQLite cannot directly supply that SQL Server view. This computer has SQL Server 2025 Express LocalDB, sqlcmd, and Visual Studio installed. Therefore this implementation uses **SQL Server LocalDB locally and Azure SQL when deployed**. Both required provider packages are installed, but `UseSqlServer` selects the active provider. This also prepares the project for Assignment 2's mandatory Azure database.

Local server: `(localdb)\MSSQLLocalDB`; database: `WorkshopRsvpLab4`; table: `dbo.Rsvps`.

LocalDB cannot serve as the deployed App Service database. The application rejects the development LocalDB connection string in Production and explains that `DefaultConnection` must be configured.

## Azure configuration (requires approval before changes)

1. Select the existing intended Azure App Service and an Azure SQL database, or approve creation and the cost of new resources first. No Azure resource has been created or changed by this work.
2. For a framework-dependent deployment, configure a Windows App Service with a **.NET 10** runtime. The existing publish profile remains local and ignored. If that runtime is unavailable on the chosen plan, publish self-contained for the correct Windows architecture instead; do not assume the pre-existing Lab 3 runtime setting is sufficient.
3. In App Service **Settings -> Environment variables -> Connection strings**, add **DefaultConnection**, type **SQLAzure**, with the Azure SQL connection string. Alternatively add an app setting named `ConnectionStrings__DefaultConnection`. Use one method consistently. Keep the real value in Azure, never in appsettings.json or Git.
4. An SQL-auth connection string has this shape (placeholders only):

   ```text
   Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<database>;User ID=<existing-login>;Password=<secret>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

   Use existing approved credentials, or configure a managed identity with the necessary SQL permissions separately. This work has not created or changed credentials. Do not copy local `Trusted_Connection=True` or `TrustServerCertificate=True` into Azure.
5. Configure Azure SQL networking so the intended App Service can reach the server. Allow only the required network path; SQL Server Object Explorer also needs an approved path from your workstation. Authentication and database permissions must match the chosen connection method.
6. Keep `ASPNETCORE_ENVIRONMENT=Production`. Choose one schema deployment method:
   - **Reviewed script:** open [Deploy-Lab4.sql](../scripts/Deploy-Lab4.sql) against the intended Azure database and execute it after approval. It creates the schema and migration history once. Keep `Database__ApplyMigrations=false` (the default).
   - **Single-instance lab alternative:** set app setting `Database__ApplyMigrations=true` for initial startup, with an account permitted to apply schema changes. After successful initialization, return it to `false`. Do not run concurrent migration/seed startups across multiple instances.
7. The startup initializer inserts the three sample records when `dbo.Rsvps` is empty. Therefore the runtime database identity needs SELECT and INSERT permissions, even though the UI only reads records. The migration script contains the schema, not the seed records. Restarting an initialized single-instance app does not duplicate data.
8. Deploy the prepared Release output only after approval. Restart App Service, then verify the URL and database below. No deployment has been performed.

Microsoft references: [App Service connection strings](https://learn.microsoft.com/en-us/azure/app-service/configure-common#configure-connection-strings), [ASP.NET Core with Azure SQL](https://learn.microsoft.com/en-us/azure/app-service/tutorial-dotnetcore-sqldb-app), [SQL Server EF provider](https://learn.microsoft.com/en-us/ef/core/providers/sql-server/).

## Publish locally

Run from the repository root:

```powershell
dotnet restore WorkshopRsvp.sln
dotnet tool restore
dotnet build WorkshopRsvp.sln -c Release --no-restore
dotnet publish WorkshopRsvp/WorkshopRsvp.csproj -c Release --no-restore -o artifacts/publish
```

The publish output includes the generated IIS `web.config`. It is ignored by Git. For self-contained Windows x64 publishing when necessary:

```powershell
dotnet publish WorkshopRsvp/WorkshopRsvp.csproj -c Release -r win-x64 --self-contained true -o artifacts/publish-win-x64
```

The self-contained variant is an alternative command, not a build that was tested in this session.

## Verify after deployment

Open `https://<actual-app-host>/Workshops/Registrations`. Expect Alice Martin, Bilal Khan, and Chloe Tremblay, all workshop titles, accommodation values No/Yes/No, and the original registration dates. Check Home, Privacy, Workshops, RSVP Form, validation, and confirmation too. A new form confirmation intentionally does not add a row in Lab 4.

Run the read-only smoke checks against the actual deployment:

```powershell
./scripts/Test-Lab4.ps1 -BaseUrl 'https://<actual-app-host>' -SkipFormPosts
```

If local PowerShell execution policy prevents directly running the reviewed script, the same checks can run without changing the system execution policy:

```powershell
& ([scriptblock]::Create((Get-Content scripts/Test-Lab4.ps1 -Raw))) -BaseUrl 'https://<actual-app-host>' -SkipFormPosts
```

Connect SQL Server Object Explorer to the **same Azure SQL database used by the application** and run [Verify-Database.sql](../scripts/Verify-Database.sql). Confirm three rows and migration `20261108004420_InitialCreate`. Restart App Service once and check the count remains three. These Azure checks remain pending until deployment is approved and completed.

If startup fails: check App Service logs for a missing connection override, an unsupported .NET runtime, SQL firewall/authentication failure, or an unapplied schema. Do not publish raw logs containing connection secrets.

## Exact Brightspace deliverables

The PDF requires **two screenshots and two links**:

1. **Browser screenshot from Azure:** show the deployed `/Workshops/Registrations` page with its three database records and the Azure address visible. A localhost screenshot does not satisfy this deliverable.
2. **SQL Server Object Explorer -> View Data screenshot:** in Visual Studio choose **View -> SQL Server Object Explorer**, connect to the database, expand **Databases -> <database> -> Tables -> dbo.Rsvps**, right-click **View Data**. Show the three records and table/server context. Using the deployed Azure database makes the connection between the screenshots clearest. Do not expose passwords or publish-profile secrets.
3. Submit the deployed Azure website link in Brightspace.
4. Submit the GitHub repository link, pointing to the `lab4-ef` branch so the grader can find the Lab 4 code.

The two screenshots have not been captured. Native Chrome access timed out waiting for app approval, and the new code has not been deployed to Azure. The CLI SQL results are verification evidence, not a replacement for the requested Object Explorer screenshot.

## GitHub submission

Existing remote: `https://github.com/devgothi9/WorkshopRsvp.git`. The completed local implementation is committed on `lab4-ef`; the original `master` branch is preserved. Publish profiles, database files, tools, and build output are ignored. No credentials have been changed, and no changes have been pushed.

After approving the push, publish the branch:

```powershell
git push -u origin lab4-ef
```

Submission branch URL after that push: `https://github.com/devgothi9/WorkshopRsvp/tree/lab4-ef`. Confirm that the branch, Data/Migrations, context, seed initializer, README, and verification scripts are visible before submitting.
