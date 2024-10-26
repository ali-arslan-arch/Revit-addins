using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class CroppedPlanBasedOnFamilyDimensions : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document document = uidoc.Document;
            try
            {
                FormSelectTemplate temp = new FormSelectTemplate(document);
                //temp.ShowDialog();
                if (temp.ShowDialog() == DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }
                FilteredElementCollector collector = new FilteredElementCollector(document, document.ActiveView.Id);
                IList<FamilyInstance> eles = collector.OfClass(typeof(FamilyInstance)).ToElements().Cast<FamilyInstance>().ToList();
                List<FamilyInstance> fgs = eles.GroupBy(x => x.Symbol.Id).Select(g => g.First()).ToList();
                FilteredElementCollector col = new FilteredElementCollector(document);
                ViewFamilyType vt = col.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().First(x => x.ViewFamily == ViewFamily.FloorPlan);
                foreach (FamilyInstance fi in fgs)
                {
                    //FamilySmbol ve type mark parametresi çekme
                    Element ele = fi as Element;
                    FamilySymbol fs = fi.Symbol;
                    Parameter p2 = fs.get_Parameter(BuiltInParameter.WINDOW_TYPE_ID);
                    string a = p2.AsValueString();
                    //FamilySmbol ve type mark parametresi çekme

                    //Boundingbox için instance'ın oldugu leveldan bir plan view çekme
                    ElementId lid = ele.LevelId;
                    FilteredElementCollector viewcol = new FilteredElementCollector(document).OfClass(typeof(ViewPlan));
                    ViewPlan plan = viewcol.Cast<ViewPlan>().FirstOrDefault(vp => vp.GenLevel.Id == lid);
                    //Boundingbox için instance'ın oldugu leveldan bir plan view çekme

                    //Boundingbox Plan cropu belirleme
                    BoundingBoxXYZ bo = ele.get_BoundingBox(plan);
                    BoundingBoxXYZ crop = bo;
                    Double dx = (bo.Max.X - bo.Min.X) / 4;
                    Double dy = (bo.Max.Y - bo.Min.Y) / 4;
                    XYZ yenimax = new XYZ(bo.Max.X + dx, bo.Max.Y + dy, bo.Max.Z);
                    XYZ yenimin = new XYZ(bo.Min.X - dx, bo.Min.Y - dy, bo.Min.Z);
                    crop.Max = yenimax; crop.Min = yenimin;
                    //Boundingbox Plan cropu belirleme

                    
                    
                    using (Transaction trans = new Transaction(document, "plan"))
                    {
                        trans.Start();
                        ViewPlan vp = ViewPlan.Create(document, vt.Id, ele.LevelId);
                        vp.CropBox = crop;
                        vp.CropBoxActive = true;
                        vp.Name = a;
                        vp.ViewTemplateId = temp.Template.Id;
                        trans.Commit();

                    }
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
