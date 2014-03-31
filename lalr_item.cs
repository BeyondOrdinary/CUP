namespace CUP
{
	using System;
	
	/// <summary>This class represents an LALR item. Each LALR item consists of 
	/// a production, a "dot" at a position within that production, and
	/// a set of lookahead symbols (terminal).  (The first two of these parts
	/// are provide by the super class).  An item is designed to represent a 
	/// configuration that the parser may be in.  For example, an item of the 
	/// form: <pre>
	/// [A ::= B * C d E  , {a,b,c}]
	/// </pre>
	/// indicates that the parser is in the middle of parsing the production <pre>
	/// A ::= B C d E
	/// </pre>
	/// that B has already been parsed, and that we will expect to see a lookahead 
	/// of either a, b, or c once the complete RHS of this production has been 
	/// found.<p>
	/// *
	/// Items may initially be missing some items from their lookahead sets.  
	/// Links are maintained from each item to the set of items that would need 
	/// to be updated if symbols are added to its lookahead set.  During 
	/// "lookahead propagation", we add symbols to various lookahead sets and 
	/// propagate these changes across these dependency links as needed. 
	/// 
	/// </summary>
	/// <seealso cref="     CUP.lalr_item_set
	/// "/>
	/// <seealso cref="     CUP.lalr_state
	/// "/>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	public class lalr_item:lr_item_core
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Full constructor. 
		/// </summary>
		/// <param name="prod">the production for the item.
		/// </param>
		/// <param name="pos"> the position of the "dot" within the production.
		/// </param>
		/// <param name="look">the set of lookahead symbols.
		/// 
		/// </param>
		public lalr_item(production prod, int pos, terminal_set look):base(prod, pos)
		{
			_lookahead = look;
			_propagate_items = new CUP.runtime.SymbolStack();
			needs_propagation = true;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor with default position (dot at start). 
		/// </summary>
		/// <param name="prod">the production for the item.
		/// </param>
		/// <param name="look">the set of lookahead symbols.
		/// 
		/// </param>
		public lalr_item(production prod, terminal_set look):this(prod, 0, look)
		{
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor with default position and empty lookahead set. 
		/// </summary>
		/// <param name="prod">the production for the item.
		/// 
		/// </param>
		public lalr_item(production prod):this(prod, 0, new terminal_set())
		{
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>The lookahead symbols of the item. 
		/// </summary>
		protected internal terminal_set _lookahead;
		
		/// <summary>The lookahead symbols of the item. 
		/// </summary>
		public virtual terminal_set lookahead()
		{
			return _lookahead;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Links to items that the lookahead needs to be propagated to. 
		/// </summary>
		protected internal CUP.runtime.SymbolStack _propagate_items;
		
		/// <summary>Links to items that the lookahead needs to be propagated to 
		/// </summary>
		public virtual CUP.runtime.SymbolStack propagate_items()
		{
			return _propagate_items;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Flag to indicate that this item needs to propagate its lookahead 
		/// (whether it has changed or not). 
		/// </summary>
		protected internal bool needs_propagation;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Add a new item to the set of items we propagate to. 
		/// </summary>
		public virtual void  add_propagate(lalr_item prop_to)
		{
			System.Object temp_object;
			temp_object = prop_to;
			System.Object generatedAux = temp_object;
			_propagate_items.Push(temp_object);
			needs_propagation = true;
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Propagate incoming lookaheads through this item to others need to 
		/// be changed.
		/// </summary>
		/// <param name="incoming">symbols to potentially be added to lookahead of this item.
		/// 
		/// </param>
		public virtual void  propagate_lookaheads(terminal_set incoming)
		{
			bool change = false;
			
			/* if we don't need to propagate, then bail out now */
			if (!needs_propagation && (incoming == null || incoming.empty()))
				return ;
			
			/* if we have null incoming, treat as an empty set */
			if (incoming != null)
			{
				/* add the incoming to the lookahead of this item */
				change = lookahead().add(incoming);
			}
			
			/* if we changed or need it anyway, propagate across our links */
			if (change || needs_propagation)
			{
				/* don't need to propagate again */
				needs_propagation = false;
				
				/* propagate our lookahead into each item we are linked to */
				 for (int i = 0; i < propagate_items().Count; i++)
					((lalr_item) (propagate_items().Peek(propagate_items().Count - (i + 1)))).propagate_lookaheads(lookahead());
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Produce the new lalr_item that results from shifting the dot one position
		/// to the right. 
		/// </summary>
		public virtual lalr_item shift()
		{
			lalr_item result;
			
			/* can't shift if we have dot already at the end */
			if (dot_at_end())
				throw new internal_error("Attempt to shift past end of an lalr_item");
			
			/* create the new item w/ the dot shifted by one */
			result = new lalr_item(the_production(), dot_pos() + 1, new terminal_set(lookahead()));
			
			/* change in our lookahead needs to be propagated to this item */
			add_propagate(result);
			
			return result;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Calculate lookahead representing symbols that could appear after the
		/// symbol that the dot is currently in front of.  Note: this routine must
		/// not be invoked before first sets and nullability has been calculated
		/// for all non terminals. 
		/// </summary>
		public virtual terminal_set calc_lookahead(terminal_set lookahead_after)
		{
			terminal_set result;
			int pos;
			production_part part;
			symbol sym;
			
			/* sanity check */
			if (dot_at_end())
				throw new internal_error("Attempt to calculate a lookahead set with a completed item");
			
			/* start with an empty result */
			result = new terminal_set();
			
			/* consider all nullable symbols after the one to the right of the dot */
			 for (pos = dot_pos() + 1; pos < the_production().rhs_length(); pos++)
			{
				part = the_production().rhs(pos);
				
				/* consider what kind of production part it is -- skip actions */
				if (!part.is_action())
				{
					sym = ((symbol_part) part).the_symbol();
					
					/* if its a terminal add it in and we are done */
					if (!sym.is_non_term())
					{
						result.add((terminal) sym);
						return result;
					}
					else
					{
						/* otherwise add in first set of the non terminal */
						result.add(((non_terminal) sym).first_set());
						
						/* if its nullable we continue adding, if not, we are done */
						if (!((non_terminal) sym).nullable())
							return result;
					}
				}
			}
			
			/* if we get here everything past the dot was nullable 
			we add in the lookahead for after the production and we are done */
			result.add(lookahead_after);
			return result;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Determine if everything from the symbol one beyond the dot all the 
		/// way to the  end of the right hand side is nullable.  This would indicate
		/// that the lookahead of this item must be included in the lookaheads of
		/// all items produced as a closure of this item.  Note: this routine should 
		/// not be invoked until after first sets and nullability have been 
		/// calculated for all non terminals. 
		/// </summary>
		public virtual bool lookahead_visible()
		{
			production_part part;
			symbol sym;
			
			/* if the dot is at the end, we have a problem, but the cleanest thing
			to do is just return true. */
			if (dot_at_end())
				return true;
			
			/* walk down the rhs and bail if we get a non-nullable symbol */
			 for (int pos = dot_pos() + 1; pos < the_production().rhs_length(); pos++)
			{
				part = the_production().rhs(pos);
				
				/* skip actions */
				if (!part.is_action())
				{
					sym = ((symbol_part) part).the_symbol();
					
					/* if its a terminal we fail */
					if (!sym.is_non_term())
						return false;
					
					/* if its not nullable we fail */
					if (!((non_terminal) sym).nullable())
						return false;
				}
			}
			
			/* if we get here its all nullable */
			return true;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality comparison -- here we only require the cores to be equal since
		/// we need to do sets of items based only on core equality (ignoring 
		/// lookahead sets). 
		/// </summary>
		public virtual bool equals(lalr_item other)
		{
			if (other == null)
				return false;
			return base.Equals(other);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality comparison. 
		/// </summary>
		public  override bool Equals(object other)
		{
			if (!(other is lalr_item))
				return false;
			else
				return equals((lalr_item) other);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Return a hash code -- here we only hash the core since we only test core
		/// matching in LALR items. 
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to string. 
		/// </summary>
		public override string ToString()
		{
			System.String result = "";
			
			// additional output for debugging:
			// result += "(" + obj_hash() + ")"; 
			result += "[";
			//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
			result += base.ToString();
			result += ", ";
			if (lookahead() != null)
			{
				result += "{";
				 for (int t = 0; t < terminal.number(); t++)
					if (lookahead().contains(t))
						result += terminal.find(t).name_Renamed_Method() + " ";
				result += "}";
			}
			else
				result += "NULL LOOKAHEAD!!";
			result += "]";
			
			// additional output for debugging:
			// result += " -> ";
			// for (int i = 0; i<propagate_items().size(); i++)
			//   result+=((lalr_item)(propagate_items().Peek(i))).obj_hash()+" ";
			//
			// if (needs_propagation) result += " NP";
			
			return result;
		}
		/*-----------------------------------------------------------*/
	}
}