using System.ComponentModel;

namespace WindowsForms.Console;

/// <summary>
/// Represents a custom console control for Windows Forms.
/// </summary>
public class FConsole : RichTextBox
{
    private const string _press_any_key_text = " Press Any Key...";
    private bool _inputEnable;
    private readonly object _lockInputEnable = new();
    private readonly object _lockLines = new();
    private readonly object _lockPause = new();
    private readonly object _lockReadLine = new();
    private bool _pause;
    private QueueTask _writeLineQueue = null;
    private readonly AutoResetEvent _autoResetEventInputEnable = new(false);

    /// <summary>
    /// Initializes a new instance of the FConsole class.
    /// </summary>
    public FConsole()
    {
        KeyDown += FConsole_KeyDown;
        LinkClicked += FConsole_LinkClicked;
        MouseDown += FConsole_MouseDown;
        MouseMove += FConsole_MouseMove;
        MouseUp += FConsole_MouseUp;
        InitializeFConsole();
        Form.CheckForIllegalCrossThreadCalls = false; // Important for async access with multi-threading
    }

    /// <summary>
    /// Gets or sets the arguments passed to the console.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string[] Arguments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the console should automatically scroll to the end line.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AutoScrollToEndLine { get; set; }

    /// <summary>
    /// Gets the current lines of text in the console.
    /// </summary>
    public string[] CurrentLines
    {
        get
        {
            lock (_lockLines)
            {
                return (string[])Lines.Clone(); // Avoiding external modifications
            }
        }
    }

    /// <summary>
    /// Gets or sets the color of hyperlinks in the console.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color HyperlinkColor { get; set; }

    /// <summary>
    /// Gets the last used color in the console.
    /// </summary>
    public Color LastUsedColor { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the console should be read-only after calling ReadLine.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SecureReadLine { get; set; }

    /// <summary>
    /// Gets or sets the current state of the console.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ConsoleState State { get; set; }

    /// <summary>
    /// Gets or sets the title of the console.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Title
    {
        get => Parent?.Text ?? string.Empty;
        set
        {
            if (Parent != null)
            {
                Parent.Text = value;
            }
        }
    }

    private char CurrentKey { get; set; }

    private string CurrentLine { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether input is enabled.
    /// </summary>
    private bool InputEnable
    {
        get
        {
            lock (_lockInputEnable)
                return _inputEnable;
        }
        set
        {
            lock (_lockInputEnable)
            {
                _inputEnable = value;

                if (_inputEnable)
                    _autoResetEventInputEnable.Reset();
                else
                    _autoResetEventInputEnable.Set();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the console is paused.
    /// </summary>
    private bool Pause
    {
        get
        {
            lock (_lockPause)
                return _pause;
        }
        set
        {
            lock (_lockPause)
                _pause = value;
        }
    }

    private int ReadPoint { get; set; }

    private int RecentCount { get; set; }

    /// <summary>
    /// Gets or sets the recent list of lines entered in the console.
    /// </summary>
    private List<string> _recentList = new();

    /// <summary>
    /// Clears the console and resets the recent list.
    /// </summary>
    public new void Clear()
    {
        base.Clear();
        _recentList.Clear();
    }

    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the FConsole and its resources.
    /// </summary>
    /// <param name="disposing">Indicates whether the method was called from the Dispose method.</param>
    public new void Dispose(bool disposing)
    {
        try { _writeLineQueue?.Dispose(); } catch { }

        try { _autoResetEventInputEnable?.Dispose(); } catch { }

        _recentList.Clear();
        RecentCount = 0;

        base.Dispose(disposing);
    }

    /// <summary>
    /// Adjusts the RecentCount to ensure it stays within valid bounds.
    /// </summary>
    public void HistoryJumper()
    {
#if !NET48
        RecentCount = Math.Clamp(RecentCount, 0, _recentList.Count - 1);
#else
        if (RecentCount >= _recentList.Count)
            RecentCount = _recentList.Count - 1;
        else if (RecentCount < 0)
            RecentCount = 0;
#endif
    }

    /// <summary>
    /// Initializes the FConsole with default settings.
    /// </summary>
    public void InitializeFConsole()
    {
        Name = "FConsole";
        Text = Name;
        Title = Name;
        Arguments = Array.Empty<string>();
        BackColor = Color.Black;
        ForeColor = Color.FromArgb(0xdf, 0xd8, 0xc2);
        Dock = DockStyle.None;
        BorderStyle = BorderStyle.None;
        ReadOnly = true;
        Font = new Font("consolas", 10);
        MinimumSize = new Size(470, 200);
        ScrollBars = RichTextBoxScrollBars.Vertical;
        Pause = false;
        InputEnable = false;

        if (Parent != null)
        {
            ((Form)Parent).WindowState = FormWindowState.Normal;
            Parent.BackColor = BackColor;
        }

        DetectUrls = true;
        _recentList = new List<string>();
        SecureReadLine = true;
        AutoScrollToEndLine = true;
        State = ConsoleState.Writing;
        LastUsedColor = SelectionColor;
        MaxLength = 32767;
        _writeLineQueue = new QueueTask(this);
    }

    /// <summary>
    /// Reads a single key from the console.
    /// </summary>
    /// <param name="color">The color of the text to display.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The key pressed by the user.</returns>
    public Task<char> ReadKey(Color? color = null, CancellationToken cancellationToken = default)
    {
        if (State == ConsoleState.ReadKey)
            InputEnable = false;

        State = ConsoleState.Writing;
        Color displayColor = color ?? ForeColor;

        WriteLine(_press_any_key_text, displayColor);

        return Task.Run(() =>
        {
            lock (_lockReadLine)
            {
                var recentReadState = ReadOnly;
                CurrentKey = ' ';
                ReadPoint = Text.Length;
                InputEnable = true;
                ReadOnly = false;
                State = ConsoleState.ReadKey;

                _autoResetEventInputEnable.WaitOne();

                ReadOnly = SecureReadLine ? true : recentReadState;
                State = ConsoleState.Writing;

                return CurrentKey;
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Reads a line of text from the console.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The line of text entered by the user.</returns>
    public Task<string> ReadLine(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            lock (_lockReadLine)
            {
                var recentReadState = ReadOnly;
                CurrentLine = string.Empty;
                ReadPoint = TextLength;
                InputEnable = true;
                ReadOnly = false;
                State = ConsoleState.ReadLine;

                _autoResetEventInputEnable.WaitOne();
                Cursor = Cursors.IBeam;

                ReadOnly = SecureReadLine ? true : recentReadState;
                State = ConsoleState.Writing;

                return CurrentLine;
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Redoes the last undone entry in the console's history.
    /// </summary>
    /// <returns>The redone line.</returns>
    public string RecentRedo()
    {
        if (_recentList.Count > 0)
        {
            RecentCount++;
            HistoryJumper();

            return _recentList[RecentCount];
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Undoes the last entry in the console's history.
    /// </summary>
    /// <returns>The undone line.</returns>
    public string RecentUndo()
    {
        if (_recentList.Count > 0)
        {
            RecentCount--;
            HistoryJumper();

            return _recentList[RecentCount];
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Selects the last line in the console.
    /// </summary>
    public void SelectLastLine()
    {
        if (CurrentLines.Length > 0)
        {
            var line = CurrentLines.Length - 1;
            var s1 = GetFirstCharIndexOfCurrentLine();
            var s2 = line < CurrentLines.Length - 1 ? GetFirstCharIndexFromLine(line + 1) - 1 : Text.Length;
            Select(s1, s2 - s1);
        }
    }

    /// <summary>
    /// Sets the text in the console with the specified color.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="color">The color to display the message in.</param>
    public void SetText(string message, Color? color = null)
    {
        color ??= ForeColor;
        LastUsedColor = SelectionColor = color.Value;
        SelectedText = message;
    }

    /// <summary>
    /// Updates a specific line in the console with new text and color.
    /// </summary>
    /// <param name="line">The line number to update.</param>
    /// <param name="message">The new message to display.</param>
    /// <param name="color">The color to display the message in.</param>
    public void UpdateLine(int line, string message, Color? color = null)
    {
        ReadOnly = true;
        color ??= ForeColor;
        SelectLastLine();
        SetText(message, color);
    }

    /// <summary>
    /// Writes a message to the console with optional color and timestamp.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="color">The color to display the message in.</param>
    /// <param name="showTimeTag">Indicates whether to show a timestamp with the message.</param>
    public void Write(string message, Color? color = null, bool showTimeTag = false)
    {
        _writeLineQueue.Enqueue(new QueueTaskObject(message, color, showTimeTag));
    }

    /// <summary>
    /// Writes a message to the console followed by a newline, with optional color and timestamp.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="color">The color to display the message in.</param>
    /// <param name="showTimeTag">Indicates whether to show a timestamp with the message.</param>
    public void WriteLine(string message, Color? color = null, bool showTimeTag = false)
    {
        Write(message + Environment.NewLine, color, showTimeTag);
    }

    /// <summary>
    /// Writes the specified message to the console.
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <param name="color">The color to display the message in.</param>
    /// <param name="showTimeTag">Indicates whether to show a timestamp with the message.</param>
    /// <returns>True if the message was written successfully; otherwise, false.</returns>
    internal bool OriginalWrite(string message, Color? color = null, bool showTimeTag = false)
    {
        if ((message.Length == _press_any_key_text.Length && !_press_any_key_text.Equals(message.Replace(Environment.NewLine, string.Empty))) && State == ConsoleState.ReadKey)
            return false;

        if (IsDisposed)
            return false;

        var recentReadState = ReadOnly;
        Select(TextLength, 0);

        if (!message.EndsWith(Environment.NewLine) || State == ConsoleState.ReadLine || State == ConsoleState.ReadKey || message == Environment.NewLine)
            showTimeTag = false;

        if (showTimeTag)
            message = $"{DateTime.Now}: {message}";

        SetText(message, color);
        DeselectAll();

        if (AutoScrollToEndLine)
            ScrollToCaret();

        ReadOnly = SecureReadLine ? true : recentReadState;

        return true;
    }

    private void FConsole_KeyDown(object sender, KeyEventArgs e)
    {
        if (State == ConsoleState.Closing)
        {
            e.SuppressKeyPress = true;
            return;
        }

        if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && State == ConsoleState.ReadLine)
        {
            if (_recentList.Count != 0)
            {
                var recentText = e.KeyCode == Keys.Up ? RecentUndo() : RecentRedo();
                SelectLastLine();
                SelectedText = recentText;
            }

            e.SuppressKeyPress = true;
            return;
        }

        Select(TextLength, 0);

        if (e.KeyData == (Keys.Control | Keys.V))
        {
            MultiplePaste();
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Enter && InputEnable && State == ConsoleState.ReadLine)
        {
            Cursor = Cursors.WaitCursor;
            ReadOnly = true;
            CurrentLine = CurrentLines[CurrentLines.Length - 1];
            _recentList.Add(CurrentLine);
            WriteLine(string.Empty);
            InputEnable = false;
            e.SuppressKeyPress = true;
        }
        else if (InputEnable && State == ConsoleState.ReadKey)
        {
            ReadOnly = true;
            CurrentKey = (char)e.KeyCode;
            InputEnable = false;
        }
        else if (e.KeyCode == Keys.Escape && !InputEnable)
        {
            Pause = false;
        }
        else if (e.KeyCode == Keys.Space && !InputEnable)
        {
            Pause = !Pause;
        }
        else if (e.KeyCode == Keys.Back && InputEnable && ReadPoint + 1 > TextLength)
        {
            e.SuppressKeyPress = true;
        }
    }

    private void FConsole_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        try
        {
            var url = e.LinkText;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                Process.Start(url);
            }
        }
        catch
        {
            // Handle or log exceptions
        }
    }

    private void FConsole_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right && InputEnable)
        {
            MultiplePaste();
        }
        else if (!InputEnable)
        {
            Select(Text.Length, 0);
        }
    }

    private void FConsole_MouseMove(object sender, MouseEventArgs e)
    {
        if (!InputEnable)
        {
            Select(Text.Length, 0);
        }
    }

    private void FConsole_MouseUp(object sender, MouseEventArgs e)
    {
        if (!InputEnable)
        {
            Select(Text.Length, 0);
        }
    }

    /// <summary>
    /// Handles multiple pastes by inserting the clipboard text at the current cursor position.
    /// </summary>
    private void MultiplePaste()
    {
        ReadOnly = true;
        CurrentLine = Clipboard.GetText();
        InputEnable = false;
    }
}
