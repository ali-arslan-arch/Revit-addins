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
    internal class Section : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element ele = doc.GetElement(pickedobj);
                LocationPoint lp = ele.Location as LocationPoint;
                BoundingBoxXYZ bo = ele.get_BoundingBox(null);
                FamilyInstance f = ele as FamilyInstance;
                Element host = f.Host;
                Wall wall = host as Wall; 
                LocationCurve lc = wall.Location as LocationCurve;
                Line l = lc.Curve as Line;
                XYZ p = l.GetEndPoint(0); 
                XYZ q = l.GetEndPoint(1);
                XYZ v = q - p;
                Transform t = new Transform(Transform.Identity);
                t.Origin = lp.Point;
                t.BasisZ = -f.FacingOrientation.Normalize();
                t.BasisX = v.Normalize();
                t.BasisY = XYZ.BasisZ;
                
                FamilySymbol symbol = f.Symbol;

                XYZ min = bo.Min;
                XYZ max = bo.Max;

                // BasisX vektörünü alıyoruz (pencerenin yerel X eksenini temsil eder)
                XYZ basisX = bo.Transform.BasisX.Normalize();

                // Min ve Max noktaları arasındaki fark vektörünü bulun
                XYZ vectorBetween = max - min;

                // Bu fark vektörünü basisX yönünde projekte ederek genişliği bulun
                double width = vectorBetween.DotProduct(basisX);
                double a = width*304.8;

                TaskDialog.Show("asd", a.ToString());


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
