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
    public partial class SelectLevel : System.Windows.Forms.Form
    {
        public Document doc { get; set; }
        public List<Level> Levels { get; set; }
        public Autodesk.Revit.DB.View Template { get; set; }
        public string Forename { get; set; }
        public SelectLevel(Document d)
        {
            doc = d;
            Levels = new List<Level>();
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            foreach(LevelWrapper ll in listBox1.SelectedItems)
            {
                Levels.Add(ll.level);
                    
            }
            ViewWrapper xv = listBox2.SelectedItem as ViewWrapper;
            Template = xv.view;
            Forename = textBox1.Text;
            this.DialogResult = DialogResult.OK;
            Close();

        }

        private void SelectLevel_Load(object sender, EventArgs e)
        {
            listBox1.SelectionMode = SelectionMode.MultiExtended;
            listBox2.SelectionMode = SelectionMode.One;
            FilteredElementCollector col = new FilteredElementCollector(doc);
            List<Level> list =col.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements().Cast<Level>().ToList();
            foreach (Level level in list)
            {
                LevelWrapper wrapper = new LevelWrapper(level);
                listBox1.Items.Add(wrapper);
            }
            FilteredElementCollector colt = new FilteredElementCollector(doc);
            List<Autodesk.Revit.DB.View> temps = colt.OfClass(typeof(Autodesk.Revit.DB.View)).ToElements().Cast<Autodesk.Revit.DB.View>().Where(x => x.IsTemplate == true).ToList();
            foreach(Autodesk.Revit.DB.View v  in temps)
            {
                listBox2.Items.Add(new ViewWrapper(v));
            }

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
    public class LevelWrapper
    {
        public Level level { get; set; }
        public LevelWrapper(Level le)
        {
            level = le;
        }

        public override string ToString()
        {
            return level.Name;
        }
    }
    public class ViewWrapper
    {
        public Autodesk.Revit.DB.View view { get; set; }
        public ViewWrapper(Autodesk.Revit.DB.View le)
        {
            view = le;
        }

        public override string ToString()
        {
            return view.Name;
        }
    }
}
