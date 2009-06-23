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
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_job_progress.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_job_progress.asp")]
	public sealed class JobProgress
	{
		/// <summary>
		/// Constructor
		/// </summary>
		internal JobProgress()
		{
		}
		
		internal void CopyFrom(_BG_JOB_PROGRESS p)
		{
			BytesTotal = p.BytesTotal;
			BytesTransferred = p.BytesTransferred;
			FilesTotal = p.FilesTotal;
			FilesTransferred = p.FilesTransferred;
		}
		
		/// <summary>
		/// Total number of bytes to transfer for the job.
		/// </summary>
		public ulong BytesTotal;

		/// <summary>
		/// Number of bytes transferred
		/// </summary>
		public ulong BytesTransferred;

		/// <summary>
		/// Total number of files to transfer for this job
		/// </summary>
		public uint FilesTotal;

		/// <summary>
		/// Number of files transferred. 
		/// </summary>
		public uint FilesTransferred;
	}
}
