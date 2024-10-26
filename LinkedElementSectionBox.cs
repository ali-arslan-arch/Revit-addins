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
    internal class LinkedElementSectionBox : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement);
                RevitLinkInstance revitLinkInstance = doc.GetElement(pickedobj) as RevitLinkInstance;
                Document linkdocument = revitLinkInstance.GetLinkDocument();
                ElementId eleid = pickedobj.LinkedElementId;
                Element ele = linkdocument.GetElement(eleid);

                SectionBoxForm form = new SectionBoxForm(doc);
                if (form.ShowDialog() == DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }


                //FilteredElementCollector viewcol = new FilteredElementCollector(doc);
                View3D trid = form.view;
                FilteredElementCollector col = new FilteredElementCollector(doc);
                ViewFamilyType vt = col.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().First(x => x.ViewFamily == ViewFamily.ThreeDimensional);
                using(Transaction trans = new Transaction(doc,"section box"))
                {
                    trans.Start();
                    if (trid == null)
                    {
                        trid = View3D.CreateIsometric(doc, vt.Id);

                    }
                    trid.SetSectionBox(ele.get_BoundingBox(null));
                    trans.Commit();

                }
                uidoc.ActiveView = trid;
                

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
