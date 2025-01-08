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
    internal class FacePaint2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element ele = doc.GetElement(pickedobj);
                using (Transaction trans = new Transaction(doc, "asd"))
                {
                    trans.Start();
                    face(doc, ele);
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
        public void face(Autodesk.Revit.DB.Document doc, Element ele)
        {
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
            FilteredElementCollector col = new FilteredElementCollector(doc);
            Element mat = col.OfCategory(BuiltInCategory.OST_Materials).First(x => x.Name == "Oak Flooring");
            ElementId matid = mat.Id;
            doc.Paint(w.Id, largestFace, matid);
            doc.Paint(w.Id, secondLargestFace, matid);



            //yatayfaces.Remove(largestFace);
            //yatayfaces.Remove(secondLargestFace);
            /*BoundingBoxXYZ doorBoundingBox = ele.get_BoundingBox(null);
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
                if (distance < max)
                {
                    max = distance;
                    topFace = face;
                }
            }
            XYZ facepoint = topFace.Evaluate(new UV());
            Level walllevel = doc.GetElement(ele.LevelId) as Level;
            Double yukseklik = facepoint.Z - walllevel.Elevation;*/

        }
    }
}
