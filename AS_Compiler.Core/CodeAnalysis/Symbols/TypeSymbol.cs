namespace AS_Compiler.Core.CodeAnalysis.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        internal TypeSymbol(string name)
            : base(name)
        {

        }

        public override SymbolType SymbolType => SymbolType.Type;
    }
}