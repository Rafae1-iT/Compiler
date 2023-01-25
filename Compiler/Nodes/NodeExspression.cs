using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class NodeExpression : Node
    {
        private protected SymType cachedType = new SymType("");
        public SymType GetCachedType()
        {
            return cachedType;
        }
        public virtual SymType CalcType()
        {
            return new SymType("");
        }
        public virtual string Print(string prefix)
        {
            return "";
        }
    }
    public partial class NodeCast : NodeExpression
    {
        SymType cast;
        NodeExpression exp;
        public NodeCast(SymType cast, NodeExpression exp)
        {
            this.cast = cast;
            this.exp = exp;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"{cast.GetName()}\r\n";
            res += prefix + $"└─── {exp.ToPrint(prefix + "    ")}";
            return res;
        }
    }
    public partial class NodeBinOp : NodeExpression
    {
        public object opname;
        public NodeExpression left;
        public NodeExpression right;
        public NodeBinOp(object opname, NodeExpression left, NodeExpression right)
        {
            this.opname = opname;
            this.left = left;
            this.right = right;
            cachedType = CalcType();
        }
        public override SymType CalcType()
        {
            SymType leftType = left.GetCachedType();
            SymType rightType = right.GetCachedType();
            if (leftType.GetType() != rightType.GetType())
            {
                if (leftType is SymInteger || rightType is SymReal)
                {
                    left = new NodeCast(rightType, left);
                    return rightType;
                }
                if (rightType is SymInteger || leftType is SymReal)
                {
                    right = new NodeCast(leftType, left);
                    return leftType;
                }
                throw new Exception($"Incompatible types in expression");
            }
            if (opname is OperationSign op)
            {
                if ((op != OperationSign.Plus) && leftType.GetType() == typeof(SymString))
                {
                    throw new Exception("operator is not overloaded in expression");
                }
                if (op == OperationSign.Equal || op == OperationSign.Less || op == OperationSign.LessOrEqual ||
                   op == OperationSign.Greater || op == OperationSign.GreaterOrEqual || op == OperationSign.NotEqual)
                {
                    return new SymInteger("integer");
                }
            }
            return leftType;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            string? opnameStr = opname.ToString();
            if (opnameStr != null)
            {
                opnameStr = opnameStr.ToLower();
            }
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
    public partial class NodeUnOp : NodeExpression
    {
        object opname;
        NodeExpression arg;
        public NodeUnOp(object opname, NodeExpression arg)
        {
            this.opname = opname;
            this.arg = arg;
            cachedType = CalcType();
        }
        public override SymType CalcType()
        {
            return arg.GetCachedType();
        }
        public override string ToPrint(string prefix)
        {
            string res;
            string? opnameStr = opname.ToString();
            if (opnameStr != null)
            {
                opnameStr = opnameStr.ToLower();
            }
            if (opname.GetType() == typeof(OperationSign))
            {
                opnameStr = opname.ToString();
            }
            res = $"{opnameStr}\r\n";
            res += prefix + $"└─── {arg.ToPrint(prefix + "    ")}";
            return res;
        }
    }
    public partial class NodeVar : NodeExpression
    {
        public SymVar var;
        public string GetName()
        {
            return var.GetName();
        }
        public SymVar GetSymVar()
        {
            return var;
        }
        public NodeVar(SymVar var)
        {
            this.var = var;
            cachedType = CalcType();
        }
        public override SymType CalcType()
        {
            return var.GetTypeVar();
        }
        public override string ToPrint(string prefix)
        {
            return $"{var.GetName()}";
        }
    }
    public partial class NodeInt : NodeExpression
    {
        public int value;
        public NodeInt(int value)
        {
            this.value = value;
            cachedType = CalcType();
        }
        public override SymType CalcType()
        {
            return new SymInteger("integer");
        }
        public override string ToPrint(string prefix)
        {
            return $"{value.ToString()}";
        }
    }
    public partial class NodeReal : NodeExpression
    {
        public double value;
        public NodeReal(double value)
        {
            this.value = value;
            cachedType = CalcType();
        }
        public override SymType CalcType()
        {
            return new SymReal("real");
        }
        public override string ToPrint(string prefix)
        {
            return $"{value.ToString()}";
        }
    }
    public partial class NodeString : NodeExpression
    {
        public string value;
        public string GetValue()
        {
            return value;
        }
        public NodeString(string value)
        {
            this.value = value;
            cachedType = CalcType();
        }
        public override SymType CalcType()
        {
            return new SymString("string");
        }
        public override string ToPrint(string prefix)
        {
            return $"\'{value}\'";
        }
    }
}