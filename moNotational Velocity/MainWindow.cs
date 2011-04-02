using System;
using Gtk;
using GLib;
using moNotationalVelocity;
using System.Collections;

public partial class MainWindow : Gtk.Window
{

	Gtk.NodeStore store;

	private string notesDirPath;
	private NotesStore notesStore;

	private TextBuffer buf;
	private string currentNote;

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
		
		this.KeyPressEvent += new global::Gtk.KeyPressEventHandler (this.OnKeyPressEvent);
		
		noteslist.NodeSelection.Changed += new System.EventHandler (OnSelectionChanged);
		editor.Buffer.Changed += new System.EventHandler (onTextChange);
		searchbar.Changed += new global::System.EventHandler (onSearchBarChanged);
		searchbar.KeyPressEvent += new global::Gtk.KeyPressEventHandler (onSearchbarKeyEvent);
		
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

	void OnSelectionChanged (object o, System.EventArgs args)
	{
		try {
			Gtk.NodeSelection selection = (Gtk.NodeSelection)o;
			NoteNode node = (NoteNode)selection.SelectedNode;
			
			currentNote = node.Title;
			buf = editor.Buffer;
			buf.Text = notesStore.getNoteContent (node.Title);
			
		} catch (Exception e) {
			Console.WriteLine ("selection changed ERROR: " + e.Message);
		}
	}

	protected virtual void onTextChange (object o, System.EventArgs args)
	{
		if (currentNote != null) {
			
			Console.WriteLine ("Storing note " + currentNote);
			
			string text = buf.Text;
			notesStore.storeNoteContent (currentNote, text);
		}
	}

	protected virtual void OnKeyPressEvent (object o, Gtk.KeyPressEventArgs args)
	{
		
		string key = args.Event.Key.ToString ();
		Console.WriteLine ("global key: " + key);
		
		if (key.Equals ("Escape")) {
			searchbar.Text = "";
			searchbar.GrabFocus ();
		}
	}

	[ConnectBefore]
	protected virtual void onSearchbarKeyEvent (object o, Gtk.KeyPressEventArgs args)
	{
		
		string key = args.Event.Key.ToString ();
		Console.WriteLine ("searchbar key: " + key);
		
		if (key.Equals ("Return") && !notesStore.doesNoteExist(searchbar.Text.Trim ()) && searchbar.Text != null && searchbar.Text.Length > 0) {
			Note newNote = new Note (searchbar.Text.Trim (), "");
			store.AddNode (new NoteNode (newNote));
			currentNote = newNote.title;
			notesStore.createNote (currentNote);
			buf = editor.Buffer;
			buf.Text = "";
			editor.GrabFocus ();
		}
		
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	protected virtual void onSearchBarChanged (object sender, System.EventArgs e)
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
			}
		} else {
			loadNotes ();
		}
		
	}
	
	
}
