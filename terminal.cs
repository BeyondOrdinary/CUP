namespace CUP
{
	using System;
	
	/// <summary>This class represents a terminal symbol in the grammar.  Each terminal 
	/// has a textual name, an index, and a string which indicates the type of 
	/// object it will be implemented with at runtime (i.e. the class of object 
	/// that will be returned by the scanner and pushed on the parse stack to 
	/// represent it). 
	/// *
	/// </summary>
	/// <version> last updated: 7/3/96
	/// </version>
	/// <author>  Frank Flannery
	/// 
	/// </author>
	public class terminal:symbol
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Full constructor.
		/// </summary>
		/// <param name="nm">the name of the terminal.
		/// </param>
		/// <param name="tp">the type of the terminal.
		/// 
		/// </param>
		public terminal(System.String nm, System.String tp, int precedence_side, int precedence_num):base(nm, tp)
		{
			
			/* add to set of all terminals and check for duplicates */
			System.Object conflict = SupportClass.PutElement(_all, nm, this);
			if (conflict != null)
				(new internal_error("Duplicate terminal (" + nm + ") created")).crash();
			
			/* assign a unique index */
			_index = next_index++;
			
			/* set the precedence */
			_precedence_num = precedence_num;
			_precedence_side = precedence_side;
			
			/* add to by_index set */
			SupportClass.PutElement(_all_by_index, _index, this);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor for non-precedented terminal
		/// </summary>
		
		public terminal(System.String nm, System.String tp):this(nm, tp, assoc.no_prec, - 1)
		{
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor with default type. 
		/// </summary>
		/// <param name="nm">the name of the terminal.
		/// 
		/// </param>
		public terminal(System.String nm):this(nm, null)
		{
		}
		
		/*-----------------------------------------------------------*/
		/*-------------------  Class Variables  ---------------------*/
		/*-----------------------------------------------------------*/
		
		private int _precedence_num;
		private int _precedence_side;
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Static (Class) Variables ------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Table of all terminals.  Elements are stored using name strings as 
		/// the key 
		/// </summary>
		protected internal static System.Collections.Hashtable _all = new System.Collections.Hashtable();
		
		/// <summary>Access to all terminals. 
		/// </summary>
		public static System.Collections.IEnumerator all()
		{
			return _all.Values.GetEnumerator();
		}
		
		/// <summary>Lookup a terminal by name string. 
		/// </summary>
		public static terminal find(System.String with_name)
		{
			if (with_name == null)
				return null;
			else
				return (terminal) _all[with_name];
		}
		
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Table of all terminals indexed by their index number. 
		/// </summary>
		protected internal static System.Collections.Hashtable _all_by_index = new System.Collections.Hashtable();
		
		/// <summary>Lookup a terminal by index. 
		/// </summary>
		public static terminal find(int indx)
		{
			System.Int32 the_indx = indx;
			
			return (terminal) _all_by_index[the_indx];
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Total number of terminals. 
		/// </summary>
		public static int number()
		{
			return _all.Count;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Static counter to assign unique index. 
		/// </summary>
		protected internal static int next_index = 0;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Special terminal for end of input. 
		/// </summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'EOF '. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1003"'
		public static terminal EOF = new terminal("EOF");
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>special terminal used for error recovery 
		/// </summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'error '. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1003"'
		public static terminal error = new terminal("error");
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Report this symbol as not being a non-terminal. 
		/// </summary>
		public override bool is_non_term()
		{
			return false;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to a string. 
		/// </summary>
		public override System.String ToString()
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
			return base.ToString() + "[" + index() + "]";
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>get the precedence of a terminal 
		/// </summary>
		public virtual int precedence_num()
		{
			return _precedence_num;
		}
		public virtual int precedence_side()
		{
			return _precedence_side;
		}
		
		/// <summary>set the precedence of a terminal 
		/// </summary>
		public virtual void  set_precedence(int p, int new_prec)
		{
			_precedence_side = p;
			_precedence_num = new_prec;
		}
		
		/*-----------------------------------------------------------*/
	}
}