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
    internal class ViewFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc  = uidoc.Document;
            List<ElementId> ids = new List<ElementId>();
            ids.Add(new ElementId(BuiltInCategory.OST_Furniture));
            FilterRule fl = ParameterFilterRuleFactory.CreateContainsRule(new ElementId(BuiltInParameter.SYMBOL_NAME_PARAM), "asd", false);
            ElementParameterFilter filter = new ElementParameterFilter(fl);

            
            try
            {
                using(Transaction trans = new Transaction(doc, "filter"))
                {
                    trans.Start();
                    ParameterFilterElement x =ParameterFilterElement.Create(doc, "filter", ids, filter);
                    doc.ActiveView.AddFilter(x.Id);
                    doc.ActiveView.SetFilterVisibility(x.Id, false);
                    
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
