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
    internal class DimensionCollector : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                FilteredElementCollector dimcol = new FilteredElementCollector(doc,doc.ActiveView.Id);
                List<Dimension> dimensions = dimcol.OfCategory(BuiltInCategory.OST_Dimensions).WhereElementIsNotElementType().Cast<Dimension>().ToList();
                int a = dimensions[0].References.Size;
                ReferenceArray ra = dimensions[0].References;
                TaskDialog.Show("asd", a.ToString());
                foreach(Reference refe in ra)
                {
                    TaskDialog.Show("asd",refe.ElementReferenceType.ToString());
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
