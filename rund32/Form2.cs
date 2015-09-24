using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace rund32
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "qwertyuiop" && textBox2.Text == "123456789" && textBox3.Text == "disable")
            {

                MessageBox.Show("Virus has been disabled");
                  Form1.myfunc();

                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}
