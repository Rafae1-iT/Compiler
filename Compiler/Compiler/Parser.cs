
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Node
    {
        public string type;
        public string value;
        public List<Node?> children;
        public Node(string type, string value, List<Node?> children)
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
            while (currentLex.typeLex == "operation" && (currentLex.value == "+" || currentLex.value == "-"))
            {
                LexicalAnalyzer.Lex op = currentLex;
                currentLex = lexer.GetLex();
                left = new Node("binOp", op.value, new List<Node?> { left, ParseTerm() });
            }
            return left;
        }

        public Node ParseTerm()
        {
            Node left = ParseFactor();
            while (currentLex.typeLex == "operation" && (currentLex.value == "*" || currentLex.value == "/"))
            {
                LexicalAnalyzer.Lex op = currentLex;
                currentLex = lexer.GetLex();
                left = new Node("binOp", op.value, new List<Node?> { left, ParseFactor() });
            }
            return left;
        }

        public Node ParseFactor()
        {
            if (currentLex.typeLex == "separators" && (currentLex.value == "("))
            {
                currentLex = lexer.GetLex();

                Node e = ParseExpression();
                if (currentLex.typeLex == "separators" && currentLex.value == ")")
                {
                    currentLex = lexer.GetLex();
                } 
                else
                {
                    throw new Exception ($"({currentLex.line_number},{currentLex.numLexStart}) ERROR: don't have ')'");
                }
                return e;
            }

            if (currentLex.typeLex == "Integer" || currentLex.typeLex == "real" || currentLex.typeLex == "identifier")
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                return new Node(factor.typeLex, factor.value, new List<Node?> { null });
            }

            throw new Exception ($"({currentLex.line_number},{currentLex.numLexStart}) ERROR: don't have factor");
        }
    }
}