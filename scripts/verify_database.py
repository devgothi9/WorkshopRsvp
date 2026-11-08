"""Read-only integration check for the Lab 4 SQLite database (standard library only)."""

import argparse
import json
import sqlite3
from pathlib import Path


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "database", nargs="?",
        default=Path(__file__).resolve().parents[1] / "WorkshopRsvp" / "WorkshopRsvp.db",
        type=Path,
    )
    args = parser.parse_args()
    database = args.database.resolve(strict=True)
    with sqlite3.connect(database.as_uri() + "?mode=ro", uri=True) as connection:
        assert connection.execute("PRAGMA integrity_check").fetchone()[0] == "ok", "Integrity check failed"
        columns = connection.execute("PRAGMA table_info('Rsvps')").fetchall()
        actual = [(column[1], column[2], column[3], column[5]) for column in columns]
        expected = [
            ("Id", "INTEGER", 1, 1),
            ("FullName", "TEXT", 1, 0),
            ("NeedsAccommodation", "INTEGER", 1, 0),
            ("WorkshopTitle", "TEXT", 1, 0),
            ("RegistrationDate", "TEXT", 1, 0),
        ]
        assert actual == expected, f"Unexpected table schema: {actual}"
        rows = connection.execute(
            "SELECT Id, FullName, NeedsAccommodation, WorkshopTitle, RegistrationDate "
            "FROM Rsvps ORDER BY Id"
        ).fetchall()
        expected_rows = [
            (1, "Alice Martin", 0, "Routing Basics", "2026-10-01 09:30:00"),
            (2, "Bilal Khan", 1, "EF Core Intro", "2026-10-02 14:15:00"),
            (3, "Chloe Tremblay", 0, "Razor Essentials", "2026-10-03 11:00:00"),
        ]
        assert rows == expected_rows, f"Unexpected seeded registrations: {rows}"
        migrations = connection.execute(
            "SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId"
        ).fetchall()
        assert len(migrations) == 1 and migrations[0][0].endswith("_InitialSqlite"), migrations
        assert migrations[0][1] == "8.0.22", migrations
    print(json.dumps({
        "database": str(database), "integrity": "ok", "schema": actual,
        "registrations": rows, "migrations": migrations, "result": "PASS",
    }, indent=2))


if __name__ == "__main__":
    main()
