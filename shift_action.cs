namespace CUP
{
	using System;
	
	/// <summary>This class represents a shift action within the parse table. 
	/// The action simply stores the state that it shifts to and responds 
	/// to queries about its type.
	/// *
	/// </summary>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	public class shift_action:parse_action
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Simple constructor. 
		/// </summary>
		/// <param name="shft_to">the state that this action shifts to.
		/// 
		/// </param>
		public shift_action(lalr_state shft_to)
		{
			/* sanity check */
			if (shft_to == null)
				throw new internal_error("Attempt to create a shift_action to a null state");
			
			_shift_to = shft_to;
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>The state we shift to. 
		/// </summary>
		protected lalr_state _shift_to;
		
		/// <summary>The state we shift to. 
		/// </summary>
		public virtual lalr_state shift_to()
		{
			return _shift_to;
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Quick access to type of action. 
		/// </summary>
		public override int kind()
		{
			return SHIFT;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality test. 
		/// </summary>
		public virtual bool equals(shift_action other)
		{
			return other != null && other.shift_to() == shift_to();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality test. 
		/// </summary>
		public  override bool Equals(System.Object other)
		{
			if (other is shift_action)
				return equals((shift_action) other);
			else
				return false;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Compute a hash code. 
		/// </summary>
		public override int GetHashCode()
		{
			/* use the hash code of the state we are shifting to */
			return shift_to().GetHashCode();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to a string. 
		/// </summary>
		public override System.String ToString()
		{
			return "SHIFT(to state " + shift_to().index() + ")";
		}
		
		/*-----------------------------------------------------------*/
	}
}