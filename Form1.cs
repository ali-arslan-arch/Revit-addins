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
using static System.Windows.Forms.ListBox;

namespace MyRevitCommands
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public Document doc;
        public List<Level> Levels { get; private set; }
        public Form1(Document document)
        {
            InitializeComponent();
            doc = document;
            Levels = new List<Level>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.SelectionMode = SelectionMode.MultiExtended;
            FilteredElementCollector levelcol = new FilteredElementCollector(doc);
            IList<Element> list = levelcol.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements();
            List<Level> list2 = list.Cast<Level>().ToList();
            foreach (Level level in list2)
            {
                listBox1.Items.Add(new LevelWrapper(level));
            }
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(LevelWrapper lw in listBox1.SelectedItems) 
            {
                Level sl = lw.level;
                Levels.Add(sl);
            }
        }
    }

    public class LevelWrapper
    {
        public Level level { get; set; }
        public LevelWrapper(Level l)
        {
            level = l;
        }

        public override string ToString()
        {
            return level.Name;
        }
    }
}
