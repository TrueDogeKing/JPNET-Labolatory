using System;
using System.Diagnostics;
using System.Reflection;

namespace JP_NET.Lab1
{
    public partial class ClassA
    {
        private int _a;
        public int A { get => _a; set => _a = value; }
        public int b;
        public const char nameConst = 'B';
        public readonly char nameReadonly = 'B';

        public void MethodOfClassA()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            var x = String.Format("{0}.{1}", fvi.ProductMajorPart, fvi.ProductMinorPart);
            PartialMethodOfClassA(x);
        }

        partial void PartialMethodOfClassA(object x);
        partial void PartialMethodOfClassA(object x)
        {
            Console.WriteLine("Wydruk z nowej wersji 1.0.0.1. {0}", x);
        }
    }
}