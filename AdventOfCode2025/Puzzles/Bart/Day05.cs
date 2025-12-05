using System.Diagnostics.CodeAnalysis;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Bart;

[SuppressMessage("ReSharper", "EnforceIfStatementBraces")]
[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public class Day05 : HappyPuzzleBase<long>
{

	public override long SolvePart1(Input input)
	{
		long sum = 0;

		scoped Span<Range> ranges = stackalloc Range[input.Lines.Length];

		var rowIndex = 0;
		while (input.Lines[rowIndex] != "")
		{
			var index = 0;
			var numberOne = ReadNumber(input.Lines[rowIndex],  ref index);
			index++;
			var numberTwo = ReadNumber(input.Lines[rowIndex],  ref index);
			ranges[rowIndex] = new Range(numberOne, numberTwo);
			rowIndex++;
		}

		ranges = ranges.Slice(0, rowIndex);

		for (var i = rowIndex + 1; i < input.Lines.Length; i++)
		{
			var number = ReadNumber(input.Lines[i]);
			if(InAnyRange(number, ref ranges)) sum ++;
		}

		return sum;
	}

	private static bool InAnyRange(long number, ref Span<Range> ranges)
	{
		for (var r = 0; r < ranges.Length; r++)
		{
			if (ranges[r].IsValueInRange(number)) return true;
		}
		return false;
	}

	private static long ReadNumber(string inputLine, ref int index)
	{
		long number = inputLine[index++] - '0';

		while (index < inputLine.Length && inputLine[index] >= '0' && inputLine[index] <= '9')
		{
			var lastDigit = inputLine[index++] - '0';
			number = number * 10 + lastDigit;
		}

		return number;
	}

	private static long ReadNumber(string inputLine)
	{
		long number = 0;

		for(var i = 0; i < inputLine.Length; i++)
		{
			var lastDigit = inputLine[i] - '0';
			number = number * 10 + lastDigit;
		}

		return number;
	}

	public override long SolvePart2(Input input)
	{
		long sum = 0;

		// Step 1 read all ranges
		scoped Span<Range> ranges = stackalloc Range[input.Lines.Length];
		var row = 0;

		while (input.Lines[row] != "")
		{
			var index = 0;
			var numberOne = ReadNumber(input.Lines[row],  ref index);
			index++;
			var numberTwo = ReadNumber(input.Lines[row],  ref index);
			ranges[row] = new Range(numberOne, numberTwo);
			row++;
		}
		ranges = ranges.Slice(0, row);

		// Step 2 merge ranges, loops until no more are merged
		while (TryMergeRanges(ref ranges))
		{

		}

		// Step 3 sum all range counts
		for (var i = 0; i < ranges.Length; i++)
		{
			sum += (ranges[i].End - ranges[i].Start + 1);
		}

		return sum;
	}

	private static bool TryMergeRanges(ref Span<Range> ranges)
	{
		if(ranges.Length == 1) return false;

		var atLeastOneMerged = false;

		var i = 0;
		while (i < ranges.Length)
		{
			var j = i + 1;
			while (j < ranges.Length)
			{
				if (ranges[i].DoesOverlap(ranges[j]))
				{
					atLeastOneMerged = true;
					var merged =Range.Merge(ref ranges[i], ref ranges[j]);
					ranges[i] = merged;

					ranges[j] = ranges[^1]; //Move last range to the removed one.
					ranges = ranges.Slice(0, ranges.Length - 1);
				}

				j++;
			}
			i++;
		}

		return atLeastOneMerged;
	}

}


public readonly struct Range
{
	public readonly long Start;
	public readonly long End;

	public Range(long start, long end)
	{
		Start = start;
		End = end;
	}

	public bool IsValueInRange(long value)
	{
		return value >= Start && value <= End;
	}

	public bool DoesOverlap(Range other)
	{
		return !(End < other.Start || Start > other.End);
	}

	public static Range Merge(ref Range a, ref Range b)
	{
		var start = a.Start < b.Start ? a.Start : b.Start;
		var end = a.End > b.End ? a.End : b.End;
		return new Range(start, end);
	}
}
