using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Creation;
using System.Windows.Forms;



namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class PlaceLintel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            
            try
            {
                PlaceLintelForm form = new PlaceLintelForm(doc);
                //form.ShowDialog();
                if(form.ShowDialog() == DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }
                FilteredElementCollector doorcol = new FilteredElementCollector(doc,uidoc.ActiveView.Id);
                List<Element> doors =  doorcol.OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType().ToElements().ToList();
                //FilteredElementCollector lentocol = new FilteredElementCollector(doc);
                //FamilySymbol lento = lentocol.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().ToElements().Cast<FamilySymbol>().ToList().First(y => y.Name == "lento");

                FamilySymbol lento200 = form.lintelfor200;
                FamilySymbol lento100 = form.lintelfor100;
                FamilySymbol lento250 = form.lintelfor250;
                FamilySymbol lento150 = form.lintelfor150;


                using (Transaction trans = new Transaction(doc, "Place Lintel"))
                {
                    trans.Start();
                    if (!lento200.IsActive)
                    {
                        lento200.Activate();
                    }
                    if (!lento100.IsActive)
                    {
                        lento100.Activate();
                    }
                    if (!lento250.IsActive)
                    {
                        lento250.Activate();
                    }
                    if (!lento150.IsActive)
                    {
                        lento150.Activate();
                    }

                    foreach (Element door in doors)
                    {
                        FamilyInstance fdoor = door as FamilyInstance;
                        BoundingBoxXYZ bo = door.get_BoundingBox(null);
                        Double doorheight = bo.Max.Z - bo.Min.Z;
                        ElementId levelid = door.LevelId;
                        Level l = doc.GetElement(levelid) as Level;
                        Double elev = l.Elevation;
                        Double genislik = lentogenisligi(doc, fdoor);
                        Element wallele = fdoor.Host;
                        Wall wall = wallele as Wall;

                        LocationPoint lp = door.Location as LocationPoint;
                        //XYZ point = new XYZ(lp.Point.X,lp.Point.Y,lp.Point.Z + doorheight - elev);
                        Double yukseklik = lentoyuksekligi(doc, door);
                        XYZ facecent = Facecenter(doc, door);
                        //XYZ point = new XYZ(lp.Point.X, lp.Point.Y, bo.Max.Z - elev);
                        //XYZ point = new XYZ(lp.Point.X, lp.Point.Y, yukseklik);
                        XYZ point = new XYZ(facecent.X, facecent.Y, yukseklik);
                        XYZ doorvector = fdoor.FacingOrientation;
                        Line line = Line.CreateBound(point, new XYZ(point.X, point.Y, point.Z + 1));
                        Double width200 = 200 / 304.8;
                        
                        Double width250 = 250 / 304.8;
                        
                        Double width150 = 150 / 304.8;
                        double tolerance = 0.001;

                        if (wall.Width == width200 )
                        {
                            FamilyInstance lintel = doc.Create.NewFamilyInstance(point, lento200, l, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                            XYZ lintelvector = lintel.FacingOrientation;
                            Double angle = lintelvector.AngleTo(doorvector);
                            
                            XYZ zAxis = XYZ.BasisZ;
                            XYZ crossProduct = lintelvector.CrossProduct(doorvector);
                            double dotProductWithZ = crossProduct.DotProduct(zAxis);
                            if (dotProductWithZ < 0)
                            {
                                // Eğer negatifse, ters yönde (counterclockwise) olduğundan,
                                // açıyı 360 dereceye tamamlamak için şu işlemi yapıyoruz
                                angle = 2 * Math.PI - angle; // Radyan cinsinden 360 dereceyi çıkarıyoruz
                            }

                            ElementTransformUtils.RotateElement(doc, lintel.Id, line, angle);

                            
                            
                            
                            Parameter Length = lintel.LookupParameter(form.parametername200length);
                            BoundingBoxXYZ bound200 = lintel.get_BoundingBox(null);
                            
                            
                            ElementTransformUtils.MoveElement(doc, lintel.Id, doorvector*(100 / 304.8));


                            Parameter p2001 = lintel.LookupParameter(form.parametername2001);
                            Parameter p2002 = lintel.LookupParameter(form.parametername2002);
                            Parameter p200leftanchor = lintel.LookupParameter(form.parametername200leftank);
                            Parameter p200rightanchor = lintel.LookupParameter(form.parametername200rightank);
                            int off = 0;
                            
                            p200leftanchor.Set(off);
                            p200rightanchor.Set(off);
                            if (genislik >= form.offsetdifference)
                            {
                                p2001.Set(form.greatoffset);
                                p2002.Set(form.greatoffset);
                                Length.Set(genislik + form.greatoffset + form.greatoffset);
                            }
                            else
                            {
                                p2001.Set(form.lessoffset);
                                p2002.Set(form.lessoffset);
                                Length.Set(genislik + form.lessoffset + form.lessoffset);

                            }



                        }
                        if(wall.Width == width250)
                        {
                            FamilyInstance lintel = doc.Create.NewFamilyInstance(point, lento250, l, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                            XYZ lintelvector = lintel.FacingOrientation;
                            Double angle = lintelvector.AngleTo(doorvector);
                            XYZ zAxis = XYZ.BasisZ;
                            XYZ crossProduct = lintelvector.CrossProduct(doorvector);
                            double dotProductWithZ = crossProduct.DotProduct(zAxis);
                            if (dotProductWithZ < 0)
                            {
                                // Eğer negatifse, ters yönde (counterclockwise) olduğundan,
                                // açıyı 360 dereceye tamamlamak için şu işlemi yapıyoruz
                                angle = 2 * Math.PI - angle; // Radyan cinsinden 360 dereceyi çıkarıyoruz
                            }
                            ElementTransformUtils.RotateElement(doc, lintel.Id, line, angle);
                            ElementTransformUtils.MoveElement(doc, lintel.Id, doorvector * (125 / 304.8));
                            Parameter Length = lintel.LookupParameter(form.parametername200length);

                            
                            Parameter p2501 = lintel.LookupParameter(form.parametername2001);
                            Parameter p2502 = lintel.LookupParameter(form.parametername2002);
                            Parameter p250leftanchor = lintel.LookupParameter(form.parametername200leftank);
                            Parameter p250rightanchor = lintel.LookupParameter(form.parametername200rightank);
                            int off = 0;

                            p250leftanchor.Set(off);
                            p250rightanchor.Set(off);
                            if (genislik >= form.offsetdifference)
                            {
                                p2501.Set(form.greatoffset);
                                p2502.Set(form.greatoffset);
                                Length.Set(genislik + form.greatoffset + form.greatoffset);
                            }
                            else
                            {
                                p2501.Set(form.lessoffset);
                                p2502.Set(form.lessoffset);
                                Length.Set(genislik + form.lessoffset + form.lessoffset);

                            }

                        }
                        if (Math.Abs(wall.Width - width150) < tolerance)
                        {
                            FamilyInstance lintel = doc.Create.NewFamilyInstance(point, lento150, l, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                            XYZ lintelvector = lintel.FacingOrientation;
                            Double angle = lintelvector.AngleTo(doorvector);
                            XYZ zAxis = XYZ.BasisZ;
                            XYZ crossProduct = lintelvector.CrossProduct(doorvector);
                            double dotProductWithZ = crossProduct.DotProduct(zAxis);
                            if (dotProductWithZ < 0)
                            {
                                // Eğer negatifse, ters yönde (counterclockwise) olduğundan,
                                // açıyı 360 dereceye tamamlamak için şu işlemi yapıyoruz
                                angle = 2 * Math.PI - angle; // Radyan cinsinden 360 dereceyi çıkarıyoruz
                            }
                            ElementTransformUtils.RotateElement(doc, lintel.Id, line, angle);
                            ElementTransformUtils.MoveElement(doc, lintel.Id, doorvector * (75 / 304.8));
                            Parameter Length = lintel.LookupParameter(form.parametername200length);


                            Parameter p1501 = lintel.LookupParameter(form.parametername2001);
                            Parameter p1502 = lintel.LookupParameter(form.parametername2002);
                            Parameter p150leftanchor = lintel.LookupParameter(form.parametername200leftank);
                            Parameter p150rightanchor = lintel.LookupParameter(form.parametername200rightank);
                            int off = 0;

                            p150leftanchor.Set(off);
                            p150rightanchor.Set(off);
                            if (genislik >= form.offsetdifference)
                            {
                                p1501.Set(form.greatoffset);
                                p1502.Set(form.greatoffset);
                                Length.Set(genislik + form.greatoffset + form.greatoffset);
                            }
                            else
                            {
                                p1501.Set(form.lessoffset);
                                p1502.Set(form.lessoffset);
                                Length.Set(genislik + form.lessoffset + form.lessoffset);

                            }

                        }
                        if (wall.Width < width150)
                        {
                            FamilyInstance lintel = doc.Create.NewFamilyInstance(point, lento100, l, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                            XYZ lintelvector = lintel.FacingOrientation;
                            Double angle = lintelvector.AngleTo(doorvector);
                            XYZ zAxis = XYZ.BasisZ;
                            XYZ crossProduct = lintelvector.CrossProduct(doorvector);
                            double dotProductWithZ = crossProduct.DotProduct(zAxis);
                            if (dotProductWithZ < 0)
                            {
                                // Eğer negatifse,, ters yönde (counterclockwise) olduğundan,
                                // açıyı 360 dereceye tamamlamak için şu işlemi yapıyoruz
                                angle = 2 * Math.PI - angle; // Radyan cinsinden 360 dereceyi çıkarıyoruz
                            }
                            ElementTransformUtils.RotateElement(doc, lintel.Id, line, angle);
                            Element hw = fdoor.Host;
                            Wall wa = hw as Wall;
                            Double halfwidth = (wa.Width) / 2; 
                            ElementTransformUtils.MoveElement(doc, lintel.Id, doorvector * (halfwidth));
                            Parameter Length = lintel.LookupParameter(form.parametername100length);
                            BoundingBoxXYZ bound = lintel.get_BoundingBox(null);
                            Double boundz = bound.Min.Z;
                            Double facez = facecent.Z;
                            ElementTransformUtils.MoveElement(doc, lintel.Id, new XYZ(0, 0, -(200/304.8)));
                            
                            Parameter p1001 = lintel.LookupParameter(form.parametername1001);
                            Parameter p1002 = lintel.LookupParameter(form.parametername1002);
                            Parameter p100leftanchor = lintel.LookupParameter(form.parametername100leftank);
                            Parameter p100rightanchor = lintel.LookupParameter(form.parametername100rightank);
                            int off = 0;

                            p100leftanchor.Set(off);
                            p100rightanchor.Set(off);
                            if (genislik >= form.offsetdifference)
                            {
                                p1001.Set(form.greatoffset);
                                p1002.Set(form.greatoffset);
                                Length.Set(genislik + form.greatoffset + form.greatoffset);
                            }
                            else
                            {
                                p1001.Set(form.lessoffset);
                                p1002.Set(form.lessoffset);
                                Length.Set(genislik + form.lessoffset + form.lessoffset);

                            }


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

        public static Double lentogenisligi(Autodesk.Revit.DB.Document doc,Element ele)
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
            //yatayfaces.Remove(largestFace);
            //yatayfaces.Remove(secondLargestFace);
            BoundingBoxXYZ doorBoundingBox = ele.get_BoundingBox(null);
            double doortopZ = doorBoundingBox.Max.Z;
            XYZ doorcenter = (doorBoundingBox.Max + doorBoundingBox.Min) / 2;
            Parameter heightp = ele.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM);
            Double halfheight = heightp.AsDouble() / 2;
            Level walllevel = doc.GetElement(ele.LevelId) as Level;
            Double h = walllevel.Elevation + heightp.AsDouble();
            XYZ doortop = new XYZ(doorcenter.X, doorcenter.Y, h/*doorcenter.Z + halfheight*/);
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
            Double genislik = maxLength;


            return genislik;

        }
        public static Double lentoyuksekligi(Autodesk.Revit.DB.Document doc, Element ele)
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
            //Face largestFace = sortedFaces[0];
            //Face secondLargestFace = sortedFaces[1];
            //yatayfaces.Remove(largestFace);
            //yatayfaces.Remove(secondLargestFace);
            BoundingBoxXYZ doorBoundingBox = ele.get_BoundingBox(null);
            double doortopZ = doorBoundingBox.Max.Z;
            XYZ doorcenter = (doorBoundingBox.Max + doorBoundingBox.Min) / 2;
            Parameter heightp = ele.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM);
            Double halfheight = heightp.AsDouble()/2;
            Level walllevel = doc.GetElement(ele.LevelId) as Level;
            Double h = walllevel.Elevation + heightp.AsDouble();
            XYZ doortop = new XYZ(doorcenter.X,doorcenter.Y,h/*doorcenter.Z+halfheight*/);
            Double max = Double.MaxValue;
            Face topFace = null;


            foreach (Face face in yatayfaces)
            {
                BoundingBoxUV faceboundingbox = face.GetBoundingBox();
                XYZ faceorigin = face.Evaluate((faceboundingbox.Max + faceboundingbox.Min) / 2);
                Double facez = faceorigin.Z;
                //Double distance = doorcenter.DistanceTo(faceorigin);
                Double distance = doortop.DistanceTo(faceorigin);
                if (distance < max)
                {
                    max = distance;
                    topFace = face;
                }
            }
            XYZ facepoint = topFace.Evaluate(new UV());
            
            Double yukseklik = facepoint.Z - walllevel.Elevation;
            

            //Parameter baseoffset = w.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);


            return yukseklik;
            
            
            
            













            


            

        }
        public static XYZ Facecenter(Autodesk.Revit.DB.Document doc, Element ele)
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
            //Face largestFace = sortedFaces[0];
            //Face secondLargestFace = sortedFaces[1];
            //yatayfaces.Remove(largestFace);
            //yatayfaces.Remove(secondLargestFace);
            BoundingBoxXYZ doorBoundingBox = ele.get_BoundingBox(null);
            double doortopZ = doorBoundingBox.Max.Z;
            XYZ dc = (doorBoundingBox.Max + doorBoundingBox.Min) / 2;
            Double z = (doorBoundingBox.Min.Z + doorBoundingBox.Max.Z) * 3 / 4;
            Parameter heightp = ele.get_Parameter(BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM);
            Double halfheight = heightp.AsDouble() / 2;
            Level walllevel = doc.GetElement(ele.LevelId) as Level;
            Double h = walllevel.Elevation + heightp.AsDouble();
            XYZ doorcenter =new XYZ(dc.X, dc.Y, h);
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
            UV uvPoint = new UV(0.5, 0.5); // Yüzeyin ortasını temsil eden U ve V parametreleri.
            BoundingBoxUV facebound = topFace.GetBoundingBox();
            XYZ faceorig = topFace.Evaluate((facebound.Max + facebound.Min) / 2);


            













            


            return faceorig;

        }
    }
}
