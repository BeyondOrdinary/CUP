namespace CUP
{
	using System;
	
	/// <summary>This class serves as the base class for entries in a parse action table.  
	/// Full entries will either be SHIFT(state_num), REDUCE(production), NONASSOC,
	/// or ERROR. Objects of this base class will default to ERROR, while
	/// the other three types will be represented by subclasses. 
	/// 
	/// </summary>
	/// <seealso cref="     CUP.reduce_action
	/// "/>
	/// <seealso cref="     CUP.shift_action
	/// "/>
	/// <version> last updated: 7/2/96
	/// </version>
	/// <author>  Frank Flannery
	/// 
	/// </author>
	
	public class parse_action
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Simple constructor. 
		/// </summary>
		public parse_action()
		{
			/* nothing to do in the base class */
		}
		
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Static (Class) Variables ------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Constant for action type -- error action. 
		/// </summary>
		public const int ERROR = 0;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constant for action type -- shift action. 
		/// </summary>
		public const int SHIFT = 1;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constants for action type -- reduce action. 
		/// </summary>
		public const int REDUCE = 2;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constants for action type -- reduce action. 
		/// </summary>
		public const int NONASSOC = 3;
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Quick access to the type -- base class defaults to error. 
		/// </summary>
		public virtual int kind()
		{
			return ERROR;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality test. 
		/// </summary>
		public virtual bool equals(parse_action other)
		{
			/* we match all error actions */
			return other != null && other.kind() == ERROR;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality test. 
		/// </summary>
		public  override bool Equals(object other)
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
			return 0xCafe123;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to string. 
		/// </summary>
		public override string ToString()
		{
			return "ERROR";
		}
		
		/*-----------------------------------------------------------*/
	}
}