using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsForms.Console.Enums;

namespace WindowsForms.Console
{
    /// <summary>
    /// new Console GUI
    /// </summary>
    public class FConsole : RichTextBox
    {
        private const string _press_any_key_text = " Press Any Key...";
        private bool _pause;
        private AutoResetEvent autoResetEventInputEnable = new AutoResetEvent(false);
        private QueueTask _writeLineQueue = null;

        public FConsole()
        {
            KeyDown += FConsole_KeyDown;
            TextChanged += FConsole_TextChanged;
            LinkClicked += FConsole_LinkClicked;
            MouseDown += FConsole_MouseDown;
            MouseMove += FConsole_MouseMove;
            MouseUp += FConsole_MouseUp;
            InitializeFConsole();
            Form.CheckForIllegalCrossThreadCalls = false;//this is important to the async access for multi-threading
        }

        private void FConsole_MouseUp(object sender, MouseEventArgs e)
        {
            if (!InputEnable)
                Select(Text.Length, 0);
        }

        public new void Clear()
        {
            base.Clear();
            recentlist.Clear();
        }

        private void FConsole_MouseMove(object sender, MouseEventArgs e)
        {
            if (!InputEnable)
                Select(Text.Length, 0);
        }

        private void FConsole_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && InputEnable)
                MultiplePaste();
            else if (!InputEnable)
                Select(Text.Length, 0);
        }

        /// <summary>
        /// parameters
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// link color
        /// </summary>
        public Color HyperlinkColor { get; set; }

        /// <summary>
        /// states of console
        /// </summary>
        public ConsoleState State { get; set; }

        /// <summary>
        /// after calling readline method sets ReadOnly= true (default:true)
        /// </summary>
        public bool SecureReadLine { get; set; }

        /// <summary>
        /// auto scrolling to end when adds newLine (default:true)
        /// </summary>
        public bool AutoScrollToEndLine { get; set; }

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
                    return "";
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

        private int ReadPoint { get; set; }

        /// <summary>
        /// recent list of console
        /// </summary>
        private List<string> recentlist { get; set; }

        private int RecentCount { get; set; }

        private object _lockInputEnable = new object();

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

        private object _lockPause = new object();

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

        /// <summary>
        /// Initialize Console
        /// </summary>
        public void InitializeFConsole()
        {
            Name = "FConsole";
            Text = Name;
            Title = Name;
            Arguments = new string[0];
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
                //Parent.MinimumSize = MinimumSize;
                ((Form)Parent).WindowState = FormWindowState.Normal;
                Parent.BackColor = BackColor;
            }
            DetectUrls = true;
            recentlist = new List<string>();
            SecureReadLine = true;
            AutoScrollToEndLine = true;
            State = ConsoleState.Writing;
            LastUsedColor = SelectionColor;
            MaxLength = 32767;
            _writeLineQueue = new QueueTask(this);
        }

        public string RecentUndo()
        {
            if (recentlist.Count > 0)
            {
                RecentCount--;
                HistoryJumper();
                return recentlist[RecentCount];
            }
            else
            {
                return "";
            }
        }

        public void HistoryJumper()
        {
            if (RecentCount >= recentlist.Count)
                RecentCount = recentlist.Count - 1;
            else if (RecentCount < 0)
                RecentCount = 0;
        }

        public string RecentRedo()
        {
            if (recentlist.Count > 0)
            {
                RecentCount++;
                HistoryJumper();
                return recentlist[RecentCount];
            }
            else
            {
                return "";
            }
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
                if (recentlist.Count != 0)
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
                    WriteLine("");
                    InputEnable = false;
                    e.SuppressKeyPress = true;
                }
                else if (InputEnable == true && State == ConsoleState.ReadKey)
                {
                    ReadOnly = true;
                    CurrentKey = (char)e.KeyCode;
                    InputEnable = false;
                }
                else if ((int)e.KeyCode == (int)Keys.Escape && InputEnable == false)//esc exit
                {
                    Pause = false;
                }
                else if ((int)e.KeyCode == (int)Keys.Space && InputEnable == false)//space pause
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
        /// mouse right click event
        /// </summary>
        private void MultiplePaste()
        {
            ReadOnly = true;
            CurrentLine = Clipboard.GetText();
            InputEnable = false;
        }

        private object _lockReadKey = new object();

        /// <summary>
        /// stop line
        /// </summary>
        /// <returns>
        /// </returns>
        public async Task<char> ReadKey(Color? color = null)
        {
            if (State == ConsoleState.ReadKey)
                InputEnable = false;
            State = ConsoleState.Writing;
            Color Color = ForeColor;
            if (color.HasValue)
                Color = color.Value;
            WriteLine(_press_any_key_text, Color);
            return await Task.Run(() =>
            {
                var recentReadState = ReadOnly;
                CurrentKey = ' ';
                ReadPoint = Text.Length;
                InputEnable = true;
                ReadOnly = false;
                State = ConsoleState.ReadKey;
                autoResetEventInputEnable.WaitOne();
                if (SecureReadLine)
                    ReadOnly = true;
                else
                    ReadOnly = recentReadState;
                State = ConsoleState.Writing;
                return CurrentKey;
            });
        }

        private object _lockReadLine = new object();

        /// <summary>
        /// read line
        /// </summary>
        /// <returns>
        /// </returns>
        public async Task<string> ReadLine()
        {
            return await Task.Run(() =>
            {
                lock (_lockReadLine)
                {
                    var recentReadState = ReadOnly;
                    CurrentLine = "";
                    ReadPoint = TextLength;
                    InputEnable = true;
                    ReadOnly = false;
                    State = ConsoleState.ReadLine;
                    autoResetEventInputEnable.WaitOne();
                    Cursor = Cursors.IBeam;
                    if (SecureReadLine)
                        ReadOnly = true;
                    else
                        ReadOnly = recentReadState;
                    State = ConsoleState.Writing;
                    return CurrentLine;
                }
            });
        }

        private object _lockLines = new object();

        public string[] CurrentLines
        {
            get
            {
                lock (_lockLines)
                    return Lines;
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

        private readonly object _lockWrite = new object();
        private bool _inputEnable;

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
            _writeLineQueue.Enqueue(new QueueTaskObject(message, color, showTimeTag));
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
            //State = ConsoleState.Writing;
            return true;
        }

        public void SetText(string message, Color? color = null)
        {
            if (!color.HasValue)
                color = ForeColor;
            LastUsedColor = SelectionColor = color.Value;
            SelectedText = message;
        }

        /// <summary>
        /// stored last used color on console
        /// </summary>
        public Color LastUsedColor { get; private set; }

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

        /// <summary>
        /// link click router
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void FConsole_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void FConsole_TextChanged(object sender, EventArgs e)
        {
            //string re1 = "((?:http|https)(?::\\/{2}[\\w]+)(?:[\\/|\\.]?)(?:[^\\s\"]*))";
            //Regex hyperlink = new Regex(re1, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //bool update = false;
            //foreach (Match m in hyperlink.Matches(Text))
            //{
            //    Hyperlink hpl = new Hyperlink(m.Index, m.Index + m.Length, m.Value);
            //    if (Hyperlinks.Where(p => p.ToString() == hpl.ToString()).Count() == 0)
            //    {
            //        Select(m.Index, m.Length);
            //        SelectionColor = HyperlinkColor;
            //        Font f = SelectionFont;
            //        Font f2 = new Font(f.FontFamily, f.Size - 1.5f, FontStyle.Underline | FontStyle.Bold | FontStyle.Italic);
            //        SelectionFont = f2;
            //        Hyperlinks.Add(hpl);
            //        update = true;
            //    }
            //}
            //if (update)
            //    SelectLastLine();
        }

        public new void Dispose(bool disposing)
        {
            _writeLineQueue.Dispose();
            base.Dispose(disposing);
        }
    }
}