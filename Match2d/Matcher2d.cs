using System;
using System.Collections.Generic;
using System.Linq;

namespace task_Match2d
{
	class Matcher2d
	{
		public static List<Tuple<int, int>> PatternMatches<TChar>(
				IList<IList<TChar>> pattern, IList<IList<TChar>> matrix) 
				where TChar : IComparable<TChar>
		{
			if (pattern[0].Count == 0 || pattern.Any(row => row.Count != pattern[0].Count))
				throw new ArgumentException();
			if (matrix[0].Count == 0 || matrix.Any(row => row.Count != matrix[0].Count))
				throw new ArgumentException();
			int p = pattern.Count, q = pattern[0].Count;
			int m = matrix.Count, n = matrix[0].Count;
			var result = new List<Tuple<int, int>>();

			//Используя Ахо-Корасик, реализуйте поиск двумерных паттернов за O((pq + mn)log s)

			return result;
		}

		public static List<Tuple<int, int>> NaivePatternMatches<TChar>(
				IList<IList<TChar>> pattern, IList<IList<TChar>> matrix) 
				where TChar : IComparable<TChar>
		{
			if (pattern[0].Count == 0 || pattern.Any(row => row.Count != pattern[0].Count))
				throw new ArgumentException();
			if (matrix[0].Count == 0 || matrix.Any(row => row.Count != matrix[0].Count))
				throw new ArgumentException();
			int p = pattern.Count, q = pattern[0].Count;
			int m = matrix.Count, n = matrix[0].Count;
			var result = new List<Tuple<int, int>>();
			for (int i = 0; i <= m - p; i++)
				for (int j = 0; j <= n - q; j++)
					if (pattern.Select((row, k) => Enumerable.SequenceEqual(
						matrix[i + k].Skip(j).Take(q), pattern[k])).All(b => b))
					{
						result.Add(Tuple.Create(i, j));
					}
			return result;
		}
	}
}
