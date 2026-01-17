using System.Diagnostics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day07 : HappyPuzzleBase<long>
	{
		private ref struct HashSet
		{
			private readonly Span<int> _values;
			private readonly Span<bool> _filter;
			public HashSet(Span<int> span, Span<bool> filter)
			{
				_values = span;
				_filter = filter;
			}

			public readonly Span<int> Values => _values.Slice(0, Count);
			public int Count = 0;

			public void Add(int range)
			{
				if (_filter[range])
				{
					return;
				}

				_values[Count] = range;
				_filter[range] = true;
				Count++;
			}

			public void RemoveAt(int index)
			{
				ref var value = ref _values[index];
				_filter[value] = false;
				value = _values[Count - 1];
				Count--;
			}

			public readonly int this[int index]
			{
				get => _values[index];
				set => _values[index] = value;
			}
		}

		public override long SolvePart1(Input input)
		{
			var total = 0;
			Span<int> buffer = stackalloc int[input.Lines[0].Length];
			Span<bool> filter = stackalloc bool[input.Lines[0].Length];
			var beams = new HashSet(buffer, filter);
			var start = input.Lines[0].IndexOf('S');
			beams.Add(start);
			for (var j = 2; j < input.Lines.Length; j += 2)
			{
				var line = input.Lines[j];
				var bc = beams.Count;
				for (var i = bc - 1; i >= 0; i--)
				{
					var beam = beams[i];
					var c = line[beam];
					if (c == '^')
					{
						// Remove origin beam
						beams.RemoveAt(i);
						// Add left beam
						beams.Add(beam - 1);
						// Add right beam
						beams.Add(beam + 1);
						total++;
					}
				}
			}
			return total;
		}

		public override long SolvePart2(Input input)
		{
			var total = 0L;
			Span<int> buffer = stackalloc int[input.Lines[0].Length];
			Span<bool> filter = stackalloc bool[input.Lines[0].Length];

			Span<long> previousLine = stackalloc long[input.Lines[0].Length];
			Span<long> currentLine = stackalloc long[input.Lines[0].Length];
			var beams = new HashSet(buffer, filter);
			var start = input.Lines[0].IndexOf('S');
			beams.Add(start);
			previousLine[start] = 1;
			for (var j = 2; j < input.Lines.Length; j += 2)
			{
				var line = input.Lines[j];
				var bc = beams.Count;
				for (var i = bc - 1; i >= 0; i--)
				{
					var beam = beams[i];
					var c = line[beam];
					if (c == '^')
					{
						var val = previousLine[beam];
						currentLine[beam - 1] += val;
						currentLine[beam + 1] += val;
						// Remove origin beam
						beams.RemoveAt(i);
						// Add left beam
						beams.Add(beam - 1);
						// Add right beam
						beams.Add(beam + 1);
					}
					else
					{
						currentLine[beam] += previousLine[beam];
					}
					previousLine[beam] = 0;
				}
				var prev = previousLine;
				previousLine = currentLine;
				currentLine = prev;
			}

			foreach (var count in previousLine)
			{
				total += count;
			}
			return total;
		}
	}
}
