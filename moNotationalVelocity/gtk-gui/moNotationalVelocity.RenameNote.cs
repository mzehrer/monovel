
// This file has been generated by the GUI designer. Do not modify.
namespace moNotationalVelocity
{
	public partial class RenameNote
	{
		private global::Gtk.Label label1;

		private global::Gtk.Entry noteTitle;

		private global::Gtk.Button buttonCancel;

		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget moNotationalVelocity.RenameNote
			this.Name = "moNotationalVelocity.RenameNote";
			this.Title = global::Mono.Unix.Catalog.GetString ("Rename note");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child moNotationalVelocity.RenameNote.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.Spacing = 5;
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Rename note to:");
			this.label1.Wrap = true;
			w1.Add (this.label1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(w1[this.label1]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.noteTitle = new global::Gtk.Entry ();
			this.noteTitle.CanFocus = true;
			this.noteTitle.Name = "noteTitle";
			this.noteTitle.IsEditable = true;
			this.noteTitle.InvisibleChar = '•';
			w1.Add (this.noteTitle);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(w1[this.noteTitle]));
			w3.Position = 1;
			w3.Expand = false;
			w3.Fill = false;
			// Internal child moNotationalVelocity.RenameNote.ActionArea
			global::Gtk.HButtonBox w4 = this.ActionArea;
			w4.Name = "dialog1_ActionArea";
			w4.Spacing = 10;
			w4.BorderWidth = ((uint)(5));
			w4.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w5 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w4[this.buttonCancel]));
			w5.Expand = false;
			w5.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w6 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w4[this.buttonOk]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 296;
			this.DefaultHeight = 130;
			this.Show ();
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnOkClick);
		}
	}
}
