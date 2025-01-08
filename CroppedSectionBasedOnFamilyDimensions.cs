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
    internal class CroppedSectionBasedOnFamilyDimensions : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            try
            {
                FormSelectTemplate temp = new FormSelectTemplate(doc);
                
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
                            if (bo == null)
                            {
                                throw new Exception("BoundingBoxXYZ is null for the element.");
                            }
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








                            XYZ windowLocation = (f.Location as LocationPoint).Point;
                            XYZ facingOrientation = f.FacingOrientation.Normalize();
                            XYZ upDirection = XYZ.BasisZ;


                            Transform sectionTransform = Transform.Identity;
                            sectionTransform.Origin = windowLocation;
                            sectionTransform.BasisZ = facingOrientation.Negate();
                            sectionTransform.BasisX = upDirection.CrossProduct(sectionTransform.BasisZ).Normalize();
                            sectionTransform.BasisY = sectionTransform.BasisZ.CrossProduct(sectionTransform.BasisX).Normalize();

                            // BoundingBoxXYZ 
                            BoundingBoxXYZ sectionBox = new BoundingBoxXYZ();
                            sectionBox.Transform = sectionTransform;
                            
                            Parameter widthParam = f.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM);
                                   

                            if (widthParam == null)
                            {

                                widthParam = f.Symbol.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM);
                                             
                            }
                            Parameter heightParam = f.get_Parameter(BuiltInParameter.GENERIC_HEIGHT);


                            if (heightParam == null)
                            {

                                heightParam = f.Symbol.get_Parameter(BuiltInParameter.GENERIC_HEIGHT);


                            }


                            double width = /*widthParam.AsDouble();*/DistanceVerticalFaces(doc, ele);//f.Symbol.get_Parameter(BuiltInParameter.WINDOW_WIDTH).AsDouble();
                            double height = /*heightParam.AsDouble();*/bo.Max.Z - bo.Min.Z; //f.Symbol.get_Parameter(BuiltInParameter.WINDOW_HEIGHT).AsDouble();



                            sectionBox.Min = new XYZ(-0.6- (width / 2), -0.3, -1);
                            sectionBox.Max = new XYZ(0.6+ (width / 2), 0.6+ height, 1);

                            if (sectionBox == null || sectionViewType == null)
                            {
                                throw new Exception("Section box or view type is invalid.");
                            }

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
            catch(Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
        public static Double DistanceVerticalFaces(Autodesk.Revit.DB.Document doc, Element ele)
        {
            FamilyInstance fam = ele as FamilyInstance;
            
            Element w = fam.Host;
            if (fam.Host == null)
            {
                throw new Exception("FamilyInstance does not have a host.");
            }
            ElementId wallid = w.Id;
            //FilteredElementCollector col = new FilteredElementCollector(doc);
            //Element mat = col.OfCategory(BuiltInCategory.OST_Materials).First(x => x.Name == "Oak Flooring");
            //ElementId matid = mat.Id;
            LocationCurve lc = w.Location as LocationCurve;
            if (lc == null || !(lc.Curve is Line))
            {
                throw new Exception("Host element does not have a valid LocationCurve or is not a Line.");
            }
            Line l =lc.Curve as Line;
            
            XYZ p = l.GetEndPoint(0);
            XYZ q = l.GetEndPoint(1);
            XYZ v = q - p;
            XYZ vec =v.Normalize();
            Options op = new Options();
            op.DetailLevel = ViewDetailLevel.Fine;
            GeometryElement gele = w.get_Geometry(op);
            if (gele == null)
            {
                throw new Exception("GeometryElement is null for the host element.");
            }

            
            List<Solid> wallgeo = new List<Solid>();
            List<Face> dikeyfaces = new List<Face>();
            Double kt = 180 / Math.PI;
            foreach (GeometryObject geom in gele)
            {
                Solid sol = geom as Solid;
                wallgeo.Add(sol);

            }
            foreach (Solid sol in wallgeo)
            {
                FaceArray faces = sol.Faces;
                foreach (Face facew in faces)
                {
                    XYZ facenormal = facew.ComputeNormal(new UV());

                    if (facenormal.AngleTo(vec)*kt < 5 || facenormal.AngleTo(vec.Negate())*kt < 5)
                    {
                        dikeyfaces.Add(facew);

                    }
                }

            }
            List<Face> sortedFaces = dikeyfaces.OrderByDescending(f => f.Area).ToList();
            //Face largestFace = sortedFaces[0];
            //Face secondLargestFace = sortedFaces[1];
            //dikeyfaces.Remove(largestFace);
            //dikeyfaces.Remove(secondLargestFace);
            BoundingBoxXYZ doorBoundingBox = ele.get_BoundingBox(null);
            double doortopZ = doorBoundingBox.Max.Z;
            XYZ doorcenter = (doorBoundingBox.Max + doorBoundingBox.Min) / 2;
            Double max = Double.MaxValue;
            Double secondMax = Double.MaxValue;
            Face topFace = null;
            Face secondtopFace = null;


            foreach (Face face in dikeyfaces)
            {
                BoundingBoxUV faceboundingbox = face.GetBoundingBox();
                XYZ faceorigin = face.Evaluate((faceboundingbox.Max + faceboundingbox.Min) / 2);
                Double facez = faceorigin.Z;
                Double distance = doorcenter.DistanceTo(faceorigin);
                if (distance < max)
                {
                    secondMax = max;
                    secondtopFace = topFace;
                    max = distance;
                    topFace = face;
                }
                else if (distance < secondMax && face != topFace)
                {
                    secondMax = distance;
                    secondtopFace = face;
                }
            }
            //doc.Paint(wallid, topFace, matid);
            //doc.Paint(wallid, secondtopFace, matid);
            UV uvPoint = new UV(0.5, 0.5); // Yüzeyin ortasını temsil eden U ve V parametreleri
            BoundingBoxUV facebound1 = topFace.GetBoundingBox();
            BoundingBoxUV facebound2 = secondtopFace.GetBoundingBox();
            XYZ faceorig1 = topFace.Evaluate((facebound1.Max + facebound1.Min) / 2);
            XYZ faceorig2 = secondtopFace.Evaluate((facebound2.Max + facebound2.Min) / 2);
            Double distance11 = faceorig1.DistanceTo(faceorig2);



















            return distance11;

        }
    }

    
}
