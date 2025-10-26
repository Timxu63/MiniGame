set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.
set CODE_OUTPUT=%WORKSPACE%\Assets\Scripts\DataTable\LocalModel
set DATA_OUTPUT=%WORKSPACE%\Assets\_Resources\LocalModel
dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-bin ^
    -d bin^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=%CODE_OUTPUT% ^
    -x outputDataDir=%DATA_OUTPUT%

pause