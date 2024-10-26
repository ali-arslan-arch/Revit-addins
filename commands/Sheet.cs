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
    internal class Sheet : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FilteredElementCollector col = new FilteredElementCollector(doc);
            FamilySymbol fs = col.OfCategory(BuiltInCategory.OST_TitleBlocks).WhereElementIsElementType().Cast<FamilySymbol>().FirstOrDefault();
            try
            {
                using(Transaction trans = new Transaction(doc, "sheet"))
                {
                    trans.Start();
                    if(!fs.IsActive)
                    {
                        fs.Activate();

                    }
                    ViewSheet s = ViewSheet.Create(doc, fs.Id);
                    s.SheetNumber = "13";
                    s.Name = "plan";
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
