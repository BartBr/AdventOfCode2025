using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day01 : HappyPuzzleBase<int>
	{
		private const int DIAL_SIZE = 100;

		public override int SolvePart1(Input input)
		{
			var current = 50;
			var count = 0;
			foreach (var line in input.Lines)
			{
				var stepCount = ParseInt(line.AsSpan(1)) % DIAL_SIZE;
				switch (line[0])
				{
					case 'L':
						current -= stepCount;
						if (current < 0)
						{
							current += DIAL_SIZE;
						}
						break;
					case 'R':
						current += stepCount;
						if (current >= DIAL_SIZE)
						{
							current -= DIAL_SIZE;
						}
						break;
				}

				if (current == 0)
				{
					count++;
				}
			}

			return count;
		}

		public override int SolvePart2(Input input)
		{
			var current = 50;
			var count = 0;
			foreach (var line in input.Lines)
			{
				var stepCount = ParseInt(line.AsSpan(1));
				switch (line[0])
				{
					case 'L':
						var previous = current;
						current -= stepCount % DIAL_SIZE;
						if (current < 0)
						{
							current += DIAL_SIZE;
							if (previous != 0)
							{
								count++;
							}
						}
						break;
					case 'R':
						current += stepCount % DIAL_SIZE;
						if (current >= DIAL_SIZE)
						{
							current -= DIAL_SIZE;
							if (current != 0)
							{
								count++;
							}
						}
						break;
				}

				count += stepCount / DIAL_SIZE;

				if (current == 0)
				{
					count++;
				}
			}

			return count;
		}

		private static int ParseInt(ReadOnlySpan<char> input)
		{
			var number = 0;
			for (var i = 0; i < input.Length; i++)
			{
				var lastDigit = input[i] - '0';
				number = number * 10 + lastDigit;
			}
			return number;
		}
	}
}
