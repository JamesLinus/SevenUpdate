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
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache.asp")]
	public sealed class Cache
	{
		private IBackgroundCopyCache bcc;
		private Manager manager;

		/// <summary>
		/// Constructor
		/// </summary>
		internal Cache(Manager manager)
		{
			this.manager = manager;
			
			try
			{
				if (manager.Version >= BITSVersion.V3_0)
					bcc = (IBackgroundCopyCache)manager.BCM;
			}
			catch (InvalidCastException)
			{
			}
		}
		
		/// <summary>
		/// The default cache location.
		/// </summary>
		public const string DefaultCacheLocation = @"%AllUsersProfile%\Microsoft\Network\Downloader\Cache";
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getcachelocation.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getcachelocation.asp")]
		public string CacheLocation
		{
			get
			{
				return Get<string>(delegate(out string d) { bcc.GetCacheLocation(out d); });
			}
			set
			{
				Set(delegate() { bcc.SetCacheLocation(value != null ? value : string.Empty); });
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getcachelimitasp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getcachelimit.asp")]
		public ulong CacheLimit
		{
			get
			{
				return Get<ulong>(delegate(out ulong d) { bcc.GetCacheLimit(out d); });
			}
			set
			{
				Set(delegate() { bcc.SetCacheLimit(value); });
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getcacheexpirationtime.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getcacheexpirationtime.asp")]
		public ulong CacheExpirationTime
		{
			get
			{
				return Get<ulong>(delegate(out ulong d) { bcc.GetCacheExpirationTime(out d); });
			}
			set
			{
				Set(delegate() { bcc.SetCacheExpirationTime(value); });
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getallowcaching.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getallowcaching.asp")]
		public bool AllowCaching
		{
			get
			{
				return Get<bool>(delegate(out bool d) { bcc.GetAllowCaching(out d); });
			}
			set
			{
				Set(delegate() { bcc.SetAllowCaching(value); });
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getusepeercache.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getusepeercache.asp")]
		public bool UsePeerCache
		{
			get
			{
				return Get<bool>(delegate(out bool d) { bcc.GetUsePeerCache(out d); });
			}
			set
			{
				Set(delegate() { bcc.SetUsePeerCache(value); });
			}
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_enumrecords.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_enumrecords.asp")]
		public CacheRecord[] Records
		{
			get
			{
				return Get<CacheRecord[]>(delegate(out CacheRecord[] records)
				{
					IEnumBackgroundCopyCacheRecords ccrs = null;
					CacheRecord[] temp = null;

					bcc.EnumRecords(out ccrs);
					
					Utils.Release<IEnumBackgroundCopyCacheRecords>(ref ccrs, delegate()
					{
						uint count = 0;
						uint fetched;
						
						ccrs.GetCount(out count);
						temp = new CacheRecord[count];
						
						for (int i = 0; i < count; i++)
						{
							IBackgroundCopyCacheRecord ccr = null;

							ccrs.Next(1, out ccr, out fetched);
							System.Diagnostics.Debug.Assert(fetched == 1);
							Utils.Release<IBackgroundCopyCacheRecord>(ref ccr, delegate()
							{
								temp[i] = new CacheRecord(ccr);
							});
						}
					});

					records = temp;
				});
			}
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getrecord.asp
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_getrecord.asp")]
		public CacheRecord GetRecord(Guid id)
		{
			return Get<CacheRecord>(delegate(out CacheRecord record)
			{
				IBackgroundCopyCacheRecord ccr = null;
				CacheRecord temp = null;

				bcc.GetRecord(ref id, out ccr);
				Utils.Release<IBackgroundCopyCacheRecord>(ref ccr, delegate()
				{
					temp = new CacheRecord(ccr);
				});
				
				record = temp;
			});
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_clearrecords.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_clearrecords.asp")]
		public void ClearRecords()
		{
			Method(delegate() { bcc.ClearRecords(); });
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_deleterecord.asp
		/// </summary>
		/// <param name="id"></param>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_deleterecord.asp")]
		public void DeleteRecord(Guid id)
		{
			Method(delegate() { bcc.DeleteRecord(ref id); });
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_deleteurl.asp
		/// </summary>
		/// <param name="url"></param>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache_deleteurl.asp")]
		public void DeleteUrl(string url)
		{
			Method(delegate() { bcc.DeleteUrl(url); });
		}

		private delegate void DoAction<Ret>(out Ret r);

		/// <summary>
		/// generic IBackgroundCopyCache.GetXXX impl
		/// </summary>
		/// <typeparam name="Ret"></typeparam>
		/// <param name="action"></param>
		private Ret Get<Ret>(DoAction<Ret> action)
		{
			Ret r = default(Ret);

			try
			{
				action(out r);
			}
			catch (Exception e)
			{
				manager.HandleException(e);
			}

			return r;
		}

		private delegate void DoAction();

		/// <summary>
		/// generic IBackgroundCopyCache.SetXXX impl
		/// </summary>
		/// <param name="action"></param>
		private void Set(DoAction action)
		{
			if (bcc == null)
				throw new BITSUnsupportedException();

			try
			{
				action();
			}
			catch (Exception e)
			{
				manager.HandleException(e);
			}
		}
		
		/// <summary>
		/// generic IBackgroundCopyCache method call impl
		/// </summary>
		/// <param name="action"></param>
		private void Method(DoAction action)
		{
			Set(action);
		}
	}
}
