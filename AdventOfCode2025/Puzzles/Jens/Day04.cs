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

		for (var i = extendedWidth + 1; i < paperRollBuffer.Length - extendedWidth - 1; i++)
		{
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

		scoped Span<int> removalBuffer = stackalloc int[2_000];
		var removalBufferSize = 0;

		while (true)
		{
			for (var i = extendedWidth + 1; i < paperRollBuffer.Length - extendedWidth - 1; i++)
			{
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
					removalBuffer[removalBufferSize++] = i;
				}
			}

			if (removalBufferSize == 0)
			{
				break;
			}

			Debug.WriteLine("Removing " + removalBufferSize + " accessible paper rolls");
			for(var i = 0; i < removalBufferSize; i++)
			{
				paperRollBuffer[removalBuffer[i]] = false;
			}

			removalBufferSize = 0;
		}

		return accessiblePaperRolls;
	}
}