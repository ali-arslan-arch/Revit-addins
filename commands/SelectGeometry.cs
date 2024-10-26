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
    internal class SelectGeometry : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                if(pickedobj != null)
                {
                    ElementId eleid = pickedobj.ElementId;
                    Element ele = doc.GetElement(eleid);
                    Options goptions = new Options();
                    goptions.DetailLevel = ViewDetailLevel.Fine;
                    GeometryElement ge = ele.get_Geometry(goptions);
                    double x = 0;
                    foreach(GeometryObject e in ge) 
                    {
                        Solid gsolid = e as Solid;
                        foreach(Face face in gsolid.Faces)
                        {
                            x += face.Area; 
                        }

                    }
                    
                    TaskDialog.Show("Area", string.Format("Area = {0}", x));
                }
                return Result.Succeeded;
            }
            catch  (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
