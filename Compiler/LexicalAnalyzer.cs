using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public enum TypeLex
    {
        Integer,
        String,
        Real,
        Eof,
        Space,
        Comments,
        Error,
        Identifier,
        Key_Word,
        Char,
        Operation,
        Separators
    }
    public enum KeyWord
    {
        Unidentified,
        AND,
        ARRAY,
        AS,
        ASM,
        BEGIN,
        CASE,
        CONST,
        CONSTRUCTOR,
        DESTRUCTOR,
        DIV,
        DO,
        DOWNTO,
        ELSE,
        END,
        FILE,
        FOR,
        FOREACH,
        FUNCTION,
        GOTO,
        IMPLEMENTATION,
        IF,
        IN,
        INHERITED,
        INLINE,
        INTERFACE,
        LABEL,
        MOD,
        NIL,
        NOT,
        OBJECT,
        OF,
        OPERATOR,
        OR,
        PACKED,
        PROCEDURE,
        PROGRAM,
        RECORD,
        REPEAT,
        SELF,
        SET,
        SHL,
        SHR,
        STRING,
        THEN,
        TO,
        TYPE,
        UNIT,
        UNTIL,
        USES,
        VAR,
        WHILE,
        WITH,
        XOR,
        DISPOSE,
        EXIT,
        FALSE,
        NEW,
        TRUE,
        CLASS,
        DISPINTERFACE,
        EXCEPT,
        EXPORTS,
        FINALIZATION,
        FINALLY,
        INITIALIZATION,
        IS,
        LIBRARY,
        ON,
        OUT,
        PROPERTY,
        RAISE,
        RESOURCESTRING,
        THREADVAR,
        TRY
    }
    public enum OperationSign
    {
        Unidentified,
        Equal, // =
        Colon, // :
        Plus, // +
        Minus, // -
        Multiply, // *
        Divide, // /
        Greater, //>
        Less, //<
        At, // @
        BitwiseShiftToTheLeft, // <<
        BitwiseShiftToTheRight, //>>
        NotEqual, //<>
        SymmetricalDifference, // ><
        LessOrEqual, // <=
        GreaterOrEqual, // >=
        Assignment, // :=
        Addition, // +=
        Subtraction, // -=
        Multiplication, // *=
        Division, // /=
        PointRecord, // .
    }
    public enum Separator
    {
        Unidentified,
        Comma, // ,
        Semiсolon, // ;
        OpenParenthesis, // (
        CloseParenthesis, // )
        OpenBracket, // [
        CloseBracket, // ]
        Point, // .
        DoublePoint // ..
    }
    public class LexicalAnalyzer
    {
        public struct Lex
        {
            public int line_number;
            public int numLexStart;
            public TypeLex typeLex;
            public object value;
            public string lex;

            public Lex(int line_number, int numLexStart, TypeLex typeLex, object value, string lex)
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
            "shl", "shr", "then", "to", "type", "unit", "until", "uses", "var", "while", "with", "xor", "dispose", "exit",
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
        public static TypeLex GetTypeLex(string input, ref int lenLex, int numLexStart)
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
                if(keyWords.Contains(input.ToLower().Substring(0, lenLex)))
                {
                    statusDFA = 9;
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
                    return TypeLex.Error;
                case 1:
                    return TypeLex.Error;
                case 2:
                    return TypeLex.String;
                case 3:
                    return TypeLex.Integer;
                case 4:
                    return TypeLex.Integer;
                case 5:
                    return TypeLex.Integer;
                case 6:
                    return TypeLex.Integer;
                case 7:
                    return TypeLex.Real;
                case 8:
                    return TypeLex.Identifier;
                case 9:
                    return TypeLex.Key_Word;
                case 10:
                    return TypeLex.Char;
                case 11:
                    return TypeLex.Comments;
                case 12:
                    return TypeLex.Space;
                case 13:
                    return TypeLex.Operation;
                case 14:
                    return TypeLex.Separators;
                case 15:
                    return TypeLex.Eof;
            }

            return TypeLex.Error;
        }
        public Lex GetLex()
        {
            int lenLex = 0;
            Lex lex = new Lex(line_number, numLexStart, TypeLex.Eof, "Eof", "");
            if (line_number <= input.Count)
            {
                TypeLex typeLex = LexicalAnalyzer.GetTypeLex(input[line_number - 1], ref lenLex, numLexStart);
                object value = LexicalAnalyzer.GetValue(input[line_number - 1].Substring(0, lenLex), ref typeLex);

                if (typeLex == TypeLex.Error)
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

                while (lex.typeLex == TypeLex.Space || lex.typeLex == TypeLex.Comments)
                {
                    lex = GetLex();
                }
            }
            lastLex = lex;
            return lex;
        }

        public static object GetValue(string lexem, ref TypeLex typeLexem)
        {
            if (typeLexem == TypeLex.Integer && lexem[0] == '%')
            {
                lexem = lexem.Replace("%", "");
                int value;
                try
                {
                    value = Convert.ToInt32(lexem, 2);
                }
                catch (Exception)
                {
                    typeLexem = TypeLex.Error;
                    string a = "Integer";
                    return a;
                }
                typeLexem = TypeLex.Integer;
                return value;

            }

            if (typeLexem == TypeLex.Integer && lexem[0] == '&')
            {

                lexem = lexem.Replace("&", "");
                int value;
                try
                {
                    value = Convert.ToInt32(lexem, 8);
                }
                catch (Exception)
                {
                    typeLexem = TypeLex.Error;
                    string a = "Integer";
                    return a;
                }
                typeLexem = TypeLex.Integer;
                return value;
            }

            if (typeLexem == TypeLex.Integer)
            {
                int value;
                try
                {
                    value = Convert.ToInt32(lexem, 10);
                }
                catch (Exception)
                {
                    typeLexem = TypeLex.Error;
                    string a = "Integer";
                    return a;
                }
                typeLexem = TypeLex.Integer;
                return value;
            }

            if (typeLexem == TypeLex.Integer && lexem[0] == '$')
            {
                lexem = lexem.Replace("$", "");
                int value;
                try
                {
                    value = Convert.ToInt32(lexem, 16);
                }
                catch (Exception)
                {
                    typeLexem = TypeLex.Error;
                    string a = "Integer";
                    return a;
                }
                typeLexem = TypeLex.Integer;
                return value;
            }

            if (typeLexem == TypeLex.Real) // убираем не значащие 0
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

            if (typeLexem == TypeLex.Real) // перевод real
            {
                double valueReal = 0;
                lexem = lexem.Replace(".", ",");
                valueReal = Convert.ToDouble(lexem);
                return valueReal;
            }

            if (typeLexem == TypeLex.Integer) // убираем не значащие 0
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

            if (typeLexem == TypeLex.String)
            {
                string value = lexem.Replace("'", "");
                return value;
            }
            if (typeLexem == TypeLex.Key_Word)
            {
                lexem = lexem.ToLower();
                switch (lexem)
                {
                    case "end":
                        return KeyWord.END;
                    case "begin":
                        return KeyWord.BEGIN;
                    case "const":
                        return KeyWord.CONST;
                    case "var":
                        return KeyWord.VAR;
                    case "program":
                        return KeyWord.PROGRAM;
                    case "procedure":
                        return KeyWord.PROCEDURE;
                    default:
                        return KeyWord.Unidentified;
                }
            }
            if (typeLexem == TypeLex.Operation)
            {
                switch (lexem)
                {
                    case "=":
                        return OperationSign.Equal;
                    case ":":
                        return OperationSign.Colon;
                    case "+":
                        return OperationSign.Plus;
                    case "-":
                        return OperationSign.Minus;
                    case "*":
                        return OperationSign.Multiply;
                    case "/":
                        return OperationSign.Divide;
                    case ">":
                        return OperationSign.Greater;
                    case "<":
                        return OperationSign.Less;
                    case "@":
                        return OperationSign.At;
                    case "<<":
                        return OperationSign.BitwiseShiftToTheLeft;
                    case ">>":
                        return OperationSign.BitwiseShiftToTheRight;
                    case "<>":
                        return OperationSign.NotEqual;
                    case "><":
                        return OperationSign.SymmetricalDifference;
                    case "<=":
                        return OperationSign.LessOrEqual;
                    case ">=":
                        return OperationSign.GreaterOrEqual;
                    case ":=":
                        return OperationSign.Assignment;
                    case "+=":
                        return OperationSign.Addition;
                    case "-=":
                        return OperationSign.Subtraction;
                    case "*=":
                        return OperationSign.Multiplication;
                    case "/=":
                        return OperationSign.Division;
                    default:
                        return OperationSign.Unidentified;
                }
            }
            if (typeLexem == TypeLex.Separators)
            {
                switch (lexem)
                {
                    case ",":
                        return Separator.Comma;
                    case ";":
                        return Separator.Semiсolon;
                    case "(":
                        return Separator.OpenParenthesis;
                    case ")":
                        return Separator.CloseParenthesis;
                    case "[":
                        return Separator.OpenBracket;
                    case "]":
                        return Separator.CloseBracket;
                    case ".":
                        return Separator.Point;
                    case "..":
                        return Separator.DoublePoint;
                    default:
                        return Separator.Unidentified;
                }
            }

            return lexem;
        }
    }
}