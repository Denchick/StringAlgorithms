using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DocsReport
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("почему же у меня не выходит нихуя?", "абсолютно нихуя.", "")]
        [TestCase("abcabb0", "ca", "2")]
        [TestCase("c", "c", "0")]
        [TestCase("abcd", "abcd", "0")]
        [TestCase("abcd", "abce", "")]
        [TestCase("abc", "c", "2")]
        [TestCase("abc", "b", "1")]
        [TestCase("abc", "a", "0")]
        [TestCase("abc", "ab", "0")]
        [TestCase("abc", "bc", "1")]
        [TestCase("abc", "abc", "0")]
        [TestCase("abcd", "abc", "0")]
        [TestCase("abba", "bb", "1")]
        [TestCase("mississippi", "ississippi", "1")]
        [TestCase("even", "e", "0,2")]
        [TestCase("abababababab", "ab", "0,2,4,6,8,10")]
        public void SuffixTreeTests(string text, string pattern, string expected)
        {
            var alphabet = new HashSet<char>((text + pattern).ToCharArray());
            var expectedList = expected.Length > 0 
                ? expected.Split(',').Select(int.Parse).ToArray()
                : new int[]{};

            var tree = new SuffixTree<char>(text.ToCharArray(), alphabet);
            var result = tree.FindAllOccurrences(pattern.ToCharArray());

            CollectionAssert.AreEquivalent(expectedList, result);
        }
    }
}
