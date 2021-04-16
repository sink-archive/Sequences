namespace Sequences
{
	public class CubicSequence : Sequence
	{
		public double         Coefficient = 1;
		public QuadraticSequence Offset      = new() { Coefficient = 0, Offset = new() { Coefficient = 0, Offset = 0 } };
	}
}