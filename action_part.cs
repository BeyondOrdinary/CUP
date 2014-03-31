namespace CUP
{
	using System;
	
	/// 
	/// <summary> This class represents a part of a production which contains an
	/// action.  These are eventually eliminated from productions and converted
	/// to trailing actions by factoring out with a production that derives the
	/// empty string (and ends with this action).
	/// *
	/// </summary>
	/// <seealso cref=" CUP.production
	/// "/>
	/// <version> last update: 11/25/95
	/// </version>
	/// <author> Scott Hudson
	/// 
	/// </author>
	
	public class action_part:production_part
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructors ------------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Simple constructor. 
		/// </summary>
		/// <param name="code_str">string containing the actual user code.
		/// 
		/// </param>
		public action_part(string code_str):base(null)
		{
			_code_string = code_str;
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>String containing code for the action in question. 
		/// </summary>
		protected string _code_string;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>String containing code for the action in question. 
		/// </summary>
		public virtual string code_string()
		{
			return _code_string;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Set the code string. 
		/// </summary>
		public virtual void  set_code_string(string new_str)
		{
			_code_string = new_str;
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Override to report this object as an action. 
		/// </summary>
		public override bool is_action()
		{
			return true;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Equality comparison for properly typed object. 
		/// </summary>
		public virtual bool equals(action_part other)
		{
			/* compare the strings */
			return other != null && base.equals(other) && other.code_string().Equals(code_string());
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Generic equality comparison. 
		/// </summary>
		public  override bool Equals(object other)
		{
			if (!(other is action_part))
				return false;
			else
				return equals((action_part) other);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Produce a hash code. 
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (code_string() == null?0:code_string().GetHashCode());
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Convert to a string.  
		/// </summary>
		public override string ToString()
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
			return base.ToString() + "{" + code_string() + "}";
		}
		
		/*-----------------------------------------------------------*/
	}
}