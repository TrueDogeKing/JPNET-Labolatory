using System;
using JP_NET.Lab1;

public class Program
{
    public static void Main(string[] args)
    {
        ClassA a = new ClassA();
        a.MethodOfClassA();
        a.A = 1;
        a.b = 2;
        Console.WriteLine("Const {0}", ClassA.nameConst);
        Console.WriteLine("Readonly {0}", a.nameReadonly);
    }
}