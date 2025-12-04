using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Bart;

[SuppressMessage("ReSharper", "EnforceIfStatementBraces")]
[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public class Day01 : HappyPuzzleBase<int>
{

	public override int SolvePart1(Input input)
	{
		var amountOfTimesHitZero = 0;
		var currentPosition = 50;

		for (var i = 0; i < input.Lines.Length; i++)
		{
			var number = ReadNumber(input.Lines[i]);

			number %= 100;

			currentPosition = number + currentPosition;

			if (currentPosition < 0) currentPosition += 100;
			if (currentPosition > 99) currentPosition -= 100;

			if(currentPosition == 0) amountOfTimesHitZero++;
		}

		return amountOfTimesHitZero;
	}

	private static int ReadNumber(string input)
	{
		var number = 0;
		for (var i = 0; i < input.Length - 1; i++)
		{
			var lastDigit = input[i + 1] - '0';
			number = number * 10 + lastDigit;
		}

		if (input[0] == 'L')
		{
			number *= -1;
		}

		return number;
	}

	public override int SolvePart2(Input input)
	{
		scoped Span<int> reportNumbers = stackalloc int[input.Lines.Length];

		for (var i = 0; i < input.Lines.Length; i++)
		{
			reportNumbers[i] = ReadNumber(input.Lines[i]);
		}

		var amountOfTimesHitZero = 0;
		var currentPosition = 50;
		var overTheDialPosition = 0;

		for(var i = 0; i< input.Lines.Length;i++)
		{
			//Console.Write($"Pos: {currentPosition:00} - ");
			var prevPosition = currentPosition;
			currentPosition += (reportNumbers[i] % 100);
			var timesOverTheDial = Abs(reportNumbers[i] / 100);

			if (currentPosition < 0)
			{
				if(prevPosition != 0) timesOverTheDial++;

				currentPosition += 100;
			}

			if (currentPosition > 99)
			{
				currentPosition -= 100;
				if(currentPosition != 0) timesOverTheDial++;
			}
			if(currentPosition == 0) amountOfTimesHitZero++;

			overTheDialPosition += timesOverTheDial;

			// Console.Write("The dial is rotated" + (reportNumbers[i] < 0 ? " L" : " R") + Abs(reportNumbers[i]) + " to point " + currentPosition);
			// if (timesOverTheDial > 0)
			// {
			// 	Console.WriteLine($"; during this rotation, it points at 0 {timesOverTheDial} times.");
			// }
			// else
			// {
			// 	Console.WriteLine(".");
			// }
		}

		return amountOfTimesHitZero + overTheDialPosition;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Abs(int x)
	{
		return x < 0 ? -x : x;
	}
}