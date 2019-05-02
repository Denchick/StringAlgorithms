using System;
using System.Collections.Generic;
using System.Linq;
using LZW;

namespace task_LZW
{
	class LZW
	{
	    private const int Dimension = 256;

		private static int[] DataToLZW(byte[] data)
		{
            if (data.Length == 0)
                return new int[] {};

		    var result = new List<int>();
            var table = GetInitialEncodingTable(Dimension);
		    var index = Dimension;
            var pool = new List<byte> {data[0]}; // сюда читаем новые символы, которых еще нет в словаре
		    foreach (var nextChar in data.Skip(1))
		    {
		        var poolNextChar = new List<byte>(pool) { nextChar };
		        if (table.ContainsKey(poolNextChar))
		            pool = new List<byte>(poolNextChar);
		        else
		        {
		            result.Add(table[pool]);
		            table[poolNextChar] = index++;
		            pool = new List<byte> {nextChar};
		        }
                    
		        //result.Add(table[pool]);
		    }

            if (pool.Count > 0)
                result.Add(table[pool]);

		    return result.ToArray();
		}
		
		private static byte[] LZWtoData(int[] lzw)
		{
		    var codingTable = GetInitialDecodingTable(Dimension);
		    var codingIndex = Dimension;
		    var previousCode = lzw[0];
		    var result = codingTable[previousCode].ToList();
            for (int i = 1; i < lzw.Length; i++)
            {
                // k == currentCode, w = previousCode
                var currentCode = lzw[i];
                if (codingTable.ContainsKey(currentCode))
                {
                    var entry = codingTable[currentCode];
                    codingTable[codingIndex++] = new List<byte>(codingTable[previousCode]) {entry[0]};
                    previousCode = currentCode;
                }

                else
                {
                    var kek = codingTable[previousCode];
                    codingTable[codingIndex++] = new List<byte>(kek) { kek[0] }; ;
                    previousCode = codingIndex;
                }
            }


		    return result.ToArray();
		}
		
		public static byte[] Compress(byte[] data)
		{
			var lzw = DataToLZW(data);
			var output = new List<byte>();
			int id_bits = 8, curr_bit = 0;
			for (int id = 0; id < lzw.Length; id++)
			{
				int new_size = (curr_bit + id_bits + 7) / 8 + 8;
				output.AddRange(new byte[new_size - output.Count]);
				var bytes = BitConverter.GetBytes((ulong)lzw[id] << curr_bit % 8);
				for (int k = 0; k < 2 + id_bits/8; k++)
					output[curr_bit/8 + k] |= bytes[k];
				curr_bit += id_bits;
				if (256 + id >= (1 << id_bits))
					id_bits++;
			}
			output.RemoveRange((curr_bit + 7) / 8, 8);
			return output.ToArray();
		}
		
		public static byte[] Decompress(byte[] data)
		{
			var lzw = new List<int>();
			int id_bits = 8, curr_bit = 0, last_bit = data.Length * 8;
			data = data.Concat(new byte[8]).ToArray();
			for (int id = 0; curr_bit + id_bits <= last_bit; id++)
			{
				ulong x = BitConverter.ToUInt64(data, curr_bit / 8);
				lzw.Add((int)(x >> curr_bit % 8) & (1 << id_bits) - 1);
				curr_bit += id_bits;
				if (256 + id >= (1 << id_bits))
					id_bits++;
			}
			return LZWtoData(lzw.ToArray());
		}

	    private static Dictionary<List<byte>, int> GetInitialEncodingTable(int dimension)
	    {
	        var table = new Dictionary<List<byte>, int>(new MyEqualityComparer());
	        for (int i = 0; i < dimension; i++)
	            table[new List<byte> { (byte)i }] = i;
	        return table;
	    }

	    private static Dictionary<int, List<byte>> GetInitialDecodingTable(int dimension)
	    {
	        var table = new Dictionary<int, List<byte>>();
	        for (int i = 0; i < dimension; i++)
	            table[i] = new List<byte> { (byte)i };
	        return table;
	    }
    }
}
