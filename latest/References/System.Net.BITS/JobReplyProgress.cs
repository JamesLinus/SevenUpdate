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
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_job_times.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_job_times.asp")]
	public sealed class JobReplyProgress
	{
		/// <summary>
		/// BytesTotal is BG_SIZE_UNKNOWN if the reply has not begun.
		/// </summary>
		public ulong BG_SIZE_UNKNOWN = ulong.MaxValue;
		
		/// <summary>
		/// Constructor
		/// </summary>
		internal JobReplyProgress()
		{
		}
		
		internal void CopyFrom(_BG_JOB_REPLY_PROGRESS rp)
		{
			BytesTotal = rp.BytesTotal;
			BytesTransferred = rp.BytesTransferred;
		}
		
		/// <summary>
		/// Size of the file in bytes. The value is BG_SIZE_UNKNOWN if the reply has not begun.
		/// </summary>
		[Description("Size of the file in bytes. The value is BG_SIZE_UNKNOWN if the reply has not begun.")]
		public ulong BytesTotal;
		
		/// <summary>
		/// Number of bytes transferred.
		/// </summary>
		[Description("Number of bytes transferred.")]
		public ulong BytesTransferred;
	}
}
