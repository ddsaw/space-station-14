@echo off
cd /d "%~dp0"

set platform=
dotnet build
dotnet run --project content.server

pause
