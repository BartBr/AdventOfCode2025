using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Michiel;

public class Day04 : HappyPuzzleBase<long>
{
	public override long SolvePart1(Input input)
	{
		var xSize = input.Lines[0].Length;
		var ySize = input.Lines.Length;

		var grid = new char[xSize, ySize];
		var adjacents = new int[xSize, ySize];
		var adjacentCountFewerThan4 = 0;

		for (int y = 0; y < ySize; y++)
		{
			var line = input.Lines[y];
			for (int x = 0; x < xSize; x++)
			{
				grid[x, y] = line[x];

				if (line[x] != '@')
					continue;

				var adjacent = 0;
				for (int my = -1; my <= 1; my++)
				{
					for (int mx = -1; mx <= 1; mx++)
					{
						if (mx == 0 && my == 0)
							continue;
						var checkX = x + mx;
						var checkY = y + my;
						if (checkX >= 0 && checkX < xSize && checkY >= 0 && checkY < ySize)
						{
							if (input.Lines[checkY][checkX] == '@')
							{
								adjacent++;
							}
						}
					}
				}
				adjacents[x, y] = adjacent;
				if (adjacent < 4)
				{
					adjacentCountFewerThan4++;
				}
			}
		}

		//Console.WriteLine();
		//for (int y = 0; y < ySize; y++)
		//{
		//    for (int x = 0; x < xSize; x++)
		//    {
		//        if (grid[x,y]=='@' && adjacents[x,y] < 4)
		//            Console.Write('x');
		//        else
		//            Console.Write(grid[x, y]);
		//    }
		//    Console.WriteLine();
		//}

		return adjacentCountFewerThan4;
	}

	public override long SolvePart2(Input input)
	{
		var xSize = input.Lines[0].Length;
		var ySize = input.Lines.Length;

		var grid = new char[xSize, ySize];
		var adjacents = new int[xSize, ySize];
		var adjacentCountFewerThan4 = 0;

		for (int y = 0; y < ySize; y++)
		{
			var line = input.Lines[y];
			for (int x = 0; x < xSize; x++)
			{
				grid[x, y] = line[x];

				if (line[x] != '@')
					continue;

				var adjacent = 0;
				for (int my = -1; my <= 1; my++)
				{
					for (int mx = -1; mx <= 1; mx++)
					{
						if (mx == 0 && my == 0)
							continue;
						var checkX = x + mx;
						var checkY = y + my;
						if (checkX >= 0 && checkX < xSize && checkY >= 0 && checkY < ySize)
						{
							if (input.Lines[checkY][checkX] == '@')
							{
								adjacent++;
							}
						}
					}
				}
				adjacents[x, y] = adjacent;
				if (adjacent < 4)
				{
					adjacentCountFewerThan4++;
				}
			}
		}

		var totalRemoved = 0;

		while (adjacentCountFewerThan4 > 0)
		{
			for (int y = 0; y < ySize; y++)
			{
				for (int x = 0; x < xSize; x++)
				{
					if (grid[x, y] == '@' && adjacents[x, y] < 4)
						grid[x, y] = '.';
				}
			}

			totalRemoved += adjacentCountFewerThan4;

			adjacents = new int[xSize, ySize];
			adjacentCountFewerThan4 = 0;

			for (int y = 0; y < ySize; y++)
			{
				for (int x = 0; x < xSize; x++)
				{
					if (grid[x, y] != '@')
						continue;

					var adjacent = 0;
					for (int my = -1; my <= 1; my++)
					{
						for (int mx = -1; mx <= 1; mx++)
						{
							if (mx == 0 && my == 0)
								continue;
							var checkX = x + mx;
							var checkY = y + my;
							if (checkX >= 0 && checkX < xSize && checkY >= 0 && checkY < ySize)
							{
								if (grid[checkX, checkY] == '@')
								{
									adjacent++;
								}
							}
						}
					}
					adjacents[x, y] = adjacent;
					if (adjacent < 4)
					{
						adjacentCountFewerThan4++;
					}
				}
			}
		}

		return totalRemoved;
	}
}

