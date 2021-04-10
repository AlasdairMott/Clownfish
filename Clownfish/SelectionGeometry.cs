using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

namespace Clownfish
{
	class SelectionGeometry
	{
		public Brep brep;
		public Mesh brep_renderMesh;
		public bool selected;
		
		private ClownfishComponent parent;

		public SelectionGeometry(Brep b, ClownfishComponent c) {
			brep = b;
			selected = false;
			parent = c;
			
			brep_renderMesh = new Mesh();

			Mesh[] mesh = Mesh.CreateFromBrep(b, MeshingParameters.FastRenderMesh);
			foreach (Mesh m in mesh) brep_renderMesh.Append(m);

			Subscribe();
		}

		public void Subscribe() {parent.mouseSelector.MousePressed += OnMouseDown;}

		public void Unsubscribe() {parent.mouseSelector.MousePressed -= OnMouseDown; }

		private void OnMouseDown(object sender, MouseSelectHandler e)
		{
			if (selected && !e.remove) return;

			Point2d mouse_pt = new Point2d(e.point.X, e.point.Y);
			
			Line l = e.viewport.ClientToWorld(mouse_pt);
			Plane projection_plane = e.viewport.GetConstructionPlane().Plane;

			Point3d viewport_point;
			if (Rhino.Geometry.Intersect.Intersection.LinePlane(l, projection_plane, out double p)) viewport_point = l.PointAt(p);
			else return;

			Point3d camera_pt = e.viewport.CameraLocation;
			Vector3d direction = new Vector3d(viewport_point - camera_pt);

			Ray3d r = new Rhino.Geometry.Ray3d(camera_pt, direction);

			p = Rhino.Geometry.Intersect.Intersection.MeshRay(brep_renderMesh, r);
			if (p > 0.0)
			{
				if (e.remove)
				{
					selected = false;
					Rhino.RhinoApp.WriteLine("Removed Geometry");
				}
				else {
					selected = true;
					Rhino.RhinoApp.WriteLine("Selected Geometry");
				}
			}
		}
	}
}
