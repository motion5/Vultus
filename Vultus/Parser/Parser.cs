using System;

namespace Vultus.Parser
{
    public class Parser : IParser
    {
        private readonly Lexer.Lexer lexer;
        
        public Parser(Lexer.Lexer lexer)
        {
            this.lexer = lexer;
        }

        public void Init()
        {
            var token = lexer.NextToken();
            Console.WriteLine("First token");
            Console.WriteLine(token.TokenType);
            Console.WriteLine(token.Value);
        }
    }
}