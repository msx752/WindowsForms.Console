namespace WindowsForms.Console.Extensions;

/// <summary>
/// Provides extension methods for easy access to the properties and methods of FConsole.
/// </summary>
public static class FConsoleExtensions
{
    /// <summary>
    /// Extension method to read a single key from the console within the specified Form.
    /// </summary>
    /// <param name="f">The Form containing the FConsole control.</param>
    /// <param name="color">The color of the text to be displayed when prompting for a key.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the key read from the console.</returns>
    public static Task<char> ReadKey(this Form f, Color? color = null, CancellationToken cancellationToken = default)
    {
        FConsole console = CheckFConsole(f);
        return console.ReadKey(color, cancellationToken);
    }

    /// <summary>
    /// Extension method to read a line of text from the console within the specified Form.
    /// </summary>
    /// <param name="f">The Form containing the FConsole control.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the line read from the console.</returns>
    public static Task<string> ReadLine(this Form f, CancellationToken cancellationToken = default)
    {
        FConsole console = CheckFConsole(f);
        return console.ReadLine(cancellationToken);
    }

    /// <summary>
    /// Extension method to write a message to the console within the specified Form.
    /// </summary>
    /// <param name="f">The Form containing the FConsole control.</param>
    /// <param name="message">The message to be written to the console.</param>
    /// <param name="color">The color of the text to be displayed.</param>
    public static void Write(this Form f, string message = null, Color? color = null)
    {
        FConsole console = CheckFConsole(f);
        message ??= string.Empty;
        console.Write(message, color, false);
    }

    /// <summary>
    /// Extension method to write a message followed by a newline to the console within the specified Form.
    /// </summary>
    /// <param name="f">The Form containing the FConsole control.</param>
    /// <param name="message">The message to be written to the console.</param>
    /// <param name="color">The color of the text to be displayed.</param>
    /// <param name="showTimeTag">Indicates whether to show a timestamp with the message.</param>
    public static void WriteLine(this Form f, string message = null, Color? color = null, bool showTimeTag = false)
    {
        FConsole console = CheckFConsole(f);
        message ??= string.Empty;
        console.WriteLine(message, color, showTimeTag);
    }

    /// <summary>
    /// Checks and retrieves the FConsole control from the specified Form.
    /// </summary>
    /// <param name="f">The Form to search for an FConsole control.</param>
    /// <returns>The FConsole control found in the Form.</returns>
    /// <exception cref="Exception">Thrown if no FConsole control is found in the Form.</exception>
    /// <exception cref="NotSupportedException">Thrown if more than one FConsole control is found in the Form.</exception>
    private static FConsole CheckFConsole(Form f)
    {
        var fconsoles = FindControlByType<FConsole>(f, true);
        var count = fconsoles.Count();

        if (count == 0)
            throw new Exception("This WinForm does not have any FConsole component.");
        else if (count > 1)
            throw new NotSupportedException("Multiple FConsole components detected in the Form. Only one FConsole component is supported per Form.");

        return fconsoles.First();
    }

    /// <summary>
    /// Finds all controls of a specific type within the specified Form.
    /// </summary>
    /// <typeparam name="T">The type of controls to search for.</typeparam>
    /// <param name="mainControl">The main control to search within (e.g., the Form).</param>
    /// <param name="getAllChild">Indicates whether to search within child controls as well.</param>
    /// <returns>An enumerable collection of controls of the specified type.</returns>
    private static List<T> FindControlByType<T>(Control mainControl, bool getAllChild = false) where T : Control
    {
        var result = new List<T>();

        var stack = new Stack<Control>();
        stack.Push(mainControl);

        while (stack.Count > 0)
        {
            var control = stack.Pop();
            if (control is T t)
            {
                result.Add(t);
            }

            if (getAllChild)
            {
                foreach (Control child in control.Controls)
                {
                    stack.Push(child);
                }
            }
        }

        return result;
    }
}
