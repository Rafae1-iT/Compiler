using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class SymType : Symbol
    {
        public SymType(string name, Node node) : base(name, node) { }
    }
    public class SymInteger : SymType
    {
        public SymInteger(string name, Node node) : base(name, node) { }
    }
    public class SymReal : SymType
    {
        public SymReal(string name, Node node) : base(name, node) { }
    }
    public class SymString : SymType
    {
        public SymString(string name, Node node) : base(name, node) { }
    }
    public class SymBoolean : SymType
    {
        public SymBoolean(string name, Node node) : base(name, node) { }
    }

    public class SymTypeAlias : SymType
    {
        SymType original;
        public SymType GetOriginalType()
        {
            return original;
        }
        public SymTypeAlias(string name, SymType original, Node node) : base(name, node)
        {
            this.original = original;
        }
    }
}