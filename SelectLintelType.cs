using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;


namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    internal class SelectLintelType : IExternalCommand
    {
        public static FamilySymbol lintelfor200 { get; set; }
        public static FamilySymbol lintelfor100 { get; set; }
        public static FamilySymbol lintelfor250 { get; set; }
        public static FamilySymbol lintelfor150 { get; set; }

        public static string parametername2001 { get; set; }
        public static string parametername2002 { get; set; }
        public static string parametername1001 { get; set; }
        public static string parametername1002 { get; set; }
        public static double offsetdifference { get; set; }
        public static double lessoffset { get; set; }
        public static double greatoffset { get; set; }

        public static string parametername200leftank { get; set; }
        public static string parametername200rightank { get; set; }
        public static string parametername100leftank { get; set; }
        public static string parametername100rightank { get; set; }



        public static string parametername200length { get; set; }
        public static string parametername100length { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                PlaceLintelForm form = new PlaceLintelForm(doc);
                if (form.ShowDialog() == DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }
                lintelfor200 = form.lintelfor200;
                lintelfor100 = form.lintelfor100;
                lintelfor250 = form.lintelfor250;
                lintelfor150 = form.lintelfor150;
                parametername2001 = form.parametername2001;
                parametername2002 = form.parametername2002;
                parametername1001 = form.parametername1001;
                parametername1002 = form.parametername1002;
                offsetdifference = form.offsetdifference;
                lessoffset = form.lessoffset;
                greatoffset = form.greatoffset;
                parametername200length = form.parametername200length;
                parametername100length = form.parametername100length;
                parametername200leftank  = form.parametername200leftank;
                parametername200rightank = form.parametername200rightank;
                parametername100leftank = form.parametername100leftank;
                parametername100rightank = form.parametername100rightank;
                return Result.Succeeded;

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
