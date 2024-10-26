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
    internal class FragmentPlanByCurve : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element region = doc.GetElement(pickedobj);
                FilledRegion fregion = region as FilledRegion;
                IList<CurveLoop> curves = fregion.GetBoundaries();

                FilteredElementCollector vtcol = new FilteredElementCollector(doc);
                ViewFamilyType vt = vtcol.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().Where(y => y.ViewFamily == ViewFamily.FloorPlan).First();
                FilteredElementCollector levelcol = new FilteredElementCollector(doc);
                Level l1 = levelcol.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().Cast<Level>().First(x => x.Name == "Level 1");
                Level l2 = levelcol.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().Cast<Level>().First(x => x.Name == "Level 2");
                Level l3 = levelcol.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().Cast<Level>().First(x => x.Name == "Level 3");
                List<ElementId> ids = new List<ElementId>();
                ids.Add(l1.Id); ids.Add(l2.Id); ids.Add(l3.Id); 
                using(Transaction trans = new Transaction(doc, "plan"))
                {
                    foreach(ElementId id in ids)
                    {
                        trans.Start();
                        ViewPlan xx = ViewPlan.Create(doc, vt.Id,id );
                        FilteredElementCollector vc = new FilteredElementCollector(doc);
                        
                        ViewPlan v = vc.OfClass(typeof(ViewPlan)).WhereElementIsNotElementType().Cast<ViewPlan>().FirstOrDefault();
                        xx.CropBox = region.get_BoundingBox(v);
                        xx.GetCropRegionShapeManager().SetCropShape(curves[0]);
                        xx.CropBoxActive = true; xx.CropBoxVisible = true;
                        trans.Commit();

                    }
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
