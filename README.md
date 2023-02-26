[![nuget](https://img.shields.io/badge/Nuget-WindowsForms.Console-brightgreen.svg?maxAge=259200)](https://www.nuget.org/packages/WindowsForms.Console)
[![NuGet](https://img.shields.io/nuget/v/WindowsForms.Console.svg)](https://www.nuget.org/packages/WindowsForms.Console)
[![Build status](https://ci.appveyor.com/api/projects/status/enn19h5tkvhy2w95?svg=true)](https://ci.appveyor.com/project/msx752/windowsform-console)

# WindowsForms.Console
Component of WindowsForm

# Usage
- import nuget package to the project

```
        Install-Package WindowsForms.Console
```
- add 'FConsole' component to FormControl ([you can see how to](https://stackoverflow.com/questions/2101171/how-to-add-user-control-in-the-toolbox-for-c-net-for-winforms-by-importing-the))

- look at sample project [(for more example)](https://github.com/msx752/WindowsForms.Console/tree/master/SampleProject/SampleFormApplicationCore)

- example codes
```c#
        fconsole1.WriteLine("text");
        this.WriteLine("text");//(extension method)
        
        fconsole1.WriteLine("text",Color.White);
        this.WriteLine("text",Color.White);//(extension method)
        
        fconsole1.Write("text");
        this.Write("text");//(extension method)
        
        fconsole1.Write("text",Color.White);
        this.Write("text",Color.White);//(extension method)
        
        
        var line = fconsole1.ReadLine();//used in async method
        //or
        var line = this.ReadLine();//this as any Form (extension method)
        
       
       var line = await fconsole1.ReadKey(); //used in async method
       //or
       var line = await this.ReadKey();//(extension method)
```
![FConsole](https://raw.githubusercontent.com/msx752/WindowsForms.Console/master/example1.png)

# Supported Platforms
[moved to security.md](https://github.com/msx752/WindowsForms.Console/blob/master/SECURITY.md)

# Dependencies
- System.Windows.Form
- System
# FrameworkReferences for the .NetCore and upper
- Microsoft.WindowsDesktop.App.WindowsForm

# Example Project
- component is used in [MSniper Project](https://github.com/msx752/MSniper)
