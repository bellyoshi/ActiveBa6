using System.Collections.Generic;
using System.Text;

namespace BLanguage
{

    public class Scope
    {

        public Scope? Parent { get; set; }
        public Dictionary<string, Variable> Variables { get; set; }
        public Dictionary<string, Variable> LocalVariables { get; set; } = new Dictionary<string, Variable>();
        public int ArgCount { get; set; }
        public string ScopeName { get; set; }
        public string ReturnType { get; set; }

        public bool IsScope(String name)
        {
            return name.Equals(ScopeName);
        }

        public Scope() : this(null, "grobal")
        {

        }
        public Scope(Scope? p, string scopeName)
        {
            Parent = p;
            Variables  = new Dictionary<string, Variable>();
            ScopeName = scopeName;
        }

        public void AssignParameter(string var, Variable value)
        {
            if(Variables.TryGetValue(var, out Variable? v))
            {
                var oldValue = Variables[var];
                Variables[var].Value = value;
            }
            else
            {
                Variables.TryAdd(var, value);
            }
        }

        public void Assign(string var, Variable @value)
        {
            if(Resolve(var, !IsScope(ScopeName))!= null)
            {
                this.ReAssign(var, @value);
            }
            else
            {
                Variables.TryAdd(var, value);
            }
        }

        public bool IsGlobalScope()
        {
            return Parent == null;
        }

        public void ReAssign(string identifier, Variable value)
        {
            if (Variables.ContainsKey(identifier))
            {
                Variables[identifier] =  value;//todo : try addorupdate
            }else if(Parent != null)
            {
                Parent.ReAssign(identifier, value);
            }
        }



        public Variable? Resolve(string var)
        {
            return Resolve(var, true);
        }

        private Variable? Resolve(string var, bool checkParrent)
        {
            Variables.TryGetValue(@var, out var value);

            if(value != null)
            {
                return value;
            }
            else if(checkParrent && Parent != null)
            {
                return Parent.Resolve(var, !Parent.IsScope(ScopeName));
            }
            else { 
                return null;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var item in Variables)
            {
                sb.Append(item.Key)
                  .Append("->")
                  .Append(item.Value)
                  .Append(",");
            }
            return sb.ToString();
        }

    }
}
