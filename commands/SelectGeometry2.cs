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
    internal class SelectGeometry2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element); 
                
                if (pickedobj != null)
                {
                    ElementId elementId = pickedobj.ElementId;
                    Element ele = doc.GetElement(elementId);
                    Options op = new Options();
                    op.DetailLevel = ViewDetailLevel.Fine;
                    GeometryElement l = ele.get_Geometry(op);
                    Double x = 0;
                    foreach (GeometryObject e in l)
                    {
                        Solid s = e as Solid;
                        foreach(Face f in s.Faces)
                        {
                            x += f.Area;
                        }

                    }
                    TaskDialog.Show("Select Geo", string.Format("Area = {0}", x));
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
