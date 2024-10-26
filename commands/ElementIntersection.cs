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
    internal class ElementIntersection : IExternalCommand
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
                    ElementId eleid = pickedobj.ElementId;
                    Element ele = doc.GetElement(eleid);
                    Options op = new Options();
                    op.DetailLevel = ViewDetailLevel.Fine;
                    GeometryElement ge = ele.get_Geometry(op);


                    
                    Solid sol = null;

                    foreach(GeometryObject go in ge)
                    {
                        GeometryInstance gi = go as GeometryInstance;
                        GeometryElement gel = gi.GetInstanceGeometry();
                        foreach (GeometryObject g in gel)
                        {
                            sol = g as Solid;
                        }
                    }
                    
                    
                    FilteredElementCollector col = new FilteredElementCollector(doc);
                    ElementIntersectsSolidFilter filter = new ElementIntersectsSolidFilter(sol);
                    ICollection<ElementId> list = col.WherePasses(filter).ToElementIds();
                    list.Remove(eleid);
                    

                    uidoc.Selection.SetElementIds(list);
                    


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
