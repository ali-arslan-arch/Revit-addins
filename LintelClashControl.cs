using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    internal class LintelClashControl : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<ElementId> elementsIds = new List<ElementId>();
            try
            {
                //Aktif viewdaki family instance'ların seçimi
                FilteredElementCollector col = new FilteredElementCollector(doc, doc.ActiveView.Id);
                List<FamilyInstance> familyInstances = col.WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();
                //Aktif viewdaki family instance'ların seçimi

                List<Solid> lintelsolids = new List<Solid>();

                //Linkteki kolon ve duvarlar
                /*Reference pickedobj = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                
                Element linkele = doc.GetElement(pickedobj);*/
                List<Solid> linksolids = new List<Solid>();
                Links rvtlink = new Links(doc);
                if(rvtlink.ShowDialog() == DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }
                RevitLinkInstance link = rvtlink.rl;
                    //linkele as RevitLinkInstance;
                Document linkdoc = link.Document;
                FilteredElementCollector linkcol = new FilteredElementCollector(linkdoc);
                IList<BuiltInCategory> categories = new List<BuiltInCategory>(){BuiltInCategory.OST_Walls,BuiltInCategory.OST_StructuralColumns};

                Options op = new Options();
                op.DetailLevel = ViewDetailLevel.Fine;
                ElementMulticategoryFilter multiCategoryFilter = new ElementMulticategoryFilter(categories);
                List<Element> linklist = linkcol.WherePasses(multiCategoryFilter).WhereElementIsNotElementType().ToElements().ToList();
                
                foreach (Element ee in linklist)
                {
                    
                    if(ee.GetType() == typeof(Wall))
                    {
                        
                        IList<Solid> wallsolids = GetTargetSolids(ee);
                        foreach(Solid wsol in wallsolids)
                        {
                            
                            
                            if(wsol.Volume > 0) 
                            {
                                linksolids.Add(wsol);
                            }

                        }
                    }
                    if(ee.GetType() == typeof(FamilyInstance))
                    {

                        IList<Solid> colsolids = GetTargetSolids(ee);

                        foreach (Solid csol in colsolids)
                        {
                            
                            
                            if (csol.Volume > 0)
                            {
                                linksolids.Add(csol);
                            }

                        }


                    }
                }
                //Linkteki kolon ve duvarlar

                //Lento geometrisi
                /*FilteredElementCollector col2 = new FilteredElementCollector(doc, doc.ActiveView.Id);
                List<ElementId> idlist = new List<ElementId>();
                foreach (Solid s in linksolids)
                {
                    ElementIntersectsSolidFilter eisf = new ElementIntersectsSolidFilter(s);
                    
                        
                        
                        IList<Element> eles = col2.OfClass(typeof(FamilyInstance)).WherePasses(eisf).ToList();
                        foreach (Element ee in eles)
                        {
                            idlist.Add(ee.Id);
                        }


                    

                }
                TaskDialog.Show("asd", idlist.Count.ToString());*/
                
                foreach (FamilyInstance fi in familyInstances)
                {
                    
                    List<Solid> solids = new List<Solid>();
                    ICollection<ElementId> subComponentIds = fi.GetSubComponentIds();
                    bool idAdded = false;
                    foreach (ElementId subComponentId in subComponentIds)
                    {
                        // Alt family instance
                        FamilyInstance nestedFamilyInstance = doc.GetElement(subComponentId) as FamilyInstance;

                        if (nestedFamilyInstance != null)
                        {
                            IList<Solid> solids2 = GetTargetSolids(nestedFamilyInstance);
                            GeometryElement geometryElement = nestedFamilyInstance.get_Geometry(op);

                            // GeometryElement'i işlemeye başlayabilirsiniz
                            
                            
                                
                                
                                foreach (Solid sol in solids2)
                                {
                                    
                                    if (sol.Volume > 0)
                                    {
                                        //solids.Add(ss);
                                        bool a = CheckSolidIntersection(sol, linksolids);
                                        if (a)
                                        {
                                             if (!idAdded) 
                                             {
                                                elementsIds.Add(fi.Id);
                                               idAdded = true;
                                                break;

                                             }

                                            

                                        }
                                    }
                                }
                                if (idAdded) break;

                            
                        }
                        if (idAdded) break;
                    }
                    
                                                             
                    


                }
                //lento geometrisi
                /*foreach(ElementId elementId in elementsIds)
                {
                    TaskDialog.Show("asd",elementId.ToString());
                }*/
                return Result.Succeeded;
            }
            catch(Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            

        }
        public static bool CheckSolidIntersection(Solid solid, List<Solid> solidList)
        {
            foreach (Solid otherSolid in solidList)
            {
                
                Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(solid, otherSolid, BooleanOperationsType.Intersect);
                
                
                if (intersection != null)
                {
                    if (intersection.Volume > 0 || intersection.SurfaceArea > 0)
                    {
                        return true; 
                    }
                }
            }

            return false; 
        }
        public static IList<Solid> GetTargetSolids(Element element)
        {
            List<Solid> solids = new List<Solid>();


            Options options = new Options();
            options.DetailLevel = ViewDetailLevel.Fine;
            GeometryElement geomElem = element.get_Geometry(options);
            foreach (GeometryObject geomObj in geomElem)
            {
                if (geomObj is Solid)
                {
                    Solid solid = (Solid)geomObj;
                    if (solid.Faces.Size > 0 && solid.Volume > 0.0)
                    {
                        solids.Add(solid);
                    }
                    // Single-level recursive check of instances. If viable solids are more than
                    // one level deep, this example ignores them.
                }
                else if (geomObj is GeometryInstance)
                {
                    GeometryInstance geomInst = (GeometryInstance)geomObj;
                    GeometryElement instGeomElem = geomInst.GetInstanceGeometry();
                    foreach (GeometryObject instGeomObj in instGeomElem)
                    {
                        if (instGeomObj is Solid)
                        {
                            Solid solid = (Solid)instGeomObj;
                            if (solid.Faces.Size > 0 && solid.Volume > 0.0)
                            {
                                solids.Add(solid);
                            }
                        }
                    }
                }
            }
            return solids;
        }

    }
}
