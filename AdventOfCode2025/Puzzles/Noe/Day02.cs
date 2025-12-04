using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day02 : HappyPuzzleBase<long>
	{
		public override long SolvePart1(Input input)
		{
			var text = input.Text.AsSpan();
			var total = 0L;
			foreach (var range in text.Split(','))
			{
				var line = text[range];
				var splitIndex = line.IndexOf('-');
				var start = long.Parse(line.Slice(0, splitIndex));
				var end = long.Parse(line.Slice(splitIndex + 1));
				for (var i = start; i <= end; i++)
				{
					if (IsNumberInvalid(i))
					{	
						total += i;
					}
				}
			}

			return total;
		}

		public override long SolvePart2(Input input)
		{
			return 0;
		}

		private static bool IsNumberInvalid(long number)
		{
			Span<char> chars = stackalloc char[10];
			number.TryFormat(chars, out var charWritten);
			if (charWritten % 2 != 0)
			{
				return false;
			}

			var middle = charWritten / 2;
			return chars.Slice(0, middle).SequenceEqual(chars.Slice(middle, charWritten - middle));
		}
	}
}
