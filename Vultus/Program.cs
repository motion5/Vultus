using System;
using System.IO;

namespace Vultus
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Vultus Compiler.");
            
            /* Front end */
            // lexing
            
            // Parsing 
            var fileToParse = "/Users/rob/code/dotnet/vultus/Vultus/data/pseudo";
            var lexer = new Lexer.Lexer(fileToParse);
            var parser = new Parser.Parser(lexer);
            parser.Init();
            
            /* intermediate code representation */
            
            /* Back end */
        }
    }
}