using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    /*public class Node
    {
        public TypeNode type;
        public string value;
        public List<Node> children;
        public Node(TypeNode type, string value, List<Node> children)
        {
            this.type = type;
            this.value = value;
            this.children = children;
        }
    }

    public class BinOpNode: Node
    {
        TypeNode CalcType()
        {
            TypeNode left = children[0].type;
            TypeNode right = children[1].type;
            if (left == TypeNode.Var)
            {
                SymVar symVar = (SymVar)children[0];
                if (symVar.symType is SymInteger)
                {
                    left = TypeNode.Integer;
                }
                if (symVar.symType is SymString)
                {
                    left = TypeNode.String;
                }
                if (symVar.symType is SymReal)
                {
                    left = TypeNode.Real;
                }
            }
            if (right == TypeNode.Var)
            {
                SymVar symVar = (SymVar)children[1];
                if (symVar.symType is SymInteger)
                {
                    right = TypeNode.Integer;
                }
                if (symVar.symType is SymString)
                {
                    right = TypeNode.String;
                }
                if (symVar.symType is SymReal)
                {
                    right = TypeNode.Real;
                }
            }
            if ((left != right) && !(left == TypeNode.Integer && right == TypeNode.Real) && !(left == TypeNode.Real && right == TypeNode.Integer))
            {
                throw new Exception($"Incompatible types \"{left}\" {value} \"{right}\"");
            }
            return left;
        }
        public BinOpNode(TypeNode type, string value, List<Node> children) : base(type, value, children)
        {
            type = CalcType();
        }
    }
    public class AssigmentNode : Node
    {
        TypeNode CalcType()
        {
            TypeNode left = children[0].type;
            TypeNode right = children[1].type;
            if (left == TypeNode.Var)
            {
                SymVar symVar = (SymVar)children[0];
                if (symVar.symType is SymInteger)
                {
                    left = TypeNode.Integer;
                }
                if (symVar.symType is SymString)
                {
                    left = TypeNode.String;
                }
                if (symVar.symType is SymReal)
                {
                    left = TypeNode.Real;
                }
            }
            if (right == TypeNode.Var)
            {
                SymVar symVar = (SymVar)children[1];
                if (symVar.symType is SymInteger)
                {
                    right = TypeNode.Integer;
                }
                if (symVar.symType is SymString)
                {
                    right = TypeNode.String;
                }
                if (symVar.symType is SymReal)
                {
                    right = TypeNode.Real;
                }
            }
            if ((left != right) && !(left == TypeNode.Real && right == TypeNode.Integer))
            {
                throw new Exception($"Incompatible types \"{left}\" {value} \"{right}\"");
            }
            return left;
        }
        public AssigmentNode(TypeNode type, string value, List<Node> children) : base(type, value, children)
        {
            type = CalcType();
        }
    }
    public enum TypeNode
    {
        BinOp,
        Var,
        Result,
        Statemant,
        Assigment,
        Integer,
        String,
        Real,
        While,
        If,
        Else,
        Repeat,
        Block,
        For,
        To,
        NullStmt,
        MainProgram,
        Const,
        ConstDef,
        VarDef,
        Type,
    }*/
    
}
