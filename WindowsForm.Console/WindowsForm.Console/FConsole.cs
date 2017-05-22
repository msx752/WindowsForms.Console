using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace WindowsForm.Console
{
    /// <summary>
    /// new Console GUI 
    /// </summary>
    public class FConsole : RichTextBox
    {
        public FConsole()
        {
            KeyDown += FConsole_KeyDown;
            TextChanged += FConsole_TextChanged;
            LinkClicked += FConsole_LinkClicked;
            MouseDown += FConsole_MouseDown;
            MouseMove += FConsole_MouseMove;
            MouseUp += FConsole_MouseUp;
            InitializeFConsole();
        }

        private void FConsole_MouseUp(object sender, MouseEventArgs e)
        {
            if (!InputEnable)
            { Select(Text.Length, 0); }
        }

        private void FConsole_MouseMove(object sender, MouseEventArgs e)
        {
            if (!InputEnable)
                Select(Text.Length, 0);
        }

        private void FConsole_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && InputEnable)
            {
                MultiplePaste();
            }
            else
            {
                if (!InputEnable)
                    Select(Text.Length, 0);
            }
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
        /// console title (not necessary :) ) 
        /// </summary>
        public string Title { get { if (Parent != null) return Parent.Text; else return ""; } set { if (Parent != null) Parent.Text = value; } }

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

        /// <summary>
        /// recent list of console limiter 
        /// </summary>
        private int RecentCount { get; set; }

        /// <summary>
        /// read mode checker 
        /// </summary>
        private bool InputEnable { get; set; }

        /// <summary>
        /// console pause checker 
        /// </summary>
        private bool Pause { get; set; }

        public void InitializeFConsole()
        {
            Name = "FConsole";
            Text = Name;
            Title = Name;
            Arguments = new string[0];
            BackColor = Color.Black;
            ForeColor = Color.FromArgb(0xdf, 0xd8, 0xc2);
            Dock = DockStyle.Fill;
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
                    CurrentLine = Lines[Lines.Count() - 1];
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

        /// <summary>
        /// stop line 
        /// </summary>
        /// <returns>
        /// </returns>
        public char ReadKey()
        {
            CurrentKey = ' ';
            ReadPoint = Text.Length;
            InputEnable = true;
            ReadOnly = false;
            State = ConsoleState.ReadKey;
            while (InputEnable) Thread.Sleep(1);

            return CurrentKey;
        }

        /// <summary>
        /// stop line 
        /// </summary>
        /// <returns>
        /// </returns>
        public string ReadLine()
        {
            CurrentLine = "";
            ReadPoint = TextLength;
            InputEnable = true;
            ReadOnly = false;
            State = ConsoleState.ReadLine;
            while (InputEnable) Thread.Sleep(1);
            Cursor = Cursors.IBeam;
            return CurrentLine;
        }

        /// <summary>
        /// last line gets 
        /// </summary>
        public void SelectLastLine()
        {
            if (Lines.Any())
            {
                var line = Lines.Count() - 1;
                var s1 = GetFirstCharIndexOfCurrentLine();
                var s2 = line < Lines.Count() - 1 ?
                          GetFirstCharIndexFromLine(line + 1) - 1 :
                          Text.Length;
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

        /// <summary>
        /// write function 
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="color">
        /// </param>
        public void Write(string message, Color? color = null)
        {
            while (Pause) Thread.Sleep(1);
            Select(TextLength, 0);
            SetText(message, color);
            DeselectAll();
        }

        /// <summary>
        /// it will be private method 
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="color">
        /// </param>
        public void SetText(string message, Color? color = null)
        {
            if (!color.HasValue)
                color = ForeColor;
            SelectionColor = color.Value;
            SelectedText = message;
        }

        /// <summary>
        /// writeline function 
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="color">
        /// </param>
        public void WriteLine(string message, Color? color = null)
        {
            Write(message + Environment.NewLine, color);
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
    }
}