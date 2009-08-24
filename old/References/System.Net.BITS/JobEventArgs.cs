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
	/// Base class for Job related event args
	/// </summary>
	public abstract class JobEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor
		/// </summary>
		internal JobEventArgs(Job job)
		{
			this.Job = job;
		}
		
		/// <summary>
		/// The job that the event is for.
		/// </summary>
		[Description("The job that the event is for.")]
		public readonly Job Job;
	}
}
