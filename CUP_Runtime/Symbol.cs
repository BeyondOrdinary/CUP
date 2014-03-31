namespace CUP.runtime
{
	using System;
	
	/// <summary> Defines the Symbol class, which is used to represent all terminals
	/// and nonterminals while parsing.  The lexer should pass CUP Symbols 
	/// and CUP returns a Symbol.
	/// *
	/// </summary>
	/// <version> last updated: 7/3/96
	/// </version>
	/// <author>  Frank Flannery
	/// 
	/// </author>
	
	/* ****************************************************************
	Class Symbol
	what the parser expects to receive from the lexer. 
	the token is identified as follows:
	sym:    the symbol type
	parse_state: the parse state.
	value:  is the lexical value of type Object
	left :  is the left position in the original input file
	right:  is the right position in the original input file
	******************************************************************/
	
	public class Symbol
	{
		
		/// <summary>****************************
		/// Constructor for l,r values
		/// *****************************
		/// </summary>
		
		public Symbol(int id, int l, int r, object o):this(id)
		{
			left = l;
			right = r;
			Value = o;
		}
		
		/// <summary>****************************
		/// Constructor for no l,r values
		/// ******************************
		/// </summary>
		
		public Symbol(int id, object o):this(id, - 1, - 1, o)
		{
		}
		
		/// <summary>**************************
		/// Constructor for no value
		/// *************************
		/// </summary>
		
		public Symbol(int id, int l, int r):this(id, l, r, null)
		{
		}
		
		/// <summary>********************************
		/// Constructor for no value or l,r
		/// *********************************
		/// </summary>
		
		public Symbol(int sym_num):this(sym_num, - 1)
		{
			left = - 1;
			right = - 1;
			Value = null;
		}
		
		/// <summary>********************************
		/// Constructor to give a start state
		/// *********************************
		/// </summary>
		internal Symbol(int sym_num, int state)
		{
			sym = sym_num;
			parse_state = state;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>The symbol number of the terminal or non terminal being represented 
		/// </summary>
		public int sym;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>The parse state to be recorded on the parse stack with this symbol.
		/// This field is for the convenience of the parser and shouldn't be 
		/// modified except by the parser. 
		/// </summary>
		public int parse_state;
		/// <summary>This allows us to catch some errors caused by scanners recycling
		/// symbols.  For the use of the parser only. [CSA, 23-Jul-1999] 
		/// </summary>
		internal bool used_by_parser = false;
		
		/// <summary>****************************
		/// The data passed to parser
		/// *****************************
		/// </summary>
		
		public int left, right;
		public object Value;
		
		/// <summary>**************************
		/// Printing this token out. (Override for pretty-print).
		/// **************************
		/// </summary>
		public override string ToString()
		{
			return "#" + sym;
		}
	}
}