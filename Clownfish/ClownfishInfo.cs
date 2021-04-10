using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Clownfish
{
	public class ClownfishInfo : GH_AssemblyInfo
	{
		public override string Name
		{
			get
			{
				return "Clownfish";
			}
		}
		public override Bitmap Icon
		{
			get
			{
				//Return a 24x24 pixel bitmap to represent this GHA library.
				return null;
			}
		}
		public override string Description
		{
			get
			{
				//Return a short string describing the purpose of this GHA library.
				return "";
			}
		}
		public override Guid Id
		{
			get
			{
				return new Guid("d4259b81-cdbb-41a7-b633-235987f6c913");
			}
		}

		public override string AuthorName
		{
			get
			{
				//Return a string identifying you or your company.
				return "";
			}
		}
		public override string AuthorContact
		{
			get
			{
				//Return a string representing your preferred contact details.
				return "";
			}
		}
	}
}
