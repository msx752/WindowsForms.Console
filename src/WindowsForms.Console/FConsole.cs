using System.Threading.Tasks;

namespace WindowsForms.Console
{
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

        public FConsole()
        {
            KeyDown += FConsole_KeyDown;
            TextChanged += FConsole_TextChanged;
            LinkClicked += FConsole_LinkClicked;
            MouseDown += FConsole_MouseDown;
            MouseMove += FConsole_MouseMove;
            MouseUp += FConsole_MouseUp;
            InitializeFConsole();
            Form.CheckForIllegalCrossThreadCalls = false; // Important for async access with multi-threading
        }

        public string[] Arguments { get; set; }

        public bool AutoScrollToEndLine { get; set; }

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

        public Color HyperlinkColor { get; set; }

        public Color LastUsedColor { get; private set; }

        public bool SecureReadLine { get; set; }

        public ConsoleState State { get; set; }

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

        private List<string> _recentList = new();

        public new void Clear()
        {
            base.Clear();
            _recentList.Clear();
        }

        public new void Dispose(bool disposing)
        {
            try
            {
                _writeLineQueue?.Dispose();
            }
            catch { }
            finally
            {
                _recentList.Clear();
                RecentCount = 0;
            }

            base.Dispose(disposing);
            GC.SuppressFinalize(this);
        }

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

        public void SetText(string message, Color? color = null)
        {
            color ??= ForeColor;
            LastUsedColor = SelectionColor = color.Value;
            SelectedText = message;
        }

        public void UpdateLine(int line, string message, Color? color = null)
        {
            ReadOnly = true;
            color ??= ForeColor;
            SelectLastLine();
            SetText(message, color);
        }

        public void Write(string message, Color? color = null, bool showTimeTag = false)
        {
            _writeLineQueue.Enqueue(new QueueTaskObject(message, color, showTimeTag));
        }

        public void WriteLine(string message, Color? color = null, bool showTimeTag = false)
        {
            Write(message + Environment.NewLine, color, showTimeTag);
        }

        internal bool OriginalWrite(string message, Color? color = null, bool showTimeTag = false)
        {
            if ((message.Length == _press_any_key_text.Length && !_press_any_key_text.Equals(message.Replace(Environment.NewLine, ""))) && State == ConsoleState.ReadKey)
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

        private void FConsole_TextChanged(object sender, EventArgs e)
        {
        }

        private void MultiplePaste()
        {
            ReadOnly = true;
            CurrentLine = Clipboard.GetText();
            InputEnable = false;
        }
    }
}
