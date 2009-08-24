/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Threading;

namespace System.Net.BITS
{
	/// <summary>
	/// Mostly a wrapper around IBackgroundCopyJob with added code to support
	/// the designing of Job before the IBackgroundCopyJob is created
	/// </summary>
	[
		DefaultProperty("Files"),
		TypeConverter(typeof(JobTypeConverter)),
		DesignTimeVisible(false),
		ToolboxItem(false)
	]
	public partial class Job : Component, ICloneable
	{
		/// <summary>
		/// events come in on several threads
		/// </summary>
		private Mutex mutex = new Mutex();
		/// <summary>
		/// we talk to the manager in plenty of places
		/// </summary>
		private Manager manager;
		/// <summary>
		/// The job interface
		/// </summary>
		private IBackgroundCopyJob bcj;
		/// <summary>
		/// The Id of this job
		/// </summary>
		private Guid guid = Guid.Empty;
		/// <summary>
		/// The collection of files
		/// </summary>
		private FileCollection files;
		/// <summary>
		/// user defined data
		/// </summary>
		private object tag = null;
		/// <summary>
		/// Will this Job fire Manager.JobError, Manager.JobTransferred, etc... events
		/// </summary>
		private bool eventsEnabled = true;
		/// <summary>
		/// automatically cancel job on JobError event
		/// </summary>
		private bool autoCancelOnError = true;
		/// <summary>
		/// automatically cancel job on JobModification event when state is TransientError
		/// </summary>
		private bool autoCancelOnTransientError = true;
		/// <summary>
		/// automatically complete job on JobTransferred event
		/// </summary>
		private bool autoComplete = true;
		/// <summary>
		/// automatically validate file on FileTransferred event
		/// </summary>
		private bool autoValidate = true;
		/// <summary>
		/// if a command line has been set allow it to run after the JobError and
		/// JobTransferred events
		/// </summary>
		private bool autoRunCmdLine = true;
		
		private string name = string.Empty;
		private string desc = string.Empty;
		private JobError je = new JobError();
		private JobType type = JobType.Download;
		private JobPriority priority = JobPriority.Normal;
		private int minimumRetryDelay = 600;
		private int noProgressTimeout = 1209600;
		private JobProxySettings proxy = new JobProxySettings();
		private Dictionary<int, Credentials> cred = new Dictionary<int, Credentials>();
		private NotifyCmdLine cmdLine = new NotifyCmdLine();
		private JobState state = JobState.Inactive;
		private JobProgress progress = new JobProgress();
		private string owner = string.Empty;
		private uint errorCount = 0;
		private JobTimes times = new JobTimes();
		private JobReplyProgress replyProgress = new JobReplyProgress();
		private byte[] replyData;
		private string replyFileName = string.Empty;
		private CopyFileFlags aclFlags = CopyFileFlags.None;
		private string ch = string.Empty;
		private SSL ssl = SSL.None;
		private HttpRedirectPolicy hrp = HttpRedirectPolicy.Allowsilent;
		private bool hhr = false;
		private bool ac = false;
		private bool upc = false;
		private ulong oil = 0;
		private bool oes = false;
		private int mdt = 54000;
		private ClientCertificate cc = new ClientCertificate();

		/// <summary>
		/// Constructor
		/// </summary>
		public Job()
			: this(string.Empty, JobType.Download)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="container"></param>
		public Job(IContainer container)
			: this(string.Empty, JobType.Download)
		{
			container.Add(this);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		public Job(string name)
			: this(name, JobType.Download)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public Job(string name, JobType type)
			: this(name, type, JobPriority.Normal)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="priority"></param>
		public Job(string name, JobType type, JobPriority priority)
		{
			this.name = name;
			this.type = type;
			this.priority = priority;
			this.files = new FileCollection(this);
			InitializeComponent();
		}

		/// <summary>
		/// Builds the Job from the Guid if it existed already otherwise build it from scratch
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="bcj"></param>
		/// <param name="guid"></param>
		internal void Construct(Manager manager, IBackgroundCopyJob bcj, Guid guid)
		{
			this.manager = manager;

			if (this.guid != Guid.Empty)
			{
				System.Diagnostics.Debug.Assert(this.bcj == null);
				Construct();
			}
			else
			{
				this.bcj = bcj;
				this.guid = guid;
				
				if (bcj != null)
				{
					try
					{					
						files.AddExisting();
						EventPolicy();
					}
					catch (Exception e)
					{
						HandleException(e);
					}
				}
			}
		}

		private void EventPolicy()
		{
			if (bcj == null)
				return;

			if (!eventsEnabled)
			{
				bcj.SetNotifyInterface(null);
				bcj.SetNotifyFlags(	NotificationType.JobError |
									NotificationType.JobTransferred);
			}
			else
			{
				bcj.SetNotifyInterface(new BackgroundCopyCallback(this));
				bcj.SetNotifyFlags(manager.Version >= BITSVersion.V3_0
										? NotificationType.All_3_0
										: NotificationType.All);
			}
		}
		
		/// <summary>
		/// Actually create the job. Job creation and complete object init is deferred
		/// until Manager.EndInit()
		/// </summary>
		private void Construct()
		{			
			if (bcj != null)
				return;

			try
			{
				if (guid == Guid.Empty)
					manager.BCM.CreateJob(name, type, out guid, out bcj);
				else
				{
					manager.BCM.GetJob(ref guid, out bcj);
					
					foreach (File file in files)
						file.Construct(this, null);
				}

				bcj.SetDescription(desc);
				bcj.SetPriority(priority);
				bcj.SetMinimumRetryDelay(minimumRetryDelay);
				bcj.SetNoProgressTimeout(noProgressTimeout);
				
				if (proxy.ProxyUsage != ProxyUsage.Preconfig)
					ProxySettings = proxy;

				if (manager.Version >= BITSVersion.V1_5)
				{
					foreach (KeyValuePair<int, Credentials> entry in cred)
						SetCredentials(entry.Value);

					if (cmdLine.Program != string.Empty)
						NotifyCmdLine = cmdLine;
				}

				if (manager.Version >= BITSVersion.V2_0)
				{
					if (aclFlags != CopyFileFlags.None)
						FileACLFlags = aclFlags;
					
					if (type == JobType.UploadReply && replyFileName != string.Empty)
						ReplyFileName = replyFileName;
				}
				
				if (manager.Version >= BITSVersion.V2_5)
				{
					if (ch != string.Empty)
						CustomHeaders = ch;
						
					if (ssl != SSL.None)
						SSLFlags = ssl;
				}

				if (manager.Version >= BITSVersion.V3_0)
				{
					if (hrp != HttpRedirectPolicy.Allowsilent)
						HttpRedirectPolicy = hrp;
						
					if (hhr != false)
						EnableHttpsToHttpRedirect = hhr;
						
					if (ac != false)
						AllowCaching = ac;
						
					if (upc != false)
						UsePeerCache = upc;
					
					if (mdt != 54000)
						MaxDownloadTime = mdt;
						
					if (cc.StoreLocation != CertStoreLocation.None)
						ClientCertificate = cc;
				}

				EventPolicy();
			}
			catch (Exception e)
			{
				HandleException(e);
			}
		}
		
		internal void Close()
		{
			MutexProtect(delegate()
			{
				try
				{
					foreach (File file in files)
						file.Close();

					Set(delegate()
					{
						JobState state = State;

						if (state != JobState.Acknowledged && state != JobState.Cancelled)
						{
							bcj.SetNotifyFlags(NotificationType.Disable);
							bcj.SetNotifyInterface(null);
						}
					});
				}
				finally
				{
					Utils.Release<IBackgroundCopyJob>(ref bcj, delegate() {});
					manager = null;
				}
			});
		}
		
		internal void HandleException(Exception e)
		{
			manager.HandleException(e);
		}

		/// <summary>
		/// Determines whether two Object instances are equal.
		/// </summary>
		/// <param name="obj">obj can be a Job or a Guid</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
				
			Guid guid = Guid.Empty;
			
			if (obj is Job)
				guid = ((Job)obj).Id;

			if (guid == Guid.Empty && obj is Guid)
				guid = (Guid)obj;

			if (guid != Guid.Empty)
				return Guid.Equals(guid, Id);
			
			return base.Equals(obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. GetHashCode is suitable for use in hashing algorithms and data structures like a hash table. 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(Properties.Resources.JobToStringFormat, DisplayName, Id);
		}
		
		/// <summary>
		/// 
		/// </summary>
		internal IBackgroundCopyJob BCJ { get { return bcj; } }
		
		/// <summary>
		/// parent object
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Manager Parent { get { return manager; } }

		/// <summary>
		/// user defined tag
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
		/// The list of files to download the file to upload
		/// </summary>
		[
			TypeConverter(typeof(CollectionConverter)),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
			Category("Behavior")
		]
		public FileCollection Files { get { return files; } }
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getid.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getid.asp")
		]
		public Guid Id { get { return guid; } }

		/// <summary>
		/// Will this Job fire Manager.JobError, Manager.JobTransferred, etc... events
		/// </summary>
		[
			DefaultValue(true),
			Category("Event Handling"),
			Description("Will this Job fire Manager.JobError, Manager.JobTransferred, etc... events")
		]
		public bool EventsEnabled
		{
			get { return eventsEnabled; }
			set
			{
				MutexProtect(delegate()
				{
					if (eventsEnabled != value)
					{
						eventsEnabled = value;
						EventPolicy();
					}
				});
			}
		}
		
		/// <summary>
		/// Automatically cancel job on JobError event
		/// </summary>
		[
			DefaultValue(true),
			Category("Event Handling"),
			Description("Automatically cancel job on JobError event")
		]
		public bool AutoCancelOnError
		{
			get { return autoCancelOnError; }
			set { MutexProtect(delegate() { autoCancelOnError = value; }); }
		}

		/// <summary>
		/// Automatically cancel job on JobModification event when state is TransientError
		/// </summary>
		[
			DefaultValue(true),
			Category("Event Handling"),
			Description("Automatically cancel job on JobModification event when state is TransientError")
		]
		public bool AutoCancelOnTransientError
		{
			get { return autoCancelOnTransientError; }
			set { MutexProtect(delegate() { autoCancelOnTransientError = value; }); }
		}

		/// <summary>
		/// Automatically complete job on JobTransferred event
		/// </summary>
		[
			DefaultValue(true),
			Category("Event Handling"),
			Description("Automatically complete job on JobTransferred event")
		]
		public bool AutoComplete
		{
			get { return autoComplete; }
			set { MutexProtect(delegate() { autoComplete = value; }); }
		}

		/// <summary>
		/// Automatically validate file on FileTransferred event
		/// </summary>
		[
			DefaultValue(true),
			Category("Event Handling"),
			Description("Automatically validate file on FileTransferred event")
		]
		public bool AutoValidateFile
		{
			get { return autoValidate; }
			set { MutexProtect(delegate() { autoValidate = value; }); }
		}

		/// <summary>
		/// If a command line has been set allow it to run after the JobError and
		/// JobTransferred events
		/// </summary>
		[
			DefaultValue(true),
			Category("Event Handling"),
			Description("If a command line has been set allow it to run after the JobError and JobTransferred events")
		]
		private bool AutoRunCommandLine
		{
			get { return autoRunCmdLine; }
			set { MutexProtect(delegate() { autoRunCmdLine = value; }); }
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getdisplayname.asp
		/// </summary>
		[
			DefaultValue(""),
			Localizable(true),
			Bindable(true),
			Category("Misc"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getdisplayname.asp")
		]
		public string DisplayName
		{
			get
			{
				Get(delegate() { bcj.GetDisplayName(out name); });
				return name;
			}
			set
			{
				name = value != null ? value : string.Empty;
				Set(delegate() { bcj.SetDisplayName(name); });
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getdescription.asp
		/// </summary>
		[
			DefaultValue(""),
			Localizable(true),
			Bindable(true),
			Category("Misc"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getdescription.asp")
		]
		public string Description
		{
			get
			{
				Get(delegate() { bcj.GetDescription(out desc); });
				return desc;
			}
			set
			{
				desc = value != null ? value : string.Empty;
				Set(delegate() { bcj.SetDescription(desc); });
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_geterror.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_geterror.asp")
		]
		public JobError Error
		{
			get
			{				
				Get(delegate()
				{
					IBackgroundCopyError e = null;

					bcj.GetError(out e);
					Utils.Release<IBackgroundCopyError>(ref e, delegate()
					{
						je.CopyFrom(this, e);
					});
				});	
							
				return je;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getpriority.asp
		/// </summary>
		[
			DefaultValue(JobPriority.Normal),
			Bindable(true),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getpriority.asp")
		]
		public JobPriority Priority
		{
			get
			{
				Get(delegate() { bcj.GetPriority(out priority); });
				return priority;
			}
			set
			{
				priority = value;
				Set(delegate() { bcj.SetPriority(priority); });
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getminimumretrydelay.asp
		/// </summary>
		[
			DefaultValue(600),
			Bindable(true),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getminimumretrydelay.asp")
		]
		public int MinimumRetryDelay
		{
			get
			{
				Get(delegate() { bcj.GetMinimumRetryDelay(out minimumRetryDelay); });
				return minimumRetryDelay;
			}
			set
			{
				minimumRetryDelay = value > 0 ? value : 0;
				Set(delegate() { bcj.SetMinimumRetryDelay(minimumRetryDelay); });
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getnoprogresstimeout.asp
		/// </summary>
		[
			DefaultValue(1209600),
			Bindable(true),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getnoprogresstimeout.asp")
		]
		public int NoProgressTimeout
		{
			get
			{
				Get(delegate() { bcj.GetNoProgressTimeout(out noProgressTimeout); });
				return noProgressTimeout;
			}
			set
			{
				noProgressTimeout = value > 0 ? value : 0;
				Set(delegate() { bcj.SetNoProgressTimeout(noProgressTimeout); });
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getstate.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getstate.asp")
		]
		public JobState State
		{
			get
			{
				state = JobState.Inactive;
				Get(delegate() { bcj.GetState(out state); });
				return state;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_gettype.asp
		/// </summary>
		[
			DefaultValue(JobType.Download),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_gettype.asp")
		]
		public JobType Type
		{
			get
			{
				Get(delegate() { bcj.GetType(out type); });
				return type;
			}
			set
			{
				if (!CanRemoveFiles)
					throw new BITSPreResumeOnlyException(true);
					
				type = value;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getowner.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getowner.asp")
		]
		public string Owner
		{
			get
			{
				Get(delegate() { bcj.GetOwner(out owner); owner = SID.GetName(owner); });
				return owner;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getprogress.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getprogress.asp")
		]
		public JobProgress Progress
		{
			get
			{
				Get(delegate()
				{
					_BG_JOB_PROGRESS p;
					bcj.GetProgress(out p);
					progress.CopyFrom(p);
				});
				return progress;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_gettimes.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_gettimes.asp")
		]
		public JobTimes Times
		{
			get
			{
				Get(delegate()
				{
					_BG_JOB_TIMES t;
					bcj.GetTimes(out t);
					times.CopyFrom(t);
				});
				return times;
				}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_geterrorcount.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_geterrorcount.asp")
		]
		public uint ErrorCount
		{
			get
			{
				Get(delegate() { bcj.GetErrorCount(out errorCount); });
				return errorCount;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getproxysettings.asp
		/// </summary>
		[
			Category("Behavior"),
			DefaultValue(typeof(JobProxySettings), "Preconfig, \"\", \"\""),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getproxysettings.asp")
		]
		public JobProxySettings ProxySettings
		{
			get
			{
				Get(delegate()
				{
					string pl, pbl;
					ProxyUsage pu;

					bcj.GetProxySettings(out pu, out pl, out pbl);
					proxy.ProxyUsage = pu;
					proxy.ProxyList = pl;
					proxy.ProxyBypassList = pbl;
				});
				return proxy;
			}
			set
			{
				proxy.ProxyUsage = value != null ? value.ProxyUsage : ProxyUsage.Preconfig;
				proxy.ProxyList = value != null ? value.ProxyList : string.Empty;
				proxy.ProxyBypassList = value != null ? value.ProxyBypassList : string.Empty;				
				Set(delegate()
				{
					bcj.SetProxySettings(	proxy.ProxyUsage,
											proxy.ArgProxyList,
											proxy.ArgProxyBypassList);
				});
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_getnotifycmdline.asp
		/// </summary>
		[
			Category("Behavior"),
			DefaultValue(typeof(NotifyCmdLine), "\"\", \"\""),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_getnotifycmdline.asp")
		]
		public NotifyCmdLine NotifyCmdLine
		{
			get
			{
				GetX<IBackgroundCopyJob2>(delegate(IBackgroundCopyJob2 bcj2)
				{
					string pr, pa;
					
					bcj2.GetNotifyCmdLine(out pr, out pa);
					cmdLine.Program = pr;
					cmdLine.Parameters = pa;
				});
				return cmdLine;
			}
			set
			{
				cmdLine.Program = value != null ? value.Program : string.Empty;
				cmdLine.Parameters = value != null ? value.Parameters : string.Empty;
				SetX<IBackgroundCopyJob2>(delegate(IBackgroundCopyJob2 bcj2)
				{
					bcj2.SetNotifyCmdLine(cmdLine.ArgProgram, cmdLine.ArgParam);
				});
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_getreplyprogress.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_getreplyprogress.asp")
		]
		public JobReplyProgress ReplyProgress
		{
			get
			{
				GetX<IBackgroundCopyJob2>(delegate(IBackgroundCopyJob2 bcj2)
				{
					_BG_JOB_REPLY_PROGRESS jrp;
					bcj2.GetReplyProgress(out jrp);
					replyProgress.CopyFrom(jrp);
				});
				return replyProgress;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_getreplydata.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_getreplydata.asp")
		]
		public byte[] ReplyData
		{
			get
			{
				GetX<IBackgroundCopyJob2>( delegate(IBackgroundCopyJob2 bcj2)
				{
					IntPtr rd = IntPtr.Zero;
					ulong len;
												
					Utils.TaskFree(ref rd, delegate()
					{
						bcj2.GetReplyData(ref rd, out len);							
						replyData = new byte[len];
						Marshal.Copy(rd, replyData, 0, (int)len);
					});
				});
				
				return replyData;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_getreplyfilename.asp
		/// </summary>
		[
			EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor)),
			DefaultValue(""),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_getreplyfilename.asp")
		]
		public string ReplyFileName
		{
			get
			{
				if (type == JobType.UploadReply)
				{
					GetX<IBackgroundCopyJob2>(delegate(IBackgroundCopyJob2 bcj2)
					{
						bcj2.GetReplyFileName(out replyFileName);
					});
				}
				
				return replyFileName;
			}
			set
			{
				replyFileName = value != null ? value : string.Empty;
				SetX<IBackgroundCopyJob2>(	delegate(IBackgroundCopyJob2 bcj2)
				{
					bcj2.SetReplyFileName(replyFileName);
				});
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob3_getfileaclflags.asp
		/// </summary>
		[
			DefaultValue(CopyFileFlags.None),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob3_getfileaclflags.asp")
		]
		public CopyFileFlags FileACLFlags
		{
			get
			{
				GetX<IBackgroundCopyJob3>(
					delegate(IBackgroundCopyJob3 bcj3)
					{
						bcj3.GetFileACLFlags(out aclFlags);
					});
				return aclFlags;
			}
			set
			{
				aclFlags = value;
				SetX<IBackgroundCopyJob3>(	delegate(IBackgroundCopyJob3 bcj3)
											{
												bcj3.SetFileACLFlags(aclFlags);
											});
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob4_getcustomheaders.asp
		/// </summary>
		[
			DefaultValue(""),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob4_getcustomheaders.asp")
		]
		public string CustomHeaders
		{
			get
			{
				GetX<IBackgroundCopyJob4>(
					delegate(IBackgroundCopyJob4 bcj4)
					{
						bcj4.GetCustomHeaders(out ch);
					});
				return ch;
			}
			set
			{
				ch = value != null ? value : string.Empty;
				SetX<IBackgroundCopyJob4>(	delegate(IBackgroundCopyJob4 bcj4)
											{
												bcj4.SetCustomHeaders(ch.Length == 0 ? null : ch);
											});
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[
			DefaultValue(SSL.None),
			Category("Behavior"),
			Description("")
		]
		public SSL SSLFlags
		{
			get
			{
				GetX<IBackgroundCopyJob4>(delegate(IBackgroundCopyJob4 bcj4)
				{
					bcj4.GetSSLFlags(out ssl);
				});
				return ssl;
			}
			set
			{
				ssl = value;
				SetX<IBackgroundCopyJob4>(delegate(IBackgroundCopyJob4 bcj4)
				{
					bcj4.SetSSLFlags(ssl);
				});
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob5_gethttpredirectpolicy.asp
		/// </summary>
		[
			DefaultValue(HttpRedirectPolicy.Allowsilent),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob5_gethttpredirectpolicy.asp")
		]
		public HttpRedirectPolicy HttpRedirectPolicy
		{
			get
			{
				GetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.GetHttpRedirectPolicy(out hrp);
				});
				return hrp;
			}
			set
			{
				hrp = value;
				SetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.SetHttpRedirectPolicy(hrp);
				});
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob5_getenablehttpstohttpredirect.asp
		/// </summary>
		[
			DefaultValue(false),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob5_getenablehttpstohttpredirect.asp")
		]
		public bool EnableHttpsToHttpRedirect
		{
			get
			{
				GetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.GetEnableHttpsToHttpRedirect(out hhr);
				});
				return hhr;
			}
			set
			{
				hhr = value;
				SetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.SetEnableHttpsToHttpRedirect(hhr);
				});
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob5_getallowcaching.asp
		/// </summary>
		[
			DefaultValue(false),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob5_getallowcaching.asp")
		]
		public bool AllowCaching
		{
			get
			{
				GetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.GetAllowCaching(out ac);
				});
				return ac;
			}
			set
			{
				ac = value;
				SetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.SetAllowCaching(ac);
				});
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob5_getusepeercache.asp
		/// </summary>
		[
			DefaultValue(false),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob5_getusepeercache.asp")
		]
		public bool UsePeerCache
		{
			get
			{
				GetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.GetUsePeerCache(out upc);
				});
				return upc;
			}
			set
			{
				upc = value;
				SetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.SetUsePeerCache(upc);
				});
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("")
		]
		public ulong OwnerIntegrityLevel
		{
			get
			{
				GetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.GetOwnerIntegrityLevel(out oil);
				});
				return oil;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("")
		]
		public bool OwnerElevationState
		{
			get
			{
				GetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.GetOwnerElevationState(out oes);
				});
				return oes;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[
			DefaultValue(54000),
			Category("Behavior"),
			Description("")
		]
		public int MaxDownloadTime
		{
			get
			{
				GetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.GetMaxDownloadTime(out mdt);
				});
				return mdt;
			}
			set
			{
				mdt = value;
				SetX<IBackgroundCopyJob5>(delegate(IBackgroundCopyJob5 bcj5)
				{
					bcj5.SetMaxDownloadTime(mdt);
				});
			}
		}

		/// <summary>
		/// Can files from the Files collection be removed
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("Can files from the Files collection be removed")
		]
		public bool CanRemoveFiles { get { return guid == Guid.Empty; } }

		/// <summary>
		/// Is the job in a state where it can be suspended
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("Is the job in a state where it can be suspended")
		]
		public bool CanSuspend { get { return CanCancel; } }

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_suspend.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_suspend.asp")]
		public void Suspend()
		{
			Method(delegate() { bcj.Suspend(); });
		}

		private bool HasManager()
		{
			bool ret = false;
			MutexProtect(delegate() { ret = manager != null; });
			return ret;
		}

		/// <summary>
		/// Is the job in a state where it can be activated
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("Is the job in a state where it can be activated")
		]
		public bool CanActivate
		{
			get
			{
				return HasManager() && bcj == null && files.Count > 0;
			}
		}

		/// <summary>
		/// Create the IBackgroundCopyJob and add the files to the job. After this
		/// files can not be removed tom from the Files collection.
		/// </summary>
		[Description("Create the IBackgroundCopyJob and add the files to the job. After this files can not be removed tom from the Files collection.")]
		public void Activate()
		{
			MutexProtect(delegate()
			{
				if (manager != null)
				{
					Construct(); // actually create the job if not already
					AddFiles(); // actually add the files to the IBackgroundCopyJob
					JobModification(bcj, 0);
				}
			});
		}

		/// <summary>
		/// Is the job in a state where it can be resumed
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("Is the job in a state where it can be resumed")
		]
		public bool CanResume
		{
			get
			{
				return CanActivate || CanCancel;
			}
		}

		/// <summary>
		/// Job.Activate will be called before IBackgroundCopyJob.Resume. http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_resume.asp
		/// </summary>
		[Description("Job.Activate will be called before IBackgroundCopyJob.Resume. http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_resume.asp")]
		public void Resume()
		{
			Activate();
			Method(delegate() { bcj.Resume(); });
		}

		/// <summary>
		/// Is the job in a state where it can be completed
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("Is the job in a state where it can be completed")
		]
		public bool CanComplete
		{
			get
			{
				if (bcj == null)
					return false;
				
				JobState state = State;

				return Type == JobType.Download
							?	state != JobState.Acknowledged &&
								state != JobState.Cancelled
							: state == JobState.Transferred;
			}
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_complete.asp
		/// </summary>
		/// <returns>The BG_S_XXX success code</returns>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_complete.asp")]
		public int Complete()
		{
			int hr = 0;
			
			Method(delegate()
			{
				hr = bcj.Complete();
				
				if (hr < 0)
					throw new COMException(manager.GetErrorDescription(hr), hr);
			});
			
			return hr;
		}

		/// <summary>
		/// Is the job in a state where it can be cancelled
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("Is the job in a state where it can be cancelled")
		]
		public bool CanCancel
		{
			get
			{
				JobState state = State;
			
				return	bcj != null &&
						state != JobState.Acknowledged &&
						state != JobState.Cancelled;
			}
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_cancel.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_cancel.asp")]
		public void Cancel()
		{
			Method(delegate() { bcj.Cancel(); });
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_takeownership.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_takeownership.asp")]
		public void TakeOwnership()
		{
			Method(delegate() { bcj.TakeOwnership(); });
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_setcredentials.asp
		/// </summary>
		/// <param name="Credentials"></param>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_setcredentials.asp")]
		public void SetCredentials(Credentials Credentials)
		{
			MethodX<IBackgroundCopyJob2>(delegate(IBackgroundCopyJob2 bcj2)
			{
				_BG_AUTH_CREDENTIALS auth;

				Credentials.CopyTo(out auth);
				bcj2.SetCredentials(ref auth);
			});
			cred[Credentials.GetHashCode()] = Credentials;
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_removecredentials.asp
		/// </summary>
		/// <param name="Target"></param>
		/// <param name="Scheme"></param>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_removecredentials.asp")]
		public void RemoveCredentials(AuthTarget Target, AuthScheme Scheme)
		{
			MethodX<IBackgroundCopyJob2>(delegate(IBackgroundCopyJob2 bcj2)
			{
				bcj2.RemoveCredentials(Target, Scheme);
			});

			int code = Credentials.GetHashCode(Target, Scheme);

			if (cred.ContainsKey(code))
				cred.Remove(code);	
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob3_replaceremoteprefix.asp
		/// </summary>
		/// <param name="oldPrefix"></param>
		/// <param name="newPrefix"></param>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob3_replaceremoteprefix.asp")]
		public void ReplaceRemotePrefix(string oldPrefix, string newPrefix)
		{
			MethodX<IBackgroundCopyJob3>(delegate(IBackgroundCopyJob3 bcj3)
			{
				bcj3.ReplaceRemotePrefix(oldPrefix, newPrefix);
			});
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob4_removeclientcertificate.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob4_removeclientcertificate.asp")]
		public void RemoveClientCertificate()
		{
			MethodX<IBackgroundCopyJob4>(delegate(IBackgroundCopyJob4 bcj4)
			{
				bcj4.RemoveClientCertificate();
				cc.Reset();
			});
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob4_getclientcertificate.asp
		/// </summary>
		[
			DefaultValue(typeof(ClientCertificate), "None, \"\", \"0000000000000000000000000000000000000000\", \"\", False"),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob4_getclientcertificate.asp")
		]
		public ClientCertificate ClientCertificate
		{
			get
			{
				GetX<IBackgroundCopyJob4>(delegate(IBackgroundCopyJob4 bcj4)
				{
					IntPtr certHashBlob = IntPtr.Zero;

					Utils.TaskFree(ref certHashBlob, delegate()
					{
						CertStoreLocation csl;
						string sn;
						string subjectName;
						int hr = bcj4.GetClientCertificate(	out csl,
															out sn,
															out certHashBlob,
															out subjectName);

						if (hr < 0)
							throw new COMException(manager.GetErrorDescription(hr), hr);

						if (hr == 1)
							cc.Reset();
						else
						{
							byte[] blob = new byte[20];

							if (certHashBlob != IntPtr.Zero)
								Marshal.Copy(certHashBlob, blob, 0, blob.Length);

							cc.ArgCertHashBlob = blob;
							cc.StoreLocation = csl;
							cc.StoreName = sn;
							cc.SubjectName = subjectName;
							cc.SetByName = !string.IsNullOrEmpty(cc.SubjectName);
						}
					});
				});
				
				return cc;
			}
			set
			{
				if (value == null)
					cc.Reset();
				else
				{				
					cc.StoreLocation = value.StoreLocation;
					cc.StoreName = value.StoreName;
					cc.CertHashBlob = value.CertHashBlob;	
					cc.SubjectName = value.SubjectName;
					cc.SetByName = value.SetByName;
				}
				
				SetX<IBackgroundCopyJob4>(delegate(IBackgroundCopyJob4 bcj4)
				{
					if (cc.StoreLocation == CertStoreLocation.None)
						bcj4.RemoveClientCertificate();
					else if (cc.SetByName)
						bcj4.SetClientCertificateByName(cc.StoreLocation, cc.StoreName, cc.SubjectName);
					else
					{
						IntPtr mem = Marshal.AllocCoTaskMem(20);
						
						Utils.TaskFree(ref mem, delegate()
						{
							Marshal.Copy(cc.ArgCertHashBlob, 0, mem, 20);
							bcj4.SetClientCertificateByID(cc.StoreLocation, cc.StoreName, mem);
						});
					}
				});
			}
		}
		
		/// <summary>
		/// Run through the files collection and add and files to the IBackgroundCopyJob
		/// that aren't already in there.
		/// </summary>
		internal void AddFiles()
		{
			if (bcj == null)
				return;
				
			List<File> list = new List<File>(files.Count);
			int index = 0;
			
			foreach (File file in files)
			{	// File objects are mapped to their IBackgroundCopyFile's by index
				// in the IEnumBackgroundCopyFiles
				file.Index = index++;

				if (file.BCF == null) // may already have an IBackgroundCopyFile
				{
					if (!AddWithRange(file))
						list.Add(file);
				}
			}

			AddRange(list);
			
			foreach (File file in files)
			{	// attach the file object to its IBackgroundCopyFile
				if (file.BCF == null)
					file.Construct(files.Find(file));
			}
		}
		
		/// <summary>
		/// Files with ranges must be set special
		/// </summary>
		/// <param name="file"></param>
		/// <returns>if the file was added</returns>	
		private bool AddWithRange(File file)
		{
			if (file.Ranges == null || file.Ranges.Length == 0)
				return false;

			MethodX<IBackgroundCopyJob3>(delegate(IBackgroundCopyJob3 bcj3)
			{
				_BG_FILE_RANGE[] bfrs = new _BG_FILE_RANGE[file.Ranges.Length];

				for (int i = 0; i < file.Ranges.Length; i++)
					file.Ranges[i].CopyTo(ref bfrs[i]);

				IntPtr mem = Marshal.AllocCoTaskMem(Marshal.SizeOf(bfrs[0]) *
													file.Ranges.Length);

				Utils.TaskFree(ref mem, delegate()
				{
					IntPtr walk = mem;

					for (int i = 0; i < file.Ranges.Length; i++)
					{
						Marshal.StructureToPtr(bfrs[i], walk, false);
						walk = (IntPtr)((int)walk + Marshal.SizeOf(bfrs[0]));
					}

					bcj3.AddFileWithRanges(	file.RemoteFileName,
											file.LocalFileName,
											file.Ranges.Length,
											mem);
				});
			});
			
			return true;
		}

		/// <summary>
		/// efficiently add files to job
		/// </summary>
		/// <param name="list"></param>
		private void AddRange(List<File> list)
		{
			if (list.Count == 1)
			{
				Method(	delegate()
				{
					bcj.AddFile(list[0].RemoteFileName, list[0].LocalFileName);
				});
			}
			else if (list.Count > 1)
			{
				Method(	delegate()
				{
					_BG_FILE_INFO[] fis = new _BG_FILE_INFO[list.Count];

					for (int i = 0; i < list.Count; i++)
					{
						System.Diagnostics.Debug.Assert(list[i].BCF == null);
						fis[i].RemoteName = list[i].RemoteFileName;
						fis[i].LocalName = list[i].LocalFileName;
					}

					IntPtr mem = Marshal.AllocCoTaskMem(Marshal.SizeOf(fis[0]) *
														list.Count);
					
					Utils.TaskFree(ref mem, delegate()
					{
						IntPtr walk = mem;

						for (int i = 0; i < list.Count; i++)
						{
							Marshal.StructureToPtr(fis[i], walk, false);
							walk = (IntPtr)((int)walk + Marshal.SizeOf(fis[0]));
						}

						bcj.AddFileSet((uint)list.Count, mem);
					});
				});
			}
		}
		
		/// <summary>
		/// Job transferred callback
		/// </summary>
		/// <param name="pJob"></param>
		/// <returns>S_OK or E_FAIL, E_FAIL only if a cms line is set and
		/// the external client OK's it</returns>
		private void JobTransferred(IBackgroundCopyJob pJob)
		{
			MutexProtect(delegate()
			{
				if (manager != null)
				{
					JobTransferredEventArgs args = new JobTransferredEventArgs(this);
					
					args.RunCmdLine = autoRunCmdLine && cmdLine.Program != string.Empty;
					args.Complete = autoComplete;
					manager.FireJobTransferred(args);
					
					if (args.Complete)
						pJob.Complete();

					unchecked
					{
						if (cmdLine.Program != string.Empty && args.RunCmdLine)
							throw new COMException(string.Empty, (int)0x80004005);
					}
				}
			});
		}

		private void JobError(IBackgroundCopyJob pJob, IBackgroundCopyError pError)
		{
			MutexProtect(delegate()
			{
				if (manager != null)
				{
					JobErrorEventArgs args = new JobErrorEventArgs(this, je);
					
					je.CopyFrom(this, pError);
					args.RunCmdLine = autoRunCmdLine && cmdLine.Program != string.Empty;
					args.Cancel = autoCancelOnError;
					manager.FireJobError(args);

					if (args.Cancel && CanCancel)
						Cancel();

					unchecked
					{
						if (cmdLine.Program != string.Empty && args.RunCmdLine)
							throw new COMException(string.Empty, (int)0x80004005);
					}
				}
			});
		}

		private void JobModification(IBackgroundCopyJob pJob, uint dwReserved)
		{
			MutexProtect(delegate()
			{
				if (manager != null)
				{
					JobModificationEventArgs args = new JobModificationEventArgs(this);
					
					args.Cancel = State == JobState.TransientError && autoCancelOnTransientError;
					manager.FireJobModification(args);

					if (args.Cancel && CanCancel)
						Cancel();
				}
			});
		}

		private void FileTransferred(IBackgroundCopyJob pJob, IBackgroundCopyFile pFile)
		{
			MutexProtect(delegate()
			{
				if (manager != null)
				{
					FileTransferredEventArgs args = new FileTransferredEventArgs(this, pFile);
					
					args.Validate = autoValidate;
					manager.FireFileTransferred(args);
					
					if (args.Validate)
						args.File.ValidationState = args.IsValid;
				}
			});
		}

		private delegate void DoAction();

		/// <summary>
		/// generic IBackgroundCopyJob.GetXXX impl
		/// </summary>
		/// <param name="action"></param>
		private void Get(DoAction action) 
		{
			Set(action);				
		}

		/// <summary>
		/// generic IBackgroundCopyJob.SetXXX impl
		/// </summary>
		/// <param name="action"></param>
		private void Set(DoAction action)
		{
			try
			{
				if (bcj != null)
					action();
			}
			catch (Exception e)
			{
				HandleException(e);
			}
		}

		/// <summary>
		/// generic IBackgroundCopyJob.Method() impl
		/// </summary>
		/// <param name="action"></param>
		private void Method(DoAction action)
		{
			if (bcj == null)
				throw new BITSException(Properties.Resources.JobInvalid);

			Set(action);
		}

		private void MutexProtect(DoAction action)
		{
			mutex.WaitOne();

			try
			{
				action();
			}
			finally
			{
				mutex.ReleaseMutex();
			}
		}

		private delegate void DoActionX<T>(T bcjX) where T : IBackgroundCopyJob2;

		/// <summary>
		/// generic IBackgroundCopyJobX.GetXXX impl where X >= 2
		/// </summary>
		/// <typeparam name="T">IBackgroundCopyJob2 or 3</typeparam>
		/// <param name="action"></param>
		private void GetX<T>(DoActionX<T> action) where T : IBackgroundCopyJob2
		{
			SetX<T>(action);			
		}

		/// <summary>
		/// generic IBackgroundCopyJobX.SetXXX impl where X >= 2
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		private void SetX<T>(DoActionX<T> action) where T : IBackgroundCopyJob2
		{
			if (bcj != null)
			{
				T bcjX;

				try
				{
					bcjX = (T)bcj;
				}
				catch
				{
					throw new BITSUnsupportedException();
				}

				try
				{
					action(bcjX);
				}
				catch (Exception e)
				{
					HandleException(e);
				}
			}
		}
		
		/// <summary>
		/// generic IBackgroundCopyJobX.SetXXX impl where X >= 2
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		private void MethodX<T>(DoActionX<T> action) where T : IBackgroundCopyJob2
		{
			if (bcj == null)
				throw new BITSException(Properties.Resources.JobInvalid);

			SetX<T>(action);
		}

		/// <summary>
		/// callback impl class
		/// </summary>
		private class BackgroundCopyCallback : IBackgroundCopyCallback2
		{
			private Job job;

			public BackgroundCopyCallback(Job job)
			{
				this.job = job;
			}

			#region IBackgroundCopyCallback Members

			public void JobError(IBackgroundCopyJob pJob, IBackgroundCopyError pError)
			{
				job.JobError(pJob, pError);
			}

			public void JobModification(IBackgroundCopyJob pJob, uint dwReserved)
			{
				job.JobModification(pJob, dwReserved);
			}

			public void JobTransferred(IBackgroundCopyJob pJob)
			{
				job.JobTransferred(pJob);
			}

			public void FileTransferred(IBackgroundCopyJob pJob, IBackgroundCopyFile pFile)
			{
				job.FileTransferred(pJob, pFile);
			}

			#endregion
		};

		/// <summary>
		/// Destructor
		/// </summary>
		~Job()
		{
			Close();
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>The new Job</returns>
		public object Clone()
		{
			Job job = new Job(DisplayName, Type, Priority);
			File[] list = new File[files.Count];
			
			for (int i = 0; i < files.Count; i++)
				list[i] = (File)files[i].Clone();
			
			job.Files.AddRange(list);
			job.Description = desc;
			job.MinimumRetryDelay = minimumRetryDelay;
			job.NoProgressTimeout = noProgressTimeout;
			
			if (proxy.ProxyUsage != ProxyUsage.Preconfig)
				job.ProxySettings = proxy;
			
			if (manager.Version >= BITSVersion.V1_5)
			{
				foreach (KeyValuePair<int, Credentials> entry in cred)
					job.SetCredentials(entry.Value);
				
				if (cmdLine.Program != string.Empty)
					job.NotifyCmdLine = cmdLine;
			}
				
			if (manager.Version >= BITSVersion.V2_0)
			{
				if (replyFileName != string.Empty)
					job.ReplyFileName = replyFileName;
				
				if (aclFlags != CopyFileFlags.None)
					job.FileACLFlags = aclFlags;
			}

			if (manager.Version >= BITSVersion.V2_5)
			{
				if (ch != string.Empty)
					job.CustomHeaders = ch;

				if (ssl != SSL.None)
					job.SSLFlags = ssl;
			}

			if (manager.Version >= BITSVersion.V3_0)
			{
				if (hrp != HttpRedirectPolicy.Allowsilent)
					job.HttpRedirectPolicy = hrp;

				if (hhr != false)
					job.EnableHttpsToHttpRedirect = hhr;

				if (ac != true)
					job.AllowCaching = ac;

				if (upc != true)
					job.UsePeerCache = upc;

				if (mdt != 0)
					job.MaxDownloadTime = mdt;
					
				if (cc.StoreLocation != CertStoreLocation.None)
					job.ClientCertificate = cc;
			}

			return job;
		}

		#endregion
	}

	internal class JobTypeConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
		{
			if (destType == typeof(InstanceDescriptor))
				return true;

			return base.CanConvertTo(context, destType);
		}

		public override object ConvertTo(ITypeDescriptorContext context,System.Globalization.CultureInfo culture, object value, Type destType)
		{
			if (destType == typeof(InstanceDescriptor) && value is Job)
			{
				Job job = (Job)value;
				MemberInfo mi = typeof(File).GetConstructor(new Type[]
																{	
																	typeof(string),
																	typeof(JobType),
																	typeof(JobPriority)
																});
				object[] Arguments = new object[] { job.DisplayName, job.Type, job.Priority };

				return new InstanceDescriptor(mi, Arguments);
			}

			return base.ConvertTo(context, culture, value, destType);
		}
	}
}
