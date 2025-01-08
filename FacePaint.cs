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
    internal class FacePaint : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element ele = doc.GetElement(pickedobj);
                using(Transaction trans = new Transaction(doc, "asd"))
                {
                    trans.Start();
                    CroppedSectionBasedOnFamilyDimensions.DistanceVerticalFaces(doc, ele);
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
