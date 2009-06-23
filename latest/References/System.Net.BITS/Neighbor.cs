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
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighbor.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighbor.asp")]
	public sealed class Neighbor
	{
		/// <summary>
		/// Constructor
		/// </summary>
		internal Neighbor(IBackgroundCopyNeighbor cn)
		{
			cn.GetPrincipalName(out PrincipalName);
			cn.IsAuthenticated(out Authenticated);
			cn.IsAvailable(out Available);
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighbor_getprincipalname.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighbor_getprincipalname.asp")]
		public readonly string PrincipalName;

		/// <summary>
		/// 
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighbor_isauthenticated.asp")]
		public readonly bool Authenticated;

		/// <summary>
		/// 
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighbor_isavailable.asp")]
		public readonly bool Available;
	}
}
