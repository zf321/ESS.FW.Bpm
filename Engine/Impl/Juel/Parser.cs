using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Common;


namespace ESS.FW.Bpm.Engine.Impl.Juel
{
    

	/// <summary>
	/// Handcrafted top-down parser.
	/// 
	/// 
	/// </summary>
	public class Parser
	{
		/// <summary>
		/// Parse exception type
		/// </summary>
		public class ParseException : System.Exception
		{
			internal readonly int position;
			internal readonly string encountered;
			internal readonly string expected;
			public ParseException(int position, string encountered, string expected) 
                : base($"error.parse  {position}, {encountered}, {expected}")
			{
				this.position = position;
				this.encountered = encountered;
				this.expected = expected;
			}
		}

		/// <summary>
		/// Token type (used to store lookahead)
		/// </summary>
		private sealed class LookaheadToken
		{
			internal readonly Scanner.Token token;
			internal readonly int position;

			internal LookaheadToken(Scanner.Token token, int position)
			{
				this.token = token;
				this.position = position;
			}
		}

		public enum ExtensionPoint
		{
			OR,
			AND,
			EQ,
			CMP,
			ADD,
			MUL,
			UNARY,
			LITERAL
		}

		/// <summary>
		/// Provide limited support for syntax extensions.
		/// </summary>
		public abstract class ExtensionHandler
		{
			internal readonly ExtensionPoint point;

			public ExtensionHandler(ExtensionPoint point)
			{
				this.point = point;
			}

			/// <returns> the extension point specifying where this syntax extension is active </returns>
			public virtual ExtensionPoint ExtensionPoint
			{
				get
				{
					return point;
				}
			}

			/// <summary>
			/// Called by the parser if it handles a extended token associated with this handler
			/// at the appropriate extension point. </summary>
			/// <param name="children"> </param>
			/// <returns> abstract syntax tree node </returns>
			public abstract AstNode CreateAstNode(params AstNode[] children);
		}

		private static readonly string EXPR_FIRST = Scanner.Symbol.IDENTIFIER + "|" + Scanner.Symbol.STRING + "|" + Scanner.Symbol.FLOAT + "|" + Scanner.Symbol.INTEGER + "|" + Scanner.Symbol.TRUE + "|" + Scanner.Symbol.FALSE + "|" + Scanner.Symbol.NULL + "|" + Scanner.Symbol.MINUS + "|" + Scanner.Symbol.NOT + "|" + Scanner.Symbol.EMPTY + "|" + Scanner.Symbol.LPAREN;

		protected internal readonly Builder context;
		protected internal readonly Scanner scanner;

		private IList<IIdentifierNode> identifiers = new List<IIdentifierNode>();
		private IList<IFunctionNode> functions = new List<IFunctionNode>();
		private IList<LookaheadToken> lookahead = new List<LookaheadToken>();

		private Scanner.Token token; // current token
		private int position; // current token's position

		protected internal IDictionary<Scanner.Token, ExtensionHandler> extensions = new Dictionary<Scanner.Token, ExtensionHandler>();

		public Parser(Builder context, string input)
		{
			this.context = context;
			this.scanner = CreateScanner(input);
		}

		protected internal virtual Scanner CreateScanner(string expression)
		{
			return new Scanner(expression);
		}

		public virtual void PutExtensionHandler(Scanner.ExtensionToken token, ExtensionHandler extension)
		{
			if (extensions.Count == 0)
			{
				extensions = new Dictionary<Scanner.Token, ExtensionHandler>(16);
			}
			extensions[token] = extension;
		}

		protected internal virtual ExtensionHandler GetExtensionHandler(Scanner.Token token)
		{
			return extensions[token];
		}

		/// <summary>
		/// Parse an integer literal. </summary>
		/// <param name="string"> string to parse </param>
		/// <returns> <code>long.valueOf(string)</code> </returns>
		protected internal virtual decimal ParseInteger(string @string)
		{
			try
			{
				return Convert.ToInt64(@string);
			}
			catch (System.FormatException)
			{
				Fail(Scanner.Symbol.INTEGER);
				return 0;
			}
		}

		/// <summary>
		/// Parse a floating point literal. </summary>
		/// <param name="string"> string to parse </param>
		/// <returns> <code>Double.valueOf(string)</code> </returns>
		protected internal virtual decimal ParseFloat(string s)
		{
			try
			{
				return Convert.ToDecimal(s);
			}
			catch (System.FormatException)
			{
				Fail(Scanner.Symbol.FLOAT);
				return 0;
			}
		}

		protected internal virtual AstBinary CreateAstBinary(AstNode left, AstNode right, AstBinary.Operator @operator)
		{
			return new AstBinary(left, right, @operator);
		}

		protected internal virtual AstBracket CreateAstBracket(AstNode @base, AstNode property, bool lvalue, bool strict)
		{
			return new AstBracket(@base, property, lvalue, strict);
		}

		protected internal virtual AstChoice CreateAstChoice(AstNode question, AstNode yes, AstNode no)
		{
			return new AstChoice(question, yes, no);
		}

		protected internal virtual AstComposite CreateAstComposite(IList<AstNode> nodes)
		{
			return new AstComposite(nodes);
		}

		protected internal virtual AstDot CreateAstDot(AstNode @base, string property, bool lvalue)
		{
			return new AstDot(@base, property, lvalue);
		}

		protected internal virtual AstFunction CreateAstFunction(string name, int index, AstParameters @params)
		{
			return new AstFunction(name, index, @params, context.IsEnabled(Builder.Feature.VARARGS));
		}

		protected internal virtual AstIdentifier CreateAstIdentifier(string name, int index)
		{
			return new AstIdentifier(name, index);
		}

		protected internal virtual AstMethod CreateAstMethod(AstProperty property, AstParameters @params)
		{
			return new AstMethod(property, @params);
		}

		protected internal virtual AstUnary CreateAstUnary(AstNode child, AstUnary.Operator @operator)
		{
			return new AstUnary(child, @operator);
		}

		protected internal IList<IFunctionNode> Functions
		{
			get
			{
				return functions;
			}
		}

		protected internal IList<IIdentifierNode> Identifiers
		{
			get
			{
				return identifiers;
			}
		}

		protected internal Scanner.Token Token
		{
			get
			{
				return token;
			}
		}

		/// <summary>
		/// throw exception
		/// </summary>
		protected internal virtual void Fail(string expected)
		{
			throw new ParseException(position, "'" + token.Image + "'", expected);
		}

		/// <summary>
		/// throw exception
		/// </summary>
		protected internal virtual void Fail(Scanner.Symbol expected)
		{
			Fail(expected.ToString());
		}

		/// <summary>
		/// get lookahead symbol.
		/// </summary>
		protected internal Scanner.Token Lookahead(int index)
		{
			if (lookahead.Count == 0)
			{
				lookahead = new List<LookaheadToken>();
			}
			while (index >= lookahead.Count)
			{
				lookahead.Add(new LookaheadToken(scanner.Next(), scanner.Position));
			}
			return lookahead[index].token;
		}

		/// <summary>
		/// consume current token (get next token). </summary>
		/// <returns> the consumed token (which was the current token when calling this method) </returns>
		protected internal Scanner.Token ConsumeToken()
		{
			Scanner.Token result = token;
			if (lookahead.Count == 0)
			{
				token = scanner.Next();
				position = scanner.Position;
			}
			else
			{
				LookaheadToken next = lookahead[0];
				token = next.token;
				position = next.position;
                lookahead.RemoveAt(0);
            }
			return result;
		}

		/// <summary>
		/// consume current token (get next token); throw exception if the current token doesn't
		/// match the expected symbol.
		/// </summary>
		protected internal Scanner.Token ConsumeToken(Scanner.Symbol expected)
		{
			if (token.Symbol != expected)
			{
				Fail(expected);
			}
			return ConsumeToken();
		}

		/// <summary>
		/// tree := text? ((dynamic text?)+ | (deferred text?)+)? 
		/// </summary>
		public virtual Tree Tree()
		{
			ConsumeToken();
			AstNode t = Text();
			if (token.Symbol == Scanner.Symbol.EOF)
			{
				if (t == null)
				{
					t = new AstText("");
				}
				return new Tree(t, functions, identifiers, false);
			}
			AstEval e = Eval();
			if (token.Symbol == Scanner.Symbol.EOF && t == null)
			{
				return new Tree(e, functions, identifiers, e.Deferred);
			}
			List<AstNode> list = new List<AstNode>();
			if (t != null)
			{
				list.Add(t);
			}
			list.Add(e);
			t = Text();
			if (t != null)
			{
				list.Add(t);
			}
			while (token.Symbol != Scanner.Symbol.EOF)
			{
				if (e.Deferred)
				{
					list.Add(Eval(true, true));
				}
				else
				{
					list.Add(Eval(true, false));
				}
				t = Text();
				if (t != null)
				{
					list.Add(t);
				}
			}
			return new Tree(CreateAstComposite(list), functions, identifiers, e.Deferred);
		}

		/// <summary>
		/// text := &lt;TEXT&gt;
		/// </summary>
		protected internal virtual AstNode Text()
		{
			AstNode v = null;
			if (token.Symbol == Scanner.Symbol.TEXT)
			{
				v = new AstText(token.Image);
				ConsumeToken();
			}
			return v;
		}

		/// <summary>
		/// eval := dynamic | deferred
		/// </summary>
		protected internal virtual AstEval Eval()
		{
			AstEval e = Eval(false, false);
			if (e == null)
			{
				e = Eval(false, true);
				if (e == null)
				{
					Fail(Scanner.Symbol.START_EVAL_DEFERRED + "|" + Scanner.Symbol.START_EVAL_DYNAMIC);
				}
			}
			return e;
		}

		/// <summary>
		/// dynmamic := &lt;START_EVAL_DYNAMIC&gt; expr &lt;END_EVAL&gt;
		/// deferred := &lt;START_EVAL_DEFERRED&gt; expr &lt;END_EVAL&gt;
		/// </summary>
		protected internal virtual AstEval Eval(bool required, bool deferred)
		{
			AstEval v = null;
			Scanner.Symbol start_eval = deferred ? Scanner.Symbol.START_EVAL_DEFERRED : Scanner.Symbol.START_EVAL_DYNAMIC;
			if (token.Symbol == start_eval)
			{
				ConsumeToken();
				v = new AstEval(Expr(true), deferred);
				ConsumeToken(Scanner.Symbol.END_EVAL);
			}
			else if (required)
			{
				Fail(start_eval);
			}
			return v;
		}

		/// <summary>
		/// expr := or (&lt;QUESTION&gt; expr &lt;COLON&gt; expr)?
		/// </summary>
		protected internal virtual AstNode Expr(bool required)
		{
			AstNode v = Or(required);
			if (v == null)
			{
				return null;
			}
			if (token.Symbol == Scanner.Symbol.QUESTION)
			{
				ConsumeToken();
				AstNode a = Expr(true);
				ConsumeToken(Scanner.Symbol.COLON);
				AstNode b = Expr(true);
				v = CreateAstChoice(v, a, b);
			}
			return v;
		}

		/// <summary>
		/// or := and (&lt;OR&gt; and)*
		/// </summary>
		protected internal virtual AstNode Or(bool required)
		{
			AstNode v = And(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
			    if (token.Symbol == Scanner.Symbol.OR)
			    {
                    ConsumeToken();
                    v = CreateAstBinary(v, And(true), AstBinary.OR);
                }
                else if (token.Symbol == Scanner.Symbol.EXTENSION)
                {
                    if (GetExtensionHandler(token).ExtensionPoint == ExtensionPoint.OR)
                    {
                        v = GetExtensionHandler(ConsumeToken()).CreateAstNode(v, And(true));
                    }
                }
			    else
			    {
			        return v;
			    }
			}
		}

		/// <summary>
		/// and := eq (&lt;AND&gt; eq)*
		/// </summary>
		protected internal virtual AstNode And(bool required)
		{
			AstNode v = Eq(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
                if (token.Symbol == Scanner.Symbol.AND)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Eq(true), AstBinary.AND);
                }
                else if (token.Symbol == Scanner.Symbol.EXTENSION)
                {
                    if (GetExtensionHandler(token).ExtensionPoint == ExtensionPoint.AND)
                    {
                        v = GetExtensionHandler(ConsumeToken()).CreateAstNode(v, Eq(true));
                    }
                }
                else
                {
                    return v;
                }
			}
		}

		/// <summary>
		/// eq := cmp (&lt;EQ&gt; cmp | &lt;NE&gt; cmp)*
		/// </summary>
		protected internal virtual AstNode Eq(bool required)
		{
			AstNode v = Cmp(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
                if (token.Symbol == Scanner.Symbol.EQ)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Cmp(true), AstBinary.EQ);
                }
                else if (token.Symbol == Scanner.Symbol.NE)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Cmp(true), AstBinary.NE);
                }
                else if (token.Symbol == Scanner.Symbol.EXTENSION)
                {
                    if (GetExtensionHandler(token).ExtensionPoint == ExtensionPoint.EQ)
                    {
                        v = GetExtensionHandler(ConsumeToken()).CreateAstNode(v, Cmp(true));
                    }
                }
                else
                {
                    return v;
                }
			}
		}

		/// <summary>
		/// cmp := add (&lt;LT&gt; add | &lt;LE&gt; add | &lt;GE&gt; add | &lt;GT&gt; add)*
		/// </summary>
		protected internal virtual AstNode Cmp(bool required)
		{
			AstNode v = Add(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
                if (token.Symbol == Scanner.Symbol.LT)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Add(true), AstBinary.LT);
                }
                else if (token.Symbol == Scanner.Symbol.LE)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Add(true), AstBinary.LE);
                }
                else if (token.Symbol == Scanner.Symbol.GE)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Add(true), AstBinary.GE);
                }
                else if (token.Symbol == Scanner.Symbol.GT)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Add(true), AstBinary.GT);
                }
                else if (token.Symbol == Scanner.Symbol.EXTENSION)
                {
                    if (GetExtensionHandler(token).ExtensionPoint == ExtensionPoint.CMP)
                    {
                        v = GetExtensionHandler(ConsumeToken()).CreateAstNode(v, Add(true));
                    }
                }
                else
                {
                    return v;
                }
			}
		}

		/// <summary>
		/// add := add (&lt;PLUS&gt; mul | &lt;MINUS&gt; mul)*
		/// </summary>
		protected internal virtual AstNode Add(bool required)
		{
			AstNode v = Mul(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
                if (token.Symbol == Scanner.Symbol.PLUS)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Mul(true), AstBinary.ADD);
                }
                else if (token.Symbol == Scanner.Symbol.MINUS)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Mul(true), AstBinary.SUB);
                }
                else if (token.Symbol == Scanner.Symbol.EXTENSION)
                {
                    if (GetExtensionHandler(token).ExtensionPoint == ExtensionPoint.ADD)
                    {
                        v = GetExtensionHandler(ConsumeToken()).CreateAstNode(v, Mul(true));
                    }
                }
                else
                {
                    return v;
                }
                
			}
		}

		/// <summary>
		/// mul := unary (&lt;MUL&gt; unary | &lt;DIV&gt; unary | &lt;MOD&gt; unary)*
		/// </summary>
		protected internal virtual AstNode Mul(bool required)
		{
			AstNode v = Unary(required);
			if (v == null)
			{
				return null;
			}
			while (true)
			{
                if (token.Symbol == Scanner.Symbol.MUL)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Unary(true), AstBinary.MUL);
                }
                else if (token.Symbol == Scanner.Symbol.DIV)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Unary(true), AstBinary.DIV);
                }
                else if (token.Symbol == Scanner.Symbol.MOD)
                {
                    ConsumeToken();
                    v = CreateAstBinary(v, Unary(true), AstBinary.MOD);
                }
                else if (token.Symbol == Scanner.Symbol.EXTENSION)
                {
                    if (GetExtensionHandler(token).ExtensionPoint == ExtensionPoint.MUL)
                    {
                        v = GetExtensionHandler(ConsumeToken()).CreateAstNode(v, Unary(true));
                    }
                }
                else
                {
                    return v;
                }
			}
		}

		/// <summary>
		/// unary := &lt;NOT&gt; unary | &lt;MINUS&gt; unary | &lt;EMPTY&gt; unary | value
		/// </summary>
		protected internal virtual AstNode Unary(bool required)
		{
			AstNode v = null;
            if (token.Symbol == Scanner.Symbol.NOT)
            {
                ConsumeToken();
                v = CreateAstUnary(Unary(true), AstUnary.NOT);
            }
            else if (token.Symbol == Scanner.Symbol.MINUS)
            {
                ConsumeToken();
                v = CreateAstUnary(Unary(true), AstUnary.NEG);
            }
            else if (token.Symbol == Scanner.Symbol.EMPTY)
            {
                ConsumeToken();
                v = CreateAstUnary(Unary(true), AstUnary.EMPTY);
            }
            else if (token.Symbol == Scanner.Symbol.EXTENSION)
            {
                if (GetExtensionHandler(token).ExtensionPoint == ExtensionPoint.UNARY)
                {
                    v = GetExtensionHandler(ConsumeToken()).CreateAstNode(Unary(true));
                }
            }
            else
            {
                v = Value();
            }
			if (v == null && required)
			{
				Fail(EXPR_FIRST);
			}
			return v;
		}

		/// <summary>
		/// value := (nonliteral | literal) (&lt;DOT&gt; &lt;IDENTIFIER&gt; | &lt;LBRACK&gt; expr &lt;RBRACK&gt;)*
		/// </summary>
		protected internal virtual AstNode Value()
		{
			bool lvalue = true;
			AstNode v = Nonliteral();
			if (v == null)
			{
				v = Literal();
				if (v == null)
				{
					return null;
				}
				lvalue = false;
			}
			while (true)
			{
                if (token.Symbol == Scanner.Symbol.DOT)
                {
                    ConsumeToken();
                    string name = ConsumeToken(Scanner.Symbol.IDENTIFIER).Image;
                    AstDot dot = CreateAstDot(v, name, lvalue);
                    if (token.Symbol == Scanner.Symbol.LPAREN && context.IsEnabled(Builder.Feature.METHOD_INVOCATIONS))
                    {
                        v = CreateAstMethod(dot, @params());
                    }
                    else
                    {
                        v = dot;
                    }
                }
                else if (token.Symbol == Scanner.Symbol.LBRACK)
                {
                    ConsumeToken();
                    AstNode property = Expr(true);
                    bool strict = !context.IsEnabled(Builder.Feature.NULL_PROPERTIES);
                    ConsumeToken(Scanner.Symbol.RBRACK);
                    AstBracket bracket = CreateAstBracket(v, property, lvalue, strict);
                    if (token.Symbol == Scanner.Symbol.LPAREN && context.IsEnabled(Builder.Feature.METHOD_INVOCATIONS))
                    {
                        v = CreateAstMethod(bracket, @params());
                    }
                    else
                    {
                        v = bracket;
                    }
                }
                else
                {
                    return v;
                }
			}
		}

		/// <summary>
		/// nonliteral := &lt;IDENTIFIER&gt; | function | &lt;LPAREN&gt; expr &lt;RPAREN&gt;
		/// function   := (&lt;IDENTIFIER&gt; &lt;COLON&gt;)? &lt;IDENTIFIER&gt; &lt;LPAREN&gt; list? &lt;RPAREN&gt;
		/// </summary>
		protected internal virtual AstNode Nonliteral()
		{
			AstNode v = null;
            if (token.Symbol == Scanner.Symbol.IDENTIFIER)
            {
                string name = ConsumeToken().Image;
                if (token.Symbol == Scanner.Symbol.COLON && Lookahead(0).Symbol == Scanner.Symbol.IDENTIFIER && Lookahead(1).Symbol == Scanner.Symbol.LPAREN)
                { // ns:f(...)
                    ConsumeToken();
                    name += ":" + token.Image;
                    ConsumeToken();
                }
                if (token.Symbol == Scanner.Symbol.LPAREN)
                {
                    v = Function(name, @params());
                }
                else
                { // identifier
                    v = Identifier(name);
                }
            }
            else if (token.Symbol == Scanner.Symbol.LPAREN)
            {
                ConsumeToken();
                v = Expr(true);
                ConsumeToken(Scanner.Symbol.RPAREN);
                v = new AstNested(v);
            }
                return v;
		}

		/// <summary>
		/// params := &lt;LPAREN&gt; (expr (&lt;COMMA&gt; expr)*)? &lt;RPAREN&gt;
		/// </summary>
		protected internal virtual AstParameters @params()
		{
			ConsumeToken(Scanner.Symbol.LPAREN);
			IList<AstNode> l = new List<AstNode>();
			AstNode v = Expr(false);
			if (v != null)
			{
				l = new List<AstNode>();
				l.Add(v);
				while (token.Symbol == Scanner.Symbol.COMMA)
				{
					ConsumeToken();
					l.Add(Expr(true));
				}
			}
			ConsumeToken(Scanner.Symbol.RPAREN);
			return new AstParameters(l);
		}

		/// <summary>
		/// literal := &lt;TRUE&gt; | &lt;FALSE&gt; | &lt;STRING&gt; | &lt;INTEGER&gt; | &lt;FLOAT&gt; | &lt;NULL&gt;
		/// </summary>
		protected internal virtual AstNode Literal()
		{
			AstNode v = null;
		    if (token.Symbol == Scanner.Symbol.TRUE)
		    {
                v = new AstBoolean(true);
                ConsumeToken();
            }
            else if (token.Symbol == Scanner.Symbol.FALSE)
            {
                v = new AstBoolean(false);
                ConsumeToken();
            }
            else if (token.Symbol == Scanner.Symbol.STRING)
            {
                v = new AstString(token.Image);
                ConsumeToken();
            }
            else if (token.Symbol == Scanner.Symbol.INTEGER)
            {
                v = new AstNumber(ParseInteger(token.Image));
                ConsumeToken();
            }
            else if (token.Symbol == Scanner.Symbol.FLOAT)
            {
                v = new AstNumber(ParseFloat(token.Image));
                ConsumeToken();
            }
            else if (token.Symbol == Scanner.Symbol.NULL)
            {
                v = new AstNull();
                ConsumeToken();
            }
            else if (token.Symbol == Scanner.Symbol.EXTENSION)
            {
                if (GetExtensionHandler(token).ExtensionPoint == ExtensionPoint.LITERAL)
                {
                    v = GetExtensionHandler(ConsumeToken()).CreateAstNode();
                }
            }

			return v;
		}

		protected internal AstFunction Function(string name, AstParameters @params)
		{
			if (functions.Count == 0)
			{
				functions = new List<IFunctionNode>(4);
			}
			AstFunction function = CreateAstFunction(name, functions.Count, @params);
			functions.Add(function);
			return function;
		}

		protected internal AstIdentifier Identifier(string name)
		{
			if (identifiers.Count == 0)
			{
				identifiers = new List<IIdentifierNode>(4);
			}
			AstIdentifier identifier = CreateAstIdentifier(name, identifiers.Count);
			identifiers.Add(identifier);
			return identifier;
		}
	}
}