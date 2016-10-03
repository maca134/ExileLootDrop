set MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe"
set MAKEPBO="C:\Program Files (x86)\Mikero\DePboTools\bin\MakePbo.exe"

rd /s /q @ExileLootDrop
mkdir @ExileLootDrop\addons

%MSBUILD% src\ExileLootDrop.sln /property:Configuration=release /target:Rebuild /verbosity:normal /nologo
copy src\ExileLootDrop\bin\Release\ExileLootDrop.dll @ExileLootDrop
copy src\ExileLootDrop\bin\Release\ExileLootDrop.cfg @ExileLootDrop
copy src\ExileLootDrop\bin\Release\ExileLootDrop.ini @ExileLootDrop
copy src\ExileLootDropTester\bin\Release\ExileLootDropTester.exe @ExileLootDrop
copy LICENSE.txt @ExileLootDrop
copy README.md @ExileLootDrop

%MAKEPBO% -U -W -P -@=ExileLootDrop sqf @ExileLootDrop\addons\ExileLootDrop.pbo
del @ExileLootDrop.zip
powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('@ExileLootDrop', '@ExileLootDrop.zip'); }"
pause