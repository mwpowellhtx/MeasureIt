
@echo off

setlocal

:: Expecting NuGet to be installed in the Path in System Environment Variables.
set nuget_exe=NuGet.exe
set csproj_files=*.csproj
set nuspec_files=*.nuspec

pushd ..\..

echo Packing NuGet projects...

call :pack MeasureIt.Core
call :pack MeasureIt.Castle.Interception
:: TODO: TBD: add additional projects here...

REM for /R %%f in (MeasureIt.Castle.Windsor.AspNet.WebApi\%nuspec_files%) do (
    REM "%nuget_exe%" pack "%%f" -symbols
REM )

:end

popd

endlocal

pause

:: Be sure that we are exiting the command batch file scope
exit /b 0

:: Leave the function scope alone upon script exit.
:pack
echo Packing %* ...
%nuget_exe% pack %*\%*.csproj -symbols
exit /b 0
