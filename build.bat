set MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe"
set MAKEPBO="C:\Program Files (x86)\Mikero\DePboTools\bin\MakePbo.exe"

rd /s /q @ExileLootDrop
mkdir @ExileLootDrop\addons

%MSBUILD% src\ExileLootDrop.sln /property:Configuration=release /target:Rebuild /verbosity:normal /nologo
copy src\ExileLootDrop\bin\Release\ExileLootDrop.dll @ExileLootDrop
copy src\ExileLootDrop\bin\Release\ExileLootDrop.cfg @ExileLootDrop
copy LICENSE.txt @ExileLootDrop
copy README.txt @ExileLootDrop


%MAKEPBO% -U -W -P -@=ExileLootDrop sqf @ExileLootDrop\addons\ExileLootDrop.pbo
