using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Noe
{
	public class Day08 : HappyPuzzleBase<long>
	{
		private const int ITERATION_COUNT = 1000;

		private ref struct SortedConnectionSet
		{
			private readonly int _maxLength;
			public int Count = 0;
			private readonly Span<Connection> _connections;

			public SortedConnectionSet(Span<Connection> connections, int maxLength)
			{
				_connections = connections;
				_maxLength = maxLength;
			}

			public void Reset()
			{
				Count = 0;
			}

			public void Add(in Connection connection)
			{
				if (Count == 0)
				{
					_connections[0] = connection;
					Count++;
					return;
				}

				if (connection.Length > _connections[Count - 1].Length)
				{
					return;
				}

				for (var i = 0; i < Count; i++)
				{
					ref var c = ref _connections[i];
					if (c.Length <= connection.Length)
					{
						continue;
					}

					var source = _connections.Slice(i, Count - i);
					var dest = _connections.Slice(i + 1);
					source.CopyTo(dest);
					_connections[i] = connection;
					Count = Math.Min(_maxLength, Count + 1);
					return;
				}
			}
		}

		private readonly struct Node
		{
			public readonly int X;
			public readonly int Y;
			public readonly int Z;

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

			public readonly long DistanceSquared(in Node other)
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

		public override long SolvePart1(Input input)
		{
			var inputs = input.Lines;
			Span<Connection> connections = stackalloc Connection[ITERATION_COUNT + 1];
			Span<Node> nodes = stackalloc Node[input.Lines.Length];

			// Create all nodes
			for (var i = 0; i < inputs.Length; i++)
			{
				nodes[i] = new Node(inputs[i]);
			}

			// Fetch all ITERATION_COUNT shortest distances
			var set = new SortedConnectionSet(connections, connections.Length - 1);
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

			Span<int> groups = stackalloc int[nodes.Length];
			var currentGroupId = 1;
			var maxNodeId = -1;
			var minNodeId = groups.Length;

			for (var i = 0; i < set.Count; i++)
			{
				ref var connection = ref connections[i];
				ref var startGroup = ref groups[connection.StartIndex];
				ref var endGroup = ref groups[connection.EndIndex];
				// If both are already in the same group, just ignore
				if (startGroup != 0 && startGroup == endGroup)
				{
					continue;
				}

				maxNodeId = connection.EndIndex > maxNodeId ? connection.EndIndex : maxNodeId;
				minNodeId = connection.StartIndex < minNodeId ? connection.StartIndex : minNodeId;

				var startIsInGroup = startGroup != 0;
				var endIsInGroup = endGroup != 0;
				if (startIsInGroup && endIsInGroup)
				{
					MergeGroup(groups.Slice(minNodeId, maxNodeId - minNodeId + 1), endGroup, startGroup);
					continue;
				}

				if (startIsInGroup != endIsInGroup)
				{
					if (startIsInGroup)
					{
						endGroup = startGroup;
					}
					else
					{
						startGroup = endGroup;
					}
					continue;
				}

				startGroup = currentGroupId;
				endGroup = currentGroupId;
				currentGroupId++;
			}

			Span<int> groupSizes = stackalloc int[currentGroupId];
			foreach (var group in groups)
			{
				if (group != 0)
				{
					groupSizes[group]++;
				}
			}

			groupSizes.Sort();

			return groupSizes[^1] * groupSizes[^2] * groupSizes[^3];
		}

		public override long SolvePart2(Input input)
		{
			var inputs = input.Lines;
			Span<Connection> connections = stackalloc Connection[inputs.Length / 2 + 1];
			Span<Node> nodes = stackalloc Node[input.Lines.Length];

			// Create all nodes
			for (var i = 0; i < inputs.Length; i++)
			{
				nodes[i] = new Node(inputs[i]);
			}

			var set = new SortedConnectionSet(connections, connections.Length - 1);
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

			Span<int> groups = stackalloc int[nodes.Length];
			var currentGroupId = 1;
			var maxNodeId = -1;
			var minNodeId = groups.Length;

			var a = -1;
			var b = -1;

			while (true)
			{
				for (var i = 0; i < set.Count; i++)
				{
					ref var connection = ref connections[i];
					ref var startGroup = ref groups[connection.StartIndex];
					ref var endGroup = ref groups[connection.EndIndex];
					// If both are already in the same group, just ignore
					if (startGroup != 0 && startGroup == endGroup)
					{
						continue;
					}
					a = connection.StartIndex;
					b = connection.EndIndex;
					maxNodeId = connection.EndIndex > maxNodeId ? connection.EndIndex : maxNodeId;
					minNodeId = connection.StartIndex < minNodeId ? connection.StartIndex : minNodeId;

					var startIsInGroup = startGroup != 0;
					var endIsInGroup = endGroup != 0;
					if (startIsInGroup && endIsInGroup)
					{
						MergeGroup(groups.Slice(minNodeId, maxNodeId - minNodeId + 1), endGroup, startGroup);
						continue;
					}

					if (startIsInGroup != endIsInGroup)
					{
						if (startIsInGroup)
						{
							endGroup = startGroup;
						}
						else
						{
							startGroup = endGroup;
						}
						continue;
					}

					startGroup = currentGroupId;
					endGroup = currentGroupId;
					currentGroupId++;
				}

				if (IsSameGroup(groups))
				{
					break;
				}

				FetchNextConnections(nodes, set, groups, connections[^1].Length);
			}

			return nodes[a].X * nodes[b].X;
		}

		private static void FetchNextConnections(in Span<Node> nodes, SortedConnectionSet set, in Span<int> groups, long minLength)
		{
			set.Reset();
			for (var i = 0; i < nodes.Length - 1; i++)
			{
				ref var start = ref nodes[i];
				var startGroup = groups[i];
				for (var j = i + 1; j < nodes.Length; j++)
				{
					var endGroup = groups[j];
					// If both are already in the same group, just ignore
					if (startGroup != 0 && startGroup == endGroup)
					{
						continue;
					}
					ref var end = ref nodes[j];
					var dist = start.DistanceSquared(in end);
					if (dist < minLength)
					{
						continue;
					}

					var c = new Connection(dist, i, j);
					set.Add(in c);
				}
			}
		}

		private static bool IsSameGroup(Span<int> groups)
		{
			var prev = groups[0];
			for (var i = 1; i < groups.Length; i++)
			{
				if (prev != groups[i])
				{
					return false;
				}
				prev = groups[i];
			}
			return true;
		}

		private static void MergeGroup(Span<int> groups, int previousGroupId, int newGroupId)
		{
			for (var i = 0; i < groups.Length; i++)
			{
				if (groups[i] == previousGroupId)
				{
					groups[i] = newGroupId;
				}
			}
		}
	}
}
