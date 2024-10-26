using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;


namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    public class GetElementId : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document document = uidoc.Document;
            try
            {
                Reference pickedObj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                ElementId eleId = pickedObj.ElementId;
                Element ele = document.GetElement(eleId);
                ElementId eTypeId = ele.GetTypeId();
                ElementType eType = (ElementType)document.GetElement(eTypeId);
                if (pickedObj != null)
                {
                    TaskDialog.Show("Element Classification", eleId.ToString() + Environment.NewLine + "Category " + ele.Category.Name
                       + Environment.NewLine + "Instance " + ele.Name + Environment.NewLine + "Type " + eType.Name
                       + Environment.NewLine + "Family " + eType.FamilyName + Environment.NewLine + "class " + ele.GetType());
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
