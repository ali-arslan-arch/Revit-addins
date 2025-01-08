using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class CropSec : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            try
            {
                FormSelectTemplate temp = new FormSelectTemplate(doc);
                temp.CutPointVisible();
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
                FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
                IList<FamilyInstance> eles = collector.OfClass(typeof(FamilyInstance)).ToElements().Cast<FamilyInstance>().ToList();
                List<FamilyInstance> fgs = eles.GroupBy(x => x.Symbol.get_Parameter(BuiltInParameter.WINDOW_TYPE_ID).AsValueString()).Select(g => g.First()).ToList();
                ViewFamilyType sectionViewType = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .FirstOrDefault(x => x.ViewFamily == ViewFamily.Section);
                Random r = new Random();
                int b = 1;

                Double CutPoint = Convert.ToDouble(temp.Cutpoint);
                using (Transaction trans = new Transaction(doc, "Create Window Section"))
                {
                    trans.Start();

                    foreach (FamilyInstance f in fgs)
                    {
                        try
                        {
                            Element ele = f as Element;
                            FamilySymbol fs = f.Symbol;
                            BoundingBoxXYZ bo = ele.get_BoundingBox(null);
                            Parameter p2 = fs.get_Parameter(BuiltInParameter.WINDOW_TYPE_ID);
                            int sayi = r.Next(1000);
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

                            Element w = f.Host;
                            Wall wall = w as Wall;
                            Double wt = wall.Width;
                            LocationCurve lc = w.Location as LocationCurve;
                            Line l = lc.Curve as Line;
                            XYZ p = l.GetEndPoint(0);
                            XYZ q = l.GetEndPoint(1);
                            XYZ v = q - p;






                            XYZ windowLocation = (f.Location as LocationPoint).Point;
                            XYZ facingOrientation = f.FacingOrientation.Normalize();
                            XYZ upDirection = XYZ.BasisZ;


                            Transform sectionTransform = Transform.Identity;
                            sectionTransform.Origin = windowLocation;
                            sectionTransform.BasisZ = v.Normalize();
                            sectionTransform.BasisX = upDirection.CrossProduct(sectionTransform.BasisZ).Normalize();
                            sectionTransform.BasisY = sectionTransform.BasisZ.CrossProduct(sectionTransform.BasisX).Normalize();

                            // BoundingBoxXYZ 
                            BoundingBoxXYZ sectionBox = new BoundingBoxXYZ();
                            sectionBox.Transform = sectionTransform;


                            double width = CroppedSectionBasedOnFamilyDimensions.DistanceVerticalFaces(doc, ele);//f.Symbol.get_Parameter(BuiltInParameter.WINDOW_WIDTH).AsDouble();
                            double height = bo.Max.Z - bo.Min.Z; //f.Symbol.get_Parameter(BuiltInParameter.WINDOW_HEIGHT).AsDouble();



                            sectionBox.Min = new XYZ(/*-1.3 * (width / 2)*/-wt-0.2, -0.4, /*0.4*/CutPoint);
                            sectionBox.Max = new XYZ(/*1.3 * (width / 2)*/wt+0.3, /*1.1 * height*/height + 0.4, 1.3 * (width / 2));



                            ViewSection sectionView = ViewSection.CreateSection(doc, sectionViewType.Id, sectionBox);
                            sectionView.Name = a;
                            sectionView.ViewTemplateId = temp.Template.Id;

                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("Warning", $"Error with element ID {f.Id}: {ex.Message}");
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
