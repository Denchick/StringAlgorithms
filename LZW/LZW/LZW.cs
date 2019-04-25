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
		    var result = new List<int>();
            var codingTable = GetInitialEncodingTable(Dimension);
		    var codingIndex = Dimension;
            var pool = new List<byte>(); // сюда читаем новые символы, которых еще нет в словаре
		    for (int i = 0; i < data.Length; i++)
		    {
		        pool.Add(data[i]);
		        var poolArray = pool.ToArray();
                if (codingTable.ContainsKey(poolArray) && i != data.Length - 1)
                    continue;
		        
		        codingTable[poolArray] = codingIndex++;
		        pool = new List<byte> {data[i]};
                result.Add(codingTable[poolArray]);
		    }

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
                    codingTable[codingIndex++] = 
                        AddElelementToArray(codingTable[previousCode], entry[0]);
                    previousCode = currentCode;
                }

                else
                {
                    var kek = codingTable[previousCode];
                    var entry = AddElelementToArray(kek, kek[0]);
                    codingTable[codingIndex++] = entry;
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

	    private static Dictionary<byte[], int> GetInitialEncodingTable(int dimension)
	    {
	        var table = new Dictionary<byte[], int>(new MyEqualityComparer());
	        for (int i = 0; i < dimension; i++)
	            table[new[] { (byte)i }] = i;
	        return table;
	    }

	    private static Dictionary<int, byte[]> GetInitialDecodingTable(int dimension)
	    {
	        var table = new Dictionary<int, byte[]>();
	        for (int i = 0; i < dimension; i++)
	            table[i] = new[] { (byte)i };
	        return table;
	    }

	    private static byte[] AddElelementToArray(byte[] array, byte element)
	    {
	        var result = array.ToList();
            result.Add(element);
	        return result.ToArray();
	    }
    }
}
