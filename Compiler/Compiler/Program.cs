using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compiler
{
    class MainClass
    {
        const int countStatus = 16;
        const int countCharacters = 256;
        static int[,] table = new int[countStatus, countCharacters];
        static string[] keyWords = {"var", "begin", "end", };
        /*
         * 0- error (ошибка)
         * 1- start (начальное состояние)
         * 2- string (строка)
         * 3- int x2
         * 4- int x8
         * 5- int x10
         * 6- int x16 
         * 7- real numbers (флоат)
         * 8- identifier (индификатор)
         * 9- Key Word (ключевое слово)
         * 10- char (символ)
         * 11- comments (коментарии)
         * 12- space (пробелы)
         * 13- operation (операторы)
         * 14- separators (разделители)
         * 15- end (конец файла)
         */
        public static void CreatTable()
        {
            //string
            table[1, (int)'\''] = 2;
            for (int i = 0; i < countCharacters; i++)
            {
                table[2, i] = 2;
            }

            //indifier
            table[1, (int)'_'] = 8;
            table[8, (int)'_'] = 8;
            for (int i = (int)'a'; i <= (int)'z'; i++)
            {
                table[1, i] = 8;
                table[8, i] = 8;
            }
            for (int i = (int)'A'; i <= (int)'Z'; i++)
            {
                table[1, i] = 8;
                table[8, i] = 8;
            }
            for (int i = (int)'0'; i <= (int)'9'; i++)
            {
                table[8, i] = 8;
            }

            //integer
            table[1, (int)'%'] = 3; // 2-ичная
            for (int i = (int)'0'; i <= (int)'1'; i++)
            {
                table[3, i] = 3;
            }

            table[1, (int)'&'] = 4; // 8-ичная
            for (int i = (int)'0'; i <= (int)'7'; i++)
            {
                table[4, i] = 4;
            }

            for (int i = (int)'0'; i <= (int)'9'; i++) // 10-ичная
            {
                table[1, i] = 5;
                table[5, i] = 5;
            }

            table[1, (int)'$'] = 6; // 16-ричная
            for (int i = (int)'0'; i <= (int)'9'; i++)
            {
                table[6, i] = 6;
            }
            for (int i = (int)'a'; i <= (int)'f'; i++)
            {
                table[6, i] = 6;
            }
            for (int i = (int)'A'; i <= (int)'F'; i++)
            {
                table[6, i] = 6;
            }

            // real numbers
            table[5, (int)'.'] = 7;
            for (int i = (int)'0'; i <= (int)'9'; i++)
            {
                table[7, i] = 7;
            }

            // space
            table[1, (int)' '] = 12;
            table[12, (int)' '] = 12;

            // operation
            table[1, (int)'='] = 13;
            table[1, (int)':'] = 13;
            table[1, (int)'+'] = 13;
            table[1, (int)'-'] = 13;
            table[1, (int)'*'] = 13;
            table[1, (int)'/'] = 13;
            table[1, (int)'>'] = 13;
            table[1, (int)'<'] = 13;

            //separators (разделители)
            table[1, (int)','] = 14;
            table[1, (int)';'] = 14;
            table[1, (int)'('] = 14;
            table[1, (int)')'] = 14;
            table[1, (int)'['] = 14;
            table[1, (int)']'] = 14;
        }

        public static string LexicalAnalyzer(string input, ref int lenLex)
        {
            int statusDFA = 1;
            lenLex = 0;
            for (int i = lenLex; i < input.Length; i++)
            {
                if (table[statusDFA, input[i] > countCharacters ? countCharacters - 1 : (int)input[i]] != 0)
                {
                    lenLex += 1;
                    statusDFA = table[statusDFA, (int)input[i]];
                }
                else
                {
                    if (i == 0)
                    {
                        lenLex += 1;
                    }
                    break;
                }

            }
            if ((statusDFA == 3 || statusDFA == 4 || statusDFA == 6) && lenLex == 1)
            {
                statusDFA = 0;
            }

            if (statusDFA == 8)
            {
                foreach (string i in keyWords) {
                    if(input.Substring(0, lenLex) == i)
                    {
                        statusDFA = 9;
                    }
                }
            }

            switch (statusDFA)
            {
                case 0:
                    return "ERROR";
                    break;
                case 1:
                    return "ERROR";
                    break;
                case 2:
                    return "string";
                    break;
                case 3:
                    return "int_x2";
                    break;
                case 4:
                    return "int_x8";
                    break;
                case 5:
                    return "int_x10";
                    break;
                case 6:
                    return "int_x16";
                    break;
                case 7:
                    return "real_number";
                    break;
                case 8:
                    return "identifier";
                    break;
                case 9:
                    return "Key_Word";
                    break;
                case 10:
                    return "char";
                    break;
                case 11:
                    return "comments";
                    break;
                case 12:
                    return "space";
                    break;
                case 13:
                    return "operation";
                    break;
                case 14:
                    return "separators";
                    break;
                case 15:
                    return "end";
                    break;
            }
            return "ERROR";
        }

        public static void Main(string[] args)
        {
            int lenLex = 0;

            string? input = Console.ReadLine();
            CreatTable();

            string path = @"D:\Projects\Compiler\Compiler\input.txt";
            int line_number = 1;
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    while (line.Length > 0)
                    {
                        string typeLex = LexicalAnalyzer(line, ref lenLex);

                        Console.WriteLine(line_number + " " + typeLex + " " + line.Substring(0, lenLex));

                        line = line.Substring(lenLex);
                    }
                    line_number++;
                }
            }
        }
    }
}
