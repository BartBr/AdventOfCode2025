using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Jens;

public class Day09 : HappyPuzzleBase<ulong, ulong>
{
	public override ulong SolvePart1(Input input)
	{
		ulong maxSurfaceArea = 0;

		scoped Span<Point2D> pointBuffer = stackalloc Point2D[input.Lines.Length];

		for (var i = 0; i < pointBuffer.Length; i++)
		{
			var point = Point2D.Parse(input.Lines[i]);

			for (var j = 0; j < i - 1; j++)
			{
				var pointB = pointBuffer[j];
				var surfaceArea = point.CalculateSurfaceArea(pointB);
				if (surfaceArea > maxSurfaceArea)
				{
					maxSurfaceArea = surfaceArea;
				}
			}

			pointBuffer[i] = point;
		}

		return maxSurfaceArea;
	}

	public override ulong SolvePart2(Input input)
	{
		scoped Span<Point2D> pointBuffer = stackalloc Point2D[input.Lines.Length];
		scoped Span<Line2D> edgeBuffer = stackalloc Line2D[pointBuffer.Length];

		// Calculate initial point so we can assign the edges in the same loop
		var previousPoint = pointBuffer[0] = Point2D.Parse(input.Lines[0]);

		for (var i = 1; i < pointBuffer.Length; i++)
		{
			var currentPoint = Point2D.Parse(input.Lines[i]);

			pointBuffer[i] = currentPoint;

			// Premature optimization to reduce needing to figure this out later on in the algorithm on every single iteration...
			if (previousPoint.X > currentPoint.X || previousPoint.Y > currentPoint.Y)
			{
				// Flip previous and current so that the line is always aligned from bottom-left to top-right (assuming coordinates only consisting of positive integers)
				edgeBuffer[i - 1] = new Line2D(currentPoint, previousPoint);
			}
			else
			{
				edgeBuffer[i - 1] = new Line2D(previousPoint, currentPoint);
			}

			previousPoint = currentPoint;
		}

		// Add closing edge between last and first point
		var currentPointFinal = pointBuffer[0];
		if (previousPoint.X > currentPointFinal.X || previousPoint.Y > currentPointFinal.Y)
		{
			edgeBuffer[pointBuffer.Length - 1] = new Line2D(currentPointFinal, previousPoint);
		}
		else
		{
			edgeBuffer[pointBuffer.Length - 1] = new Line2D(previousPoint, currentPointFinal);
		}


		edgeBuffer.Sort();

		ulong maxSurfaceArea = 0;

		for (var i = 0; i < pointBuffer.Length - 1; i++)
		{
			var referenceStartingPoint = pointBuffer[i];
			for (var j = i + 1; j < pointBuffer.Length; j++)
			{
				var referenceEndingPoint = pointBuffer[j];

				int minX, minY, maxX, maxY;
				if (referenceStartingPoint.X <  referenceEndingPoint.X)
				{
					minX = referenceStartingPoint.X;
					maxX = referenceEndingPoint.X;
				}
				else
				{
					minX = referenceEndingPoint.X;
					maxX = referenceStartingPoint.X;
				}

				if (referenceStartingPoint.Y <  referenceEndingPoint.Y)
				{
					minY = referenceStartingPoint.Y;
					maxY = referenceEndingPoint.Y;
				}
				else
				{
					minY = referenceEndingPoint.Y;
					maxY = referenceStartingPoint.Y;
				}

				// Create bottom-left and top-right aligned points for rectangle
				var minPoint = new Point2D(minX, minY);
				var maxPoint = new Point2D(maxX, maxY);

				var intersects = CheckIfIntersectsWithEdges(edgeBuffer, minPoint, maxPoint);

				if (!intersects)
				{
					var surfaceArea = referenceStartingPoint.CalculateSurfaceArea(referenceEndingPoint);
					if (surfaceArea > maxSurfaceArea)
					{
						maxSurfaceArea = surfaceArea;
					}
				}
			}
		}

		return maxSurfaceArea;

		// Simple check to see whether our axis aligned bounding box intersects with any of the pre-computed edges
		// Assumes that start is bottom-left and end is top-right aligned, as such it might be necessary to flip the points before calling this method
		// It also assumes that all edges are aligned from bottom-left to top-right as well
		static bool CheckIfIntersectsWithEdges(scoped in ReadOnlySpan<Line2D> edgeBuffer, in Point2D start, in Point2D end)
		{
			foreach (var edge in edgeBuffer)
			{
				if (start.X < edge.End.X && end.X > edge.Start.X && start.Y < edge.End.Y && end.Y > edge.Start.Y)
				{
					return true;
				}

				// No possible intersection, check next edge
			}

			// No intersections found
			return false;
		}
	}

}

readonly file record struct Point2D(int X, int Y)
{
	public static Point2D Parse(in ReadOnlySpan<char> line)
	{
		var currentX = 0;
		var currentY = 0;

		// Parse coordinates
		for (var i = 0; i < line.Length; i++)
		{
			ref var target = ref currentX;

			for (; i < line.Length; i++)
			{
				var c = line[i];

				if (c == ',')
				{
					target = ref currentY;
				}

#if DEBUG
				else if (c is >= '0' and <= '9')
				{
					target = target * 10 + (c - '0');
				}
				else
				{
					System.Diagnostics.Debug.Fail("Unexpected character while parsing Point2D coordinates");
				}
#else
				else
				{
					target = target * 10 + (c - '0');
				}
#endif
			}
		}

		return new Point2D(currentX, currentY);
	}

	public ulong CalculateSurfaceArea(Point2D other) => (ulong) (Math.Abs(X - other.X) + 1) * (ulong) (Math.Abs(Y - other.Y) + 1);
}

readonly file record struct Line2D : IComparable<Line2D>
{
	public Line2D(Point2D start, Point2D end)
	{
		Start = start;
		End = end;
		ManhattenDistance = CalculateManhattenDistance(Start, End);
	}

	public readonly Point2D Start;
	public readonly Point2D End;
	public readonly ulong ManhattenDistance;

	private static ulong CalculateManhattenDistance(Point2D start, Point2D end) => (ulong)(Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y));

	// Sort by longest Manhatten distance first, longer distances tend to interfere with more other lines
	public int CompareTo(Line2D other)
	{
		// Other before this if other distance is larger
		return other.ManhattenDistance.CompareTo(ManhattenDistance);
	}
}