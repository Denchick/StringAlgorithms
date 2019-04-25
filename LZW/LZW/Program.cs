using System;
using System.Linq;
using System.IO;

namespace task_LZW
{
	class Program
	{
		static void Main(string[] args)
		{
			string prefix = @"..\..\data-files\";
			foreach (var filename in new []{ "program.cs.txt", "lotr.txt", "img.bmp" })
			{
				var data = File.ReadAllBytes(prefix + filename);
				var compressed = LZW.Compress(data);
				File.WriteAllBytes(prefix + filename + ".compessed", compressed);
				Console.WriteLine($"'{filename}' {data.Length:n0} B to {compressed.Length:n0} B");
				if (!LZW.Decompress(compressed).SequenceEqual(data))
					Console.WriteLine("Decompression error");
			}
		}
	}
}
