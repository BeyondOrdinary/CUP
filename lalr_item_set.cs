namespace CUP
{
	using System;
	
	/// <summary>This class represents a set of LALR items.  For purposes of building
	/// these sets, items are considered unique only if they have unique cores
	/// (i.e., ignoring differences in their lookahead sets).<p>
	/// *
	/// This class provides fairly conventional set oriented operations (union,
	/// sub/super-set tests, etc.), as well as an LALR "closure" operation (see 
	/// compute_closure()).
	/// *
	/// </summary>
	/// <seealso cref="     CUP.lalr_item
	/// "/>
	/// <seealso cref="     CUP.lalr_state
	/// "/>
	/// <version> last updated: 3/6/96
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	
	public class lalr_item_set
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
		public lalr_item_set()
		{
			InitBlock();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor for cloning from another set. 
		/// </summary>
		/// <param name="other">indicates set we should copy from.
		/// 
		/// </param>
		public lalr_item_set(lalr_item_set other)
		{
			InitBlock();
			not_null(other);
			_all = (System.Collections.Hashtable) other._all.Clone();
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>A hash table to implement the set.  We store the items using themselves
		/// as keys. 
		/// </summary>
		//UPGRADE_NOTE: The initialization of  '_all' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		protected internal System.Collections.Hashtable _all;
		
		/// <summary>Access to all elements of the set. 
		/// </summary>
		public virtual System.Collections.IEnumerator all()
		{
			return _all.Values.GetEnumerator();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Cached hashcode for this set. 
		/// </summary>
		protected internal System.Int32 hashcode_cache = int.MinValue;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Size of the set 
		/// </summary>
		public virtual int size()
		{
			return _all.Count;
		}
		
		/*-----------------------------------------------------------*/
		/*--- Set Operation Methods ---------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Does the set contain a particular item? 
		/// </summary>
		/// <param name="itm">the item in question.
		/// 
		/// </param>
		public virtual bool contains(lalr_item itm)
		{
			return _all.ContainsKey(itm);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Return the item in the set matching a particular item (or null if not 
		/// found) 
		/// </summary>
		/// <param name="itm">the item we are looking for.
		/// 
		/// </param>
		public virtual lalr_item find(lalr_item itm)
		{
			return (lalr_item) _all[itm];
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Is this set an (improper) subset of another? 
		/// </summary>
		/// <param name="other">the other set in question.
		/// 
		/// </param>
		public virtual bool is_subset_of(lalr_item_set other)
		{
			not_null(other);
			
			/* walk down our set and make sure every element is in the other */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator e = all(); e.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				if (!other.contains((lalr_item) e.Current))
					return false;
			}
			
			/* they were all there */
			return true;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Is this set an (improper) superset of another? 
		/// </summary>
		/// <param name="other">the other set in question.
		/// 
		/// </param>
		public virtual bool is_superset_of(lalr_item_set other)
		{
			not_null(other);
			return other.is_subset_of(this);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Add a singleton item, merging lookahead sets if the item is already 
		/// part of the set.  returns the element of the set that was added or 
		/// merged into.
		/// </summary>
		/// <param name="itm">the item being added.
		/// 
		/// </param>
		public virtual lalr_item add(lalr_item itm)
		{
			lalr_item other;
			
			not_null(itm);
			
			/* see if an item with a matching core is already there */
			other = (lalr_item) _all[itm];
			
			/* if so, merge this lookahead into the original and leave it */
			if (other != null)
			{
				other.lookahead().add(itm.lookahead());
				return other;
			}
			else
			{
				/* invalidate cached hashcode */
				hashcode_cache = int.MinValue;
				
				SupportClass.PutElement(_all, itm, itm);
				return itm;
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Remove a single item if it is in the set. 
		/// </summary>
		/// <param name="itm">the item to remove.
		/// 
		/// </param>
		public virtual void  remove(lalr_item itm)
		{
			not_null(itm);
			
			/* invalidate cached hashcode */
			hashcode_cache = int.MinValue;
			
			/* remove it from hash table implementing set */
			_all.Remove(itm);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Add a complete set, merging lookaheads where items are already in 
		/// the set 
		/// </summary>
		/// <param name="other">the set to be added.
		/// 
		/// </param>
		public virtual void  add(lalr_item_set other)
		{
			not_null(other);
			
			/* walk down the other set and do the adds individually */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator e = other.all(); e.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				add((lalr_item) e.Current);
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Remove (set subtract) a complete set. 
		/// </summary>
		/// <param name="other">the set to remove.
		/// 
		/// </param>
		public virtual void  remove(lalr_item_set other)
		{
			not_null(other);
			
			/* walk down the other set and do the removes individually */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator e = other.all(); e.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				remove((lalr_item) e.Current);
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Remove and return one item from the set (done in hash order). 
		/// </summary>
		public virtual lalr_item get_one()
		{
			System.Collections.IEnumerator the_set;
			lalr_item result;
			
			the_set = all();
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			if (the_set.MoveNext())
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				result = (lalr_item) the_set.Current;
				remove(result);
				return result;
			}
			else
				return null;
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Helper function for null test.  Throws an interal_error exception if its
		/// parameter is null.
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
		
		/// <summary>Compute the closure of the set using the LALR closure rules.  Basically
		/// for every item of the form: <pre>
		/// [L ::= a *N alpha, l] 
		/// </pre>
		/// (where N is a a non terminal and alpha is a string of symbols) make 
		/// sure there are also items of the form:  <pre>
		/// [N ::= *beta, first(alpha l)] 
		/// </pre>
		/// corresponding to each production of N.  Items with identical cores but 
		/// differing lookahead sets are merged by creating a new item with the same 
		/// core and the union of the lookahead sets (the LA in LALR stands for 
		/// "lookahead merged" and this is where the merger is).  This routine 
		/// assumes that nullability and first sets have been computed for all 
		/// productions before it is called.
		/// </summary>
		public virtual void  compute_closure()
		{
			lalr_item_set consider;
			lalr_item itm, new_itm, add_itm;
			non_terminal nt;
			terminal_set new_lookaheads;
			System.Collections.IEnumerator p;
			production prod;
			bool need_prop;
			
			
			
			/* invalidate cached hashcode */
			hashcode_cache = int.MinValue;
			
			/* each current element needs to be considered */
			consider = new lalr_item_set(this);
			
			/* repeat this until there is nothing else to consider */
			while (consider.size() > 0)
			{
				/* get one item to consider */
				itm = consider.get_one();
				
				/* do we have a dot before a non terminal */
				nt = itm.dot_before_nt();
				if (nt != null)
				{
					/* create the lookahead set based on first after dot */
					new_lookaheads = itm.calc_lookahead(itm.lookahead());
					
					/* are we going to need to propagate our lookahead to new item */
					need_prop = itm.lookahead_visible();
					
					/* create items for each production of that non term */
					//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
					 for (p = nt.productions(); p.MoveNext(); )
					{
						//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
						prod = (production) p.Current;
						
						/* create new item with dot at start and that lookahead */
						new_itm = new lalr_item(prod, new terminal_set(new_lookaheads));
						
						/* add/merge item into the set */
						add_itm = add(new_itm);
						/* if propagation is needed link to that item */
						if (need_prop)
							itm.add_propagate(add_itm);
						
						/* was this was a new item*/
						if (add_itm == new_itm)
						{
							/* that may need further closure, consider it also */
							consider.add(new_itm);
						}
					}
				}
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality comparison. 
		/// </summary>
		public virtual bool equals(lalr_item_set other)
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
				/* can't throw error from here (because superclass doesn't) so crash */
				e.crash();
				return false;
			}
			
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality comparison. 
		/// </summary>
		public  override bool Equals(object other)
		{
			if (!(other is lalr_item_set))
				return false;
			else
				return equals((lalr_item_set) other);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Return hash code. 
		/// </summary>
		public override int GetHashCode()
		{
			int result = 0;
			System.Collections.IEnumerator e;
			int cnt;
			
			/* only compute a new one if we don't have it cached */
			if ((object) hashcode_cache == null)
			{
				/* hash together codes from at most first 5 elements */
				//   CSA fix! we'd *like* to hash just a few elements, but
				//   that means equal sets will have inequal hashcodes, which
				//   we're not allowed (by contract) to do.  So hash them all.
				//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
				 for (e = all(), cnt = 0; e.MoveNext(); cnt++)
				{
					//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
					result ^= ((lalr_item) e.Current).GetHashCode();
				}
				
				hashcode_cache = result;
			}
			
			return hashcode_cache;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to string. 
		/// </summary>
		public override string ToString()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			
			result.Append("{\n");
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator e = all(); e.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				result.Append("  " + (lalr_item) e.Current + "\n");
			}
			result.Append("}");
			
			return result.ToString();
		}
		/*-----------------------------------------------------------*/
	}
}