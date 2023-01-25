using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Symbol : Node
    {
        string name;
        public string GetName()
        {
            return name;
        }
        public Symbol(string name)
        {
            this.name = name;
        }
    }
    public class SymVar : Symbol
    {
        public SymType symType;
        public SymType GetTypeVar()
        {
            return symType;
        }
        public SymVar(string name, SymType type) : base(name)
        {
            this.symType = type;
        }
    }
    public class SymVarConst : SymVar
    {
        public SymVarConst(string name, SymType type) : base(name, type) { }
    }
    public class SymVarGlobal : SymVar
    {
        public SymVarGlobal(string name, SymType type) : base(name, type) { }
    }
    public class SymVarLocal : SymVar
    {
        public SymVarLocal(string name, SymType type) : base(name, type) { }
    }
    public class SymProc : Symbol
    {
        List<SymVar> args;
        SymTable locals;
        BlockStmt body;
        public List<SymVar> GetParams()
        {
            return args;
        }
        public SymTable GetLocals()
        {
            return locals;
        }
        public BlockStmt GetBody()
        {
            return body;
        }
        public SymProc(string name, List<SymVar> args, SymTable locals, BlockStmt body) : base(name)
        {
            this.args = args;
            this.locals = locals;
            this.body = body;
        }
        public SymProc(string name) : base(name)
        {
            this.args = new List<SymVar>();
            this.locals = new SymTable(new Dictionary<string, Symbol>());
            this.body = new BlockStmt(new List<NodeStatement>());
        }
    }
}