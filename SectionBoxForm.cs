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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    public partial class SectionBoxForm : System.Windows.Forms.Form
    {
        public Document doc { get; set; }
        public View3D view { get; set; }
        public SectionBoxForm(Document d)
        {
            InitializeComponent();
            doc = d;
        }

        private void SectionBoxForm_Load(object sender, EventArgs e)
        {
            FilteredElementCollector viewcol = new FilteredElementCollector(doc);
            IList<View3D> views = viewcol.OfClass(typeof(View3D)).WhereElementIsNotElementType().ToElements().Cast<View3D>().ToList();
            foreach(View3D v in views)
            {
                comboBox1.Items.Add(new ViewWrapper(v));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ViewWrapper vr = (ViewWrapper)comboBox1.SelectedItem;
            view = vr.view as View3D;
            this.DialogResult = DialogResult.OK;
            Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
