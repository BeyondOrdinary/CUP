namespace CUP
{
	using System;
	
	/// <summary>This class represents a non-terminal symbol in the grammar.  Each
	/// non terminal has a textual name, an index, and a string which indicates
	/// the type of object it will be implemented with at runtime (i.e. the class
	/// of object that will be pushed on the parse stack to represent it). 
	/// *
	/// </summary>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	
	public class non_terminal:symbol
	{
		private void  InitBlock()
		{
			_productions = new System.Collections.Hashtable(11);
			_first_set = new terminal_set();
		}
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Full constructor.
		/// </summary>
		/// <param name="nm"> the name of the non terminal.
		/// </param>
		/// <param name="tp"> the type string for the non terminal.
		/// 
		/// </param>
		public non_terminal(System.String nm, System.String tp):base(nm, tp)
		{
			InitBlock();
			
			/* add to set of all non terminals and check for duplicates */
			System.Object conflict = SupportClass.PutElement(_all, nm, this);
			if (conflict != null)
				(new internal_error("Duplicate non-terminal (" + nm + ") created")).crash();
			
			/* assign a unique index */
			_index = next_index++;
			
			/* add to by_index set */
			SupportClass.PutElement(_all_by_index, _index, this);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor with default type. 
		/// </summary>
		/// <param name="nm"> the name of the non terminal.
		/// 
		/// </param>
		public non_terminal(System.String nm):this(nm, null)
		{
			InitBlock();
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Static (Class) Variables ------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Table of all non-terminals -- elements are stored using name strings 
		/// as the key 
		/// </summary>
		protected internal static System.Collections.Hashtable _all = new System.Collections.Hashtable();
		
		/// <summary>Access to all non-terminals. 
		/// </summary>
		public static System.Collections.IEnumerator all()
		{
			return _all.Values.GetEnumerator();
		}
		
		/// <summary>lookup a non terminal by name string 
		/// </summary>
		public static non_terminal find(System.String with_name)
		{
			if (with_name == null)
				return null;
			else
				return (non_terminal) _all[with_name];
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Table of all non terminals indexed by their index number. 
		/// </summary>
		protected internal static System.Collections.Hashtable _all_by_index = new System.Collections.Hashtable();
		
		/// <summary>Lookup a non terminal by index. 
		/// </summary>
		public static non_terminal find(int indx)
		{
			System.Int32 the_indx = indx;
			
			return (non_terminal) _all_by_index[the_indx];
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Total number of non-terminals. 
		/// </summary>
		public static int number()
		{
			return _all.Count;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Static counter to assign unique indexes. 
		/// </summary>
		protected internal static int next_index = 0;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Static counter for creating unique non-terminal names 
		/// </summary>
		protected internal static int next_nt = 0;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>special non-terminal for start symbol 
		/// </summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'START_nt '. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1003"'
		public static non_terminal START_nt = new non_terminal("$START");
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>flag non-terminals created to embed action productions 
		/// </summary>
		public bool is_embedded_action = false; /* added 24-Mar-1998, CSA */
		
		/*-----------------------------------------------------------*/
		/*--- Static Methods ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Method for creating a new uniquely named hidden non-terminal using 
		/// the given string as a base for the name (or "NT$" if null is passed).
		/// </summary>
		/// <param name="prefix">base name to construct unique name from. 
		/// 
		/// </param>
		internal static non_terminal create_new(System.String prefix)
		{
			if (prefix == null)
				prefix = "NT$";
			return new non_terminal(prefix + next_nt++);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>static routine for creating a new uniquely named hidden non-terminal 
		/// </summary>
		internal static non_terminal create_new()
		{
			return create_new(null);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Compute nullability of all non-terminals. 
		/// </summary>
		public static void  compute_nullability()
		{
			bool change = true;
			non_terminal nt;
			System.Collections.IEnumerator e;
			production prod;
			
			/* repeat this process until there is no change */
			while (change)
			{
				/* look for a new change */
				change = false;
				
				/* consider each non-terminal */
				//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
				 for (e = all(); e.MoveNext(); )
				{
					//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
					nt = (non_terminal) e.Current;
					
					/* only look at things that aren't already marked nullable */
					if (!nt.nullable())
					{
						if (nt.looks_nullable())
						{
							nt._nullable = true;
							change = true;
						}
					}
				}
			}
			
			/* do one last pass over the productions to finalize all of them */
			 for (e = production.all(); e.MoveNext(); )
			{
				prod = (production) e.Current;
				prod.set_nullable(prod.check_nullable());
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Compute first sets for all non-terminals.  This assumes nullability has
		/// already computed.
		/// </summary>
		public static void  compute_first_sets()
		{
			bool change = true;
			System.Collections.IEnumerator n;
			System.Collections.IEnumerator p;
			non_terminal nt;
			production prod;
			terminal_set prod_first;
			
			/* repeat this process until we have no change */
			while (change)
			{
				/* look for a new change */
				change = false;
				
				/* consider each non-terminal */
				 for (n = all(); n.MoveNext(); )
				{
					nt = (non_terminal) n.Current;
					
					/* consider every production of that non terminal */
					 for (p = nt.productions(); p.MoveNext(); )
					{
						prod = (production) p.Current;
						
						/* get the updated first of that production */
						prod_first = prod.check_first_set();
						
						/* if this going to add anything, add it */
						if (!prod_first.is_subset_of(nt._first_set))
						{
							change = true;
							nt._first_set.add(prod_first);
						}
					}
				}
			}
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Table of all productions with this non terminal on the LHS. 
		/// </summary>
		//UPGRADE_NOTE: The initialization of  '_productions' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		protected internal System.Collections.Hashtable _productions;
		
		/// <summary>Access to productions with this non terminal on the LHS. 
		/// </summary>
		public virtual System.Collections.IEnumerator productions()
		{
			return _productions.Values.GetEnumerator();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Total number of productions with this non terminal on the LHS. 
		/// </summary>
		public virtual int num_productions()
		{
			return _productions.Count;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Add a production to our set of productions. 
		/// </summary>
		public virtual void  add_production(production prod)
		{
			/* catch improper productions */
			if (prod == null || prod.lhs() == null || prod.lhs().the_symbol() != this)
				throw new internal_error("Attempt to add invalid production to non terminal production table");
			
			/* add it to the table, keyed with itself */
			SupportClass.PutElement(_productions, prod, prod);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Nullability of this non terminal. 
		/// </summary>
		protected internal bool _nullable;
		
		/// <summary>Nullability of this non terminal. 
		/// </summary>
		public virtual bool nullable()
		{
			return _nullable;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>First set for this non-terminal. 
		/// </summary>
		//UPGRADE_NOTE: The initialization of  '_first_set' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		protected internal terminal_set _first_set;
		
		/// <summary>First set for this non-terminal. 
		/// </summary>
		public virtual terminal_set first_set()
		{
			return _first_set;
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Indicate that this symbol is a non-terminal. 
		/// </summary>
		public override bool is_non_term()
		{
			return true;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Test to see if this non terminal currently looks nullable. 
		/// </summary>
		protected internal virtual bool looks_nullable()
		{
			/* look and see if any of the productions now look nullable */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator e = productions(); e.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				if (((production) e.Current).check_nullable())
					return true;
			}
			
			/* none of the productions can go to empty, so we are not nullable */
			return false;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>convert to string 
		/// </summary>
		public override System.String ToString()
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
			return base.ToString() + "[" + index() + "]" + (nullable()?"*":"");
		}
		
		/*-----------------------------------------------------------*/
	}
}