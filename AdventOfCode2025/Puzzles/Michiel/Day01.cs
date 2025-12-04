using System.Diagnostics.CodeAnalysis;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Michiel;

public class Day01 : HappyPuzzleBase<int>
{
	public override int SolvePart1(Input input)
	{
		var startValue = 50;
		var counter = 0;

		foreach (var line in input.Lines)
		{
			var op = line[0];
			var value = int.Parse(line[1..]) % 100;
			switch (op)
			{
				case 'R':
					startValue += value;
					break;
				case 'L':
					startValue -= value;
					break;
				default:
					throw new Exception("unknown operation");
			}

			startValue = startValue % 100;
			if (startValue < 0)
			{
				startValue += 100;
			}

			if (startValue == 0)
			{
				counter++;
			}
		}

		return counter;
	}

	public override int SolvePart2(Input input)
	{
		var startValue = 50;
		var counter = 0;

		foreach (var line in input.Lines)
		{
			var op = line[0];
			var value = int.Parse(line[1..]);

			var s = startValue;

			counter += (value / 100);
			value = value % 100;
			switch (op)
			{
				case 'R':
					startValue += value;
					if (startValue > 100 || startValue % 100 == 0)
						counter++;
					break;
				case 'L':
					startValue -= value;
					if (startValue <= 0 && startValue + value != 0)
						counter++;
					break;
				default:
					throw new Exception("unknown operation");
			}

			startValue = startValue % 100;

			if (startValue < 0)
			{
				startValue += 100;
			}
		}

		return counter;
	}
}