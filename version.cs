namespace CUP
{
	using System;
	
	/// <summary>This class contains version and authorship information. 
	/// It contains only static data elements and basically just a central 
	/// place to put this kind of information so it can be updated easily
	/// for each release.  
	/// *
	/// Version numbers used here are broken into 3 parts: major, minor, and 
	/// update, and are written as v<major>.<minor>.<update> (e.g. v0.10a).  
	/// Major numbers will change at the time of major reworking of some 
	/// part of the system.  Minor numbers for each public release or 
	/// change big enough to cause incompatibilities.  Finally update
	/// letter will be incremented for small bug fixes and changes that
	/// probably wouldn't be noticed by a user.  
	/// *
	/// </summary>
	/// <version> last updated: 12/22/97 [CSA]
	/// </version>
	/// <author>  Frank Flannery
	/// 
	/// </author>
	
	public class version
	{
		/// <summary>The major version number. 
		/// </summary>
		public const int major = 0;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>The minor version number. 
		/// </summary>
		public const int minor = 10;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>The update letter. 
		/// </summary>
		public const char update = 'k';
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>String for the current version. 
		/// </summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'version_str '. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1003"'
		public static readonly string version_str = "v" + major + "." + minor + update;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Full title of the system 
		/// </summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'title_str '. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1003"'
		public static readonly string title_str = "CUP " + version_str;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Name of the author 
		/// </summary>
		public const string author_str = "Scott E. Hudson, Frank Flannery, and C. Scott Ananian";
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>The command name normally used to invoke this program 
		/// </summary>
		public const string program_name = "java_cup";
	}
}