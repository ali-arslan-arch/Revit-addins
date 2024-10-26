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
    internal class PlaceFamily : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document document = uidoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(document);
            IList<Element> familysymbols = collector.OfClass(typeof(FamilySymbol)).WhereElementIsElementType().ToElements();
            FamilySymbol symbol = null;
            foreach (Element ele in familysymbols) 
            {
                if (ele.Name == "1525 x 762mm") 
                {
                    symbol = (FamilySymbol)ele;
                    break;
                }
            }
            try
            {
                using (Transaction trans = new Transaction(document, "Place Family"))
                {
                    trans.Start();
                    if(!symbol.IsActive)
                    {
                        symbol.Activate();

                    }
                    XYZ p = new XYZ();
                    document.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    trans.Commit();                   

                }
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
