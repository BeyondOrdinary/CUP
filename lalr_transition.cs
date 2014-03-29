namespace CUP
{
	using System;
	
	/// <summary>This class represents a transition in an LALR viable prefix recognition 
	/// machine.  Transitions can be under terminals for non-terminals.  They are
	/// internally linked together into singly linked lists containing all the 
	/// transitions out of a single state via the _next field.
	/// *
	/// </summary>
	/// <seealso cref="     CUP.lalr_state
	/// "/>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// *
	/// 
	/// </author>
	public class lalr_transition
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Full constructor.
		/// </summary>
		/// <param name="on_sym"> symbol we are transitioning on.
		/// </param>
		/// <param name="to_st">  state we transition to.
		/// </param>
		/// <param name="nxt">    next transition in linked list.
		/// 
		/// </param>
		public lalr_transition(symbol on_sym, lalr_state to_st, lalr_transition nxt)
		{
			/* sanity checks */
			if (on_sym == null)
				throw new internal_error("Attempt to create transition on null symbol");
			if (to_st == null)
				throw new internal_error("Attempt to create transition to null state");
			
			/* initialize */
			_on_symbol = on_sym;
			_to_state = to_st;
			_next = nxt;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Constructor with null next. 
		/// </summary>
		/// <param name="on_sym"> symbol we are transitioning on.
		/// </param>
		/// <param name="to_st">  state we transition to.
		/// 
		/// </param>
		public lalr_transition(symbol on_sym, lalr_state to_st):this(on_sym, to_st, null)
		{
		}
		
		/*-----------------------------------------------------------*/
		/*--- (Access to) Instance Variables ------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>The symbol we make the transition on. 
		/// </summary>
		protected symbol _on_symbol;
		
		/// <summary>The symbol we make the transition on. 
		/// </summary>
		public virtual symbol on_symbol()
		{
			return _on_symbol;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>The state we transition to. 
		/// </summary>
		protected lalr_state _to_state;
		
		/// <summary>The state we transition to. 
		/// </summary>
		public virtual lalr_state to_state()
		{
			return _to_state;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Next transition in linked list of transitions out of a state 
		/// </summary>
		protected lalr_transition _next;
		
		/// <summary>Next transition in linked list of transitions out of a state 
		/// </summary>
		public virtual lalr_transition next()
		{
			return _next;
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Convert to a string. 
		/// </summary>
		public override System.String ToString()
		{
			System.String result;
			
			result = "transition on " + on_symbol().name_Renamed_Method() + " to state [";
			result += _to_state.index();
			result += "]";
			
			return result;
		}
		
		/*-----------------------------------------------------------*/
	}
}