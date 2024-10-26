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
    internal class PlaceLineElement : IExternalCommand
    {
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
           // FilteredElementCollector c2 = new FilteredElementCollector(doc);
            //FamilySymbol fs = c2.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().Cast<FamilySymbol>().First(w => w.Name == "line");
            //Level level = collector.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().Cast<Level>().First(l => l.Name == "Ground Floor");
            Level level = (Level)collector.OfClass(typeof(Level)).First(l => l.Name == "Level 1");
            ElementId levelid = level.Id;
            XYZ origin = new XYZ(0,0,0);
            XYZ direction = new XYZ(0,500,0);
            Line line =Line.CreateBound(origin, direction);
            try
            {
                using(Transaction trans = new Transaction(doc, "Place Line Element")) 
                { 
                    trans.Start();
                    Wall.Create(doc, line, levelid, false);
                    
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
