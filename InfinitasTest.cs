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
            var st2 = Enumerable.Range(1, 5).AsStream();
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
            var st1 = Enumerable.Range(1, 3).AsStream();
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
            var st1 = Enumerable.Range(1, 3).AsStream();
            var st2 = Enumerable.Range(3, 5).AsStream();
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
            var st1 = Enumerable.Range(2, 3).AsStream();
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
            var st1 = Enumerable.Range(10, 2).AsStream();
            var st2 = Enumerable.Range(20, 4).AsStream();
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
        }

        [TestMethod]
        public void IterateTest()
        {
            Stream<double> goldenStream = Iterate(x => 1.0 + 1 / x, 1.0);
            Assert.IsTrue(Math.Abs(goldenStream.ElementAt(100) - 1.618) < 0.001);
        }

        [TestMethod]
        public void ConstantTest()
        {
            var stream = Constant(27);
            Assert.AreEqual(27, stream.Car);
            Assert.AreEqual(27, stream.Cdr.Car);
            Assert.AreEqual(27, stream.Cdr.Cdr.Car);
            Assert.AreEqual(27, stream.Cdr.Cdr.Cdr.Car);
            Assert.AreEqual(27, stream.Cdr.Cdr.Cdr.Cdr.Car);
        }

        [TestMethod]
        public void SelectManyTest()
        {
            var res1 = from a in Enumerable.Range(1, 3).AsStream()
                       from b in Enumerable.Range(2, 2).AsStream()
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
            var res1 = Enumerable.Range(1, 3).AsStream().Concat(Enumerable.Range(4, 2).AsStream());
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
            var s1 = Enumerable.Range(1, 4).AsStream();
            var s2 = Enumerable.Range(2, 3).AsStream();
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
    }
}
