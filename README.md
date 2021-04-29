# Infinitas

Infinitas is an implementation of streams like Structure and Implementation of Computer Programs (SICP) by C#.  
Features of Infinitas are shown as follows.

* Treating delayed list (delayed cons cell)
* Treating infinite list (infinite cons cell)
* Conversion Stream to IEnumerable and vice versa

## Methods

### Delay and Force
Delay and Force are delaying evaluation of thunk and getting the value from delayed object, respectively.  
Forced value is memoized.

```
var delayed = Delay(() => 765);
delayed.Force();   // 765
```

### Cons
Cons creates a delayed list whose values are first value and thunk of rest values.

```
// creates a stream of integers
static Stream<int> Integers(int n)
{
    return Cons(n, () => Integers(n + 1));
}
```

### Car and Cdr Property
Car and Cdr property are getting the first value and the rest values, respectivelly.  
If the stream is finite and has no more values, Cdr will be null.

```
var integers = Integers(1);
integers.Car;  // 1
integers.Cdr;  // Integers(2)
```

### ToStream
ToStream creates a singleton stream which consists of the given value.

```
var stream = 765.ToStream();  // a stream which contains only 765
```

### Where
Where filters the stream by predicate function.

```
var stream = Integers(1).Where(x => x % 2 == 0);  // stream of even integers
```

### Select
Select maps the stream by the given function.  
Select method is overridden using two streams.

```
var stream = Integers(1).Select(x => x * x);  // stream of squared integers
```

### ElementAt
ElementAt gets the value indexed by the given index.  
Index is start from 0.
If index is out of range, this throws ArgumentOutOfRangeException.

```
var value = Integers(1).ElementAt(3);  // 4
```

### Interleave
Interleave creates the stream whose value is gotten from two streams alternatively.

```
var stream1 = Enumerable.Range(10, 2).ToStream();
var stream2 = Enumerable.Range(20, 4).ToStream();
var stream3 = stream1.Interleave(stream2);
stream3.ElementAt(0);   // 10
stream3.ElementAt(1);   // 20
stream3.ElementAt(2);   // 11
stream3.ElementAt(3);   // 21
stream3.ElementAt(4);   // 22
stream3.ElementAt(5);   // 23
```

### ToStream
ToStream converts IEnumerable to Stream.

```
var stream = Enumerable.Range(1, 3).ToStream();  // stream of 1, 2, 3
```

### ToEnumerator
ToEnumerator converts Stream to IEnumerable.

```
var enumerator = Integers(1).ToEnumerable();  // infinite IEnumerable of integers
```

## Example
Computing exp(1), sin(1), cos(1) by integrating series represented by stream.

```
    static class Example
    {
        static Stream<double> IntegrateSeries(double count, Stream<double> series)
        {
            return Cons(series.Car / count, () => IntegrateSeries(count + 1, series.Cdr));
        }

        static Stream<double> IntegrateSeries(Stream<double> series)
        {
            return IntegrateSeries(1, series);
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
        }
    }
```

