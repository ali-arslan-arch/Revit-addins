using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MyRevitCommands
{
    internal class ExternalApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //Create Ribbon Tab
            application.CreateRibbonTab("AA-Tools");

            string path = Assembly.GetExecutingAssembly().Location;
            PushButtonData button1 = new PushButtonData("Button1", "PlaceLintel      ", path, "MyRevitCommands.PlaceLintel");
            PushButtonData button2 = new PushButtonData("Button2", "CroppedPlanBasedOnFamilyDimensions        ", path, "MyRevitCommands.CroppedPlanBasedOnFamilyDimensions");
            PushButtonData button3 = new PushButtonData("Button3", "LinkedElementSectionBox       ", path, "MyRevitCommands.LinkedElementSectionBox");
            PushButtonData button4 = new PushButtonData("Button4", "AddRoomSeparator         ", path, "MyRevitCommands.AddRoomSeparator");
            PushButtonData button5 = new PushButtonData("Button5", "FragmentPlanbyRegion", path, "MyRevitCommands.FragmentPlanbyRegion");
            
            RibbonPanel panel = application.CreateRibbonPanel("AA-Tools", "Commands");

            panel.AddItem(button1);
            panel.AddItem(button2);
            panel.AddItem(button3);
            panel.AddItem(button4);
            panel.AddItem(button5);

            return Result.Succeeded;
        }
    }
}
