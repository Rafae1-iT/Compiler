using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Compiler;

namespace Compiler
{
    class MainClass
    {
        static class Program
        {
            private static void Main(string[] args)
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Options:");
                    Console.WriteLine("  -help    Display help");
                    //return;
                }
                LexicalAnalyzer lexer = new LexicalAnalyzer("D:/Projects/Cmp/Compiler/Compiler/test.txt");

                if (args.Contains("-help"))
                {
                    Console.WriteLine("Usage:");
                    Console.WriteLine("  dotnet run [file] [options]");
                    Console.WriteLine("Options:");
                    Console.WriteLine("  -lexer       lexical parser");
                    Console.WriteLine("  -sparser      simple expression parser");
                    return;
                }
                Tester.StartTest(args[0]);
            }
        }
    }
}