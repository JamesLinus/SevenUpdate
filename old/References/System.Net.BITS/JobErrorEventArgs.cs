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
	/// Used for the Manager.JobError event
	/// </summary>
	public sealed class JobErrorEventArgs : JobEventArgs
	{
		private JobError error;
		private bool cancel;
		private bool runCmdLine = true;

		/// <summary>
		/// Constructor
		/// </summary>
		internal JobErrorEventArgs(Job job, JobError error)
			: base(job)
		{
			this.error = error;
		}
		
		/// <summary>
		/// The error object
		/// </summary>
		public JobError Error { get { return error; } }
		/// <summary>
		/// To have Job.Cancel() called
		/// </summary>
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		/// <summary>
		/// Assuming a cmd line has been set in the job allow it to run.
		/// </summary>
		public bool RunCmdLine { get { return runCmdLine; } set { runCmdLine = value; } }
	}
}
