namespace CUP
{
	using System;
	
	/// <summary>A specialized version of a production used when we split an existing
	/// production in order to remove an embedded action.  Here we keep a bit 
	/// of extra bookkeeping so that we know where we came from.
	/// </summary>
	/// <version> last updated: 11/25/95
	/// </version>
	/// <author>  Scott Hudson
	/// 
	/// </author>
	
	public class action_production:production
	{
		
		/// <summary>Constructor.
		/// </summary>
		/// <param name="base">      the production we are being factored out of.
		/// </param>
		/// <param name="lhs_sym">   the LHS symbol for this production.
		/// </param>
		/// <param name="rhs_parts"> array of production parts for the RHS.
		/// </param>
		/// <param name="rhs_len">   how much of the rhs_parts array is valid.
		/// </param>
		/// <param name="action_str">the trailing reduce action for this production.
		/// 
		/// </param>
		public action_production(production base_Renamed, non_terminal lhs_sym, production_part[] rhs_parts, int rhs_len, System.String action_str):base(lhs_sym, rhs_parts, rhs_len, action_str)
		{
			_base_production = base_Renamed;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>The production we were taken out of. 
		/// </summary>
		protected production _base_production;
		
		/// <summary>The production we were taken out of. 
		/// </summary>
		public virtual production base_production()
		{
			return _base_production;
		}
	}
}