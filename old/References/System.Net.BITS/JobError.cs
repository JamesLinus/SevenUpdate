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
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror.asp")]
	public sealed class JobError
	{
		/// <summary>
		/// Constructor
		/// </summary>
		internal JobError()
		{
		}
		
		internal void CopyFrom(Job job, IBackgroundCopyError error)
		{
			IBackgroundCopyFile bcf = null;

			error.GetFile(out bcf);
			Utils.Release<IBackgroundCopyFile>(ref bcf, delegate()
			{	// find already existing File instance
				File = job.Files.Find(bcf);
			});
			error.GetError(out Context, out HResult);
			error.GetErrorContextDescription(0, out ContextDescription);
			ContextDescription = ContextDescription.TrimEnd(new char[] { '\r', '\n' });
			error.GetErrorDescription(0, out Description);
			Description = Description.TrimEnd(new char[] { '\r', '\n' });
			error.GetProtocol(out Protocol);
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_getfile.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_getfile.asp")]
		public File File;

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_geterror.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_geterror.asp")]
		public ErrorContext Context;

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_geterror.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_geterror.asp")]
		public uint HResult;

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_geterrorcontextdescription.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_geterrorcontextdescription.asp")]
		public string ContextDescription;

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_geterrordescription.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_geterrordescription.asp")]
		public string Description;

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_getprotocol.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyerror_getprotocol.asp")]
		public string Protocol;
	}
}
