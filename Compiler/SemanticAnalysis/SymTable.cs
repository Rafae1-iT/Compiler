using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class SymTable
    {
        Dictionary<string, Symbol> data;
        public Dictionary<string, Symbol> GetData()
        {
            return data;
        }
        public int GetSize()
        {
            return data.Count;
        }
        public void Add(string name, Symbol value)
        {
            if (!data.TryAdd(name, value))
            {
                throw new Exception($"Duplicate identifier \"{name}\"");
            }
        }
        public Symbol Get(string name)
        {
            Symbol value;
            Symbol? check;
            if (!data.TryGetValue(name, out check))
            {
                throw new Exception("Variable not declared");
            }
            value = check;
            return value;
        }
        public SymTable(Dictionary<string, Symbol> data)
        {
            this.data = data;
        }
    }
    public class SymTableStack
    {
        List<SymTable> tables;
        public SymTable GetBackTable()
        {
            return tables[^1];
        }
        public void AddTable(SymTable table)
        {
            tables.Add(table);
        }
        public void Add(string name, Symbol value)
        {
            try
            {
                GetBackTable().Add(name, value);
            }
            catch
            {
                throw new Exception($"Duplicate identifier \"{name}\"");
            }
        }
        public Symbol Get(string name)
        {
            Symbol res = new Symbol("", new Node(TypeNode.NullStmt, "", new List<Node>())); // для return
            bool decl = false;
            for (int i = tables.Count - 1; i >= 0; i--) // пробежать по стеку снизу вверх
            {
                try
                {
                    res = tables[i].Get(name);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    decl = true;
                }
            }
            if (!decl)
            {
                throw new Exception("Variable not declared");
            }
            return res;
        }
        public SymTableStack()
        {
            tables = new List<SymTable>();
        }
    }
}