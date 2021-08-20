/*
 * This source code is under the Unlicense
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using static Morilib.Infinitas;

namespace Morilib
{
    [TestClass]
    public class InfinitasTest
    {
        [TestMethod]
        public void DelayForceTest()
        {
            var res = Delay(() => 765);

            Assert.AreEqual(res.Force(), 765);
            Assert.AreEqual(res.Force(), 765);
        }

        [TestMethod]
        public void StreamToStreamTest()
        {
            var res = ToStream(765);

            Assert.AreEqual(765, res.Car);
            Assert.IsNull(res.Cdr);
        }

        [TestMethod]
        public void StreamWhereTest1()
        {
            var st2 = Range(1, 5);
            var res2 = st2.Where(x => x % 2 == 0);
            Assert.AreEqual(2, res2.Car);
            Assert.AreEqual(4, res2.Cdr.Car);
            Assert.IsNull(res2.Cdr.Cdr);
        }

        private Stream<int> Integers(int n)
        {
            return Cons(n, () => Integers(n + 1));
        }

        [TestMethod]
        public void StreamWhereTest2()
        {
            var st1 = Integers(1);
            var res1 = st1.Where(x => x % 2 == 0);
            Assert.AreEqual(2, res1.Car);
            Assert.AreEqual(4, res1.Cdr.Car);
            Assert.AreEqual(6, res1.Cdr.Cdr.Car);
            Assert.AreEqual(8, res1.Cdr.Cdr.Cdr.Car);
        }

        [TestMethod]
        public void StreamSelectTest1()
        {
            var st1 = Range(1, 3);
            var res1 = st1.Select(x => "0" + x);
            Assert.AreEqual("01", res1.Car);
            Assert.AreEqual("02", res1.Cdr.Car);
            Assert.AreEqual("03", res1.Cdr.Cdr.Car);
            Assert.IsNull(res1.Cdr.Cdr.Cdr);
        }

        [TestMethod]
        public void StreamSelectTest2()
        {
            var st1 = Integers(1);
            var res1 = st1.Select(x => "0" + x);
            Assert.AreEqual("01", res1.Car);
            Assert.AreEqual("02", res1.Cdr.Car);
            Assert.AreEqual("03", res1.Cdr.Cdr.Car);
        }

        [TestMethod]
        public void StreamSelectTest3()
        {
            var st1 = Range(1, 3);
            var st2 = Range(3, 5);
            var res1 = Select(st1, st2, (x, y) => "0" + (x + y));
            Assert.AreEqual("04", res1.Car);
            Assert.AreEqual("06", res1.Cdr.Car);
            Assert.AreEqual("08", res1.Cdr.Cdr.Car);
            Assert.IsNull(res1.Cdr.Cdr.Cdr);
        }

        [TestMethod]
        public void StreamSelectTest4()
        {
            var st1 = Integers(1);
            var st2 = Integers(3);
            var res1 = Select(st1, st2, (x, y) => "0" + (x + y));
            Assert.AreEqual("04", res1.Car);
            Assert.AreEqual("06", res1.Cdr.Car);
            Assert.AreEqual("08", res1.Cdr.Cdr.Car);
        }

        [TestMethod]
        public void StreamElementAtTest1()
        {
            var st1 = Range(2, 3);
            Assert.AreEqual(2, st1.ElementAt(0));
            Assert.AreEqual(3, st1.ElementAt(1));
            Assert.AreEqual(4, st1.ElementAt(2));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => st1.ElementAt(3));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => st1.ElementAt(-1));
        }

        [TestMethod]
        public void StreamElementAtTest2()
        {
            var st1 = Integers(2);
            Assert.AreEqual(2, st1.ElementAt(0));
            Assert.AreEqual(3, st1.ElementAt(1));
            Assert.AreEqual(4, st1.ElementAt(2));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => st1.ElementAt(-1));
        }

        [TestMethod]
        public void StreamInterleaveTest()
        {
            var st1 = Range(10, 2);
            var st2 = Range(20, 4);
            var res1 = st1.Interleave(st2);
            Assert.AreEqual(10, res1.ElementAt(0));
            Assert.AreEqual(20, res1.ElementAt(1));
            Assert.AreEqual(11, res1.ElementAt(2));
            Assert.AreEqual(21, res1.ElementAt(3));
            Assert.AreEqual(22, res1.ElementAt(4));
            Assert.AreEqual(23, res1.ElementAt(5));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => res1.ElementAt(6));
        }

        [TestMethod]
        public void StreamToEnumerableTest()
        {
            var st1 = Integers(1);
            Assert.AreEqual(55, st1.AsEnumerable().Take(10).Sum());
        }

        [TestMethod]
        public void IEnumerableToStreamTest()
        {
            var res1 = Enumerable.Range(1, 3).AsStream();
            Assert.AreEqual(1, res1.Car);
            Assert.AreEqual(2, res1.Cdr.Car);
            Assert.AreEqual(3, res1.Cdr.Cdr.Car);
            Assert.IsNull(res1.Cdr.Cdr.Cdr);
            Assert.AreEqual(1, res1.Car);
            Assert.AreEqual(2, res1.Cdr.Car);
            Assert.AreEqual(3, res1.Cdr.Cdr.Car);
            Assert.IsNull(res1.Cdr.Cdr.Cdr);
        }

        [TestMethod]
        public void IterateTest()
        {
            Stream<double> goldenStream = Iterate(x => 1.0 + 1 / x, 1.0);
            Assert.IsTrue(Math.Abs(goldenStream.ElementAt(100) - 1.618) < 0.001);
        }

        [TestMethod]
        public void RepeatTest()
        {
            var stream1 = Repeat(27);
            var stream2 = Repeat(27, 4);
            Assert.AreEqual(27, stream1.Car);
            Assert.AreEqual(27, stream1.Cdr.Car);
            Assert.AreEqual(27, stream1.Cdr.Cdr.Car);
            Assert.AreEqual(27, stream1.Cdr.Cdr.Cdr.Car);
            Assert.AreEqual(27, stream1.Cdr.Cdr.Cdr.Cdr.Car);
            Assert.AreEqual(27, stream2.Car);
            Assert.AreEqual(27, stream2.Cdr.Car);
            Assert.AreEqual(27, stream2.Cdr.Cdr.Car);
            Assert.AreEqual(27, stream2.Cdr.Cdr.Cdr.Car);
            Assert.IsNull(stream2.Cdr.Cdr.Cdr.Cdr);
        }

        [TestMethod]
        public void SelectManyTest()
        {
            var res1 = from a in Range(1, 3)
                       from b in Range(2, 2)
                       select a + b;
            Assert.AreEqual(3, res1.ElementAt(0));
            Assert.AreEqual(4, res1.ElementAt(1));
            Assert.AreEqual(4, res1.ElementAt(2));
            Assert.AreEqual(5, res1.ElementAt(3));
            Assert.AreEqual(5, res1.ElementAt(4));
            Assert.AreEqual(6, res1.ElementAt(5));
            Assert.AreEqual(6, res1.AsEnumerable().Count());
        }

        [TestMethod]
        public void ConcatTest()
        {
            var res1 = Range(1, 3).Concat(Range(4, 2));
            Assert.AreEqual(1, res1.ElementAt(0));
            Assert.AreEqual(2, res1.ElementAt(1));
            Assert.AreEqual(3, res1.ElementAt(2));
            Assert.AreEqual(4, res1.ElementAt(3));
            Assert.AreEqual(5, res1.ElementAt(4));
            Assert.AreEqual(5, res1.AsEnumerable().Count());
        }

        [TestMethod]
        public void ZipTest()
        {
            var s1 = Range(1, 4);
            var s2 = Range(2, 3);
            var res1 = s1.Zip(s2, (x, y) => x + y);
            var res2 = s2.Zip(s1, (x, y) => x + y);
            Assert.AreEqual(3, res1.ElementAt(0));
            Assert.AreEqual(5, res1.ElementAt(1));
            Assert.AreEqual(7, res1.ElementAt(2));
            Assert.AreEqual(3, res1.AsEnumerable().Count());
            Assert.AreEqual(3, res2.ElementAt(0));
            Assert.AreEqual(5, res2.ElementAt(1));
            Assert.AreEqual(7, res2.ElementAt(2));
            Assert.AreEqual(3, res2.AsEnumerable().Count());
        }

        [TestMethod]
        public void RangeTest()
        {
            var res1 = Range(2, 3);
            Assert.AreEqual(2, res1.ElementAt(0));
            Assert.AreEqual(3, res1.ElementAt(1));
            Assert.AreEqual(4, res1.ElementAt(2));
            Assert.AreEqual(3, res1.AsEnumerable().Count());
        }

        [TestMethod]
        public void FlatTest()
        {
            var streams = new Stream<int>[3];
            Stream<int> res1;
            streams[0] = Range(1, 3);
            streams[1] = new int[0].AsStream();
            streams[2] = Range(1, 2);
            res1 = streams.AsStream().Flat();
            Assert.AreEqual(1, res1.ElementAt(0));
            Assert.AreEqual(2, res1.ElementAt(1));
            Assert.AreEqual(3, res1.ElementAt(2));
            Assert.AreEqual(1, res1.ElementAt(3));
            Assert.AreEqual(2, res1.ElementAt(4));
            Assert.AreEqual(5, res1.AsEnumerable().Count());
        }

        [TestMethod]
        public void SkipTest()
        {
            var res1 = Range(1, 5).Skip(3);
            var res2 = Range(1, 3).Skip(3);
            var res3 = Range(1, 2).Skip(3);
            var res4 = Range(1, 2).Skip(0);
            Assert.AreEqual(4, res1.ElementAt(0));
            Assert.AreEqual(5, res1.ElementAt(1));
            Assert.AreEqual(2, res1.AsEnumerable().Count());
            Assert.AreEqual(0, res2.AsEnumerable().Count());
            Assert.AreEqual(0, res3.AsEnumerable().Count());
            Assert.AreEqual(1, res4.ElementAt(0));
            Assert.AreEqual(2, res4.ElementAt(1));
            Assert.AreEqual(2, res4.AsEnumerable().Count());
        }

        [TestMethod]
        public void SkipWhlieTest()
        {
            var res1 = Range(1, 5).SkipWhile(x => x <= 3);
            var res2 = Range(1, 3).SkipWhile(x => x <= 3);
            var res3 = Range(1, 2).SkipWhile(x => x <= 3);
            Assert.AreEqual(4, res1.ElementAt(0));
            Assert.AreEqual(5, res1.ElementAt(1));
            Assert.AreEqual(2, res1.AsEnumerable().Count());
            Assert.AreEqual(0, res2.AsEnumerable().Count());
            Assert.AreEqual(0, res3.AsEnumerable().Count());
        }
    }
}
