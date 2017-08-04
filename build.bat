set MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MsBuild.exe"
set MAKEPBO="C:\Program Files (x86)\Mikero\DePboTools\bin\MakePbo.exe"

rd /s /q @ExileLootDrop
mkdir @ExileLootDrop\addons

%MSBUILD% src\ExileLootDrop.sln /property:Configuration=release /property:Platform=x86 /target:Rebuild /verbosity:normal /nologo
copy src\ExileLootDrop\bin\x86\Release\ExileLootDrop.dll @ExileLootDrop\ExileLootDrop.dll
copy src\ExileLootDropTester\bin\x86\Release\ExileLootDropTester.exe @ExileLootDrop

%MSBUILD% src\ExileLootDrop.sln /property:Configuration=release /property:Platform=x64 /target:Rebuild /verbosity:normal /nologo
copy src\ExileLootDrop\bin\x64\Release\ExileLootDrop.dll @ExileLootDrop\ExileLootDrop_x64.dll
copy src\ExileLootDrop\bin\x64\Release\ExileLootDrop.cfg @ExileLootDrop
copy src\ExileLootDrop\bin\x64\Release\ExileLootDrop.ini @ExileLootDrop
copy LICENSE.txt @ExileLootDrop
copy README.md @ExileLootDrop

%MAKEPBO% -U -W -P -@=ExileLootDrop sqf @ExileLootDrop\addons\ExileLootDrop.pbo
del @ExileLootDrop.zip
powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('@ExileLootDrop', '@ExileLootDrop.zip'); }"
pause