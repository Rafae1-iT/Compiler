using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class SymType : Symbol
    {
        public SymType(string name) : base(name) { }
    }
    public class SymInteger : SymType
    {
        public SymInteger(string name) : base(name) { }
    }
    public class SymReal : SymType
    {
        public SymReal(string name) : base(name) { }
    }
    public class SymString : SymType
    {
        public SymString(string name) : base(name) { }
    }
}