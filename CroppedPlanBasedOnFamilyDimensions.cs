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
using static Autodesk.Revit.DB.SpecTypeId;

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
                if (temp.Template == null)
                {
                    message = "No template selected.";
                    return Result.Failed;
                }
                List<FamilyInstance> allInstances = new List<FamilyInstance>();

                FilteredElementCollector collector = new FilteredElementCollector(document, document.ActiveView.Id);
                IList<FamilyInstance> eles = collector.OfClass(typeof(FamilyInstance)).ToElements().Cast<FamilyInstance>().ToList();
                foreach (FamilyInstance ele in eles)
                {
                    allInstances.Add(ele);
                }
                /*//linkten gelenler
                FilteredElementCollector linkCollector = new FilteredElementCollector(document).OfClass(typeof(RevitLinkInstance));
                foreach (RevitLinkInstance linkInstance in linkCollector)
                {
                    
                    Document linkDoc = linkInstance.GetLinkDocument();
                    if (linkDoc == null) continue; 

                    
                    FilteredElementCollector linkElemCollector = new FilteredElementCollector(linkDoc)
                        .OfClass(typeof(FamilyInstance));

                    foreach (FamilyInstance linkFamilyInstance in linkElemCollector)
                    {
                        
                        allInstances.Add(linkFamilyInstance);
                    }
                }
                //linkten gelenler*/
                List<FamilyInstance> fgs = allInstances.GroupBy(x => x.Symbol.get_Parameter(BuiltInParameter.WINDOW_TYPE_ID).AsValueString()).Select(g => g.First()).ToList();
                FilteredElementCollector col = new FilteredElementCollector(document);
                ViewFamilyType vt = col.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().First(x => x.ViewFamily == ViewFamily.FloorPlan);
                Random r = new Random();
                int b = 1;
                using (Transaction trans = new Transaction(document, "plan"))
                {
                    trans.Start();
                    foreach (FamilyInstance fi in fgs)
                    {
                        try
                        {
                            //FamilySmbol ve type mark parametresi çekme
                            Element ele = fi as Element;
                            FamilySymbol fs = fi.Symbol;
                            if (fs == null)
                            {
                                message = "Family symbol is null.";
                                return Result.Failed;
                            }
                            Parameter p2 = fs.get_Parameter(BuiltInParameter.WINDOW_TYPE_ID);

                            //int sayi = r.Next(1000);
                            string a = null;
                            if (temp.Typemark)
                            {
                                a = p2.AsValueString() + temp.ViewName;

                            }
                            else
                            {
                                a = temp.ViewName + b.ToString();
                            }
                            b++;
                            //FamilySmbol ve type mark parametresi çekme

                            //Boundingbox için instance'ın oldugu leveldan bir plan view çekme
                            /*ElementId lid = ele.LevelId;
                            if (lid == null)
                            {
                                message = "LevelId is null.";
                                return Result.Failed;
                            }
                            FilteredElementCollector viewcol = new FilteredElementCollector(document).OfClass(typeof(ViewPlan));
                            ViewPlan plan = viewcol.Cast<ViewPlan>().FirstOrDefault(vp => vp.GenLevel.Id == lid);*/
                            //Boundingbox için instance'ın oldugu leveldan bir plan view çekme
                            //asdfg

                            /*duvarn x ekseni ile açısını hesaplama
                            Element elewall = fi.Host;
                            Wall wall = elewall as Wall;
                            Double angle = wall.Orientation.AngleTo(XYZ.BasisX);
                            Transform transf =Transform.CreateRotation(XYZ.BasisZ, angle);*/



                            //
                            //Boundingbox Plan cropu belirleme
                            BoundingBoxXYZ bo = ele.get_BoundingBox(null);
                            BoundingBoxXYZ crop = bo;
                            //crop.Transform = transf;
                            /*Double dx = (bo.Max.X - bo.Min.X) / 4;
                            Double dy = (bo.Max.Y - bo.Min.Y) / 4;*/
                            Double dx = 0.6;
                            Double dy = 0.6;
                            XYZ yenimax = new XYZ(bo.Max.X + dx, bo.Max.Y + dy, bo.Max.Z);
                            XYZ yenimin = new XYZ(bo.Min.X - dx, bo.Min.Y - dy, bo.Min.Z);
                            crop.Max = yenimax; crop.Min = yenimin;
                            //Boundingbox Plan cropu belirleme





                            ViewPlan vp = ViewPlan.Create(document, vt.Id, ele.LevelId);
                            vp.CropBox = crop;
                            vp.CropBoxActive = true;
                            vp.Name = a;
                            vp.ViewTemplateId = temp.Template.Id;

                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("Warning", $"Error with element ID {fi.Id}: {ex.Message}");
                            continue;

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
