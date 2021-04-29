﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morilib
{
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
        public static Stream<T> ToStream<T>(this T value)
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
            return Cons(func(stream.Car), () => Select(stream.Cdr, func));
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
            return Cons(func(stream1.Car, stream2.Car), () => Select(stream1.Cdr, stream2.Cdr, func));
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

            for(int i = 0; i < index; i++)
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

        internal class StreamEnumerator<T> : IEnumerator<T>
        {
            internal StreamEnumerator(Stream<T> stream)
            {
                this.stream = stream;
            }

            private bool read = false;
            private Stream<T> stream;

            public T Current => read ? stream.Car : default(T);

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if(read)
                {
                    stream = stream.Cdr;
                }
                else
                {
                    read = true;
                }
                return stream != null;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        internal class StreamEnumerable<T> : IEnumerable<T>
        {
            private readonly Stream<T> stream;

            internal StreamEnumerable(Stream<T> stream)
            {
                this.stream = stream;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new StreamEnumerator<T>(stream);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// converts the stream to IEnumerable.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="stream">stream to convert</param>
        /// <returns>converted IEnumerable</returns>
        public static IEnumerable<T> ToEnumerable<T>(this Stream<T> stream)
        {
            return new StreamEnumerable<T>(stream);
        }

        private static Stream<T> ToStream<T>(IEnumerator<T> enumerator)
        {
            return enumerator.MoveNext() ? Cons(enumerator.Current, () => ToStream(enumerator)) : null;
        }

        /// <summary>
        /// converts the IEnumertable to stream.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="enumerable">IEnumerable to convert</param>
        /// <returns>converted stream</returns>
        public static Stream<T> ToStream<T>(this IEnumerable<T> enumerable)
        {
            return ToStream(enumerable.GetEnumerator());
        }
    }
}