using System;
using System.Windows;

class Program
{
    [STAThread]
    static void Main()
    {
        var app = new Application();
        app.Run(LabCli.WindowFactory.Create());
    }
}