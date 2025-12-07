using System.Diagnostics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day06 : HappyPuzzleBase<long>
{
	/// <summary>
	/// Solution for part 1
	/// Plan of attack:
	///		1. Read operator
	///		2. Determine max width of operands by skipping until next operator or EOL (operator is always known to have a stable position per problem)
	///		3. Parse first operand individually
	///		4. Parse subsequent operands and apply operator to current number
	///		5. Add resulting number to total
	/// </summary>
	public override long SolvePart1(Input input)
	{
		var inputLines = input.Lines;

#if DEBUG

        // Ensure all input lines have the same length in debug mode.
		var referenceLength = inputLines[0].Length;
		for(var i = 1; i < inputLines.Length; i++) {
			Debug.Assert(inputLines[i].Length == referenceLength, "All input lines should have same length");
		}

#endif

		var operandCount = inputLines.Length - 1;
		var operators = inputLines[^1];

		long sum = 0;

		// Processing happens from Left-to-Right
		for (var sectionStartIndex = 0; sectionStartIndex < operators.Length; )
		{
			// Read operator for current "problem"
			var @operator = operators[sectionStartIndex];

			Debug.Assert(@operator != ' ', "Not a valid operator");

			// Determine max width for current "problem" by skipping until the next operator or EOL
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

			var maxTargetOperandSectionIndex = sectionStartIndex + problemWidth - 1;

			// Parse the first operand individually.
			// This makes it easier down the line if the operator is '*' as we can just keep multiplying the original number without additional checks
			var operandLine = inputLines[0];
			long number = 0;
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

			// Parse subsequent operands and apply the operator to the current number
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

	/// <summary>
	/// Solution for part 1
	/// Plan of attack:
	///		1. Read operator
	///		2. Determine max operand count by skipping until next operator or EOL (operator is always known to have a stable position per problem)
	///		3. Parse first operand individually (except now just take all digits at the same index of the operator)
	///		4. Parse subsequent operands column-by-column and apply operator to current number, incrementing the general index by 1 each time
	///		5. Add resulting number to total
	///		6. General index is incremented by 1 every time we start a new "problem"
	/// </summary>
	/// <remarks>
	///	Solution "technically violates" the reading RTL part as order operands doesn't matter for the applicable operators
	/// </remarks>
	public override long SolvePart2(Input input)
	{
		var inputLines = input.Lines;

#if DEBUG

        // Ensure all input lines have the same length in debug mode.
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
			// Read operator for current "problem"
			var @operator = operators[sectionStartIndex];

			Debug.Assert(@operator != ' ', "Not a valid operator");

			// Determine max width for current "problem" by skipping until the next operator or EOL
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

			// Parse the first operand individually (by taking all digits at the same index of the operator).
			// This makes it easier down the line if the operator is '*' as we can just keep multiplying the original number without additional checks
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

			// Parse subsequent operands column-by-column and apply the operator to the current number
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