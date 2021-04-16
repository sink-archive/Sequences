namespace Sequences
{
	public class QuadraticSequence : Sequence
	{
		public double         Coefficient = 1;
		public LinearSequence Offset      = new() { Coefficient = 0, Offset = 0 };
	}
}