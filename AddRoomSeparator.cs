using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Creation;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class AddRoomSeparator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            FilteredElementCollector col = new FilteredElementCollector(doc,doc.ActiveView.Id);
            List<Element> eles = col.OfCategory(BuiltInCategory.OST_Rooms).ToElements().ToList();
            
            
            try
            {
                using(Transaction trans = new Transaction(doc,"room separator"))
                {
                    trans.Start();
                    foreach (Element e in eles)
                    {

                        SpatialElement spa = e as SpatialElement;
                        
                        if (spa.Area == 0)
                        {
                            continue;
                        }
                        if (spa.Location == null)
                        {
                            continue;
                        }



                        Level l = spa.Level;
                        if (l == null)
                        {
                            message = "SpatialElement does not have a valid Level.";
                            return Result.Failed;
                        }

                        FilteredElementCollector viewcol = new FilteredElementCollector(doc);
                        ViewPlan vp = viewcol.OfClass(typeof(ViewPlan))
                                             .WhereElementIsNotElementType()
                                             .ToElements().Cast<ViewPlan>().FirstOrDefault(x => x.get_Parameter(BuiltInParameter.PLAN_VIEW_LEVEL).AsValueString() == l.Name);
                        if (vp == null)
                        {
                            message = "No ViewPlan found for the selected level.";
                            return Result.Failed;
                        }

                        IList<IList<BoundarySegment>> bss = spa.GetBoundarySegments(new SpatialElementBoundaryOptions());


                        CurveArray curves = new CurveArray();
                        foreach (IList<BoundarySegment> segs in bss)
                        {
                            foreach (BoundarySegment seg in segs)
                            {
                                Curve c = seg.GetCurve();
                                curves.Append(c);
                            }
                        }

                        
                        
                            
                        SketchPlane sketchPlane = SketchPlane.Create(doc, l.Id);
                        doc.Create.NewRoomBoundaryLines(sketchPlane, curves, vp);
                            
                        

                    }
                    trans.Commit();
                }
                
                return Result.Succeeded; ;
            }
            catch(Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
