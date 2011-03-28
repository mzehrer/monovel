using System;
using System.Collections;

namespace moNotationalVelocity
{
	public interface NotesStore
	{
		
		ArrayList getAllNotes();
		ArrayList getNotesMatchingTitle(string titlesearch);
		string getNoteContent (string title);
		
	}
}

