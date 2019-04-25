using System.Collections.Generic;

namespace LZW
{
    public class MyEqualityComparer : IEqualityComparer<List<byte>>
    {
        public bool Equals(List<byte> x, List<byte> y)
        {
            if (x.Count != y.Count)
            {
                return false;
            }
            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(List<byte> obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Count; i++)
            {
                unchecked
                {
                    result = result * 23 + obj[i];
                }
            }
            return result;
        }
    }
}
