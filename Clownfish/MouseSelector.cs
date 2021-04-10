using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.UI;
using Rhino.Display;

namespace Clownfish
{
	public class MouseSelector : MouseCallback
	{
		public event EventHandler<MouseSelectHandler> MousePressed;
		private ClownfishComponent parent;

		public MouseSelector(ClownfishComponent p) { parent = p; } 

		protected override void OnMouseDown(MouseCallbackEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.MouseButton == MouseButton.Right) return;

			MouseSelectHandler mouseSelect = new MouseSelectHandler(e.View.ActiveViewport, e.ViewportPoint, e.CtrlKeyDown);
			MousePressed?.Invoke(this, mouseSelect);
			parent.ExpireSolution(true);

			e.View.Redraw();
		}
	}

	public class MouseSelectHandler : EventArgs
	{
		private readonly RhinoViewport _viewport;
		private readonly System.Drawing.Point _point;
		private readonly bool _remove;

		public MouseSelectHandler(RhinoViewport vp, System.Drawing.Point pt, bool r)
		{
			_viewport = vp;
			_point = pt;
			_remove = r;
		}

		public RhinoViewport viewport { get { return _viewport; } }
		public System.Drawing.Point point { get { return _point; } }
		public bool remove { get { return _remove; } }
	}
}
