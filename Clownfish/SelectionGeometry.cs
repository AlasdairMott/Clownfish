using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

namespace Clownfish
{
	public class SelectionGeometry
	{
		public Brep brep;
		public Mesh brep_renderMesh;
		public Mesh[] face_renderMesh;
		public bool selected;
		public bool[] face_selected;
		
		public double parameter;
		public double[] face_parameters;

		private ClownfishComponent parent;

		public SelectionGeometry(Brep b, ClownfishComponent c) {
			brep = b;
			selected = false;
			parent = c;
			
			brep_renderMesh = new Mesh();

			face_renderMesh = Mesh.CreateFromBrep(b, MeshingParameters.FastRenderMesh);
			foreach (Mesh m in face_renderMesh) brep_renderMesh.Append(m);

			face_selected = new bool[face_renderMesh.Length];
			face_parameters = new double[face_renderMesh.Length];

			Subscribe();
		}

		public void Subscribe() {parent.mouseSelector.MousePressed += OnMouseDown;}

		public void Unsubscribe() {parent.mouseSelector.MousePressed -= OnMouseDown; }

		private void OnMouseDown(object sender, MouseSelectHandler e)
		{
			if ((selected && !e.remove) || (!selected && e.remove)) return;

			//Find Screen Point
			Point2d mouse_pt = new Point2d(e.point.X, e.point.Y);
			Line l = e.viewport.ClientToWorld(mouse_pt);
			Plane projection_plane = e.viewport.GetConstructionPlane().Plane;
			Point3d viewport_point;
			if (Rhino.Geometry.Intersect.Intersection.LinePlane(l, projection_plane, out double p)) viewport_point = l.PointAt(p);
			else return;

			//Create Ray from camera
			Point3d camera_pt = e.viewport.CameraLocation;
			Vector3d direction = new Vector3d(viewport_point - camera_pt);
			Ray3d r = new Rhino.Geometry.Ray3d(camera_pt, direction);

			if (!parent.selectFaces)
			{
				//Object selection
				parameter = Rhino.Geometry.Intersect.Intersection.MeshRay(brep_renderMesh, r);
				if (parameter > 0.0)
				{
					parent.mouseSelector.selected.Add(this);
				}
			}
			else 
			{
				//Face selection
				for (int i = 0; i < face_renderMesh.Length; i++) {
					p = Rhino.Geometry.Intersect.Intersection.MeshRay(face_renderMesh[i], r);
					face_parameters[i] = p;
				}
				int index_max = face_parameters.ToList<double>().IndexOf(face_parameters.Max());
				if (face_parameters[index_max] > 0) 
				{ 
					face_selected[index_max] = true;
					parent.mouseSelector.selected.Add(this);
				}
			}
		}
	}
}
