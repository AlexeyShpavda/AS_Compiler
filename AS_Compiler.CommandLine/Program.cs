namespace AS_Compiler.CommandLine
{
    internal class Program
    {
        private static void Main()
        {
            var readEvalPrintLoop = new CompilerReadEvalPrintLoop();

            readEvalPrintLoop.Run();
        }
    }
}