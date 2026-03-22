@echo off
echo Running Railway Management System Unit Tests...
echo ===============================================

cd /d "%~dp0"

echo.
echo Building the test project...
dotnet build

if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo Running unit tests (excluding integration and email tests)...
dotnet test --filter "TestCategory!=Integration&TestCategory!=Email" --logger "console;verbosity=normal"

if %ERRORLEVEL% NEQ 0 (
    echo Some tests failed!
) else (
    echo Unit tests completed successfully!
)

echo.
echo Test execution completed.
pause