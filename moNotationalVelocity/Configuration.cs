using System;
using Microsoft.Win32;

namespace moNotationalVelocity
{
	public class Configuration
	{

		public string notesDirPath;

		private string defaulNotesDirtPath = System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "moNotational Data");

		private string platform;

		private RegistryKey OurAppRootKey = null;
		private RegistryKey ConfigKey = null;
		
		public Configuration ()
		{
			OperatingSystem os = Environment.OSVersion;
			Console.WriteLine ("OS platform: " + os.Platform);
			this.platform = os.Platform.ToString ();
			
			if (this.platform.StartsWith ("Win")) {
				
				RegistryKey CurrentUserKey = Microsoft.Win32.Registry.CurrentUser;
				
				string OurAppKeyStr = @"SOFTWARE\moNotationalVelocity";
				OurAppRootKey = CurrentUserKey.CreateSubKey (OurAppKeyStr);
				ConfigKey = OurAppRootKey.CreateSubKey ("config");
				
				this.notesDirPath = ConfigKey.GetValue ("notesDirPath") as string;
				if (this.notesDirPath == null) {
					Console.WriteLine ("No configuration");
					this.notesDirPath = defaulNotesDirtPath;
					ConfigKey.SetValue ("notesDirPath", this.notesDirPath, RegistryValueKind.String);
				}
				
				ConfigKey.Flush ();
				
			} else {
				
				
				
				this.notesDirPath = defaulNotesDirtPath;
			}
		
		}
	}
}

