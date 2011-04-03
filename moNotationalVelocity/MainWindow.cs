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
		
		noteslist.NodeSelection.Mode = SelectionMode.Single;
		noteslist.NodeStore = Store;
		noteslist.AppendColumn ("Title", new Gtk.CellRendererText (), "text", 0);
		noteslist.AppendColumn ("Date modified", new Gtk.CellRendererText (), "text", 1);
		
		editor.WrapMode = WrapMode.Word;
		
		this.notesDirPath = cfg.notesDirPath;
		
		Console.WriteLine ("Using notes at " + notesDirPath);
		
		if (!System.IO.File.Exists (notesDirPath))
			System.IO.Directory.CreateDirectory (notesDirPath);
		
		notesStore = new FilesystemNotesStore (notesDirPath);
		
		loadNotes ();
		
		this.KeyPressEvent += new global::Gtk.KeyPressEventHandler (this.OnKeyPressEvent);
		
		noteslist.NodeSelection.Changed += new System.EventHandler (OnSelectionChanged);
		noteslist.KeyPressEvent += new global::Gtk.KeyPressEventHandler (onListKeyEvent);
		noteslist.ButtonPressEvent += new ButtonPressEventHandler (onListButtonPress);
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
		
		store.Clear ();
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
			
			loadNoteToBuffer (node);
			
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
			GoBackToSearch ();
		}
	}

	[ConnectBefore]
	protected virtual void onSearchbarKeyEvent (object o, Gtk.KeyPressEventArgs args)
	{
		
		string key = args.Event.Key.ToString ();
		Console.WriteLine ("searchbar key: " + key);
		
		if (key.Equals ("Return")) {
			
			if (!notesStore.doesNoteExist (searchbar.Text.Trim ()) && searchbar.Text != null && searchbar.Text.Length > 0) {
				Note newNote = new Note (searchbar.Text.Trim (), "");
				store.AddNode (new NoteNode (newNote));
				currentNote = newNote.title;
				notesStore.createNote (currentNote);
				buf = editor.Buffer;
				buf.Text = "";
				editor.GrabFocus ();
			}
			
		} else if (key.Equals ("Down")) {
			noteslist.GrabFocus ();
			
			NoteNode node = noteslist.NodeSelection.SelectedNode as NoteNode;
			
			loadNoteToBuffer (node);
		}
		
	}

	[ConnectBefore]
	protected virtual void onListKeyEvent (object o, Gtk.KeyPressEventArgs args)
	{
		string key = args.Event.Key.ToString ();
		Console.WriteLine ("noteslist key: " + key);
		
		NoteNode node = noteslist.NodeSelection.SelectedNode as NoteNode;
		
		if (key.Equals ("Return")) {
			editor.GrabFocus ();
		} else if (key.Equals ("d") || key.Equals ("Delete")) {
			if (node != null) {
				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.YesNo, "Are you sure you want to delete the note " + node.Title + "" + "?");
				ResponseType result = (ResponseType)md.Run ();
				
				if (result == ResponseType.Yes) {
					noteslist.NodeStore.RemoveNode (node);
					notesStore.deleteNote (node.Title);
					GoBackToSearch ();
				}
				md.Destroy ();
				
			}
		} else if (key.Equals ("r")) {
			if (node != null) {
				RenameNote renameDialog = new RenameNote ();
				renameDialog.NoteTitle = node.Title;
				renameDialog.init ();
				ResponseType result = (ResponseType)renameDialog.Run ();
				
				if (result == ResponseType.Ok) {
					string newTitle = renameDialog.NoteTitle.Trim();
					Console.WriteLine ("Response: " + newTitle);
					
					if (newTitle != null && newTitle.Length > 0 && !newTitle.Equals (node.Title)) {
						notesStore.RenameNote (node.Title, newTitle);
						node.Title = newTitle;
					}
				}
				renameDialog.Destroy ();
			}
		}
		
	}

	private void GoBackToSearch ()
	{
		searchbar.Text = "";
		searchbar.GrabFocus ();
		loadNotes ();
		currentNote = null;
		buf.Text = "";
	}

	private void loadNoteToBuffer (NoteNode node)
	{
		if (node != null && !searchbar.HasFocus) {
			currentNote = node.Title;
			searchbar.Text = node.Title;
			loadNoteToBuffer (node.Title);
		}
	}

	private void loadNoteToBuffer (String title)
	{
		if (title != null) {
			buf = editor.Buffer;
			buf.Text = notesStore.getNoteContent (title);
		}
	}

	[GLib.ConnectBeforeAttribute]
	protected void onListButtonPress (object sender, ButtonPressEventArgs e)
	{
		uint button = e.Event.Button;
		Console.WriteLine ("noteslist button: " + button);
		
		noteslist.GrabFocus ();
		
		
		NoteNode node = noteslist.NodeSelection.SelectedNode as NoteNode;
		
		loadNoteToBuffer (node);
		
		
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected virtual void onSearchBarChanged (object sender, System.EventArgs e)
	{
		string text = searchbar.Text.Trim ();
		if (text == null || text.Length < 1) {
			loadNotes ();
		} else if (searchbar.HasFocus) {
			store.Clear ();
			string search = searchbar.Text.Trim ();
			if (search != null && search.Length > 0) {
				ArrayList notes = notesStore.getNotesMatchingTitle (search);
				if (notes.Count > 0) {
					for (int i = 0; i < notes.Count; i++) {
						Note noteEntry = notes[i] as Note;
						NoteNode node = new NoteNode (noteEntry);
						store.AddNode (node);
						if (i == 0)
							noteslist.NodeSelection.SelectNode (node);
					}
					
				}
			} else {
				loadNotes ();
			}
		}
	}
	
	
}
