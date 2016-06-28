using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatWalk {
	public static class Calculators {
		public static ICalculator<T> GetFor<T>() {
			if (typeof(T) == typeof(double)) {
				return (ICalculator<T>)Double;
			} else if (typeof(T) == typeof(int)) {
				return (ICalculator<T>)Int32;
			} else if (typeof(T) == typeof(float)) {
				return (ICalculator<T>)Float;
			} else if (typeof(T) == typeof(decimal)) {
				return (ICalculator<T>)Decimal;
			} else {
				throw new ArgumentException("T");
			}
		}

		public static ICalculator<double> Double {
			get {
				return DoubleCalculator.Default;
			}
		}

		public static ICalculator<int> Int32 {
			get {
				return Int32Calculator.Default;
			}
		}

		public static ICalculator<long> Long {
			get {
				return LongCalculator.Default;
			}
		}

		public static ICalculator<float> Float{
			get {
				return FloatCalculator.Default;
			}
		}

		public static ICalculator<decimal> Decimal {
			get {
				return DecimalCalculator.Default;
			}
		}

		private class DoubleCalculator : Calculator<double> {
			private static Lazy<DoubleCalculator> _Default = new Lazy<DoubleCalculator>(() => {
				return new DoubleCalculator();
			});
			public static DoubleCalculator Default {
				get {
					return _Default.Value;
				}
			}

			public override double Add(double a, double b) {
				return a + b;
			}

			public override double Divide(double a, double b) {
				return a / b;
			}

			public override double Mod(double a, double b) {
				return a % b;
			}

			public override double Multiply(double a, double b) {
				return a * b;
			}

			public override double Negate(double a) {
				return -a;
			}
		}

		private class FloatCalculator : Calculator<float> {
			private static Lazy<FloatCalculator> _Default = new Lazy<FloatCalculator>(() => {
				return new FloatCalculator();
			});
			public static FloatCalculator Default {
				get {
					return _Default.Value;
				}
			}

			public override float Add(float a, float b) {
				return a + b;
			}

			public override float Divide(float a, float b) {
				return a / b;
			}

			public override float Mod(float a, float b) {
				return a % b;
			}

			public override float Multiply(float a, float b) {
				return a * b;
			}

			public override float Negate(float a) {
				return -a;
			}
		}


		private class Int32Calculator : Calculator<int> {
			private static Lazy<Int32Calculator> _Default = new Lazy<Int32Calculator>(() => {
				return new Int32Calculator();
			});
			public static Int32Calculator Default {
				get {
					return _Default.Value;
				}
			}

			public override int Add(int a, int b) {
				return a + b;
			}

			public override int Divide(int a, int b) {
				return a / b;
			}

			public override int Mod(int a, int b) {
				return a % b;
			}

			public override int Multiply(int a, int b) {
				return a * b;
			}

			public override int Negate(int a) {
				return -a;
			}
		}

		private class LongCalculator : Calculator<long> {
			private static Lazy<LongCalculator> _Default = new Lazy<LongCalculator>(() => {
				return new LongCalculator();
			});
			public static LongCalculator Default {
				get {
					return _Default.Value;
				}
			}

			public override long Add(long a, long b) {
				return a + b;
			}

			public override long Divide(long a, long b) {
				return a / b;
			}

			public override long Mod(long a, long b) {
				return a % b;
			}

			public override long Multiply(long a, long b) {
				return a * b;
			}

			public override long Negate(long a) {
				return -a;
			}
		}


		private class DecimalCalculator : Calculator<decimal> {
			private static Lazy<DecimalCalculator> _Default = new Lazy<DecimalCalculator>(() => {
				return new DecimalCalculator();
			});
			public static DecimalCalculator Default {
				get {
					return _Default.Value;
				}
			}

			public override decimal Add(decimal a, decimal b) {
				return a + b;
			}

			public override decimal Divide(decimal a, decimal b) {
				return a / b;
			}

			public override decimal Mod(decimal a, decimal b) {
				return a % b;
			}

			public override decimal Multiply(decimal a, decimal b) {
				return a * b;
			}

			public override decimal Negate(decimal a) {
				return -a;
			}
		}

	}
}
