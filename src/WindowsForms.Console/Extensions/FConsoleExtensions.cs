﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms.Console.Extensions
{
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
        public static async Task<char> ReadKey(this Form f, Color? color = null)
        {
            FConsole console = CheckFConsole(f);
            var line = await console.ReadKey(color);
            return line;
        }

        /// <summary>
        /// extension of readline
        /// </summary>
        /// <returns>
        /// </returns>
        public static async Task<string> ReadLine(this Form f)
        {
            FConsole console = CheckFConsole(f);
            var line = await console.ReadLine();
            return line;
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
            message = message ?? "";
            console.WriteLine(message, color, showTimeTag);
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
            message = message ?? "";
            console.Write(message, color, false);
        }

        private static FConsole CheckFConsole(Form f)
        {
            var fconsoles = FindControlByType<FConsole>(f, true);
            if (fconsoles.Count == 0)
                throw new Exception("this WinForm does not have any FConsole component");
            else if (fconsoles.Count > 1)
                throw new Exception("conflict occurs, more than one FConsole components detected.(supported only one component on Form)");
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
        private static List<T> FindControlByType<T>(Control mainControl, bool getAllChild = false) where T : Control
        {
            List<T> lt = new List<T>();
            for (int i = 0; i < mainControl.Controls.Count; i++)
            {
                if (mainControl.Controls[i] is T)
                    lt.Add((T)mainControl.Controls[i]);
                if (getAllChild)
                    lt.AddRange(FindControlByType<T>(mainControl.Controls[i], getAllChild));
            }
            return lt;
        }
    }
}