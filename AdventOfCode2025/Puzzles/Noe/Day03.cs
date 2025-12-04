using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day03 : HappyPuzzleBase<long>
	{
		public override long SolvePart1(Input input)
		{
			var sum = 0L;
			foreach (var item in input.Lines)
			{
				var max = GetMax(item);
				sum += max;
			}
			return sum;
		}

		private static int GetMax(ReadOnlySpan<char> item)
		{
			var dozen = Max(item.Slice(0, item.Length - 1), out var index);
			return dozen * 10 + Max(item.Slice(index + 1), out _);
		}

		public override long SolvePart2(Input input)
		{
			var sum = 0L;
			foreach (var item in input.Lines)
			{
				var max = GetMax2(item);
				sum += max;
			}
			return sum;
		}

		private static long GetMax2(ReadOnlySpan<char> item)
		{
			const int SIZE = 12;
			var sum = 0L;
			for (var i = 0; i < SIZE; i++)
			{
				var remainingLength = item.Length - (SIZE - i - 1);
				var range = item.Slice(0, remainingLength);

				var current = Max(range, out var index);
				item = item.Slice(index + 1);

				sum = (sum * 10) + current;
			}
			return sum;
		}

		private static byte Max(ReadOnlySpan<char> line, out int index)
		{
			byte max = 0;
			index = 0;
			for (var i = 0; i < line.Length; i++)
			{
				var number = (byte) (line[i] - '0');

				if (number > max)
				{
					index = i;
					max = number;
				}

				if (max == 9)
				{
					return 9;
				}
			}

			return max;
		}
	}
}
