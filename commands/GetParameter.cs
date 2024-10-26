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
    internal class GetParameter : IExternalCommand
    {
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                if (pickedobj != null)
                {
                    ElementId eleid = pickedobj.ElementId;
                    Element ele = doc.GetElement(eleid);
                    Parameter parameter = ele.LookupParameter("Head Height");
                    InternalDefinition paramdef = (InternalDefinition)parameter.Definition;
                    TaskDialog.Show("Get Parameter", parameter.AsValueString() +
                        string.Format("Parameter {0}  with BuiltIn parameter {1} ", paramdef.Name, paramdef.BuiltInParameter));
                   
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
