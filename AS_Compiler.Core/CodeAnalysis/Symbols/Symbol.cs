namespace AS_Compiler.Core.CodeAnalysis.Symbols
{
    public abstract class Symbol
    {
        private protected Symbol(string name)
        {
            Name = name;
        }

        public abstract SymbolType SymbolType { get; }
        public string Name { get; }
    }
}