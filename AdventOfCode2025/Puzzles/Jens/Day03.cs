using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day03 : HappyPuzzleBase<int, long>
{
	public override int SolvePart1(Input input)
	{
		var totalJoltages = 0;

		foreach (var line in input.Lines)
		{
			var lineSpan = line.AsSpan();

			var mostSignificantDigit = lineSpan[0];
			var leastSignificantDigit = '0'; // '0' is safe value because it's both lower than the ascii value of the other digits (and doesn't occur in the puzzle input)

			for (var i = 1; i < lineSpan.Length - 1; i++)
			{
				var digit = lineSpan[i];
				if (mostSignificantDigit < digit)
				{
					mostSignificantDigit = digit;
					leastSignificantDigit = '0';
				}
				else if (leastSignificantDigit < digit)
				{
					leastSignificantDigit = digit;
				}
			}

			// Loop optimization
			// Check if last digit is still larger than the least significant digit, if so assign
			// Not necessary to check again mostSignificantDigit
			if (leastSignificantDigit < lineSpan[^1])
			{
				leastSignificantDigit = lineSpan[^1];
			}

			var joltage = (mostSignificantDigit - '0') * 10 + (leastSignificantDigit - '0');
			totalJoltages += joltage;
		}

		return totalJoltages;
	}

	public override long SolvePart2(Input input)
	{
		var totalJoltages = 0L;

		foreach (var line in input.Lines)
		{
			scoped var lineSpan = line.AsSpan();

			totalJoltages += FindLargestNumberInLine(lineSpan);
		}

		return totalJoltages;
	}

	private static long FindLargestNumberInLine(ReadOnlySpan<char> lineSpan)
	{
		scoped Span<char> digitBuffer = stackalloc char[12];
		lineSpan.Slice(0, 12).CopyTo(digitBuffer);

		for (var i = 12; i < lineSpan.Length; i++)
		{
			var digitChar = lineSpan[i];
			ref var targetChar = ref digitBuffer[11];

			if (Compress(ref digitBuffer) || targetChar < digitChar)
			{
				targetChar = digitChar;
			}
		}

		long number = 0;
		foreach (var digitChar in digitBuffer)
		{
			number *= 10;
			number += digitChar - '0';
		}

		return number;
	}

	private static bool Compress(ref Span<char> digitBuffer)
	{

		for (var i = 0; i < digitBuffer.Length - 1; i++)
		{
			ref var targetChar = ref digitBuffer[i];
			var sourceChar = digitBuffer[i + 1];

			if (sourceChar <= targetChar)
			{
				continue;
			}

			targetChar = sourceChar;
			UnconditionallyOffsetAfterRemoval(ref digitBuffer, i + 1);
			return true;
		}

		return false;
	}

	private static void UnconditionallyOffsetAfterRemoval(ref Span<char> digitBuffer, int startIndex)
	{
		for (var i = startIndex; i < digitBuffer.Length - 1; i++)
		{
			digitBuffer[i] = digitBuffer[i + 1];
		}
	}
}
