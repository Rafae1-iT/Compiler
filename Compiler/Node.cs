using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Node
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
    }
    public enum KeyWord
    {
        AND,
        ARRAY,
        AS,
        ASM,
        BEGIN,
        CASE,
        CONST,
        CONSTRUCTOR,
        DESTRUCTOR,
        DIV,
        DO,
        DOWNTO,
        ELSE,
        END,
        FILE,
        FOR,
        FOREACH,
        FUNCTION,
        GOTO,
        IMPLEMENTATION,
        IF,
        IN,
        INHERITED,
        INLINE,
        INTERFACE,
        LABEL,
        MOD,
        NIL,
        NOT,
        OBJECT,
        OF,
        OPERATOR,
        OR,
        PACKED,
        PROCEDURE,
        PROGRAM,
        RECORD,
        REPEAT,
        SELF,
        SET,
        SHL,
        SHR,
        STRING,
        THEN,
        TO,
        TYPE,
        UNIT,
        UNTIL,
        USES,
        VAR,
        WHILE,
        WITH,
        XOR,
        DISPOSE,
        EXIT,
        FALSE,
        NEW,
        TRUE,
        CLASS,
        DISPINTERFACE,
        EXCEPT,
        EXPORTS,
        FINALIZATION,
        FINALLY,
        INITIALIZATION,
        IS,
        LIBRARY,
        ON,
        OUT,
        PROPERTY,
        RAISE,
        RESOURCESTRING,
        THREADVAR,
        TRY
    }
    public enum OperationSign
    {
        Unidentified,
        Equal, // =
        Colon, // :
        Plus, // +
        Minus, // -
        Multiply, // *
        Divide, // /
        Greater, //>
        Less, //<
        At, // @
        BitwiseShiftToTheLeft, // <<
        BitwiseShiftToTheRight, //>>
        NotEqual, //<>
        SymmetricalDifference, // ><
        LessOrEqual, // <=
        GreaterOrEqual, // >=
        Assignment, // :=
        Addition, // +=
        Subtraction, // -=
        Multiplication, // *=
        Division, // /=
        PointRecord, // .
    }
    public enum Separator
    {
        Unidentified,
        Comma, // ,
        Semiсolon, // ;
        OpenParenthesis, // (
        CloseParenthesis, // )
        OpenBracket, // [
        CloseBracket, // ]
        Point, // .
        DoublePoint // ..
    }
}
