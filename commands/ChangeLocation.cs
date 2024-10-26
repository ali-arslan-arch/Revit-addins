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
    internal class ChangeLocation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                ElementId eleid = pickedobj.ElementId;
                Element ele = doc.GetElement(eleid);
                LocationPoint locp = ele.Location as LocationPoint;

                using (Transaction trans = new Transaction(doc, "Change Location"))
                {
                    trans.Start();
                    ele.Location.Move(new XYZ(8, 0, 0));

                    /*XYZ loc = locp.Point;
                    XYZ nl = new XYZ(loc.X + 3, loc.Y, loc.Z);
                    locp.Point = nl;  bu da hocanın yöntemi*/
                    
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
