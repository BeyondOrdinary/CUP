namespace CUP
{
	using System;
	
	/// <summary>This class represents a reduce action within the parse table. 
	/// The action simply stores the production that it reduces with and 
	/// responds to queries about its type.
	/// *
	/// </summary>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	public class reduce_action:parse_action
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Simple constructor. 
		/// </summary>
		/// <param name="prod">the production this action reduces with.
		/// 
		/// </param>
		public reduce_action(production prod)
		{
			/* sanity check */
			if (prod == null)
				throw new internal_error("Attempt to create a reduce_action with a null production");
			
			_reduce_with = prod;
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>The production we reduce with. 
		/// </summary>
		protected production _reduce_with;
		
		/// <summary>The production we reduce with. 
		/// </summary>
		public virtual production reduce_with()
		{
			return _reduce_with;
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Quick access to type of action. 
		/// </summary>
		public override int kind()
		{
			return REDUCE;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality test. 
		/// </summary>
		public virtual bool equals(reduce_action other)
		{
			return other != null && other.reduce_with() == reduce_with();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality test. 
		/// </summary>
		public  override bool Equals(System.Object other)
		{
			if (other is reduce_action)
				return equals((reduce_action) other);
			else
				return false;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Compute a hash code. 
		/// </summary>
		public override int GetHashCode()
		{
			/* use the hash code of the production we are reducing with */
			return reduce_with().GetHashCode();
		}
		
		
		/// <summary>Convert to string. 
		/// </summary>
		public override System.String ToString()
		{
			return "REDUCE(with prod " + reduce_with().index() + ")";
		}
		
		/*-----------------------------------------------------------*/
	}
}