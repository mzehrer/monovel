using System;
using System.Collections;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System.Runtime.CompilerServices;

namespace moNotationalVelocity
{
	public class FilesystemNotesStore : NotesStore
	{

		ArrayList notes = new ArrayList ();
		string noteStorePath;
		Lucene.Net.Store.Directory lucIdx;
		Analyzer analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer ();

		public FilesystemNotesStore (string path)
		{
			this.noteStorePath = path;
			ReLoadNotes();
			
		}

		public ArrayList getAllNotes ()
		{
			return notes;
		}

		public ArrayList getNotesMatchingTitle (string search)
		{
			
			ArrayList snotes = new ArrayList ();
			
			try {
				QueryParser parser = new QueryParser ("title", analyzer);
				
				string lucsearch = search + "*^4" + " content:" + search + "*";
				
				Query query = parser.Parse (lucsearch);
				IndexSearcher searcher = new IndexSearcher (lucIdx);
				Hits hits = searcher.Search (query);
				
				int results = hits.Length ();
				Console.WriteLine ("Found {0} results", results);
				for (int i = 0; i < results; i++) {
					Document doc = hits.Doc (i);
					//float score = hits.Score (i);
					snotes.Add (new Note (doc.Get ("title"), doc.Get ("lastmod")));
				}
			} catch (Exception e) {
				Console.WriteLine ("ERROR Search: " + e.Message);
			}
			
			return snotes;
		}

		public bool doesNoteExist (string title)
		{
			if (new FileInfo (noteStorePath + "/" + title + ".txt").Exists)
				return true;
			
			return false;
			
		}

		public void createNote (string title)
		{
			TextWriter textWriter = new StreamWriter (noteStorePath + "/" + title + ".txt");
			textWriter.Write ("");
			textWriter.Close ();
			ReLoadNotes();
		}

		public string getNoteContent (string title)
		{
			StreamReader streamReader = new StreamReader (noteStorePath + "/" + title + ".txt");
			string text = streamReader.ReadToEnd ();
			streamReader.Close ();
			return text;
			
		}

        public void deleteNote (string title)
        {
            FileInfo noteFile = new FileInfo (noteStorePath + "/" + title + ".txt");
			if (noteFile.Exists)
                noteFile.Delete();
			
			ReLoadNotes();
        }

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void storeNoteContent (string title, string content)
		{
			TextWriter textWriter = new StreamWriter (noteStorePath + "/" + title + ".txt");
			textWriter.Write (content);
			textWriter.Close ();
		}
		

		private void  ReLoadNotes()
		{
			
			lucIdx = new Lucene.Net.Store.RAMDirectory ();
			IndexWriter writer = new IndexWriter (lucIdx, analyzer, true);
			
			DirectoryInfo di = new DirectoryInfo (noteStorePath);
			FileInfo[] rgFiles = di.GetFiles ("*.txt");
			foreach (FileInfo fi in rgFiles) {
				
				string noteTitle = Path.GetFileNameWithoutExtension (fi.FullName);
				string noteLastMod = fi.LastAccessTime.ToShortDateString ();
				
				notes.Add (new Note (noteTitle, noteLastMod));
				Document doc = new Document ();
				doc.Add (new Field ("title", noteTitle, true, true, true));
				doc.Add (new Field ("lastmod", noteLastMod, true, true, true));
				string content = getNoteContent (noteTitle);
				if (content != null && content.Length > 0)
					doc.Add (new Field ("content", content, false, true, true, true));
				writer.AddDocument (doc);
			}
			
			writer.Optimize ();
			writer.Close ();
		}
}
}

