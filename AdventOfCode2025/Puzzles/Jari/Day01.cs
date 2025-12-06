using System.Diagnostics.CodeAnalysis;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jari;

[SuppressMessage("ReSharper", "EnforceIfStatementBraces")]
[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public class Day01 : HappyPuzzleBase<int>
{
	public override int SolvePart1(Input input)
	{
		var zeroEncounters = 0;
		var dialPosition = 50;

		for (var i = 0; i < input.Lines.Length; i++)
		{
			var line = input.Lines[i].AsSpan();
			var rotation = ReadNumber(line[1..]);
			if (line[0] == 'L')
			{
				rotation *= -1;
			}

			dialPosition = (100 + (dialPosition + (rotation))) % 100;

			if (dialPosition == 0)
			{
				zeroEncounters++;
			}
		}

		return zeroEncounters;
	}

	private int ReadNumber(ReadOnlySpan<char> numberString)
	{
		var number = 0;
		for (var i = 0; i < numberString.Length; i++)
		{
			number = number * 10 + (numberString[i] - '0');
		}

		return number;
	}


	public override int SolvePart2(Input input)
	{
		return 1;
	}
}