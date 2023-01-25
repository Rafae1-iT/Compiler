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
            builts.Add("integer", new SymInteger("integer"));
            builts.Add("string", new SymString("string"));
            builts.Add("real", new SymReal("real"));
            symTableStack.AddTable(new SymTable(builts));
        }
        void NextLex()
        {
            currentLex = lexer.GetLex();
        }

        private void Require(object value)
        {
            if (!Equals(currentLex.value, value))
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected {value}, but met {currentLex.value}");
            }
        }
        private void Require(TypeLex typeLex)
        {
            if (currentLex.typeLex != typeLex)
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected {typeLex}, but met {currentLex.typeLex} ({currentLex.value})");
            }
        }

        public NodeMainProgram ParseMainProgram()
        {
            string name = "";
            List<NodeDefs> types = new List<NodeDefs>();
            BlockStmt body;
            if ((currentLex.value is KeyWord keyWrd) && (keyWrd == KeyWord.PROGRAM))
            {
                name = ParseProgramName();
            } 
            types = ParseDefs();
            Require(KeyWord.BEGIN);
            body = ParseBlock();
            Require(Separator.Point);
            NextLex();
            return new NodeMainProgram(name, types, body);
        }
        public string ParseProgramName()
        {
            string result;
            NextLex();
            Require(TypeLex.Identifier);
            result = (string)currentLex.value;
            NextLex();
            Require(Separator.Semiсolon);
            NextLex();
            return result;
        }
        public List<NodeDefs> ParseDefs()
        {
            List<NodeDefs> types = new List<NodeDefs>();
            while (currentLex.typeLex == TypeLex.Key_Word)
            {
                switch (currentLex.value)
                {
                    case KeyWord.VAR:
                        types.Add(ParseVar());
                        continue;
                    case KeyWord.CONST:
                        types.Add(ParseConst());
                        continue;
                    case KeyWord.PROCEDURE:
                        types.Add(ParseProcedure());
                        continue;
                    default:
                        return types;
                }
            }
            return types;
        }
        public VarDefsNode ParseVar(bool inProcedureDeclaration = false)
        {
            List<VarDeclarationNode> body = new List<VarDeclarationNode>();
            NextLex();
            do
            {
                List<string> varNames = new List<string>();
                List<SymVar> bodyVarDef = new List<SymVar>();
                SymType symType;
                do
                {
                    if ((currentLex.value is Separator sep) && (sep == Separator.Comma))
                    {
                        NextLex();
                    }
                    Require(TypeLex.Identifier);
                    varNames.Add((string)currentLex.value);
                    NextLex();
                }
                while ((currentLex.value is Separator sep2) && (sep2 == Separator.Comma));

                Require(OperationSign.Colon);
                NextLex();
                Require(TypeLex.Identifier);
                try
                {
                    symType = (SymType)symTableStack.Get((string)currentLex.value);
                }
                catch (Exception ex)
                {
                    throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) {ex.Message}");
                }
                if (!inProcedureDeclaration)
                {
                    NextLex();
                    Require(Separator.Semiсolon);
                }
                for (int i = 0; i < varNames.Count; i++)
                {
                    bodyVarDef.Add(new SymVar(varNames[i], symType));
                    if (inProcedureDeclaration) symTableStack.Add(varNames[i], new SymVarLocal(varNames[i], symType)); // добавление в таблицу с типом
                    else symTableStack.Add(varNames[i], new SymVarGlobal(varNames[i], symType));
                }
                body.Add(new VarDeclarationNode(bodyVarDef, symType));
                NextLex();
            }
            while (currentLex.typeLex == TypeLex.Identifier);
            return new VarDefsNode(body);
        }
        public NodeDefs ParseConst()
        {
            List<ConstDeclarationNode> body = new List<ConstDeclarationNode>();
            NextLex();
            Require(TypeLex.Identifier);
            while (currentLex.typeLex == TypeLex.Identifier)
            {
                string name = (string)currentLex.value;
                NextLex();
                Require(OperationSign.Equal);
                NextLex();
                NodeExpression value = ParseExpression();
                Require(Separator.Semiсolon);

                SymVarConst symVarConst = new SymVarConst(name, value.GetCachedType());
                symTableStack.Add(name, symVarConst); // добавление в таблицу с типом

                body.Add(new ConstDeclarationNode(symVarConst, value ));
                NextLex();
            }
            return new ConstDefsNode(body);
        }
        public NodeDefs ParseProcedure()
        {
            string name;
            VarDefsNode paramsNode = new VarDefsNode(new List<VarDeclarationNode>());
            SymTable locals = new SymTable(new Dictionary<string, Symbol>());
            List<SymVar> args = new List<SymVar>();
            NextLex();
            Require(TypeLex.Identifier);
            name = (string)currentLex.value;
            symTableStack.FindDuplicate(name);
            NextLex();
            symTableStack.AddTable(locals);
            if ((currentLex.value is Separator sep) && (sep == Separator.OpenParenthesis))
            {
                paramsNode = ParseVar(inProcedureDeclaration: true);
                Require(Separator.CloseParenthesis);
                NextLex();
            }
            Require(Separator.Semiсolon);
            NextLex();
            foreach (VarDeclarationNode varDeclNode in paramsNode.body)
            {
                foreach (SymVar var in varDeclNode.GetVars())
                {
                    args.Add(var);
                }
            }
            List<NodeDefs> localsTypes = ParseDefs();
            locals = symTableStack.GetBackTable();
            BlockStmt body = ParseBlock();
            Require(Separator.Semiсolon);
            NextLex();
            symTableStack.PopBack();
            SymProc symProc = new SymProc(name, args, locals, body);
            symTableStack.Add(name, symProc);
            return new ProcedureDefNode(paramsNode, localsTypes, symProc);
        }
        public NodeStatement ParseStatements()
        {
            NodeStatement result;
            if (currentLex.typeLex == TypeLex.Identifier)
            {
                result = ParseAssigmentOrCall();
            }
            else
            {
                //structStmt
                switch (currentLex.value)
                {
                    case KeyWord.BEGIN:
                        result = ParseBlock();
                        break;
                    case KeyWord.IF:
                        result = ParseIf();
                        break;
                    case KeyWord.FOR:
                        result = ParseFor();
                        break;
                    case KeyWord.WHILE:
                        result = ParseWhile();
                        break;
                    case KeyWord.REPEAT:
                        result = ParseRepeat();
                        break;
                    case Separator.Semiсolon:
                        result = new NullStmt();
                        break;
                    default:
                        throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected statements");
                }
            }
            return result;
        }
        public NodeStatement ParseAssigmentOrCall()
        {
            OperationSign currentValue;

            Require(TypeLex.Identifier);
            Symbol symVar;
            try
            {
                symVar = symTableStack.Get((string)currentLex.value);
            }
            catch (Exception ex)
            {
                throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) {ex.Message}");
            }

            if (!((symVar is SymVarGlobal)||(symVar is SymVarLocal)))
            {
                if (symVar is SymProc)
                {
                    return ParseProcedureCall((string)currentLex.value);
                }
                throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) Undefined variable '{symVar.GetName()}'");
            }
            NodeExpression left = ParseFactor(); // var - переменная которая выводится на экран
            if ((currentLex.value is OperationSign op) && (op == OperationSign.Assignment || op == OperationSign.Addition ||
                 op == OperationSign.Subtraction || op == OperationSign.Multiplication || op == OperationSign.Division || op == OperationSign.PointRecord))
            {
                currentValue = op;
            }
            else
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected operation sign");
            }
            NextLex();
            return new AssignmentStmt(currentValue, left, ParseSimpleExpression() );
        }
        public NodeStatement ParseProcedureCall(string name)
        {
            List<NodeExpression?> parameter = new List<NodeExpression?>();
            SymProc proc;
            try
            {
                proc = (SymProc)symTableStack.Get(name);
            }
            catch
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Procedure not found \"{name}\"");
            }
            NextLex();
            if ((currentLex.value is Separator sep) && (sep == Separator.OpenParenthesis))
            {
                NextLex();
                while (!((currentLex.value is Separator sep2) && (sep2 == Separator.CloseParenthesis)))
                {
                    NodeExpression param = ParseSimpleExpression();
                    parameter.Add(param);
                    if ((currentLex.value is Separator sep3) && (sep3 == Separator.Comma))
                    {
                        NextLex();
                    }
                    else
                    {
                        break;
                    }
                }
                Require(Separator.CloseParenthesis);
                NextLex();
            }
            return new CallStmt(proc, parameter);
        }
        public BlockStmt ParseBlock()
        {
            List<NodeStatement> stmts = new List<NodeStatement> { };
            Require(KeyWord.BEGIN);
            NextLex();

            while (!(currentLex.value is KeyWord.END))
            {
                stmts.Add(ParseStatements());

                if ((currentLex.value is Separator sep) && (sep == Separator.Semiсolon))
                {
                    NextLex();
                }
                else
                {
                    if (!(currentLex.value is KeyWord))
                    {
                        throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected ;");
                    }
                }
            }
            Require(KeyWord.END);
            NextLex();

            return new BlockStmt(stmts);

        }
        public NodeStatement ParseWhile()
        {
            Require(KeyWord.WHILE);
            NextLex();
            NodeExpression exp = ParseExpression();
            Require(KeyWord.DO);
            NextLex();
            NodeStatement stmt = ParseStatements();

            return new WhileStmt(exp, stmt);
        }
        public NodeStatement ParseIf()
        {
            Require(KeyWord.IF);
            NextLex();
            NodeExpression condition = ParseExpression();

            Require(KeyWord.THEN);
            NextLex();
            NodeStatement stmt = ParseStatements();
            NodeStatement elseStmt = new NodeStatement();
            if ((currentLex.value is KeyWord keyWrd) && (keyWrd == KeyWord.ELSE))
            {
                NextLex();
                elseStmt = ParseStatements();
            }

             return new IfStmt(condition, stmt, elseStmt);

        }
        public NodeStatement ParseRepeat()
        {
            List<NodeStatement> stmt = new List<NodeStatement> { };

            Require(KeyWord.REPEAT);
            NextLex();

            if (!((currentLex.value is KeyWord keyWrd) && (keyWrd == KeyWord.UNTIL)))
            {
                stmt.Add(ParseStatements());
            }
            while ((currentLex.value is Separator sep) && (sep == Separator.Semiсolon))
            {
                NextLex();
                if ((currentLex.value is KeyWord keyWrd2) && !(keyWrd2 == KeyWord.UNTIL))
                {
                    break;
                }
                stmt.Add(ParseAssigmentOrCall());
            }

            Require(KeyWord.UNTIL);
            NextLex();

            NodeExpression condition = ParseExpression();

            return new RepeatStmt(stmt, condition);
        }
        public NodeStatement ParseFor()
        {
            NextLex();
            NodeStatement stmt = ParseAssigmentOrCall();
            KeyWord ToOrDownto;
            if (currentLex.value is KeyWord keyWord)
            {
                if (!(keyWord == KeyWord.TO || keyWord == KeyWord.DOWNTO))
                {
                    throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected 'to' or 'downto'");
                }
                ToOrDownto = (KeyWord)currentLex.value;
            }
            else
            {
                throw new Exception($"({lexer.line_number},{lexer.numLexStart - 1}) Expected 'to' or 'downto'");
            }
            NextLex();
            NodeExpression to = ParseSimpleExpression();
            Require(KeyWord.DO);
            NextLex();
            NodeStatement stmt2 = ParseStatements();

            return new ForStmt(stmt, ToOrDownto, to, stmt2 );
        }
        public NodeExpression ParseExpression()
        {
            NodeExpression left = ParseSimpleExpression();
            while (currentLex.value is OperationSign op)
            {
                if (op == OperationSign.Equal || op == OperationSign.NotEqual || op == OperationSign.Less || 
                    op == OperationSign.LessOrEqual || op == OperationSign.Greater || op == OperationSign.GreaterOrEqual)
                {
                    LexicalAnalyzer.Lex opSign = currentLex;
                    currentLex = lexer.GetLex();
                    left = new NodeBinOp(opSign.value, left, ParseSimpleExpression());
                }
                else
                {
                    break;
                }
            }
            return left;
        }
        public NodeExpression ParseSimpleExpression()
        {
            NodeExpression left = ParseTerm();
            while (currentLex.value is OperationSign op)
            {
                if (op == OperationSign.Plus || op == OperationSign.Minus)
                {
                    LexicalAnalyzer.Lex opSign = currentLex;
                    currentLex = lexer.GetLex();
                    left = new NodeBinOp(opSign.value, left, ParseTerm());
                }
                else
                {
                    break;
                }
            }
            return left;
        }
        public NodeExpression ParseTerm()
        {
            NodeExpression left = ParseFactor();
            while (currentLex.value is OperationSign op)
            {
                if (op == OperationSign.Multiply || op == OperationSign.Divide)
                {
                    LexicalAnalyzer.Lex opSign = currentLex;
                    currentLex = lexer.GetLex();
                    left = new NodeBinOp(opSign.value, left, ParseFactor());
                }
                else
                {
                    break;
                }
            }
            return left;
        }
        public NodeExpression ParseFactor()
        {
            if(currentLex.value is Separator sep)
            {
                if (sep == Separator.OpenParenthesis)
                {
                    currentLex = lexer.GetLex();

                    NodeExpression e = ParseExpression();
                    Require(Separator.CloseParenthesis);
                    currentLex = lexer.GetLex();
                    return e;
                }
            }

            if (currentLex.typeLex == TypeLex.Identifier)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                Symbol var;
                try
                {
                    var = symTableStack.Get((string)factor.value);
                }
                catch (Exception ex)
                {
                    throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) {ex.Message}");
                }
                if(!((var is SymVarGlobal)||(var is SymVarConst) || (var is SymVarLocal)))
                {
                    throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) Undefined variable");
                }
                return new NodeVar(new SymVar((string)factor.value, ((SymVar)var).GetTypeVar()));
            }
            if (currentLex.typeLex == TypeLex.Integer)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                return new NodeInt((int)factor.value);
            }
            if (currentLex.typeLex == TypeLex.String)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                return new NodeString((string)factor.value);
            }
            if (currentLex.typeLex == TypeLex.Real)
            {
                LexicalAnalyzer.Lex factor = currentLex;
                currentLex = lexer.GetLex();
                return new NodeReal((double)factor.value);
            }

            throw new Exception($"({currentLex.line_number},{currentLex.numLexStart}) ERROR: don't have factor");
        }
    }
}
