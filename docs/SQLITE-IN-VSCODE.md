# Viewing your SQLite database in VS Code

Installed extension: **SQLite Viewer**, publisher Florian Klampfer, extension ID `qwtel.sqlite-viewer`, version 26.9.1. [Official Marketplace page](https://marketplace.visualstudio.com/items?itemName=qwtel.sqlite-viewer).

Your own file is:

```text
C:\Users\devgo\OneDrive\Desktop\WorkshopRsvp\WorkshopRsvp\WorkshopRsvp.db
```

1. Use the VS Code window titled **WorkshopRsvp.db - WorkshopRsvp**.
2. In Explorer, expand the inner **WorkshopRsvp** folder and open **WorkshopRsvp.db**.
3. Choose **Rsvps** in the viewer's table list. You should see five columns and three rows: Alice Martin, Bilal Khan, and Chloe Tremblay.
4. Select **__EFMigrationsHistory** to see `20261108165306_InitialSqlite`, EF Core 8.0.22.
5. If the database opens as a binary/text file, right-click its editor tab -> **Reopen Editor With... -> SQLite Viewer**. The viewer may need reopening after an external database change.

The standard viewer is sufficient for inspecting rows; no paid viewer feature is needed for this assignment. The extension was installed and a command opened this exact file. An independent SQLite query verified the schema and rows. Automated visual confirmation of the table grid was blocked when the app-access approval timed out, so that visual step remains unverified.

## Expected table

| Id | FullName | NeedsAccommodation | WorkshopTitle | RegistrationDate |
| --- | --- | --- | --- | --- |
| 1 | Alice Martin | 0 | Routing Basics | 2026-10-01 09:30:00 |
| 2 | Bilal Khan | 1 | EF Core Intro | 2026-10-02 14:15:00 |
| 3 | Chloe Tremblay | 0 | Razor Essentials | 2026-10-03 11:00:00 |

`WorkshopRsvp.db-wal` and `WorkshopRsvp.db-shm`, if present while tools are connected, are SQLite journal/support files. Open the main `.db` file. Do not remove journal files while the application or a viewer is using the database.

## The reference screenshot and grading

The supplied reference screenshot is **Visual Studio**, not VS Code. Its pane is **SQLite/SQL Server Compact Toolbox**. Its Rsvps grid has the same five-column schema now verified in your database. Your project retains your own sample registrations and existing MVC code rather than copying the reference's people or application.

The PDF literally asks for **SQL Server Object Explorer -> View Data**, even though it permits SQLite for Lab 4. VS Code's SQLite grid is useful equivalent data evidence, but it is not that exact tool. This discrepancy remains unresolved for grading: confirm whether your instructor accepts a SQLite Viewer or SQLite/SQL Server Compact Toolbox screenshot. Do not label a VS Code screenshot as SQL Server Object Explorer.

For a clear SQLite evidence screenshot, show the database filename, Rsvps selected, all five column headers, and all three records. The other required screenshot must show the Registrations page running from **Azure**, with the deployed address visible; localhost is not a substitute.
