using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class NodeDefs : Node { }
    public partial class ConstDefsNode : NodeDefs
    {
        List<ConstDeclarationNode> body;
        public ConstDefsNode(List<ConstDeclarationNode> body)
        {
            this.body = body;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"const\r\n";
            int i = 1;
            foreach (ConstDeclarationNode el in body)
            {
                if (i == body.Count)
                {
                    if (el != null)
                    {
                        res += prefix + $"└─── {el.ToPrint(prefix + "    ")}";
                    }
                }
                else
                {
                    if (el != null)
                    {
                        res += prefix + $"├─── {el.ToPrint(prefix + "    ")}\r\n";
                    }
                    i++;
                }
            }
            return res;
        }
    }
    public partial class VarDefsNode : NodeDefs
    {
        public List<VarDeclarationNode> body;
        public VarDefsNode(List<VarDeclarationNode> body)
        {
            this.body = body;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"var\r\n";
            int i = 1;
            foreach (VarDeclarationNode el in body)
            {
                if (i == body.Count)
                {
                    if (el != null)
                    {
                        res += prefix + $"└─── {el.ToPrint(prefix + "    ")}";
                    }
                }
                else
                {
                    if (el != null)
                    {
                        res += prefix + $"├─── {el.ToPrint(prefix + "    ")}\r\n";
                    }
                    i++;
                }
            }
            return res;
        }
    }

    public partial class ProcedureDefNode : NodeDefs
    {
        VarDefsNode params_;
        List<NodeDefs> localsTypes;
        SymProc symProc;
        public ProcedureDefNode(VarDefsNode params_, List<NodeDefs> localsTypes, SymProc symProc)
        {
            this.params_ = params_;
            this.localsTypes = localsTypes;
            this.symProc = symProc;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"procedure {symProc.GetName()}\r\n";
            res += prefix + $"├─── {params_.ToPrint(prefix + "    ")}\r\n";
            foreach (NodeDefs? el in localsTypes)
            {
                res += prefix + $"├─── {el.ToPrint(prefix + "    ")}\r\n";
            }
            res += prefix + $"└─── {symProc.GetBody().ToPrint(prefix + "    ")}";
            return res;
        }
    }
    public class DeclarationNode : Node { }
    public partial class VarDeclarationNode : DeclarationNode
    {
        List<SymVar> vars;
        SymType type;
        public List<SymVar> GetVars()
        {
            return vars;
        }
        public VarDeclarationNode(List<SymVar> name, SymType type)
        {
            this.vars = name;
            this.type = type;
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $":\r\n";
            if (vars.Count > 1)
            {
                res += prefix + $"├─── \r\n";
                for (int i = 0; i < vars.Count - 1; i++)
                {
                    res += prefix + $"│    ├─── {vars[i].GetName()}\r\n";
                }
                res += prefix + $"│    └─── {vars[^1].GetName()}\r\n";
            }
            else
            {
                res += prefix + $"├─── {vars[0].GetName()}\r\n";
            }
            res += prefix + $"└─── {type.ToPrint(prefix + "    ")}";
            return res;
        }
    }
    public partial class ConstDeclarationNode : DeclarationNode
    {
        SymVarConst var;
        NodeExpression value;
        public ConstDeclarationNode(SymVarConst var, NodeExpression value)
        {
            this.var = var;
            this.value = value;
            if ((value.GetCachedType().GetType() != typeof(SymInteger)) &&
                (value.GetCachedType().GetType() != typeof(SymReal)) &&
                (value.GetCachedType().GetType() != typeof(SymString)))
            {
                throw new Exception($"Incompatible types");
            }
        }
        public override string ToPrint(string prefix)
        {
            string res;
            res = $"=\r\n";
            res += prefix + $"├─── {var.GetName()}\r\n";
            res += prefix + $"└─── {value.ToPrint(prefix + "    ")}";
            return res;
        }
    }
}