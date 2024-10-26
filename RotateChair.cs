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
    internal class RotateChair : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                Reference rmasa = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element masa = doc.GetElement(rmasa);
                Reference rsand = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element sand = doc.GetElement(rsand);
                LocationPoint lp = masa.Location as LocationPoint;
                XYZ p1 = lp.Point;
                XYZ p2 = new XYZ(p1.X, p1.Y, p1.Z + 10);
                Line axis = Line.CreateBound(p1, p2);
                double angle = Math.PI / 3;
                using(Transaction trans = new Transaction(doc, "rotate"))
                {
                    trans.Start();
                    ElementTransformUtils.CopyElement(doc, sand.Id, new XYZ(0, 0, 0));
                    for (int i = 0; i < 5; i++)
                    {
                        
                        ElementTransformUtils.RotateElement(doc, sand.Id, axis, angle);
                        if (i < 4)
                        {
                            ElementTransformUtils.CopyElement(doc, sand.Id, new XYZ(0, 0, 0));
                        }
                        

                        
                    }
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
