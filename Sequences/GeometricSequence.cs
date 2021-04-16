namespace Sequences
{
	public class GeometricSequence : Sequence
	{
		public double StartNum    = 1;
		public double IndexBase   = 2;
		public double IndexOffset = -1;

		public void SimplifyFully()
		{
			while (!IsFullySimplified()) SimplifyOnce();
		}
		
		public void SimplifyOnce()
		{
			if (IsFullySimplified()) return;
			StartNum /= IndexBase;
			IndexOffset++;
		}

		public bool IsFullySimplified() => StartNum % IndexBase != 0;
	}
}