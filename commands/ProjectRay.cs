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
    [TransactionAttribute(TransactionMode.ReadOnly)]
    internal class ProjectRay : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document document = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element ele = document.GetElement(pickedobj);
                LocationPoint x = ele.Location as LocationPoint;
                XYZ p1 = x.Point;
                XYZ p2 = new XYZ(0, 0, 1);
               
                ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Roofs);
                ReferenceIntersector intersector = new ReferenceIntersector(filter, FindReferenceTarget.Face, (View3D)document.ActiveView);
                ReferenceWithContext rc = intersector.FindNearest(p1,p2);
                Reference r = rc.GetReference();
                XYZ p3 = r.GlobalPoint;
                double d = p1.DistanceTo(p3);
                TaskDialog.Show("a", string.Format("Distance: {0}",d) );
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
