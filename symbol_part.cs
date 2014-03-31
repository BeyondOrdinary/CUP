namespace CUP
{
	using System;
	
	/// <summary>This class represents a part of a production which is a symbol (terminal
	/// or non terminal).  This simply maintains a reference to the symbol in 
	/// question.
	/// *
	/// </summary>
	/// <seealso cref="     CUP.production
	/// "/>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	public class symbol_part:production_part
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Full constructor. 
		/// </summary>
		/// <param name="sym">the symbol that this part is made up of.
		/// </param>
		/// <param name="lab">an optional label string for the part.
		/// 
		/// </param>
		public symbol_part(symbol sym, string lab):base(lab)
		{
			
			if (sym == null)
				throw new internal_error("Attempt to construct a symbol_part with a null symbol");
			_the_symbol = sym;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor with no label. 
		/// </summary>
		/// <param name="sym">the symbol that this part is made up of.
		/// 
		/// </param>
		public symbol_part(symbol sym):this(sym, null)
		{
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>The symbol that this part is made up of. 
		/// </summary>
		protected symbol _the_symbol;
		
		/// <summary>The symbol that this part is made up of. 
		/// </summary>
		public virtual symbol the_symbol()
		{
			return _the_symbol;
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Respond that we are not an action part. 
		/// </summary>
		public override bool is_action()
		{
			return false;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality comparison. 
		/// </summary>
		public virtual bool equals(symbol_part other)
		{
			return other != null && base.equals(other) && the_symbol().Equals(other.the_symbol());
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality comparison. 
		/// </summary>
		public  override bool Equals(object other)
		{
			if (!(other is symbol_part))
				return false;
			else
				return equals((symbol_part) other);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Produce a hash code. 
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (the_symbol() == null?0:the_symbol().GetHashCode());
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to a string. 
		/// </summary>
		public override string ToString()
		{
			if (the_symbol() != null)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
				return base.ToString() + the_symbol();
			}
			else
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
				return base.ToString() + "$$MISSING-SYMBOL$$";
			}
		}
		
		/*-----------------------------------------------------------*/
	}
}