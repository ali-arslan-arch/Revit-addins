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
    [TransactionAttribute(TransactionMode.ReadOnly)]
    internal class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element ele = doc.GetElement(pickedobj);
                Parameter leftank = ele.LookupParameter(SelectLintelType.parametername200leftank);
                Parameter rightank = ele.LookupParameter(SelectLintelType.parametername200rightank);
                TaskDialog.Show("Asd", leftank.AsInteger().ToString());
                TaskDialog.Show("Asd", rightank.AsInteger().ToString());
                return Result.Succeeded;
            }
            catch 
            { 
                return Result.Failed;
            }

        }
    }
}
