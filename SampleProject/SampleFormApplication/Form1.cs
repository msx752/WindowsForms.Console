using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsForms.Console.Extensions;

namespace SampleFormApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//this is important for async threading
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Write("this is an example line text!.",
                Color.FromName(comboBox1.Items[comboBox1.SelectedIndex].ToString()));//used SelectedIndex for preventing a crash
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add(nameof(Color.Aqua));
            comboBox1.Items.Add(nameof(Color.Red));
            comboBox1.Items.Add(nameof(Color.Blue));
            comboBox1.Items.Add(nameof(Color.White));
            comboBox1.Items.Add(nameof(Color.Yellow));
            comboBox1.Items.Add(nameof(Color.LightBlue));
            comboBox1.Items.Add(nameof(Color.DarkSeaGreen));
            comboBox1.SelectedIndex = 3;

            fconsole1.Text = "";

            cmbTimeTag.Items.Add("Hide Time Tag");
            cmbTimeTag.Items.Add("Show Time Tag");
            cmbTimeTag.SelectedIndex = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            fconsole1.ReadOnly = !fconsole1.ReadOnly;
            if (fconsole1.ReadOnly)
            {
                button4.Text = "ReadOnly (ON)";
            }
            else
            {
                button4.Text = "ReadOnly (OFF)";

                MessageBox.Show("ReadOnly=False is not recommended");
                //MessageBox.Show("some of features may not work with readonly mode such as readline and read key, but now it is okey until you see any error :)");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WriteLine("this is an example line text!.",
                Color.FromName(comboBox1.Items[comboBox1.SelectedIndex].ToString()), cmbTimeTag.SelectedIndex == 1);////used SelectedIndex for preventing a crash
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.WriteLine(" http://github.com/msx752/WindowsForms.Console",
            Color.FromName(comboBox1.Items[comboBox1.SelectedIndex].ToString()), cmbTimeTag.SelectedIndex == 1);//used SelectedIndex for preventing a crash)
        }

        private void button5_Click(object sender, EventArgs e)
        {
            fconsole1.ForeColor = Color.FromName(comboBox1.Items[comboBox1.SelectedIndex].ToString());
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            var line = await this.ReadLine();
            MessageBox.Show($"Value of ReadLine is '{line}'");
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            var key = await this.ReadKey();
            MessageBox.Show($"Value of ReadKey is '{key}'");
        }

        private void btnAsyncWrite_Click(object sender, EventArgs e)
        {
            fconsole1.Clear();
            int counter = 1;
            this.WriteLine($"[Minimum Number:\t1\t]",
                Color.FromName(comboBox1.Items[0].ToString()), cmbTimeTag.SelectedIndex == 1);
            List<Task> tlist = new List<Task>();
            for (int i = 0; i < 7; i++)
            {
                int current = i;
                int count = counter;
                counter++;
                tlist.Add(Task.Run(() =>
                {
                    this.WriteLine($"\t[Number:\t{count}\t]",
                        Color.FromName(comboBox1.Items[current].ToString()), cmbTimeTag.SelectedIndex == 1);////used SelectedIndex for preventing a crash
                }));
            }
            for (int i = 0; i < 7; i++)
            {
                int current = i;
                int count = counter;
                counter++;
                tlist.Add(Task.Run(() =>
                {
                    this.WriteLine($"\t[Number:\t{count}\t]",
                        Color.FromName(comboBox1.Items[current].ToString()), cmbTimeTag.SelectedIndex == 1);////used SelectedIndex for preventing a crash
                }));
            }
            Task.WaitAll(tlist.ToArray());// if you dont do this, writeline squence may be unstable
            this.WriteLine($"[Maximum Number:\t14\t]",
                Color.FromName(comboBox1.Items[0].ToString()), cmbTimeTag.SelectedIndex == 1);
        }

        private void btnAsyncWrite_Click_1(object sender, EventArgs e)
        {
            fconsole1.Clear();
            List<Task> tlist = new List<Task>();
            this.WriteLine($"[Minimum Number:\t1\t]",
                Color.FromName(comboBox1.Items[0].ToString()), cmbTimeTag.SelectedIndex == 1);
            for (int i = 0; i < 7; i++)
            {
                int current = i;
                tlist.Add(Task.Run(() =>
                {
                    this.Write($"{current + 1}\t",
                        Color.FromName(comboBox1.Items[current].ToString()));////used SelectedIndex for preventing a crash
                }));
            }
            Task.WaitAll(tlist.ToArray());// if you dont do this, writeline squence may be unstable
            this.WriteLine();
            this.WriteLine($"[Maximum Number:\t7\t]",
                Color.FromName(comboBox1.Items[0].ToString()), cmbTimeTag.SelectedIndex == 1);
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            fconsole1.Clear();
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            var key = await this.ReadKey(Color.FromName(comboBox1.Items[comboBox1.SelectedIndex].ToString()));
            MessageBox.Show($"Value of ReadKey is '{key}'");
        }
    }
}