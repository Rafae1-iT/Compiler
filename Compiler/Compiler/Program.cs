using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compiler
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Введите '1' для запуска тестов: Лексического анализатора");
            Console.WriteLine("Введите имя файла 'Пример: test.txt' для запуска ручново ввода, файл должен находиться в папке tests");
            string? l = "0";
            l = Console.ReadLine();
            if(l == "1")
            {
                Tester.StartTest();
            } 
            else
            {
                StartLexer(l, "console");
            }
            
        }
        public static void StartLexer(string path = @"../../../tests/input.txt", string pathOut = "")
        {
            int lenLex = 0;
            int numLexStart = 1;

            if(pathOut == "console")
            path = $"../../../tests/{path}";

            LexicalAnalyzer.CreatTable();

            List<string> ans = new List<string>();
            int line_number = 1;
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                string? line;
                bool endRead = false;
                while ((line = sr.ReadLine()) != null && !endRead)
                {
                    while (line.Length > 0)
                    {
                        string typeLex = LexicalAnalyzer.GetLex(line, ref lenLex, numLexStart);
                        string value = LexicalAnalyzer.GetValue(line.Substring(0, lenLex), ref typeLex);

                        if (typeLex == "ERROR_Overflow") // переполнение
                        {
                            ans.Add($"{line_number} {numLexStart} {typeLex} {value}\n");
                            break;
                        }

                        if (typeLex == "ERROR")
                        {
                            ans.Add($"{line_number} {numLexStart} {typeLex}\n");
                            endRead = true;
                            break;
                        }

                        if (typeLex == "endFile")
                        {
                            ans.Add($"{line_number} {numLexStart} {typeLex} { value} { line.Substring(0, lenLex)}\n");
                            endRead = true;
                            break;
                        }

                        if (typeLex != "space" && typeLex != "comments")
                        {
                            ans.Add($"{line_number} {numLexStart} {typeLex} { value} { line.Substring(0, lenLex)}\n");
                        }

                        numLexStart = numLexStart + lenLex;

                        line = line.Substring(lenLex);
                    }
                    line_number++;
                    numLexStart = 1; // обновляем символ с которого начинается лексема
                }
            }
            if (pathOut != "console")
            {
                using (StreamWriter sw = new StreamWriter(pathOut, false, Encoding.Default))
                {
                    foreach (string lineOut in ans)
                    {
                        sw.Write(lineOut);
                    }
                }
            }
            else
            {
                foreach (string lineOut in ans)
                {
                    Console.Write(lineOut);
                }
            }
        }

    }
}
