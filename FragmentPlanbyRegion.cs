using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class FragmentPlanbyRegion : IExternalCommand
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
                
                MyRevitCommands.SelectLevel selectLevel = new SelectLevel(doc);
                //selectLevel.ShowDialog();
                if (selectLevel.ShowDialog() == DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }

                using (Transaction trans = new Transaction(doc, "plan"))
                {
                    trans.Start();
                    foreach (Level l in selectLevel.Levels)
                    {
                        
                        ViewPlan xx = ViewPlan.Create(doc, vt.Id, l.Id);
                        FilteredElementCollector vc = new FilteredElementCollector(doc);

                        //ViewPlan v = vc.OfClass(typeof(ViewPlan)).WhereElementIsNotElementType().Cast<ViewPlan>().FirstOrDefault();
                        xx.CropBox = region.get_BoundingBox(null);
                        xx.GetCropRegionShapeManager().SetCropShape(curves[0]);
                        xx.CropBoxActive = true; xx.CropBoxVisible = true;
                        xx.ViewTemplateId = selectLevel.Template.Id;
                        xx.Name = selectLevel.Forename +" " + l.Name;

                        

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
