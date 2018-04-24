using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{



    /// <summary>
    /// Handcrafted scanner.
    /// 
    /// 
    /// </summary>
    public class Scanner
    {
        CodeDomProvider _provider = CodeDomProvider.CreateProvider("C#");
        /// <summary>
        /// Scan exception type
        /// </summary>
        public class ScanException : System.Exception
        {
            internal readonly int position;
            internal readonly string encountered;
            internal readonly string expected;
            public ScanException(int position, string encountered, string expected)
                : base($"error.scan, {position}, {encountered}, {expected}")
            {
                this.position = position;
                this.encountered = encountered;
                this.expected = expected;
            }
        }

        public class Token
        {
            internal readonly Symbol symbol;
            internal readonly string image;
            internal readonly int length;
            public Token(Symbol symbol, string image) : this(symbol, image, image.Length)
            {
            }
            public Token(Symbol symbol, string image, int length)
            {
                this.symbol = symbol;
                this.image = image;
                this.length = length;
            }
            public virtual Symbol Symbol
            {
                get
                {
                    return symbol;
                }
            }
            public virtual string Image
            {
                get
                {
                    return image;
                }
            }
            public virtual int Size
            {
                get
                {
                    return length;
                }
            }
            public override string ToString()
            {
                return symbol.ToString();
            }
        }

        public class ExtensionToken : Token
        {
            public ExtensionToken(string image) : base(Scanner.Symbol.EXTENSION, image)
            {
            }
        }

        /// <summary>
        /// Symbol type
        /// </summary>
        public sealed class Symbol
        {
            public static readonly Symbol EOF = new Symbol("EOF", InnerEnum.EOF);
            public static readonly Symbol PLUS = new Symbol("PLUS", InnerEnum.PLUS, "'+'");
            public static readonly Symbol MINUS = new Symbol("MINUS", InnerEnum.MINUS, "'-'");
            public static readonly Symbol MUL = new Symbol("MUL", InnerEnum.MUL, "'*'");
            public static readonly Symbol DIV = new Symbol("DIV", InnerEnum.DIV, "'/'|'div'");
            public static readonly Symbol MOD = new Symbol("MOD", InnerEnum.MOD, "'%'|'mod'");
            public static readonly Symbol LPAREN = new Symbol("LPAREN", InnerEnum.LPAREN, "'('");
            public static readonly Symbol RPAREN = new Symbol("RPAREN", InnerEnum.RPAREN, "')'");
            public static readonly Symbol IDENTIFIER = new Symbol("IDENTIFIER", InnerEnum.IDENTIFIER);
            public static readonly Symbol NOT = new Symbol("NOT", InnerEnum.NOT, "'!'|'not'");
            public static readonly Symbol AND = new Symbol("AND", InnerEnum.AND, "'&&'|'and'");
            public static readonly Symbol OR = new Symbol("OR", InnerEnum.OR, "'||'|'or'");
            public static readonly Symbol EMPTY = new Symbol("EMPTY", InnerEnum.EMPTY, "'empty'");
            public static readonly Symbol INSTANCEOF = new Symbol("INSTANCEOF", InnerEnum.INSTANCEOF, "'instanceof'");
            public static readonly Symbol INTEGER = new Symbol("INTEGER", InnerEnum.INTEGER);
            public static readonly Symbol FLOAT = new Symbol("FLOAT", InnerEnum.FLOAT);
            public static readonly Symbol TRUE = new Symbol("TRUE", InnerEnum.TRUE, "'true'");
            public static readonly Symbol FALSE = new Symbol("FALSE", InnerEnum.FALSE, "'false'");
            public static readonly Symbol STRING = new Symbol("STRING", InnerEnum.STRING);
            public static readonly Symbol NULL = new Symbol("NULL", InnerEnum.NULL, "'null'");
            public static readonly Symbol LE = new Symbol("LE", InnerEnum.LE, "'<='|'le'");
            public static readonly Symbol LT = new Symbol("LT", InnerEnum.LT, "'<'|'lt'");
            public static readonly Symbol GE = new Symbol("GE", InnerEnum.GE, "'>='|'ge'");
            public static readonly Symbol GT = new Symbol("GT", InnerEnum.GT, "'>'|'gt'");
            public static readonly Symbol EQ = new Symbol("EQ", InnerEnum.EQ, "'=='|'eq'");
            public static readonly Symbol NE = new Symbol("NE", InnerEnum.NE, "'!='|'ne'");
            public static readonly Symbol QUESTION = new Symbol("QUESTION", InnerEnum.QUESTION, "'?'");
            public static readonly Symbol COLON = new Symbol("COLON", InnerEnum.COLON, "':'");
            public static readonly Symbol TEXT = new Symbol("TEXT", InnerEnum.TEXT);
            public static readonly Symbol DOT = new Symbol("DOT", InnerEnum.DOT, "'.'");
            public static readonly Symbol LBRACK = new Symbol("LBRACK", InnerEnum.LBRACK, "'['");
            public static readonly Symbol RBRACK = new Symbol("RBRACK", InnerEnum.RBRACK, "']'");
            public static readonly Symbol COMMA = new Symbol("COMMA", InnerEnum.COMMA, "','");
            public static readonly Symbol START_EVAL_DEFERRED = new Symbol("START_EVAL_DEFERRED", InnerEnum.START_EVAL_DEFERRED, "'#{'");
            public static readonly Symbol START_EVAL_DYNAMIC = new Symbol("START_EVAL_DYNAMIC", InnerEnum.START_EVAL_DYNAMIC, "'${'");
            public static readonly Symbol END_EVAL = new Symbol("END_EVAL", InnerEnum.END_EVAL, "'}'");
            public static readonly Symbol EXTENSION = new Symbol("EXTENSION", InnerEnum.EXTENSION); // used in syntax extensions

            private static readonly IList<Symbol> valueList = new List<Symbol>();

            static Symbol()
            {
                valueList.Add(EOF);
                valueList.Add(PLUS);
                valueList.Add(MINUS);
                valueList.Add(MUL);
                valueList.Add(DIV);
                valueList.Add(MOD);
                valueList.Add(LPAREN);
                valueList.Add(RPAREN);
                valueList.Add(IDENTIFIER);
                valueList.Add(NOT);
                valueList.Add(AND);
                valueList.Add(OR);
                valueList.Add(EMPTY);
                valueList.Add(INSTANCEOF);
                valueList.Add(INTEGER);
                valueList.Add(FLOAT);
                valueList.Add(TRUE);
                valueList.Add(FALSE);
                valueList.Add(STRING);
                valueList.Add(NULL);
                valueList.Add(LE);
                valueList.Add(LT);
                valueList.Add(GE);
                valueList.Add(GT);
                valueList.Add(EQ);
                valueList.Add(NE);
                valueList.Add(QUESTION);
                valueList.Add(COLON);
                valueList.Add(TEXT);
                valueList.Add(DOT);
                valueList.Add(LBRACK);
                valueList.Add(RBRACK);
                valueList.Add(COMMA);
                valueList.Add(START_EVAL_DEFERRED);
                valueList.Add(START_EVAL_DYNAMIC);
                valueList.Add(END_EVAL);
                valueList.Add(EXTENSION);
            }

            public enum InnerEnum
            {
                EOF,
                PLUS,
                MINUS,
                MUL,
                DIV,
                MOD,
                LPAREN,
                RPAREN,
                IDENTIFIER,
                NOT,
                AND,
                OR,
                EMPTY,
                INSTANCEOF,
                INTEGER,
                FLOAT,
                TRUE,
                FALSE,
                STRING,
                NULL,
                LE,
                LT,
                GE,
                GT,
                EQ,
                NE,
                QUESTION,
                COLON,
                TEXT,
                DOT,
                LBRACK,
                RBRACK,
                COMMA,
                START_EVAL_DEFERRED,
                START_EVAL_DYNAMIC,
                END_EVAL,
                EXTENSION
            }

            private readonly string nameValue;
            private readonly int ordinalValue;
            private readonly InnerEnum innerEnumValue;
            private static int nextOrdinal = 0;
            internal readonly string @string;
            internal Symbol(string name, InnerEnum innerEnum)
            {

                nameValue = name;
                ordinalValue = nextOrdinal++;
                innerEnumValue = innerEnum;
            }
            internal Symbol(string name, InnerEnum innerEnum, string @string)
            {
                this.@string = @string;

                nameValue = name;
                ordinalValue = nextOrdinal++;
                innerEnumValue = innerEnum;
            }
            public override string ToString()
            {
                return string.ReferenceEquals(@string, null) ? "<" + base.ToString() + ">" : @string;
            }

            public static IList<Symbol> values()
            {
                return valueList;
            }

            public InnerEnum InnerEnumValue()
            {
                return innerEnumValue;
            }

            public int ordinal()
            {
                return ordinalValue;
            }

            public static Symbol valueOf(string name)
            {
                foreach (Symbol enumInstance in valueList)
                {
                    if (enumInstance.nameValue == name)
                    {
                        return enumInstance;
                    }
                }
                throw new System.ArgumentException(name);
            }
        }

        private static readonly Dictionary<string, Token> KEYMAP = new Dictionary<string, Token>();
        private static readonly Dictionary<Symbol, Token> FIXMAP = new Dictionary<Symbol, Token>();

        private static void AddFixToken(Token token)
        {
            FIXMAP[token.Symbol] = token;
        }

        private static void AddKeyToken(Token token)
        {
            KEYMAP[token.Image] = token;
        }

        static Scanner()
        {
            AddFixToken(new Token(Symbol.PLUS, "+"));
            AddFixToken(new Token(Symbol.MINUS, "-"));
            AddFixToken(new Token(Symbol.MUL, "*"));
            AddFixToken(new Token(Symbol.DIV, "/"));
            AddFixToken(new Token(Symbol.MOD, "%"));
            AddFixToken(new Token(Symbol.LPAREN, "("));
            AddFixToken(new Token(Symbol.RPAREN, ")"));
            AddFixToken(new Token(Symbol.NOT, "!"));
            AddFixToken(new Token(Symbol.AND, "&&"));
            AddFixToken(new Token(Symbol.OR, "||"));
            AddFixToken(new Token(Symbol.EQ, "=="));
            AddFixToken(new Token(Symbol.NE, "!="));
            AddFixToken(new Token(Symbol.LT, "<"));
            AddFixToken(new Token(Symbol.LE, "<="));
            AddFixToken(new Token(Symbol.GT, ">"));
            AddFixToken(new Token(Symbol.GE, ">="));
            AddFixToken(new Token(Symbol.QUESTION, "?"));
            AddFixToken(new Token(Symbol.COLON, ":"));
            AddFixToken(new Token(Symbol.COMMA, ","));
            AddFixToken(new Token(Symbol.DOT, "."));
            AddFixToken(new Token(Symbol.LBRACK, "["));
            AddFixToken(new Token(Symbol.RBRACK, "]"));
            AddFixToken(new Token(Symbol.START_EVAL_DEFERRED, "#{"));
            AddFixToken(new Token(Symbol.START_EVAL_DYNAMIC, "${"));
            AddFixToken(new Token(Symbol.END_EVAL, "}"));
            AddFixToken(new Token(Symbol.EOF, null, 0));

            AddKeyToken(new Token(Symbol.NULL, "null"));
            AddKeyToken(new Token(Symbol.TRUE, "true"));
            AddKeyToken(new Token(Symbol.FALSE, "false"));
            AddKeyToken(new Token(Symbol.EMPTY, "empty"));
            AddKeyToken(new Token(Symbol.DIV, "div"));
            AddKeyToken(new Token(Symbol.MOD, "mod"));
            AddKeyToken(new Token(Symbol.NOT, "not"));
            AddKeyToken(new Token(Symbol.AND, "and"));
            AddKeyToken(new Token(Symbol.OR, "or"));
            AddKeyToken(new Token(Symbol.LE, "le"));
            AddKeyToken(new Token(Symbol.LT, "lt"));
            AddKeyToken(new Token(Symbol.EQ, "eq"));
            AddKeyToken(new Token(Symbol.NE, "ne"));
            AddKeyToken(new Token(Symbol.GE, "ge"));
            AddKeyToken(new Token(Symbol.GT, "gt"));
            AddKeyToken(new Token(Symbol.INSTANCEOF, "instanceof"));
        }

        private Token token; // current token
        private int position; // start position of current token
        private readonly string input;

        protected internal readonly StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Constructor. </summary>
        /// <param name="input"> expression string </param>
        protected internal Scanner(string input)
        {
            this.input = input;
        }

        public virtual string Input
        {
            get
            {
                return input;
            }
        }

        /// <returns> current token </returns>
        public virtual Token GetToken()
        {
            return token;
        }

        /// <returns> current input position </returns>
        public virtual int Position
        {
            get
            {
                return position;
            }
        }

        /// <returns> <code>true</code> iff the specified character is a digit </returns>
        protected internal virtual bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <param name="s"> name </param>
        /// <returns> token for the given keyword or <code>null</code> </returns>
        protected internal virtual Token Keyword(string s)
        {
            Token t;
            KEYMAP.TryGetValue(s, out t);
            return t;
        }

        /// <param name="symbol"> </param>
        /// <returns> token for the given symbol </returns>
        protected internal virtual Token @fixed(Symbol symbol)
        {
            Token t;
            FIXMAP.TryGetValue(symbol, out t);
            return t;
        }

        protected internal virtual Token NewToken(Symbol symbol, string value, int length)
        {
            return new Token(symbol, value, length);
        }

        protected internal virtual bool Eval
        {
            get
            {
                return token != null && token.Symbol != Symbol.TEXT && token.Symbol != Symbol.END_EVAL;
            }
        }

        /// <summary>
        /// text token
        /// </summary>
        protected internal virtual Token NextText()
        {
            builder.Length = 0;
            int i = position;
            int l = input.Length;
            bool escaped = false;
            while (i < l)
            {
                char c = input[i];
                switch (c)
                {
                    case '\\':
                        if (escaped)
                        {
                            builder.Append('\\');
                        }
                        else
                        {
                            escaped = true;
                        }
                        break;
                    case '#':
                    case '$':
                        if (i + 1 < l && input[i + 1] == '{')
                        {
                            if (escaped)
                            {
                                builder.Append(c);
                            }
                            else
                            {
                                return NewToken(Symbol.TEXT, builder.ToString(), i - position);
                            }
                        }
                        else
                        {
                            if (escaped)
                            {
                                builder.Append('\\');
                            }
                            builder.Append(c);
                        }
                        escaped = false;
                        break;
                    default:
                        if (escaped)
                        {
                            builder.Append('\\');
                        }
                        builder.Append(c);
                        escaped = false;
                        break;
                }
                i++;
            }
            if (escaped)
            {
                builder.Append('\\');
            }
            return NewToken(Symbol.TEXT, builder.ToString(), i - position);
        }

        /// <summary>
        /// string token
        /// </summary>
        protected internal virtual Token NextString()
        {
            builder.Length = 0;
            char quote = input[position];
            int i = position + 1;
            int l = input.Length;
            while (i < l)
            {
                char c = input[i++];
                if (c == '\\')
                {
                    if (i == l)
                    {
                        throw new ScanException(position, "unterminated string", quote + " or \\");
                    }
                    else
                    {
                        c = input[i++];
                        if (c == '\\' || c == quote)
                        {
                            builder.Append(c);
                        }
                        else
                        {
                            throw new ScanException(position, "invalid escape sequence \\" + c, "\\" + quote + " or \\\\");
                        }
                    }
                }
                else if (c == quote)
                {
                    return NewToken(Symbol.STRING, builder.ToString(), i - position);
                }
                else
                {
                    builder.Append(c);
                }
            }
            throw new ScanException(position, "unterminated string", quote.ToString());
        }

        /// <summary>
        /// number token
        /// </summary>
        protected internal virtual Token NextNumber()
        {
            int i = position;
            int l = input.Length;
            while (i < l && IsDigit(input[i]))
            {
                i++;
            }
            Symbol symbol = Symbol.INTEGER;
            if (i < l && input[i] == '.')
            {
                i++;
                while (i < l && IsDigit(input[i]))
                {
                    i++;
                }
                symbol = Symbol.FLOAT;
            }
            if (i < l && (input[i] == 'e' || input[i] == 'E'))
            {
                int e = i;
                i++;
                if (i < l && (input[i] == '+' || input[i] == '-'))
                {
                    i++;
                }
                if (i < l && IsDigit(input[i]))
                {
                    i++;
                    while (i < l && IsDigit(input[i]))
                    {
                        i++;
                    }
                    symbol = Symbol.FLOAT;
                }
                else
                {
                    i = e;
                }
            }
            return NewToken(symbol, input.Substring(position, i - position), i - position);
        }

        /// <summary>
        /// token inside an eval expression
        /// </summary>
        protected internal virtual Token NextEval()
        {
            char c1 = input[position];
            char c2 = position < input.Length - 1 ? input[position + 1] : (char)0;

            switch (c1)
            {
                case '*':
                    return @fixed(Symbol.MUL);
                case '/':
                    return @fixed(Symbol.DIV);
                case '%':
                    return @fixed(Symbol.MOD);
                case '+':
                    return @fixed(Symbol.PLUS);
                case '-':
                    return @fixed(Symbol.MINUS);
                case '?':
                    return @fixed(Symbol.QUESTION);
                case ':':
                    return @fixed(Symbol.COLON);
                case '[':
                    return @fixed(Symbol.LBRACK);
                case ']':
                    return @fixed(Symbol.RBRACK);
                case '(':
                    return @fixed(Symbol.LPAREN);
                case ')':
                    return @fixed(Symbol.RPAREN);
                case ',':
                    return @fixed(Symbol.COMMA);
                case '.':
                    if (!IsDigit(c2))
                    {
                        return @fixed(Symbol.DOT);
                    }
                    break;
                case '=':
                    if (c2 == '=')
                    {
                        return @fixed(Symbol.EQ);
                    }
                    break;
                case '&':
                    if (c2 == '&')
                    {
                        return @fixed(Symbol.AND);
                    }
                    break;
                case '|':
                    if (c2 == '|')
                    {
                        return @fixed(Symbol.OR);
                    }
                    break;
                case '!':
                    if (c2 == '=')
                    {
                        return @fixed(Symbol.NE);
                    }
                    return @fixed(Symbol.NOT);
                case '<':
                    if (c2 == '=')
                    {
                        return @fixed(Symbol.LE);
                    }
                    return @fixed(Symbol.LT);
                case '>':
                    if (c2 == '=')
                    {
                        return @fixed(Symbol.GE);
                    }
                    return @fixed(Symbol.GT);
                case '"':
                case '\'':
                    return NextString();
            }

            if (IsDigit(c1) || c1 == '.')
            {
                return NextNumber();
            }

            if (_provider.IsValidIdentifier(c1.ToString()))
            {
                int i = position + 1;
                int l = input.Length;
                //while (i < l && _provider.IsValidIdentifier(input[i].ToString()))
                while(i<l && IsValidIdentifierPart(input[i]))
                {
                    i++;
                }
                string name = input.Substring(position, i - position);
                Token keyword = Keyword(name);
                return keyword == null ? NewToken(Symbol.IDENTIFIER, name, i - position) : keyword;
            }

            throw new ScanException(position, "invalid character '" + c1 + "'", "expression token");
        }

        private bool IsValidIdentifierPart(char ch)
        {
            UnicodeCategory uc = Char.GetUnicodeCategory(ch);
            switch (uc)
            {
                case UnicodeCategory.UppercaseLetter:        // Lu
                case UnicodeCategory.LowercaseLetter:        // Ll
                case UnicodeCategory.TitlecaseLetter:        // Lt
                case UnicodeCategory.ModifierLetter:         // Lm
                case UnicodeCategory.LetterNumber:           // Lm
                case UnicodeCategory.OtherLetter:            // Lo

                case UnicodeCategory.NonSpacingMark:         // Mn
                case UnicodeCategory.SpacingCombiningMark:   // Mc
                case UnicodeCategory.ConnectorPunctuation:   // Pc
                case UnicodeCategory.DecimalDigitNumber:     // Nd
                    return true;
                default:
                    return false;
            }
        }

        protected internal virtual Token NextToken()
        {
            if (Eval)
            {
                if (input[position] == '}')
                {
                    return @fixed(Symbol.END_EVAL);
                }
                return NextEval();
            }
            else
            {
                if (position + 1 < input.Length && input[position + 1] == '{')
                {
                    switch (input[position])
                    {
                        case '#':
                            return @fixed(Symbol.START_EVAL_DEFERRED);
                        case '$':
                            return @fixed(Symbol.START_EVAL_DYNAMIC);
                    }
                }
                return NextText();
            }
        }

        /// <summary>
        /// Scan next token.
        /// After calling this method, <seealso cref="#getToken()"/> and <seealso cref="#getPosition()"/>
        /// can be used to retreive the token's image and input position. </summary>
        /// <returns> scanned token </returns>
        public virtual Token Next()
        {
            if (token != null)
            {
                position += token.Size;
            }

            int length = input.Length;

            if (Eval)
            {
                while (position < length && char.IsWhiteSpace(input[position]))
                {
                    position++;
                }
            }

            if (position == length)
            {
                return token = @fixed(Symbol.EOF);
            }

            return token = NextToken();
        }
    }

}