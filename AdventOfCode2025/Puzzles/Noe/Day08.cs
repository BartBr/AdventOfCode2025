using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day08 : HappyPuzzleBase<long>
	{
		private readonly struct Vector3
		{
			public readonly int X;
			public readonly int Y;
			public readonly int Z;

			public Vector3(ReadOnlySpan<char> line)
			{
				var number = 0;
				var i = 0;
				for (; i < line.Length; i++)
				{
					if (line[i] == ',')
					{
						i++;
						break;
					}
					var lastDigit = line[i] - '0';
					number = number * 10 + lastDigit;
				}
				X = number;
				number = 0;

				for (; i < line.Length; i++)
				{
					if (line[i] == ',')
					{
						i++;
						break;
					}
					var lastDigit = line[i] - '0';
					number = number * 10 + lastDigit;
				}
				Y = number;
				number = 0;

				for (; i < line.Length; i++)
				{
					if (line[i] == ',')
					{
						break;
					}
					var lastDigit = line[i] - '0';
					number = number * 10 + lastDigit;
				}
				Z = number;
			}

			public readonly long DistanceSquared(Vector3 other)
			{
				var dx = X - other.X;
				var dy = Y - other.Y;
				var dz = Z - other.Z;
				return dx * dx + dy * dy + dz * dz;
			}
		}

		private readonly struct Connection(long length, int startIndex, int endIndex)
			: IComparable<Connection>
		{
			public readonly long Length = length;
			public readonly int StartIndex = startIndex;
			public readonly int EndIndex = endIndex;

			public readonly int CompareTo(Connection other)
			{
				return StartIndex.CompareTo(other.StartIndex);
			}

			public override string ToString()
			{
				return $"{StartIndex} -> {EndIndex} : {Length}";
			}
		}

		private ref struct SortedConnectionSet
		{
			public int Count = 0;
			private readonly Span<Connection> _connections;

			public SortedConnectionSet(Span<Connection> connections)
			{
				_connections = connections;
			}

			public void Add(Connection connection)
			{
				var i = 0;
				for (; i < _connections.Length; i++)
				{
					ref var c = ref _connections[i];
					if (c.Length == 0 || c.Length > connection.Length)
					{
						var length = Math.Max(Count - i, 0);
						var source = _connections.Slice(i, length);
						var dest = _connections.Slice(i + 1, length);
						source.CopyTo(dest);
						_connections[i] = connection;
						Count = Math.Min(_connections.Length - 1, Count + 1);
						return;
					}
				}
			}
		}

		public override long SolvePart1(Input input)
		{
			const int ITERATION_COUNT = 10;
			Span<Vector3> nodes = stackalloc Vector3[input.Lines.Length];
			for (var i = 0; i < input.Lines.Length; i++)
			{
				nodes[i] = new Vector3(input.Lines[i]);
			}

			// Gather all closest connections
			Span<Connection> connections = stackalloc Connection[ITERATION_COUNT + 1];
			var set = new SortedConnectionSet(connections);
			for (var i = 0; i < nodes.Length - 1; i++)
			{
				ref var start = ref nodes[i];
				for (var j = i + 1; j < nodes.Length; j++)
				{
					ref var end = ref nodes[j];
					var dist = start.DistanceSquared(end);
					var c = new Connection(dist, i, j);
					set.Add(c);
				}
			}

			// Only keep ITERATION_COUNT shortest ones
			connections = connections.Slice(0, ITERATION_COUNT);

			Span<int> groupItems = stackalloc int[connections.Length];
			Span<int> groupSize = stackalloc int[connections.Length];

			var groupId = 0;
			var prevEndIndex = connections[0].EndIndex;
			// First pass to make group based on EndIndex (group end together)
			for (var i = 0; i < connections.Length; i++)
			{
				// Create a new group
				if (prevEndIndex != connections[i].StartIndex)
				{
					groupId++;
				}

				groupItems[i] = groupId;
				groupSize[groupId] = groupSize[groupId] + 1;
				prevEndIndex = connections[i].StartIndex;
			}

			for (var i = 0; i < connections.Length; i++)
			{
				ref var c = ref connections[i];
				var a = groupItems[c.StartIndex];
			}

			groupSize = groupSize.Slice(0, ITERATION_COUNT);
			groupSize.Sort();
			var result = 1;
			for (var i = ITERATION_COUNT - 1; i >= ITERATION_COUNT - 4; i--)
			{
				result *= groupSize[i];
			}

			return result;
		}


		private static void SearchRecursive(
			Connection connection,
			int startIndex,
			Span<Connection> connections,
			Span<int> groupItems,
			Span<int> groupSize)
		{
			for (var i = startIndex; i < connections.Length; i++)
			{

			}
		}

		private static int CompareConnectionEnd(Connection a, Connection b)
		{
			var r = a.EndIndex.CompareTo(b.EndIndex);
			if (r == 0)
			{
				r = a.StartIndex.CompareTo(b.StartIndex);
			}

			return r;
		}

		public override long SolvePart2(Input input)
		{
			return 0;
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
