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
	/// Event args for the Manager.FileTransferred event
	/// </summary>
	public sealed class FileTransferredEventArgs : JobEventArgs
	{
		private File file;
		private bool validate = true;
		private bool isValid = true;

		/// <summary>
		/// Constructor
		/// </summary>
		internal FileTransferredEventArgs(Job job, IBackgroundCopyFile bcf)
			: base(job)
		{
			this.file = job.Files.Find(bcf);
		}
		
		/// <summary>
		/// The file that was transferred
		/// </summary>
		public File File { get { return file; } }
		/// <summary>
		/// Set the validation state for this file
		/// </summary>
		public bool Validate { get { return validate; } set { validate = value; } }
		/// <summary>
		/// The validation state to set
		/// </summary>
		public bool IsValid { get { return isValid; } set { isValid = value; } }
	}
}
