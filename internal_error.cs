namespace CUP
{
	using System;
	
	/// <summary>Exception subclass for reporting internal errors in JavaCup. 
	/// </summary>
	public class internal_error:System.Exception
	{
		/// <summary>Constructor with a message 
		/// </summary>
		public internal_error(string msg):base(msg)
		{
		}
		
		/// <summary>Method called to do a forced error exit on an internal error
		/// for cases when we can't actually throw the exception.  
		/// </summary>
		public virtual void  crash()
		{
			System.Console.Error.WriteLine("JavaCUP Fatal Internal Error Detected");
			System.Console.Error.WriteLine(Message);
			SupportClass.WriteStackTrace(this, Console.Error);
			System.Environment.Exit(- 1);
		}
	}
}