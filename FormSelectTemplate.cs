
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MyRevitCommands
{
    public partial class FormSelectTemplate : System.Windows.Forms.Form
    {
        public Document doc { get; set; }
        public bool Typemark;
        
        public string ViewName;
        public string Cutpoint;
        
        public Autodesk.Revit.DB.View Template { get; set; }
        
        public FormSelectTemplate(Document d)
        {
            doc = d;
            InitializeComponent();
        }

        private void FormSelectTemplate_Load(object sender, EventArgs e)
        {
            FilteredElementCollector colt = new FilteredElementCollector(doc);
            List<Autodesk.Revit.DB.View> temps = colt.OfClass(typeof(Autodesk.Revit.DB.View)).ToElements().Cast<Autodesk.Revit.DB.View>().Where(x => x.IsTemplate == true).ToList();
            foreach (Autodesk.Revit.DB.View v in temps)
            {
                listBox1.Items.Add(new ViewWrapper(v));
                listBox1.Sorted = true;
            }
            listBox1.SelectionMode = SelectionMode.One;
           

        }
        public void CutPointVisible()
        {
            textBox2.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ViewWrapper xv = listBox1.SelectedItem as ViewWrapper;
            Template = xv.view;

            
            
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Typemark = true;
            }
            else { Typemark = false; }
            

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ViewName = textBox1.Text;

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Cutpoint = textBox2.Text.Replace('.',',');
        }

        
    }
    
}
