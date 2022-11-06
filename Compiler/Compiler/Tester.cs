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
        static FolderTests[] foldersTests = new FolderTests[] { new("string", 5), new("indifier", 8), new("integer", 19), new("real", 3), new("space", 2), 
            new("comment", 4), new("key word", 4), new("end file", 4), new("operation sign", 3), new("separator", 3), new("errors", 3) };

        public static void StartTest()
        {
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
                    string pathIn = $"../../../tests/{foldersTests[folder].folder}/" + $"{testStr}_input.txt";
                    string pathOut = $"../../../tests/{foldersTests[folder].folder}/" + $"{testStr}_out.txt";
                    string pathCorrect = $"../../../tests/{foldersTests[folder].folder}/" + $"{testStr}_correct.txt";
                    MainClass.StartLexer(pathIn, pathOut);
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