# ACZ Cursor Agent Runbook (Hybrid ACZ)

Use this when an agent must rebuild and deploy a launcher-served custom client zip.

## Goal

Produce `Content.Client.zip` and place it at:

- `bin/Content.Server/Content.Client.zip`

This enables Hybrid ACZ when server `build.download_url` and `build.manifest_url` are empty.

## Safe Procedure (Do This Exactly)

From repo root (`space-station-14`):

1) Rebuild client binaries:

`dotnet build Content.Client/Content.Client.csproj -c Release --nologo /v:m /t:Rebuild /p:FullRelease=true /m`

2) Package client zip **without wiping bin**:

`dotnet run --project Content.Packaging -- client --skip-build --no-wipe-release --configuration Release`

3) Ensure server output directory exists:

`New-Item -ItemType Directory -Force -Path "bin/Content.Server" | Out-Null`

4) Copy packaged zip to Hybrid ACZ location:

`Copy-Item -Force "release/SS14.Client.zip" "bin/Content.Server/Content.Client.zip"`

5) Verify source/destination are identical (size + mtime).

## Why This Specific Flow

- `Content.Packaging` wipes `bin` unless `--skip-build` is used.
- Rebuilding client first + packaging with `--skip-build` gives a fresh zip without deleting server binaries.
- `release/SS14.Client.zip` is the expected packaging output for client packaging.

## Server Launch Reminder

Hybrid ACZ is used when:

- `build.download_url=` (empty)
- `build.manifest_url=` (empty)
- `bin/Content.Server/Content.Client.zip` exists

Example launch pattern:

`dotnet run --project Content.Server -- --cvar build.download_url= --cvar build.manifest_url= --cvar build.fork_id=redtruce --cvar build.version=dev-xyz`

## Common Failure Cases

- **Zip copied to wrong location**: must be exactly `bin/Content.Server/Content.Client.zip`.
- **Bin folder unexpectedly missing**: usually caused by packaging without `--skip-build`.
- **Launcher uses stale cache**: bump `build.version` (and optionally `build.fork_id`) to force redownload.
- **No custom behavior in launcher client**: confirm changes are content-side, not engine-side.

## For Future Agents

Before declaring success, always report:

- exact source zip path
- exact destination zip path
- byte size of both files
- last-write timestamp of both files

If any differ, deployment is not complete.
