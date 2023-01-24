using System;
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
        SymTableStack symTableStack = new SymTableStack();

        public BigParser(LexicalAnalyzer l)
        {
            lexer = l;
            currentLex = lexer.GetLex();
            Dictionary<string, Symbol> builts = new Dictionary<string, Symbol>();
            builts.Add("integer", new SymInteger("integer", new Node(TypeNode.Type, "integer", new List<Node> { })));
            builts.Add("string", new SymString("string", new Node(TypeNode.Type, "string", new List<Node> { })));
            builts.Add("real", new SymReal("real", new Node(TypeNode.Type, "real", new List<Node> { })));
            symTableStack.AddTable(new SymTable(builts));
        }
        void NextLex()
        {
            currentLex = lexer.GetLex();
        }

        private void Require(TypeLex typeLex, string value)
        {
            if (currentLex.typeLex != typeLex || currentLex.value != value)
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected '{value}'");
            }
        }
        private void Require(TypeLex typeLex)
        {
            if (currentLex.typeLex != typeLex)
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected {typeLex}");
            }
        }

        public Node ParseProgram()
        {
            string name = "";
            List<Node> types = new List<Node>();
            List<Node> body;
            if (currentLex.typeLex == TypeLex.Key_Word && currentLex.value == "program")
            {
                name = ParseProgramName();
            } 
            types = ParseDefs();
            Require(TypeLex.Key_Word, "begin");
            body = new List<Node>(types);
            body.Add(ParseBlock());
            Require(TypeLex.Separators, ".");
            NextLex();
            return new Node(TypeNode.MainProgram, $"program {name}", body);
        }
        public string ParseProgramName()
        {
            string result;
            NextLex();
            Require(TypeLex.Identifier);
            result = currentLex.value;
            NextLex();
            Require(TypeLex.Separators, ";");
            NextLex();
            return result;
        }
        public List<Node> ParseDefs()
        {
            List<Node> types = new List<Node>();
            while (currentLex.typeLex == TypeLex.Key_Word)
            {
                switch (currentLex.value)
                {
                    case "var":
                        types.Add(ParseVar());
                        continue;
                    case "const":
                        types.Add(ParseConst());
                        continue;
                    default:
                        return types;
                }
            }
            return types;
        }
        public Node ParseVar()
        {
            List<Node> body = new List<Node>();
            NextLex();
            do
            {
                List<Node> bodyVarDef = new List<Node>();
                SymType symType;
                do
                {
                    if (currentLex.typeLex == TypeLex.Separators && currentLex.value == ",")
                    {
                        NextLex();
                    }
                    Require(TypeLex.Identifier);
                    bodyVarDef.Add(new Node(TypeNode.Var, currentLex.value, new List<Node> { }));
                    NextLex();
                }
                while (currentLex.typeLex == TypeLex.Separators && currentLex.value == ",");

                Require(TypeLex.Operation, ":");
                NextLex();
                Require(TypeLex.Identifier);
                try
                {
                    symType = (SymType)symTableStack.Get(currentLex.value);
                    bodyVarDef.Add(symType); // нужно для общего типа объвленных переменных
                }
                catch (Exception ex)
                {
                    throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) {ex.Message}");
                }
                NextLex();
                body.Add(new Node(TypeNode.VarDef, ":", bodyVarDef));
                Require(TypeLex.Separators, ";");
                for (int i = 0; i < bodyVarDef.Count - 1; i++)
                {
                    symTableStack.Add(bodyVarDef[i].value, new SymVarGlobal(bodyVarDef[i].value, symType, bodyVarDef[i])); // добавление в таблицу с типом
                }
                NextLex();
            }
            while (currentLex.typeLex == TypeLex.Identifier);
            return new Node(TypeNode.Var, "var", body);
        }
        public Node ParseConst()
        {
            List<Node> body = new List<Node>();
            NextLex();
            Require(TypeLex.Identifier);
            while (currentLex.typeLex == TypeLex.Identifier)
            {
                Node name = new Node(TypeNode.Var, currentLex.value, new List<Node> { });
                NextLex();
                Require(TypeLex.Operation, "=");
                NextLex();
                Node value = ParseExpression();
                Require(TypeLex.Separators, ";");
                body.Add(new Node(TypeNode.ConstDef, "=", new List<Node> { name, value }));
                NextLex();
            }
            return new Node(TypeNode.Const, "const", body);
        }
        public Node ParseStatements()
        {
            Node result;
            if (currentLex.typeLex == TypeLex.Identifier)
            {
                result = ParseAssigment();
            }
            else
            {
                //structStmt
                if (currentLex.typeLex == TypeLex.Key_Word || (currentLex.typeLex == TypeLex.Separators && currentLex.value == ";"))
                {
                    switch (currentLex.value)
                    {
                        case "begin":
                            result = ParseBlock();
                            break;
                        case "if":
                            result = ParseIf();
                            break;
                        case "for":
                            result = ParseFor();
                            break;
                        case "while":
                            result = ParseWhile();
                            break;
                        case "repeat":
                            result = ParseRepeat();
                            break;
                        case ";":
                            result = new Node(TypeNode.NullStmt, "", new List<Node>());
                            break;
                        default:
                            throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected statements");
                    }
                }
                else
                {
                    throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected statements");
                }
            }
            return result;
        }
        public Node ParseAssigment()
        {
            string currentValue;

            Require(TypeLex.Identifier);
            Symbol symVar;
            try
            {
                symVar = symTableStack.Get(currentLex.value);
            }
            catch (Exception ex)
            {
                throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) {ex.Message}");
            }
            if (symVar.GetType() != typeof(SymVarGlobal))
            {
                throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) Undefined variable");
            }
            Node var = new SymVar(currentLex.value, ((SymVar)symVar).GetTypeVar(), new Node(TypeNode.Var, currentLex.value, new List<Node> { })); // var - переменная которая выводится на экран
            NextLex();
            if (!(currentLex.typeLex == TypeLex.Operation && (currentLex.value == ":=" || currentLex.value == "*=" || currentLex.value == "/=" || currentLex.value == "+=" || currentLex.value == "-=")))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected operation sign");
            }
            currentValue = currentLex.value;
            NextLex();
            return new AssigmentNode(TypeNode.Assigment, currentValue, new List<Node> { var, ParseSimpleExpression()} );
        }
        public Node ParseBlock()
        {
            List<Node> stmt = new List<Node> { };
            Require(TypeLex.Key_Word, "begin");
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
            Require(TypeLex.Key_Word, "end");
            NextLex();

            List<Node> children = new List<Node>(stmt);

            return new Node(TypeNode.Block, "begin", children);

        }
        public Node ParseWhile()
        {
            Require(TypeLex.Key_Word, "while");
            NextLex();
            Node exp = ParseExpression();
            Require(TypeLex.Key_Word, "do");
            NextLex();
            Node stmt = ParseStatements();

            return new Node(TypeNode.While, "while", new List<Node> { exp, stmt });
        }
        public Node ParseIf()
        {
            Require(TypeLex.Key_Word, "if");
            NextLex();
            Node exp = ParseExpression();

            Require(TypeLex.Key_Word, "then");
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

            Require(TypeLex.Key_Word, "repeat");
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
                stmt.Add(ParseAssigment());
            }

            Require(TypeLex.Key_Word, "until");
            NextLex();

            Node exp = ParseExpression();
            List<Node> children = new List<Node> (stmt);
            children.Add (exp);

            return new Node(TypeNode.Repeat, "repeat", children);
        }
        public Node ParseFor()
        {
            
            NextLex();
            Node stmt = ParseAssigment();
            //NextLex();
            if (!(currentLex.typeLex == TypeLex.Key_Word && (currentLex.value == "to" || currentLex.value == "downto")))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected Key_Word to or downto");
            }
            NextLex();
            Node to = new Node(TypeNode.To, "to", new List<Node> { ParseSimpleExpression() });
            Require(TypeLex.Key_Word, "do");
            NextLex();
            Node stmt2 = ParseStatements();

            return new Node(TypeNode.For, "for", new List<Node> { stmt, to, stmt2 });

        }
        public Node ParseExpression()
        {
            Node left = ParseSimpleExpression();
            if (currentLex.typeLex == TypeLex.Operation && (currentLex.value == "<" || currentLex.value == "<=" || currentLex.value == ">" || currentLex.value == ">=" || currentLex.value == "=" || currentLex.value == "<>"))
            {
                LexicalAnalyzer.Lex op = currentLex;
                NextLex();
                left = new BinOpNode(TypeNode.BinOp, op.value, new List<Node> { left, ParseSimpleExpression() });
            }
            return left;
        }
        public Node ParseSimpleExpression()
        {
            Node left = ParseTerm();
            while (currentLex.typeLex == TypeLex.Operation && (currentLex.value == "+" || currentLex.value == "-" || currentLex.value == "or" || currentLex.value == "xor"))
            {
                LexicalAnalyzer.Lex op = currentLex;
                currentLex = lexer.GetLex();
                left = new BinOpNode(TypeNode.BinOp, op.value, new List<Node> { left, ParseTerm() });
            }
            return left;
        }
        public Node ParseTerm()
        {
            Node left = ParseFactor();
            while (currentLex.typeLex == TypeLex.Operation && (currentLex.value == "*" || currentLex.value == "/" || currentLex.value == "end"))
            {
                LexicalAnalyzer.Lex op = currentLex;
                currentLex = lexer.GetLex();
                left = new BinOpNode(TypeNode.BinOp, op.value, new List<Node> { left, ParseFactor() });
            }
            return left;
        }
        public Node ParseFactor()
        {
            if (currentLex.typeLex == TypeLex.Separators && (currentLex.value == "("))
            {
                currentLex = lexer.GetLex();

                Node e = ParseExpression();
                Require(TypeLex.Separators, ")");
                currentLex = lexer.GetLex();
                return e;
            }

            if (currentLex.typeLex == TypeLex.Identifier)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                Symbol var;
                try
                {
                    var = symTableStack.Get(factor.value);
                }
                catch (Exception ex)
                {
                    throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) {ex.Message}");
                }
                if(var.GetType() != typeof(SymVarGlobal))
                {
                    throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) Undefined variable");
                }
                return new SymVar(factor.value, ((SymVar)var).GetTypeVar(), new Node(TypeNode.Var, factor.value, new List<Node> { }));
            }
            if (currentLex.typeLex == TypeLex.Integer)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                return new SymInteger("integer", new Node(TypeNode.Integer, factor.value, new List<Node> { }));
            }
            if (currentLex.typeLex == TypeLex.String)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                return new SymString("string", new Node(TypeNode.String, factor.value, new List<Node> { }));
            }
            if (currentLex.typeLex == TypeLex.Real)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                return new SymReal("real", new Node(TypeNode.Real, factor.value, new List<Node> { }));
            }

            throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) ERROR: don't have factor");
        }
    }
}
