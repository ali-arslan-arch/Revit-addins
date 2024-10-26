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
    internal class PlaceFloor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            FloorType floorType = (FloorType)collector.OfClass(typeof(FloorType)).First(f => f.Name == "Generic 150mm");
            FilteredElementCollector col = new FilteredElementCollector(doc);
            Level level = (Level)col.OfClass(typeof(Level)).First(l => l.Name == "Level 1");
            XYZ p1 = new XYZ(0,0,0);
            XYZ p2 = new XYZ(500, 0, 0);
            XYZ p3 = new XYZ(500, 500, 0);
            XYZ p4 = new XYZ(0, 500, 0);
            Line l1 = Line.CreateBound(p1, p2);
            Line l2 = Line.CreateBound(p2, p3);
            Line l3 = Line.CreateBound(p3, p4);
            Line l4 = Line.CreateBound(p4, p1);
            

            CurveLoop cl = new CurveLoop();
            cl.Append(l1);
            cl.Append(l2);
            cl.Append(l3);
            cl.Append(l4);
            List<CurveLoop> loop = new List<CurveLoop> { cl};

            try
            {
                using (Transaction trans = new Transaction(doc,"Place Floor"))
                {
                    trans.Start();
                    Floor.Create(doc, loop, floorType.Id, level.Id);
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
