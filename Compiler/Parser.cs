using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public enum TypeNode
    {
        BinOp,
        Var,
        Result,
        Statemant,
        Integer,
        Real,
        While,
        If,
        Else,
        Repeat,
        Block,
        For,
        To,
        NullStmt,
        MainProgram,
        Const,
        ConstDef,
        VarDef,
        Type,
    }
    public class Node
    {
        public TypeNode type;
        public string value;
        public List<Node> children;
        public Node(TypeNode type, string value, List<Node> children)
        {
            this.type = type;
            this.value = value;
            this.children = children;
        }

    }
    public class Parser
    {

        LexicalAnalyzer lexer;
        public LexicalAnalyzer.Lex currentLex;
        public Parser(LexicalAnalyzer l)
        {
            lexer = l;
            currentLex = lexer.GetLex();
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
                    throw new Exception ($"({currentLex.line_number},{currentLex.numLexStart}) ERROR: don't have ')'");
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
                return new Node(TypeNode.Real, factor.value, new List<Node> { });
            }

            throw new Exception ($"({currentLex.line_number},{currentLex.numLexStart}) ERROR: expected factor");
        }
    }
}