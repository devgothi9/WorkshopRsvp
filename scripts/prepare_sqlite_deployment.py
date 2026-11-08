"""Prepare a read-only Lab 4 deployment from existing Release publish output."""

import json
import shutil
import sqlite3
from pathlib import Path


root = Path(__file__).resolve().parents[1]
published = root / "artifacts" / "publish-sqlite"
output = root / "artifacts" / "azure-readonly"
database = root / "WorkshopRsvp" / "WorkshopRsvp.db"

if not (published / "WorkshopRsvp.dll").is_file():
    raise SystemExit("Run the README's Release publish command first.")
if not database.is_file():
    raise SystemExit("Run migrations and start the app to seed your database first.")

shutil.copytree(published, output, dirs_exist_ok=True)
# SQLite's backup API includes committed WAL contents and gives a consistent copy.
with sqlite3.connect(database.as_uri() + "?mode=ro", uri=True) as source:
    with sqlite3.connect(output / "WorkshopRsvp.db") as target:
        source.backup(target)
        target.execute("PRAGMA journal_mode=DELETE")
        assert target.execute("PRAGMA integrity_check").fetchone()[0] == "ok"
        assert target.execute("SELECT COUNT(*) FROM Rsvps").fetchone()[0] == 3

settings = {
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=WorkshopRsvp.db;Mode=ReadOnly;Pooling=False"
    },
    "Database": {"ApplyMigrations": False},
}
(output / "appsettings.Production.json").write_text(
    json.dumps(settings, indent=2) + "\n", encoding="utf-8"
)
print(f"Prepared read-only deployment: {output}")
print("No files have been uploaded or deployed.")
