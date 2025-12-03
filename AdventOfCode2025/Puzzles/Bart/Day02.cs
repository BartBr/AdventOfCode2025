using System.Diagnostics.CodeAnalysis;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Bart;

[SuppressMessage("ReSharper", "EnforceIfStatementBraces")]
[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public class Day02 : HappyPuzzleBase<long>
{

	public override long SolvePart1(Input input)
	{
		long sum = 0;

		var cursor = 0;
		while (cursor < input.Text.Length)
		{
			var firstNumber = ReadNumber(input.Text, ref cursor);
			cursor++;
			var secondNumber = ReadNumber(input.Text, ref cursor);
			cursor++;

			//Console.Write($"{firstNumber} - {secondNumber} => ");


			for (var i = firstNumber; i <= secondNumber; i++)
			{
				if (IsRepeatedPattern(i))
				{
					sum += i;
					//Console.Write($"{i},");
				}
			}

			//Console.WriteLine("");
		}

		return sum;
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

	private static bool IsRepeatedPattern(long number)
	{
		var digits = Digits(number);

		if(digits % 2 == 1) return false;

		var patternLength = digits / 2;
		var pattern = number / Pow10[patternLength];
		var rest = number % Pow10[patternLength];

		return rest == pattern;
	}

	private static bool IsRepeatedPattern2(long number)
	{
		var digits = Digits(number);

		for (var patternLength = 1; patternLength <= digits / 2; patternLength++)
		{
			if (digits % patternLength != 0)
				continue;

			var pow = Pow10[patternLength];
			var pattern = number % pow;

			var t = number;
			var allMatch = true;

			while (t > 0)
			{
				if (t % pow != pattern)
				{
					allMatch = false;
					break;
				}

				t /= pow;
			}

			if (allMatch)
				return true;
		}

		return false;
	}

	/// <returns>
	/// Returns the number of digits in a number
	/// </returns>
	private static int Digits(long number)
	{
		var tmp = number;
		var digits = 0;
		while (tmp > 0)
		{
			digits++;
			tmp /= 10;
		}
		return digits;
	}

	private static long ReadNumber(string input, ref int cursor)
	{
		long number = 0;

		while (cursor < input.Length)
		{
			//Skip newline
			//if(input[cursor] == '\n') cursor++;

			if(input[cursor] == '-' || input[cursor] == ',') return number;

			var lastDigit = input[cursor] - '0';
			number = number * 10 + lastDigit;

			cursor++;
		}

		return number;
	}

	public override long SolvePart2(Input input)
	{
		long sum = 0;

		var cursor = 0;
		while (cursor < input.Text.Length)
		{
			var firstNumber = ReadNumber(input.Text, ref cursor);
			cursor++;
			var secondNumber = ReadNumber(input.Text, ref cursor);
			cursor++;

			//Console.Write($"{firstNumber} - {secondNumber} => ");


			for (var i = firstNumber; i <= secondNumber; i++)
			{
				if (IsRepeatedPattern2(i))
				{
					sum += i;
					//Console.Write($"{i},");
				}
			}

			//Console.WriteLine("");
		}

		return sum;
	}
}