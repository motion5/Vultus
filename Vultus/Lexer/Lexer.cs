using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Vultus.Lexer
{
    public enum TokenType
    {
        Keyword,
        Identifier,

        String,
        Character,

        Integer,
        Decimal,

        Operator
    }

    public class Lexer
    {
        private static readonly Regex LineComment = new Regex(@"//(.*?)\n");
        private static readonly Regex BlockComment = new Regex(@"/\*(.*?)\*/", RegexOptions.Singleline);
        private static readonly Regex EmptyLines = new Regex(@"^\s+$[\r\n]*", RegexOptions.Multiline);
        public Dictionary<TokenType, Regex> TokenMatch { get; set; }
        private StreamReader sr;
        private MemoryStream ms;

        /* Settings */
        private const int MaxTokenLength = 50;

        public Lexer(string filePath)
        {
            CreateMemoryStreamFromInput(filePath);
            InitMatches();
            RemoveCommentsFromSource();
        }

        private void CreateMemoryStreamFromInput(string filePath)
        {
            ms = new MemoryStream();

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                file.CopyTo(ms);
            
            // Set the position to the beginning of the stream.
            ms.Seek(0, SeekOrigin.Begin);
            
            // read from stream
            sr = new StreamReader(ms);
        }

        private void RemoveCommentsFromSource()
        {
            var text = sr.ReadToEnd();

            // remove block comments
            var sanitizedSource = LineComment.Replace(text, String.Empty);
            sanitizedSource = BlockComment.Replace(sanitizedSource, String.Empty);
            sanitizedSource = EmptyLines.Replace(sanitizedSource, String.Empty);

            // convert string to stream
            byte[] byteArray = Encoding.ASCII.GetBytes(sanitizedSource);
            // write back to stream
            ms = new MemoryStream(byteArray);
            // read from stream
            sr = new StreamReader(ms);
        }

        private void InitMatches()
        {
            const string or = @"\|\|";
            const string and = @"&&";
            const string not = @"\!|\<>";

            TokenMatch = new Dictionary<TokenType, Regex>
            {
                {
                    //keyword = begin | end | if | then | else
                    TokenType.Keyword,
                    new Regex("(f|if|else|elif|type|operator)")
                },
                {
                    //identifier = letter (letter | digit | underscore)*
                    TokenType.Identifier,
                    new Regex("[A-z][A-z0-9]")
                },
                {
                    //char = a | b | ... | z | A | B | ... | Z
                    TokenType.Character,
                    new Regex(@"[A-z]")
                },
                {
                    //string = "char+" | 'char+'
                    TokenType.String,
                    new Regex("(\"|')[A-z]+(\"|')")
                },
                {
                    //integer = 0 | 1 | .. | 10000
                    TokenType.Integer,
                    new Regex(@"\d+")
                },
                {
                    //decimal = 0.12 | 1 | .. | 10000
                    TokenType.Decimal,
                    new Regex(@"\d+\.\d+")
                },
                {
                    //operator = < | <= | = | <> | > | >=
                    TokenType.Operator,
                    new Regex($"(==|<=|>=|<|>|${or}|${and}|${not})")
                }
            };
        }

        public Token NextToken()
        {
            var tokenComplete = false;
            while (sr.Peek() >= 0 && !tokenComplete)
            {
                var nextChar = (char) sr.Read();

                var next = nextChar.ToString();

                if (string.IsNullOrWhiteSpace(next)) continue;

                Console.Write($"Printing next char: {nextChar}\n");

                var unidentifiedToken = new StringBuilder(next, MaxTokenLength);
                // now get next set of input until is whitespace
                while (!tokenComplete)
                {
                    var nextCharInToken = (char) sr.Peek();
                    if (!string.IsNullOrWhiteSpace(nextCharInToken.ToString()))
                    {
                        unidentifiedToken.Append(nextCharInToken.ToString());
                        continue;
                    }

                    tokenComplete = true;
                }

                return IdentifyToken(unidentifiedToken.ToString());
            }

            if (sr.EndOfStream)
            {
                Console.WriteLine("End of file reached");
                return null;
            }

            throw new Exception("Unexpected error");
        }

        private Token IdentifyToken(string unidentifiedToken)
        {
            foreach (var (tokenType, regex) in TokenMatch)
            {
                if (regex.IsMatch(unidentifiedToken))
                {
                    return new Token
                    {
                        TokenType = tokenType,
                        Value = unidentifiedToken
                    };
                }
            }

            throw new Exception($"Unidentified token: {unidentifiedToken}");
        }
    }
}
