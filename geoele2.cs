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
    internal class geoele2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                //kolon
                Reference pickedkolon = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element elecol = doc.GetElement(pickedkolon);
                FamilyInstance col = elecol as FamilyInstance;
                FamilySymbol colfs = col.Symbol;
                Options op = new Options();
                op.DetailLevel = ViewDetailLevel.Fine;
                GeometryElement gg = colfs.get_Geometry(op);
                Solid sl = null;
                //sl = GetTargetSolids(elecol)[0];
                /*foreach (GeometryObject geo in gg)
                {
                    Solid csol = null;
                    csol = geo as Solid;
                    if (csol.Volume > 0)
                    {
                        sl = csol;
                        break;
                    }

                }*/
                //lento
                /*Reference pickedlento = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element lentoele = doc.GetElement(pickedlento);
                FamilyInstance fi = (FamilyInstance)lentoele;
                ICollection<ElementId> subComponentIds = fi.GetSubComponentIds();
                List<FamilyInstance> subComponents = new List<FamilyInstance>();
                Solid sl2 = null;
                foreach (ElementId subComponentId in subComponentIds)
                {
                    
                    FamilyInstance nestedFamilyInstance = doc.GetElement(subComponentId) as FamilyInstance;
                    subComponents.Add(nestedFamilyInstance);
                    
                }
                List<FamilyInstance> nsc = subComponents.Where(x => x.Symbol.FamilyName.Contains("Уголок") == true).ToList();
                
                GeometryElement geometryElement = nsc[0].get_Geometry(op);
                TaskDialog.Show("asd", geometryElement.Count().ToString());

               


                foreach (GeometryObject geometryObject in geometryElement)
                {
                    GeometryInstance gi = geometryObject as GeometryInstance;
                    GeometryElement gel = gi.GetInstanceGeometry();
                    foreach (GeometryObject g in gel)
                    {
                        Solid sol = null;
                        sol = g as Solid;
                        if (sol.Volume > 0)
                        {
                            sl2 = sol; break;
                        }
                    }
                }*/
                //wall
                Reference pickedwall = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element elewall = doc.GetElement(pickedwall);
                Solid sl3 = null;
                GeometryElement wallge = elewall.get_Geometry(op);
                foreach (GeometryObject wall in wallge)
                {
                    Solid wallsol = wall as Solid;
                    
                }
                //sl3 = GetTargetSolids(elewall)[0];
                
                /*if (sl2 != null)
                {
                    TaskDialog.Show("asd", sl2.Volume.ToString());
                }*/
                if (sl != null)
                {
                    TaskDialog.Show("asd", sl.Volume.ToString());
                }
                Solid clonesl = SolidUtils.Clone(sl);
                //Solid clonesl2 = SolidUtils.Clone(sl2);
                Solid clonesl3 = SolidUtils.Clone(sl3);
                if (CheckSolidIntersection2(sl, sl3))
                {
                    TaskDialog.Show("asd", "yes");
                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

        }
        public static bool CheckSolidIntersection2(Solid solid, Solid sol2)
        {


            Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(solid, sol2, BooleanOperationsType.Intersect);

            
            if (intersection != null)
            {
                TaskDialog.Show("asd", intersection.Volume.ToString());
                if (intersection.Volume > 0 || intersection.SurfaceArea > 0)
                {
                    return true; 
                }
            }


            return false; // Çakışma yok
        }
        
    }
}
