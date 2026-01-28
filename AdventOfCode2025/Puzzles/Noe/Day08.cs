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
						i++;
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
				return Length.CompareTo(other.Length);
			}

			public override string ToString()
			{
				return $"{StartIndex} -> {EndIndex} : {Length}";
			}
		}

		public override long SolvePart1(Input input)
		{
			Span<Vector3> nodes = stackalloc Vector3[input.Lines.Length];
			for (var i = 0; i < input.Lines.Length; i++)
			{
				nodes[i] = new Vector3(input.Lines[i]);
			}

			// Gather all closest connections
			Span<Connection> connections = stackalloc Connection[nodes.Length];
			for (var i = 0; i < nodes.Length; i++)
			{
				ref var start = ref nodes[i];
				var maxIndex = -1;
				var currentSize = long.MaxValue;
				for (var j = 0; j < nodes.Length; j++)
				{
					if (j == i)
					{
						continue;
					}

					if ((i != 0 && connections[j].EndIndex == i))
					{
						break;
					}

					ref var end = ref nodes[j];
					var dist = start.DistanceSquared(end);
					if (dist < currentSize)
					{
						currentSize = dist;
						maxIndex = j;
					}
				}
				connections[i] = new Connection(currentSize, i, maxIndex);
			}

			// Only keep X shortest ones
			const int ITERATION_COUNT = 10;
			connections.Sort();
			connections = connections.Slice(0, ITERATION_COUNT);
			connections.Sort(CompareConnectionEnd);

			Span<int> groupItems = stackalloc int[connections.Length];
			Span<int> groupSize = stackalloc int[connections.Length];

			var groupId = 0;
			for (var i = 0; i < connections.Length; i++)
			{
				var end = connections[i].EndIndex;
				var j = 0;
				while (j < connections.Length && (connections[j].EndIndex != end || j == i))
				{
					j++;
				}

				// New group
				if (j == connections.Length)
				{
					groupItems[i] = groupId++;
					groupSize[i] = 1;
					continue;
				}

				while (j < connections.Length && connections[j].EndIndex == end)
				{

				}
			}
			//for (var i = 0; i < connections.Length; i++)
			//{
			//	ref var connection = ref connections[i];
			//	var currentGroupId = (int?) null;
			//	for (var j = 0; j < i; j++)
			//	{
			//		ref var other = ref connections[j];
			//		if (other.StartIndex == connection.EndIndex ||
			//			other.EndIndex == connection.StartIndex ||
			//			other.EndIndex == connection.EndIndex)
			//		{
			//			// If not yet part of a group, add it
			//			if (currentGroupId == null)
			//			{
			//				currentGroupId = groupItems[j];
			//			}
			//			else
			//			{

			//			}

			//			groupItems[i] = currentGroupId.Value;
			//			groupSize[currentGroupId.Value] = groupSize[currentGroupId.Value] + 1;
			//		}
			//	}
			//}

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
