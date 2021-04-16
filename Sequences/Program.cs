using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sequences
{
	// ReSharper disable once ClassNeverInstantiated.Global
	// ReSharper disable once ArrangeTypeModifiers
	class Program
	{
		// ReSharper disable once InconsistentNaming
		private const double TOLERANCE = 0.001;
		
		private static void Main(string[] args)
		{
			// this stuff was for my testing
			/*
			// Linear
			// minimum required values: 2
			Sequence(new double[] {6, 7, 8, 9}, out _);
			// quadratic
			// minimum required values: 4
			Sequence(new double[] {4, 9, 16, 25}, out _);
			// cubic
			// minimum required values: 5
			Sequence(new double[] {4, 14, 40, 88, 164}, out _);
			// geometric sequence
			// minimum required values: 3
			Sequence(new double[] {110,220,440}, out _);
			
			// same value multiple times
			Sequence(new double[] {6, 6, 6, 6}, out _);
			// x^2
			Sequence(new double[] {1, 4, 9, 16}, out _);
			*/

			Console.WriteLine("Enter a sequence separated by commas");
			var input    = Console.ReadLine()?.Split(',').Select(double.Parse).ToArray();
			var sequence = Sequence(input, out var seqType);
			switch (seqType.Name)
			{
				case "LinearSequence":
					PrintLinear(sequence);
					break;
				case "QuadraticSequence":
					PrintQuadratic(sequence);
					break;
				case "CubicSequence":
					PrintCubic(sequence);
					break;
				case "GeometricSequence":
					PrintGeometric(sequence);
					break;
			}
		}

		private static void PrintLinear(Sequence sequence)
		{
			var seq = (LinearSequence) sequence;
			var a = Math.Abs(seq.Coefficient - 1) > TOLERANCE
				? seq.Coefficient.ToString(CultureInfo.InvariantCulture)
				: string.Empty;
			var b = seq.Offset != 0 ? "+" + seq.Offset : string.Empty;
			Console.WriteLine($"nth term: {a}n{b}".Replace("+-", "-"));
		}

		private static void PrintQuadratic(Sequence sequence)
		{
			var seq = (QuadraticSequence) sequence;
			var a = Math.Abs(seq.Coefficient - 1) > TOLERANCE
				? seq.Coefficient.ToString(CultureInfo.InvariantCulture)
				: string.Empty;
			var b = seq.Offset.Coefficient != 0 ? "+" + seq.Offset.Coefficient + "n" : string.Empty;
			var c = seq.Offset.Offset      != 0 ? "+" + seq.Offset.Offset : string.Empty;
			Console.WriteLine($"nth term: {a}n²{b}{c}".Replace("+-", "-"));
		}

		private static void PrintCubic(Sequence sequence)
		{
			var seq = (CubicSequence) sequence;
			var a = Math.Abs(seq.Coefficient - 1) > TOLERANCE
				? seq.Coefficient.ToString(CultureInfo.InvariantCulture)
				: string.Empty;
			var b = seq.Offset.Coefficient        != 0 ? "+" + seq.Offset.Coefficient        + "n²" : string.Empty;
			var c = seq.Offset.Offset.Coefficient != 0 ? "+" + seq.Offset.Offset.Coefficient + "n" : string.Empty;
			var d = seq.Offset.Offset.Offset      != 0 ? "+" + seq.Offset.Offset.Offset : string.Empty;
			Console.WriteLine($"nth term: {a}n³{b}{c}{d}".Replace("+-", "-"));
		}

		private static void PrintGeometric(Sequence sequence)
		{
			var seq = (GeometricSequence) sequence;
			var a = Math.Abs(seq.StartNum - 1) > TOLERANCE
				? seq.StartNum + "*"
				: string.Empty;
			var r = seq.IndexBase != 0 ? seq.IndexBase.ToString(CultureInfo.InvariantCulture) : string.Empty;
			var i = seq.IndexOffset != 0 ? seq.IndexOffset.ToString(CultureInfo.InvariantCulture) : string.Empty;
			Console.WriteLine($"nth term: {a}{r}^(n{i})");
		}

		public static Sequence Sequence(IReadOnlyList<double> sequence, out Type seqType)
		{
			if (Linear(sequence, out var linearResult))
			{
				seqType = linearResult.GetType();
				return linearResult;
			}
			
			if (Geometric(sequence, out var geometricResult))
			{
				seqType = geometricResult.GetType();
				return geometricResult;
			}

			if (Quadratic(sequence, out var quadraticResult))
			{
				seqType = quadraticResult.GetType();
				return quadraticResult;
			}
			
			if (Cubic(sequence, out var cubicResult))
			{
				seqType = cubicResult.GetType();
				return cubicResult;
			}

			throw new Exception("The sequence could not be calculated");
		}

		private static bool Linear(IReadOnlyList<double> sequence, out LinearSequence result)
		{
			result = new();
			
			var differences = new double[sequence.Count - 1];
			for (var i = 0; i < differences.Length; i++)
				differences[i] = sequence[i + 1] - sequence[i];

			if (differences.Any(d => Math.Abs(d - differences[0]) > TOLERANCE))
				return false;

			result.Coefficient = differences[0];
			result.Offset      = sequence[0] - result.Coefficient;

			return true;
		}
		
		private static bool Quadratic(IReadOnlyList<double> sequence, out QuadraticSequence result)
		{
			result = new();
			
			var firstDifferences = new double[sequence.Count - 1];
			for (var i = 0; i < firstDifferences.Length; i++)
				firstDifferences[i] = sequence[i + 1] - sequence[i];
			
			var secondDifferences = new double[firstDifferences.Length - 1];
			for (var i = 0; i < secondDifferences.Length; i++)
				secondDifferences[i] = firstDifferences[i + 1] - firstDifferences[i];
			
			if (secondDifferences.Any(d => Math.Abs(d - secondDifferences[0]) > TOLERANCE))
				return false;
			
			result.Coefficient = secondDifferences[0] / 2;

			var diffs = new double[sequence.Count];
			for (var i = 0; i < sequence.Count; i++)
				diffs[i] = sequence[i] - result.Coefficient * (i + 1) * (i + 1);
			
			return Linear(diffs, out result.Offset);
		}

		private static bool Cubic(IReadOnlyList<double> sequence, out CubicSequence result)
		{
			result = new();
			
			var firstDifferences = new double[sequence.Count - 1];
			for (var i = 0; i < firstDifferences.Length; i++)
				firstDifferences[i] = sequence[i + 1] - sequence[i];
			
			var secondDifferences = new double[firstDifferences.Length - 1];
			for (var i = 0; i < secondDifferences.Length; i++)
				secondDifferences[i] = firstDifferences[i + 1] - firstDifferences[i];
			
			var thirdDifferences = new double[secondDifferences.Length - 1];
			for (var i = 0; i < thirdDifferences.Length; i++)
				thirdDifferences[i] = secondDifferences[i + 1] - secondDifferences[i];
			
			if (thirdDifferences.Any(d => Math.Abs(d - thirdDifferences[0]) > TOLERANCE))
				return false;
			
			result.Coefficient = thirdDifferences[0] / 6;

			var diffs = new double[sequence.Count];
			for (var i = 0; i < sequence.Count; i++)
				diffs[i] = sequence[i] - result.Coefficient * (i + 1) * (i + 1) * (i + 1);
			
			return Quadratic(diffs, out result.Offset);
		}

		private static bool Geometric(IReadOnlyList<double> sequence, out GeometricSequence result)
		{
			result = new();
			
			var divs = new double[sequence.Count - 1];
			for (var i = 0; i < divs.Length; i++)
				divs[i] = sequence[i + 1] / sequence[i];
			
			if (divs.Any(d => Math.Abs(d - divs[0]) > TOLERANCE))
				return false;

			result.IndexBase = divs[0];
			result.StartNum  = sequence[0];
			result.SimplifyFully();
			return true;
		}
	}
}