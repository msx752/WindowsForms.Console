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

## Example Usage

All examples assume you have added the FConsole control to your form (named `fconsole1`). Both direct usage and extension methods are shown. For input, async/await is recommended.

```csharp
// Write a line to the console
fconsole1.WriteLine("Hello, World!");
this.WriteLine("Hello, World!"); // Extension method (inside a Form)

// Write a line with a specific color
fconsole1.WriteLine("Success!", Color.Green);
this.WriteLine("Success!", Color.Green); // Extension method

// Write text without a newline
fconsole1.Write("Processing...");
this.Write("Processing..."); // Extension method

// Write colored text without a newline
fconsole1.Write("Warning!", Color.Orange);
this.Write("Warning!", Color.Orange); // Extension method

// Read a line from the console (asynchronous)
string input = await fconsole1.ReadLine();
string input2 = await this.ReadLine(); // Extension method

// Read a single key from the console (asynchronous)
char key = await fconsole1.ReadKey();
char key2 = await this.ReadKey(); // Extension method
```

> **Note:** `ReadLine` and `ReadKey` are asynchronous and should be awaited inside an async method for best UI responsiveness. Synchronous blocking is possible but not recommended on the UI thread.

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
