using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day05 : HappyPuzzleBase<long>
	{
		private struct Range
		{
			public long Start;
			public long End;
		}

		private ref struct List
		{
			public List(Span<Range> span)
			{
				_ranges = span;
			}
			public readonly Span<Range> Ranges => _ranges.Slice(0, Count);
			public int Count = 0;
			private readonly Span<Range> _ranges;

			public void Add(Range range)
			{
				_ranges[Count] = range;
				Count++;
			}

			public void Remove(int index)
			{
				_ranges[index] = _ranges[Count - 1];
				Count--;
			}

			public readonly Range this[int index]
			{
				get => _ranges[index];
				set => _ranges[index] = value;
			}
		}

		public override long SolvePart1(Input input)
		{
			var count = 0;
			Span<Range> span = stackalloc Range[input.Lines.Length];
			var ranges = new List(span);
			var i = 0;
			for (i = 0; i < input.Lines.Length; i++)
			{
				var line = input.Lines[i].AsSpan();
				if (line.IsEmpty)
				{
					break;
				}
				var middle = line.IndexOf('-');
				var range = new Range
				{
					Start = ParseLong(line.Slice(0, middle)),
					End = ParseLong(line.Slice(middle + 1))
				};
				AddRange(ref ranges, range);
			}
			for (i = i + 1; i < input.Lines.Length; i++)
			{
				var line = input.Lines[i].AsSpan();
				var current = ParseLong(line);
				if (IsInRange(ranges, current))
				{
					count++;
				}
			}
			return count;
		}

		public override long SolvePart2(Input input)
		{
			var count = 0L;
			Span<Range> span = stackalloc Range[input.Lines.Length];
			var ranges = new List(span);
			var i = 0;
			for (i = 0; i < input.Lines.Length; i++)
			{
				var line = input.Lines[i].AsSpan();
				if (line.IsEmpty)
				{
					break;
				}
				var middle = line.IndexOf('-');
				var range = new Range
				{
					Start = ParseLong(line.Slice(0, middle)),
					End = ParseLong(line.Slice(middle + 1))
				};
				AddRange(ref ranges, range);
			}

			foreach (var range in ranges.Ranges)
			{
				count += (range.End - range.Start) + 1L;
			}

			return count;
		}

		private static void AddRange(ref List ranges, Range range)
		{
			var previousIndex = ranges.Count;
			ranges.Add(range);

			for (var i = ranges.Count - 2; i >= 0; i--)
			{
				ref var currentRange = ref ranges.Ranges[i];
				// Added range is contained within current range
				if (currentRange.Start <= range.Start && currentRange.End >= range.End)
				{
					ranges.Remove(previousIndex);
					return;
				}

				// Current range is contained within added range
				// Range is then replaced
				if (currentRange.Start > range.Start && currentRange.End < range.End)
				{
					ranges.Remove(i);
					continue;
				}

				// Start is in current range but not End
				if (range.Start >= currentRange.Start && range.Start <= currentRange.End)
				{
					currentRange.End = range.End;
					range = currentRange;
					ranges.Remove(previousIndex);
					previousIndex = i;
				}
				// End is in current range but not Start
				else if (range.End >= currentRange.Start && range.End <= currentRange.End)
				{
					currentRange.Start = range.Start;
					range = currentRange;
					ranges.Remove(previousIndex);
					previousIndex = i;
				}
			}
		}

		private static bool IsInRange(List ranges, long number)
		{
			foreach (var range in ranges.Ranges)
			{
				if (number >= range.Start && number <= range.End)
				{
					return true;
				}
			}
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
