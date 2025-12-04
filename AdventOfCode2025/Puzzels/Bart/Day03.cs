using System.Diagnostics.CodeAnalysis;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Bart;

[SuppressMessage("ReSharper", "EnforceIfStatementBraces")]
[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public class Day03 : HappyPuzzleBase<long>
{

	public override long SolvePart1(Input input)
	{
		long sum = 0;
		var bankLength = input.Lines[0].Length;
		scoped Span<int> batteryBank = stackalloc int[bankLength];

		for (var i = 0; i < input.Lines.Length; i++)
		{
			batteryBank[0] = input.Lines[i][0] - '0';
			var firstLargest = batteryBank[0];
			var indexOfLargest = 0;

			for (var j = 1; j < bankLength-1; j++)
			{
				batteryBank[j] = input.Lines[i][j] - '0';

				if (batteryBank[j] > firstLargest)
				{
					firstLargest = batteryBank[j];
					indexOfLargest = j;
				}
			}
			batteryBank[bankLength-1] = input.Lines[i][bankLength-1] - '0';

			//Console.WriteLine(firstLargest + "  index: " + indexOfLargest);
			var secondLargestArray = batteryBank[(indexOfLargest+1)..];
			secondLargestArray.Sort();
			var secondLargest = secondLargestArray[^1];

			var joltage = firstLargest * 10 + secondLargest;
			sum += joltage;
		}

		return sum;
	}

	public override long SolvePart2(Input input)
	{
		long sum = 0;
		var bankLength = input.Lines[0].Length;
		scoped Span<int> batteryBank = stackalloc int[bankLength];

		for (var i = 0; i < input.Lines.Length; i++)
		{
			for (var j = 0; j < bankLength; j++)
			{
				batteryBank[j] = input.Lines[i][j] - '0';
			}

			var findInRangeStartIndex = 0;
			var findInRangeEndIndex = batteryBank.Length - 11;


			// Find largest number, in a set that each time one shorter, starting with the index
			long number = 0;
			for (var numberIndex = 0; numberIndex < 12; numberIndex++)
			{
				var slice = batteryBank.Slice(findInRangeStartIndex, findInRangeEndIndex - findInRangeStartIndex);

				var maxIndex = MaxIndex(slice);

				number += slice[maxIndex] * Pow10[12  - numberIndex-1];

				//DEBUG
				//Console.WriteLine($"findInRangeStartIndex: {findInRangeStartIndex}: in [{string.Join("", slice.ToArray().Select(x => x.ToString()))}] found max number {slice[maxIndex]} on index: {maxIndex} number so far: {number}" );

				findInRangeStartIndex += (maxIndex+1);
				findInRangeEndIndex++;
			}
			//Console.WriteLine(number);

			sum += number;
		}

		return sum;
	}

	private static int MaxIndex(Span<int> slice)
	{
		var largestNumber = slice[0];
		var largestIndex = 0;

		for (var i = 1; i < slice.Length; i++)
		{
			if (slice[i] > largestNumber)
			{
				largestNumber = slice[i];
				largestIndex = i;
			}
		}
		return largestIndex;
	}

	private static readonly long[] Pow10 =
	{
		1,
		10,
		100,
		1000,
		10000,
		100000,
		1000000,
		10000000,
		100000000,
		1000000000,
		10000000000,
		100000000000,
		1000000000000,
		10000000000000,
		100000000000000,
		1000000000000000,
		10000000000000000,
		100000000000000000,
		1000000000000000000
	};
}

