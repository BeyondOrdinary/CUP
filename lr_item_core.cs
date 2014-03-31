namespace CUP
{
	using System;
	
	/// <summary>The "core" of an LR item.  This includes a production and the position
	/// of a marker (the "dot") within the production.  Typically item cores 
	/// are written using a production with an embedded "dot" to indicate their 
	/// position.  For example: <pre>
	/// A ::= B * C d E
	/// </pre>
	/// This represents a point in a parse where the parser is trying to match
	/// the given production, and has succeeded in matching everything before the 
	/// "dot" (and hence is expecting to see the symbols after the dot next).  See 
	/// lalr_item, lalr_item_set, and lalr_start for full details on the meaning 
	/// and use of items.
	/// *
	/// </summary>
	/// <seealso cref="     CUP.lalr_item
	/// "/>
	/// <seealso cref="     CUP.lalr_item_set
	/// "/>
	/// <seealso cref="     CUP.lalr_state
	/// "/>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// </author>
	
	public class lr_item_core
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Full constructor.
		/// </summary>
		/// <param name="prod">production this item uses.
		/// </param>
		/// <param name="pos"> position of the "dot" within the item.
		/// 
		/// </param>
		public lr_item_core(production prod, int pos)
		{
			// symbol after_dot = null;
			production_part part;
			
			if (prod == null)
				throw new internal_error("Attempt to create an lr_item_core with a null production");
			
			_the_production = prod;
			
			if (pos < 0 || pos > _the_production.rhs_length())
				throw new internal_error("Attempt to create an lr_item_core with a bad dot position");
			
			_dot_pos = pos;
			
			/* compute and cache hash code now */
			_core_hash_cache = 13 * _the_production.GetHashCode() + pos;
			
			/* cache the symbol after the dot */
			if (_dot_pos < _the_production.rhs_length())
			{
				part = _the_production.rhs(_dot_pos);
				if (!part.is_action())
					_symbol_after_dot = ((symbol_part) part).the_symbol();
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor for dot at start of right hand side. 
		/// </summary>
		/// <param name="prod">production this item uses.
		/// 
		/// </param>
		public lr_item_core(production prod):this(prod, 0)
		{
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>The production for the item. 
		/// </summary>
		protected internal production _the_production;
		
		/// <summary>The production for the item. 
		/// </summary>
		public virtual production the_production()
		{
			return _the_production;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>The position of the "dot" -- this indicates the part of the production 
		/// that the marker is before, so 0 indicates a dot at the beginning of 
		/// the RHS.
		/// </summary>
		protected internal int _dot_pos;
		
		/// <summary>The position of the "dot" -- this indicates the part of the production 
		/// that the marker is before, so 0 indicates a dot at the beginning of 
		/// the RHS.
		/// </summary>
		public virtual int dot_pos()
		{
			return _dot_pos;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Cache of the hash code. 
		/// </summary>
		protected internal int _core_hash_cache;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Cache of symbol after the dot. 
		/// </summary>
		protected internal symbol _symbol_after_dot = null;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Is the dot at the end of the production? 
		/// </summary>
		public virtual bool dot_at_end()
		{
			return _dot_pos >= _the_production.rhs_length();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Return the symbol after the dot.  If there is no symbol after the dot
		/// we return null. 
		/// </summary>
		public virtual symbol symbol_after_dot()
		{
			/* use the cached symbol */
			return _symbol_after_dot;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Determine if we have a dot before a non terminal, and if so which one 
		/// (return null or the non terminal). 
		/// </summary>
		public virtual non_terminal dot_before_nt()
		{
			symbol sym;
			
			/* get the symbol after the dot */
			sym = symbol_after_dot();
			
			/* if it exists and is a non terminal, return it */
			if (sym != null && sym.is_non_term())
				return (non_terminal) sym;
			else
				return null;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Produce a new lr_item_core that results from shifting the dot one 
		/// position to the right. 
		/// </summary>
		public virtual lr_item_core shift_core()
		{
			if (dot_at_end())
				throw new internal_error("Attempt to shift past end of an lr_item_core");
			
			return new lr_item_core(_the_production, _dot_pos + 1);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality comparison for the core only.  This is separate out because we 
		/// need separate access in a super class. 
		/// </summary>
		public virtual bool core_equals(lr_item_core other)
		{
			return other != null && _the_production.equals(other._the_production) && _dot_pos == other._dot_pos;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality comparison. 
		/// </summary>
		public virtual bool equals(lr_item_core other)
		{
			return core_equals(other);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality comparison. 
		/// </summary>
		public  override bool Equals(object other)
		{
			if (!(other is lr_item_core))
				return false;
			else
				return equals((lr_item_core) other);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Hash code for the core (separated so we keep non overridden version). 
		/// </summary>
		public virtual int core_hashCode()
		{
			return _core_hash_cache;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Hash code for the item. 
		/// </summary>
		public override int GetHashCode()
		{
			return _core_hash_cache;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Return the hash code that object would have provided for us so we have 
		/// a (nearly) unique id for debugging.
		/// </summary>
		protected internal virtual int obj_hash()
		{
			return base.GetHashCode();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to a string (separated out from toString() so we can call it
		/// from subclass that overrides toString()).
		/// </summary>
		public virtual string to_simple_string()
		{
			System.String result;
			production_part part;
			
			if (_the_production.lhs() != null && _the_production.lhs().the_symbol() != null && _the_production.lhs().the_symbol().name_Renamed_Method() != null)
				result = _the_production.lhs().the_symbol().name_Renamed_Method();
			else
				result = "$$NULL$$";
			
			result += " ::= ";
			
			 for (int i = 0; i < _the_production.rhs_length(); i++)
			{
				/* do we need the dot before this one? */
				if (i == _dot_pos)
					result += "(*) ";
				
				/* print the name of the part */
				if (_the_production.rhs(i) == null)
				{
					result += "$$NULL$$ ";
				}
				else
				{
					part = _the_production.rhs(i);
					if (part == null)
						result += "$$NULL$$ ";
					else if (part.is_action())
						result += "{ACTION} ";
					else if (((symbol_part) part).the_symbol() != null && ((symbol_part) part).the_symbol().name_Renamed_Method() != null)
						result += ((symbol_part) part).the_symbol().name_Renamed_Method() + " ";
					else
						result += "$$NULL$$ ";
				}
			}
			
			/* put the dot after if needed */
			if (_dot_pos == _the_production.rhs_length())
				result += "(*) ";
			
			return result;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to a string 
		/// </summary>
		public override string ToString()
		{
			/* can't throw here since super class doesn't, so we crash instead */
			try
			{
				return to_simple_string();
			}
			catch (internal_error e)
			{
				e.crash();
				return null;
			}
		}
		
		/*-----------------------------------------------------------*/
	}
}