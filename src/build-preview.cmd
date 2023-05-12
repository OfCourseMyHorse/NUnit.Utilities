@echo off

set GETTIMEKEY=powershell get-date -format "{yyyyMMdd-HHmm}"
for /f %%i in ('%GETTIMEKEY%') do set TIMEKEY=%%i

set VERSIONSUFFIX=Preview-%TIMEKEY%

echo Building %VERSIONSUFFIX%

dotnet pack -c:Release --version-suffix %VERSIONSUFFIX% /p:Authors=vpenades ..\NUnitUtilities.sln

md bin

for /r %%i in (*.*nupkg) do move %%i bin
for /r %%i in (*.*snupkg) do move %%i bin

pause