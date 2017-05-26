[![nuget](https://img.shields.io/badge/Nuget-WindowsForm.Console-brightgreen.svg?maxAge=259200)](https://www.nuget.org/packages/WindowsForm.Console)
[![NuGet](https://img.shields.io/nuget/v/WindowsForm.Console.svg)](https://www.nuget.org/packages/WindowsForm.Console)

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
        fconsole1.WriteLine("text",Color.White);
        
        fconsole1.Write("text");
        fconsole1.Write("text",Color.White);
        
        //Task used due to access the other thread (needs improvement)
        Task.Run(() =>
            {
                
                var line = fconsole1.ReadLine();
            });
        
        //Task used due to access the other thread (needs improvement)
        Task.Run(() =>
            {
                //needs improvement
                var line = fconsole1.ReadKey();
            });
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
