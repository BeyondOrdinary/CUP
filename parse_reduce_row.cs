namespace CUP
{
	using System;
	
	/// <summary>This class represents one row (corresponding to one machine state) of the 
	/// reduce-goto parse table. 
	/// </summary>
	public class parse_reduce_row
	{
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Simple constructor. Note: this should not be used until the number
		/// of terminals in the grammar has been established.
		/// </summary>
		public parse_reduce_row()
		{
			/* make sure the size is set */
			if (_size <= 0)
				_size = non_terminal.number();
			
			/* allocate the array */
			under_non_term = new lalr_state[size()];
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Static (Class) Variables ------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Number of columns (non terminals) in every row. 
		/// </summary>
		protected static int _size = 0;
		
		/// <summary>Number of columns (non terminals) in every row. 
		/// </summary>
		public static int size()
		{
			return _size;
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Actual entries for the row. 
		/// </summary>
		public lalr_state[] under_non_term;
	}
}