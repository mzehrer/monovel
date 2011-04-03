using System;
using System.Collections;

namespace moNotationalVelocity
{
	public interface NotesStore
	{
		
		ArrayList getAllNotes();
		ArrayList getNotesMatchingTitle(string titlesearch);
		string getNoteContent (string title);
		void storeNoteContent (string title, string content);
		void createNote(string title);
		bool doesNoteExist(string title); 
		void deleteNote (string title);
		
	}
}

