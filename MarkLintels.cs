using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class MarkLintels : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                LintelMarkForm form = new LintelMarkForm();
                if (form.ShowDialog() == DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }

                FilteredElementCollector lintelcol =new FilteredElementCollector(doc,doc.ActiveView.Id);
                List<FamilyInstance> familyInstances = lintelcol.OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();
                FamilySymbol lento200 = SelectLintelType.lintelfor200;
                FamilySymbol lento100 = SelectLintelType.lintelfor100;
                FamilySymbol lento250 = SelectLintelType.lintelfor250;
                List<FamilyInstance> ikiyüz = new List<FamilyInstance>();
                List<FamilyInstance> ikiyüzelli = new List<FamilyInstance>();
                List<FamilyInstance> yüz = new List<FamilyInstance>();
                List<FamilyInstance> other = new List<FamilyInstance>();
                
                foreach (FamilyInstance fi in familyInstances)
                {
                    if(fi.Symbol.Id == lento200.Id)
                    {
                        ikiyüz.Add(fi);
                    }
                    else if(fi.Symbol.Id == lento250.Id)
                    {
                        ikiyüzelli.Add(fi);
                    }
                    else if (fi.Symbol.Id == lento100.Id)
                    {
                        yüz.Add(fi);
                    }
                    else { other.Add(fi); }
                    
                    

                }
                //gruplama
                ILookup<string, FamilyInstance> ikiyüzlength = ikiyüz.ToLookup(fi =>
                {
                    Parameter lengthParam = fi.LookupParameter(SelectLintelType.parametername200length);
                    double length = lengthParam?.AsDouble() ?? 0;

                    // LeftAnk değeri
                    Parameter leftAnkParam = fi.LookupParameter(SelectLintelType.parametername200leftank);
                    int leftAnk = leftAnkParam?.AsInteger() ?? 0;

                    // RightAnk değeri
                    Parameter rightAnkParam = fi.LookupParameter(SelectLintelType.parametername200rightank);
                    int rightAnk = rightAnkParam?.AsInteger() ?? 0;

                    
                    return $"{length:F2}-{leftAnk}-{rightAnk}";
                });
                ILookup<string, FamilyInstance> ikiyüzellilength = ikiyüzelli.ToLookup(fi =>
                {
                    Parameter lengthParam = fi.LookupParameter(SelectLintelType.parametername200length);
                    double length = lengthParam?.AsDouble() ?? 0;

                    // LeftAnk değeri
                    Parameter leftAnkParam = fi.LookupParameter(SelectLintelType.parametername200leftank);
                    int leftAnk = leftAnkParam?.AsInteger() ?? 0;

                    // RightAnk değeri
                    Parameter rightAnkParam = fi.LookupParameter(SelectLintelType.parametername200rightank);
                    int rightAnk = rightAnkParam?.AsInteger() ?? 0;

                    
                    return $"{length:F2}-{leftAnk}-{rightAnk}";
                });
                ILookup<string, FamilyInstance> yüzlength = yüz.ToLookup(fi =>
                {
                    Parameter lengthParam = fi.LookupParameter(SelectLintelType.parametername100length);
                    double length = lengthParam?.AsDouble() ?? 0;

                    // LeftAnk değeri
                    Parameter leftAnkParam = fi.LookupParameter(SelectLintelType.parametername100leftank);
                    int leftAnk = leftAnkParam?.AsInteger() ?? 0;

                    // RightAnk değeri
                    Parameter rightAnkParam = fi.LookupParameter(SelectLintelType.parametername100rightank);
                    int rightAnk = rightAnkParam?.AsInteger() ?? 0;

                    
                    return $"{length:F2}-{leftAnk}-{rightAnk}";
                });
                string first = form.korpus;
                string mark = first + "-ПР";
                int imark = form.number;
                using (Transaction trans = new Transaction(doc, "Mark the lintels"))
                {
                    trans.Start();
                    foreach (var group in ikiyüzlength)
                    {
                        string groupKey = group.Key;
                        string markValue = $"{mark}-{imark}";
                        foreach (FamilyInstance fi in group)
                        {
                            Parameter marka = fi.LookupParameter("06_МАРКА_ПЕРЕМЫЧКИ");
                            if (marka != null && !marka.IsReadOnly)
                            {
                                marka.Set(markValue);
                            }
                        }

                        imark++;

                    }
                    foreach (var group in ikiyüzellilength)
                    {
                        string groupKey = group.Key;
                        string markValue = $"{mark}-{imark}";
                        foreach (FamilyInstance fi in group)
                        {
                            Parameter marka = fi.LookupParameter("06_МАРКА_ПЕРЕМЫЧКИ");
                            if (marka != null && !marka.IsReadOnly)
                            {
                                marka.Set(markValue);
                            }
                        }

                        imark++;



                    }
                    foreach (var group in yüzlength)
                    {
                        string groupKey = group.Key;
                        string markValue = $"{mark}-{imark}";
                        foreach (FamilyInstance fi in group)
                        {
                            Parameter marka = fi.LookupParameter("06_МАРКА_ПЕРЕМЫЧКИ");
                            if (marka != null && !marka.IsReadOnly)
                            {
                                marka.Set(markValue);
                            }
                        }

                        imark++;

                    }

                    trans.Commit();

                }
                
                //06_МАРКА_ПЕРЕМЫЧКИ
                //К1 - ПР - 7

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
