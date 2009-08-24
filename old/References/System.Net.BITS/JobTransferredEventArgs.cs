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
	/// Used for Manager.OnTransferred event
	/// </summary>
	public sealed class JobTransferredEventArgs : JobEventArgs
	{
		/// <summary>
		/// Constructor
		/// </summary>
		internal JobTransferredEventArgs(Job job)
			: base(job)
		{
		}
		
		/// <summary>
		/// To have Job.Complete() called
		/// </summary>
		public bool Complete = true;
		/// <summary>
		/// Assuming a cmd line has been set in the job allow it to run.
		/// </summary>
		public bool RunCmdLine = true;
	}
}
