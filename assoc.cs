namespace CUP
{
	using System;
	
	/* Defines integers that represent the associativity of terminals
	* @version last updated: 7/3/96
	* @author  Frank Flannery
	*/
	
	public class assoc
	{
		
		/* various associativities, no_prec being the default value */
		public const int left = 0;
		public const int right = 1;
		public const int nonassoc = 2;
		public static int no_prec = - 1;
	}
}