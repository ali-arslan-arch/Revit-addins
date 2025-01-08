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
    internal class PlaceLintelSingle : IExternalCommand
    {
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                //PlaceLintelForm form = new PlaceLintelForm(doc);
                //form.ShowDialog();
                /*if (form.ShowDialog() == DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }*/
                 
                Element door = doc.GetElement(pickedobj);
                /*FamilySymbol lento200 = form.lintelfor200;
                FamilySymbol lento100 = form.lintelfor100;
                FamilySymbol lento250 = form.lintelfor250;*/
                FamilySymbol lento200 = SelectLintelType.lintelfor200;
                FamilySymbol lento100 = SelectLintelType.lintelfor100;
                FamilySymbol lento250 = SelectLintelType.lintelfor250;
                FamilySymbol lento150 = SelectLintelType.lintelfor150;

                using (Transaction trans = new Transaction(doc, "lintel"))
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
                    FamilyInstance fdoor = door as FamilyInstance;
                    BoundingBoxXYZ bo = door.get_BoundingBox(null);
                    Double doorheight = bo.Max.Z - bo.Min.Z;
                    ElementId levelid = door.LevelId;
                    Level l = doc.GetElement(levelid) as Level;
                    Double elev = l.Elevation;
                    Double genislik = PlaceLintel.lentogenisligi(doc, fdoor);
                    Element wallele = fdoor.Host;
                    Wall wall = wallele as Wall;

                    LocationPoint lp = door.Location as LocationPoint;
                    //XYZ point = new XYZ(lp.Point.X,lp.Point.Y,lp.Point.Z + doorheight - elev);
                    Double yukseklik = PlaceLintel.lentoyuksekligi(doc, door);
                    XYZ facecent = PlaceLintel.Facecenter(doc, fdoor);
                    //XYZ point = new XYZ(lp.Point.X, lp.Point.Y, bo.Max.Z - elev);
                    //XYZ point = new XYZ(lp.Point.X, lp.Point.Y, yukseklik);
                    XYZ point = new XYZ(facecent.X, facecent.Y, yukseklik);
                    XYZ doorvector = fdoor.FacingOrientation;
                    Line line = Line.CreateBound(point, new XYZ(point.X, point.Y, point.Z + 1));
                    Double width200 = 200 / 304.8;

                    Double width250 = 250 / 304.8;
                    Double width150 = 150 / 304.8;
                    double tolerance = 0.001;

                    if (wall.Width == width200)
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
                            // açıyı 360 dereceye tamamlamak için şu işlemi yapıyoruz.
                            angle = 2 * Math.PI - angle; // Radyan cinsinden 360 dereceyi çıkarıyoruz
                        }

                        ElementTransformUtils.RotateElement(doc, lintel.Id, line, angle);




                        Parameter Length = lintel.LookupParameter(SelectLintelType.parametername200length);
                        BoundingBoxXYZ bound200 = lintel.get_BoundingBox(null);


                        ElementTransformUtils.MoveElement(doc, lintel.Id, doorvector * (100 / 304.8));


                        Parameter p2001 = lintel.LookupParameter(SelectLintelType.parametername2001);
                        Parameter p2002 = lintel.LookupParameter(SelectLintelType.parametername2002);
                        Parameter p200leftanchor = lintel.LookupParameter(SelectLintelType.parametername200leftank);
                        Parameter p200rightanchor = lintel.LookupParameter(SelectLintelType.parametername200rightank);
                        int off = 0;

                        p200leftanchor.Set(off);
                        p200rightanchor.Set(off);
                        if (genislik >= SelectLintelType.offsetdifference)
                        {
                            p2001.Set(SelectLintelType.greatoffset);
                            p2002.Set(SelectLintelType.greatoffset);
                            Length.Set(genislik + SelectLintelType.greatoffset + SelectLintelType.greatoffset);
                        }
                        else
                        {
                            p2001.Set(SelectLintelType.lessoffset);
                            p2002.Set(SelectLintelType.lessoffset);
                            Length.Set(genislik + SelectLintelType.lessoffset + SelectLintelType.lessoffset);

                        }



                    }
                    if (wall.Width == width250)
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
                        Parameter Length = lintel.LookupParameter(SelectLintelType.parametername200length);


                        Parameter p2501 = lintel.LookupParameter(SelectLintelType.parametername2001);
                        Parameter p2502 = lintel.LookupParameter(SelectLintelType.parametername2002);
                        Parameter p250leftanchor = lintel.LookupParameter(SelectLintelType.parametername200leftank);
                        Parameter p250rightanchor = lintel.LookupParameter(SelectLintelType.parametername200rightank);
                        int off = 0;

                        p250leftanchor.Set(off);
                        p250rightanchor.Set(off);
                        if (genislik >= SelectLintelType.offsetdifference)
                        {
                            p2501.Set(SelectLintelType.greatoffset);
                            p2502.Set(SelectLintelType.greatoffset);
                            Length.Set(genislik + SelectLintelType.greatoffset + SelectLintelType.greatoffset);
                        }
                        else
                        {
                            p2501.Set(SelectLintelType.lessoffset);
                            p2502.Set(SelectLintelType.lessoffset);
                            Length.Set(genislik + SelectLintelType.lessoffset + SelectLintelType.lessoffset);

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
                        Parameter Length = lintel.LookupParameter(SelectLintelType.parametername200length);


                        Parameter p1501 = lintel.LookupParameter(SelectLintelType.parametername2001);
                        Parameter p1502 = lintel.LookupParameter(SelectLintelType.parametername2002);
                        Parameter p150leftanchor = lintel.LookupParameter(SelectLintelType.parametername200leftank);
                        Parameter p150rightanchor = lintel.LookupParameter(SelectLintelType.parametername200rightank);
                        int off = 0;

                        p150leftanchor.Set(off);
                        p150rightanchor.Set(off);
                        if (genislik >= SelectLintelType.offsetdifference)
                        {
                            p1501.Set(SelectLintelType.greatoffset);
                            p1502.Set(SelectLintelType.greatoffset);
                            Length.Set(genislik + SelectLintelType.greatoffset + SelectLintelType.greatoffset);
                        }
                        else
                        {
                            p1501.Set(SelectLintelType.lessoffset);
                            p1502.Set(SelectLintelType.lessoffset);
                            Length.Set(genislik + SelectLintelType.lessoffset + SelectLintelType.lessoffset);

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
                            // Eğer negatifse, ters yönde (counterclockwise) olduğundan,
                            // açıyı 360 dereceye tamamlamak için şu işlemi yapıyoruz
                            angle = 2 * Math.PI - angle; // Radyan cinsinden 360 dereceyi çıkarıyoruz
                        }
                        ElementTransformUtils.RotateElement(doc, lintel.Id, line, angle);
                        Element hw = fdoor.Host;
                        Wall wa = hw as Wall;
                        Double halfwidth = (wa.Width) / 2;
                        ElementTransformUtils.MoveElement(doc, lintel.Id, doorvector * (halfwidth));
                        Parameter Length = lintel.LookupParameter(SelectLintelType.parametername100length);
                        BoundingBoxXYZ bound = lintel.get_BoundingBox(null);
                        Double boundz = bound.Min.Z;
                        Double facez = facecent.Z;
                        ElementTransformUtils.MoveElement(doc, lintel.Id, new XYZ(0, 0, -(200 / 304.8)));

                        Parameter p1001 = lintel.LookupParameter(SelectLintelType.parametername1001);
                        Parameter p1002 = lintel.LookupParameter(SelectLintelType.parametername1002);
                        Parameter p100leftanchor = lintel.LookupParameter(SelectLintelType.parametername100leftank);
                        Parameter p100rightanchor = lintel.LookupParameter(SelectLintelType.parametername100rightank);
                        int off = 0;

                        p100leftanchor.Set(off);
                        p100rightanchor.Set(off);
                        if (genislik >= SelectLintelType.offsetdifference)
                        {
                            p1001.Set(SelectLintelType.greatoffset);
                            p1002.Set(SelectLintelType.greatoffset);
                            Length.Set(genislik + SelectLintelType.greatoffset + SelectLintelType.greatoffset);
                        }
                        else
                        {
                            p1001.Set(SelectLintelType.lessoffset);
                            p1002.Set(SelectLintelType.lessoffset);
                            Length.Set(genislik + SelectLintelType.lessoffset + SelectLintelType.lessoffset);

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
    }
}
