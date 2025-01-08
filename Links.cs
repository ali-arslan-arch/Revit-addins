using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    public partial class Links : System.Windows.Forms.Form
    {
        public Document doc { get; set; }
        public RevitLinkInstance rl { get; set; } 
        public Links(Document d)
        {
            doc = d;
            InitializeComponent();
        }

        private void Links_Load(object sender, EventArgs e)
        {
            FilteredElementCollector colt = new FilteredElementCollector(doc);
            List<Autodesk.Revit.DB.RevitLinkInstance> links = colt.OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().ToElements().Cast<Autodesk.Revit.DB.RevitLinkInstance>().ToList();
            foreach (Autodesk.Revit.DB.RevitLinkInstance l in links)
            {
                listBox1.Items.Add(new LinkWrapper(l));
            }
            listBox1.SelectionMode = SelectionMode.One;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            LinkWrapper xv = listBox1.SelectedItem as LinkWrapper;
            rl = xv.link;

            this.DialogResult = DialogResult.OK;
            Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
    public class LinkWrapper
    {
        public RevitLinkInstance link { get; set; }
        public LinkWrapper(RevitLinkInstance l)
        {
            link = l;
        }

        public override string ToString()
        {
            return link.Name;
        }
    }
}
