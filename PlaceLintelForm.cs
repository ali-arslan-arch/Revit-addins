
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
    public partial class PlaceLintelForm : System.Windows.Forms.Form
    {
        public Document doc { get; set; }
        public FamilySymbol lintelfor200 { get; set; }
        public FamilySymbol lintelfor100 { get; set; }
        public FamilySymbol lintelfor250 { get; set; }
        public string parametername2001 { get; set; }
        public string parametername2002 { get; set; }
        public string parametername1001 { get; set; }
        public string parametername1002 { get; set; }
        public double offsetdifference { get; set; }
        public double lessoffset { get; set; }
        public double greatoffset { get; set; }
        
        public string parametername200leftank { get; set; }
        public string parametername200rightank { get; set; }
        public string parametername100leftank { get; set; }
        public string parametername100rightank { get; set; }
        public string parametername200length { get; set; }
        public string parametername100length { get; set; }

        //private ExternalEvent _externalEvent;


        public PlaceLintelForm(Document d)
        {

            //this.FormClosing += MyForm_FormClosing;
            doc = d;
            InitializeComponent();
        }

        private void PlaceLintelForm_Load(object sender, EventArgs e)
        {

            textBox2001.Text = "ПРИВЯЗКА ЛИНИИ ПРОЕМА СЛЕВА";
            textBox2002.Text = "ПРИВЯЗКА ЛИНИИ ПРОЕМА СПРАВА";
            text200leftank.Text = "СЛЕВА";
            text200rightank.Text = "СПРАВА";
            textBox1001.Text = "ПРИВЯЗКА ЛИНИИ ПРОЕМА_СЛЕВА";
            textBox1002.Text = "ПРИВЯЗКА ЛИНИИ ПРОЕМА_СПРАВА";
            text100leftank.Text = "СЛЕВА";
            text100rightank.Text = "СПРАВА";
            textBoxdiffer.Text = "1500";
            smalloffset.Text = "150";
            bigoffset.Text = "200";
            text100length.Text = "020_Длина_перемычки";
            text200length.Text = "020_Длина_перемычки";

            


            FilteredElementCollector framingcol1 = new FilteredElementCollector(doc);
            List<FamilySymbol> list2 = framingcol1.OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsElementType().ToElements().Cast<FamilySymbol>().ToList();
            foreach (FamilySymbol famsym in list2)
            {
                SymbolWrapper sw = new SymbolWrapper(famsym);
                comboBox1.Items.Add(sw);

            }
            FilteredElementCollector framingcol2 = new FilteredElementCollector(doc);
            List<FamilySymbol> list3 = framingcol2.OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsElementType().ToElements().Cast<FamilySymbol>().ToList();
            foreach (FamilySymbol famsymb in list3)
            {
                SymbolWrapper swr = new SymbolWrapper(famsymb);
                comboBox2.Items.Add(swr);
                comboBox3.Items.Add(swr);

            }

        }
        /*private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Kullanıcı formu kapatırsa add-in'i iptal et
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // İşlem iptal edilir
                _externalEvent.Raise();
            }
        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            SymbolWrapper sw1 = comboBox1.SelectedItem as SymbolWrapper;
            lintelfor200 = sw1.symbol;
            SymbolWrapper sw2 = comboBox2.SelectedItem as SymbolWrapper;
            lintelfor100 = sw2.symbol;
            SymbolWrapper sw3 = comboBox3.SelectedItem as SymbolWrapper;
            lintelfor250 = sw3.symbol;
            parametername2001 = textBox2001.Text;
            parametername1001 = textBox1001.Text;
            parametername2002 = textBox2002.Text;
            parametername1002 = textBox1002.Text;
            offsetdifference = Double.Parse(textBoxdiffer.Text) / 304.8;
            lessoffset = Double.Parse(smalloffset.Text) / 304.8;
            greatoffset = Double.Parse(bigoffset.Text) / 304.8;
            parametername200leftank = text200leftank.Text;
            parametername200rightank = text200rightank.Text;
            parametername100leftank = text100leftank.Text;
            parametername100rightank = text100rightank.Text;
            parametername200length = text200length.Text;
            parametername100length = text100length.Text;
            

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void bigoffset_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void smalloffset_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBoxdiffer_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1002_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2002_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBoxthickness_TextChanged(object sender, EventArgs e)
        {

        }

        private void text100rightank_TextChanged(object sender, EventArgs e)
        {

        }

        private void text100leftank_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void text200rightank_TextChanged(object sender, EventArgs e)
        {

        }

        private void text200leftank_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
    public class SymbolWrapper
    {
        public FamilySymbol symbol { get; set; }
        public SymbolWrapper(FamilySymbol fs)
        {
            symbol = fs;
        }

        public override string ToString()
        {
            return symbol.FamilyName + " - " + symbol.Name;
        }
    }
}
