/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;

namespace System.Net.BITS
{
	/// <summary>
	/// Used for Manager.OnModification event
	/// </summary>
	public sealed class JobModificationEventArgs : JobEventArgs
	{
		/// <summary>
		/// Constructor
		/// </summary>
		internal JobModificationEventArgs(Job job)
			: base(job)
		{
			if (job.State == JobState.TransientError)
				Error = job.Error;
		}
		
		/// <summary>
		/// To have Job.Cancel() called
		/// </summary>
		public bool Cancel = false;
		/// <summary>
		/// The job error when JobState is TransientError
		/// </summary>
		public readonly JobError Error;
	}
}
