﻿using System;
using System.Collections.Generic;

namespace Match2d
{
    public class Node<TChar>
    {
        public Node<TChar> Parent;
        public TChar ParentChar;
        public Node<TChar> Link;
        public bool Mark;
        public int StringId = -1;
        public Dictionary<TChar, Node<TChar>> Children = new Dictionary<TChar, Node<TChar>>();
        public Dictionary<TChar, Node<TChar>> Transitions = new Dictionary<TChar, Node<TChar>>();

        public Node(Node<TChar> parent, TChar parentChar)
        {
            Parent = parent;
            ParentChar = parentChar;
        }

        public Node<TChar> GetNext(TChar label)
        {
            if (!Transitions.ContainsKey(label))
            {
                if (Children.TryGetValue(label, out var nextNode))
                    Transitions[label] = nextNode;
                else if (Parent == null)
                    Transitions[label] = this;
                else
                    Transitions[label] = GetSuffixLink().GetNext(label);
            }

            return Transitions[label];
        }

        public Node<TChar> GetSuffixLink()
        {
            if (Link != null)
                return Link;

            if (Parent == null)
                Link = this;
            else if (Parent.Parent == null)
                Link = Parent;
            else Link = Parent.GetSuffixLink().GetNext(ParentChar);

            return Link;
        }
    }

    public class AhoCorasick<TChar> where TChar : IComparable<TChar>
    {
        public readonly Node<TChar> Root;
        public readonly List<int> Pattern = new List<int>();
        public delegate void ReportAction(int endPosition, int id);

        public void ReportOccurrencesIds(IEnumerable<TChar> input, ReportAction report)
        {
            var id = 0;
            var node = Root;
            foreach (var c in input)
            {
                node = node.GetNext(c);
                if (node.Mark)
                    report(id, node.StringId);
                id++;
            }
        }

        public AhoCorasick(IEnumerable<IList<TChar>> strings)
        {
            Root = new Node<TChar>(null, default(TChar));
            var stringId = 1;
            foreach (var pattern in strings)
                Add(pattern, ref stringId);
        }

        private void Add(IEnumerable<TChar> pattern, ref int stringId)
        {
            var node = Root;
            foreach (var c in pattern)
            {
                if (node.Children.TryGetValue(c, out var nextNode))
                    node = nextNode;
                else
                {
                    nextNode = new Node<TChar>(node, c);
                    node.Children.Add(c, nextNode);
                    node = nextNode;
                }
            }

            node.Mark = true;
            if (node.StringId == -1)
            {
                node.StringId = stringId;
                Pattern.Add(stringId++);
                return;
            }

            Pattern.Add(node.StringId);
        }
    }
}
