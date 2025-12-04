using System.Diagnostics.CodeAnalysis;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Bart;

[SuppressMessage("ReSharper", "EnforceIfStatementBraces")]
[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public class Day04 : HappyPuzzleBase<int>
{

	public override int SolvePart1(Input input)
	{
		var sum = 0;
		var rows = input.Lines[0].Length;
		var columns = input.Lines.Length;

		for (var row = 0; row < rows; row++)
		{
			for (var col = 0; col < columns; col++)
			{
				if (input.Lines[row][col] == '@' && CountAdjacentPaperRolls(row, col, input) < 4)
				{
					sum++;
				}
			}
		}

		return sum;
	}

	private static int CountAdjacentPaperRolls(int row, int col, Input input)
	{
		var rows = input.Lines[row].Length;
		var columns = input.Lines[0].Length;
		var sum = 0;

		//1 Hori Left
		if (col > 0 && input.Lines[row][col-1] == '@') sum++;
		//2 Diag Left Up
		if (row > 0 && col > 0 && input.Lines[row-1][col-1] == '@') sum++;
		//3 Vert Up
		if (row > 0 && input.Lines[row-1][col] == '@') sum++;
		//4 Diag Right up
		if (row > 0 && col < columns-1 && input.Lines[row-1][col+1] == '@') sum++;
		//5 Hori Right
		if (col < rows-1 && input.Lines[row][col+1] == '@') sum++;
		//6 Diag Right Down
		if (row < rows-1 && col < columns-1 && input.Lines[row+1][col+1] == '@') sum++;
		//7 Vert Down
		if (row < rows-1 && input.Lines[row+1][col] == '@') sum++;
		//8 Diag Down Left
		if (row < rows-1 && col > 0 && input.Lines[row+1][col-1] == '@') sum++;

		return sum;
	}

	public override int SolvePart2(Input input)
	{
		var sum = 0;

		var rows = input.Lines[0].Length;
		var columns = input.Lines.Length;
		scoped Span<char> gridSource = stackalloc char[(rows +2) * (columns +2)];
		gridSource.Fill('-');
		var grid = new Grid<char>(gridSource, rows+2,  columns+2 );

		//Copy the input in a grid that is one bigger on all sides, so no off grid checks are required
		for (var row = 0; row < rows; row++)
		{
			for (var col = 0; col < columns; col++)
			{
				grid[row+1, col+1] = input.Lines[row][col];
			}
		}

		var lastRun = RemovePaperRows(ref grid);
		sum += lastRun;

		while (lastRun > 0)
		{
			lastRun = RemovePaperRows(ref grid);
			sum += lastRun;
		}

		return sum;
	}

	private static int RemovePaperRows(ref Grid<char> grid)
	{
		var sum = 0;
		for (var row = 1; row < grid.Rows-1; row++)
		{
			for (var col = 1; col < grid.Columns-1; col++)
			{
				if (grid[row,col] == '@' && CountAdjacentPaperRolls2(row, col,ref grid) < 4)
				{
					sum++;
					grid[row,col] = '.';
				}
			}
		}
		return sum;
	}

	private static int CountAdjacentPaperRolls2(int row, int col, ref Grid<char> grid)
	{
		var sum = 0;

		//1 Hori Left
		if (grid[(row) , col-1] == '@') sum++;
		//2 Diag Left Up
		if (grid[(row-1) , col-1] == '@') sum++;
		//3 Vert Up
		if (grid[(row-1) , col] == '@') sum++;
		//4 Diag Right up
		if (grid[(row-1) , col+1] == '@') sum++;
		//5 Hori Right
		if (grid[(row) , col+1] == '@') sum++;
		//6 Diag Right Down
		if (grid[(row+1) , col+1] == '@') sum++;
		//7 Vert Down
		if (grid[(row+1) , col] == '@') sum++;
		//8 Diag Down Left
		if (grid[(row+1) , col-1] == '@') sum++;

		return sum;
	}
}

internal ref struct Grid<T> where T : struct
{
	private Span<T> _source;

	public int Rows { get; }
	public int Columns { get; }

	public Grid(Span<T> source, int rows, int columns)
	{
		_source = source;

		Columns = columns;
		Rows = rows;
	}

	// 2D indexer: grid[row, col]
	public ref T this[int row, int col] => ref _source[row * Columns + col];
}