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
				var start = ParseLong(line.Slice(0, splitIndex));
				var end = ParseLong(line.Slice(splitIndex + 1));
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
			var text = input.Text.AsSpan();
			var total = 0L;
			foreach (var range in text.Split(','))
			{
				var line = text[range];
				var splitIndex = line.IndexOf('-');
				var start = ParseLong(line.Slice(0, splitIndex));
				var end = ParseLong(line.Slice(splitIndex + 1));
				for (var i = start; i <= end; i++)
				{
					if (IsNumberInvalid2(i))
					{
						total += i;
					}
				}
			}

			return total;
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
			var chunkSize = middle - 1;

			for (var i = 0; i < chunkSize; i++)
			{
				if (chars[i] != chars[middle + i])
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsNumberInvalid2(long number)
		{
			Span<char> chars = stackalloc char[10];
			number.TryFormat(chars, out var charWritten);
			chars = chars.Slice(0, charWritten);

			for (var chunkSize = 1; chunkSize < chars.Length / 2; chunkSize++)
			{
				if (AllChunksAreEqual(chars, chunkSize))
				{
					return true;
				}
			}

			return false;
		}

		private static bool AllChunksAreEqual(ReadOnlySpan<char> chars, int chunkSize)
		{
			if (chars.Length % chunkSize != 0)
			{
				return false;
			}

			var chunkCount = chars.Length / chunkSize;
			var previousChunk = chars.Slice(0, chunkSize);
			for (var j = 1; j < chunkCount; j++)
			{
				var currentChunk = chars.Slice(j * chunkSize, chunkSize);
				if (!previousChunk.SequenceEqual(currentChunk))
				{
					return false;
				}
				previousChunk = currentChunk;
			}
			return true;
		}

		private static long ParseLong(ReadOnlySpan<char> input)
		{
			var number = 0L;
			for (var i = 0; i < input.Length; i++)
			{
				var lastDigit = input[i] - '0';
				number = number * 10L + lastDigit;
			}
			return number;
		}
	}
}
