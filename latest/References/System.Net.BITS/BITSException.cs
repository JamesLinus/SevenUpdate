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
	/// Base class for any exception thrown by BITS
	/// </summary>
	public abstract class BITSExceptionBase : Exception
	{
		internal BITSExceptionBase(string error)
			: base(error)
		{
		}

		internal BITSExceptionBase(string error, Exception inner)
			: base(error, inner)
		{
		}
	}
	
	/// <summary>
	/// All exceptions are caught and a BITSException is rethrown
	/// The actual exception, if any, is in InnerException
	/// </summary>
	public sealed class BITSException : BITSExceptionBase
	{
		internal BITSException(string error)
			: base(error)
		{
		}
		
		internal BITSException(string error, Exception inner)
			: base(error, inner)
		{
		}
	}
	
	/// <summary>
	/// Thrown when trying to use functionality not available in the
	/// version of BITS installed
	/// </summary>
	public sealed class BITSUnsupportedException : BITSExceptionBase
	{
		internal BITSUnsupportedException()
			: base(Properties.Resources.Unsupported)
		{
		}
	}
	
	/// <summary>
	/// Certain properties in the Job and File objects can not be changed
	/// after the Job.Resume has been called
	/// </summary>
	public sealed class BITSPreResumeOnlyException : BITSExceptionBase
	{
		internal BITSPreResumeOnlyException(bool property)
			: base(property
					? Properties.Resources.PreResumeProperty
					: Properties.Resources.PreResumeMethod)
		{
		}
	}
}
