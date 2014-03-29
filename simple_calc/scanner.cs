// Simple Example Scanner Class
namespace simple_calc
{
	using System;
	using Symbol = CUP.runtime.Symbol;
	using Scanner = CUP.runtime.Scanner;
	
	public class scanner : Scanner
	{
		private System.IO.Stream instream;
		
		public scanner(System.IO.Stream is_Renamed)
		{
			instream = is_Renamed;
		}
		
		/* single lookahead character */
		protected internal int next_char = - 2;
		
		/* advance input by one character */
		protected internal virtual void  advance()
		{
			next_char = instream.ReadByte();
		}
		
		/* initialize the scanner */
		private void  init()
		{
			advance();
		}
		
		/* recognize and return the next complete token */
		public virtual Symbol next_token()
		{
			if (next_char == - 2)
				init();
			// set stuff up first time we are called.
			 for (; ; )
				switch (next_char)
				{
					case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9': 
						/* parse a decimal integer */
						int i_val = 0;
						do 
						{
							i_val = i_val * 10 + (next_char - '0');
							advance();
						}
						while (next_char >= '0' && next_char <= '9');
						return new Symbol(sym.NUMBER, i_val);
						
					
					
					case ';': 
						advance(); return new Symbol(sym.SEMI);
					
					case '+': 
						advance(); return new Symbol(sym.PLUS);
					
					case '-': 
						advance(); return new Symbol(sym.MINUS);
					
					case '*': 
						advance(); return new Symbol(sym.TIMES);
					
					case '/': 
						advance(); return new Symbol(sym.DIVIDE);
					
					case '%': 
						advance(); return new Symbol(sym.MOD);
					
					case '(': 
						advance(); return new Symbol(sym.LPAREN);
					
					case ')': 
						advance(); return new Symbol(sym.RPAREN);
						
					
					
					case - 1: 
						return new Symbol(sym.EOF);
						
					
					
					default: 
						/* in this simple scanner we just ignore everything else */
						advance();
						break;
					
				}
		}
	}
	
}