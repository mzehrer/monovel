using System;

namespace moNotationalVelocity
{
	public class Note
	{
		
		public string title;
		public string lastModified;
		
		public Note ()
		{
		}
		
		public Note (string title, string lastModified)
		{
			this.title = title;
			this.lastModified = lastModified;
		}
		
	}
}

