using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day12 : HappyPuzzleBase<int, string>
{
	public override int SolvePart1(Input input)
	{
		scoped Span<int> shapeAreas = stackalloc int[10];
		var shapeAreaCount = 0;

		var i = 0;

		// Parse shape areas
		for (; i < input.Lines.Length; i++)
		{
			var line = input.Lines[i].AsSpan();
			if (line.EndsWith(':'))
			{ ;
				line = input.Lines[++i].AsSpan();

				do
				{
					shapeAreas[shapeAreaCount] += line.Count('#');
					line = input.Lines[++i].AsSpan();
				} while (line.Length > 0);

				shapeAreaCount++;

				continue;
			}

			break;
		}

		shapeAreas = shapeAreas[..shapeAreaCount];

		var validGrids = 0;
		for (; i < input.Lines.Length; i++)
		{
			if (IsValidGrid(input.Lines[i].AsSpan(), shapeAreas))
			{
				validGrids++;
			}
		}

		return validGrids;
	}

	private static bool IsValidGrid(ReadOnlySpan<char> gridDescriptor, Span<int> shapeAreas)
	{
		var height = 0;
		var width = 0;

		scoped Span<int> shapeCounts = stackalloc int[shapeAreas.Length];
		var shapeCount = 0;

		ref var target = ref height;
		for (var i = 0; i < gridDescriptor.Length; i++)
		{
			var c = gridDescriptor[i];
			switch (c)
			{
				case 'x':
					target = ref width;
					break;
				case ':':
					target = ref shapeCounts[shapeCount++];
					i++; // skip first whitespace
					break;

				case ' ':
					target = ref shapeCounts[shapeCount++];
					break;
				default:
					target = target * 10 + (c - '0');
					break;
			}
		}

		var totalAvailableGridArea = height * width;
		var minimumRequiredShapeArea = 0;
		for (var i = 0; i < shapeCounts.Length; i++)
		{
			minimumRequiredShapeArea += shapeAreas[i] * shapeCounts[i];
		}

		return totalAvailableGridArea >= minimumRequiredShapeArea;
	}

	public override string SolvePart2(Input input)
	{
		return "Click!";
	}
}