using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day05 : HappyPuzzleBase<int, long>
{
	public override int SolvePart1(Input input)
	{
		scoped Span<FreshFoodRange> ranges = stackalloc FreshFoodRange[250];
		var freshFoodRangesSize = 0;

		var i = 0;
		for (; ; i++)
		{
			var lineSpan = input.Lines[i].AsSpan();
			if (lineSpan.IsEmpty)
			{
				break;
			}

			long firstNumber = 0;
			long secondNumber = 0;

			for (var j = 0; j < lineSpan.Length; j++)
			{
				ref var target = ref firstNumber;

				for (; j < lineSpan.Length; j++)
				{
					var c = lineSpan[j];

					if (c is >= '0' and <= '9')
					{
						target = target * 10 + (c - '0');
					}
					else if (c == '-')
					{
						target = ref secondNumber;
					}
				}
			}

			ranges[freshFoodRangesSize++] = new FreshFoodRange(firstNumber, secondNumber);
		}

		ranges = ranges.Slice(0, freshFoodRangesSize);
		MergeIntervals(ref ranges);

		var count = 0;
		for (; i < input.Lines.Length; i++)
		{
			var lineSpan = input.Lines[i].AsSpan();

			var number = 0L;
			for (var j = 0; j < lineSpan.Length; j++)
			{
				number = number * 10 + (lineSpan[j] - '0');
			}

			foreach (var range in ranges)
			{
				if (range.Contains(number))
				{
					count++;
					break;
				}
			}
		}

		return count;
	}

	public override long SolvePart2(Input input)
	{
		scoped Span<FreshFoodRange> ranges = stackalloc FreshFoodRange[250];
		var freshFoodRangesSize = 0;

		var i = 0;
		for (; ; i++)
		{
			var lineSpan = input.Lines[i].AsSpan();
			if (lineSpan.IsEmpty)
			{
				break;
			}

			long firstNumber = 0;
			long secondNumber = 0;

			for (var j = 0; j < lineSpan.Length; j++)
			{
				ref var target = ref firstNumber;

				for (; j < lineSpan.Length; j++)
				{
					var c = lineSpan[j];

					if (c is >= '0' and <= '9')
					{
						target = target * 10 + (c - '0');
					}
					else if (c == '-')
					{
						target = ref secondNumber;
					}
				}
			}

			ranges[freshFoodRangesSize++] = new FreshFoodRange(firstNumber, secondNumber);
		}

		ranges = ranges.Slice(0, freshFoodRangesSize);
		MergeIntervals(ref ranges);

		var count = 0L;
		foreach (var range in ranges)
		{
			count += range.Length;
		}

		return count;
	}

	private static void MergeIntervals(ref Span<FreshFoodRange> ranges)
	{
		ranges.Sort(FreshFoodRangeComparison);

		var mergedRangesSize = 1;

		for (var i = 0; i < ranges.Length; i++)
		{
			ref var last = ref ranges[mergedRangesSize - 1];
			var current = ranges[i];

			if (current.Start <= last.End)
			{
				var mergedRange = new FreshFoodRange(last.Start, Math.Max(last.End, current.End));
				last = mergedRange;
			}
			else
			{
				ranges[mergedRangesSize++] = current;
			}
		}

		ranges = ranges.Slice(0, mergedRangesSize);
	}

	private static readonly Comparison<FreshFoodRange> FreshFoodRangeComparison = (rangeA, rangeB) => rangeA.Start != rangeB.Start
		? rangeA.Start.CompareTo(rangeB.Start)
		: rangeA.End.CompareTo(rangeB.End);

	private readonly struct FreshFoodRange
	{
		public readonly long Start;
		public readonly long End;

		public FreshFoodRange(long start, long end)
		{
			Start = start;
			End = end;
		}

		public bool Contains(long value)
		{
			return value >= Start && value <= End;
		}

		public long Length => End - Start + 1;
	}
}

