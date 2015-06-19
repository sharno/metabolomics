using System;

namespace PathwaysLib.Utilities
{
	/// <sourcefile>
	///		<project>PathwaysLib</project>
	///		<filepath>PathwaysLib/HypergeometicDistribution.cs</filepath>
	///		<creation>2005/02/23</creation>
	///		<author>
	///			<name>Marc R. Reynolds</name>
	///			<initials>mrr</initials>
	///			<email>marc.reynolds@cwru.edu</email>
	///		</author>
	///		<cvs>
	///			<cvs_author>$Author: mustafa $</cvs_author>
	///			<cvs_date>$Date: 2008/05/16 21:15:58 $</cvs_date>
	///			<cvs_header>$Header: /var/lib/cvs/PathCase_SystemsBiology/PathwaysLib/Utilities/HypergeometricDistribution.cs,v 1.1 2008/05/16 21:15:58 mustafa Exp $</cvs_header>
	///			<cvs_branch>$Name:  $</cvs_branch>
	///			<cvs_revision>$Revision: 1.1 $</cvs_revision>
	///		</cvs>
	///</sourcefile>
	/// <summary>
	/// Class which computes the hypergeometric distribution from given parameters
	/// </summary>
	public class HypergeometricDistribution
	{
		/// <summary>
		/// Computes the hypergeometric distribution from given parameters
		/// </summary>
		/// <param name="sampleSize">The sample size</param>
		/// <param name="sampleSuccesses">The number of successful tests in the sample</param>
		/// <param name="populationSize">The population size</param>
		/// <param name="populationSuccesses">The number of successful tests in the population</param>
		/// <returns>The P-value from the given parameters</returns>
		/// <remarks>
		/// This libarary currently uses Spreadsheet Gear, which seems to choke on a couple cases of computing the
		/// Hypergeometric Distribution, specifically when 'sample successes' equals 'population successes'
		/// </remarks>
		public static double Evaluate(int sampleSize, int sampleSuccesses, int populationSize, int populationSuccesses)
		{
			//			IWorkbook book = Factory.GetWorkbook();
			//			IWorksheet sheet = book.Worksheets["Sheet1"];
			//			IRange a1 = sheet.Cells["A1"];
			//
			//			// Set a formula.
			//
			//			a1.Formula = string.Format("=HYPGEOMDIST({0},{1},{2},{3})", sampleSuccesses, sampleSize, populationSuccesses, populationSize);
			//			if(!(a1.Value is double)) throw new Exception("Error while computing Hypergeometric Distribution");
			//			return (double)a1.Value;

			// (GJS) Got rid of the SpreadsheetGear dependency

			// Perform error-checks...
			if( sampleSuccesses < 0 ||
				sampleSuccesses > Math.Min(sampleSize, populationSuccesses) ||
				sampleSuccesses < Math.Max(0, sampleSize - populationSize + populationSuccesses) ||
				sampleSize < 0 ||
				populationSuccesses > populationSize ||
				populationSize < 0 )
				//throw new Exception("Error while computing hypergeometric distribution: the arguments are incorrect. sampleSize: " +  + "{0}, sampleSuccess: {1}, populationSize: {2}, populationSuccess: {3}", sampleSize, sampleSuccesses, populationSize, populationSuccesses);
				throw new Exception("Error while computing hypergeometric distribution: the arguments are incorrect. sampleSize: " + sampleSize + ", sampleSuccess: " + sampleSuccesses +", populationSize: " + populationSize + ", populationSuccess: " + populationSuccesses);
            
			// Apply the formula
            double pValue = 0;
            for (int i = sampleSuccesses; i <= sampleSize && i <= populationSuccesses; i++ )
                pValue += Combination(populationSuccesses, i) * Combination(populationSize - populationSuccesses, sampleSize - i) /
                            Combination(populationSize, sampleSize);
            return pValue;
		}

		private static double Combination(int n, int r)
		{
            return CombinationSolver.Combination(n, r);

            ///--------------------------------------------------

			BigInteger a = new BigInteger( FactorialSolver.Factorial(n), 10 );
			BigInteger b = new BigInteger( FactorialSolver.Factorial(r), 10 );
			BigInteger c = new BigInteger( FactorialSolver.Factorial(n-r), 10 );

			BigInteger d = a / ( b * c );

			return double.Parse( d.ToString() );
		}
	}

    public class CombinationSolver
    {
        /// <summary>
        /// Default constructor; not useful.
        /// </summary>
        public CombinationSolver() { }

        private static long N;

        /// <summary>
        /// Quickly computes the factorial of a number.
        /// </summary>
        /// <param name="n">An integer</param>
        /// <returns>n!</returns>
        public static double Combination(int n, int r)
        {
            if (n < 0)
            {
                throw new System.ArgumentException(
                    ": n >= 0 required, but was " + n);
            }

            if (n < r)
            {
                throw new System.ArgumentException("Exception : n >= r required, but was " + n + " and r was " + r);
            }

            if (n < 2) return 1;
            if (n == r) return 1;

            BigInteger a = 1;
            int z = Math.Max(r, n - r);
            int y = Math.Min(r, n - r);
            for (int k = n; k > z; k--)
                a *= k; 
            BigInteger denom = new BigInteger(FactorialSolver.Factorial(y), 10);
            BigInteger result = a / denom;
            return double.Parse(result.ToString());
        }

        private static DecInteger Product(int n)
        {
            int m = n / 2;
            if (m == 0) return new DecInteger(N += 2);
            if (n == 2) return new DecInteger((N += 2) * (N += 2));
            return Product(n - m) * Product(m);
        }
    }

	// This fast factorial algorithm was developed by Perer Luschny
	// FastFactorialFunctions: The Homepage of Factorial Algorithms
	// http://www.luschny.de/math/factorial/FastFactorialFunctions.htm
	
	/// <summary>
	/// Solves factorials.
	/// </summary>
    /// 

	public class FactorialSolver
	{
		/// <summary>
		/// Default constructor; not useful.
		/// </summary>
		public FactorialSolver() {}

		private static long N;

		/// <summary>
		/// Quickly computes the factorial of a number.
		/// </summary>
		/// <param name="n">An integer</param>
		/// <returns>n!</returns>
		public static string Factorial(int n)
		{
			if (n < 0)
			{
				throw new System.ArgumentException(
					": n >= 0 required, but was " + n);
			}

			if (n < 2) return "1";

			DecInteger p = new DecInteger(1);
			DecInteger r = new DecInteger(1);

			N = 1;

			int h = 0, shift = 0, high = 1;
			int log2n = (int)System.Math.Floor(System.Math.Log(n) * 1.4426950408889634);

			while (h != n)
			{
				shift += h;
				h = n >> log2n--;
				int len = high;
				high = (h & 1) == 1 ? h : h - 1;
				len = (high - len) / 2;

				if (len > 0)
				{
					p = p * Product(len);
					r = r * p;
				}
			}

			r = r * DecInteger.Pow2(shift);
			return r.ToString();
		}

		private static DecInteger Product(int n)
		{
			int m = n / 2;
			if (m == 0) return new DecInteger(N += 2);
			if (n == 2) return new DecInteger((N += 2) * (N += 2));
			return Product(n - m) * Product(m);
		}
	}

	class DecInteger
	{
		private int[] digits;
		private int digitsLength;
		private const long mod = 100000000L;

		public DecInteger(long value)
		{
			digits = new int[] { (int)value, (int)(value >> 32) };
			digitsLength = 2;
		}

		private DecInteger(int[] digits, int length)
		{
			this.digits = digits;
			digitsLength = length;
		}

		static public DecInteger Pow2(int e)
		{
			if (e < 31) return new DecInteger((int)System.Math.Pow(2, e));
			return Pow2(e / 2) * Pow2(e - e / 2);
		}

		static public DecInteger operator *(DecInteger a, DecInteger b)
		{
			int alen = a.digitsLength, blen = b.digitsLength;
			int clen = alen + blen + 1;
			int[] digits = new int[clen];

			for (int i = 0; i < alen; i++)
			{
				long temp = 0;
				for (int j = 0; j < blen; j++)
				{
					temp = temp + ((long)a.digits[i] * (long)b.digits[j]) + digits[i + j];
					digits[i + j] = (int)(temp % mod);
					temp = temp / mod;
				}
				digits[i + blen] = (int)temp;
			}

			int k = clen - 1;
			while (digits[k] == 0) k--;

			return new DecInteger(digits, k + 1);
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(digitsLength * 10);
			sb = sb.Append(digits[digitsLength - 1]);
			for (int j = digitsLength - 2; j >= 0; j--)
			{
				sb = sb.Append((digits[j] + (int)mod).ToString().Substring(1));
			}
			return sb.ToString();
		}
	}
}