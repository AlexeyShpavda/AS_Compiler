using System;

namespace AS_Compiler.Core.CodeAnalysis.Symbols
{
    public sealed class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadOnly, Type type)
            : base(name)
        {
            IsReadOnly = isReadOnly;
            Type = type;
        }

        public bool IsReadOnly { get; }
        public Type Type { get; }
        public override SymbolType SymbolType => SymbolType.Variable;

        public override string ToString() => Name;
    }
}