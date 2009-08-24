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
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighborhood.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighborhood.asp")]
	public sealed class Neighborhood
	{
		private IBackgroundCopyNeighborhood bcn;
		private Manager manager;

		/// <summary>
		/// Constructor
		/// </summary>
		internal Neighborhood(Manager manager)
		{
			this.manager = manager;
			
			try
			{
				if (manager.Version >= BITSVersion.V3_0)
					bcn = (IBackgroundCopyNeighborhood)manager.BCM;
			}
			catch (InvalidCastException)
			{
			}
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighborhood_enumneighbors.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighborhood_enumneighbors.asp")]
		public Neighbor[] Neighbors
		{
			get
			{
				if (bcn == null)
					throw new BITSUnsupportedException();
				
				IEnumBackgroundCopyNeighbors bcns = null;
				Neighbor[] records = null;
				
				bcn.EnumNeighbors(out bcns);
				Utils.Release<IEnumBackgroundCopyNeighbors>(ref bcns, delegate()
				{
					uint count = 0;

					bcns.GetCount(out count);
					records = new Neighbor[count];

					for (int i = 0; i < count; i++)
					{
						IBackgroundCopyNeighbor cn = null;
						uint fetched;

						bcns.Next(1, out cn, out fetched);
						Utils.Release<IBackgroundCopyNeighbor>(ref cn, delegate()
						{
							records[i] = new Neighbor(cn);
						});
					}
				});

				return records;
			}
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighborhood_clearneighbors.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighborhood_clearneighbors.asp")]
		public void ClearNeighbors()
		{
			if (bcn == null)
				throw new BITSUnsupportedException();

			try
			{
				bcn.ClearNeighbors();
			}
			catch (Exception e)
			{
				manager.HandleException(e);
			}
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighborhood_discoverneighbors.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighborhood_discoverneighbors.asp")]
		public void DiscoverNeighbors()
		{
			if (bcn == null)
				throw new BITSUnsupportedException();
				
			try
			{
				bcn.DiscoverNeighbors();
			}
			catch (Exception e)
			{
				manager.HandleException(e);
			}
		}
	}
}
