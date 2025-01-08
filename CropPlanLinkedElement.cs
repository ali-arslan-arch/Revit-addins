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
    internal class CropPlanLinkedElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
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
                FilteredElementCollector col = new FilteredElementCollector(doc);
                ViewFamilyType vt = col.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().First(x => x.ViewFamily == ViewFamily.FloorPlan);
                Random r = new Random();

                using (Transaction trans = new Transaction(doc, "plan"))
                {
                    trans.Start();

                    //FamilySmbol ve type mark parametresi çekme

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
                    else { a = temp.ViewName; }

                    //FamilySmbol ve type mark parametresi çekme








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





                    ViewPlan vp = ViewPlan.Create(doc, vt.Id, ele.LevelId);
                    vp.CropBox = crop;
                    vp.CropBoxActive = true;
                    vp.Name = a;
                    vp.ViewTemplateId = temp.Template.Id;




                    trans.Commit();
                    uidoc.ActiveView = vp;
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
