using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace Compiler
{
    class Tester
    {
        struct FolderTests{
            public string folder;
            public int number;
            public FolderTests(string folder = "/", int number = 0)
            {
                this.folder = folder;
                this.number = number;
            }
        }
        static FolderTests[] ?foldersTests;

        public static void StartTest(string key)
        {
            switch (key)
            {
                case "-lexer":
                    foldersTests = new FolderTests[] { new("string", 5), new("indifier", 8), new("integer", 19), new("real", 3), new("space", 2),
                                   new("comment", 4), new("key word", 4), new("operation sign", 3), new("separator", 3), new("errors", 3) };
                    break;
                case "-sparser":
                    foldersTests = new FolderTests[] { new("simple parser", 12) };
                    break;
                default:
                    foldersTests = new FolderTests[] { };
                    break;
            }
            int okey = 0;
            int errors = 0;
            for (int folder = 0; folder < foldersTests.Length; folder++)
            {
                Console.Write($"{foldersTests[folder].folder}:\n");
                for (int test = 1; test <= foldersTests[folder].number; test++)
                {
                    string testStr = test.ToString();
                    if (testStr.Length < 2)
                    {
                        testStr = "0" + testStr;
                    }
                    string pathIn = Environment.CurrentDirectory + $"/tests/{foldersTests[folder].folder}/" + $"{testStr}_input.txt";
                    string pathOut = Environment.CurrentDirectory + $"/tests/{foldersTests[folder].folder}/" + $"{testStr}_out.txt";
                    string pathCorrect = Environment.CurrentDirectory + $"/tests/{foldersTests[folder].folder}/" + $"{testStr}_correct.txt";

                    LexicalAnalyzer lexer;
                    using (StreamWriter sw = new StreamWriter(pathOut))
                    {
                        try
                        {
                            lexer = new LexicalAnalyzer(pathIn);
                            if (key == "-lexer")
                            {
                                LexicalAnalyzer.Lex lex = lexer.GetLex();
                                sw.Write($"{lex.line_number} {lex.numLexStart} {lex.typeLex} {lex.value} {lex.lex}\r\n");
                                while (lex.typeLex != "Eof")
                                {
                                    lex = lexer.GetLex();
                                    sw.Write($"{lex.line_number} {lex.numLexStart} {lex.typeLex} {lex.value} {lex.lex}\r\n");
                                }
                            }
                            if (key == "-sparser")
                            {
                                LexicalAnalyzer lexerPar = new LexicalAnalyzer(pathIn);
                                Parser parser = new Parser(lexerPar);
                                Node ans = parser.ParseExpression();
                                if (lexerPar.lastLex.typeLex != "Eof")
                                {
                                    throw new Exception($"({lexerPar.lastLex.line_number},{lexerPar.lastLex.numLexStart}) ERROR: expected factor");
                                }
                                void PrintNode(Node? node, int tab = 0, bool isLeft = true)
                                {
                                    if (node != null)
                                    {
                                        for (int i = 0; i < tab - 1; i++)
                                        {
                                            sw.Write("    ");
                                        }
                                        if (tab > 0)
                                        {
                                            if (isLeft)
                                            {
                                                sw.Write("├───");
                                            }
                                            else
                                            {
                                                sw.Write("└───");
                                            }
                                        }
                                        tab += 1;
                                        sw.WriteLine(node.value);
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
                        }
                        catch (Exception e)
                        {
                            sw.Write($"{e.Message}\r\n");
                        }
                    }

                    string? checkFile;
                    string? outFile;
                    using (StreamReader sr = new StreamReader(pathCorrect, Encoding.UTF8))
                    {
                        checkFile = sr.ReadToEnd();
                    }
                    using (StreamReader sr = new StreamReader(pathOut, Encoding.UTF8))
                    {
                        outFile = sr.ReadToEnd();
                    }

                    bool flag = true;
                    int minLenght = 0;
                    if (checkFile.Length > outFile.Length)
                    {
                        minLenght = outFile.Length;
                    }
                    else
                    {
                        minLenght = checkFile.Length;
                    }
                    for (int i = 0; i < minLenght; i++)
                    {
                        if (checkFile[i] != outFile[i])
                        {
                            flag = false;
                        }
                    }

                    if (flag)
                    {
                        Console.WriteLine($"{test}) OK");
                        okey += 1;
                    }
                    else
                    {
                        Console.WriteLine($"{test}) ERROR");
                        errors += 1;
                    }
                }
            }
            Console.WriteLine($"OK: {okey}\nERRORS: {errors}");
        }
    }
}