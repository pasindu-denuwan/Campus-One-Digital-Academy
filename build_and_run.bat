@echo off
echo ===================================================
echo  Campus One Digital Academy - Build and Run
echo ===================================================
echo.

set MSBUILD="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"

if not exist %MSBUILD% (
    echo [ERROR] MSBuild was not found at %MSBUILD%
    pause
    exit /b 1
)

echo [1/2] Compiling solution in Release mode...
%MSBUILD% CampusOneDigitalAcademy.csproj /t:Build /p:Configuration=Release /v:minimal

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build failed. Please check the output above.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] Launching Campus One Digital Academy...
start "" "bin\Release\CampusOneDigitalAcademy.exe"
echo Done!
