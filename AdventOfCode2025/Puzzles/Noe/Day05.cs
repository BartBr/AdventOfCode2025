using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day05 : HappyPuzzleBase<long>
	{
		public override long SolvePart1(Input input)
		{
			var count = 0;
			Span<long> ranges = stackalloc long[input.Lines.Length * 2];
			var index = 0;
			var i = 0;
			for (i = 0; i < input.Lines.Length; i++)
			{
				var line = input.Lines[i].AsSpan();
				if (line.IsEmpty)
				{
					break;
				}
				var middle = line.IndexOf('-');
				var int1 = ParseLong(line.Slice(0, middle));
				var int2 = ParseLong(line.Slice(middle + 1));
				AddRange(ranges, int1, int2, ref index);
			}
			ranges = ranges.Slice(0, index);
			for (i = i + 1; i < input.Lines.Length; i++)
			{
				var line = input.Lines[i].AsSpan();
				var current = ParseLong(line);
				if (IsInRange(ranges, current, out _))
				{
					count++;
				}
			}
			return count;
		}

		public override long SolvePart2(Input input)
		{
			var count = 0L;
			Span<long> ranges = stackalloc long[input.Lines.Length * 2];
			var index = 0;
			for (var i = 0; i < input.Lines.Length; i++)
			{
				var line = input.Lines[i].AsSpan();
				if (line.IsEmpty)
				{
					break;
				}
				var middle = line.IndexOf('-');
				var int1 = ParseLong(line.Slice(0, middle));
				var int2 = ParseLong(line.Slice(middle + 1));
				AddRange(ranges, int1, int2, ref index);
			}
			ranges = ranges.Slice(0, index);

			for (var i = 0; i < ranges.Length; i++)
			{
				if (IsInRange(ranges.Slice(i % 2 == 0 ? i + 2 : i + 1), ranges[i], out var idx))
				{
					Console.WriteLine(ranges[i]);
				}
			}

			for (var i = 0; i < ranges.Length / 2; i++)
			{
				var low = ranges[i * 2];
				var up = ranges[i * 2 + 1];
				count += (up - low) + 1L;
			}

			return count;
		}

		// Add a check for existing ranges that fall into a new range
		private static void AddRange(Span<long> ranges, long rangeLow, long rangeUp, ref int size)
		{
			var lowRange = (rangeLow, rangeUp);
			var upRange = (rangeLow, rangeUp);
			var isLowInRange = IsInRange(ranges.Slice(0, size), rangeLow, out var lowRangeIndex);
			var isUpInRange = IsInRange(ranges.Slice(0, size), rangeUp, out var upRangeIndex);
			if (!isLowInRange && !isUpInRange)
			{
				ranges[size++] = rangeLow;
				ranges[size++] = rangeUp;
				return;
			}

			if (lowRangeIndex == upRangeIndex)
			{
				return;
			}

			var targetIndex = lowRangeIndex ?? upRangeIndex ?? size;
			if (isLowInRange)
			{
				lowRange = (ranges[lowRangeIndex.Value], ranges[lowRangeIndex.Value + 1]);
			}

			if (isUpInRange)
			{
				upRange = (ranges[upRangeIndex.Value], ranges[upRangeIndex.Value + 1]);
			}

			ranges[targetIndex] = Math.Min(lowRange.rangeLow, upRange.rangeLow);
			ranges[targetIndex + 1] = Math.Max(lowRange.rangeUp, upRange.rangeUp);
			if (isLowInRange && isUpInRange)
			{
				if (upRangeIndex < size - 2)
				{
					ranges[upRangeIndex.Value] = ranges[size - 2];
					ranges[upRangeIndex.Value + 1] = ranges[size - 1];
				}
				size -= 2;
			}
			ReduceRange(ranges, ranges[targetIndex], ranges[targetIndex + 1], ref size);
		}

		private static void ReduceRange(Span<long> ranges, long low, long up, ref int size)
		{
			
		}

		private static bool IsInRange(Span<long> ints, long number, out int? index)
		{
			for (var i = 0; i < ints.Length / 2; i++)
			{
				if (number >= ints[i * 2] && number <= ints[i * 2 + 1])
				{
					index = i * 2;
					return true;
				}
			}
			index = null;
			return false;
		}

		private static long ParseLong(ReadOnlySpan<char> input)
		{
			var number = 0L;
			for (var i = 0; i < input.Length; i++)
			{
				var lastDigit = input[i] - '0';
				number = number * 10L + lastDigit;
			}
			return number;
		}
	}
}
