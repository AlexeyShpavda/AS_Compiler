namespace AS_Compiler.Core.CodeAnalysis.Symbols
{
    public sealed class ParameterSymbol : VariableSymbol
    {
        public ParameterSymbol(string name, TypeSymbol type)
            : base(name, true, type)
        {
        }

        public override SymbolType SymbolType => SymbolType.Parameter;
    }
}