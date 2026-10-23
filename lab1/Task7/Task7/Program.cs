using System;

class Program
{
    static void Main()
    {
        HeavyMethod();
        SpanDemo.Run();
    }

    static void HeavyMethod()
    {
        double result = 0;
        for (int i = 0; i < 50000; i++) result += Math.Sqrt(i) * Math.Sin(i);
        Console.WriteLine(result);
    }
}

static class SpanDemo
{
    const string Sample = "STATUS=200;ID=49201;LINE=12;DELAY=00:04:15";

    public static (int line, TimeSpan delay) ParseClassic(string s)
    {
        int line = 0; TimeSpan delay = default;
        foreach (string part in s.Split(';'))
        {
            int eq = part.IndexOf('=');
            string key = part.Substring(0, eq);
            string value = part.Substring(eq + 1);
            if (key == "LINE") line = int.Parse(value);
            else if (key == "DELAY") delay = TimeSpan.Parse(value);
        }
        return (line, delay);
    }

    public static (int line, TimeSpan delay) ParseSpan(ReadOnlySpan<char> s)
    {
        int line = 0; TimeSpan delay = default;
        while (!s.IsEmpty)
        {
            int sep = s.IndexOf(';');
            ReadOnlySpan<char> part = sep >= 0 ? s.Slice(0, sep) : s;
            s = sep >= 0 ? s.Slice(sep + 1) : ReadOnlySpan<char>.Empty;

            int eq = part.IndexOf('=');
            ReadOnlySpan<char> key = part.Slice(0, eq);
            ReadOnlySpan<char> value = part.Slice(eq + 1);

            if (key.SequenceEqual("LINE".AsSpan())) line = int.Parse(value);
            else if (key.SequenceEqual("DELAY".AsSpan())) delay = TimeSpan.Parse(value);
        }
        return (line, delay);
    }

    public static void Run()
    {
        ParseClassic(Sample);
        ParseSpan(Sample);

        const int N = 1000;
        int check = 0;

        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < N; i++) check += ParseClassic(Sample).line;
        long classic = GC.GetAllocatedBytesForCurrentThread() - before;

        before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < N; i++) check += ParseSpan(Sample).line;
        long span = GC.GetAllocatedBytesForCurrentThread() - before;

        var r = ParseSpan(Sample);
        Console.WriteLine("LINE=" + r.line + ", DELAY=" + r.delay);
        Console.WriteLine("Classic: " + (classic / N) + " B na wywolanie");
        Console.WriteLine("Span:    " + (span / N) + " B na wywolanie");
    }
}