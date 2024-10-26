using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class Tag : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            FilteredElementCollector col = new FilteredElementCollector(doc,doc.ActiveView.Id);
            FilteredElementCollector col2 = new FilteredElementCollector(doc);
            FamilySymbol fs =col2.OfCategory(BuiltInCategory.OST_WindowTags).WhereElementIsElementType().Cast<FamilySymbol>().FirstOrDefault();
            IList<Element> eles = col.OfCategory(BuiltInCategory.OST_Windows).ToElements();
            Dictionary<Reference, XYZ> ages = new Dictionary<Reference, XYZ>();
            
            foreach (Element ele in eles)
            {
                LocationPoint lp = ele.Location as LocationPoint;
                
                ages.Add(new Reference(ele), lp.Point );
            }
            try
            {
                using(Transaction trans = new Transaction(doc, "tag"))
                {
                    trans.Start();
                    foreach(KeyValuePair<Reference, XYZ> item in ages)
                    {
                        IndependentTag.Create(doc,fs.Id, doc.ActiveView.Id, item.Key, true, TagOrientation.Horizontal, item.Value);

                    }
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
