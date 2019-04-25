using System;
using System.Collections.Generic;
using System.Linq;

namespace task_Zfunc
{
	class Program
	{
		static bool IsTestPassed<TChar>(IList<TChar> pattern, IList<TChar> text)
		{
			var occs = Matcher.CountPrefixMatches(pattern, text);
			var occs_expected = Matcher.NaiveCountPrefixMatches(pattern, text);
			return Enumerable.SequenceEqual(occs, occs_expected);
		}
		static void Main(string[] args)
		{
			var rand = new Random();
			var pattern = new byte[100];
			var text = new byte[2000];
			int i;
			for (i = 0; i < 1000; i++)
			{
				rand.NextBytes(pattern);
				rand.NextBytes(text);
				if (!IsTestPassed(pattern, text))
				{
					Console.WriteLine("TEST ERROR!");
					return;
				}
			}
			Console.WriteLine("All random tests passed successfully");
		}
	}
}
