using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Michiel;

public class Day03 : HappyPuzzleBase<long>
{
	public override long SolvePart1(Input input)
	{
		var sum = 0L;

		foreach (var line in input.Lines)
		{
			var digits = line.Select(c => c - '0').ToList();
			var lastDigit = digits.Last();
			digits.RemoveAt(digits.Count - 1);

			var joltage = string.Empty;
			while (digits.Count > 0 && joltage.Length < 2)
			{
				var max = digits.Max();
				var index = digits.IndexOf(max);
				digits.RemoveRange(0, index + 1);
				if (joltage.Length == 0)
				{
					digits.Add(lastDigit);
				}
				joltage += max.ToString();
			}
			sum += int.Parse(joltage);
		}

		return sum;
	}

	public override long SolvePart2(Input input)
	{
		var sum = 0L;

		foreach (var line in input.Lines)
		{
			var digits = line.Select(c => c - '0').ToList();
			var lastDigit = digits.Last();
			var endDigits = digits[(digits.Count - 11)..];
			digits.RemoveRange(digits.Count - 11, 11);

			var joltage = string.Empty;
			while (digits.Count > 0 && joltage.Length < 12)
			{
				var max = digits.Max();
				var index = digits.IndexOf(max);
				digits.RemoveRange(0, index + 1);
				if (endDigits.Count > 0)
				{
					digits.Add(endDigits[0]);
					endDigits.RemoveAt(0);
				}
				joltage += max.ToString();
			}
			sum += long.Parse(joltage);
		}

		return sum;
	}
}

