
using WindowsForms.Console.Enums;

namespace SampleFormApplication
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.btnAsyncWriteLine = new System.Windows.Forms.Button();
            this.btnAsyncWrite = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.fconsole1 = new WindowsForms.Console.FConsole();
            this.cmbTimeTag = new System.Windows.Forms.ComboBox();
            this.btnReadKeyColored = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(548, 46);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 35);
            this.button1.TabIndex = 1;
            this.button1.Text = "Write";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(238, 46);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 35);
            this.button2.TabIndex = 1;
            this.button2.Text = "WriteLine";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(912, 43);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(112, 35);
            this.button3.TabIndex = 1;
            this.button3.Text = "WriteLink";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(18, 8);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(210, 28);
            this.comboBox1.TabIndex = 2;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(399, 5);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(140, 35);
            this.button4.TabIndex = 1;
            this.button4.Text = "ReadOnly (ON)";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(238, 5);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(152, 35);
            this.button5.TabIndex = 4;
            this.button5.Text = "Change All Color";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(912, 5);
            this.button6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(112, 35);
            this.button6.TabIndex = 5;
            this.button6.Text = "ReadLine";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(548, 5);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(112, 35);
            this.button7.TabIndex = 5;
            this.button7.Text = "ReadKey";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // btnAsyncWriteLine
            // 
            this.btnAsyncWriteLine.ForeColor = System.Drawing.Color.Navy;
            this.btnAsyncWriteLine.Location = new System.Drawing.Point(399, 46);
            this.btnAsyncWriteLine.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAsyncWriteLine.Name = "btnAsyncWriteLine";
            this.btnAsyncWriteLine.Size = new System.Drawing.Size(140, 35);
            this.btnAsyncWriteLine.TabIndex = 1;
            this.btnAsyncWriteLine.Text = "Async WriteLine";
            this.btnAsyncWriteLine.UseVisualStyleBackColor = true;
            this.btnAsyncWriteLine.Click += new System.EventHandler(this.btnAsyncWrite_Click);
            // 
            // btnAsyncWrite
            // 
            this.btnAsyncWrite.ForeColor = System.Drawing.Color.Navy;
            this.btnAsyncWrite.Location = new System.Drawing.Point(669, 46);
            this.btnAsyncWrite.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAsyncWrite.Name = "btnAsyncWrite";
            this.btnAsyncWrite.Size = new System.Drawing.Size(112, 35);
            this.btnAsyncWrite.TabIndex = 1;
            this.btnAsyncWrite.Text = "Async Write";
            this.btnAsyncWrite.UseVisualStyleBackColor = true;
            this.btnAsyncWrite.Click += new System.EventHandler(this.btnAsyncWrite_Click_1);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(790, 45);
            this.btnClearAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(112, 35);
            this.btnClearAll.TabIndex = 1;
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // fconsole1
            // 
            this.fconsole1.Arguments = new string[0];
            this.fconsole1.AutoScrollToEndLine = true;
            this.fconsole1.BackColor = System.Drawing.Color.Black;
            this.fconsole1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fconsole1.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.fconsole1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(216)))), ((int)(((byte)(194)))));
            this.fconsole1.HyperlinkColor = System.Drawing.Color.Empty;
            this.fconsole1.Location = new System.Drawing.Point(0, 82);
            this.fconsole1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fconsole1.MaxLength = 32767;
            this.fconsole1.MinimumSize = new System.Drawing.Size(705, 308);
            this.fconsole1.Name = "fconsole1";
            this.fconsole1.ReadOnly = true;
            this.fconsole1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.fconsole1.SecureReadLine = true;
            this.fconsole1.Size = new System.Drawing.Size(1299, 471);
            this.fconsole1.State = WindowsForms.Console.Enums.ConsoleState.Writing;
            this.fconsole1.TabIndex = 3;
            this.fconsole1.Text = "WindowsForm Console";
            this.fconsole1.Title = "Form1";
            // 
            // cmbTimeTag
            // 
            this.cmbTimeTag.FormattingEnabled = true;
            this.cmbTimeTag.Location = new System.Drawing.Point(18, 46);
            this.cmbTimeTag.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbTimeTag.Name = "cmbTimeTag";
            this.cmbTimeTag.Size = new System.Drawing.Size(210, 28);
            this.cmbTimeTag.TabIndex = 2;
            // 
            // btnReadKeyColored
            // 
            this.btnReadKeyColored.ForeColor = System.Drawing.Color.Navy;
            this.btnReadKeyColored.Location = new System.Drawing.Point(669, 5);
            this.btnReadKeyColored.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReadKeyColored.Name = "btnReadKeyColored";
            this.btnReadKeyColored.Size = new System.Drawing.Size(234, 35);
            this.btnReadKeyColored.TabIndex = 5;
            this.btnReadKeyColored.Text = "ReadKey Colored";
            this.btnReadKeyColored.UseVisualStyleBackColor = true;
            this.btnReadKeyColored.Click += new System.EventHandler(this.button8_Click);
            // 
            // button8
            // 
            this.button8.ForeColor = System.Drawing.Color.Navy;
            this.button8.Location = new System.Drawing.Point(1067, 7);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(238, 74);
            this.button8.TabIndex = 6;
            this.button8.Text = "WriteLine from multi-threads  (massive Write call not recommended)";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 557);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.btnReadKeyColored);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.fconsole1);
            this.Controls.Add(this.cmbTimeTag);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnAsyncWriteLine);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnAsyncWrite);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox1;
        private WindowsForms.Console.FConsole fconsole1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button btnAsyncWriteLine;
        private System.Windows.Forms.Button btnAsyncWrite;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.ComboBox cmbTimeTag;
        private System.Windows.Forms.Button btnReadKeyColored;
        private System.Windows.Forms.Button button8;
    }
}

