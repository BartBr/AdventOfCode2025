using System.Diagnostics.CodeAnalysis;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Bart;

[SuppressMessage("ReSharper", "EnforceIfStatementBraces")]
[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public class Day06 : HappyPuzzleBase<long>
{

	public override long SolvePart1(Input input)
	{
		long sum = 0;

		// Double while loop, first fetch the length of the first number (stop at next number or linebreak)
		// all numbers have spaces until the next column of numbers is reached
		// the outer while reads the numbers on the first line
		// the inner while reads the number and math sign at the last row

		var col = 0;
		var operatorsLine = input.Lines[^1];
		while (col < operatorsLine.Length) //Loop over the blocks
		{
			//Step 1 read the operator
			while (col < operatorsLine.Length && operatorsLine[col] == ' ')
			{
				col++;
			}

			// Quick check to eliminate out-of-bounds exception
			if (col == operatorsLine.Length)
			{
				break;
			}

			var mathOperator = operatorsLine[col];
			//Console.WriteLine($"{mathOperator} at col {col}");

			var blockStartIndex = col;
			col++;

			// Step 2 read all the prev numbers and make the sum/product
			var columnResult = ReadNumber(input.Lines[0], blockStartIndex);

			// Step 3 read the next numbers and use the operator with the current result of the column
			for(var row = 1; row < input.Lines.Length -1; row++)
			{
				var number = ReadNumber(input.Lines[row], blockStartIndex);

				if (mathOperator == '+')
				{
					//Console.Write(" + " + number);
					columnResult += number;
				}
				else
				{
					//Console.Write(" * " + number);
					columnResult *= number;
				}
			}

			//Console.WriteLine(" =  " + columnResult);

			sum += columnResult;
		}

		return sum;
	}

	private static long ReadNumber(string inputLine, int index)
	{
		while (inputLine[index] == ' ')
		{
			index++;
		}

		long number = inputLine[index++] - '0';
		while (index < inputLine.Length && inputLine[index] >= '0')
		{
			var digit = inputLine[index++] - '0';
			number = number * 10 + digit;
		}
		return number;
	}

	public override long SolvePart2(Input input)
	{
		long sum = 0;

		scoped Span<int> operatorPositions = stackalloc int[input.Lines[^1].Length];
		var operatorCount = 0;
		for (var i = 0; i < input.Lines[^1].Length; i++)
		{
			if (input.Lines[^1][i] != ' ')
			{
				operatorPositions[operatorCount++] = i;
			}
		}

		// Block loop
		for (var i = 0; i < operatorCount -1; i++)
		{
			var startBlockIndex = operatorPositions[i];
			var nextBlockIndex = operatorPositions[i+1] -1;

			var mathOperator = input.Lines[^1][startBlockIndex];

			var blockResult = ReadColumnNumber(input, startBlockIndex);
			//Console.Write(blockResult);
			for (var col = startBlockIndex+1; col < nextBlockIndex; col++)
			{
				var number = ReadColumnNumber(input, col);
				if (mathOperator == '+')
				{
					//Console.Write(" + " + number);
					blockResult += number;
				}
				else
				{
					//Console.Write(" * " + number);
					blockResult *= number;
				}
			}
			//Console.WriteLine(" = " + blockResult);
			sum += blockResult;
		}

		//Last block, be carefull not all lines have the same length
		var lastBlockIndex = operatorPositions[operatorCount-1];
		var lastMathOperator = input.Lines[^1][lastBlockIndex];
		var lastBlockResult = ReadColumnNumber(input, lastBlockIndex);
		//Console.Write(lastBlockResult);

		// assuming the max amount of last numbers to be 10, early return if the read number is 0
		for(var col= 1; col < 10; col++)
		{
			var number = SafeReadColumnNumber(input, lastBlockIndex + col);
			if(number == 0) continue;
			if (lastMathOperator == '+')
			{
				//Console.Write(" + " + number);
				lastBlockResult += number;
			}
			else
			{
				//Console.Write(" * " + number);
				lastBlockResult *= number;
			}
		}
		//Console.WriteLine(" = " + lastBlockResult);
		sum += lastBlockResult;

		return sum;
	}

	private static long ReadColumnNumber(Input input, int col)
	{
		long number = 0;
		for (var row = 0; row < input.Lines.Length - 1; row++) //Last operator line not included
		{
			var c = input.Lines[row][col];
			if (c != ' ')
			{
				number = number * 10 + c - '0';
			}
		}
		return number;
	}

	private static long SafeReadColumnNumber(Input input, int col)
	{
		long number = 0;
		for (var row = 0; row < input.Lines.Length - 1; row++) //Last operator line not included
		{
			if(col < input.Lines[row].Length)
			{
				var c = input.Lines[row][col];
				if (c != ' ')
				{
					number = number * 10 + c - '0';
				}
			}
		}
		return number;
	}
}
