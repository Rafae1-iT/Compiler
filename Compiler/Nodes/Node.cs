using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Node
    {
        public Node() { }
        public virtual string ToPrint(string prefix)
        {
            return "node";
        }
    }
    public partial class NodeMainProgram : Node
    {
        string name;
        List<NodeDefs> types;
        BlockStmt body;
        public NodeMainProgram(string name, List<NodeDefs> types, BlockStmt body)
        {
            this.name = name;
            this.types = types;
            this.body = body;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"program {name}\r\n";
            foreach (NodeDefs type in types)
            {
                res += prefix + $"├─── {type.ToPrint(prefix + "    ")}\r\n";
            }
            res += prefix + $"└─── {body.ToPrint(prefix + "    ")}";
            return res;
        }
    }
    public partial class NodeProgram : Node
    {
        List<NodeDefs> types;
        BlockStmt body;
        public NodeProgram(List<NodeDefs> types, BlockStmt body)
        {
            this.types = types;
            this.body = body;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"program\r\n";
            foreach (NodeDefs? type in types)
            {
                if (type != null)
                {
                    res += prefix + $"├─── {type.ToPrint(prefix + "    ")}\r\n";
                }
            }
            res += prefix + $"└─── {body.ToPrint(prefix + "    ")}";
            return res;
        }
    }
}