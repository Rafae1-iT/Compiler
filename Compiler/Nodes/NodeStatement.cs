using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class NodeStatement : Node { }
    public class NullStmt : NodeStatement
    {
        public NullStmt() { }
        public override string ToPrint(string prefix)
        {
            return prefix + "";
        }
    }
    public partial class AssignmentStmt : NodeStatement
    {
        OperationSign opname;
        NodeExpression left;
        NodeExpression right;
        public AssignmentStmt(OperationSign opname, NodeExpression left, NodeExpression right)
        {
            this.opname = opname;
            this.left = left;
            this.right = right;
            if (left.GetCachedType().GetType() != right.GetCachedType().GetType())
            {
                if (left.GetCachedType().GetType() == typeof(SymReal) && right.GetCachedType().GetType() == typeof(SymInteger))
                {
                    this.right = new NodeCast(left.GetCachedType(), right);
                }
                else
                {
                    throw new Exception($"Incompatible types");
                }
            }
        }
        public override string ToPrint(string prefix)
        {
            string res;
            string opnameStr = "";
            if (opname.GetType() == typeof(OperationSign))
            {
                opnameStr = opname.ToString();
            }
            res = $"{opnameStr}\r\n";
            res += prefix + $"├─── {left.ToPrint(prefix + "    ")}\r\n";
            res += prefix + $"└─── {right.ToPrint(prefix + "    ")}";
            return res;
        }
    }
    public partial class CallStmt : NodeStatement
    {
        public SymProc proc;
        List<NodeExpression?> args;
        public CallStmt(Symbol proc, List<NodeExpression?> arg)
        {
            this.proc = (SymProc)proc;
            this.args = arg;
        }
        public CallStmt(SymProc proc, List<NodeExpression?> arg)
        {
            this.proc = proc;
            this.args = arg;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"{proc.GetName()}";
            if (args != null && args.Count > 0)
            {
                res += prefix + $"\r\n";
                int i = 1;
                foreach (NodeExpression? arg in args)
                {
                    if (i == args.Count)
                    {
                        if (arg != null)
                        {
                            res += prefix + $"└─── {arg.ToPrint(prefix + "    ")}";
                        }
                    }
                    else
                    {
                        if (arg != null)
                        {
                            res += prefix + $"├─── {arg.ToPrint(prefix + "    ")}\r\n";
                        }
                        i++;
                    }
                }
            }
            return res;
        }
    }
    public partial class IfStmt : NodeStatement
    {
        NodeExpression condition;
        NodeStatement body;
        NodeStatement elseBody;
        public IfStmt(NodeExpression condition, NodeStatement body, NodeStatement elseBody)
        {
            this.condition = condition;
            this.body = body;
            this.elseBody = elseBody;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"if\r\n";
            res += prefix + $"├─── {condition.ToPrint(prefix + "    ")}\r\n";
            res += prefix + $"├─── {body.ToPrint(prefix + "    ")}\r\n";
            res += prefix + $"└─── {elseBody.ToPrint(prefix + "    ")}";
            return res;
        }
    }
    public partial class WhileStmt : NodeStatement
    {
        NodeExpression condition;
        NodeStatement body;
        public WhileStmt(NodeExpression condition, NodeStatement body)
        {
            this.condition = condition;
            this.body = body;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"while\r\n";
            res += prefix + $"├─── {condition.ToPrint(prefix + "    ")}\r\n";
            res += prefix + $"└─── {body.ToPrint(prefix + "    ")}";
            return res;
        }
    }
    public partial class ForStmt : NodeStatement
    {
        NodeStatement startStmt;
        KeyWord toOrDownto;
        NodeExpression finalVal;
        NodeStatement body;
        public ForStmt(NodeStatement startStmt, KeyWord toOrDownto, NodeExpression finalVal, NodeStatement body)
        {
            this.startStmt = startStmt;
            this.toOrDownto = toOrDownto;
            this.finalVal = finalVal;
            this.body = body;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"for\r\n";
            res += prefix + $"├─── {startStmt.ToPrint(prefix + "    ")}" +
                   prefix + $"├─── {toOrDownto.ToString().ToLower()}\r\n" +
                   prefix + $"│    └─── {finalVal.ToPrint(prefix + "    ")}\r\n" +
                   prefix + $"└─── {body.ToPrint(prefix + "    ")}";
            return res;
        }
    }
    public partial class RepeatStmt : NodeStatement
    {
        NodeExpression condition;
        List<NodeStatement> body;
        public RepeatStmt(List<NodeStatement> body, NodeExpression condition)
        {
            this.condition = condition;
            this.body = body;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"repeat\r\n";
            foreach (NodeStatement? stmt in body)
            {
                res += prefix + $"├─── {stmt.ToPrint(prefix + "    ")}\r\n";
            }
            res += prefix + $"└─── {condition.ToPrint(prefix + "    ")}";
            return res;
        }
    }
    public partial class BlockStmt : NodeStatement
    {
        public List<NodeStatement> body;
        public BlockStmt(List<NodeStatement> body)
        {
            this.body = body;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"begin\r\n";
            foreach (NodeStatement? stmt in body)
            {
                res += prefix + $"├─── {stmt.ToPrint(prefix + "    ")}\r\n";
            }
            res += prefix + $"└─── end";
            return res;
        }
    }
}