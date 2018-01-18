using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Rhino.Geometry;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace AutoSavePlus
{
    public class InitializeAutoSavePlus : Grasshopper.Kernel.GH_AssemblyPriority
    {

        public override GH_LoadingInstruction PriorityLoad()
        { 
            Instances.CanvasCreated += AppendAutoSavePlus; 
            return GH_LoadingInstruction.Proceed;
        }

        private void AppendAutoSavePlus( GH_Canvas canvas)
        {
            Instances.CanvasCreated -= AppendAutoSavePlus;
            foreach (Grasshopper.GUI.Widgets.IGH_Widget w in canvas.Widgets) if (w.Name == "AutoSave Plus") return;
            canvas.Widgets.Add(new AutoSavePlus()); 
        }

       
    }

    public class AutoSavePlus : Grasshopper.GUI.Widgets.GH_Widget
    {
        bool enabled; 

        public AutoSavePlus( ) : base(){
            enabled = Grasshopper.Instances.Settings.GetValue("Widget.AutoSavePlus.Show", true);
            Owner = Grasshopper.Instances.ActiveCanvas;
            if (enabled) Owner.DocumentChanged += DocumentChanged;
        }

        public void DocumentChanged(GH_Canvas canvas, Grasshopper.GUI.Canvas.GH_CanvasDocumentChangedEventArgs args)
        {
            if (!enabled || args.NewDocument == null) return;
            if (!args.NewDocument.IsFilePathDefined) {
                DateTime now = DateTime.Now; 
                string name = string.Format("unnamed ({0}.{1}.{2}, {3}.{4}.{5})", now.Day,now.Month,now.Year,now.Hour,now.Minute,now.Second) ;
                args.NewDocument.Properties.ProjectFileName = name;
                args.NewDocument.FilePath = Grasshopper.Folders.AutoSaveFolder + "\\"+name+".gh";
                args.NewDocument.AutoSave(GH_AutoSaveTrigger.data_matching_event, Guid.Empty);
            }
           
        }

        public override bool Visible {
            get { return enabled; }
            set {
                if (value == enabled) return ;
                Grasshopper.Instances.Settings.SetValue("Widget.AutoSavePlus.Show", value);
                enabled = value;
                if (value)
                {
                    Owner.DocumentChanged += DocumentChanged;
                }
                else {
                    Owner.DocumentChanged -= DocumentChanged;
                }
            }
        }
        public override string Name => "AutoSave Plus";
        public override string Description => "Automatically saves new documents";
        public override Bitmap Icon_24x24 => Properties.Resources.Icon24x24;
        public override bool Contains(System.Drawing.Point pt_control, PointF pt_canvas) => false;
        public override void Render(GH_Canvas Canvas) { }
        public override string TooltipText => Description;
    }

    public class AutoSavePlusInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "AutoSavePlus";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                return Properties.Resources.Icon24x24;
            }
        }
        public override string Description
        {
            get
            {
                return "Perform autosave on an unsaved document";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("93242324-4e09-40b6-9b13-6eace2f859ce");
            }
        }
        public override string AuthorName
        {
            get
            {
                return "Daniel Abalde";
            }
        }
        public override string AuthorContact
        {
            get
            {
                return "dga_3@hotmail.com";
            }
        } 
        public override string Version => "2nd";
    }
}
