using System.Runtime.CompilerServices;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day08 : HappyPuzzleBase<long>
	{
		private const int ITERATION_COUNT = 1000;

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
				if (Count == 0)
				{
					_connections[0] = connection;
					Count++;
				}

				if (connection.Length > _connections[Count - 1].Length)
				{
					return;
				}

				for (var i = 0; i < ITERATION_COUNT; i++)
				{
					ref var c = ref _connections[i];
					if (c.Length != 0 && c.Length <= connection.Length)
					{
						continue;
					}

					var length = Count - i;
					var source = _connections.Slice(i, length);
					var dest = _connections.Slice(i + 1, length);
					source.CopyTo(dest);
					_connections[i] = connection;
					Count = Math.Min(_connections.Length - 1, Count + 1);
					return;
				}
			}
		}

		private struct Node
		{
			public readonly int X;
			public readonly int Y;
			public readonly int Z;

			public int ConnectionIndex = -1;
			public int ConnectionCount;

			public Node(ReadOnlySpan<char> line)
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

			public readonly long DistanceSquared(Node other)
			{
				var dx = (long) (X - other.X);
				var dy = (long) (Y - other.Y);
				var dz = (long) (Z - other.Z);
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
				var r = StartIndex.CompareTo(other.StartIndex);
				return r == 0 ? EndIndex.CompareTo(other.EndIndex) : r;
			}

			public override string ToString()
			{
				return $"{StartIndex} -> {EndIndex} : {Length}";
			}
		}

		private readonly ref struct Graph
		{
			private readonly Span<Connection> _connections;
			private readonly Span<Node> _nodes;

			public Graph(string[] inputs, Span<Connection> connections, Span<Node> nodes)
			{
				_nodes = nodes;
				_connections = connections;
				// Create all nodes
				for (var i = 0; i < inputs.Length; i++)
				{
					_nodes[i] = new Node(inputs[i]);
				}

				// Gather all ITERATION_COUNT closest connections
				var set = new SortedConnectionSet(_connections);
				for (var i = 0; i < _nodes.Length - 1; i++)
				{
					ref var start = ref _nodes[i];
					for (var j = i + 1; j < _nodes.Length; j++)
					{
						ref var end = ref _nodes[j];
						var dist = start.DistanceSquared(end);
						var c = new Connection(dist, i, j);
						set.Add(c);
					}
				}

				for (var i = 0; i < ITERATION_COUNT; i++)
				{
					ref var c = ref _connections[i];
					_connections[ITERATION_COUNT + i] = new Connection(c.Length, c.EndIndex, c.StartIndex);
				}

				// Sort by StartIndex
				_connections.Sort();

				// Make a map of all connections by node
				var prevIdx = -1;
				for (var i = 0; i < _connections.Length; i++)
				{
					var idx = _connections[i].StartIndex;
					ref var node = ref _nodes[idx];
					node.ConnectionCount++;
					if (prevIdx == idx)
					{
						continue;
					}

					node.ConnectionIndex = i;
					prevIdx = idx;
				}
			}

			public Span<int> GetGroupSize(Span<int> groupSizes)
			{
				scoped Span<bool> visited = stackalloc bool[_nodes.Length];

				for (var i = 0; i < _nodes.Length; i++)
				{
					if (visited[i])
					{
						continue;
					}
					ref var groupSize = ref groupSizes[i];
					SearchRecursive(i, visited, ref groupSize);
				}
				groupSizes.Sort();
				return groupSizes;
			}

			private void SearchRecursive(
				int index,
				Span<bool> visited,
				ref int count)
			{
				if (visited[index])
				{
					return;
				}

				visited[index] = true;
				count++;

				ref var node = ref _nodes[index];
				if (node.ConnectionIndex == -1)
				{
					return;
				}

				for (var i = 0; i < node.ConnectionCount; i++)
				{
					var end = _connections[node.ConnectionIndex + i].EndIndex;
					SearchRecursive(end, visited, ref count);
				}
			}
		}

		public override long SolvePart1(Input input)
		{
			Span<Connection> connections = stackalloc Connection[ITERATION_COUNT * 2];
			Span<Node> nodes = stackalloc Node[input.Lines.Length];

			var graph = new Graph(input.Lines, connections, nodes);

			Span<int> groupSizes = stackalloc int[nodes.Length];
			groupSizes = graph.GetGroupSize(groupSizes);

			var result = 1L;
			for (var i = 1; i <= 3; i++)
			{
				result *= groupSizes[groupSizes.Length - i];
			}
			return result;
		}

		public override long SolvePart2(Input input)
		{
			return SolvePart1(input);
		}

		private static int LogN(int n)
		{
			var res = 0;
			for (var i = 1; i < n; i++)
			{
				res += i;
			}
			return res;
		}

		private static int CompareConnectionLength(Connection a, Connection b)
		{
			return a.Length.CompareTo(b.Length);
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
