REM original version https://risk-of-thunder.github.io/R2Wiki/Mod-Creation/C%23-Programming/Networking/UNet/
REM open this in vs it'll be so much nicer

REM call with ./weave.bat $(TargetDir)
set Target=DriverMod
set Output=%1
set Libs=Weaver\libs
set Zip=..\Release\Release.zip
set Store=..\Release
set Log=%Output%OUTPUT.log

REM copy unpatched dll to weaver folder in case its needed
robocopy    %Output%   .\Weaver     %Target%.dll     %Target%.pdb    /log:%Log%
ren .\Weaver\%Target%.dll   %Target%.dll.prepatch
ren .\Weaver\%Target%.pdb   %Target%.pdb.prepatch

REM le epic networking patch
.\Weaver\Unity.UNetWeaver.exe   %Libs%\UnityEngine.CoreModule.dll   %Libs%\com.unity.multiplayer-hlapi.Runtime.dll  %Output%    %Output%%Target%.dll   %Libs%

REM move prepatch back to output
robocopy    .\Weaver    %Output%    %Target%.dll.prepatch    %Target%.pdb.prepatch   /log:%Log%
del Weaver\%Target%.dll.prepatch
del Weaver\%Target%.pdb.prepatch

REM that's it. This is meant to pretend we just built a dll like any other time except this one is networked
REM add your postbuilds in vs like it's any other project
robocopy    %Output%   %Store%      DriverMod.dll DriverMod.pdb   /log+:%Log%

REM delete old zip
if exist %Zip% Del %Zip%

powershell Compress-Archive -Path '%Store%\*' -DestinationPath '%Zip%' -Force