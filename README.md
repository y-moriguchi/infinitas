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

```csharp
var delayed = Delay(() => 765);
delayed.Force();   // 765
```

### Cons
Cons creates a delayed list whose values are first value and thunk of rest values.

```csharp
// creates a stream of integers
static Stream<int> Integers(int n)
{
    return Cons(n, () => Integers(n + 1));
}
```

### Car and Cdr Property
Car and Cdr property are getting the first value and the rest values, respectivelly.  
If the stream is finite and has no more values, Cdr will be null.

```csharp
var integers = Integers(1);
integers.Car;  // 1
integers.Cdr;  // Integers(2)
```

### ToStream
ToStream creates a singleton stream which consists of the given value.

```csharp
var stream = ToStream(765);  // a stream which contains only 765
```

### Where
Where filters the stream by predicate function.

```csharp
var stream = Integers(1).Where(x => x % 2 == 0);  // stream of even integers
```

### Select
Select maps the stream by the given function.  
Select method is overridden using two streams.

```csharp
var stream = Integers(1).Select(x => x * x);  // stream of squared integers
```

### SelectMany and LINQ query syntax
SelectMany is a monadic bind function of stream.  
LINQ query syntax is available by this method.

```csharp
var stream = from a in Range(1, 3)
             from b in Range(2, 2)
             select a + b;         // stream of 3, 4, 4, 5, 5, 6
```

### ElementAt
ElementAt gets the value indexed by the given index.  
Index is start from 0.
If index is out of range, this throws ArgumentOutOfRangeException.

```csharp
var value = Integers(1).ElementAt(3);  // 4
```

### Interleave
Interleave creates the stream whose value is gotten from two streams alternatively.

```csharp
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

### AsStream
AsStream converts IEnumerable to Stream.

```csharp
var stream = Enumerable.Range(1, 3).AsStream();  // stream of 1, 2, 3
```

### AsEnumerator
AsEnumerator converts Stream to IEnumerable.

```csharp
var enumerator = Integers(1).AsEnumerable();  // infinite IEnumerable of integers
```

### Concat
Concat concatenates two streams.

```csharp
var stream = Range(1, 3).Concat(Range(4, 2));  // stream of 1, 2, 3, 4, 5
```

### Zip
Zip creates new stream whose element is result of applied the given function
to corresponding element of the given streams.

```csharp
var s1 = Range(1, 4);
var s2 = Range(2, 3);
var stream1 = s1.Zip(s2, (x, y) => x + y);  // stream of 3, 5, 7
```

### Iterate
Iterate returns stream whose first value is the baseValue and rest value is Iterate(func, func(baseValue)).  
Below example computes golden ratio.

```csharp
Stream<double> goldenStream = Iterate(x => 1.0 + 1 / x, 1.0);
goldenStream.ElementAt(100);   // about 1.618
```

### Range
Range generates stream of int.

```csharp
var stream = Range(1, 3);   // stream of 1, 2, 3
```

### Repeat
Repeat returns infinite stream whose value is constant.

```csharp
var stream = Repeat(1);  // stream of 1, 1, 1, ...
```

### Flat
Flat flats stream of stream.

```csharp
var streams = new Stream<int>[3];
streams[0] = Range(1, 3);             // stream if 1, 2, 3
streams[1] = new int[0].AsStream();   // empty stream
streams[2] = Range(1, 2);             // stream of 1, 2
Stream<int> stream = streams.AsStream().Flat();  // stream of 1, 2, 3, 1, 2
```

### Skip
Skip skips given elements.
If null is reached while skipping, returns null.

```csharp
var stream1 = Range(1, 5);
var stream = stream1.Skip(3);   // stream of 4, 5
```

### SkipWhile
SkipWhile skips elements while result that element applied to given predicate is true.
If null is reached while skipping, returns null.

```csharp
var stream11 = Range(1, 5);
var stream = stream1.SkipWhile(x => x <= 3);  // stream of 4, 5
```

## Example
Computing exp(1), sin(1), cos(1) by integrating series represented by stream.

```csharp
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

