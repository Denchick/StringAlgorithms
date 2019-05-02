using System;
using System.Collections.Generic;
using System.Linq;

namespace task_Match2d
{
	class AhoCorasick<TChar> where TChar : IComparable<TChar>
	{
		public delegate void ReportAction(int endPosition, int id);
		public void ReportOccurrencesIds(IEnumerable<TChar> input, ReportAction report)
		{
			//вызвать report на каждое найденное вхождение
			throw new NotImplementedException();
		}
		public AhoCorasick(IEnumerable<IList<TChar>> strings, out List<int> stringIds)
		{
			//stringIds будет содержать id для строк strings, равным строкам равные id
			throw new NotImplementedException();
		}
	}
}
