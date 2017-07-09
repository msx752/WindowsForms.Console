
if($($env:APPVEYOR)){
    $msbuild = "C:\Program Files (x86)\MSBuild\15.0\Bin\amd64\MSBuild.exe"
}else{

    $msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
}

.\nuget restore WindowsForm.Console.sln;

& $msbuild WindowsForm.Console.sln /property:Configuration=Release /p:TargetFrameworkVersion=v4.5 /p:OutputPath="bin\v4.5";
& $msbuild WindowsForm.Console.sln /property:Configuration=Release /p:TargetFrameworkVersion=v4.5.1 /p:OutputPath="bin\v4.5.1";
& $msbuild WindowsForm.Console.sln /property:Configuration=Release /p:TargetFrameworkVersion=v4.5.2 /p:OutputPath="bin\v4.5.2";
& $msbuild WindowsForm.Console.sln /property:Configuration=Release /p:TargetFrameworkVersion=v4.6 /p:OutputPath="bin\v4.6";
& $msbuild WindowsForm.Console.sln /property:Configuration=Release /p:TargetFrameworkVersion=v4.6.1 /p:OutputPath="bin\v4.6.1";
& $msbuild WindowsForm.Console.sln /property:Configuration=Release /p:TargetFrameworkVersion=v4.6.2 /p:OutputPath="bin\v4.6.2";
& $msbuild WindowsForm.Console.sln /property:Configuration=Release /p:TargetFrameworkVersion=v4.7 /p:OutputPath="bin\v4.7";

.\nuget pack WindowsForm.Console.nuspec;
