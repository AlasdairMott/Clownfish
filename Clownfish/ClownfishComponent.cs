using System;
using System.Collections.Generic;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Clownfish
{
	public class ClownfishComponent : GH_Component
	{
		public MouseSelector mouseSelector;
		private List<int> indices;
		private List<SelectionGeometry> selectionGeometries;
		public bool selectionRetrigger;
		public bool selectThroughObjects;
		private Rhino.Display.DisplayMaterial display_material;



		/// <summary>
		/// Each implementation of GH_Component must provide a public 
		/// constructor without any arguments.
		/// Category represents the Tab in which the component will appear, 
		/// Subcategory the panel. If you use non-existing tab or panel names, 
		/// new tabs/panels will automatically be created.
		/// </summary>
		public ClownfishComponent()
		  : base("Clownfish", "C",
			  "Object Selection",
			  "Params", "Clownfish")
		{
			indices = new List<int>();
			mouseSelector = new MouseSelector(this);
			selectionGeometries = new List<SelectionGeometry>();
			selectionRetrigger = false;
			selectThroughObjects = false;
			display_material = new Rhino.Display.DisplayMaterial(System.Drawing.Color.Yellow);
		}

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			pManager.AddBrepParameter("brep", "B", "breps to include in selection", GH_ParamAccess.list);
			pManager.AddBooleanParameter("Select", "S", "Select points", GH_ParamAccess.item);
		}

		/// <summary>
		/// Registers all the output parameters for this component.
		/// </summary>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.AddIntegerParameter("index", "i", "index of selected", GH_ParamAccess.list);
		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object can be used to retrieve data from input parameters and 
		/// to store data in output parameters.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			bool run = false;
			DA.GetData("Select", ref run);
			if (run) mouseSelector.Enabled = true;
			else 
			{
				mouseSelector.Enabled = false;
				selectionRetrigger = true;
			}
			
			List<Brep> breps = new List<Brep>();
			DA.GetDataList("brep", breps);

			if (breps.Count == 0) return;

			if (!selectionRetrigger) {
				//first unsubscribe the selectionGeometries in the list
				foreach (SelectionGeometry sG in selectionGeometries) {
					sG.Unsubscribe();
				}

				//Clear the list and create the selection geometries
				selectionGeometries.Clear();
				foreach (Brep b in breps)
				{
					selectionGeometries.Add(new SelectionGeometry(b, this));
				}
			}

			indices.Clear();
			for (int i = 0; i < selectionGeometries.Count; i++)
			{
				var sG = selectionGeometries[i];
				if (sG.selected) indices.Add(i);
			}

			DA.SetDataList("index", indices);

			if (mouseSelector.Enabled) selectionRetrigger = false;

		}

		public override void RemovedFromDocument(GH_Document document)
		{
			base.RemovedFromDocument(document);
			mouseSelector.Enabled = false;
			
			//Rhino.RhinoApp.WriteLine("Removed from document");
		}

		public override void DrawViewportMeshes(IGH_PreviewArgs args) 
		{
			for (int i = 0; i < selectionGeometries.Count; i++)
			{
				var sG = selectionGeometries[i];
				if (sG.selected) 
				{
					args.Display.DrawMeshShaded(sG.brep_renderMesh, display_material);
				}
			}
		}

		protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
		{
			base.AppendAdditionalComponentMenuItems(menu);
			Menu_AppendItem(menu, "Select Through Objects", Menu_SelectThroughObjects, true, selectThroughObjects);
		}

		private void Menu_SelectThroughObjects(object sender, EventArgs e)
		{
			selectThroughObjects = !selectThroughObjects;
		}

		public override bool Write(GH_IWriter writer)
		{
			writer.SetBoolean("selectThroughObjects", this.selectThroughObjects);
			return base.Write(writer);
		}

		public override bool Read(GH_IReader reader)
		{
			this.selectThroughObjects = false;
			reader.TryGetBoolean("selectThroughObjects", ref this.selectThroughObjects);
			return base.Read(reader);
		}

		/// <summary>
		/// Provides an Icon for every component that will be visible in the User Interface.
		/// Icons need to be 24x24 pixels.
		/// </summary>
		protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.Clownfish; }}
		/// <summary>
		/// Each component must have a unique Guid to identify it. 
		/// It is vital this Guid doesn't change otherwise old ghx files 
		/// that use the old ID will partially fail during loading.
		/// </summary>
		public override Guid ComponentGuid
		{
			get { return new Guid("d121aefa-fb0a-40a1-90ee-64a89050d3af"); }
		}
	}
}
