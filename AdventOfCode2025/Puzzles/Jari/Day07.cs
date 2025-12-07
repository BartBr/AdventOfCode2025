using System.Diagnostics.CodeAnalysis;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jari;

[SuppressMessage("ReSharper", "EnforceIfStatementBraces")]
[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public class Day07 : HappyPuzzleBase<long>
{
	public override long SolvePart1(Input input)
	{
		var paths = new bool[input.Lines.Length][];
		paths[0] = new bool[input.Lines[0].Length];
		for (var i = 0; i < input.Lines[0].Length; i++)
		{
			if (input.Lines[0][i] == 'S')
			{
				paths[0][i] = true;
				break;
			}
		}

		var splitterEncounterCounter = 0;
		for (var i = 1; i < input.Lines.Length; i++)
		{
			paths[i] = new bool[input.Lines[i].Length];

			for (var j = 0; j < input.Lines[i].Length; j++)
			{
				if (paths[i - 1][j])
				{
					if (input.Lines[i][j] == '^')
					{
						paths[i][j - 1] = true;
						paths[i][j + 1] = true;
						splitterEncounterCounter++;
					}
					else
					{
						paths[i][j] = true;
					}
				}
			}
		}

		return splitterEncounterCounter;
	}

	public override long SolvePart2(Input input)
	{
		var knownPaths = new long[input.Lines.Length][];
		for (var i = 0; i < input.Lines[0].Length; i++)
		{
			if (input.Lines[0][i] == 'S')
			{
				return TraverseTimeLines(i, 1, input.Lines, ref knownPaths);
			}
		}

		return 0;
	}

	private long TraverseTimeLines(int x, int y, string[] lines, ref long[][] knownPaths)
	{
		knownPaths[y] ??= new long[lines[y].Length];
		if (knownPaths[y][x] != 0)
		{
			return knownPaths[y][x];
		}

		long counter = 1;
		while (y < lines.Length && x < lines[y].Length)
		{
			if (lines[y][x] == '^')
			{
				if (y < lines.Length && x + 1 < lines[y].Length)
				{
					knownPaths[y] ??= new long[lines[y].Length];
					knownPaths[y][x + 1] = TraverseTimeLines(x + 1, y, lines, ref knownPaths);
					counter += knownPaths[y][x + 1];
				}

				x--;
			}

			y++;
		}

		return counter;
	}
}