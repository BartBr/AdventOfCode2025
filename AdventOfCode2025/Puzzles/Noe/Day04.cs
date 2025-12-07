using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day04 : HappyPuzzleBase<int>
	{
		public override int SolvePart1(Input input)
		{
			var totalCount = 0;
			var lines = input.Lines.AsSpan();
			for (var y = 0; y < lines.Length; y++)
			{
				var line = lines[y].AsSpan();
				for (var x = 0; x < line.Length; x++)
				{
					var c = line[x];
					if (c != '@')
					{
						continue;
					}

					var count = 0;
					// Left
					if (x > 0 && line[x - 1] == '@')
					{
						count++;
					}
					// Right
					if (x < line.Length - 1 && line[x + 1] == '@')
					{
						count++;
					}

					if (y > 0)
					{
						var topLine = lines[y - 1].AsSpan();
						// Top
						if (topLine[x] == '@')
						{
							count++;
						}

						// Top Left
						if (x > 0 && topLine[x - 1] == '@')
						{
							count++;
						}
						// Top Right
						if (x < line.Length - 1 && topLine[x + 1] == '@')
						{
							count++;
						}
					}

					if (y < lines.Length - 1)
					{
						var bottomLine = lines[y + 1].AsSpan();
						// Bottom
						if (bottomLine[x] == '@')
						{
							count++;
						}

						// Bottom Left
						if (x > 0 && bottomLine[x - 1] == '@')
						{
							count++;
						}
						// Bottom Right
						if (x < line.Length - 1 && bottomLine[x + 1] == '@')
						{
							count++;
						}
					}

					if (count < 4)
					{
						totalCount++;
					}
				}
			}
			return totalCount;
		}

		private const int BIT_COUNT = (sizeof(long) * 8);
		public override int SolvePart2(Input input)
		{
			var width = input.Lines[0].Length;
			var height = input.Lines.Length;
			Span<long> chars = stackalloc long[((width * height) / BIT_COUNT) + 1];
			var text = input.Text.AsSpan();
			var index = 0;
			for (var i = 0; i < text.Length; i++)
			{
				if (text[i] == '\r' || text[i] == '\n')
				{
					continue;
				}
				if (text[i] == '@')
				{
					var subIndex = index / BIT_COUNT;
					SetBit(ref chars[subIndex], index % BIT_COUNT, true);
				}
				index++;
			}

			var totalCount = 0;
			Span<long> temp = stackalloc long[chars.Length];
			var removed = true;
			while (removed)
			{
				chars.CopyTo(temp);
				removed = false;
				for (var y = 0; y < height; y++)
				{
					for (var x = 0; x < width; x++)
					{
						if (!GetAt(chars, x, y, height, width))
						{
							continue;
						}

						var count = 0;
						// Left
						if (GetAt(chars, x - 1, y, width, height))
						{
							count++;
						}
						// Right
						if (GetAt(chars, x + 1, y, width, height))
						{
							count++;
						}

						// Top
						if (GetAt(chars, x, y - 1, width, height))
						{
							count++;
						}

						// Top Left
						if (GetAt(chars, x - 1, y - 1, width, height))
						{
							count++;
						}
						// Top Right
						if (GetAt(chars, x + 1, y - 1, width, height))
						{
							count++;
						}

						// Bottom
						if (GetAt(chars, x, y + 1, width, height))
						{
							count++;
						}

						// Bottom Left
						if (GetAt(chars, x - 1, y + 1, width, height))
						{
							count++;
						}
						// Bottom Right
						if (GetAt(chars, x + 1, y + 1, width, height))
						{
							count++;
						}

						if (count < 4)
						{
							totalCount++;
							removed = true;
							SetAt(temp, x, y, false, width, height);
						}
					}
				}
				temp.CopyTo(chars);
			}

			return totalCount;
		}

		private static void SetBit(ref long originalValue, int index, bool value)
		{
			if (value)
			{
				originalValue |= 1L << index;
			}
			else
			{
				originalValue &= ~(1L << index);
			}
		}

		private static void SetAt(Span<long> grid, int x, int y, bool value, int width, int height)
		{
			var index = y * width + x;
			var longIndex = index / BIT_COUNT;
			var bitIndex = index % BIT_COUNT;
			SetBit(ref grid[longIndex], bitIndex, value);
		}

		private static bool GetAt(Span<long> chars, int x, int y, int width, int height)
		{
			if (x > 0 && x < width && y > 0 && y < height)
			{
				var index = y * width + x;
				var longIndex = index / BIT_COUNT;
				var bitIndex = index % BIT_COUNT;
				return (chars[longIndex] & 1 << bitIndex) != 0;
			}
			return false;
		}
	}
}
