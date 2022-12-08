﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class BigParser
    {
        LexicalAnalyzer lexer;
        public LexicalAnalyzer.Lex currentLex;

        public BigParser(LexicalAnalyzer l)
        {
            lexer = l;
            currentLex = lexer.GetLex();
        }
        void NextLex()
        {
            currentLex = lexer.GetLex();
        }

        public Node ParseStatements()
        {
            string currentValue;
            if(currentLex.typeLex != TypeLex.Identifier)
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected identifier");
            }
            Node var = new Node(TypeNode.Var, currentLex.value, new List<Node>()); // var - variable переменная которая выводится на экран
            NextLex();
            if (!(currentLex.typeLex == TypeLex.Operation && (currentLex.value == ":=" || currentLex.value == "*=" || currentLex.value == "/=" || currentLex.value == "+=" || currentLex.value == "-=")))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected operation sign");
            }
            currentValue = currentLex.value;
            NextLex();
            return new Node(TypeNode.Statemant, currentValue, new List<Node> { var, ParseExpression()} );
        }

        public Node ParseWhile()
        {
            if(!(currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "while"))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word");
            }
            NextLex();
            Node exp = ParseExpression();
            if (!(currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "do"))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word");
            }
            NextLex();
            Node stmt = ParseStatements();

            return new Node(TypeNode.While, "while", new List<Node> { exp, stmt });
        }

        public Node ParseIf()
        {

            if (!(currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "if"))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word");
            }
            NextLex();
            Node exp = ParseExpression();

            if (!(currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "then"))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word");
            }
            NextLex();
            Node stmt = ParseStatements();
            if (currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "else")
            {
                NextLex();
                Node elseStmt = new Node( TypeNode.Else, "else", new List<Node> { ParseStatements() });
                return new Node(TypeNode.If, "if", new List<Node> { exp, stmt, elseStmt });
            }

             return new Node(TypeNode.If, "if", new List<Node> { exp, stmt });

        }

        public Node ParseRepeat()
        {
            List<Node> stmt = new List<Node> { };

            if (!(currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "repeat"))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word repeat");
            }
            NextLex();

            if (!(currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "until"))
            {
                stmt.Add(ParseStatements());
            }
            while (currentLex.typeLex == TypeLex.Separators && currentLex.value == ";")
            {
                NextLex();
                if (currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "until")
                {
                    break;
                }
                stmt.Add(ParseStatements());
            }

            if (currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "until")
            {
                NextLex();
            }
            else
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word until");
            }
            Node exp = ParseExpression();
            List<Node> children = new List<Node> (stmt);
            children.Add (exp);

            return new Node(TypeNode.Repeat, "repeat", children);
        }

        public Node ParseBlock()
        {
            List<Node> stmt = new List<Node> { };
            if (!(currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "begin"))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word begin");
            }
            NextLex();

            while (!(currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "end"))
            {
                stmt.Add(ParseStatements());

                if (currentLex.typeLex == TypeLex.Separators && currentLex.value == ";")
                {
                    NextLex();
                }
                else
                {
                    if (!(currentLex.typeLex == TypeLex.Key_Word))
                    {
                        throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected ;");
                    }
                }
            }
            if (currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "end")
            {
                NextLex();
            }
            else
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key Word end ");
            }

            List<Node> children = new List<Node>(stmt);

            return new Node(TypeNode.Block, "begin", children);

        }

        public Node ParseFor()
        {
            //List<Node> stmt = new List<Node> { };
            if (!(currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "for"))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word for");
            }
            NextLex();
            Node stmt = ParseStatements();
            //NextLex();
            if (!(currentLex.typeLex == TypeLex.Key_Word && (currentLex.value == "to" || currentLex.value == "downto")))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word to or downto");
            }
            NextLex();
            Node to = new Node(TypeNode.To, "to", new List<Node> { ParseExpression() });
            if (!(currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "do"))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word do");
            }
            NextLex();
            Node stmt2 = ParseStatements();

            return new Node(TypeNode.For, "for", new List<Node> { stmt, to, stmt2 });

        }


        public Node ParseExpression()
        {
            Node left = ParseTerm();
            while (currentLex.typeLex == TypeLex.Operation && (currentLex.value == "+" || currentLex.value == "-"))
            {
                LexicalAnalyzer.Lex op = currentLex;
                currentLex = lexer.GetLex();
                left = new Node(TypeNode.BinOp, op.value, new List<Node> { left, ParseTerm() });
            }
            return left;
        }

        public Node ParseTerm()
        {
            Node left = ParseFactor();
            while (currentLex.typeLex == TypeLex.Operation && (currentLex.value == "*" || currentLex.value == "/"))
            {
                LexicalAnalyzer.Lex op = currentLex;
                currentLex = lexer.GetLex();
                left = new Node(TypeNode.BinOp, op.value, new List<Node> { left, ParseFactor() });
            }
            return left;
        }

        public Node ParseFactor()
        {
            if (currentLex.typeLex == TypeLex.Separators && (currentLex.value == "("))
            {
                currentLex = lexer.GetLex();

                Node e = ParseExpression();
                if (currentLex.typeLex == TypeLex.Separators && currentLex.value == ")")
                {
                    currentLex = lexer.GetLex();
                }
                else
                {
                    throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) ERROR: don't have ')'");
                }
                return e;
            }

            if (currentLex.typeLex == TypeLex.Identifier)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                return new Node(TypeNode.Var, factor.value, new List<Node> { });
            }
            if (currentLex.typeLex == TypeLex.Integer)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                return new Node(TypeNode.Integer, factor.value, new List<Node> { });
            }
            if (currentLex.typeLex == TypeLex.Real)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                return new Node(TypeNode.Real, factor.value, new List<Node> { }); ;
            }

            throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) ERROR: don't have factor");
        }
    }
}