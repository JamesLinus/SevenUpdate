/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace System.Net.BITS
{
	/// <summary>
	/// Mostly a wrapper around IBackgroundCopyFile with added code to support
	/// the designing of Job before the IBackgroundCopyFile is created.
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile.asp
	/// </summary>
	[
		TypeConverter(typeof(FileTypeConverter)),
		Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile.asp")
	]
	public class File : ICloneable, ISerializable
	{
		/// <summary>
		/// Our parent job
		/// </summary>
		private Job job;
		/// <summary>
		///
		/// </summary>
		private IBackgroundCopyFile bcf;
		/// <summary>
		/// User defined data
		/// </summary>
		private object tag = null;

		private string remote = string.Empty;
		private string local = string.Empty;
		private FileRange[] ranges = null;
		private JobFileProgress progress = new JobFileProgress();
		private string tempName = string.Empty;
		private bool valid;
		private bool fromPeer;
		
		/// <summary>
		/// Cached index in the IBackgroundCopyJob.IEnumBackgroundCopyFiles
		/// </summary>
		private int index = -1;

		/// <summary>
		/// Constructor
		/// </summary>
		public File()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="remote"></param>
		/// <param name="local"></param>
		public File(string remote, string local)
			: this(remote, local, null)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="remote"></param>
		/// <param name="local"></param>
		/// <param name="ranges"></param>
		public File(string remote, string local, FileRange[] ranges)
		{
			this.remote = remote != null ? remote : string.Empty;
			this.local = local != null ? local : string.Empty;
			this.ranges = ranges;
		}

		/// <summary>
		/// For exiting IBackgroundCopyFile's
		/// </summary>
		/// <param name="job"></param>
		/// <param name="bcf"></param>
		internal void Construct(Job job, IBackgroundCopyFile bcf)
		{
			this.job = job;

			if (index == -1)
				this.bcf = bcf;
			else
			{
				System.Diagnostics.Debug.Assert(this.bcf == null);
				this.bcf = job.Files.Find(this);
			}
		}
		
		internal void Close()
		{
			Utils.Release<IBackgroundCopyFile>(ref bcf, delegate() {});
			job = null;
		}

		/// <summary>
		/// Adding a file to a job is deferred until Job.Resume()
		/// </summary>
		/// <param name="bcf"></param>
		internal void Construct(IBackgroundCopyFile bcf)
		{
			this.bcf = bcf;
		}

		/// <summary>
		/// Determines whether two Object instances are equal.
		/// </summary>
		/// <param name="obj">obj can be a File</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			File other = obj as File;

			if (other != null)
			{
				return	LocalFileName == other.LocalFileName &&
						RemoteFileName == other.RemoteFileName;
			}

			return base.Equals(obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. GetHashCode is suitable for use in hashing algorithms and data structures like a hash table. 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format(	Properties.Resources.FileToStringFormat,
									RemoteFileName,
									LocalFileName);
		}

		internal IBackgroundCopyFile BCF { get { return bcf; } }
		internal int Index { get { return index; } set { index = value; } }
		
		/// <summary>
		/// 
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Job Parent { get { return job; } }

		/// <summary>
		/// 
		/// </summary>
		[
			TypeConverter(typeof(StringConverter)),
			Localizable(false),
			DefaultValue(null),
			Bindable(true),
			Category("Misc")
		]
		public object Tag { get { return tag; } set { tag = value; } }

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile_getremotename.asp,
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile2_setremotename.asp
		/// </summary>
		[
			DefaultValue(""),
			Localizable(true),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile_getremotename.asp, http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile2_setremotename.asp")
		]
		public string RemoteFileName
		{
			get
			{
				Get(delegate() { bcf.GetRemoteName(out remote); });
				return remote;
			}
			set
			{
				remote = value != null ? value : string.Empty;
				SetX<IBackgroundCopyFile2>(	delegate(IBackgroundCopyFile2 bcf2)
				{
					bcf2.SetRemoteName(remote);
				});
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile_getlocalname.asp
		/// </summary>
		[
			EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor)),
			DefaultValue(""),
			Localizable(true),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile_getlocalname.asp")
		]
		public string LocalFileName
		{
			get
			{
				Get(delegate() { bcf.GetLocalName(out local); });
				return local;
			}
			set
			{
				if (job != null && !job.CanRemoveFiles)
					throw new BITSPreResumeOnlyException(true);
					
				local = value != null ? value : string.Empty;
			}
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile_getprogress.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile_getprogress.asp")
		]
		public JobFileProgress FileProgress
		{
			get
			{
				Get(delegate()
				{
					_BG_FILE_PROGRESS p;
					bcf.GetProgress(out p);
					progress.CopyFrom(p);
				});
				
				return progress;
			}
		}

		internal bool ShouldSerializeRanges()
		{
			return ranges != null && ranges.Length > 0;
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile2_getfileranges.asp
		/// </summary>
		[
			DefaultValue(null),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile2_getfileranges.asp")
		]
		public FileRange[] Ranges
		{
			get
			{
				GetX<IBackgroundCopyFile2>(	delegate(IBackgroundCopyFile2 bcf2)
				{
					int count = 0;
					IntPtr mem;

					bcf2.GetFileRanges(ref count, out mem);
					Utils.Walk<_BG_FILE_RANGE, FileRange>(	ref mem,
															count,
															delegate(_BG_FILE_RANGE t) { return new FileRange(t); },
															out ranges);
				});
				
				return ranges;
			}
			set
			{
				if (job != null && !job.CanRemoveFiles)
					throw new BITSPreResumeOnlyException(true);

				ranges = value;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile3_gettemporaryname.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile3_gettemporaryname.asp")
		]
		public string TemporaryName
		{
			get
			{
				GetX<IBackgroundCopyFile3>(delegate(IBackgroundCopyFile3 bcf3)
				{
					bcf3.GetTemporaryName(out tempName);
				});
				
				return tempName;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile3_getvalidationstate.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyfile3_getvalidationstate.asp")
		]
		public bool ValidationState
		{
			get
			{
				GetX<IBackgroundCopyFile3>(delegate(IBackgroundCopyFile3 bcf3)
				{
					int v;
					bcf3.GetValidationState(out v);
					valid = v == 1;
				});
				
				return valid;
			}
			set
			{
				valid = value;
				SetX<IBackgroundCopyFile3>(delegate(IBackgroundCopyFile3 bcf3)
				{
					bcf3.SetValidationState(valid ? 1 : 0);
				});
			}
		}

		/// <summary>
		/// If any part of the file was downloaded from a peer server.
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("If any part of the file was downloaded from a peer server.")
		]
		public bool DownloadedFromPeer
		{
			get
			{
				GetX<IBackgroundCopyFile3>(delegate(IBackgroundCopyFile3 bcf3)
				{
					bcf3.DownloadedFromPeer(out fromPeer);
				});
				
				return fromPeer;
			}
		}

		private delegate void DoAction();

		/// <summary>
		/// generic IBackgroundCopyFile.GetXXX impl
		/// </summary>
		/// <param name="action"></param>
		private void Get(DoAction action)
		{
			Set(action);
		}

		/// <summary>
		/// generic IBackgroundCopyFile.SetXXX impl
		/// </summary>
		/// <param name="action"></param>
		private void Set(DoAction action)
		{
			try
			{
				if (bcf != null)
					action();
			}
			catch (Exception e)
			{
				job.HandleException(e);
			}
		}

		private delegate void DoActionX<T>(T bcfX) where T : IBackgroundCopyFile2;

		/// <summary>
		/// generic IBackgroundCopyFileX.GetXXX impl where X >= 2
		/// </summary>
		/// <typeparam name="T">IBackgroundCopyFile2 or 3</typeparam>
		/// <param name="action"></param>
		private void GetX<T>(DoActionX<T> action) where T : IBackgroundCopyFile2
		{
			SetX<T>(action);
		}

		/// <summary>
		/// generic IBackgroundCopyFileX.SetXXX impl where X >= 2
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		private void SetX<T>(DoActionX<T> action) where T : IBackgroundCopyFile2
		{
			if (bcf != null)
			{
				T bcfX;

				try
				{
					bcfX = (T)bcf;
				}
				catch
				{
					throw new BITSUnsupportedException();
				}

				try
				{
					action(bcfX);
				}
				catch (Exception e)
				{
					job.HandleException(e);
				}
			}
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~File()
		{
			Close();
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new File(remote, local, ranges);
		}

		#endregion

		#region ISerializable Members

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="UpdateInfo"></param>
		/// <param name="context"></param>
		public File(SerializationInfo UpdateInfo, StreamingContext context) 
		{
			remote = (string)UpdateInfo.GetValue("RemoteFileName", typeof(string));
			local = (string)UpdateInfo.GetValue("LocalFileName", typeof(string));
			ranges = (FileRange[])UpdateInfo.GetValue("Ranges", typeof(FileRange[]));
		}

		/// <summary>
		/// Populates a SerializationInfo with the data needed to serialize the target object.
		/// </summary>
		/// <param name="UpdateInfo"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo UpdateInfo, StreamingContext context)
		{
			UpdateInfo.AddValue("RemoteFileName", remote);
			UpdateInfo.AddValue("LocalFileName", local);
			UpdateInfo.AddValue("Ranges", ranges);
		}

		#endregion
	}
	
	internal class FileTypeConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
		{
			if (destType == typeof(InstanceDescriptor))
				return true;

			return base.CanConvertTo(context, destType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
		{
			if (destType == typeof(InstanceDescriptor) && value is File)
			{
				File file = (File)value;
				MemberInfo mi;
				object[] args;
			
				if (file.ShouldSerializeRanges())
				{
					mi = typeof(File).GetConstructor(new Type[]
														{	
															typeof(string),
															typeof(string),
															typeof(FileRange[])
														});
					args = new object[] {
											file.RemoteFileName,
											file.LocalFileName,
											file.Ranges
										};

				}
				else
				{
					mi = typeof(File).GetConstructor(new Type[]
														{	
															typeof(string),
															typeof(string)
														});
					args = new object[] { file.RemoteFileName, file.LocalFileName };
				}

				return new InstanceDescriptor(mi, args);
			}

			return base.ConvertTo(context, culture, value, destType);
		}
	}
}
