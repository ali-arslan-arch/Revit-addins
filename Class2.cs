using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    internal class Class2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            try
            {
                Element element = doc.GetElement(pickedobj);
                

                
                return Result.Succeeded;

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
        public void GetDimensions(FamilyInstance familyInstance, out double width, out double height)
        {
            // Varsayılan değerler
            width = 0.0;
            height = 0.0;

            // 1. Genişlik (Instance Parametre Kontrolü)
            Parameter widthParam = familyInstance.get_Parameter(BuiltInParameter.WINDOW_WIDTH)
                                   ?? familyInstance.get_Parameter(BuiltInParameter.DOOR_WIDTH);

            if (widthParam == null)
            {
                // 2. Eğer Instance Parametresi Yoksa, Type Parametreyi Kontrol Et
                widthParam = familyInstance.Symbol.get_Parameter(BuiltInParameter.WINDOW_WIDTH)
                             ?? familyInstance.Symbol.get_Parameter(BuiltInParameter.DOOR_WIDTH);
            }

            if (widthParam != null)
            {
                width = widthParam.AsDouble(); // Revit'in dahili birimlerinde (feet)
            }

            // 1. Yükseklik (Instance Parametre Kontrolü)
            Parameter heightParam = familyInstance.get_Parameter(BuiltInParameter.WINDOW_HEIGHT)
                                    ?? familyInstance.get_Parameter(BuiltInParameter.DOOR_HEIGHT);

            if (heightParam == null)
            {
                // 2. Eğer Instance Parametresi Yoksa, Type Parametreyi Kontrol Et
                heightParam = familyInstance.Symbol.get_Parameter(BuiltInParameter.WINDOW_HEIGHT)
                              ?? familyInstance.Symbol.get_Parameter(BuiltInParameter.DOOR_HEIGHT);
            }

            if (heightParam != null)
            {
                height = heightParam.AsDouble(); // Revit'in dahili birimlerinde (feet)
            }
        }
    }
}
