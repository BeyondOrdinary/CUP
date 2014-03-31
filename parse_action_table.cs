namespace CUP
{
	using System;
	
	/// <summary>This class represents the complete "action" table of the parser. 
	/// It has one row for each state in the parse machine, and a column for
	/// each terminal symbol.  Each entry in the table represents a shift,
	/// reduce, or an error.  
	/// *
	/// </summary>
	/// <seealso cref="     CUP.parse_action
	/// "/>
	/// <seealso cref="     CUP.parse_action_row
	/// "/>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	public class parse_action_table
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Simple constructor.  All terminals, non-terminals, and productions must 
		/// already have been entered, and the viable prefix recognizer should
		/// have been constructed before this is called.
		/// </summary>
		public parse_action_table()
		{
			/* determine how many states we are working with */
			_num_states = lalr_state.number();
			
			/* allocate the array and fill it in with empty rows */
			under_state = new parse_action_row[_num_states];
			 for (int i = 0; i < _num_states; i++)
				under_state[i] = new parse_action_row();
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>How many rows/states are in the machine/table. 
		/// </summary>
		protected int _num_states;
		
		/// <summary>How many rows/states are in the machine/table. 
		/// </summary>
		public virtual int num_states()
		{
			return _num_states;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Actual array of rows, one per state. 
		/// </summary>
		public parse_action_row[] under_state;
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Check the table to ensure that all productions have been reduced. 
		/// Issue a warning message (to System.err) for each production that
		/// is never reduced.
		/// </summary>
		public virtual void  check_reductions()
		{
			parse_action act;
			production prod;
			
			/* tabulate reductions -- look at every table entry */
			 for (int row = 0; row < num_states(); row++)
			{
				 for (int col = 0; col < CUP.parse_action_row.size(); col++)
				{
					/* look at the action entry to see if its a reduce */
					act = under_state[row].under_term[col];
					if (act != null && act.kind() == parse_action.REDUCE)
					{
						/* tell production that we used it */
						((reduce_action) act).reduce_with().note_reduction_use();
					}
				}
			}
			
			/* now go across every production and make sure we hit it */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator p = production.all(); p.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				prod = (production) p.Current;
				
				/* if we didn't hit it give a warning */
				if (prod.num_reductions() == 0)
				{
					/* count it *
					emit.not_reduced++;
					
					/* give a warning if they haven't been turned off */
					if (!emit.nowarn)
					{
						System.Console.Error.WriteLine("*** Production \"" + prod.to_simple_string() + "\" never reduced");
						lexer.warning_count++;
					}
				}
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*
		
		/** Convert to a string. */
		public override string ToString()
		{
			System.String result;
			int cnt;
			
			result = "-------- ACTION_TABLE --------\n";
			 for (int row = 0; row < num_states(); row++)
			{
				result += "From state #" + row + "\n";
				cnt = 0;
				 for (int col = 0; col < CUP.parse_action_row.size(); col++)
				{
					/* if the action is not an error print it */
					if (under_state[row].under_term[col].kind() != parse_action.ERROR)
					{
						result += " [term " + col + ":" + under_state[row].under_term[col] + "]";
						
						/* end the line after the 2nd one */
						cnt++;
						if (cnt == 2)
						{
							result += "\n";
							cnt = 0;
						}
					}
				}
				/* finish the line if we haven't just done that */
				if (cnt != 0)
					result += "\n";
			}
			result += "------------------------------";
			
			return result;
		}
		
		/*-----------------------------------------------------------*/
	}
}