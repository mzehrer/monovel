using System;
using System.Collections;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

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
				doc.Add (new Field ("content", getNoteContent (noteTitle), false, true, true, true));
				writer.AddDocument (doc);
			}
			
			writer.Optimize ();
			writer.Close ();
			
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
				Query query = parser.Parse (search + "*");
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

		public string getNoteContent (string title)
		{
			StreamReader streamReader = new StreamReader (noteStorePath + "/" + title + ".txt");
			string text = streamReader.ReadToEnd ();
			return text;
			
		}
		
	}
}

