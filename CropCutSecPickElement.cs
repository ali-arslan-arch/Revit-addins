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
    internal class CropCutSecPickElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                FormSelectTemplate temp = new FormSelectTemplate(doc);
                temp.CutPointVisible();
                //xtemp.ShowDialog();
                if (temp.ShowDialog() == DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }
                if (temp.Template == null)
                {
                    message = "No template selected.";
                    return Result.Failed;
                }
                
                Double CutPoint = Convert.ToDouble(temp.Cutpoint);
                Element ele = doc.GetElement(pickedobj);
                FamilyInstance fi = ele as FamilyInstance;
                if (fi == null)
                {
                    TaskDialog.Show("hata", "Bir Family instance seçin");
                    return Result.Failed;

                }
                ViewFamilyType sectionViewType = new FilteredElementCollector(doc)
                   .OfClass(typeof(ViewFamilyType))
                   .Cast<ViewFamilyType>()
                   .FirstOrDefault<ViewFamilyType>(x => ViewFamily.Section == x.ViewFamily);
                Random r = new Random();
                using (Transaction trans = new Transaction(doc, "Create Window Section"))
                {
                    trans.Start();

                    
                        
                        FamilySymbol fs = fi.Symbol;
                        BoundingBoxXYZ bo = ele.get_BoundingBox(null);
                        Parameter p2 = fs.get_Parameter(BuiltInParameter.WINDOW_TYPE_ID);
                    string a = null;
                    if (temp.Typemark)
                    {
                        a = p2.AsValueString() + temp.ViewName;

                    }
                    else
                    {
                        a = temp.ViewName;
                    }
                    ;
                    
                        Element w = fi.Host;
                        Wall wall = w as Wall;
                        Double wt = wall.Width;
                        LocationCurve lc = w.Location as LocationCurve;
                        Line l = lc.Curve as Line;
                        XYZ p = l.GetEndPoint(0);
                        XYZ q = l.GetEndPoint(1);
                        XYZ v = q - p;






                        XYZ windowLocation = (fi.Location as LocationPoint).Point;
                        XYZ facingOrientation = fi.FacingOrientation.Normalize();
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



                        sectionBox.Min = new XYZ(/*-1.3 * (width / 2)*/-wt - 0.2, -0.3, /*0.4*/CutPoint);
                        sectionBox.Max = new XYZ(/*1.3 * (width / 2)*/wt + 0.3, /*1.1 * height*/height+0.4, 1.3 * (width / 2)/*-CutPoint*/);



                        ViewSection sectionView = ViewSection.CreateSection(doc, sectionViewType.Id, sectionBox);
                        sectionView.Name = a;
                        sectionView.ViewTemplateId = temp.Template.Id;
                    


                    
                    trans.Commit();
                    uidoc.ActiveView = sectionView;

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
