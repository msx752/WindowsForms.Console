﻿namespace WindowsForms.Console.Extensions;

/// <summary>
/// for easy access to properties of FConsole
/// </summary>
public static class FConsoleExtensions
{
    /// <summary>
    /// extension of read key
    /// </summary>
    /// <returns>
    /// </returns>
    public static Task<char> ReadKey(this Form f, Color? color = null, CancellationToken cancellationToken = default)
    {
        FConsole console = CheckFConsole(f);
        return console.ReadKey(color, cancellationToken);
    }

    /// <summary>
    /// extension of readline
    /// </summary>
    /// <returns>
    /// </returns>
    public static Task<string> ReadLine(this Form f, CancellationToken cancellationToken = default)
    {
        FConsole console = CheckFConsole(f);
        return console.ReadLine(cancellationToken);
    }

    /// <summary>
    /// extension of write function
    /// </summary>
    /// <param name="f">
    /// </param>
    /// <param name="message">
    /// </param>
    /// <param name="color">
    /// </param>
    public static void Write(this Form f, string message = null, Color? color = null)
    {
        FConsole console = CheckFConsole(f);
        message = message ?? string.Empty;
        console.Write(message, color, false);
    }

    /// <summary>
    /// extension of writeline function
    /// </summary>
    /// <param name="f">
    /// </param>
    /// <param name="message">
    /// </param>
    /// <param name="color">
    /// </param>
    /// <param name="showTimeTag">
    /// shows time on output
    /// </param>
    public static void WriteLine(this Form f, string message = null, Color? color = null, bool showTimeTag = false)
    {
        FConsole console = CheckFConsole(f);
        message = message ?? string.Empty;
        console.WriteLine(message, color, showTimeTag);
    }

    private static FConsole CheckFConsole(Form f)
    {
        var fconsoles = FindControlByType<FConsole>(f, true);
        var count = fconsoles.Count();

        if (count == 0)
            throw new Exception("this WinForm does not have any FConsole component");
        else if (count > 1)
            throw new NotSupportedException("conflict occured while deciding to select FConsole component, more than one FConsole components detected in the one Form, please help for the improvement. Supported only one component at the same Form");

        return fconsoles.First();
    }

    /// <summary>
    /// finds all specific type of controls
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="mainControl">
    /// </param>
    /// <param name="getAllChild">
    /// search at child
    /// </param>
    /// <returns>
    /// </returns>
    /// https://stackoverflow.com/questions/4630391/get-all-controls-of-a-specific-type
    private static IEnumerable<T> FindControlByType<T>(Control mainControl, bool getAllChild = false) where T : Control
    {
        foreach (Control ctl in mainControl.Controls)
        {
            if (ctl is T t)
                yield return t;

            if (getAllChild)
                foreach (var childCtl in FindControlByType<T>(ctl, getAllChild))
                    yield return childCtl;
        }
    }
}