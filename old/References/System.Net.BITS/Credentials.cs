/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.Security;
using System.ComponentModel;

namespace System.Net.BITS
{
	/// <summary>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_setcredentials.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_setcredentials.asp")]
	public class Credentials
	{
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_auth_target.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_auth_target.asp")]
		public AuthTarget Target = AuthTarget.Proxy;

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_auth_scheme.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_auth_scheme.asp")]
		public AuthScheme Scheme = AuthScheme.Basic;

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_basic_credentials.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_basic_credentials.asp")]
		public SecureString UserName = new SecureString();

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_basic_credentials.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_basic_credentials.asp")]
		public SecureString Password = new SecureString();
		
		/// <summary>
		/// Determines whether two Object instances are equal.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is Credentials)
			{
				Credentials other = (Credentials)obj;
				
				return	Target == other.Target &&
						Scheme == other.Scheme &&
						(UserName != null ? UserName.Equals(other.UserName) : other.UserName == null) &&
						(Password != null ? Password.Equals(other.Password) : other.Password == null);
			}
			
			return base.Equals(obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. GetHashCode is suitable for use in hashing algorithms and data structures like a hash table. 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return GetHashCode(Target, Scheme);
		}

		internal void CopyTo(out _BG_AUTH_CREDENTIALS cred)
		{
			cred.Target = Target;
			cred.Scheme = Scheme;
			cred.Credentials.Basic.UserName = UserName != null ? UserName.ToString() : null;
			cred.Credentials.Basic.Password = Password != null ? Password.ToString() : null;
		}

		internal static int GetHashCode(AuthTarget Target, AuthScheme Scheme)
		{
			return ((int)Target << 16) + (int)Scheme;
		}
	}
}
