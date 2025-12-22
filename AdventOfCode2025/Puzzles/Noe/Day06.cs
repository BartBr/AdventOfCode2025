using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day06 : HappyPuzzleBase<long>
	{
		public override long SolvePart1(Input input)
		{
			scoped Span<char> operations = stackalloc char[input.Lines[0].Length];
			var operationLine = input.Lines[^1].AsSpan();
			var operationCount = 0;
			foreach (var number in operationLine.Split(' '))
			{
				if (operationLine[number].IsEmpty)
				{
					continue;
				}
				operations[operationCount++] = operationLine[number.Start];
			}
			//operations = operations.Slice(0, operationCount);

			scoped Span<long> numbers = stackalloc long[operationCount];
			for (var i = 0; i < numbers.Length; i++)
			{
				switch (operations[i])
				{
					case '+':
						numbers[i] = 0;
						break;
					case '*':
						numbers[i] = 1;
						break;
				}
			}

			for (var i = 0; i < input.Lines.Length - 1; i++)
			{
				var j = 0;
				var line = input.Lines[i].AsSpan();
				foreach (var split in line.Split(' '))
				{
					var nb = line[split];
					if (nb.IsEmpty)
					{
						continue;
					}
					var number = ParseLong(nb);
					switch (operations[j])
					{
						case '+':
							numbers[j++] += number;
							break;
						case '*':
							numbers[j++] *= number;
							break;
					}
				}
			}
			var total = 0L;
			foreach (var number in numbers)
			{
				total += number;
			}

			return total;
		}

		public override long SolvePart2(Input input)
		{
			var total = 0L;
			scoped Span<char> chars = stackalloc char[input.Lines.Length - 1];
			scoped Span<long> nums = stackalloc long[10];
			var idx = 0;
			for (var i = input.Lines[0].Length - 1; i >= 0; i--)
			{
				for (var j = 0; j < input.Lines.Length - 1; j++)
				{
					chars[j] = input.Lines[j][i];
				}
				nums[idx++] = ParseLong(chars);
				var lastChar = input.Lines[^1][i];
				switch (lastChar)
				{
					case '+':
						var sum = 0L;
						foreach (var n in nums.Slice(0, idx))
						{
							sum += n;
						}
						idx = 0;
						total += sum;
						i--;
						continue;
					case '*':
						var product = 1L;
						foreach (var n in nums.Slice(0, idx))
						{
							product *= n;
						}
						idx = 0;
						total += product;
						i--;
						continue;
				}
			}

			return total;
		}

		private static long ParseLong(ReadOnlySpan<char> input)
		{
			var number = 0L;
			for (var i = 0; i < input.Length; i++)
			{
				if (input[i] == ' ')
				{
					continue;
				}
				var lastDigit = input[i] - '0';
				number = number * 10L + lastDigit;
			}
			return number;
		}
	}
}
