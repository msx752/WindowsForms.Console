namespace WindowsForms.Console;

/// <summary>
/// new Console GUI
/// </summary>
public class FConsole : RichTextBox
{
    private const string _press_any_key_text = " Press Any Key...";
    private bool _inputEnable;
    private object _lockInputEnable = new();
    private object _lockLines = new();
    private object _lockPause = new();
    private object _lockReadLine = new();
    private bool _pause;
    private QueueTask _writeLineQueue = null;
    private AutoResetEvent autoResetEventInputEnable = new(false);

    public FConsole()
    {
        KeyDown += FConsole_KeyDown;
        TextChanged += FConsole_TextChanged;
        LinkClicked += FConsole_LinkClicked;
        MouseDown += FConsole_MouseDown;
        MouseMove += FConsole_MouseMove;
        MouseUp += FConsole_MouseUp;
        InitializeFConsole();
        Form.CheckForIllegalCrossThreadCalls = false; //this is important to the async access for multi-threading
    }

    /// <summary>
    /// parameters
    /// </summary>
    public string[] Arguments { get; set; }

    /// <summary>
    /// auto scrolling to end when adds newLine (default:true)
    /// </summary>
    public bool AutoScrollToEndLine { get; set; }

    public string[] CurrentLines
    {
        get
        {
            lock (_lockLines)
                return Lines;
        }
    }

    /// <summary>
    /// link color
    /// </summary>
    public Color HyperlinkColor { get; set; }

    /// <summary>
    /// stored last used color on console
    /// </summary>
    public Color LastUsedColor { get; private set; }

    /// <summary>
    /// after calling readline method sets ReadOnly= true (default:true)
    /// </summary>
    public bool SecureReadLine { get; set; }

    /// <summary>
    /// states of console
    /// </summary>
    public ConsoleState State { get; set; }

    /// <summary>
    /// console title (not necessary :) )
    /// </summary>
    public string Title
    {
        get
        {
            if (Parent != null)
                return Parent.Text;
            else
                return string.Empty;
        }
        set
        {
            if (Parent != null)
                Parent.Text = value;
        }
    }

    /// <summary>
    /// catch one char from console
    /// </summary>
    private char CurrentKey { get; set; }

    /// <summary>
    /// catch all chars in line from console
    /// </summary>
    private string CurrentLine { get; set; }

    /// <summary>
    /// read mode checker
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
                if ((_inputEnable = value))
                    autoResetEventInputEnable.Reset();
                else
                    autoResetEventInputEnable.Set();
            }
        }
    }

    /// <summary>
    /// console pause checker
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
    /// recent list of console
    /// </summary>
    private List<string> recentlist { get; set; }

    public new void Clear()
    {
        base.Clear();
        this.recentlist.Clear();
    }

    public new void Dispose(bool disposing)
    {
        try
        {
            this._writeLineQueue.Dispose();
        }
        catch { }
        try
        {
            this.recentlist.Clear();
        }
        catch { }
        try
        {
            this.RecentCount = 0;
            this.recentlist.Clear();
        }
        catch { }
        base.Dispose(disposing);
        GC.SuppressFinalize(this);
    }

    public void HistoryJumper()
    {
        if (RecentCount >= this.recentlist.Count)
            RecentCount = this.recentlist.Count - 1;
        else if (RecentCount < 0)
            RecentCount = 0;
    }

    /// <summary>
    /// Initialize Console
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
        Font = new("consolas", 10);
        MinimumSize = new(470, 200);
        ScrollBars = RichTextBoxScrollBars.Vertical;
        Pause = false;
        InputEnable = false;

        if (Parent != null)
        {
            ((Form)Parent).WindowState = FormWindowState.Normal;
            Parent.BackColor = BackColor;
        }

        DetectUrls = true;
        this.recentlist = new();
        SecureReadLine = true;
        AutoScrollToEndLine = true;
        State = ConsoleState.Writing;
        LastUsedColor = SelectionColor;
        MaxLength = 32767;
        this._writeLineQueue = new(this);
    }

    /// <summary>
    /// stop line
    /// </summary>
    /// <returns>
    /// </returns>
    public Task<char> ReadKey(Color? color = null, CancellationToken cancellationToken = default)
    {
        if (State == ConsoleState.ReadKey)
            InputEnable = false;

        State = ConsoleState.Writing;
        Color Color = ForeColor;

        if (color.HasValue)
            Color = color.Value;

        WriteLine(_press_any_key_text, Color);

        return Task.Run(() =>
        {
            var recentReadState = ReadOnly;
            CurrentKey = ' ';
            ReadPoint = Text.Length;
            InputEnable = true;
            ReadOnly = false;
            State = ConsoleState.ReadKey;
            this.autoResetEventInputEnable.WaitOne();

            if (SecureReadLine)
                ReadOnly = true;
            else
                ReadOnly = recentReadState;

            State = ConsoleState.Writing;

            return CurrentKey;
        }, cancellationToken);
    }

    /// <summary>
    /// read line
    /// </summary>
    /// <returns>
    /// </returns>
    public Task<string> ReadLine(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            lock (this._lockReadLine)
            {
                var recentReadState = ReadOnly;
                CurrentLine = string.Empty;
                ReadPoint = TextLength;
                InputEnable = true;
                ReadOnly = false;
                State = ConsoleState.ReadLine;
                this.autoResetEventInputEnable.WaitOne();
                Cursor = Cursors.IBeam;

                if (SecureReadLine)
                    ReadOnly = true;
                else
                    ReadOnly = recentReadState;

                State = ConsoleState.Writing;

                return CurrentLine;
            }
        }, cancellationToken);
    }

    public string RecentRedo()
    {
        if (recentlist.Count > 0)
        {
            RecentCount++;
            HistoryJumper();

            return this.recentlist[RecentCount];
        }
        else
        {
            return string.Empty;
        }
    }

    public string RecentUndo()
    {
        if (recentlist.Count > 0)
        {
            RecentCount--;
            HistoryJumper();

            return this.recentlist[RecentCount];
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// last line gets
    /// </summary>
    public void SelectLastLine()
    {
        if (CurrentLines.Any())
        {
            var line = CurrentLines.Count() - 1;
            var s1 = GetFirstCharIndexOfCurrentLine();
            var s2 = line < CurrentLines.Count() - 1 ? GetFirstCharIndexFromLine(line + 1) - 1 : Text.Length;
            Select(s1, s2 - s1);
        }
    }

    public void SetText(string message, Color? color = null)
    {
        if (!color.HasValue)
            color = ForeColor;

        LastUsedColor = SelectionColor = color.Value;
        SelectedText = message;
    }

    /// <summary>
    /// updates selected line
    /// </summary>
    /// <param name="line">
    /// </param>
    /// <param name="message">
    /// </param>
    /// <param name="color">
    /// </param>
    public void UpdateLine(int line, string message, Color? color = null)
    {
        ReadOnly = true;

        if (!color.HasValue)
            color = ForeColor;

        SelectLastLine();
        SetText(message, color);
    }

    /// <summary>
    /// write function
    /// </summary>
    /// <param name="message">
    /// </param>
    /// <param name="color">
    /// </param>
    /// <param name="showTimeTag">
    /// shows time on output
    /// </param>
    public void Write(string message, Color? color = null, bool showTimeTag = false)
    {
        this._writeLineQueue.Enqueue(new QueueTaskObject(message, color, showTimeTag));
    }

    /// <summary>
    /// writeline function
    /// </summary>
    /// <param name="message">
    /// </param>
    /// <param name="color">
    /// </param>
    public void WriteLine(string message, Color? color = null, bool showTimeTag = false)
    {
        Write(message + Environment.NewLine, color, showTimeTag);
    }

    internal bool OriginalWrite(string message, Color? color = null, bool showTimeTag = false)
    {
        if ((message.Length == _press_any_key_text.Length && !_press_any_key_text.Equals(message.Replace(Environment.NewLine, ""))) && State == ConsoleState.ReadKey)
            return false;

        if (this.IsDisposed)
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
            this.ScrollToCaret();

        if (SecureReadLine)
            ReadOnly = true;
        else
            ReadOnly = recentReadState;

        return true;
    }

    /// <summary>
    /// default console key events
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    private void FConsole_KeyDown(object sender, KeyEventArgs e)
    {
        if (State == ConsoleState.Closing)
        {
            e.SuppressKeyPress = true;
            return;
        }

        if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && State == ConsoleState.ReadLine)
        {
            if (this.recentlist.Count != 0)
            {
                var recentText = string.Empty;

                if (e.KeyCode == Keys.Up)
                {
                    recentText = RecentUndo();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    recentText = RecentRedo();
                }

                SelectLastLine();
                SelectedText = recentText;
            }

            e.SuppressKeyPress = true;
            return;
        }
        else
        {
            Select(TextLength, 0);
            if (e.KeyData == (Keys.Control | Keys.V))
            {
                MultiplePaste();
                e.SuppressKeyPress = true;
            }
            else if ((int)e.KeyCode == (int)Keys.Enter && InputEnable == true && State == ConsoleState.ReadLine)
            {
                Cursor = Cursors.WaitCursor;
                ReadOnly = true;
                CurrentLine = CurrentLines[CurrentLines.Count() - 1];
                recentlist.Add(CurrentLine);
                WriteLine(string.Empty);
                InputEnable = false;
                e.SuppressKeyPress = true;
            }
            else if (InputEnable == true && State == ConsoleState.ReadKey)
            {
                ReadOnly = true;
                CurrentKey = (char)e.KeyCode;
                InputEnable = false;
            }
            else if ((int)e.KeyCode == (int)Keys.Escape && InputEnable == false) //esc exit
            {
                Pause = false;
            }
            else if ((int)e.KeyCode == (int)Keys.Space && InputEnable == false) //space pause
            {
                Pause = !Pause;
            }
            else if ((int)e.KeyCode == (int)Keys.Back && InputEnable == true && ReadPoint + 1 > TextLength)
            {
                e.SuppressKeyPress = true;
            }
        }
    }

    /// <summary>
    /// link click router
    /// https://stackoverflow.com/questions/4580263/how-to-open-in-default-browser-in-c-sharp
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    private void FConsole_LinkClicked(object sender, LinkClickedEventArgs e)
    {
        // hack because of this: https://github.com/dotnet/corefx/issues/10361
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                Process.Start(e.LinkText);
            }
            catch
            {
                Process.Start("explorer", e.LinkText);
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", e.LinkText);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", e.LinkText);
        }
        else
        {
            Process.Start(e.LinkText);
        }
    }

    private void FConsole_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right && InputEnable)
            MultiplePaste();
        else if (!InputEnable)
            Select(Text.Length, 0);
    }

    private void FConsole_MouseMove(object sender, MouseEventArgs e)
    {
        if (!InputEnable)
            Select(Text.Length, 0);
    }

    private void FConsole_MouseUp(object sender, MouseEventArgs e)
    {
        if (!InputEnable)
            Select(Text.Length, 0);
    }

    private void FConsole_TextChanged(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// mouse right click event
    /// </summary>
    private void MultiplePaste()
    {
        ReadOnly = true;
        CurrentLine = Clipboard.GetText();
        InputEnable = false;
    }
}