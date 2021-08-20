/*
 * This source code is under the Unlicense
 */
using System;
using System.Collections.Generic;

namespace Morilib
{
    /// <summary>
    /// Implementation of streams like Structure and Implementation of Computer Programs (SICP)
    /// by C#.
    /// </summary>
    public static class Infinitas
    {
        /// <summary>
        /// A class of delayed object (promise).
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        public class Delayed<T>
        {
            private Func<T> thunk;
            private T memo;

            internal Delayed(Func<T> thunk)
            {
                this.thunk = thunk;
            }

            /// <summary>
            /// gets a value from this delayed object.
            /// The value is memoized.
            /// </summary>
            /// <returns></returns>
            public T Force()
            {
                if(thunk != null)
                {
                    memo = thunk();
                    thunk = null;
                }
                return memo;
            }
        }

        /// <summary>
        /// creates a delayed object from the thunk.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="thunk">thunk of object</param>
        /// <returns>delayed object</returns>
        public static Delayed<T> Delay<T>(Func<T> thunk)
        {
            return new Delayed<T>(thunk);
        }

        /// <summary>
        /// A stream of values.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        public class Stream<T>
        {
            private Func<Stream<T>> cdr;
            private Stream<T> memoCdr;

            internal Stream(T car, Func<Stream<T>> cdr)
            {
                this.Car = car;
                this.cdr = cdr;
            }

            /// <summary>
            /// First value of the stream.
            /// </summary>
            public T Car { get; }

            /// <summary>
            /// Rest values of the stream.
            /// </summary>
            public Stream<T> Cdr
            {
                get
                {
                    if(cdr != null)
                    {
                        memoCdr = cdr();
                        cdr = null;
                    }
                    return memoCdr;
                }
            }
        }

        /// <summary>
        /// creates a singleton stream consisted of the value.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static Stream<T> ToStream<T>(T value)
        {
            return Cons(value, () => null);
        }

        /// <summary>
        /// creates a stream from first value and thunk of rest values.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="car">first value</param>
        /// <param name="cdr">thunk of rest values</param>
        /// <returns></returns>
        public static Stream<T> Cons<T>(T car, Func<Stream<T>> cdr)
        {
            if (cdr == null) { throw new ArgumentNullException(nameof(cdr)); }
            return new Stream<T>(car, cdr);
        }

        /// <summary>
        /// filters the stream by the predicate function.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="stream">stream</param>
        /// <param name="pred">predicate function</param>
        /// <returns>filtered stream</returns>
        public static Stream<T> Where<T>(this Stream<T> stream, Func<T, bool> pred)
        {
            if (pred == null) { throw new ArgumentNullException(nameof(pred)); }
            for (var now = stream; stream != null && !pred(stream.Car); stream = stream.Cdr) { }

            return stream == null ? null : Cons(stream.Car, () => Where(stream.Cdr, pred));
        }

        /// <summary>
        /// maps the stream by the function.
        /// </summary>
        /// <typeparam name="T">type of input stream</typeparam>
        /// <typeparam name="U">type of output stream</typeparam>
        /// <param name="stream">source stream</param>
        /// <param name="func">map function</param>
        /// <returns></returns>
        public static Stream<U> Select<T, U>(this Stream<T> stream, Func<T, U> func)
        {
            if (func == null) { throw new ArgumentNullException(nameof(func)); }
            return stream == null ? null : Cons(func(stream.Car), () => Select(stream.Cdr, func));
        }

        /// <summary>
        /// maps the two streams by the function
        /// </summary>
        /// <typeparam name="T">type of first input stream</typeparam>
        /// <typeparam name="U">type of second input stream</typeparam>
        /// <typeparam name="V">type of output stream</typeparam>
        /// <param name="stream1">first source stream</param>
        /// <param name="stream2">second source stream</param>
        /// <param name="func">map function</param>
        /// <returns></returns>
        public static Stream<V> Select<T, U, V>(Stream<T> stream1, Stream<U> stream2, Func<T, U, V> func)
        {
            if (func == null) { throw new ArgumentNullException(nameof(func)); }
            return stream1 == null || stream2 == null ? null :
                Cons(func(stream1.Car, stream2.Car), () => Select(stream1.Cdr, stream2.Cdr, func));
        }

        /// <summary>
        /// A bind function of stream.
        /// </summary>
        /// <typeparam name="T">input type</typeparam>
        /// <typeparam name="U">output type</typeparam>
        /// <param name="id">stream</param>
        /// <param name="k">function to bind</param>
        /// <returns>bound stream</returns>
        public static Stream<U> SelectMany<T, U>(this Stream<T> stream, Func<T, Stream<U>> f)
        {
            if (f == null) { throw new ArgumentNullException(nameof(f)); }
            return stream == null ? null : f(stream.Car).Concat(SelectMany(stream.Cdr, f));
        }

        /// <summary>
        /// A bind function of stream.
        /// This form is for LINQ query syntax.
        /// </summary>
        /// <typeparam name="T">input type</typeparam>
        /// <typeparam name="U">output type</typeparam>
        /// <typeparam name="V">mapped type</typeparam>
        /// <param name="m">stream</param>
        /// <param name="k">function to bind</param>
        /// <param name="s">map function</param>
        /// <returns>bound stream</returns>
        public static Stream<V> SelectMany<T, U, V>(this Stream<T> m, Func<T, Stream<U>> k, Func<T, U, V> s)
        {
            if (k == null) { throw new ArgumentNullException(nameof(k)); }
            if (s == null) { throw new ArgumentNullException(nameof(s)); }
            return m.SelectMany(t => k(t).SelectMany(u => ToStream(s(t, u))));
        }

        /// <summary>
        /// concatenates two streams.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="stream1">stream</param>
        /// <param name="stream2">another stream</param>
        /// <returns>concatenated stream</returns>
        public static Stream<T> Concat<T>(this Stream<T> stream1, Stream<T> stream2)
        {
            return stream1 == null ? stream2 : Cons(stream1.Car, () => Concat(stream1.Cdr, stream2));
        }

        /// <summary>
        /// creates new stream whose element is result of applied
        /// the given function to corresponding element of the given streams.
        /// </summary>
        /// <typeparam name="T">type of stream1</typeparam>
        /// <typeparam name="U">type of stream2</typeparam>
        /// <typeparam name="V">type of result</typeparam>
        /// <param name="stream1">stream</param>
        /// <param name="stream2">another stream</param>
        /// <param name="f">function</param>
        /// <returns>stream</returns>
        public static Stream<V> Zip<T, U, V>(this Stream<T> stream1, Stream<U> stream2, Func<T, U, V> f)
        {
            if (f == null) { throw new ArgumentNullException(nameof(f)); }
            if (stream1 == null || stream2 == null)
            {
                return null;
            }
            else
            {
                return Cons(f(stream1.Car, stream2.Car), () => Zip(stream1.Cdr, stream2.Cdr, f));
            }
        }

        /// <summary>
        /// gets an element value by the index.
        /// The index is started from 0.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="stream">stream</param>
        /// <param name="index">index started from 0</param>
        /// <returns></returns>
        public static T ElementAt<T>(this Stream<T> stream, int index)
        {
            Stream<T> now = stream;

            if(index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < index; i++)
            {
                if((now = now.Cdr) == null)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            return now.Car;
        }

        /// <summary>
        /// creates the stream whose value is gotten from two streams alternatively.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="s1">first stream</param>
        /// <param name="s2">second stream</param>
        /// <returns>interleaved stream</returns>
        public static Stream<T> Interleave<T>(this Stream<T> s1, Stream<T> s2)
        {
            return s1 == null ? s2 : Cons(s1.Car, () => s2.Interleave(s1.Cdr));
        }

        /// <summary>
        /// converts the stream to IEnumerable.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="stream">stream to convert</param>
        /// <returns>converted IEnumerable</returns>
        public static IEnumerable<T> AsEnumerable<T>(this Stream<T> stream)
        {
            for(var pointer = stream; pointer != null; pointer = pointer.Cdr)
            {
                yield return pointer.Car;
            }
        }

        private static Stream<T> AsStreamAux<T>(IEnumerator<T> enumerator)
        {
            return enumerator.MoveNext() ? Cons(enumerator.Current, () => AsStreamAux(enumerator)) : null;
        }

        /// <summary>
        /// converts the IEnumertable to stream.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="enumerable">IEnumerable to convert</param>
        /// <returns>converted stream</returns>
        public static Stream<T> AsStream<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null) { throw new ArgumentNullException(nameof(enumerable)); }
            return AsStreamAux(enumerable.GetEnumerator());
        }

        /// <summary>
        /// returns stream whose first value is the baseValue and rest value is Iterate(func, func(baseValue)).
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="func">function</param>
        /// <param name="baseValue">base value</param>
        /// <returns>new stream</returns>
        public static Stream<T> Iterate<T>(Func<T, T> func, T baseValue)
        {
            if (func == null) { throw new ArgumentNullException(nameof(func)); }
            return Cons(baseValue, () => Iterate(func, func(baseValue)));
        }

        /// <summary>
        /// generates stream of int.
        /// </summary>
        /// <param name="start">start number</param>
        /// <param name="count">count</param>
        /// <returns>stream</returns>
        public static Stream<int> Range(int start, int count)
        {
            return count > 0 ? Cons(start, () => Range(start + 1, count - 1)) : null;
        }

        /// <summary>
        /// returns infinite stream whose value is constant.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="value">constant</param>
        /// <returns>constant value</returns>
        public static Stream<T> Repeat<T>(T value)
        {
            Stream<T> stream = null;

            stream = Cons(value, () => stream);
            return stream;
        }

        /// <summary>
        /// generates stream which repeats the given value during the given count.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="value">value to repeat</param>
        /// <param name="count">count</param>
        /// <returns>stream</returns>
        public static Stream<T> Repeat<T>(T value, int count)
        {
            return count > 0 ? Cons(value, () => Repeat(value, count - 1)) : null;
        }

        /// <summary>
        /// flat stream of stream.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="stream">stream of stream</param>
        /// <returns>flatten stream</returns>
        public static Stream<T> Flat<T>(this Stream<Stream<T>> stream)
        {
            if(stream == null)
            {
                return null;
            }
            else if(stream.Car == null)
            {
                return Flat(stream.Cdr);
            }
            else
            {
                return Cons(stream.Car.Car, () => Flat(Cons(stream.Car.Cdr, () => stream.Cdr)));
            }
        }

        /// <summary>
        /// skips given elements.
        /// If null is reached while skipping, returns null.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="stream">stream</param>
        /// <param name="skip">skip elements</param>
        /// <returns></returns>
        public static Stream<T> Skip<T>(this Stream<T> stream, int skip)
        {
            var ptr = stream;

            for (int i = 0; ptr != null && i < skip; i++, ptr = ptr.Cdr) { }
            return ptr;
        }

        /// <summary>
        /// skips elements while result that element applied to given predicate is true.
        /// If null is reached while skipping, returns null.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="stream">stream</param>
        /// <param name="skip">skip elements</param>
        /// <returns></returns>
        public static Stream<T> SkipWhile<T>(this Stream<T> stream, Func<T, bool> pred)
        {
            var ptr = stream;

            if (pred == null) { throw new ArgumentNullException(nameof(pred)); }
            for (; ptr != null && pred(ptr.Car); ptr = ptr.Cdr) { }
            return ptr;
        }
    }
}
