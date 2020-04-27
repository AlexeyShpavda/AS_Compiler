namespace AS_Compiler.Core.CodeAnalysis.Symbols
{
    public class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isConstant, TypeSymbol type)
            : base(name)
        {
            IsConstant = isConstant;
            Type = type;
        }

        public bool IsConstant { get; }
        public TypeSymbol Type { get; }
        public override SymbolType SymbolType => SymbolType.Variable;
    }
}