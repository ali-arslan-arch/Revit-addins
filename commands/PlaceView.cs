using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class PlaceView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FilteredElementCollector col = new FilteredElementCollector(doc);
            ViewSheet sheet =col.OfCategory(BuiltInCategory.OST_Sheets).WhereElementIsNotElementType().Cast<ViewSheet>().First(x => x.Name == "plan");
            FilteredElementCollector col2 = new FilteredElementCollector(doc);
            Element view =col2.OfCategory(BuiltInCategory.OST_Views).WhereElementIsNotElementType().ToElements().First(x => x.Name == "new plan");
            XYZ p1 = new XYZ(sheet.Outline.Max.U, sheet.Outline.Max.V, 0);
            XYZ p2 = new XYZ(sheet.Outline.Min.U, sheet.Outline.Min.V, 0);
            XYZ center = new XYZ((p1.X - p2.X) / 2, (p1.Y - p2.Y) / 2, 0);
            try
            {
                using (Transaction trans = new Transaction(doc, "sheet"))
                {
                    trans.Start();
                    Viewport.Create(doc,sheet.Id,view.Id,center);
                    trans.Commit();
                  
                }
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
