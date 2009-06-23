/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.ComponentModel;

namespace System.Net.BITS
{
	/// <summary>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_file_progress.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_file_progress.asp")]
	public sealed class JobFileProgress
	{
		/// <summary>
		/// Constructor
		/// </summary>
		internal JobFileProgress()
		{
		}
		
		internal void CopyFrom(_BG_FILE_PROGRESS p)
		{
			BytesTotal = p.BytesTotal;
			BytesTransferred = p.BytesTransferred;
			Completed = p.Completed == 1 ? true : false;
		}
		
		/// <summary>
		/// Size of the file in bytes. If the value is BG_SIZE_UNKNOWN, the total size of the file has not been determined.
		/// BITS does not set this value if it cannot determine the size of the file.
		/// For example, if the specified file or server does not exist, BITS cannot determine the size of the file.
		/// If you are downloading ranges from a file, BytesTotal reflects the total number of bytes you want to download from the file.
		/// </summary>
		public ulong BytesTotal;

		/// <summary>
		/// Number of bytes transferred. 
		/// </summary>
		public ulong BytesTransferred;

		/// <summary>
		/// For downloads, the value is TRUE if the file is available to the user; otherwise, the value is FALSE.
		/// Files are available to the user after calling the IBackgroundCopyJob::Complete method.
		/// If the Complete method generates a transient error, those files processed before the error occurred are available to the user; the others are not.
		/// Use the Completed member to determine if the file is available to the user when Complete fails.
		/// For uploads, the value is TRUE when the file upload is complete; otherwise, the value is FALSE.
		/// </summary>
		public bool Completed;
	}
}
