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
                    Console.WriteLine("Usage:");
                    Console.WriteLine("  dotnet run [file] [options]");
                    Console.WriteLine("Options:");
                    Console.WriteLine("  -lexer       lexical parser");
                    Console.WriteLine("  -sparser      simple expression parser");
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
                        Node ans = parser.ParseExpression();
                        if (lexerPar.lastLex.typeLex != TypeLex.Eof)
                        {
                            throw new Exception($"({lexerPar.lastLex.line_number},{lexerPar.lastLex.numLexStart}) ERROR: expected factor");
                        }
                        void PrintNode(Node? node, int tab = 0, bool isLeft = true)
                        {
                            if (node != null)
                            {
                                for (int i = 0; i < tab - 1; i++)
                                {
                                    Console.Write("    ");
                                }
                                if (tab > 0)
                                {
                                    if (isLeft)
                                    {
                                        Console.Write("├───");
                                    }
                                    else
                                    {
                                        Console.Write("└───");
                                    }
                                }
                                tab += 1;
                                Console.WriteLine(node.value);
                                if (node.children.Count > 0)
                                {
                                    PrintNode(node.children[0], tab, true);
                                }
                                if (node.children.Count > 1)
                                {
                                    PrintNode(node.children[1], tab, false);
                                }
                            }
                        }
                        PrintNode(ans);
                    }
                    if (args.Contains("-bparser"))
                    {
                        LexicalAnalyzer lexerPar = new LexicalAnalyzer(args[0]);
                        BigParser parser = new BigParser(lexerPar);
                        Node ans = parser.ParseProgram();
                        if (lexerPar.lastLex.typeLex != TypeLex.Eof)
                        {
                            throw new Exception($"({lexerPar.lastLex.line_number},{lexerPar.lastLex.numLexStart}) ERROR: program is over");
                        }
                        void PrintNode(Node? node, int tab = 0, bool isLeft = true)
                        {
                            if (node != null)
                            {
                                for (int i = 0; i < tab - 1; i++)
                                {
                                    Console.Write("    ");
                                }
                                if (tab > 0)
                                {
                                    if (isLeft)
                                    {
                                        Console.Write("├─── ");
                                    }
                                    else
                                    {
                                        Console.Write("└─── ");
                                    }
                                }
                                tab += 1;
                                Console.WriteLine(node.value);
                                for (int i = 0; i < node.children.Count - 1; i++)
                                {
                                    PrintNode(node.children[i], tab, true);
                                }
                                if(node.children.Count > 0)
                                {
                                    PrintNode(node.children[^1], tab, false);
                                }
                            }
                        }
                        PrintNode(ans);
                    }
                }
                catch (Exception e)
                {
                    Console.Write($"{e.Message}\r\n");
                }
            }
        }
    }
}
