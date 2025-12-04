using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Michiel;

public class Day02 : HappyPuzzleBase<long>
{
	public override long SolvePart1(Input input)
	{
		var ranges = input.Lines[0].Split(',');
		var invalidIds = new List<long>();

		foreach (var range in ranges)
		{
			var parts = range.Split('-');
			var start = long.Parse(parts[0]);
			var end = long.Parse(parts[1]);
			for (var i = start; i <= end; i++)
			{
				var s = i.ToString();
				if (s.Length % 2 == 0)
				{
					if (s[0..(s.Length / 2)] == s[(s.Length / 2)..])
					{
						invalidIds.Add(i);
					}
				}
			}
		}

		return invalidIds.Sum();
	}

	public override long SolvePart2(Input input)
	{
		var ranges = input.Lines[0].Split(',');
		var invalidIds = new List<long>();

		foreach (var range in ranges)
		{
			var parts = range.Split('-');
			var start = long.Parse(parts[0]);
			var end = long.Parse(parts[1]);

			for (var i = start; i <= end; i++)
			{
				var s = i.ToString();
				if (s.Length == 1)
					continue;

				for (int j = 1; j <= s.Length / 2; j++)
				{
					if (s.Length % j == 0 && string.Join(string.Empty, Enumerable.Repeat(s[0..j], s.Length / j)) == s)
					{
						invalidIds.Add(i);
						break;
					}
				}
			}
		}

		return invalidIds.Sum();
	}
}