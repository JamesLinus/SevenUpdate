using System;
using System.Collections.Generic;
using System.Text;

namespace WynEdge.Net.BITS
{
	public class FileInfo
	{
		public FileInfo(string RemoteName, string LocalName)
		{
			this.RemoteName = RemoteName;
			this.LocalName = LocalName;
		}
		
		public string RemoteName;
		public string LocalName;

		internal void CopyTo(out _BG_FILE_INFO p)
		{
			p.RemoteName = RemoteName;
			p.LocalName = LocalName;
		}
	}
}
