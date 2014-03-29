namespace CUP
{
	using System;
	
	/// <summary>This class represents one part (either a symbol or an action) of a 
	/// production.  In this base class it contains only an optional label 
	/// string that the user can use to refer to the part within actions.<p>
	/// *
	/// This is an abstract class.
	/// *
	/// </summary>
	/// <seealso cref="     CUP.production
	/// "/>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	public abstract class production_part
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Simple constructor. 
		/// </summary>
		public production_part(System.String lab)
		{
			_label = lab;
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Optional label for referring to the part within an action (null for 
		/// no label). 
		/// </summary>
		protected System.String _label;
		
		/// <summary>Optional label for referring to the part within an action (null for 
		/// no label). 
		/// </summary>
		public virtual System.String label()
		{
			return _label;
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Indicate if this is an action (rather than a symbol).  Here in the 
		/// base class, we don't this know yet, so its an abstract method.
		/// </summary>
		public abstract bool is_action();
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality comparison. 
		/// </summary>
		public virtual bool equals(production_part other)
		{
			if (other == null)
				return false;
			
			/* compare the labels */
			if (label() != null)
				return label().Equals(other.label());
			else
				return other.label() == null;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality comparison. 
		/// </summary>
		public  override bool Equals(System.Object other)
		{
			if (!(other is production_part))
				return false;
			else
				return equals((production_part) other);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Produce a hash code. 
		/// </summary>
		public override int GetHashCode()
		{
			return label() == null?0:label().GetHashCode();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to a string. 
		/// </summary>
		public override System.String ToString()
		{
			if (label() != null)
				return label() + ":";
			else
				return " ";
		}
		
		/*-----------------------------------------------------------*/
	}
}