using System.Diagnostics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day06 : HappyPuzzleBase<long>
{
	public override long SolvePart1(Input input)
	{
		var inputLines = input.Lines;

#if DEBUG

		var referenceLength = inputLines[0].Length;
		for(var i = 1; i < inputLines.Length; i++) {
			Debug.Assert(inputLines[i].Length == referenceLength, "All input lines should have same length");
		}

#endif

		var operandCount = inputLines.Length - 1;
		var operators = inputLines[^1];

		long sum = 0;

		for (var sectionStartIndex = 0; sectionStartIndex < operators.Length; )
		{
			var @operator = operators[sectionStartIndex];

			Debug.Assert(@operator != ' ', "Not a valid operator");

			var problemWidth = 1;
			do
			{
				++problemWidth;
				if (sectionStartIndex + problemWidth >= operators.Length)
				{
					problemWidth += 1;
					break;
				}
			} while (operators[sectionStartIndex + problemWidth] == ' ');

			long number = 0;

			var maxTargetOperandSectionIndex = sectionStartIndex + problemWidth - 1;
			var operandLine = inputLines[0];
			for (var i = sectionStartIndex; i < maxTargetOperandSectionIndex; i++)
			{
				var potentialDigit = operandLine[i];
				if (potentialDigit == ' ')
				{
					continue;
				}

				number = number * 10 + (potentialDigit - '0');
			}

			Debug.WriteLine("Starting number: " + number);

			for (var operandIndexer = 1; operandIndexer < operandCount; operandIndexer++)
			{
				operandLine = inputLines[operandIndexer];
				var parsedNumber = 0;
				for (var i = sectionStartIndex; i < maxTargetOperandSectionIndex; i++)
				{
					var potentialDigit = operandLine[i];
					if (potentialDigit == ' ')
					{
						continue;
					}

					parsedNumber = parsedNumber * 10 + (potentialDigit - '0');
				}
				Debug.WriteLine("Subsequent number: " + number);

				if (@operator == '+')
				{
					number += parsedNumber;
				}
				else
				{
					number *= parsedNumber;
				}
			}

			Debug.WriteLine("Adding to total: " + number);
			sum += number;

			sectionStartIndex += problemWidth;
		}

		return sum;
	}

	// Solution "technically violates" the reading RTL part as order operands doesn't matter for the applicable operators of operators doesn't matter here
	public override long SolvePart2(Input input)
	{
		var inputLines = input.Lines;

#if DEBUG

		var referenceLength = inputLines[0].Length;
		for(var i = 1; i < inputLines.Length; i++) {
			Debug.Assert(inputLines[i].Length == referenceLength, "All input lines should have same length");
		}

#endif

		var maxOperandDigitCount = inputLines.Length - 1;
		var operators = inputLines[^1];

		long sum = 0;

		for (var sectionStartIndex = 0; sectionStartIndex < operators.Length; sectionStartIndex++)
		{
			var @operator = operators[sectionStartIndex];

			Debug.Assert(@operator != ' ', "Not a valid operator");

			var problemWidth = 1;
			do
			{
				++problemWidth;
				if (sectionStartIndex + problemWidth >= operators.Length)
				{
					problemWidth += 1;
					break;
				}
			} while (operators[sectionStartIndex + problemWidth] == ' ');

			long number = 0;
			for (var i = 0; i < maxOperandDigitCount; i++)
			{
				var potentialDigit = inputLines[i][sectionStartIndex];
				if (potentialDigit == ' ')
				{
					continue;
				}

				number = number * 10 + (potentialDigit - '0');
			}

			++sectionStartIndex;

			Debug.WriteLine("Starting number: " + number);

			var maxTargetOperandSectionIndex = sectionStartIndex + problemWidth - 2;
			for (; sectionStartIndex < maxTargetOperandSectionIndex; sectionStartIndex++)
			{
				var parsedNumber = 0;
				for (var i = 0; i < maxOperandDigitCount; i++)
				{
					var potentialDigit = inputLines[i][sectionStartIndex];
					if (potentialDigit == ' ')
					{
						continue;
					}

					parsedNumber = parsedNumber * 10 + (potentialDigit - '0');
				}

				Debug.WriteLine("Subsequent number: " + number);

				if (@operator == '+')
				{
					number += parsedNumber;
				}
				else
				{
					number *= parsedNumber;
				}
			}

			Debug.WriteLine("Adding to total: " + number);
			sum += number;
		}

		return sum;
	}
}