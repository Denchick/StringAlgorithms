using System;
using System.Collections.Generic;
using System.Linq;

namespace Match2d
{
    public static class EnumerableExtensions
    {
        public static string Format2d(this List<Tuple<int, int>> matrix)
        {
            return string.Join("\n", matrix.Select(line => string.Join(" ", line)));
        }
    }
}
