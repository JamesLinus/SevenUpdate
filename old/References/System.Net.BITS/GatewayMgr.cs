/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace System.Net.BITS
{
	/// <summary>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/igatewaymgr.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/igatewaymgr.asp")]
	public sealed class GatewayMgr
	{
		private Manager manager;
		private IGatewayMgr mgr;

		/// <summary>
		/// Constructor
		/// </summary>
		internal GatewayMgr(Manager manager)
		{
			this.manager = manager;
			
			try
			{
				if (manager.Version >= BITSVersion.V3_0)
					mgr = (IGatewayMgr)manager.BCM;
			}
			catch (InvalidCastException)
			{
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/igatewaymgr_getgateways.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/igatewaymgr_getgateways.asp")]
		public Gateway[] Gateways
		{
			get
			{
				if (mgr == null)
					throw new BITSUnsupportedException();

				Gateway[] gateways = null;

				try
				{
					List<Gateway> list = new List<Gateway>();
					IntPtr mem = IntPtr.Zero;

					// linked list of gateways, don't know the count until you walk the list
					mgr.GetGateways(out mem);
					
					while (mem != IntPtr.Zero)
					{
						IntPtr walk = mem;
						
						Utils.TaskFree(ref walk, delegate()
						{
							_BG_GATEWAY_INFO UpdateInfo;

							UpdateInfo = (_BG_GATEWAY_INFO)Marshal.PtrToStructure(mem, typeof(_BG_GATEWAY_INFO));
							list.Add(new Gateway(UpdateInfo));
							mem = UpdateInfo.Next;
						});
					}
					
					gateways = new Gateway[list.Count];
					list.CopyTo(gateways);
				}
				catch (Exception e)
				{
					manager.HandleException(e);
				}
				
				return gateways;
			}
		}		
	}
}
