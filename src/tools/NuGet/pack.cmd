
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
call :pack MeasureIt.Autofac
call :pack MeasureIt.Castle.Windsor
call :pack MeasureIt.Web.Http.Core
call :pack MeasureIt.Web.Http.Autofac
call :pack MeasureIt.Web.Http.Castle.Windsor
call :pack MeasureIt.Web.Mvc.Core
call :pack MeasureIt.Web.Mvc.Autofac
call :pack MeasureIt.Web.Mvc.Castle.Windsor
:: call :pack call :pack ... (?)
:: TODO: TBD: add additional projects here...

:end

popd

endlocal

pause

:: Be sure that we are exiting the command batch file scope
exit /b 0

:: Leave the function scope alone upon script exit.
:pack
echo Packing %* ...
%nuget_exe% pack %*\%*.csproj -symbols -properties Configuration=Release
exit /b 0
