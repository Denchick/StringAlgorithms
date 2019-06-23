using System.Collections.Generic;
using System.Linq;

namespace DocsReport
{
	class DocumentOccurrences
	{
		public string DocumentKey { get; set; }
		public List<int> OccurrencePositions { get; set; }
    }

    class DocumentInfo
    {
        public string Name { get; set; }
        public int LeftBorder { get; set; }
        public int RightBorder { get; set; }

        public DocumentInfo(string name, int left, int right)
        {
            Name = name;
            LeftBorder = left;
            RightBorder = right;
        }
    }

    class DocumentIndex
	{
		private readonly SuffixTree<int> _tree;
	    private readonly List<DocumentInfo> _docsMapping;

        public DocumentIndex(IReadOnlyList<KeyValuePair<string, byte[]>> documents)
		{
		    var intCast = documents.Select(d => d.Value.Select(x => (int)x)).ToList();
			var inputAlphabet = Enumerable.Range(0, 256 + intCast.Count);
			var input = intCast.SelectMany((d, i) => d.Concat(new[] { 256 + i })).ToList();
			_tree = new SuffixTree<int>(input, inputAlphabet);
		    _docsMapping = BuildDocsMapping(documents);
        }

        public List<DocumentOccurrences> ReportOccurrences(byte[] pattern)
		{
		    var occurrences = _tree.FindAllOccurrences(pattern.Select(c => (int) c).ToArray());
		    return MapInsideDocuments(occurrences);
		}

	    private List<DocumentOccurrences> MapInsideDocuments(List<int> suffixTreeIndexes)
	    {
	        var result = new List<(string, int)>(); // document name, entry in the document
            foreach (var index in suffixTreeIndexes)
	        {
	            foreach (var mapping in _docsMapping)
	            {
	                if (mapping.LeftBorder <= index && index <= mapping.RightBorder)
	                    result.Add((mapping.Name, index - mapping.LeftBorder));
	            }
	        }

	        return result
	            .GroupBy(pair => pair.Item1) // by document name
	            .Select(group => new DocumentOccurrences { DocumentKey = group.Key, OccurrencePositions = group.Select(pair => pair.Item2).ToList() })
	            .ToList();
	    }

	    private List<DocumentInfo> BuildDocsMapping(IReadOnlyList<KeyValuePair<string, byte[]>> documents)
	    {
	        var result = new List<DocumentInfo>();
	        var index = 0;

	        foreach (var document in documents)
	        {
	            var map = new DocumentInfo(document.Key, index, index + document.Value.Length);
	            result.Add(map);
	            index += document.Value.Length + 1;
	        }

	        return result;
	    }
    }
}
