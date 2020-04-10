using SimplexLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace simplex1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> lst = new List<string>();
            //lst.Add(textBox1.Text);
            richTextBox1.Text.Split('\r','\n');

            //simplex s = new simplex(true, textBox1.Text,lst);
            //s.Solve();
            //richTextBox2.Text = s.fun2;

            syntax s = new syntax();
            s.syn = textBox1.Text;
            s.GetElem();
            foreach (string item in richTextBox1.Text.Split('\r', '\n'))
            {
                s.syn = item;
                s.GetElem();
            }
            
        }
    }
}
