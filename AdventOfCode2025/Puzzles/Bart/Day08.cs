using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AdventOfCode2025.Common;

namespace AdventOfCode2025.Puzzles.Bart;

[SuppressMessage("ReSharper", "EnforceIfStatementBraces")]
[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public class Day08 : HappyPuzzleBase<int, long>
{

	public override int SolvePart1(Input input)
	{
		var amountOfJunctionBoxes = input.Lines.Length;
		scoped Span<JunctionBox> junctionBoxes = stackalloc JunctionBox[amountOfJunctionBoxes];

		for (var i = 0; i < input.Lines.Length; i++)
		{
			junctionBoxes[i] = ReadJunctionBox(input.Lines[i]);
		}

		var amountOfDistances = ((amountOfJunctionBoxes -1) * amountOfJunctionBoxes) / 2;
		scoped Span<Distance> distanceSpan = stackalloc Distance[amountOfDistances];

		var distanceIndex = 0;
		for (var i = 0; i < amountOfJunctionBoxes-1; i++)
		{
			for (var j = i + 1; j < amountOfJunctionBoxes; j++)
			{
				var jbA = junctionBoxes[i];
				var jbB = junctionBoxes[j];
				var distance = JunctionBox.CalculateDistance(jbA, jbB);
				distanceSpan[distanceIndex++] = new Distance(i, j, distance);
			}
		}
		//Sort distances
		distanceSpan.Sort();

		// create a circuit number for every junctionbox. later i merge those numbers
		// example
		// [0] = 2    => 2
		// [5] = 4    => 2
		// after merge they both have circuit number 2

		scoped Span<int> junctionBoxPartOfCircuit = stackalloc int[amountOfJunctionBoxes];
		for (var i = 0; i < input.Lines.Length; i++)
		{
			junctionBoxPartOfCircuit[i] = i;
		}


		distanceIndex = 0;

		var amountOfDistancesToDo = input.Lines.Length == 20 ? 10 : 1000;

		while(distanceIndex < amountOfDistancesToDo)
		{
			var distance = distanceSpan[distanceIndex];
			var circuitA = junctionBoxPartOfCircuit[distance.JunctionAIndex];
			var circuitB = junctionBoxPartOfCircuit[distance.JunctionBIndex];

			if (circuitA != circuitB)
			{
				Debug.WriteLine($"Merging {junctionBoxes[distance.JunctionAIndex]} {junctionBoxes[distance.JunctionBIndex]}");
				//Merge the 2 circuits
				MergeCircuits(circuitA, circuitB, ref junctionBoxPartOfCircuit);
			}

			distanceIndex++;
		}

		scoped Span<int> amountOfJunctionsInCircuit =  stackalloc int[amountOfJunctionBoxes];
		amountOfJunctionsInCircuit.Fill(0);
		for (var i = 0; i < amountOfJunctionBoxes; i++)
		{
			amountOfJunctionsInCircuit[junctionBoxPartOfCircuit[i]]++;
		}
		amountOfJunctionsInCircuit.Sort();
		amountOfJunctionsInCircuit = amountOfJunctionsInCircuit[^3..];

		var sum = 1;
		foreach (var a in amountOfJunctionsInCircuit)
		{
			Debug.WriteLine( $" {a} " );
			sum *= a;
		}

		return sum;
	}

	private static void MergeCircuits(int circuitA, int circuitB, ref Span<int> junctionPartOfCircuit)
	{
		for (var i = 0; i < junctionPartOfCircuit.Length; i++)
		{
			if (junctionPartOfCircuit[i] == circuitB)
				junctionPartOfCircuit[i] = circuitA;
		}
	}

	private static JunctionBox ReadJunctionBox(string input)
	{
		var i = 0;
		var x = 0;
		while (input[i] != ',')
		{
			var digit = input[i] - '0';
			x = x * 10 + digit;
			i++;
		}

		i++;//skip ,
		var y = 0;
		while (input[i] != ',')
		{
			var digit = input[i] - '0';
			y = y * 10 + digit;
			i++;
		}

		i++;//skip ,
		var z = 0;
		while (i < input.Length)
		{
			var digit = input[i] - '0';
			z = z * 10 + digit;
			i++;
		}

		return new JunctionBox(x,y,z);
	}

	private readonly record struct JunctionBox(int X, int Y, int Z): IComparable<JunctionBox>
	{
		public static long CalculateDistance(JunctionBox a, JunctionBox b)
		{
			// below is not required. largest math sqrt is largest at all
			//return Math.Sqrt(
			return (
				((long)b.X - a.X) * ((long)b.X - a.X)
				+ ((long)b.Y - a.Y) * ((long)b.Y - a.Y)
				+ ((long)b.Z - a.Z) * ((long)b.Z - a.Z));
		}

		public override string ToString()
		{
			return $"({X:000},{Y:000},{Z:000})";
		}

		public int CompareTo(JunctionBox other)
		{
			return (X + Y + Z).CompareTo(other.X +  other.Y + other.Z);
		}
	}

	private readonly record struct Distance(int JunctionAIndex, int JunctionBIndex, long Value): IComparable<Distance>
	{
		public int CompareTo(Distance other)
		{
			return Value.CompareTo(other.Value);
		}
	}

	public override long SolvePart2(Input input)
	{
		var amountOfJunctionBoxes = input.Lines.Length;
		scoped Span<JunctionBox> junctionBoxes = stackalloc JunctionBox[amountOfJunctionBoxes];

		for (var i = 0; i < input.Lines.Length; i++)
		{
			junctionBoxes[i] = ReadJunctionBox(input.Lines[i]);
		}

		var amountOfDistances = ((amountOfJunctionBoxes -1) * amountOfJunctionBoxes) / 2;
		scoped Span<Distance> distanceSpan = stackalloc Distance[amountOfDistances];

		var distanceIndex = 0;
		for (var i = 0; i < amountOfJunctionBoxes-1; i++)
		{
			for (var j = i + 1; j < amountOfJunctionBoxes; j++)
			{
				var jbA = junctionBoxes[i];
				var jbB = junctionBoxes[j];
				var distance = JunctionBox.CalculateDistance(jbA, jbB);
				distanceSpan[distanceIndex++] = new Distance(i, j, distance);
			}
		}
		//Sort distances
		distanceSpan.Sort();

		// create a circuit number for every junctionbox. later i merge those numbers
		// example
		// [0] = 2    => 2
		// [5] = 4    => 2
		// after merge they both have circuit number 2

		scoped Span<int> junctionBoxPartOfCircuit = stackalloc int[amountOfJunctionBoxes];
		for (var i = 0; i < input.Lines.Length; i++)
		{
			junctionBoxPartOfCircuit[i] = i;
		}


		distanceIndex = 0;
		var merged = 0;


		while(merged < amountOfJunctionBoxes -1)
		{
			var distance = distanceSpan[distanceIndex];
			var circuitA = junctionBoxPartOfCircuit[distance.JunctionAIndex];
			var circuitB = junctionBoxPartOfCircuit[distance.JunctionBIndex];

			if (circuitA != circuitB)
			{
				//Merge the 2 circuits
				MergeCircuits(circuitA, circuitB, ref junctionBoxPartOfCircuit);
				merged++;
			}

			distanceIndex++;
		}

		var lastJoinedDistance = distanceSpan[distanceIndex - 1];
		long x1 = junctionBoxes[lastJoinedDistance.JunctionAIndex].X;
		long x2 = junctionBoxes[lastJoinedDistance.JunctionBIndex].X;
		return x1 * x2;
	}
}
