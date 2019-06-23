using System;
using System.Collections.Generic;
using System.Linq;

namespace DocsReport
{
	class SuffixTree<TChar> where TChar : IComparable<TChar>
	{
		public class Node
		{
			public SortedDictionary<TChar, Node> Next { get; set; }
			public SortedDictionary<TChar, Node> Plink { get; set; }
			public Node Par { get; set; }
			public int Pos { get; set; }
			public int Len { get; set; }
			public bool Mark { get; set; }
			public Node()
			{
				Next = new SortedDictionary<TChar, Node>();
				Plink = new SortedDictionary<TChar, Node>();
			}
		}

		public Node Root { get; }
		private Node fake;
	    private IReadOnlyList<TChar> input;

	    public SuffixTree(IReadOnlyList<TChar> input, IEnumerable<TChar> inputAlphabet)
		{
		    this.input = input;
			fake = new Node { Mark = false };
			Root = new Node { Par = fake, Pos = 0, Len = 1, Mark = true };
			foreach (var c in inputAlphabet)
				fake.Next[c] = fake.Plink[c] = Root;
			var last = Root;
			for (int k = input.Count - 1; k >= 0; k--)
				last = Extend(input[k], input, last);
		}

		private void Attach(Node child, Node parent, int edgeLen, TChar c)
		{
			parent.Next[c] = child;
			child.Len = edgeLen;
			child.Par = parent;
		}
		private Node Extend(TChar c, IReadOnlyList<TChar> input, Node last)
		{
			int i = input.Count;
			Node parse, split, splitChild;
			Node newNode = new Node { Mark = false };
			for (parse = last; !parse.Plink.TryGetValue(c, out split); parse = parse.Par)
				i -= parse.Len;
			if (split.Next.TryGetValue(input[i], out splitChild))
			{
				int splitEdgeStart = splitChild.Pos - splitChild.Len;
				newNode.Pos = splitEdgeStart;
				while (EqualityComparer<TChar>.Default.Equals(input[newNode.Pos], input[i]))
				{
					parse = parse.Next[input[i]];
					newNode.Pos += parse.Len;
					i += parse.Len;
				}
				parse.Plink[c] = newNode;
				Attach(newNode, split, newNode.Pos - splitEdgeStart, input[splitEdgeStart]);
				Attach(splitChild, newNode, splitChild.Pos - newNode.Pos, input[newNode.Pos]);
				split = newNode;
				newNode = new Node();
			}
			last.Plink[c] = newNode;
			Attach(newNode, split, input.Count - i, input[i]);
			newNode.Pos = input.Count;
			newNode.Mark = true;
			return newNode;
		}

	    public List<int> FindAllOccurrences(TChar[] pattern)
	    {
            // сначала убеждаемся, что такие строки есть.
            // пытаемся из корня прочитать pattern
	        var node = Root;
	        var viewed = 0;

            for (var i = 0; i < pattern.Length; i += node.Len) //идем по вершинам
	        {
                if (!node.Next.TryGetValue(pattern[i], out node))
	                return Enumerable.Empty<int>().ToList(); // не смогли прочитать, значит нет таких строк
	            viewed = 0;

	            for (int j = 0; j < node.Len && i + j < pattern.Length; j++)
	            {
	                viewed++;
	                if (!pattern[i + j].Equals(input[node.Pos - node.Len + j]))
	                    return Enumerable.Empty<int>().ToList();
	            }
            }
            // дочитали строку до конца - значит есть хотя бы одно вхождение. 
            // найдем все вхождения, дойдя до листов из вершины, в которой закончили читать. 

            return TraverseToLeafs(node, viewed) // так найдем индексы окончания вхождений
                .Select(a => a - pattern.Length) // вычтем длину паттерна - это и будет начало вхождения
                .ToList();
	    }

	    private List<int> TraverseToLeafs(Node startNode, int viewed)
	    {
	        var occurrences = new List<int>();
            var queue = new Queue<(Node, int)>();
            queue.Enqueue((startNode, startNode.Len - viewed));

	        while (queue.Count > 0)
	        {
                var (node, accumulator) = queue.Dequeue();
	            if (node.Mark)
	                occurrences.Add(node.Pos - accumulator);
	            foreach (var child in node.Next)
                    queue.Enqueue((child.Value, accumulator + child.Value.Len));
            }

	        return occurrences;
	    }
    }
}
