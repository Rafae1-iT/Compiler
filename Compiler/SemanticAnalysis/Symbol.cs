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
        public Symbol(string name, Node node) : base(node.type, node.value, node.children)
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
        public SymVar(string name, SymType type, Node node) : base(name, node)
        {
            this.symType = type;
        }
    }
    public class SymVarConst : SymVar
    {
        public SymVarConst(string name, SymType type, Node node) : base(name, type, node) { }
    }
    public class SymVarGlobal : SymVar
    {
        public SymVarGlobal(string name, SymType type, Node node) : base(name, type, node) { }
    }
}