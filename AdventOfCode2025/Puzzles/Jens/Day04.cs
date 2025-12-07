using System.Diagnostics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day04 : HappyPuzzleBase<int>
{
	public override int SolvePart1(Input input)
	{
		var inputLines = input.Lines;
		var height = inputLines.Length;
		var width = inputLines[0].Length;

		var extendedWidth = width + 2;

		scoped Span<int> adjacentOffsets = [-extendedWidth - 1, -extendedWidth, -extendedWidth + 1, -1, 1, extendedWidth - 1, extendedWidth, extendedWidth + 1];

		scoped Span<bool> paperRollBuffer = stackalloc bool[(extendedWidth) * (height + 2)];

		for (var y = 0; y < height; y++)
		{
			var inputLineSpan = inputLines[y].AsSpan();

			for (var x = 0; x < width; x++)
			{
				if (inputLineSpan[x] == '@')
				{
					paperRollBuffer[extendedWidth * (y + 1) + x + 1] = true;
				}
			}
		}

		var accessiblePaperRolls = 0;

		for (var x = 1; x < width; x++)
		{
			for (var y = 1; y < height; y++)
			{
				var i = extendedWidth * y + x;
				if (!paperRollBuffer[i])
				{
					continue;
				}

				var adjacentRollCount = 0;
				foreach (var offset in adjacentOffsets)
				{
					var adjacentRollIndex = i + offset;
					if (paperRollBuffer[adjacentRollIndex] && ++adjacentRollCount > 3)
					{
						break;
					}
				}

				if (adjacentRollCount < 4)
				{
					accessiblePaperRolls++;
				}
			}
		}

		return accessiblePaperRolls;
	}

	public override int SolvePart2(Input input)
	{
		var inputLines = input.Lines;
		var height = inputLines.Length;
		var width = inputLines[0].Length;

		var extendedWidth = width + 2;

		scoped Span<int> adjacentOffsets = [-extendedWidth - 1, -extendedWidth, -extendedWidth + 1, -1, 1, extendedWidth - 1, extendedWidth, extendedWidth + 1];

		scoped Span<bool> paperRollBuffer = stackalloc bool[(extendedWidth) * (height + 2)];

		for (var y = 0; y < height; y++)
		{
			var inputLineSpan = inputLines[y].AsSpan();

			for (var x = 0; x < width; x++)
			{
				if (inputLineSpan[x] == '@')
				{
					paperRollBuffer[extendedWidth * (y + 1) + x + 1] = true;
				}
			}
		}

		var accessiblePaperRolls = 0;

		while (true)
		{
			var removalBufferSize = 0;

			for (var x = 1; x < width; x++)
			{
				for (var y = 1; y < height; y++)
				{
					var i = extendedWidth * y + x;
					if (!paperRollBuffer[i])
					{
						continue;
					}

					var adjacentRollCount = 0;
					foreach (var offset in adjacentOffsets)
					{
						var adjacentRollIndex = i + offset;
						if (paperRollBuffer[adjacentRollIndex] && ++adjacentRollCount > 3)
						{
							break;
						}
					}

					if (adjacentRollCount < 4)
					{
						removalBufferSize++;
						paperRollBuffer[i] = false;
					}
				}
			}

			if (removalBufferSize == 0)
			{
				break;
			}

			Debug.WriteLine("This iteration removed " + removalBufferSize + " accessible paper rolls");
			accessiblePaperRolls += removalBufferSize;
		}

		return accessiblePaperRolls;
	}
}