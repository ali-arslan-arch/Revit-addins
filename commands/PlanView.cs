using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class PlanView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            try
            {

                FilteredElementCollector col = new FilteredElementCollector(doc);
                FilteredElementCollector col2 = new FilteredElementCollector(doc);
                ViewFamilyType vt = col.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().First(x => x.ViewFamily == ViewFamily.FloorPlan);
                Level l = col2.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().Cast<Level>().First(a => a.Name == "Level 1");
                using (Transaction trans = new Transaction(doc, "plan"))
                {
                    trans.Start();
                    ViewPlan vl = ViewPlan.Create(doc, vt.Id, l.Id);
                    vl.Name = "Our First Plan";
                    trans.Commit();

                }
                
                return Result.Succeeded;
            }
            catch(Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
