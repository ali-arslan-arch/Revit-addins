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
    [TransactionAttribute(TransactionMode.Manual)]
    internal class DeleteElement : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                

                if(pickedobj != null)
                {
                    using(Transaction trans = new Transaction(doc, "Delete Element"))
                    {
                        trans.Start();
                        doc.Delete(pickedobj.ElementId);
                        TaskDialog t = new TaskDialog("Delete Element");
                        t.MainContent = "Are you sure";
                        t.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

                        if(t.Show() == TaskDialogResult.Ok)
                        {
                            trans.Commit();
                            TaskDialog.Show("Delete", pickedobj.ElementId.ToString() + " deleted");

                        }
                        else
                        {
                            trans.RollBack();
                            TaskDialog.Show("Delete", pickedobj.ElementId.ToString() + " not deleted");

                        }
                        
                    }
                    

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
