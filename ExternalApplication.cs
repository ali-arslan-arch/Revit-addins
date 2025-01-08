using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            string authorInfo = "Author: Ali Arslan\nEmail: arch.aliarslan@gmail.com";
            PulldownButtonData pulldownData = new PulldownButtonData("pulldown1","Create Cropped View");
            PulldownButtonData pulldownData2 = new PulldownButtonData("pulldown2", "Place Lintel");

            PushButtonData button1 = new PushButtonData("Button1", "Place Lintel to all in view", path, "MyRevitCommands.PlaceLintel");
            
            PushButtonData button2 = new PushButtonData("Button2", "Crop plan by multiple elements", path, "MyRevitCommands.CroppedPlanBasedOnFamilyDimensions");
            PushButtonData button6 = new PushButtonData("Button6", "Crop front section by multiple elements", path, "MyRevitCommands.CroppedSectionBasedOnFamilyDimensions");
            PushButtonData button7 = new PushButtonData("Button7", "Crop center cut section by multiple elements", path, "MyRevitCommands.CropSec");
            PushButtonData button3 = new PushButtonData("Button3", "Linked Element SectionBox       ", path, "MyRevitCommands.LinkedElementSectionBox");
            PushButtonData button4 = new PushButtonData("Button4", "Add Room Separator         ", path, "MyRevitCommands.AddRoomSeparator");
            PushButtonData button5 = new PushButtonData("Button5", "Fragment Plan by Region", path, "MyRevitCommands.FragmentPlanbyRegion");
            PushButtonData button8 = new PushButtonData("Button8", "Crop plan by selected element", path, "MyRevitCommands.CropPlanPickElement");
            PushButtonData button9 = new PushButtonData("Button9", "Crop front section by selected element", path, "MyRevitCommands.CropFrontSecPickElement");
            PushButtonData button10 = new PushButtonData("Button10", "Crop Center Cut section by selected element", path, "MyRevitCommands.CropCutSecPickElement");
            PushButtonData button11 = new PushButtonData("Button11", "Crop plan by selected linked element", path, "MyRevitCommands.CropPlanLinkedElement");
            PushButtonData button12 = new PushButtonData("Button12", "Place Lintel to single element", path, "MyRevitCommands.PlaceLintelSingle");
            PushButtonData button13 = new PushButtonData("Button13", "Crop front section by selected linked element", path, "MyRevitCommands.CropFrontSeclink");
            PushButtonData button14 = new PushButtonData("Button14", "Crop Center Cut section by selected linked element", path, "MyRevitCommands.CropCutSecLink");
            PushButtonData button15 = new PushButtonData("Button15", "Select Lintel Family Type", path, "MyRevitCommands.SelectLintelType");
            PushButtonData button16 = new PushButtonData("Button16", "Mark the lintels", path, "MyRevitCommands.MarkLintels");
            button1.LongDescription = authorInfo;
            button2.LongDescription = authorInfo;
            button3.LongDescription = authorInfo;
            button4.LongDescription = authorInfo;
            button5.LongDescription = authorInfo;
            button6.LongDescription = authorInfo;
            button7.LongDescription = authorInfo;
            button8.LongDescription = authorInfo;
            button9.LongDescription = authorInfo;
            button10.LongDescription = authorInfo;
            button11.LongDescription = authorInfo;
            button12.LongDescription = authorInfo;
            button13.LongDescription = authorInfo;
            button14.LongDescription = authorInfo;
            button15.LongDescription = authorInfo;
            button16.LongDescription = authorInfo;


            button1.ToolTip = "Automatically places lintels for all doors in the active view.";
            button2.ToolTip = "Creates cropped plan views based on the dimensions of door or window elements in active view";
            button3.ToolTip = "Select an element from the linked model to create a Section Box around it.";
            button4.ToolTip = "Automatically adds Room Separators to the boundaries of all rooms in the active view.";
            button5.ToolTip = "Creates floor plans based on the boundaries of the selected region on the chosen levels.";
            button6.ToolTip = "Creates cropped section views from the front of door or window elements in active view";
            button7.ToolTip = "Creates cropped section views perpendicular to door or window elements in active view";
            button8.ToolTip = "Creates a cropped plan view based on the dimensions of selected door or window element";
            button9.ToolTip = "Creates a cropped section view from the front of selected door or window element";
            button10.ToolTip = "Creates a cropped section view perpendicular to selected door or window element";
            button11.ToolTip = "Creates a cropped plan view based on the dimensions of selected linked door or window element";
            button12.ToolTip = "Automatically places lintel for selected door element";
            button13.ToolTip = "Creates a cropped section view from the front of selected linked door or window element";
            button14.ToolTip = "Creates a cropped section view perpendicular to selected linked door or window element";
            button15.ToolTip = "Select Lintel Family types before running the \"Place Lintel to single element\" tool";
            button16.ToolTip = "Groups lintels based on their lengths, profile types, thicknesses, and anchorage conditions and marks them.";
            RibbonPanel panel = application.CreateRibbonPanel("AA-Tools", "Commands");
            
            PulldownButton pulldownButton = panel.AddItem(pulldownData) as PulldownButton;
            pulldownButton.AddPushButton(button2);
            pulldownButton.AddPushButton(button6);
            pulldownButton.AddPushButton(button7);
            pulldownButton.AddPushButton(button8);
            pulldownButton.AddPushButton(button9);
            pulldownButton.AddPushButton(button10);
            pulldownButton.AddPushButton(button11);
            pulldownButton.AddPushButton(button13);
            pulldownButton.AddPushButton(button14);

            PulldownButton pulldownButton2 = panel.AddItem(pulldownData2) as PulldownButton;
            pulldownButton2.AddPushButton(button15);
            pulldownButton2.AddPushButton(button1);
            pulldownButton2.AddPushButton(button12);
            pulldownButton2.AddPushButton(button16);
            //panel.AddItem(button2);
            panel.AddItem(button3);
            panel.AddItem(button4);
            panel.AddItem(button5);
            //panel.AddItem(button6);
            //panel.AddItem(button7);

            return Result.Succeeded;
        }
    }
}
