using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class LexicalAnalyzer
    {
        public struct Lex
        {
            public int line_number;
            public int numLexStart;
            public string? typeLex;
            public string? value;
            public string? lex;

            public Lex(int line_number, int numLexStart, string? typeLex, string? value, string? lex)
            {
                this.line_number = line_number;
                this.numLexStart = numLexStart;
                this.typeLex = typeLex;
                this.value = value;
                this.lex = lex;
            }
        }

        public Lex lastLex;

        const int countStatus = 16;
        const int countCharacters = 256;
        static int[,] table = new int[countStatus, countCharacters];
        //static string[] endFile = { "finish" };
        static string[] keyWords = {
            "and", "array", "as", "asm", "begin", "case", "const",
            "constructor", "destructor", "div", "do", "downto", "else", "end", "file", "for", "foreach",
            "function", "goto", "implementation", "if", "in", "inherited", "inline", "interface", "label", "mod",
            "nil", "not", "object", "of", "operator", "or", "packed", "procedure", "program", "record", "repeat", "self", "set",
            "shl", "shr", "string", "then", "to", "type", "unit", "until", "uses", "var", "while", "with", "xor", "dispose", "exit",
            "false", "new", "true", "as", "class", "dispinterface", "except", "exports", "finalization", "finally", "initialization",
            "inline", "is", "library", "on", "out", "packed", "property", "raise", "resourcestring", "threadvar", "try"
        };
        List<string> input;
        public int numLexStart = 1;
        public int line_number = 1;
        public LexicalAnalyzer(string path)
        {
            CreatTable();
            this.line_number = 1;
            this.numLexStart = 1;
            input = new List<string>();
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    input.Add(line);
                }
            }
        }
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
        public void CreatTable()
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
            table[1, (int)'.'] = 14;
            table[1, (int)';'] = 14;
            table[1, (int)'('] = 14;
            table[1, (int)')'] = 14;
            table[1, (int)'['] = 14;
            table[1, (int)']'] = 14;
        }
        public static string GetTypeLex(string input, ref int lenLex, int numLexStart)
        {
            int k = 0;
            int statusDFA = 1;
            lenLex = 0;
            numLexStart = 1;

            for (int i = lenLex; i < input.Length; i++)
            {
                if (table[statusDFA, input[i] > countCharacters ? countCharacters - 1 : (int)input[i]] != 0)
                {
                    lenLex += 1;
                    statusDFA = table[statusDFA, input[i] > countCharacters ? countCharacters - 1 : (int)input[i]];
                }
                else
                {
                    if (i == 0)
                    {
                        lenLex += 1;
                    }
                    break;
                }

                if (statusDFA == 2 && lenLex > 1) // проверка что string закрывается
                {
                    if (input[i] == '\'')
                    {
                        k = 1;
                        break;
                    }
                }

                //comments
                if (input[0] == '/' && input[1] == '/' && input.Length >= 1)
                {
                    statusDFA = 11;
                    lenLex = input.Length;
                }

                // operation
                if (statusDFA == 13)
                {
                    if (lenLex < input.Length &&
                        ((input[lenLex - 1] == '<' && input[lenLex] == '<') ||
                        (input[lenLex - 1] == '>' && input[lenLex] == '>') ||
                        (input[lenLex - 1] == '*' && input[lenLex] == '*') ||
                        (input[lenLex - 1] == '<' && input[lenLex] == '>') ||
                        (input[lenLex - 1] == '<' && input[lenLex] == '=') ||
                        (input[lenLex - 1] == '>' && input[lenLex] == '=') ||
                        (input[lenLex - 1] == ':' && input[lenLex] == '=') ||
                        (input[lenLex - 1] == '+' && input[lenLex] == '=') ||
                        (input[lenLex - 1] == '-' && input[lenLex] == '=') ||
                        (input[lenLex - 1] == '*' && input[lenLex] == '=') ||
                        (input[lenLex - 1] == '/' && input[lenLex] == '=')))
                    {
                        lenLex += 1;
                    }
                }
            }

            if (statusDFA == 2 && k != 1) // ошибка если string не заклылся
            {
                statusDFA = 0;
            }

            if (statusDFA == 3 && lenLex < input.Length && ((input[lenLex] > '1' && input[lenLex] <= '9') || (input[lenLex] >= 'a' && input[lenLex] <= 'z'))) // коректо ли записан int_x2
            {
                statusDFA = 0;
            }

            if (statusDFA == 4 && lenLex < input.Length && ((input[lenLex] > '7' && input[lenLex] <= '9') || (input[lenLex] >= 'a' && input[lenLex] <= 'z'))) // коректо ли записан int_x8
            {
                statusDFA = 0;
            }

            //real
            if (statusDFA == 7 && lenLex < input.Length && (input[lenLex] != ' ' && input[lenLex] != ')' && input[lenLex] != ';' && input[lenLex] != '-' && input[lenLex] != '=' && input[lenLex] != '+' && input[lenLex] != '>' && input[lenLex] != '<' && input[lenLex] != ':' && input[lenLex] != '*'))
            {
                statusDFA = 0;
            }

            // коректо ли записан int_x10
            if (statusDFA == 5 && lenLex < input.Length && (input[lenLex] != ' ' && input[lenLex] != ')' && input[lenLex] != ';' && input[lenLex] != '-' && input[lenLex] != '=' && input[lenLex] != '+' && input[lenLex] != '>' && input[lenLex] != '<' && input[lenLex] != ':' && input[lenLex] != '*'))
            {
                statusDFA = 0;
            }

            if (statusDFA == 6 && lenLex < input.Length && input[lenLex] > 'F') // коректо ли записан int_x16
            {
                statusDFA = 0;
            }

            if ((statusDFA == 3 || statusDFA == 4 || statusDFA == 6) && lenLex == 1)
            {
                statusDFA = 0;
            }


            if (statusDFA == 8)
            {
                foreach (string i in keyWords)
                {
                    if (input.ToLower().Substring(0, lenLex) == i)
                    {
                        statusDFA = 9;
                    }
                }
            }

            /*if (statusDFA == 8)
            {
                foreach (string i in endFile)
                {
                    if (input.ToLower().Substring(0, lenLex) == i)
                    {
                        statusDFA = 15;
                    }
                }
            }*/


            switch (statusDFA)
            {
                case 0:
                    return "ERROR";
                case 1:
                    return "ERROR";
                case 2:
                    return "string";
                case 3:
                    return "int_x2";
                case 4:
                    return "int_x8";
                case 5:
                    return "int_x10";
                case 6:
                    return "int_x16";
                case 7:
                    return "real_number";
                case 8:
                    return "identifier";
                case 9:
                    return "Key_Word";
                case 10:
                    return "char";
                case 11:
                    return "comments";
                case 12:
                    return "space";
                case 13:
                    return "operation";
                case 14:
                    return "separators";
                case 15:
                    return "endFile";
            }

            return "ERROR";
        }
        public Lex GetLex()
        {
            int lenLex = 0;
            Lex lex = new Lex(line_number, numLexStart, "Eof", "Eof", "");
            if (line_number <= input.Count)
            {
                string typeLex = LexicalAnalyzer.GetTypeLex(input[line_number - 1], ref lenLex, numLexStart);
                string value = LexicalAnalyzer.GetValue(input[line_number - 1].Substring(0, lenLex), ref typeLex);

                if (typeLex == "ERROR" || typeLex == "ERROR_Overflow")
                {
                    throw new Exception ($"({this.line_number},{numLexStart}) {typeLex}");
                }

                lex = new Lex(this.line_number, numLexStart, typeLex, value, input[line_number - 1].Substring(0, lenLex));

                numLexStart = numLexStart + lenLex;

                input[line_number - 1] = input[line_number - 1].Substring(lenLex);

                if (input[line_number - 1].Length == 0)
                {
                    line_number += 1;
                    numLexStart = 1; //обновляем символ с которого начинается лексема
                }

                while (lex.typeLex == "space" || lex.typeLex == "comments")
                {
                    lex = GetLex();
                }
            }
            lastLex = lex;
            return lex;
        }

        public static string GetValue(string lexem, ref string typeLexem)
        {
            if (typeLexem == "int_x2")
            {
                lexem = lexem.Replace("%", "");
                int value;
                try
                {
                    value = Convert.ToInt32(lexem, 2);
                }
                catch (Exception)
                {
                    typeLexem = "ERROR_Overflow";
                    string a = "Integer";
                    return a;
                }
                typeLexem = "Integer";
                return value.ToString();

            }

            if (typeLexem == "int_x8")
            {
                lexem = lexem.Replace("&", "");
                int value;
                try
                {
                    value = Convert.ToInt32(lexem, 8);
                }
                catch (Exception)
                {
                    typeLexem = "ERROR_Overflow";
                    string a = "Integer";
                    return a;
                }
                typeLexem = "Integer";
                return value.ToString();
            }

            if (typeLexem == "int_x10")
            {
                lexem = lexem.Replace("&", "");
                int value;
                try
                {
                    value = Convert.ToInt32(lexem, 10);
                }
                catch (Exception)
                {
                    typeLexem = "ERROR_Overflow";
                    string a = "Integer";
                    return a;
                }
                typeLexem = "Integer";
                return value.ToString();
            }

            if (typeLexem == "int_x16")
            {
                lexem = lexem.Replace("$", "");
                int value;
                try
                {
                    value = Convert.ToInt32(lexem, 16);
                }
                catch (Exception)
                {
                    typeLexem = "ERROR_Overflow";
                    string a = "Integer";
                    return a;
                }
                typeLexem = "Integer";
                return value.ToString();
            }

            if (typeLexem == "real_number") // убираем не значащие 0
            {
                for (int i = 0; i < lexem.Length; i++)
                {
                    while (lexem[0] == '0' && lexem[1] != '.')
                    {
                        lexem = lexem.Substring(1);
                        if (lexem.Length == 1)
                        {
                            break;
                        }
                    }
                }

            }

            if (typeLexem == "real_number") // перевод real
            {
                double valueReal = 0;
                string value = "";
                lexem = lexem.Replace(".", ",");
                valueReal = Convert.ToDouble(lexem);
                return value = valueReal.ToString("E10").Replace(",", ".");
            }

            if (typeLexem == "int_x10") // убираем не значащие 0
            {
                for (int i = 0; i < lexem.Length; i++)
                {
                    while (lexem[0] == '0')
                    {
                        lexem = lexem.Substring(1);
                        if (lexem.Length == 1)
                        {
                            break;
                        }
                    }
                }

            }

            if (typeLexem == "string")
            {
                var value = lexem.Replace("'", ""); ;
                return value.ToString();
            }

            /*if (typeLexem == "endFile")
            {
                var value = lexem.ToLower();
                return value.ToString();
            }*/

            return lexem;
        }
    }
}