namespace WindowsForms.Console.Enums;

/// <summary>
/// Represents the various states that the FConsole can be in.
/// </summary>
public enum ConsoleState
{
    /// <summary>
    /// The console is waiting for the user to input a full line of text.
    /// </summary>
    ReadLine = 0,

    /// <summary>
    /// The console is waiting for a single key press from the user.
    /// </summary>
    ReadKey = 1,

    /// <summary>
    /// The console is in the process of closing and is no longer accepting input.
    /// </summary>
    Closing = 2,

    /// <summary>
    /// The console is actively writing output to the display.
    /// </summary>
    Writing = 3
}
