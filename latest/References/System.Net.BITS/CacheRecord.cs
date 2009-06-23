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
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_cache_record_info.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_cache_record_info.asp")]
	public sealed class CacheRecord
	{
		/// <summary>
		/// Constructor
		/// </summary>
		internal CacheRecord(IBackgroundCopyCacheRecord ccr)
		{
			ccr.GetOriginUrl(out OriginUrl);

			_BG_CACHE_RECORD_INFO UpdateInfo;
			
			ccr.GetInformation(out UpdateInfo);
			Id = UpdateInfo.Id;
			CreationTime = Utils.ToDateTime(UpdateInfo.CreationTime);
			ModificationTime = Utils.ToDateTime(UpdateInfo.ModificationTime);
			AccessTime = Utils.ToDateTime(UpdateInfo.AccessTime);
			FileModificationTime = Utils.ToDateTime(UpdateInfo.FileModificationTime);
			FileSize = UpdateInfo.FileSize;
			FileEtag = UpdateInfo.FileEtag;
			FileValidated = UpdateInfo.FileValidated;

			IntPtr mem = IntPtr.Zero;
			int count;

			ccr.GetFileRanges(out count, out mem);
			Utils.Walk<_BG_FILE_RANGE, FileRange>(	ref mem,
													count,
													delegate(_BG_FILE_RANGE t) { return new FileRange(t); },
													out Ranges);
		}
		
		/// <summary>
		/// Identifier that uniquely identifies the cache record in the cache.
		/// </summary>
		[Description("Identifier that uniquely identifies the cache record in the cache.")]
		public readonly Guid Id;
		
		/// <summary>
		/// Date and time that the record was created. The time is specified as FILETIME.
		/// </summary>
		[Description("Date and time that the record was created.")]
		public readonly DateTime CreationTime;
		
		/// <summary>
		/// Date and time that the record was last modified. The time is specified as FILETIME.
		/// </summary>
		[Description("Date and time that the record was last modified.")]
		public readonly DateTime ModificationTime;
		
		/// <summary>
		/// Date and time that the file was last accessed.
		/// </summary>
		[Description("Date and time that the file was last accessed.")]
		public readonly DateTime AccessTime;
		
		/// <summary>
		/// Date and time that the file was last modified on the server.
		/// </summary>
		[Description("Date and time that the file was last modified on the server.")]
		public readonly DateTime FileModificationTime;
		
		/// <summary>
		/// Size of the file, in bytes.
		/// </summary>
		[Description("Size of the file, in bytes.")]
		public readonly ulong FileSize;
		
		/// <summary>
		/// Entity tag that the server generates to identify the file.
		/// </summary>
		[Description("Entity tag that the server generates to identify the file.")]
		public readonly string FileEtag;
		
		/// <summary>
		/// TRUE if the application has validated the file, otherwise, FALSE. The file is available to serve after you validate the file. To validate the file, call the IBackgroundCopyFile3::SetValidationState method. The file is implicitly validated if the application calls IBackgroundCopyJob::Complete without calling SetValidationState.
		/// </summary>
		[Description("TRUE if the application has validated the file, otherwise, FALSE. The file is available to serve after you validate the file. To validate the file, call the IBackgroundCopyFile3::SetValidationState method. The file is implicitly validated if the application calls IBackgroundCopyJob::Complete without calling SetValidationState.")]
		public readonly bool FileValidated;
		
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		public readonly string OriginUrl;
		
		/// <summary>
		/// 
		/// </summary>
		[Description("")]
		public readonly FileRange[] Ranges;
	}
}
