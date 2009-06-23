/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Net.BITS
{
	/// <summary>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bits_start_page.asp
	/// </summary>
	[
		DefaultEvent("OnModfication"),
		DefaultProperty("Jobs"),
		Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bits_start_page.asp")
	]
	public sealed partial class Manager : Component, ISupportInitialize
	{
		/// <summary>
		/// interface pointer to BITS
		/// </summary>
		private IBackgroundCopyManager bcm;
		/// <summary>
		/// what version of BITS have we created
		/// </summary>
		private BITSVersion version;
		/// <summary>
		/// The job collection
		/// </summary>
		private JobCollection jobs;
		/// <summary>
		/// 
		/// </summary>
		private GatewayMgr gatewayMgr;
		/// <summary>
		/// 
		/// </summary>
		private Cache cache;
		/// <summary>
		/// 
		/// </summary>
		private Neighborhood hood;
		/// <summary>
		/// are we inbetween ISupportInitialize.BeginInit and EndInit
		/// </summary>
		private bool init;

		/// <summary>
		/// Constructor
		/// </summary>
		public Manager()
		{
			InitializeComponent();
			InitializeManager();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="container"></param>
		public Manager(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
			InitializeManager();
		}
		
		/// <summary>
		/// The version of BITS installed on this machine
		/// </summary>
		[Description("The version of BITS installed on this machine")]
		public BITSVersion Version { get { return version; } }

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ienumbackgroundcopyjobs.asp
		/// </summary>
		[
			TypeConverter(typeof(CollectionConverter)),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
			Category("Behavior"),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ienumbackgroundcopyjobs.asp")
		]
		public JobCollection Jobs { get { return jobs; } }
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/igatewaymgr.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/igatewaymgr.asp")
		]
		public GatewayMgr GatewayManager { get { return gatewayMgr; } }
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycache.asp")
		]
		public Cache Cache { get { return cache; } }

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighborhood.asp
		/// </summary>
		[
			Browsable(false),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyneighborhood.asp")
		]
		public Neighborhood Neighborhood { get { return hood; } }

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycallback_jobtransferred.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycallback_jobtransferred.asp")]
		public event EventHandler<JobTransferredEventArgs> OnTransferred;
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycallback_joberror.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycallback_joberror.asp")]
		public event EventHandler<JobErrorEventArgs> OnError;
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycallback_jobmodification.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycallback_jobmodification.asp")]
		public event EventHandler<JobModificationEventArgs> OnModfication;
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycallback2_filetransferred.asp
		/// </summary>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopycallback2_filetransferred.asp")]
		public event EventHandler<FileTransferredEventArgs> OnFileTransferred;
		
		/// <summary>
		/// To populate the job colection with job belonging to this user or all users. http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopymanager_enumjobs.asp
		/// </summary>
		[
			DefaultValue(JobCollectionFlags.CurrentUser),
			Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopymanager_enumjobs.asp"),
			Category("Behavior")
		]
		public JobCollectionFlags JobCollectionFlags
		{
			get { return jobs.JobCollectionFlags; }
			set { jobs.JobCollectionFlags = value; }
		}
		
		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopymanager_geterrordescription.asp
		/// </summary>
		/// <param name="hr"></param>
		/// <param name="desc"></param>
		/// <returns>True if a description for the error was found</returns>
		[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopymanager_geterrordescription.asp")]
		public bool GetErrorDescription(int hr, out string desc)
		{
			desc = string.Empty;

            try
            {
                if (bcm == null)
                    return false;

                ushort lang = Properties.Resources.Culture != null
                                ? (ushort)Properties.Resources.Culture.LCID
                                : (ushort)0;

                bcm.GetErrorDescription(hr, lang, out desc);

                return true;
            }
            catch (InvalidComObjectException) { return false; }
            catch (COMException)
            {
                return false;
            }
		}

		/// <summary>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopymanager_geterrordescription.asp
		/// </summary>
		/// <param name="hr"></param>
		/// <returns>Returns string.Empty if a description for the error was not found</returns>
		public string GetErrorDescription(int hr)
		{
			string desc;
			GetErrorDescription(hr, out desc);
			return desc;
		}
		
		private bool Create<T>(BITSVersion v) where T : new()
		{
			try
			{
				bcm = (IBackgroundCopyManager)new T();
				version = v;
				
				return true;
			}
			catch (COMException e1)
			{
				return (uint)e1.ErrorCode != 0x80040154;	
			}
		}
		 
		/// <summary>
		/// Create the highest version of BITS available
		/// </summary>
		private void InitializeManager()
		{
			if (!Create<BackgroundCopyManager30>(BITSVersion.V3_0))
			{
				if (!Create<BackgroundCopyManager25>(BITSVersion.V2_5))
				{
					if (!Create<BackgroundCopyManager20>(BITSVersion.V2_0))
					{
						if (!Create<BackgroundCopyManager15>(BITSVersion.V1_5))
						{
							if (!Create<BackgroundCopyManager>(BITSVersion.V1_0))
							{
								bcm = null;
								version = BITSVersion.None;
							}
						}
					}
				}
			}
			
			jobs = new JobCollection(this);
			gatewayMgr = new GatewayMgr(this);
			cache = new Cache(this);
			hood = new Neighborhood(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		internal void HandleException(Exception e)
		{

			string desc;
			
			if (!GetErrorDescription(Marshal.GetHRForException(e), out desc))
				desc = e.Message;
		}

		/// <summary>
		/// 
		/// </summary>
		internal void CheckBCM()
		{
			if (bcm == null)
				throw new BITSException(Properties.Resources.NoBITS);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		internal void FireJobTransferred(JobTransferredEventArgs args)
		{
			if (OnTransferred != null)
				OnTransferred(this, args);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		internal void FireJobError(JobErrorEventArgs args)
		{
			if (OnError != null)
				OnError(this, args);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		internal void FireJobModification(JobModificationEventArgs args)
		{
			if (OnModfication != null)
				OnModfication(this, args);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		internal void FireFileTransferred(FileTransferredEventArgs args)
		{
			if (OnFileTransferred != null)
				OnFileTransferred(this, args);
		}

		internal IBackgroundCopyManager BCM { get { return bcm; } }
		/// <summary>
		/// In between BeginInit and EndInit or in design mode
		/// </summary>
		internal bool IsInit { get { return init || (Site != null && Site.DesignMode); } }

		#region ISupportInitialize Members

		/// <summary>
		/// Signals the object that initialization is starting.
		/// </summary>
		public void BeginInit()
		{
			init = true;
		}
		
		/// <summary>
		/// Once we're done with init, populate the jobs collection with existing jobs
		/// </summary>
		public void EndInit()
		{
			init = false;
			
			if (!IsInit)
				jobs.Update();
		}

		#endregion
	}
	
	/// <summary>
	/// Controls how is the Jobs Collection initially populated
	/// </summary>
	public enum JobCollectionFlags
	{
		/// <summary>
		/// Do not populate the Jobs collection with existing jobs
		/// </summary>
		None,
		/// <summary>
		/// Populate the Jobs collection with jobs belonging to the current user
		/// </summary>
		CurrentUser,
		/// <summary>
		/// Populate the Jobs collection with jobs belonging to the all users
		/// </summary>
		AllUsers
	}
	
	/// <summary>
	/// The version of BITS installed
	/// </summary>
	public enum BITSVersion
	{
		/// <summary>
		/// BITS not installed
		/// </summary>
		None,
		/// <summary>
		/// BITS version 1.0
		/// </summary>
		V1_0,
		/// <summary>
		/// BITS version 1.4
		/// </summary>
		V1_5,
		/// <summary>
		/// BITS version 2.0
		/// </summary>
		V2_0,
		/// <summary>
		/// BITS version 2.5
		/// </summary>
		V2_5,
		/// <summary>
		/// BITS version 3.0
		/// </summary>
		V3_0
	};
}
