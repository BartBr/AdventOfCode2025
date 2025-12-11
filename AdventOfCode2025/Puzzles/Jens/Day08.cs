using System.Diagnostics;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day08 : HappyPuzzleBase<int, ulong>
{
	public override int SolvePart1(Input input)
	{
		const int multiPointLineBufferWidth = 100;

		Span<Point3D> unorderedPoints = stackalloc Point3D[input.Lines.Length];
		Span<int> coordinateDimensionValues = stackalloc int[3];
		ref var x = ref coordinateDimensionValues[0];
		ref var y = ref coordinateDimensionValues[1];
		ref var z = ref coordinateDimensionValues[2];
		for (var i = 0; i < input.Lines.Length; i++)
		{
			var coordinateSpan = input.Lines[i].AsSpan();

			var dimIndex = 0;
			ref var target = ref coordinateDimensionValues[dimIndex];

			foreach (var c in coordinateSpan)
			{
				if (c == ',')
				{
					dimIndex++;
					target = ref coordinateDimensionValues[dimIndex];
				}
				else
				{
					target = target * 10 + c - '0';
				}
			}

			unorderedPoints[i] = new Point3D((ushort)i, x, y, z);

			x = 0;
			y = 0;
			z = 0;
		}

#if DEBUG
		// Calculate all distances between all points without duplicates, acting as a reference to debug the kD-tree
		var allDistances = new List<Line3D>(input.Lines.Length * (input.Lines.Length - 1) / 2);
		// Create all possible distances between all points, excluding start and end points being equal and reverse of each other
		for (var i = 0; i < unorderedPoints.Length - 1; i++)
		{
			for (var j = i + 1; j < unorderedPoints.Length; j++)
			{
				var a = unorderedPoints[i];
				var b = unorderedPoints[j];
				var dist = a.DistanceSquared(b);
				allDistances.Add(new Line3D(a.Id, b.Id, dist));
			}
		}

		allDistances.Sort();
#endif

		scoped Span<Point3D> stackAllocKTreeBuffer = stackalloc Point3D[unorderedPoints.Length];
		unorderedPoints.CopyTo(stackAllocKTreeBuffer);

		Span<int> treeNodeBuffer = stackalloc int[stackAllocKTreeBuffer.Length * 6];
		var kdTree3 = new VolumetricTreeAllocFree(treeNodeBuffer, stackAllocKTreeBuffer);

		Span<Line3D> orderedDistances = stackalloc Line3D[unorderedPoints.Length];
		for (var i = 0; i < unorderedPoints.Length; i++)
		{
			var point = unorderedPoints[i];
			var findNearest = kdTree3.FindNearest(point.Id, 0);
			while (orderedDistances.Slice(0, i).Contains(findNearest))
			{
				Debug.WriteLine($"Skipping {findNearest} due to already covered by opposite connection");
				findNearest = kdTree3.FindNearest(point.Id, findNearest.DistanceSquared);
			}

			orderedDistances[i] = findNearest;
		}

		orderedDistances.Sort();

		// Encoding:
		// Buffer can hold 100 (99 + 1) numbers per multi-point line
		// First number is the size of the buffer
		Span<ushort> multiPointLineBuffers = stackalloc ushort[(unorderedPoints.Length / 2) * multiPointLineBufferWidth];
		var multiPointLineBuffersSize = 0;

		var iterationCount = input.Lines.Length == 20 ? 10 : 1_000;
		for (var i = 0; i < iterationCount; i++)
		{
			var shortest = orderedDistances[0];
			Debug.WriteLine($"Iteration {i:D3}: {shortest}");

#if DEBUG
			Debug.Assert(shortest == allDistances[i], "Shortest distance not at the start of the ordered distances array");
#endif

			var startCoordinateId = shortest.Start;
			var endCoordinateId = shortest.End;
			for (var multiPointLineBufferIndex = 0; multiPointLineBufferIndex < multiPointLineBuffersSize; multiPointLineBufferIndex++)
			{
				var bufferStartIndex = multiPointLineBufferIndex * multiPointLineBufferWidth;
				var currentBufferSize = (int) multiPointLineBuffers[bufferStartIndex];
				var currentBufferSlice = multiPointLineBuffers.Slice(bufferStartIndex + 1, currentBufferSize);

				var startCoordinatePartOfBuffer = currentBufferSlice.Contains(startCoordinateId);
				var endCoordinatePartOfBuffer = currentBufferSlice.Contains(endCoordinateId);

				if (startCoordinatePartOfBuffer && endCoordinatePartOfBuffer)
				{
					Debug.WriteLineIf(startCoordinatePartOfBuffer && endCoordinatePartOfBuffer,
						$"Coordinates {shortest.Start} and {shortest.End} are already part of the same MultiPointLine, skipping");
					goto endCurrentIteration;
				}

				if (startCoordinatePartOfBuffer ^ endCoordinatePartOfBuffer)
				{
					Debug.WriteLineIf(startCoordinatePartOfBuffer, $"Coordinate {shortest.Start} is already part of a MultiPointLine, but {shortest.End} isn't. Adding to MultiPointLine");
					Debug.WriteLineIf(endCoordinatePartOfBuffer, $"Coordinate {shortest.End} is already part of a MultiPointLine, but {shortest.Start} isn't. Adding to MultiPointLine");

					var coordinateIdToAddToBuffer = startCoordinatePartOfBuffer ? endCoordinateId : startCoordinateId;
					multiPointLineBuffers[bufferStartIndex + 1 + currentBufferSize] = coordinateIdToAddToBuffer;

					Debug.Assert(currentBufferSize + 1 < multiPointLineBufferWidth, "Buffer size overflow detected");

					multiPointLineBuffers[bufferStartIndex] = (ushort) ++currentBufferSize;

					// No need to check for buffers prior to this one, as the they didn't contain either of the two coordinates anyways.
					// Only newly added coordinate can result in a merge... AND ONLY 1.
					var buffersApplicableForMerge = multiPointLineBuffers.Slice(bufferStartIndex, (multiPointLineBuffersSize - multiPointLineBufferIndex) * multiPointLineBufferWidth);
					MergeMultiPointBuffersIfApplicable(
						ref buffersApplicableForMerge,
						ref multiPointLineBuffersSize,
						coordinateIdToAddToBuffer,
						multiPointLineBufferWidth);
					goto endCurrentIteration;
				}
			}

			Debug.WriteLine($"Coordinates [{shortest.Start}, {shortest.End}] weren't part of an existing MultiPointLine, creating new MultiPointLine3D");
			var newBufferStartIndex = multiPointLineBuffersSize * multiPointLineBufferWidth;
			multiPointLineBuffers[newBufferStartIndex] = 2;
			multiPointLineBuffers[newBufferStartIndex + 1] = startCoordinateId;
			multiPointLineBuffers[newBufferStartIndex + 2] = endCoordinateId;

			++multiPointLineBuffersSize;

			endCurrentIteration: ;

			DebuggerMethod(multiPointLineBuffers, multiPointLineBufferWidth);

			// Sort in new
			var newNearest = kdTree3.FindNearest(shortest.Start, shortest.DistanceSquared);
			while (orderedDistances.Contains(newNearest))
			{
				Debug.WriteLine($"Skipping {newNearest} due to already covered by opposite connection");
				newNearest = kdTree3.FindNearest(newNearest.Start, newNearest.DistanceSquared);
			}

			var j = 0;
			for (; j < orderedDistances.Length - 1; j++)
			{
				if (newNearest.DistanceSquared < orderedDistances[j + 1].DistanceSquared)
				{
					break;
				}
			}

			orderedDistances.Slice(1, j).CopyTo(orderedDistances);
			orderedDistances[j] = newNearest;
		}

		Span<ulong> multiPointLineBufferSizes = stackalloc ulong[multiPointLineBuffersSize];
		for (var i = 0; i < multiPointLineBuffersSize; i++)
		{
			multiPointLineBufferSizes[i] = multiPointLineBuffers[i * multiPointLineBufferWidth];
		}

		multiPointLineBufferSizes.Sort();
		var solution = (int) multiPointLineBufferSizes[^1] * (int) multiPointLineBufferSizes[^2] * (int) multiPointLineBufferSizes[^3];
		Debug.WriteLine(
			$"Result: {multiPointLineBufferSizes[^1]} * {multiPointLineBufferSizes[^2]} * {multiPointLineBufferSizes[^3]} = {multiPointLineBufferSizes[^1] * multiPointLineBufferSizes[^2] * multiPointLineBufferSizes[^3]}");

		return solution;
	}

	public override ulong SolvePart2(Input input)
	{
		const int multiPointLineBufferWidth = 1001;

		Span<Point3D> unorderedPoints = stackalloc Point3D[input.Lines.Length];

		Span<int> coordinateDimensionValues = stackalloc int[3];
		ref var xCoordinate = ref coordinateDimensionValues[0];
		ref var yCoordinate = ref coordinateDimensionValues[1];
		ref var zCoordinate = ref coordinateDimensionValues[2];

		for (var i = 0; i < input.Lines.Length; i++)
		{
			var coordinateSpan = input.Lines[i].AsSpan();

			var dimIndex = 0;

			ref var target = ref coordinateDimensionValues[dimIndex];
			foreach (var c in coordinateSpan)
			{
				if (c == ',')
				{
					dimIndex++;
					target = ref coordinateDimensionValues[dimIndex];
				}
				else
				{
					target = target * 10 + c - '0';
				}
			}

			unorderedPoints[i] = new Point3D((ushort)i, xCoordinate, yCoordinate, zCoordinate);

			xCoordinate = 0;
			yCoordinate = 0;
			zCoordinate = 0;
		}

#if DEBUG
		// Calculate all distances between all points without duplicates, acting as a reference to debug the kD-tree
		var allDistances = new List<Line3D>(input.Lines.Length * (input.Lines.Length - 1) / 2);
		// Create all possible distances between all points, excluding start and end points being equal and reverse of each other
		for (var i = 0; i < unorderedPoints.Length - 1; i++)
		{
			for (var j = i + 1; j < unorderedPoints.Length; j++)
			{
				var a = unorderedPoints[i];
				var b = unorderedPoints[j];
				var dist = a.DistanceSquared(b);
				allDistances.Add(new Line3D(a.Id, b.Id, dist));
			}
		}

		allDistances.Sort();
#endif

		scoped Span<Point3D> stackAllocKTreeBuffer = stackalloc Point3D[unorderedPoints.Length];
		unorderedPoints.CopyTo(stackAllocKTreeBuffer);

		Span<int> treeNodeBuffer = stackalloc int[stackAllocKTreeBuffer.Length * 6];
		var kdTree3 = new VolumetricTreeAllocFree(treeNodeBuffer, stackAllocKTreeBuffer);

		Span<Line3D> orderedDistances = stackalloc Line3D[unorderedPoints.Length];
		for (var i = 0; i < unorderedPoints.Length; i++)
		{
			var point = unorderedPoints[i];
			var findNearest = kdTree3.FindNearest(point.Id, 0);
			while (orderedDistances.Slice(0, i).Contains(findNearest))
			{
				Debug.WriteLine($"Skipping {findNearest} due to already covered by opposite connection");
				findNearest = kdTree3.FindNearest(point.Id, findNearest.DistanceSquared);
			}

			orderedDistances[i] = findNearest;
		}

		orderedDistances.Sort();

		// Encoding:
		// Buffer can hold 1000 (+ 1 for multi-point line length) numbers per multi-point line
		// First number is the size of the buffer
		Span<ushort> multiPointLineBuffers = stackalloc ushort[(unorderedPoints.Length / 4) * multiPointLineBufferWidth];
		var multiPointLineBuffersSize = 0;

		Line3D shortest;
		for (var i = 0; ; i++)
		{
			shortest = orderedDistances[0];
			Debug.WriteLine($"Iteration {i:D3}: {shortest}");

#if DEBUG
			Debug.Assert(shortest == allDistances[i], "Shortest distance not at the start of the ordered distances array");
#endif

			var startCoordinateId = shortest.Start;
			var endCoordinateId = shortest.End;
			for (var multiPointLineBufferIndex = 0; multiPointLineBufferIndex < multiPointLineBuffersSize; multiPointLineBufferIndex++)
			{
				var bufferStartIndex = multiPointLineBufferIndex * multiPointLineBufferWidth;
				var currentBufferSize = (int) multiPointLineBuffers[bufferStartIndex];
				var currentBufferSlice = multiPointLineBuffers.Slice(bufferStartIndex + 1, currentBufferSize);

				var startCoordinatePartOfBuffer = currentBufferSlice.Contains(startCoordinateId);
				var endCoordinatePartOfBuffer = currentBufferSlice.Contains(endCoordinateId);

				if (startCoordinatePartOfBuffer && endCoordinatePartOfBuffer)
				{
					Debug.WriteLineIf(startCoordinatePartOfBuffer && endCoordinatePartOfBuffer,
						$"Coordinates {shortest.Start} and {shortest.End} are already part of the same MultiPointLine, skipping");
					goto endCurrentIteration;
				}

				if (startCoordinatePartOfBuffer ^ endCoordinatePartOfBuffer)
				{
					Debug.WriteLineIf(startCoordinatePartOfBuffer, $"Coordinate {shortest.Start} is already part of a MultiPointLine, but {shortest.End} isn't. Adding to MultiPointLine");
					Debug.WriteLineIf(endCoordinatePartOfBuffer, $"Coordinate {shortest.End} is already part of a MultiPointLine, but {shortest.Start} isn't. Adding to MultiPointLine");

					var encodedCoordinateToAddToBuffer = startCoordinatePartOfBuffer ? endCoordinateId : startCoordinateId;
					multiPointLineBuffers[bufferStartIndex + 1 + currentBufferSize] = encodedCoordinateToAddToBuffer;

					Debug.Assert(currentBufferSize + 1 < multiPointLineBufferWidth, "Buffer size overflow detected");

					multiPointLineBuffers[bufferStartIndex] = (ushort) ++currentBufferSize;

					// No need to check for buffers prior to this one, as the they didn't contain either of the two coordinates anyways.
					// Only newly added coordinate can result in a merge... AND ONLY 1.
					var buffersApplicableForMerge = multiPointLineBuffers.Slice(bufferStartIndex, (multiPointLineBuffersSize - multiPointLineBufferIndex) * multiPointLineBufferWidth);
					MergeMultiPointBuffersIfApplicable(
						ref buffersApplicableForMerge,
						ref multiPointLineBuffersSize,
						encodedCoordinateToAddToBuffer,
						multiPointLineBufferWidth);
					if (multiPointLineBuffers[0] == unorderedPoints.Length)
					{
						goto finish;
					}
					goto endCurrentIteration;
				}
			}

			Debug.WriteLine($"Coordinates [{shortest.Start}, {shortest.End}] weren't part of an existing MultiPointLine, creating new MultiPointLine3D");
			var newBufferStartIndex = multiPointLineBuffersSize * multiPointLineBufferWidth;
			multiPointLineBuffers[newBufferStartIndex] = 2;
			multiPointLineBuffers[newBufferStartIndex + 1] = startCoordinateId;
			multiPointLineBuffers[newBufferStartIndex + 2] = endCoordinateId;

			++multiPointLineBuffersSize;

			endCurrentIteration: ;

			DebuggerMethod(multiPointLineBuffers, multiPointLineBufferWidth);

			// Sort in new
			var newNearest = kdTree3.FindNearest(shortest.Start, shortest.DistanceSquared);
			while (orderedDistances.Contains(newNearest))
			{
				Debug.WriteLine($"Skipping {newNearest} due to already covered by opposite connection");
				newNearest = kdTree3.FindNearest(newNearest.Start, newNearest.DistanceSquared);
			}

			var j = 0;
			for (; j < orderedDistances.Length - 1; j++)
			{
				if (newNearest.DistanceSquared < orderedDistances[j + 1].DistanceSquared)
				{
					break;
				}
			}

			orderedDistances.Slice(1, j).CopyTo(orderedDistances);
			orderedDistances[j] = newNearest;
		}

		finish: ;
		return (ulong) unorderedPoints[shortest.Start].X * (ulong) unorderedPoints[shortest.End].X;
	}

	private static void MergeMultiPointBuffersIfApplicable(ref Span<ushort> multiPointLineBuffers, ref int multiPointLineBuffersSize, ushort newlyAddedEncodedCoordinate, int maxBufferWidth)
	{
		var referenceSize = (int) multiPointLineBuffers[0];
		// Start offset is incremented by 1 to offset the bufferSize value
		// Start offset is modified for the following reasons:
		// 1. Increased by the bufferSize value to account for the currently stored values
		// 2. Increased by 1 to actually account for the position occupied by the bufferSize value
		// 3. Decreased by 1 (offsetting the prior reduction) again as the newly added coordinate is already part of the buffer that's going to be merged in and will thus be overwritten
		// Max buffer length modified for the following reasons:
		// 1. Reduced by the current bufferSize value; accounts for the currently stored values
		// 2. Reduced by 1 to actually account for the position occupied by the bufferSize value
		// 3. Increased by 1 (offsetting the prior reduction) again as the newly added coordinate is already part of the buffer that's going to be merged in and will thus be overwritten
		var referenceBuffer = multiPointLineBuffers.Slice(referenceSize, maxBufferWidth - referenceSize);

		for (var i = maxBufferWidth; i < multiPointLineBuffers.Length; i += maxBufferWidth)
		{
			var currentSliceSize = (int) multiPointLineBuffers[i];

			var currentSlice = multiPointLineBuffers.Slice(i + 1, currentSliceSize);
			var currentSliceContainsNewCoordinate = currentSlice.Contains(newlyAddedEncodedCoordinate);
			if (currentSliceContainsNewCoordinate)
			{
				Debug.WriteLine("Merging MultiPointLineBuffers");

				// Copy over the current slice to the end of the reference buffer (excluding the size of the current slice)
				currentSlice.CopyTo(referenceBuffer);
				multiPointLineBuffers[0] = (ushort) (referenceSize - 1 + currentSliceSize);

				// Shift all subsequent slices down by the size of the current slice
				multiPointLineBuffers.Slice(i + maxBufferWidth).CopyTo(multiPointLineBuffers.Slice(i));
				--multiPointLineBuffersSize;

				return;
			}
		}

		Debug.WriteLine("No MultiPointLineBuffers to merge");
	}

	[Conditional("DEBUG")]
	private static void DebuggerMethod(Span<ushort> multiPointLineBuffers, int maxBufferWidth)
	{
		for (var i = 0; i < multiPointLineBuffers.Length; i += maxBufferWidth)
		{
			var bufferSize = (int) multiPointLineBuffers[i];
			Debug.Assert(bufferSize <= maxBufferWidth, "Buffer size overflow detected");
		}

		var groups = new List<ushort[]>();
		for (var i = 0; i < multiPointLineBuffers.Length; i += maxBufferWidth)
		{
			var bufferSize = (int) multiPointLineBuffers[i];
			var buffer = multiPointLineBuffers.Slice(i + 1, bufferSize);
			buffer.Sort();
			groups.Add(buffer.ToArray());
		}

		Debug.WriteLine($"-> {string.Join(", ", groups.Where(x => x.Length > 0).OrderByDescending(x => x.Length).Select(x => '[' + string.Join(", ", x) + ']'))}");
	}
}

/// <summary>
/// Apparently this actually called a k-d tree, but I only learned this after I had implemented it.
/// This is a space-partitioned data structure that allows for efficient lookup of the nearest neighbor of a given point.
/// Because of the additional constraint on the FindNearest method, it's possible to search for the nearest neighbor of a point given a minimum distance that has to be maintained.
/// </summary>
/// <see href="https://en.wikipedia.org/wiki/K-d_tree">Wikipedia K-d tree</see>
readonly file ref struct VolumetricTreeAllocFree
{
	private static readonly Comparison<Point3D> XComparer = (coordinateA, coordinateB) => coordinateA.X - coordinateB.X;
	private static readonly Comparison<Point3D> YComparer = (coordinateA, coordinateB) => coordinateA.Y - coordinateB.Y;
	private static readonly Comparison<Point3D> ZComparer = (coordinateA, coordinateB) => coordinateA.Z - coordinateB.Z;

	// Encoding of a tree node is the following:
	// We'll use 2 slots per tree node, either index of / 2 indicates the point id. Works by (ab)using integer division not yielding floating point numbers.
	// First slot is an encoding off all axis values offset by 17 (works bc max number is 100_000, which requires 17 bits), thus x value is not offsetted, y is offsetted by 17 and z is offstted by 34
	// For the second slot, the first 2 bits will describe the axis for comparison, following by 11 bits for the id of the left node and another 11 bits for the id of the right node.

	// Second approach:
	// I figured that, aside from being incredibly hard to debug, encoding multiple numbers would probably incur a higher performance penalty compared to just reserving a slot per number required.
	// No encoding, but we reserve 6 subsequent indices per node
	// i / 6 is the id of the point
	// At index i: x-value of the point
	// At index i + 1: y-value of the point
	// At index i + 2: z-value of the point
	// At index i + 3: the axis id, used for comparison
	// At index i + 4: the index of the left child node (divide by 6 to get the point id)
	// At index i + 5: the index of the right child node (divide by 6 to get the point id)
	private readonly Span<int> _encodedTreeNodes;
	private readonly ushort _encodedRootNodeId;

	public VolumetricTreeAllocFree(in Span<int> encoded, in Span<Point3D> points)
	{
		Debug.Assert(encoded.Length >= points.Length * 6, "Encoded buffer too small for given number of points");
		_encodedTreeNodes = encoded;

		_encodedRootNodeId = Build(ref _encodedTreeNodes, points);
	}

	private static ushort Build(ref Span<int> encodedTreeNodes, scoped Span<Point3D> points, int depth = 0)
	{
		if (points.Length == 0)
		{
			return ushort.MaxValue;
		}

		var axis = depth % 3;
		var comparer = axis switch
		{
			0 => XComparer,
			1 => YComparer,
			_ => ZComparer
		};

		points.Sort(comparer);

		var midPointIndex = points.Length / 2;

		var point = points[midPointIndex];

		var leftChildTreeNodeIndex = Build(ref encodedTreeNodes, points.Slice(0, midPointIndex), depth + 1);
		var rightChildTreeNodeIndex = Build(ref encodedTreeNodes, points.Slice(midPointIndex + 1), depth + 1);

		var nodeIndex = (ushort) (point.Id * 6);
		encodedTreeNodes[nodeIndex] = point.X;
		encodedTreeNodes[nodeIndex + 1] = point.Y;
		encodedTreeNodes[nodeIndex + 2] = point.Z;
		encodedTreeNodes[nodeIndex + 3] = axis;
		encodedTreeNodes[nodeIndex + 4] = leftChildTreeNodeIndex;
		encodedTreeNodes[nodeIndex + 5] = rightChildTreeNodeIndex;

		return point.Id;
	}

	public Line3D FindNearest(ushort targetId, long minimumDistanceSquared)
	{
		var bestPointId = ushort.MaxValue;
		var bestDistanceSquared = long.MaxValue;

		var targetNodeId = targetId * 6;

		var targetX = _encodedTreeNodes[targetNodeId];
		var targetY = _encodedTreeNodes[targetNodeId + 1];
		var targetZ = _encodedTreeNodes[targetNodeId + 2];;

		Search(_encodedTreeNodes, _encodedRootNodeId);
		return new(targetId, bestPointId, bestDistanceSquared);

		void Search(scoped Span<int> encodedTreeNodes, ushort nodeId)
		{
			Debug.WriteLine($"-- VolumetricTreeAllocFree: NodeId: {nodeId}");
			if (nodeId == ushort.MaxValue)
			{
				return;
			}

			var nodeIndex = nodeId * 6;

			scoped var nodeSlice = encodedTreeNodes.Slice(nodeIndex, 6);

			var nodeX = nodeSlice[0];
			var nodeY = nodeSlice[1];
			var nodeZ = nodeSlice[2];

			long dx = targetX - nodeX;
			long dy = targetY - nodeY;
			long dz = targetZ - nodeZ;

			var currentDistanceSquared = dx * dx + dy * dy + dz * dz;
			if (currentDistanceSquared > minimumDistanceSquared && currentDistanceSquared < bestDistanceSquared)
			{
				bestDistanceSquared = currentDistanceSquared;
				bestPointId = nodeId;
			}

			var nodeAxis = nodeSlice[3];
			var nodeLeftChildIndex = (ushort) nodeSlice[4];
			var nodeRightChildIndex = (ushort) nodeSlice[5];

			Debug.WriteLine($"-- VolumetricTreeAllocFree: Left NodeId: {nodeLeftChildIndex} | Right NodeId: {nodeRightChildIndex}");

			var diff = nodeAxis switch
			{
				0 => dx,
				1 => dy,
				_ => dz
			};

			Debug.WriteLine($"-- VolumetricTreeAllocFree: Axis: {nodeAxis} | Diff: {diff}");

			// determine which child node to search next
			var first = diff < 0 ? nodeLeftChildIndex : nodeRightChildIndex;
			Search(encodedTreeNodes, first);

			// if the best distance found so far could be improved by searching the other side of the tree, do so
			if (diff * diff < bestDistanceSquared)
			{
				var second = diff < 0 ? nodeRightChildIndex : nodeLeftChildIndex;
				Search(encodedTreeNodes, second);
			}
		}
	}
}

readonly file record struct Line3D(ushort Start, ushort End, long DistanceSquared) : IComparable<Line3D>
{
	public int CompareTo(Line3D other)
	{
		return DistanceSquared.CompareTo(other.DistanceSquared);
	}

	public bool Equals(Line3D other)
	{
		return Start.Equals(other.Start) && End.Equals(other.End) || Start.Equals(other.End) && End.Equals(other.Start);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Start, End);
	}

	public override string ToString()
	{
		return $"{Start} -> {End}, distance={Math.Sqrt(DistanceSquared)}";
	}
}

readonly file struct Point3D : IEquatable<Point3D>
{
	public readonly ushort Id;

	public readonly int X;
	public readonly int Y;
	public readonly int Z;

	public Point3D(ushort id, int x, int y, int z)
	{
		Id = id;

		X = x;
		Y = y;
		Z = z;
	}

#if DEBUG
	public long DistanceSquared(Point3D other)
	{
		long dx = X - other.X;
		long dy = Y - other.Y;
		long dz = Z - other.Z;
		return dx * dx + dy * dy + dz * dz;
	}
#endif

	public override string ToString() => $"Id={Id} ({X}, {Y}, {Z})";

	public bool Equals(Point3D other)
	{
		return Id == other.Id;
	}

	public override bool Equals(object? obj)
	{
		return obj is Point3D other && Equals(other);
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}

	public static bool operator ==(Point3D left, Point3D right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Point3D left, Point3D right)
	{
		return !left.Equals(right);
	}
}