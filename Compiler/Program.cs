using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

                if (args.Contains("-help"))
                {
                    Help();
                    return;
                }
                try
                {
                    LexicalAnalyzer lexer = new LexicalAnalyzer(args[0]);
                    if (args.Contains("-lexer"))
                    {
                        LexicalAnalyzer.Lex lex = lexer.GetLex();
                        Console.Write($"{lex.line_number} {lex.numLexStart} {lex.typeLex} {lex.value} {lex.lex}\r\n");
                        while (lex.typeLex != TypeLex.Eof)
                        {
                            lex = lexer.GetLex();
                            Console.Write($"{lex.line_number} {lex.numLexStart} {lex.typeLex} {lex.value} {lex.lex}\r\n");
                        }
                    }
                    if (args.Contains("-sparser"))
                    {
                        LexicalAnalyzer lexerPar = new LexicalAnalyzer(args[0]);
                        Parser parser = new Parser(lexerPar);
                        //Node ans = parser.ParseExpression();
                        if (lexerPar.lastLex.typeLex != TypeLex.Eof)
                        {
                            throw new Exception($"({lexerPar.lastLex.line_number},{lexerPar.lastLex.numLexStart}) ERROR: expected factor");
                        }
                        //ans.ToPrint();
                    }
                    if (args.Contains("-bparser"))
                    {
                        LexicalAnalyzer lexerPar = new LexicalAnalyzer(args[0]);
                        BigParser parser = new BigParser(lexerPar);
                        NodeMainProgram ans = parser.ParseMainProgram();
                        if (lexerPar.lastLex.typeLex != TypeLex.Eof)
                        {
                            throw new Exception($"({lexerPar.lastLex.line_number},{lexerPar.lastLex.numLexStart}) ERROR: program is over");
                        }
                        Console.WriteLine(ans.ToPrint(""));
                    }
                }
                catch (Exception e)
                {
                    Console.Write($"{e.Message}\r\n");
                }
            }

            private static void Help()
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  dotnet run [file] [options]");
                Console.WriteLine("Options:");
                Console.WriteLine("  -lexer       lexical parser");
                Console.WriteLine("  -sparser      simple expression parser");
            }
        }
    }
}
