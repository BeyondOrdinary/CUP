namespace CUP
{
	using System;
	
	/// <summary>This abstract class serves as the base class for grammar symbols (i.e.,
	/// both terminals and non-terminals).  Each symbol has a name string, and
	/// a string giving the type of object that the symbol will be represented by
	/// on the runtime parse stack.  In addition, each symbol maintains a use count
	/// in order to detect symbols that are declared but never used, and an index
	/// number that indicates where it appears in parse tables (index numbers are
	/// unique within terminals or non terminals, but not across both).
	/// *
	/// </summary>
	/// <seealso cref="     CUP.terminal
	/// "/>
	/// <seealso cref="     CUP.non_terminal
	/// "/>
	/// <version> last updated: 7/3/96
	/// </version>
	/// <author>  Frank Flannery
	/// 
	/// </author>
	public abstract class symbol
	{
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Full constructor.
		/// </summary>
		/// <param name="nm">the name of the symbol.
		/// </param>
		/// <param name="tp">a string with the type name.
		/// 
		/// </param>
		public symbol(string nm, string tp)
		{
			/* sanity check */
			if (nm == null)
				nm = "";
			
			/* apply default if no type given */
			if (tp == null)
				tp = "Object";
			
			_name = nm;
			_stack_type = tp;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor with default type. 
		/// </summary>
		/// <param name="nm">the name of the symbol.
		/// 
		/// </param>
		public symbol(string nm):this(nm, null)
		{
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>String for the human readable name of the symbol. 
		/// </summary>
		protected string _name;
		
		/// <summary>String for the human readable name of the symbol. 
		/// </summary>
		//UPGRADE_NOTE: Method name was renamed. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1029"'
		public virtual string name_Renamed_Method()
		{
			return _name;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>String for the type of object used for the symbol on the parse stack. 
		/// </summary>
		protected string _stack_type;
		
		/// <summary>String for the type of object used for the symbol on the parse stack. 
		/// </summary>
		public virtual string stack_type()
		{
			return _stack_type;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Count of how many times the symbol appears in productions. 
		/// </summary>
		protected int _use_count = 0;
		
		/// <summary>Count of how many times the symbol appears in productions. 
		/// </summary>
		public virtual int use_count()
		{
			return _use_count;
		}
		
		/// <summary>Increment the use count. 
		/// </summary>
		public virtual void  note_use()
		{
			_use_count++;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Index of this symbol (terminal or non terminal) in the parse tables.
		/// Note: indexes are unique among terminals and unique among non terminals,
		/// however, a terminal may have the same index as a non-terminal, etc. 
		/// </summary>
		protected int _index;
		
		/// <summary>Index of this symbol (terminal or non terminal) in the parse tables.
		/// Note: indexes are unique among terminals and unique among non terminals,
		/// however, a terminal may have the same index as a non-terminal, etc. 
		/// </summary>
		public virtual int index()
		{
			return _index;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Indicate if this is a non-terminal.  Here in the base class we
		/// don't know, so this is abstract.  
		/// </summary>
		public abstract bool is_non_term();
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to a string. 
		/// </summary>
		public override string ToString()
		{
			return name_Renamed_Method();
		}
		
		/*-----------------------------------------------------------*/
	}
}