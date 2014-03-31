namespace CUP
{
	using System;
	
	/// <summary>A set of terminals implemented as a bitset. 
	/// </summary>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	public class terminal_set
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		public override int GetHashCode()
		{
			return(base.GetHashCode());
		}

		/// <summary>Constructor for an empty set. 
		/// </summary>
		public terminal_set()
		{
			/* allocate the bitset at what is probably the right size */
			//UPGRADE_NOTE: Class BitArray does not allow calls to methods with index greater than Length property. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1084"'
			_elements = new System.Collections.BitArray((terminal.number() % 64 == 0?terminal.number() / 64:terminal.number() / 64 + 1) * 64);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor for cloning from another set. 
		/// </summary>
		/// <param name="other">the set we are cloning from.
		/// 
		/// </param>
		public terminal_set(terminal_set other)
		{
			not_null(other);
			_elements = (System.Collections.BitArray) other._elements.Clone();
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Static (Class) Variables ------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Constant for the empty set. 
		/// </summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'EMPTY '. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1003"'
		public static terminal_set EMPTY = new terminal_set();
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Bitset to implement the actual set. 
		/// </summary>
		protected internal System.Collections.BitArray _elements;
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Helper function to test for a null object and throw an exception if
		/// one is found. 
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
		
		/// <summary>Determine if the set is empty. 
		/// </summary>
		public virtual bool empty()
		{
			return equals(EMPTY);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Determine if the set contains a particular terminal. 
		/// </summary>
		/// <param name="sym">the terminal symbol we are looking for.
		/// 
		/// </param>
		public virtual bool contains(terminal sym)
		{
			not_null(sym);
			return _elements.Get(sym.index());
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Given its index determine if the set contains a particular terminal. 
		/// </summary>
		/// <param name="indx">the index of the terminal in question.
		/// 
		/// </param>
		public virtual bool contains(int indx)
		{
			return _elements.Get(indx);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Determine if this set is an (improper) subset of another.
		/// </summary>
		/// <param name="other">the set we are testing against.
		/// 
		/// </param>
		public virtual bool is_subset_of(terminal_set other)
		{
			not_null(other);
			
			/* make a copy of the other set */
			System.Collections.BitArray copy_other = (System.Collections.BitArray) other._elements.Clone();
			
			/* and or in */
			copy_other = copy_other.Or(_elements);
			
			/* if it hasn't changed, we were a subset */
			return BitArraysEqual(copy_other, other._elements);
				/// copy_other.Equals(other._elements);
		}

		private bool BitArraysEqual(System.Collections.BitArray bits1, System.Collections.BitArray bits2)
		{
			if(bits1.Count > bits2.Count) 
			{
				// Swap them
				System.Collections.BitArray swap = bits1;
				bits1 = bits2;
				bits2 = swap;
			}
			for(int i=0; i < bits1.Count; i++) 
			{
				if(i >= bits2.Count) 
				{
					// bits1 is longer than bits2.  If there are any
					// bits set in bits1, then the two arrays are not
					// equal.  Zeros don't count if they are trailing
					if(bits1[i]) 
					{
						return(false);
					}
				}
				else 
				{
					if(bits1[i] != bits2[i]) 
					{
						return(false);
					}
				}
			}
			return(true);
		}
		
		private string BitArrayToString(System.Collections.BitArray bits)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for(int i=0; i < bits.Count; i++) 
			{
				sb.Append(bits[i] ? "1" : "0");
			}
			return(sb.ToString());
		}

		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Determine if this set is an (improper) superset of another.
		/// </summary>
		/// <param name="other">the set we are testing against.
		/// 
		/// </param>
		public virtual bool is_superset_of(terminal_set other)
		{
			not_null(other);
			return other.is_subset_of(this);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Add a single terminal to the set.  
		/// </summary>
		/// <param name="sym">the terminal being added.
		/// </param>
		/// <returns>true if this changes the set.
		/// 
		/// </returns>
		public virtual bool add(terminal sym)
		{
			bool result;
			
			not_null(sym);
			
			/* see if we already have this */
			result = _elements.Get(sym.index());
			
			/* if not we add it */
			if (!result)
				_elements.Set(sym.index(), true);
			
			return result;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Remove a terminal if it is in the set.
		/// </summary>
		/// <param name="sym">the terminal being removed.
		/// 
		/// </param>
		public virtual void  remove(terminal sym)
		{
			not_null(sym);
			_elements.Set(sym.index(), false);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Add (union) in a complete set.  
		/// </summary>
		/// <param name="other">the set being added.
		/// </param>
		/// <returns>true if this changes the set.
		/// 
		/// </returns>
		public virtual bool add(terminal_set other)
		{
			not_null(other);
			
			/* make a copy */
			System.Collections.BitArray copy = (System.Collections.BitArray) _elements.Clone();
			
			/* or in the other set */
			//UPGRADE_NOTE: In .NET BitArrays must be of the same size to allow the 'System.Collections.BitArray.Or' operation. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1083"'
			_elements = _elements.Or(other._elements);
			
			/* changed if we are not the same as the copy */
			//UPGRADE_TODO: method 'java.util.BitSet.equals' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilBitSetequals_javalangObject"'
			return !BitArraysEqual(_elements, copy);
			// _elements.Equals(copy);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Determine if this set intersects another.
		/// </summary>
		/// <param name="other">the other set in question.
		/// 
		/// </param>
		public virtual bool intersects(terminal_set other)
		{
			not_null(other);
			
			/* make a copy of the other set */
			System.Collections.BitArray copy = (System.Collections.BitArray) other._elements.Clone();
			
			/* xor out our values */
			//UPGRADE_NOTE: In .NET BitArrays must be of the same size to allow the 'System.Collections.BitArray.Xor' operation. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1083"'
			copy = copy.Xor(this._elements);
			
			/* see if its different */
			//UPGRADE_TODO: method 'java.util.BitSet.equals' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilBitSetequals_javalangObject"'
			return !BitArraysEqual(copy, other._elements);
			// copy.Equals(other._elements);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality comparison. 
		/// </summary>
		public virtual bool equals(terminal_set other)
		{
			if (other == null)
				return false;
			else
			{
				//UPGRADE_TODO: method 'java.util.BitSet.equals' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilBitSetequals_javalangObject"'
				return _elements.Equals(other._elements);
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality comparison. 
		/// </summary>
		public  override bool Equals(object other)
		{
			if (!(other is terminal_set))
				return false;
			else
				return equals((terminal_set) other);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to string. 
		/// </summary>
		public override string ToString()
		{
			System.String result;
			bool comma_flag;
			
			result = "{";
			comma_flag = false;
			 for (int t = 0; t < terminal.number(); t++)
			{
				if (_elements.Get(t))
				{
					if (comma_flag)
						result += ", ";
					else
						comma_flag = true;
					
					result += terminal.find(t).name_Renamed_Method();
				}
			}
			result += "}";
			
			return result;
		}
		
		/*-----------------------------------------------------------*/
	}
}