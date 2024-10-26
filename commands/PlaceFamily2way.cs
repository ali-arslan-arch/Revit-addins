using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class PlaceFamily2way : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            FamilySymbol fs =(FamilySymbol)collector.OfClass(typeof(FamilySymbol)).First(x => x.Name == "1525 x 762mm");

            try
            {
                using (Transaction trans = new Transaction(doc,"Place Family"))
                {
                    trans.Start();
                    if (!fs.IsActive)
                    {
                        fs.Activate();

                    }
                    doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), fs, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
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
