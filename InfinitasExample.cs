/*
 * This source code is under the Unlicense
 */
using System;
using System.Linq;
using System.Text;
using static Morilib.Infinitas;

namespace Morilib
{
    /// <summary>
    /// Some solutions of Structure and Implementation of Computer Programs (SICP)
    /// by C#.
    /// </summary>
    static class InfinitasExample
    {
        /// <summary>
        /// adds element of two stream.
        /// </summary>
        /// <param name="s1">stream1</param>
        /// <param name="s2">stream2</param>
        /// <returns>added stream</returns>
        static Stream<double> AddStream(Stream<double> s1, Stream<double> s2)
        {
            return Select(s1, s2, (x, y) => x + y);
        }

        /// <summary>
        /// creates a stream of partial sum.
        /// </summary>
        /// <param name="stream">input stream</param>
        /// <returns>stream of partial sum</returns>
        static Stream<double> PartialSums(this Stream<double> stream)
        {
            Stream<double> result = null;
            result = Cons(stream.Car, () => AddStream(stream.Cdr, result));

            return result;
        }

        /// <summary>
        /// creates a stream of alternating series of pi.
        /// </summary>
        /// <param name="n">denominator of alternative series of pi</param>
        /// <returns>stream of alternating series</returns>
        static Stream<double> PiSummands(double n)
        {
            return Cons(1.0 / n, () => PiSummands(n + 2).Select(x => -x));
        }

        /// <summary>
        /// Euler transformation of the partial sums of alternating series.
        /// </summary>
        /// <param name="s">stream to transform</param>
        /// <returns>transformed stream</returns>
        static Stream<double> EulerTransform(Stream<double> s)
        {
            var s0 = s.ElementAt(0);
            var s1 = s.ElementAt(1);
            var s2 = s.ElementAt(2);

            return Cons(s2 - (s2 - s1) * (s2 - s1) / (s0 - 2 * s1 + s2), () => EulerTransform(s.Cdr));
        }

        /// <summary>
        /// creates a tableau (stream of stream) by the given transform and stream.
        /// </summary>
        /// <param name="transform">transform</param>
        /// <param name="s">stream</param>
        /// <returns>tableau</returns>
        static Stream<Stream<double>> MakeTableau(Func<Stream<double>, Stream<double>> transform, Stream<double> s)
        {
            return Cons(s, () => MakeTableau(transform, transform(s)));
        }

        /// <summary>
        /// creates an accerated sequence.
        /// </summary>
        /// <param name="transform">transform</param>
        /// <param name="s">stream</param>
        /// <returns>accerated sequence</returns>
        static Stream<double> AcceratedSequence(Func<Stream<double>, Stream<double>> transform, Stream<double> s)
        {
            return MakeTableau(transform, s).Select(x => x.Car);
        }

        static Stream<double> IntegrateSeries(double count, Stream<double> series)
        {
            return Cons(series.Car / count, () => IntegrateSeries(count + 1, series.Cdr));
        }

        /// <summary>
        /// integrates the series represented by the stream.
        /// </summary>
        /// <param name="series">series represented by this stream</param>
        /// <returns>integreated series</returns>
        static Stream<double> IntegrateSeries(Stream<double> series)
        {
            return IntegrateSeries(1, series);
        }

        /// <summary>
        /// computes integral by the integrand, initial value and difference.
        /// </summary>
        /// <param name="delayedIntegrand">delayed integrand</param>
        /// <param name="initialValue">initial value</param>
        /// <param name="dt">difference</param>
        /// <returns></returns>
        static Stream<double> Integral(Delayed<Stream<double>> delayedIntegrand, double initialValue, double dt)
        {
            Stream<double> inte = null;
            inte = Cons(initialValue, () => AddStream(delayedIntegrand.Force().Select(x => x * dt), inte));
            return inte;
        }

        /// <summary>
        /// solves the equation of dy/dt = f(x) and initial value y0.
        /// </summary>
        /// <param name="f">a function</param>
        /// <param name="y0">initial value</param>
        /// <param name="dt">difference</param>
        /// <returns></returns>
        static Stream<double> Solve(Func<double, double> f, double y0, double dt)
        {
            Stream<double> dy = null;
            var y = Integral(Delay(() => dy), y0, dt);
            dy = y.Select(x => f(x));
            return y;
        }

        /// <summary>
        /// generates balanced parenthesis.
        /// </summary>
        /// <param name="inner">inner string</param>
        /// <returns>balanced parenthesis</returns>
        static Stream<string> Paren(string inner)
        {
            return Cons(inner, () => Paren("(" + inner + ")"));
        }

        static void Main(string[] args)
        {
            // compute exp(1), sin(1) and cos(1) by the integrating series.
            Stream<double> exp = null;
            Stream<double> cos = null;
            Stream<double> sin = null;
            exp = Cons(1, () => IntegrateSeries(exp));
            cos = Cons(1, () => IntegrateSeries(sin).Select(x => -x));
            sin = Cons(0, () => IntegrateSeries(cos));
            Console.WriteLine("exp(1), sin(1) and cos(1) by the integrating series");
            Console.WriteLine("e               = " + exp.ToEnumerable().Take(100).Sum());
            Console.WriteLine("sin(1)          = " + sin.ToEnumerable().Take(100).Sum());
            Console.WriteLine("sin(1) (actual) = " + Math.Sin(1));
            Console.WriteLine("cos(1)          = " + cos.ToEnumerable().Take(100).Sum());
            Console.WriteLine("cos(1) (actual) = " + Math.Cos(1));
            Console.WriteLine();

            // compute approximation of pi by the stream of alternating series.
            Stream<double> piStream = PiSummands(1).PartialSums().Select(x => 4 * x);
            Console.WriteLine("approximation of pi by the stream of alternating series");
            Console.WriteLine("no acceration    : " + piStream.ElementAt(7));
            Console.WriteLine("Euler's transform: " + EulerTransform(piStream).ElementAt(7));
            Console.WriteLine("using tableau    : " + AcceratedSequence(EulerTransform, piStream).ElementAt(7));
            Console.WriteLine("actual           : " + Math.PI);
            Console.WriteLine();

            // compute e = 2.71828... by solving the differential equation dy/dt = y
            // with initial condition y(0) = 1.
            Console.WriteLine("e by solving the differential equation dy/dt = y");
            Console.WriteLine("e = " + Solve(y => y, 1, 0.001).ElementAt(1000));
            Console.WriteLine();

            // represent balanced parenthesis
            Stream<string> paren = Paren("");
            Console.WriteLine(paren.ElementAt(1));
            Console.WriteLine(paren.ElementAt(2));
            Console.WriteLine(paren.ElementAt(3));
            Console.WriteLine();

            Console.WriteLine("hit Enter key");
            Console.ReadLine();
        }
    }
}
