using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace EnglishStudy
{
    public partial class Form1 : Form
    {
        bool notify;
        bool EnglishEnabled = true;
        public Form1()
        {
            Windowmoving = false;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = new Point(500, 160);
            notify = true;
            EnglishStudy();
            this.FormBorderStyle = FormBorderStyle.None;
            this.AllowTransparency = true;
            this.BackColor = Color.Black;//цвет фона  
            this.TransparencyKey = this.BackColor;//он же будет заменен на прозрачный цвет

        }
        private void EnglishStudy()
        {
            try
            {
                if (EnglishEnabled)
                {
                    int count = File.ReadAllLines("EnglishPhrases.txt").Length;
                    Random rand = new Random();
                    int temp;
                    temp = rand.Next(count);
                    string secondLine = File.ReadLines("EnglishPhrases.txt", Encoding.Default).Skip(temp).First();
                    label1.Text = secondLine;
                }
            }
            catch (Exception) { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            EnglishStudy();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (notify == false)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon1.Visible = false;
                notify = true;
            }

        }
        private void Form1_Resize_1(object sender, EventArgs e)
        {
            if (notify == true)
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                    notifyIcon1.Visible = true;
                    notify = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Minimized;
            EnglishStudy();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        int MXold, MYold;
        bool Windowmoving;
        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Windowmoving)
            {
                Point tmp = new Point(this.Location.X, this.Location.Y);


                tmp.X += e.X - MXold;
                tmp.Y += e.Y - MYold;

                this.Location = tmp;
            }
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            Windowmoving = true;

            MXold = e.X;
            MYold = e.Y;
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            Windowmoving = false;
        }
    }
}
