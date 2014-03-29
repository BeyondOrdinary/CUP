namespace CUP
{
	using System;
	
	/// <summary>This class represents a shift/reduce nonassociative error within the 
	/// parse table.  If action_table element is assign to type
	/// nonassoc_action, it cannot be changed, and signifies that there 
	/// is a conflict between shifting and reducing a production and a
	/// terminal that shouldn't be next to each other.
	/// *
	/// </summary>
	/// <version> last updated: 7/2/96
	/// </version>
	/// <author>  Frank Flannery
	/// 
	/// </author>
	public class nonassoc_action:parse_action
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Simple constructor. 
		/// </summary>
		public nonassoc_action()
		{
			/* don't need to set anything, since it signifies error */
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Quick access to type of action. 
		/// </summary>
		public override int kind()
		{
			return NONASSOC;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality test. 
		/// </summary>
		public override bool equals(parse_action other)
		{
			return other != null && other.kind() == NONASSOC;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality test. 
		/// </summary>
		public  override bool Equals(System.Object other)
		{
			if (other is parse_action)
				return equals((parse_action) other);
			else
				return false;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Compute a hash code. 
		/// </summary>
		public override int GetHashCode()
		{
			/* all objects of this class hash together */
			return 0xCafe321;
		}
		
		
		
		/// <summary>Convert to string. 
		/// </summary>
		public override System.String ToString()
		{
			return "NONASSOC";
		}
		
		/*-----------------------------------------------------------*/
	}
}