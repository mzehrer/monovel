using System;
using Gtk;
using moNotationalVelocity;
using System.Collections;

public partial class MainWindow : Gtk.Window
{

	Gtk.NodeStore store;

	private string notesDirPath;
	private NotesStore notesStore;

	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		
		Configuration cfg = new Configuration ();
		
		noteslist.NodeStore = Store;
		noteslist.AppendColumn ("Titel", new Gtk.CellRendererText (), "text", 0);
		noteslist.AppendColumn ("Last modified", new Gtk.CellRendererText (), "text", 1);
		
		
		this.notesDirPath = cfg.notesDirPath;
		
		Console.WriteLine ("Using notes at " + notesDirPath);
		
		if (!System.IO.File.Exists (notesDirPath))
			System.IO.Directory.CreateDirectory (notesDirPath);
		
		notesStore = new FilesystemNotesStore (notesDirPath);
		
		loadNotes ();
		
		noteslist.NodeSelection.Changed += new System.EventHandler (OnSelectionChanged);
		
		searchbar.GrabFocus ();
		
	}

	[TreeNode(ListOnly = true)]
	public class NoteNode : Gtk.TreeNode
	{

		[Gtk.TreeNodeValue(Column = 0)]
		public string Title;

		[Gtk.TreeNodeValue(Column = 1)]
		public string LastModified {
			get { return last_modified; }
		}

		string last_modified;

		public NoteNode (Note note)
		{
			Title = note.title;
			this.last_modified = note.lastModified;
			
		}
	}


	Gtk.NodeStore Store {
		get {
			if (store == null) {
				store = new Gtk.NodeStore (typeof(NoteNode));
			}
			return store;
		}
	}

	protected void loadNotes ()
	{
		
		notesStore = new FilesystemNotesStore (this.notesDirPath);
		
		ArrayList notes = notesStore.getAllNotes ();
		
		for (int i = 0; i < notes.Count; i++) {
			Note noteEntry = notes[i] as Note;
			
			store.AddNode (new NoteNode (noteEntry));
		}
		
	}


	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	void OnSelectionChanged (object o, System.EventArgs args)
	{
		try {
			Gtk.NodeSelection selection = (Gtk.NodeSelection)o;
			NoteNode node = (NoteNode)selection.SelectedNode;
			
			TextBuffer buf = editor.Buffer;
			buf.Text = notesStore.getNoteContent (node.Title);
		} catch (Exception e) {
			Console.WriteLine (e.Message);
		}
	}

	protected virtual void OnKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
	{
		
		string key = args.Event.Key.ToString ();
		Console.WriteLine (key);
		
		if (key.Equals ("Escape")) {
			searchbar.GrabFocus ();
		} else if (key.Equals ("s") && noteslist.HasFocus) {
			searchbar.GrabFocus ();
		}
		
		
		
		
	}

	protected virtual void OnChanged (object sender, System.EventArgs e)
	{
		store.Clear ();
		string search = searchbar.Text.Trim ();
		if (search != null && search.Length > 0) {
			ArrayList notes = notesStore.getNotesMatchingTitle (search);
			if (notes.Count > 0) {
				for (int i = 0; i < notes.Count; i++) {
					Note noteEntry = notes[i] as Note;
					store.AddNode (new NoteNode (noteEntry));
				}
			} else {
				loadNotes ();
			}
		} else {
			loadNotes ();
		}
		
	}

	protected virtual void onSearchKey (object o, Gtk.KeyPressEventArgs args)
	{
		
		
		
	}
	
	
}
