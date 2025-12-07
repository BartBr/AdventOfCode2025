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
				}
				else
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
		long sum = 0;
		var operatorLine = input.Lines[^1];
		for (var i = 0; i < operatorLine.Length;)
		{
			if (operatorLine[i] == ' ')
			{
				i++;
				continue;
			}
			var op = operatorLine[i];
			var problem = ReadNumber(i, input.Lines);
			long number = -1;
			for (i++; i >= operatorLine.Length || number != 0; i++)
			{
				number = ReadNumber(i, input.Lines);
				if (number != 0)
				{
					if (op == '+')
					{
						problem += number;
					}
					else
					{
						problem *= number;
					}
				}
				else if (i >= operatorLine.Length)
				{
					return sum + problem;
				}
			}

			sum += problem;
		}

		return sum;
	}

	private long ReadNumber(int pos, string[] lines)
	{
		long number = 0;
		for (var i = 0; i < lines.Length - 1; i++)
		{
			if (pos >= lines[i].Length) continue;
			if (lines[i][pos] == ' ') continue;
			number = number * 10 + (lines[i][pos] - '0');
		}

		return number;
	}
}