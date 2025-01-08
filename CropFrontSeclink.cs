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
    internal class CropFrontSeclink : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {

                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement);
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
                RevitLinkInstance link = doc.GetElement(pickedobj) as RevitLinkInstance;
                Autodesk.Revit.DB.Document linkdoc = link.GetLinkDocument();
                ElementId eleid = pickedobj.LinkedElementId;
                Element ele = linkdoc.GetElement(eleid);
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
                if (sectionViewType == null)
                {
                    message = "Section ViewFamilyType bulunamadı.";
                    return Result.Failed;
                }
                Random r = new Random();
                using (Transaction trans = new Transaction(doc, "Create Window Section"))
                {
                    trans.Start();




                    FamilySymbol fs = fi.Symbol;
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
                        a = temp.ViewName;
                    }









                    XYZ windowLocation = (fi.Location as LocationPoint).Point;
                    XYZ facingOrientation = fi.FacingOrientation.Normalize();
                    XYZ upDirection = XYZ.BasisZ;


                    Transform sectionTransform = Transform.Identity;
                    sectionTransform.Origin = windowLocation;
                    sectionTransform.BasisZ = facingOrientation.Negate();
                    sectionTransform.BasisX = upDirection.CrossProduct(sectionTransform.BasisZ).Normalize();
                    sectionTransform.BasisY = sectionTransform.BasisZ.CrossProduct(sectionTransform.BasisX).Normalize();

                    // BoundingBoxXYZ 
                    BoundingBoxXYZ sectionBox = new BoundingBoxXYZ();
                    sectionBox.Transform = sectionTransform;


                    double width = CroppedSectionBasedOnFamilyDimensions.DistanceVerticalFaces(doc, ele);//f.Symbol.get_Parameter(BuiltInParameter.WINDOW_WIDTH).AsDouble();
                    double height = bo.Max.Z - bo.Min.Z; //f.Symbol.get_Parameter(BuiltInParameter.WINDOW_HEIGHT).AsDouble();



                    sectionBox.Min = new XYZ(-0.6- (width / 2), -0.3, -1);  // Derinliği arttırmak için Z ekseninde -1 ayarlıyoruz
                    sectionBox.Max = new XYZ(0.6+ (width / 2), 0.6+ height, 1);     // Z ekseninde derinlik için +1 ayarlıyoruz



                    ViewSection sectionView = ViewSection.CreateSection(doc, sectionViewType.Id, sectionBox);
                    sectionView.Name = a;
                    sectionView.ViewTemplateId = temp.Template.Id;




                    trans.Commit();
                    uidoc.ActiveView = sectionView;

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
