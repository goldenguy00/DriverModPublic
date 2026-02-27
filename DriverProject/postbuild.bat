REM call postbuild.bat $(TargetDir) $(AssemblyName)




REM bin/Release/netstandard2.1
SET Output=%1
SET Log=%Output%OUTPUT.log

REM AssemblyName.dll
SET Dll=%2.dll
SET PDB=%2.pdb

REM bin/Release/netstandard2.1/AssemblyName.dll
SET OutputDll=%Output%%Dll%

REM Directory of your final build folder, the one that you want to zip and upload to thunderstore
SET Store=..\Thunderstore
SET Zip=%Store%\Release.zip

REM weavers ref to game files
SET Libs=Weaver\libs
SET Core=%Libs%\UnityEngine.CoreModule.dll
SET UNet=%Libs%\com.unity.multiplayer-hlapi.Runtime.dll




REM WEEEEAVER
.\Weaver\Unity.UNetWeaver.exe   %Core%   %UNet%   %Output%   %OutputDll%   %Libs%

REM Zip it up
IF EXIST %Store% CALL :zip_func

REM send it
EXIT /B %ERRORLEVEL%





REM --------------------------------------------
REM This function is for everything after weaver
REM --------------------------------------------
:zip_func
IF EXIST %Log% DEL %Log%

REM      FROM       DEST     FILE NAME(s)   LOG
robocopy %Output%   %Store%  %DLL%  %PDB%   /log+:%Log%
robocopy %Store%    ..\      README.md      /log+:%Log%

REM remove existing zip
IF EXIST %Zip% DEL %Zip%

REM zip contents for thunderstore package
powershell Compress-Archive -Path '%Store%\*' -DestinationPath '%Zip%' -Force
EXIT /B 0
