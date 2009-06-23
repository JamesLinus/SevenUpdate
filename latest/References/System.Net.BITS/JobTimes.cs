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
	public sealed class JobTimes
	{
		/// <summary>
		/// Constructor
		/// </summary>
		internal JobTimes()
		{
		}
		
		internal void CopyFrom(_BG_JOB_TIMES t)
		{
			CreationTime = Utils.ToDateTime(t.CreationTime);
			ModificationTime = Utils.ToDateTime(t.ModificationTime);
			TransferCompletionTime = Utils.ToDateTime(t.TransferCompletionTime);
		}

		/// <summary>
		/// Time the job was created.
		/// </summary>
		public DateTime CreationTime;

		/// <summary>
		/// Time the job was last modified or bytes were transferred. Adding files or calling any of the set methods in the IBackgroundCopyJob* interfaces changes this value. In addition, changes to the state of the job and calling the Suspend, Resume, Cancel, and Complete methods change this value.
		/// </summary>
		public DateTime ModificationTime;

		/// <summary>
		/// Time the job entered the BG_JOB_STATE_TRANSFERRED state.
		/// </summary>
		public DateTime TransferCompletionTime;
	}
}
