namespace CUP
{
	using System;
	
	/// <summary>This class represents a set of symbols and provides a series of 
	/// set operations to manipulate them.
	/// *
	/// </summary>
	/// <seealso cref="     CUP.symbol
	/// "/>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	public class symbol_set
	{
		private void  InitBlock()
		{
			_all = new System.Collections.Hashtable(11);
		}
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Constructor for an empty set. 
		/// </summary>
		public symbol_set()
		{
			InitBlock();
		}
		
		/// <summary>Constructor for cloning from another set. 
		/// </summary>
		/// <param name="other">the set we are cloning from.
		/// 
		/// </param>
		public symbol_set(symbol_set other)
		{
			InitBlock();
			not_null(other);
			_all = (System.Collections.Hashtable) other._all.Clone();
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>A hash table to hold the set. Symbols are keyed using their name string. 
		/// </summary>
		//UPGRADE_NOTE: The initialization of  '_all' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		protected internal System.Collections.Hashtable _all;
		
		/// <summary>Access to all elements of the set. 
		/// </summary>
		public virtual System.Collections.IEnumerator all()
		{
			return _all.Values.GetEnumerator();
		}
		
		/// <summary>size of the set 
		/// </summary>
		public virtual int size()
		{
			return _all.Count;
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Helper function to test for a null object and throw an exception
		/// if one is found.
		/// </summary>
		/// <param name="obj">the object we are testing.
		/// 
		/// </param>
		protected internal virtual void  not_null(object obj)
		{
			if (obj == null)
				throw new internal_error("Null object used in set operation");
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Determine if the set contains a particular symbol. 
		/// </summary>
		/// <param name="sym">the symbol we are looking for.
		/// 
		/// </param>
		public virtual bool contains(symbol sym)
		{
			return _all.ContainsKey(sym.name_Renamed_Method());
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Determine if this set is an (improper) subset of another. 
		/// </summary>
		/// <param name="other">the set we are testing against.
		/// 
		/// </param>
		public virtual bool is_subset_of(symbol_set other)
		{
			not_null(other);
			
			/* walk down our set and make sure every element is in the other */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator e = all(); e.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				if (!other.contains((symbol) e.Current))
					return false;
			}
			
			/* they were all there */
			return true;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Determine if this set is an (improper) superset of another. 
		/// </summary>
		/// <param name="other">the set we are are testing against.
		/// 
		/// </param>
		public virtual bool is_superset_of(symbol_set other)
		{
			not_null(other);
			return other.is_subset_of(this);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Add a single symbol to the set.  
		/// </summary>
		/// <param name="sym">the symbol we are adding.
		/// </param>
		/// <returns>true if this changes the set.
		/// 
		/// </returns>
		public virtual bool add(symbol sym)
		{
			System.Object previous;
			
			not_null(sym);
			
			/* put the object in */
			previous = SupportClass.PutElement(_all, sym.name_Renamed_Method(), sym);
			
			/* if we had a previous, this is no change */
			return previous == null;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Remove a single symbol if it is in the set. 
		/// </summary>
		/// <param name="sym">the symbol we are removing.
		/// 
		/// </param>
		public virtual void  remove(symbol sym)
		{
			not_null(sym);
			_all.Remove(sym.name_Renamed_Method());
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Add (union) in a complete set.  
		/// </summary>
		/// <param name="other">the set we are adding in.
		/// </param>
		/// <returns>true if this changes the set. 
		/// 
		/// </returns>
		public virtual bool add(symbol_set other)
		{
			bool result = false;
			
			not_null(other);
			
			/* walk down the other set and do the adds individually */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator e = other.all(); e.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				result = add((symbol) e.Current) || result;
			}
			
			return result;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Remove (set subtract) a complete set. 
		/// </summary>
		/// <param name="other">the set we are removing.
		/// 
		/// </param>
		public virtual void  remove(symbol_set other)
		{
			not_null(other);
			
			/* walk down the other set and do the removes individually */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator e = other.all(); e.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				remove((symbol) e.Current);
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality comparison. 
		/// </summary>
		public virtual bool equals(symbol_set other)
		{
			if (other == null || other.size() != size())
				return false;
			
			/* once we know they are the same size, then improper subset does test */
			try
			{
				return is_subset_of(other);
			}
			catch (internal_error e)
			{
				/* can't throw the error (because super class doesn't), so we crash */
				e.crash();
				return false;
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality comparison. 
		/// </summary>
		public  override bool Equals(object other)
		{
			if (!(other is symbol_set))
				return false;
			else
				return equals((symbol_set) other);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Compute a hash code. 
		/// </summary>
		public override int GetHashCode()
		{
			int result = 0;
			int cnt;
			System.Collections.IEnumerator e;
			
			/* hash together codes from at most first 5 elements */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (e = all(), cnt = 0; e.MoveNext() && cnt < 5; cnt++)
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				result ^= ((symbol) e.Current).GetHashCode();
			}
			
			return result;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to a string. 
		/// </summary>
		public override string ToString()
		{
			System.String result;
			bool comma_flag;
			
			result = "{";
			comma_flag = false;
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator e = all(); e.MoveNext(); )
			{
				if (comma_flag)
					result += ", ";
				else
					comma_flag = true;
				
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				result += ((symbol) e.Current).name_Renamed_Method();
			}
			result += "}";
			
			return result;
		}
		
		/*-----------------------------------------------------------*/
	}
}