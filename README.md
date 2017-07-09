[![nuget](https://img.shields.io/badge/Nuget-WindowsForm.Console-brightgreen.svg?maxAge=259200)](https://www.nuget.org/packages/WindowsForm.Console)
[![NuGet](https://img.shields.io/nuget/v/WindowsForm.Console.svg)](https://www.nuget.org/packages/WindowsForm.Console)
[![Build status](https://ci.appveyor.com/api/projects/status/enn19h5tkvhy2w95?svg=true)](https://ci.appveyor.com/project/msx752/windowsform-console)

# WindowsForm.Console
Component of WindowsForm Console

# Usage
- import nuget package to the project

```
        Install-Package WindowsForm.Console
```
- initial code
```c#
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false; // <=== this is important for async access to thread
        }
```
- add 'FConsole' component to FormControl ([you can see how to add](https://stackoverflow.com/questions/2101171/how-to-add-user-control-in-the-toolbox-for-c-net-for-winforms-by-importing-the))

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
![FConsole](https://raw.githubusercontent.com/msx752/WindowsForm.Console/master/example1.png)

# Supported Applications
- [x] WindowsForm Application
- [ ] WPF (not supported yet)

# Dependencies
- System.Windows.Form
- System.Threading.*

# Example Project
- component is used for [MSniper Project](https://github.com/msx752/MSniper)
