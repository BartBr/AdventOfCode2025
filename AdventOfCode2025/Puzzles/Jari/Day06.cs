using System.Diagnostics.CodeAnalysis;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jari;

[SuppressMessage("ReSharper", "EnforceIfStatementBraces")]
[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public class Day06 : HappyPuzzleBase<long>
{
	public override long SolvePart1(Input input)
	{
		long sum = 0;
		var operatorLine = input.Lines[^1];
		for (var i = 0; i < operatorLine.Length; i++)
		{
			if (operatorLine[i] == ' ') continue;
			long problem = ReadNumber(input.Lines[0].AsSpan()[i..]);
			for (var j = 1; j < input.Lines.Length - 1; j++)
			{
				var number = ReadNumber(input.Lines[j].AsSpan()[i..]);
				if (operatorLine[i] == '+')
				{
					problem += number;
				} else
				{
					problem *= number;
				}
			}

			sum += problem;
		}
		return sum;
	}

	private long ReadNumber(ReadOnlySpan<char> numberString)
	{
		long number = 0;
		for (var i = 0; i < numberString.Length; i++)
		{
			if (numberString[i] == ' ')
			{
				if (number == 0)
				{
					continue;
				}

				return number;
			}
			number = number * 10 + (numberString[i] - '0');
		}

		return number;
	}

	public override long SolvePart2(Input input)
	{
		return 1;
	}
}