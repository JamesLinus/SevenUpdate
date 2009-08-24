/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.Collections;
using System.ComponentModel;

namespace System.Net.BITS
{
	/// <summary>
	/// A collection of Files. Files can be added or removed but not after the IBackgroundCopyJob is created.
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ienumbackgroundcopyfiles.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ienumbackgroundcopyfiles.asp")]
	public sealed class FileCollection : CollectionBase
	{
		private Job job;
		private IBackgroundCopyFile insertBCF;
		private bool deferAddFiles;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="job"></param>
		internal FileCollection(Job job)
		{
			this.job = job;
		}

		/// <summary>
		/// Add the existing IBackgroundCopyFile's if any
		/// </summary>
		internal void AddExisting()
		{	
			IEnumBackgroundCopyFiles files = null;

			job.BCJ.EnumFiles(out files);
			Utils.Release(ref files, delegate()
			{
				for (;;)
				{
					uint fetched;

					files.Next(1, out insertBCF, out fetched);
				
					if (fetched == 0)
						break;
					
					List.Add(new File());
				}
			});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		public int Add(File file)
		{
			return List.Add(file);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteName"></param>
		/// <param name="localName"></param>
		public int Add(string remoteName, string localName)
		{
			return Add(new File(remoteName, localName));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteName"></param>
		/// <param name="localName"></param>
		/// <param name="ranges"></param>
		public int Add(string remoteName, string localName, FileRange[] ranges)
		{
			return Add(new File(remoteName, localName, ranges));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="files"></param>
		public void AddRange(File[] files)
		{
			deferAddFiles = true;
			
			try
			{
				foreach (File file in files)
					Add(file);
					
				job.AddFiles();
			}
			finally
			{
				deferAddFiles = false;
			}
		}

		/// <summary>
		/// The collection allows files to be removed even though the IBackgroundCopyJob
		/// does not. But only before the IBackgroundCopyJob is created. Returns true if it's
		/// safe to call Remove, Clear, or RemoveAt.
		/// </summary>
		public bool CanRemove { get { return job.CanRemoveFiles; } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		public void Remove(File file)
		{
			List.Remove(file);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="files"></param>
		/// <param name="index"></param>
		public void CopyTo(File[] files, int index)
		{
			List.CopyTo(files, index);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public File this[int i]
		{
			get { return (File)List[i]; }
			set { List[i] = value; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public bool Contains(File file)
		{
			return List.Contains(file);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteName"></param>
		/// <param name="localName"></param>
		/// <returns></returns>
		public bool Contains(string remoteName, string localName)
		{
			foreach (File file in List)
			{
				if (remoteName == file.RemoteFileName && localName == file.LocalFileName)
					return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public int IndexOf(File file)
		{
			return List.IndexOf(file);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteName"></param>
		/// <param name="localName"></param>
		/// <returns></returns>
		public int IndexOf(string remoteName, string localName)
		{
			for (int i = 0; i < List.Count; i++)
			{
				File file = this[i];
				
				if (remoteName == file.RemoteFileName && localName == file.LocalFileName)
					return i;
			}

			return -1;
		}

		/// <summary>
		/// given a File find the IBackgroundCopyFile in the IEnumBackgroundCopyFiles
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		internal IBackgroundCopyFile Find(File file)
		{
			IEnumBackgroundCopyFiles files = null;
			IBackgroundCopyFile bcf = null;

			job.BCJ.EnumFiles(out files);
			Utils.Release<IEnumBackgroundCopyFiles>(ref files, delegate()
			{
				uint fetched;

				files.Skip((uint)file.Index);
				files.Next(1, out bcf, out fetched);
				System.Diagnostics.Debug.Assert(fetched == 1);
			});
			
			return bcf;
		}
		
		/// <summary>
		/// given a IBackgroundCopyFile find the File
		/// </summary>
		/// <param name="bcf"></param>
		/// <returns></returns>
		internal File Find(IBackgroundCopyFile bcf)
		{	
			string r, l;
			
			bcf.GetRemoteName(out r);
			bcf.GetLocalName(out l);
			
			foreach (File file in List)
			{
				if (r == file.RemoteFileName && l == file.LocalFileName)
					return file;
			}
			
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		protected override void OnValidate(object value)
		{
			if (!(value is File))
				throw new ArgumentException();

			base.OnValidate(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsert(int index, object value)
		{
			CheckInsert((File)value);
			base.OnInsert(index, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsertComplete(int index, object value)
		{
			SetupFile((File)value);

			if (!deferAddFiles)
				job.AddFiles();
			
			base.OnInsertComplete(index, value);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			CheckInsert((File)newValue);
			CheckRemove();
			base.OnSet(index, oldValue, newValue);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			((File)oldValue).Close();
			SetupFile((File)newValue);
			base.OnSetComplete(index, oldValue, newValue);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnRemove(int index, object value)
		{
			CheckRemove();
			((File)value).Close();
			base.OnRemove(index, value);
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnClear()
		{
			CheckRemove();

			foreach (File file in List)
				file.Close();

			base.OnClear();
		}

		/// We need to make the remote and locale pair unique in the list
		/// it's the only way we can match an IBackgroundCopyFile to it's File object. We
		/// care about matching so JobError can init its File member to the correct File instance
		/// which matters so you can use the Tag property set in the File object
		private void CheckInsert(File file)
		{
			if (file.Parent != null)
				throw new BITSException(Properties.Resources.FileInCollection);

			if (List.Contains(file))
				throw new BITSException(string.Format(Properties.Resources.FileUniqueFormat, file));
		}

		private void SetupFile(File file)
		{
			Utils.Release<IBackgroundCopyFile>(ref insertBCF, delegate()
			{
				file.Construct(job, insertBCF);
				insertBCF = null; // do no release this interface unless an exception is thrown
			});
		}

		private void CheckRemove()
		{
			if (!CanRemove)
				throw new BITSPreResumeOnlyException(false);
		}
	}
}
