using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class geoele : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element ele = doc.GetElement(pickedobj);
                FamilyInstance fam = ele as FamilyInstance;
                Element w = fam.Host; 
                Options op = new Options();
                op.DetailLevel = ViewDetailLevel.Fine;
                GeometryElement gele = w.get_Geometry(op);
                List<Solid> wallgeo = new List<Solid>();
                List<Face> yatayfaces = new List<Face>();
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

                        if (facenormal.AngleTo(new XYZ(0, 0, 1)) == 0 || facenormal.AngleTo(new XYZ(0, 0, -1)) == 0)
                        {
                            yatayfaces.Add(facew);

                        }
                    }

                }
                List<Face> sortedFaces = yatayfaces.OrderByDescending(f => f.Area).ToList();
                Face largestFace = sortedFaces[0];
                Face secondLargestFace = sortedFaces[1];
                yatayfaces.Remove(largestFace);
                yatayfaces.Remove(secondLargestFace);
                BoundingBoxXYZ doorBoundingBox = ele.get_BoundingBox(null);
                double doortopZ = doorBoundingBox.Max.Z;
                XYZ doorcenter = (doorBoundingBox.Max + doorBoundingBox.Min) / 2;
                Double max = Double.MaxValue;
                Face topFace = null;
               
                
                foreach (Face face in yatayfaces)
                {
                    BoundingBoxUV faceboundingbox = face.GetBoundingBox();
                    XYZ faceorigin = face.Evaluate((faceboundingbox.Max + faceboundingbox.Min) / 2);
                    Double facez = faceorigin.Z;
                    Double distance = doorcenter.DistanceTo(faceorigin);
                    if(distance < max)
                    {
                        max = distance;
                        topFace = face;
                    }
                }
                using (Transaction trans = new Transaction(doc,"paint"))
                {
                    trans.Start();
                    //foreach (Face facew in yatayfaces)
                    
                        Material paintMaterial = new FilteredElementCollector(doc)
                            .OfClass(typeof(Material))
                            .FirstOrDefault(m => m.Name == "Sash") as Material;

                        doc.Paint(w.Id, topFace, paintMaterial.Id);

                    
                    
                        
                    trans.Commit();

                }
                IList<CurveLoop> cl = topFace.GetEdgesAsCurveLoops();
                CurveLoop loop = cl[0];
                Curve longestCurve = null;
                double maxLength = 0;
                foreach (Curve c in loop)
                {
                    double curveLength = c.Length;

                    
                    if (curveLength > maxLength)
                    {
                        maxLength = curveLength;
                        longestCurve = c;
                    }

                }
                Double genislik = maxLength * 304.8;
                
                
                TaskDialog.Show("asd", genislik.ToString());
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
