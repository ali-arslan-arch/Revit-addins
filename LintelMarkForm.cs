using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyRevitCommands
{
    public partial class LintelMarkForm : Form
    {
        public string korpus { get; set; }
        public int number { get; set; }
        
        public LintelMarkForm()
        {
            InitializeComponent();
        }

        private void LintelMarkForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = "К1";
            textBox2.Text = "1";

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            korpus = textBox1.Text;

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            number = Convert.ToInt32(textBox2.Text);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
