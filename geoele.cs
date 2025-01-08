using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class geoele : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement);
                RevitLinkInstance revitLinkInstance = doc.GetElement(pickedobj) as RevitLinkInstance;
                Transform transform = revitLinkInstance.GetTransform();
                Document linkdocument = revitLinkInstance.GetLinkDocument();
                ElementId eleid = pickedobj.LinkedElementId;
                Element ele = linkdocument.GetElement(eleid);
                FamilyInstance family = (FamilyInstance)ele;
                FamilySymbol fs = family.Symbol;
                Options op = new Options();
                op.DetailLevel = ViewDetailLevel.Fine;
                GeometryElement gg = fs.get_Geometry(op);
                Solid sl = null;
                foreach (GeometryObject geo in gg)
                {
                    Solid csol = null;
                    csol = geo as Solid;
                    if (csol.Volume > 0)
                    {
                        sl = SolidUtils.CreateTransformed(csol, transform);
                    }

                }

                Reference pick2 = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                Element ele2 = doc.GetElement(pick2);
                FamilyInstance fi = (FamilyInstance)ele2;
                ICollection<ElementId> subComponentIds = fi.GetSubComponentIds();
                List<FamilyInstance> subComponents = new List<FamilyInstance>();
                
                
                /*foreach (FamilyInstance f in nsc)
                {
                    TaskDialog.Show("asd",f.Symbol.FamilyName);
                }*/
                //bool idAdded = false;
                Solid sl2 = null;
                foreach (ElementId subComponentId in subComponentIds)
                {
                    // Alt family instance'ı elde edin
                    FamilyInstance nestedFamilyInstance = doc.GetElement(subComponentId) as FamilyInstance;
                    subComponents.Add(nestedFamilyInstance);
                    //TaskDialog.Show("asd", nestedFamilyInstance.Symbol.FamilyName);

                    if (nestedFamilyInstance != null)
                    {
                        // Alt family instance'ın geometrisini al
                        
                            

                        
                    }
                }
                List<FamilyInstance> nsc = subComponents.Where(x => x.Symbol.FamilyName.Contains("Уголок") == true).ToList();
                //TaskDialog.Show("asd", nsc.Count.ToString());
                GeometryElement geometryElement = nsc[0].get_Geometry(op);
                TaskDialog.Show("asd",geometryElement.Count().ToString());

                
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
                }

                if (sl2 != null)
                {
                    TaskDialog.Show("asd",sl2.Volume.ToString());
                }
                if (sl != null)
                {
                    TaskDialog.Show("asd", sl.Volume.ToString());
                }
                if(CheckSolidIntersection2(sl, sl2))
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
            
            
                // Kesişim işlemini yapıyoruz
                Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(solid, sol2, BooleanOperationsType.Intersect);
                
                // Eğer intersection null değilse ve hacmi sıfırdan büyükse çakışma var demektir
                if (intersection != null)
                {
                    TaskDialog.Show("asd", intersection.Volume.ToString());
                    if (intersection.Volume > 0 || intersection.SurfaceArea > 0)
                    {
                        return true; // Çakışma veya temas var
                    }
                }
            

            return false; // Çakışma yok
        }

    }
}
