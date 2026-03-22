@echo off
echo Running Railway Management System Tests...
echo ==========================================

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
echo Running all tests...
dotnet test --logger "console;verbosity=detailed" --collect:"XPlat Code Coverage"

if %ERRORLEVEL% NEQ 0 (
    echo Some tests failed!
) else (
    echo All tests passed successfully!
)

echo.
echo Test execution completed.
pause