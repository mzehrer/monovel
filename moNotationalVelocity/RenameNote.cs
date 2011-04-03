using System;
using GLib;

namespace moNotationalVelocity
{
	public partial class RenameNote : Gtk.Dialog
	{

		public string NoteTitle;

		[ConnectBefore]
		public void init ()
		{
			noteTitle.Text = this.NoteTitle;
			noteTitle.GrabFocus ();
		}

		protected virtual void OnOkClick (object sender, System.EventArgs e)
		{
			this.NoteTitle = noteTitle.Text;
		}

		[ConnectBefore]
		protected virtual void onInputKeyEvent (object o, Gtk.KeyPressEventArgs args)
		{
			string key = args.Event.Key.ToString ();
			Console.WriteLine ("name input key: " + key);
			
			if (key.Equals ("Return")) {
				buttonOk.Click();
			}
		}

		public RenameNote ()
		{
			this.Build ();
			noteTitle.KeyPressEvent += new global::Gtk.KeyPressEventHandler (onInputKeyEvent);
			
		}
		
	}
}

