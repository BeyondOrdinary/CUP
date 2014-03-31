using System;
using Symbol = CUP.runtime.Symbol;
using Scanner = CUP.runtime.Scanner;
using System.Text;

// Simple Example Scanner Class
namespace simple_calc
{
	
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
					case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9': case '.':
						/* parse a decimal integer */
                        StringBuilder sb = new StringBuilder();
						decimal d_val = 0;
						do 
						{
                            sb.Append((char)next_char);
							advance();
						}
						while ((next_char >= '0' && next_char <= '9') || next_char == '.');
                        d_val = decimal.Parse(sb.ToString());
						return new Symbol(sym.REAL, d_val);
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