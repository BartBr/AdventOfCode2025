using System.Diagnostics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day02 : HappyPuzzleBase<long>
{
	public override long SolvePart1(Input input)
	{
		var inputSpan = input.Text.AsSpan();

		// Parse first number of range until '-'
		// Parse second number of range until ','
		// Determine integer length of begin and end
		// If both odd (and same length), just continue with next range
		// (Apply ceil or floor to, start and end respectively... assuming max 1 integer length diff)
		// If start is odd, then apply ceil to start to match end integer length
		// if end is odd, than apply floor to end to match start integer length
		// Determine factors for repeating patterns based on integer length
		// For each factor, determine startX and endX based on start/factor and end/factor (ceil and floor respectively)
		// For each x in range, determine repeating pattern id as factor * x
		// Use hashset-like structure to avoid double counting of repeating pattern ids (in case of multiple factors) for integer length
		// Sum all unique repeating pattern ids

		long sumOfInvalidIds = 0;
		long firstNumber = 0;
		long secondNumber = 0;

		for (var i = 0; i < inputSpan.Length; i++)
		{
			ref var target = ref firstNumber;

			for (; i < inputSpan.Length; i++)
			{
				var c = inputSpan[i];

				if (c is >= '0' and <= '9')
				{
					target = target * 10 + (c - '0');
				}
				else if (c == '-')
				{
					target = ref secondNumber;
				}
				else
				{
					break;
				}
			}

			Part1_DetermineInvalidIds(firstNumber, secondNumber, ref sumOfInvalidIds);


			firstNumber = 0;
			secondNumber = 0;
		}

		return sumOfInvalidIds;
	}

	private static void Part1_DetermineInvalidIds(long start, long end, ref long sumOfInvalidIds)
	{
		var integerLengthStart = DetermineIntegerLengthOfNumber(start);
		var integerLengthEnd = DetermineIntegerLengthOfNumber(end);

		// Safe-guarding assumption of max diff in length of 1
		Debug.Assert((integerLengthEnd - integerLengthStart) <= 1);

		// Only check whether start is even bc both lengths need to be equal in order to be eligable for skipping
		var startLengthOdd = integerLengthStart % 2 == 1;

		if (integerLengthStart == integerLengthEnd && startLengthOdd)
		{
			// Skip both start and end are odd and same length
			return;
		}

		var integerLength = integerLengthStart;

		var internalStart = start;
		if (startLengthOdd)
		{
			// If start length odd, cap to ceil
			internalStart = (long) Math.Pow(10, integerLengthStart);

			++integerLength;
		}

		// And thus... now we check whether the end is also odd or not
		var internalEnd = end;
		var endLengthOdd = integerLengthEnd % 2 == 1;
		if (endLengthOdd)
		{
			// If end length odd, cap to floor
			internalEnd = (long) Math.Pow(10, integerLengthStart);
		}

		Part2_CheckIfRepeatingForRangeInternal(internalStart, internalEnd, integerLength, ref sumOfInvalidIds);
	}

	// Parse first number of range until '-'
	// Parse second number of range until ','
	// Determine integer length of begin and end
	// If both differing lengths, split ranges in 2 so that we have 2 ranges where start and end are of equal length
	// For each range, determine factors for repeating patterns based on integer length
	// For each factor, determine startX and endX based on start/factor and end/factor (ceil and floor respectively)
	// For each x in range, determine repeating pattern id as factor * x
	// Use hashset-like structure to avoid double counting of repeating pattern ids (in case of multiple factors) for integer length
	// Sum all unique repeating pattern ids

	public override long SolvePart2(Input input)
	{
		var inputSpan = input.Text.AsSpan();

		long sumOfInvalidIds = 0;
		long firstNumber = 0;
		long secondNumber = 0;

		for (var i = 0; i < inputSpan.Length; i++)
		{
			ref var target = ref firstNumber;

			for (; i < inputSpan.Length; i++)
			{
				var c = inputSpan[i];

				if (c is >= '0' and <= '9')
				{
					target = target * 10 + (c - '0');
				}
				else if (c == '-')
				{
					target = ref secondNumber;
				}
				else
				{
					break;
				}
			}

			Part2_DetermineInvalidIdsNew(firstNumber, secondNumber, ref sumOfInvalidIds);

			firstNumber = 0;
			secondNumber = 0;
		}

		return sumOfInvalidIds;
	}

	private static void Part2_DetermineInvalidIdsNew(long start, long end, ref long sumOfInvalidIds)
	{
		var startIntegerLength = DetermineIntegerLengthOfNumber(start);
		var endIntegerLength = DetermineIntegerLengthOfNumber(end);

		Debug.WriteLine($"Solving for range [{start},{end}]");
		Debug.WriteLine($"Range length: {end - start + 1}");

		if (startIntegerLength != endIntegerLength)
		{
			var threshold = GetPowerOf10(startIntegerLength);
			Part2_CheckIfRepeatingForRangeInternal(start, threshold - 1, startIntegerLength, ref sumOfInvalidIds);
			Part2_CheckIfRepeatingForRangeInternal(threshold, end, endIntegerLength, ref sumOfInvalidIds);
		}
		else
		{
			Part2_CheckIfRepeatingForRangeInternal(start, end, startIntegerLength, ref sumOfInvalidIds);
		}
	}

	private static void Part2_CheckIfRepeatingForRangeInternal(long start, long end, int integerLength, ref long sumOfInvalidIds)
	{
		// Skip if integer length < 2 due to no possible repeating patterns
		if (integerLength < 2)
		{
			return;
		}

		// Define factors for repeating patterns based on integer length
		scoped ReadOnlySpan<long> factors = integerLength switch
		{
			2 => stackalloc long[] {11},
			3 => [111],
			4 => [0101],
			5 => [11111],
			6 => [001001, 010101],
			7 => [1111111],
			8 => [00010001, 01010101],
			9 => [001001001],
			10 => [0000100001, 0101010101],
			_ => throw new ArgumentOutOfRangeException(nameof(integerLength), integerLength, null)
		};

		// Optimization: If only 1 factor, no need for hashset-like structure
		if (factors.Length == 1)
		{
			var factorValue = factors[0];
			var startXValueForFactor = (int) Math.Ceiling((double) start / factorValue);
			var endXValueForFactor = (int) Math.Floor((double) end / factorValue);

			Debug.WriteLine($"-> Attempting for pattern: {factorValue} - > [{startXValueForFactor * factorValue}, {endXValueForFactor * factorValue}]");

			for (var xForRepeatingPatternId = startXValueForFactor; xForRepeatingPatternId <= endXValueForFactor; xForRepeatingPatternId++)
			{
				var repeatingPatternId = factorValue * xForRepeatingPatternId;
				sumOfInvalidIds += repeatingPatternId;

				Debug.WriteLine($" ----> {xForRepeatingPatternId}: {repeatingPatternId} => {sumOfInvalidIds}");
			}
		}
		else
		{
			scoped Span<long> set = stackalloc long[250]; // Hashset-like buffer
			var setSize = 0;

			foreach (var factorValue in factors)
			{
				var startXValueForFactor = (int) Math.Ceiling((double) start / factorValue);
				var endXValueForFactor = (int) Math.Floor((double) end / factorValue);

				Debug.WriteLine($"-> Attempting for pattern: {factorValue} - > [{startXValueForFactor * factorValue}, {endXValueForFactor * factorValue}]");

				for (var xForRepeatingPatternId = startXValueForFactor; xForRepeatingPatternId <= endXValueForFactor; xForRepeatingPatternId++)
				{
					var repeatingPatternId = factorValue * xForRepeatingPatternId;
					if (set.Slice(0, setSize).Contains(repeatingPatternId))
					{
						continue;
					}

					set[setSize++] = repeatingPatternId;
					sumOfInvalidIds += repeatingPatternId;

					Debug.WriteLine($"----> {xForRepeatingPatternId}: {repeatingPatternId} => {sumOfInvalidIds}");
				}
			}

			Debug.WriteLine($"Max set size at end of run: {setSize}");
		}
	}

	private static int DetermineIntegerLengthOfNumber(long number)
	{
		var length = 0;

		while (number != 0)
		{
			number /= 10;
			length++;
		}

		return length;
	}

	private static long GetPowerOf10(int exponent)
	{
		return exponent switch
		{
			1 => 10,
			2 => 100,
			3 => 1000,
			4 => 10000,
			5 => 100000,
			6 => 1000000,
			7 => 10000000,
			8 => 100000000,
			9 => 1000000000,
			10 => 10000000000,
			_ => throw new ArgumentOutOfRangeException(nameof(exponent), exponent, null)
		};
	}
}