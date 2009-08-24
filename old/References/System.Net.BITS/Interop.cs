//============================================================================================================
// Microsoft Updater Application Block for .NET
//  http://msdn.microsoft.com/library/en-us/dnbda/html/updater.asp
//	
// BITSInterop.cs
//
// Interop definitions for BITS COM interop.
// 
// For more information see the Updater Application Block Implementation Overview. 
// 
//============================================================================================================
// Copyright (C) 2000-2001 Microsoft Corporation
// All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
//============================================================================================================
//
// Added definitions to support IBackgroundCopyJob2::SetCredentials()
//
// 25/7/04 Eddie Tse (eddietse@hotmail.com)
//
//============================================================================================================

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace System.Net.BITS
{
	internal static class SID
	{
		private const int NO_ERROR = 0;
		private const int ERROR_INSUFFICIENT_BUFFER = 122;

		private enum SID_NAME_USE
		{
			SidTypeUser = 1,
			SidTypeGroup,
			SidTypeDomain,
			SidTypeAlias,
			SidTypeWellKnownGroup,
			SidTypeDeletedAccount,
			SidTypeInvalid,
			SidTypeUnknown,
			SidTypeComputer
		}
		
		[DllImport("advapi32.dll")]
		private static extern bool ConvertStringSidToSid(string StringSID, out IntPtr SID);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool LookupAccountSid(string lpSystemName,
													IntPtr SID,
													StringBuilder Name,
													ref long cbName,
													StringBuilder DomainName,
													ref long cbDomainName,
													out SID_NAME_USE psUse);
		internal static string GetName(string SID)
		{
			const int size = 1024;
			StringBuilder bufDomain = new StringBuilder(size);
			StringBuilder bufName = new StringBuilder(size);
			long cbUserName = size, cbDomainName = size;
			string domainName, userName;
			SID_NAME_USE psUse;
			IntPtr ptrSID;
			
			if (!ConvertStringSidToSid(SID, out ptrSID))
				return string.Empty;

			if (!LookupAccountSid(	String.Empty,
									ptrSID,
									bufName,
									ref cbUserName,
									bufDomain,
									ref cbDomainName,
									out psUse))
			{
				return string.Empty;
			}
			
			userName = bufName.ToString();
			domainName = bufDomain.ToString();
			
			return String.Format(	Properties.Resources.UserNameFormat,
									domainName,
									userName);
		}
	}

	// COM Interop C# classes for accessing BITS API.
	// Refer to MSDN for Details: 
	// http://msdn.microsoft.com/library/en-us/bits/bits/bits_reference.asp
	// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/service_accounts_and_bits.asp 
	// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/enumerating_jobs_in_the_transfer_queue.asp
	// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/handling_errors.asp?frame=true
	// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnwxp/html/WinXP_BITS.asp 
	// http://msdn.microsoft.com/msdnmag/issues/03/02/BITS/default.aspx 

	/// <summary>
	/// BackgroundCopyManager Class
	/// </summary>
	[GuidAttribute("4991D34B-80A1-4291-83B6-3328366B9097")]
	[ClassInterfaceAttribute(ClassInterfaceType.None)]
	[ComImportAttribute()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class BackgroundCopyManager
	{
		internal const uint BG_JOB_ENUM_ALL_USERS = 0x0001;
	}

	/// <summary>
	/// BackgroundCopyManager15 Class
	/// </summary>
	[GuidAttribute("f087771f-d74f-4c1a-bb8a-e16aca9124ea")]
	[ClassInterfaceAttribute(ClassInterfaceType.None)]
	[ComImportAttribute()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class BackgroundCopyManager15
	{
	}

	/// <summary>
	/// BackgroundCopyManager20 Class
	/// </summary>
	[GuidAttribute("6d18ad12-bde3-4393-b311-099c346e6df9")]
	[ClassInterfaceAttribute(ClassInterfaceType.None)]
	[ComImportAttribute()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class BackgroundCopyManager20
	{
	}

	/// <summary>
	/// BackgroundCopyManager25 Class
	/// </summary>
	[GuidAttribute("03ca98d6-ff5d-49b8-abc6-03dd84127020")]
	[ClassInterfaceAttribute(ClassInterfaceType.None)]
	[ComImportAttribute()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class BackgroundCopyManager25
	{
	}
	
 	/// <summary>
	/// BackgroundCopyManager30 Class
	/// </summary>
	[GuidAttribute("659cdea7-489e-11d9-a9cd-000d56965251")]
	[ClassInterfaceAttribute(ClassInterfaceType.None)]
	[ComImportAttribute()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class BackgroundCopyManager30
    {
    };

	/// <summary>
	/// Use the IBackgroundCopyManager interface to create transfer jobs, 
	/// retrieve an enumerator object that contains the jobs in the queue, 
	/// and to retrieve individual jobs from the queue.
	/// </summary>
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	[GuidAttribute("5CE34C0D-0DC9-4C1F-897C-DAA1B78CEE7C")]
	[ComImportAttribute()]
	internal interface IBackgroundCopyManager 
	{		
		/// <summary>
		/// Creates a new transfer job
		/// </summary>
		void CreateJob([MarshalAs(UnmanagedType.LPWStr)] string DisplayName, JobType Type, out Guid pJobId, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob ppJob);
		
		/// <summary>
		/// Retrieves a given job from the queue
		/// </summary>
		void GetJob(ref Guid jobID, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob ppJob);
		
		/// <summary>
		/// Retrieves an enumerator object that you use to enumerate jobs in the queue
		/// </summary>
		void EnumJobs(uint dwFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyJobs ppenum);
		
		/// <summary>
		/// Retrieves a description for the given error code
		/// </summary>
		void GetErrorDescription([MarshalAs(UnmanagedType.Error)] int hResult, int LanguageId, [MarshalAs(UnmanagedType.LPWStr)] out string pErrorDescription);
	}

 
	/// <summary>
	/// Use the IBackgroundCopyJob interface to add files to the job, 
	/// set the priority level of the job, determine the state of the
	/// job, and to start and stop the job.
	/// </summary>
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	[GuidAttribute("37668D37-507E-4160-9316-26306D150B12")]
	[ComImport]
	internal interface IBackgroundCopyJob 
	{
		/// <summary>
		/// Adds multiple files to the job
		/// </summary>
		void AddFileSet(uint cFileCount, IntPtr pFileSet);

		/// <summary>
		/// Adds a single file to the job
		/// </summary>
		void AddFile([MarshalAs(UnmanagedType.LPWStr)] string RemoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string LocalName);

		
		/// <summary>
		/// Returns an interface pointer to an enumerator
		/// object that you use to enumerate the files in the job
		/// </summary>
		void EnumFiles([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles pEnum);

		
		/// <summary>
		/// Pauses the job
		/// </summary>
		void Suspend();

		
		/// <summary>
		/// Restarts a suspended job
		/// </summary>
		void Resume();

		
		/// <summary>
		/// Cancels the job and removes temporary files from the client
		/// </summary>
		void Cancel();
		
		/// <summary>
		/// Ends the job and saves the transferred files on the client
		/// </summary>
		[PreserveSig]
		int Complete();
		
		/// <summary>
		/// Retrieves the identifier of the job in the queue
		/// </summary>
		void GetId(out Guid pVal);
		
		/// <summary>
		/// Retrieves the type of transfer being performed, 
		/// such as a file download
		/// </summary>
		void GetType(out JobType pVal);
		
		/// <summary>
		/// Retrieves job-related progress information, 
		/// such as the number of bytes and files transferred 
		/// to the client
		/// </summary>
		void GetProgress(out _BG_JOB_PROGRESS pVal);
		
		/// <summary>
		/// Retrieves timestamps for activities related
		/// to the job, such as the time the job was created
		/// </summary>
		void GetTimes(out _BG_JOB_TIMES pVal);
		
		/// <summary>
		/// Retrieves the state of the job
		/// </summary>
		void GetState(out JobState pVal);
		
		/// <summary>
		/// Retrieves an interface pointer to 
		/// the error object after an error occurs
		/// </summary>
		void GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError ppError);
		
		/// <summary>
		/// Retrieves the job owner's identity
		/// </summary>
		void GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string pVal);
		
		/// <summary>
		/// Specifies a display name that identifies the job in 
		/// a user interface
		/// </summary>
		void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Val);
		
		/// <summary>
		/// Retrieves the display name that identifies the job
		/// </summary>
		void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);
		
		/// <summary>
		/// Specifies a description of the job
		/// </summary>
		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string Val);
		
		/// <summary>
		/// Retrieves the description of the job
		/// </summary>
		void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string pVal);
		
		/// <summary>
		/// Specifies the priority of the job relative to 
		/// other jobs in the transfer queue
		/// </summary>
		void SetPriority(JobPriority Val);

		/// <summary>
		/// Retrieves the priority level you have set for the job.
		/// </summary>
		void GetPriority(out JobPriority pVal);
		
		/// <summary>
		/// Specifies the type of event notification to receive
		/// </summary>
		void SetNotifyFlags([MarshalAs(UnmanagedType.U4)] NotificationType Val);
		
		/// <summary>
		/// Retrieves the event notification (callback) flags 
		/// you have set for your application.
		/// </summary>
		void GetNotifyFlags(out uint pVal);
		
		/// <summary>
		/// Specifies a pointer to your implementation of the 
		/// IBackgroundCopyCallback interface (callbacks). The 
		/// interface receives notification based on the event 
		/// notification flags you set
		/// </summary>
		void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object Val);
		
		/// <summary>
		/// Retrieves a pointer to your implementation 
		/// of the IBackgroundCopyCallback interface (callbacks).
		/// </summary>
		void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object pVal);
		
		/// <summary>
		/// Specifies the minimum length of time that BITS waits after 
		/// encountering a transient error condition before trying to 
		/// transfer the file
		/// </summary>
		void SetMinimumRetryDelay(int Seconds);
		
		/// <summary>
		/// Retrieves the minimum length of time that BITS waits after 
		/// encountering a transient error condition before trying to 
		/// transfer the file
		/// </summary>
		void GetMinimumRetryDelay(out int Seconds);
		
		/// <summary>
		/// Specifies the length of time that BITS continues to try to 
		/// transfer the file after encountering a transient error 
		/// condition
		/// </summary>
		void SetNoProgressTimeout(int Seconds);
		
		/// <summary>
		/// Retrieves the length of time that BITS continues to try to 
		/// transfer the file after encountering a transient error condition
		/// </summary>
		void GetNoProgressTimeout(out int Seconds);
		
		/// <summary>
		/// Retrieves the number of times the job was interrupted by 
		/// network failure or server unavailability
		/// </summary>
		void GetErrorCount(out uint Errors);
		
		/// <summary>
		/// Specifies which proxy to use to transfer the files
		/// </summary>
		void SetProxySettings(ProxyUsage ProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] string ProxyList, [MarshalAs(UnmanagedType.LPWStr)] string ProxyBypassList);
		
		/// <summary>
		/// Retrieves the proxy settings the job uses to transfer the files
		/// </summary>
		void GetProxySettings(out ProxyUsage pProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyList, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyBypassList);
		
		/// <summary>
		/// Changes the ownership of the job to the current user
		/// </summary>
		void TakeOwnership();
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("54B50739-686F-45EB-9DFF-D6A9A0FAA9AF")]
	internal interface IBackgroundCopyJob2 : IBackgroundCopyJob
	{
		/// <summary>
		/// Adds multiple files to the job
		/// </summary>
		new void AddFileSet(uint cFileCount, IntPtr pFileSet);

		/// <summary>
		/// Adds a single file to the job
		/// </summary>
		new void AddFile([MarshalAs(UnmanagedType.LPWStr)] string RemoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string LocalName);
		
		/// <summary>
		/// Returns an interface pointer to an enumerator
		/// object that you use to enumerate the files in the job
		/// </summary>
		new void EnumFiles([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles pEnum);
		
		/// <summary>
		/// Pauses the job
		/// </summary>
		new void Suspend();
		
		/// <summary>
		/// Restarts a suspended job
		/// </summary>
		new void Resume();
		
		/// <summary>
		/// Cancels the job and removes temporary files from the client
		/// </summary>
		new void Cancel();
		
		/// <summary>
		/// Ends the job and saves the transferred files on the client
		/// </summary>
		[PreserveSig]
		new int Complete();
		
		/// <summary>
		/// Retrieves the identifier of the job in the queue
		/// </summary>
		new void GetId(out Guid pVal);
		
		/// <summary>
		/// Retrieves the type of transfer being performed, 
		/// such as a file download
		/// </summary>
		new void GetType(out JobType pVal);
		
		/// <summary>
		/// Retrieves job-related progress information, 
		/// such as the number of bytes and files transferred 
		/// to the client
		/// </summary>
		new void GetProgress(out _BG_JOB_PROGRESS pVal);
		
		/// <summary>
		/// Retrieves timestamps for activities related
		/// to the job, such as the time the job was created
		/// </summary>
		new void GetTimes(out _BG_JOB_TIMES pVal);
		
		/// <summary>
		/// Retrieves the state of the job
		/// </summary>
		new void GetState(out JobState pVal);
		
		/// <summary>
		/// Retrieves an interface pointer to 
		/// the error object after an error occurs
		/// </summary>
		new void GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError ppError);
		
		/// <summary>
		/// Retrieves the job owner's identity
		/// </summary>
		new void GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string pVal);
		
		/// <summary>
		/// Specifies a display name that identifies the job in 
		/// a user interface
		/// </summary>
		new void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Val);
		
		/// <summary>
		/// Retrieves the display name that identifies the job
		/// </summary>
		new void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);
		
		/// <summary>
		/// Specifies a description of the job
		/// </summary>
		new void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string Val);
		
		/// <summary>
		/// Retrieves the description of the job
		/// </summary>
		new void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string pVal);
		
		/// <summary>
		/// Specifies the priority of the job relative to 
		/// other jobs in the transfer queue
		/// </summary>
		new void SetPriority(JobPriority Val);

		/// <summary>
		/// Retrieves the priority level you have set for the job.
		/// </summary>
		new void GetPriority(out JobPriority pVal);
		
		/// <summary>
		/// Specifies the type of event notification to receive
		/// </summary>
		new void SetNotifyFlags([MarshalAs(UnmanagedType.U4)] NotificationType Val);
		
		/// <summary>
		/// Retrieves the event notification (callback) flags 
		/// you have set for your application.
		/// </summary>
		new void GetNotifyFlags(out uint pVal);
		
		/// <summary>
		/// Specifies a pointer to your implementation of the 
		/// IBackgroundCopyCallback interface (callbacks). The 
		/// interface receives notification based on the event 
		/// notification flags you set
		/// </summary>
		new void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object Val);
		
		/// <summary>
		/// Retrieves a pointer to your implementation 
		/// of the IBackgroundCopyCallback interface (callbacks).
		/// </summary>
		new void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object pVal);
		
		/// <summary>
		/// Specifies the minimum length of time that BITS waits after 
		/// encountering a transient error condition before trying to 
		/// transfer the file
		/// </summary>
		new void SetMinimumRetryDelay(int Seconds);
		
		/// <summary>
		/// Retrieves the minimum length of time that BITS waits after 
		/// encountering a transient error condition before trying to 
		/// transfer the file
		/// </summary>
		new void GetMinimumRetryDelay(out int Seconds);
		
		/// <summary>
		/// Specifies the length of time that BITS continues to try to 
		/// transfer the file after encountering a transient error 
		/// condition
		/// </summary>
		new void SetNoProgressTimeout(int Seconds);
		
		/// <summary>
		/// Retrieves the length of time that BITS continues to try to 
		/// transfer the file after encountering a transient error condition
		/// </summary>
		new void GetNoProgressTimeout(out int Seconds);
		
		/// <summary>
		/// Retrieves the number of times the job was interrupted by 
		/// network failure or server unavailability
		/// </summary>
		new void GetErrorCount(out uint Errors);
		
		/// <summary>
		/// Specifies which proxy to use to transfer the files
		/// </summary>
		new void SetProxySettings(ProxyUsage ProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] string ProxyList, [MarshalAs(UnmanagedType.LPWStr)] string ProxyBypassList);
		
		/// <summary>
		/// Retrieves the proxy settings the job uses to transfer the files
		/// </summary>
		new void GetProxySettings(out ProxyUsage pProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyList, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyBypassList);
		
		/// <summary>
		/// Changes the ownership of the job to the current user
		/// </summary>
		new void TakeOwnership();

		///
		/// Starts definition of IBackgroundCopyJob2
		///
		void SetNotifyCmdLine([In, MarshalAs(UnmanagedType.LPWStr)] string Program, [In, MarshalAs(UnmanagedType.LPWStr)] string Parameters);

		void GetNotifyCmdLine([MarshalAs(UnmanagedType.LPWStr)] out string pProgram, [MarshalAs(UnmanagedType.LPWStr)] out string pParameters);

		void GetReplyProgress([Out] out _BG_JOB_REPLY_PROGRESS pProgress);

		void GetReplyData([In, Out] ref IntPtr ppBuffer, out ulong pLength);

		void SetReplyFileName([In, MarshalAs(UnmanagedType.LPWStr)] string ReplyFileName);

		void GetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] out string pReplyFileName);

		void SetCredentials([In] ref _BG_AUTH_CREDENTIALS Credentials);

		void RemoveCredentials(AuthTarget Target, AuthScheme Scheme);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("443c8934-90ff-48ed-bcde-26f5c7450042")]
	internal interface IBackgroundCopyJob3 : IBackgroundCopyJob2
	{
		/// <summary>
		/// Adds multiple files to the job
		/// </summary>
		new void AddFileSet(uint cFileCount, IntPtr pFileSet);

		/// <summary>
		/// Adds a single file to the job
		/// </summary>
		new void AddFile([MarshalAs(UnmanagedType.LPWStr)] string RemoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string LocalName);

		/// <summary>
		/// Returns an interface pointer to an enumerator
		/// object that you use to enumerate the files in the job
		/// </summary>
		new void EnumFiles([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles pEnum);

		/// <summary>
		/// Pauses the job
		/// </summary>
		new void Suspend();

		/// <summary>
		/// Restarts a suspended job
		/// </summary>
		new void Resume();

		/// <summary>
		/// Cancels the job and removes temporary files from the client
		/// </summary>
		new void Cancel();

		/// <summary>
		/// Ends the job and saves the transferred files on the client
		/// </summary>
		[PreserveSig]
		new int Complete();

		/// <summary>
		/// Retrieves the identifier of the job in the queue
		/// </summary>
		new void GetId(out Guid pVal);

		/// <summary>
		/// Retrieves the type of transfer being performed, 
		/// such as a file download
		/// </summary>
		new void GetType(out JobType pVal);

		/// <summary>
		/// Retrieves job-related progress information, 
		/// such as the number of bytes and files transferred 
		/// to the client
		/// </summary>
		new void GetProgress(out _BG_JOB_PROGRESS pVal);

		/// <summary>
		/// Retrieves timestamps for activities related
		/// to the job, such as the time the job was created
		/// </summary>
		new void GetTimes(out _BG_JOB_TIMES pVal);

		/// <summary>
		/// Retrieves the state of the job
		/// </summary>
		new void GetState(out JobState pVal);

		/// <summary>
		/// Retrieves an interface pointer to 
		/// the error object after an error occurs
		/// </summary>
		new void GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError ppError);

		/// <summary>
		/// Retrieves the job owner's identity
		/// </summary>
		new void GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Specifies a display name that identifies the job in 
		/// a user interface
		/// </summary>
		new void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Val);

		/// <summary>
		/// Retrieves the display name that identifies the job
		/// </summary>
		new void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Specifies a description of the job
		/// </summary>
		new void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string Val);

		/// <summary>
		/// Retrieves the description of the job
		/// </summary>
		new void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Specifies the priority of the job relative to 
		/// other jobs in the transfer queue
		/// </summary>
		new void SetPriority(JobPriority Val);

		/// <summary>
		/// Retrieves the priority level you have set for the job.
		/// </summary>
		new void GetPriority(out JobPriority pVal);

		/// <summary>
		/// Specifies the type of event notification to receive
		/// </summary>
		new void SetNotifyFlags([MarshalAs(UnmanagedType.U4)] NotificationType Val);

		/// <summary>
		/// Retrieves the event notification (callback) flags 
		/// you have set for your application.
		/// </summary>
		new void GetNotifyFlags(out uint pVal);

		/// <summary>
		/// Specifies a pointer to your implementation of the 
		/// IBackgroundCopyCallback interface (callbacks). The 
		/// interface receives notification based on the event 
		/// notification flags you set
		/// </summary>
		new void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object Val);

		/// <summary>
		/// Retrieves a pointer to your implementation 
		/// of the IBackgroundCopyCallback interface (callbacks).
		/// </summary>
		new void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object pVal);

		/// <summary>
		/// Specifies the minimum length of time that BITS waits after 
		/// encountering a transient error condition before trying to 
		/// transfer the file
		/// </summary>
		new void SetMinimumRetryDelay(int Seconds);

		/// <summary>
		/// Retrieves the minimum length of time that BITS waits after 
		/// encountering a transient error condition before trying to 
		/// transfer the file
		/// </summary>
		new void GetMinimumRetryDelay(out int Seconds);

		/// <summary>
		/// Specifies the length of time that BITS continues to try to 
		/// transfer the file after encountering a transient error 
		/// condition
		/// </summary>
		new void SetNoProgressTimeout(int Seconds);

		/// <summary>
		/// Retrieves the length of time that BITS continues to try to 
		/// transfer the file after encountering a transient error condition
		/// </summary>
		new void GetNoProgressTimeout(out int Seconds);

		/// <summary>
		/// Retrieves the number of times the job was interrupted by 
		/// network failure or server unavailability
		/// </summary>
		new void GetErrorCount(out uint Errors);

		/// <summary>
		/// Specifies which proxy to use to transfer the files
		/// </summary>
		new void SetProxySettings(ProxyUsage ProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] string ProxyList, [MarshalAs(UnmanagedType.LPWStr)] string ProxyBypassList);

		/// <summary>
		/// Retrieves the proxy settings the job uses to transfer the files
		/// </summary>
		new void GetProxySettings(out ProxyUsage pProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyList, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyBypassList);

		/// <summary>
		/// Changes the ownership of the job to the current user
		/// </summary>
		new void TakeOwnership();

		new void SetNotifyCmdLine([In, MarshalAs(UnmanagedType.LPWStr)] string Program, [In, MarshalAs(UnmanagedType.LPWStr)] string Parameters);

		new void GetNotifyCmdLine([MarshalAs(UnmanagedType.LPWStr)] out string pProgram, [MarshalAs(UnmanagedType.LPWStr)] out string pParameters);

		new void GetReplyProgress([Out] out _BG_JOB_REPLY_PROGRESS pProgress);

		new void GetReplyData([In, Out] ref IntPtr ppBuffer, out ulong pLength);

		new void SetReplyFileName([In, MarshalAs(UnmanagedType.LPWStr)] string ReplyFileName);

		new void GetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] out string pReplyFileName);

		new void SetCredentials([In] ref _BG_AUTH_CREDENTIALS Credentials);

		new void RemoveCredentials(AuthTarget Target, AuthScheme Scheme);

		///
		/// Starts definition of IBackgroundCopyJob3
		///
        void ReplaceRemotePrefix([MarshalAs(UnmanagedType.LPWStr)] string OldPrefix, [MarshalAs(UnmanagedType.LPWStr)] string NewPrefix);
        
        void AddFileWithRanges([MarshalAs(UnmanagedType.LPWStr)] string RemoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string LocalName, int RangeCount, IntPtr Ranges);
        
        void SetFileACLFlags(CopyFileFlags Flags);
        
        void GetFileACLFlags(out CopyFileFlags Flags);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("bc2c92df-4972-4fa7-b8a0-444e127ba670")]
	internal interface IBackgroundCopyJob4 : IBackgroundCopyJob3
	{
		/// <summary>
		/// Adds multiple files to the job
		/// </summary>
		new void AddFileSet(uint cFileCount, IntPtr pFileSet);

		/// <summary>
		/// Adds a single file to the job
		/// </summary>
		new void AddFile([MarshalAs(UnmanagedType.LPWStr)] string RemoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string LocalName);

		/// <summary>
		/// Returns an interface pointer to an enumerator
		/// object that you use to enumerate the files in the job
		/// </summary>
		new void EnumFiles([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles pEnum);

		/// <summary>
		/// Pauses the job
		/// </summary>
		new void Suspend();

		/// <summary>
		/// Restarts a suspended job
		/// </summary>
		new void Resume();

		/// <summary>
		/// Cancels the job and removes temporary files from the client
		/// </summary>
		new void Cancel();

		/// <summary>
		/// Ends the job and saves the transferred files on the client
		/// </summary>
		[PreserveSig]
		new int Complete();

		/// <summary>
		/// Retrieves the identifier of the job in the queue
		/// </summary>
		new void GetId(out Guid pVal);

		/// <summary>
		/// Retrieves the type of transfer being performed, 
		/// such as a file download
		/// </summary>
		new void GetType(out JobType pVal);

		/// <summary>
		/// Retrieves job-related progress information, 
		/// such as the number of bytes and files transferred 
		/// to the client
		/// </summary>
		new void GetProgress(out _BG_JOB_PROGRESS pVal);

		/// <summary>
		/// Retrieves timestamps for activities related
		/// to the job, such as the time the job was created
		/// </summary>
		new void GetTimes(out _BG_JOB_TIMES pVal);

		/// <summary>
		/// Retrieves the state of the job
		/// </summary>
		new void GetState(out JobState pVal);

		/// <summary>
		/// Retrieves an interface pointer to 
		/// the error object after an error occurs
		/// </summary>
		new void GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError ppError);

		/// <summary>
		/// Retrieves the job owner's identity
		/// </summary>
		new void GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Specifies a display name that identifies the job in 
		/// a user interface
		/// </summary>
		new void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Val);

		/// <summary>
		/// Retrieves the display name that identifies the job
		/// </summary>
		new void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Specifies a description of the job
		/// </summary>
		new void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string Val);

		/// <summary>
		/// Retrieves the description of the job
		/// </summary>
		new void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Specifies the priority of the job relative to 
		/// other jobs in the transfer queue
		/// </summary>
		new void SetPriority(JobPriority Val);

		/// <summary>
		/// Retrieves the priority level you have set for the job.
		/// </summary>
		new void GetPriority(out JobPriority pVal);

		/// <summary>
		/// Specifies the type of event notification to receive
		/// </summary>
		new void SetNotifyFlags([MarshalAs(UnmanagedType.U4)] NotificationType Val);

		/// <summary>
		/// Retrieves the event notification (callback) flags 
		/// you have set for your application.
		/// </summary>
		new void GetNotifyFlags(out uint pVal);

		/// <summary>
		/// Specifies a pointer to your implementation of the 
		/// IBackgroundCopyCallback interface (callbacks). The 
		/// interface receives notification based on the event 
		/// notification flags you set
		/// </summary>
		new void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object Val);

		/// <summary>
		/// Retrieves a pointer to your implementation 
		/// of the IBackgroundCopyCallback interface (callbacks).
		/// </summary>
		new void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object pVal);

		/// <summary>
		/// Specifies the minimum length of time that BITS waits after 
		/// encountering a transient error condition before trying to 
		/// transfer the file
		/// </summary>
		new void SetMinimumRetryDelay(int Seconds);

		/// <summary>
		/// Retrieves the minimum length of time that BITS waits after 
		/// encountering a transient error condition before trying to 
		/// transfer the file
		/// </summary>
		new void GetMinimumRetryDelay(out int Seconds);

		/// <summary>
		/// Specifies the length of time that BITS continues to try to 
		/// transfer the file after encountering a transient error 
		/// condition
		/// </summary>
		new void SetNoProgressTimeout(int Seconds);

		/// <summary>
		/// Retrieves the length of time that BITS continues to try to 
		/// transfer the file after encountering a transient error condition
		/// </summary>
		new void GetNoProgressTimeout(out int Seconds);

		/// <summary>
		/// Retrieves the number of times the job was interrupted by 
		/// network failure or server unavailability
		/// </summary>
		new void GetErrorCount(out uint Errors);

		/// <summary>
		/// Specifies which proxy to use to transfer the files
		/// </summary>
		new void SetProxySettings(ProxyUsage ProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] string ProxyList, [MarshalAs(UnmanagedType.LPWStr)] string ProxyBypassList);

		/// <summary>
		/// Retrieves the proxy settings the job uses to transfer the files
		/// </summary>
		new void GetProxySettings(out ProxyUsage pProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyList, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyBypassList);

		/// <summary>
		/// Changes the ownership of the job to the current user
		/// </summary>
		new void TakeOwnership();

		new void SetNotifyCmdLine([In, MarshalAs(UnmanagedType.LPWStr)] string Program, [In, MarshalAs(UnmanagedType.LPWStr)] string Parameters);

		new void GetNotifyCmdLine([MarshalAs(UnmanagedType.LPWStr)] out string pProgram, [MarshalAs(UnmanagedType.LPWStr)] out string pParameters);

		new void GetReplyProgress([Out] out _BG_JOB_REPLY_PROGRESS pProgress);

		new void GetReplyData([In, Out] ref IntPtr ppBuffer, out ulong pLength);

		new void SetReplyFileName([In, MarshalAs(UnmanagedType.LPWStr)] string ReplyFileName);

		new void GetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] out string pReplyFileName);

		new void SetCredentials([In] ref _BG_AUTH_CREDENTIALS Credentials);

		new void RemoveCredentials(AuthTarget Target, AuthScheme Scheme);

        new void ReplaceRemotePrefix([MarshalAs(UnmanagedType.LPWStr)] string OldPrefix, [MarshalAs(UnmanagedType.LPWStr)] string NewPrefix);

		new void AddFileWithRanges([MarshalAs(UnmanagedType.LPWStr)] string RemoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string LocalName, int RangeCount, IntPtr Ranges);

		new void SetFileACLFlags(CopyFileFlags Flags);

		new void GetFileACLFlags(out CopyFileFlags Flags);

		///
		/// Starts definition of IBackgroundCopyJob4
		///
        void SetClientCertificateByID(
			CertStoreLocation StoreLocation,
			[In, MarshalAs(UnmanagedType.LPWStr)] string StoreName,
			[In] IntPtr pCertHashBlob);
        
        void SetClientCertificateByName( 
			CertStoreLocation StoreLocation,
			[In, MarshalAs(UnmanagedType.LPWStr)] string StoreName,
			[In, MarshalAs(UnmanagedType.LPWStr)] string SubjectName);
    
        void RemoveClientCertificate();
        
        [PreserveSig]
        int GetClientCertificate( 
			[Out] out CertStoreLocation pStoreLocation,
			[Out, MarshalAs(UnmanagedType.LPWStr)] out string pStoreName,
			[Out] out IntPtr ppCertHashBlob,
			[Out, MarshalAs(UnmanagedType.LPWStr)] out string pSubjectName);
        
        void SetCustomHeaders([In, MarshalAs(UnmanagedType.LPWStr)] string RequestHeaders);
        
        void GetCustomHeaders([Out, MarshalAs(UnmanagedType.LPWStr)] out string pRequestHeaders);
        
        void SetSSLFlags([In] SSL Flags);
    
        void GetSSLFlags([Out] out SSL pFlags);
    }

	[ComImport]
	[GuidAttribute("659cdeab-489e-11d9-a9cd-000d56965251")]
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IBackgroundCopyJob5 : IBackgroundCopyJob4
	{
		/// <summary>
		/// Adds multiple files to the job
		/// </summary>
		new void AddFileSet(uint cFileCount, IntPtr pFileSet);

		/// <summary>
		/// Adds a single file to the job
		/// </summary>
		new void AddFile([MarshalAs(UnmanagedType.LPWStr)] string RemoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string LocalName);

		/// <summary>
		/// Returns an interface pointer to an enumerator
		/// object that you use to enumerate the files in the job
		/// </summary>
		new void EnumFiles([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles pEnum);

		/// <summary>
		/// Pauses the job
		/// </summary>
		new void Suspend();

		/// <summary>
		/// Restarts a suspended job
		/// </summary>
		new void Resume();

		/// <summary>
		/// Cancels the job and removes temporary files from the client
		/// </summary>
		new void Cancel();

		/// <summary>
		/// Ends the job and saves the transferred files on the client
		/// </summary>
		[PreserveSig]
		new int Complete();

		/// <summary>
		/// Retrieves the identifier of the job in the queue
		/// </summary>
		new void GetId(out Guid pVal);

		/// <summary>
		/// Retrieves the type of transfer being performed, 
		/// such as a file download
		/// </summary>
		new void GetType(out JobType pVal);

		/// <summary>
		/// Retrieves job-related progress information, 
		/// such as the number of bytes and files transferred 
		/// to the client
		/// </summary>
		new void GetProgress(out _BG_JOB_PROGRESS pVal);

		/// <summary>
		/// Retrieves timestamps for activities related
		/// to the job, such as the time the job was created
		/// </summary>
		new void GetTimes(out _BG_JOB_TIMES pVal);

		/// <summary>
		/// Retrieves the state of the job
		/// </summary>
		new void GetState(out JobState pVal);

		/// <summary>
		/// Retrieves an interface pointer to 
		/// the error object after an error occurs
		/// </summary>
		new void GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError ppError);

		/// <summary>
		/// Retrieves the job owner's identity
		/// </summary>
		new void GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Specifies a display name that identifies the job in 
		/// a user interface
		/// </summary>
		new void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Val);

		/// <summary>
		/// Retrieves the display name that identifies the job
		/// </summary>
		new void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Specifies a description of the job
		/// </summary>
		new void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string Val);

		/// <summary>
		/// Retrieves the description of the job
		/// </summary>
		new void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Specifies the priority of the job relative to 
		/// other jobs in the transfer queue
		/// </summary>
		new void SetPriority(JobPriority Val);

		/// <summary>
		/// Retrieves the priority level you have set for the job.
		/// </summary>
		new void GetPriority(out JobPriority pVal);

		/// <summary>
		/// Specifies the type of event notification to receive
		/// </summary>
		new void SetNotifyFlags([MarshalAs(UnmanagedType.U4)] NotificationType Val);

		/// <summary>
		/// Retrieves the event notification (callback) flags 
		/// you have set for your application.
		/// </summary>
		new void GetNotifyFlags(out uint pVal);

		/// <summary>
		/// Specifies a pointer to your implementation of the 
		/// IBackgroundCopyCallback interface (callbacks). The 
		/// interface receives notification based on the event 
		/// notification flags you set
		/// </summary>
		new void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object Val);

		/// <summary>
		/// Retrieves a pointer to your implementation 
		/// of the IBackgroundCopyCallback interface (callbacks).
		/// </summary>
		new void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object pVal);

		/// <summary>
		/// Specifies the minimum length of time that BITS waits after 
		/// encountering a transient error condition before trying to 
		/// transfer the file
		/// </summary>
		new void SetMinimumRetryDelay(int Seconds);

		/// <summary>
		/// Retrieves the minimum length of time that BITS waits after 
		/// encountering a transient error condition before trying to 
		/// transfer the file
		/// </summary>
		new void GetMinimumRetryDelay(out int Seconds);

		/// <summary>
		/// Specifies the length of time that BITS continues to try to 
		/// transfer the file after encountering a transient error 
		/// condition
		/// </summary>
		new void SetNoProgressTimeout(int Seconds);

		/// <summary>
		/// Retrieves the length of time that BITS continues to try to 
		/// transfer the file after encountering a transient error condition
		/// </summary>
		new void GetNoProgressTimeout(out int Seconds);

		/// <summary>
		/// Retrieves the number of times the job was interrupted by 
		/// network failure or server unavailability
		/// </summary>
		new void GetErrorCount(out uint Errors);

		/// <summary>
		/// Specifies which proxy to use to transfer the files
		/// </summary>
		new void SetProxySettings(ProxyUsage ProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] string ProxyList, [MarshalAs(UnmanagedType.LPWStr)] string ProxyBypassList);

		/// <summary>
		/// Retrieves the proxy settings the job uses to transfer the files
		/// </summary>
		new void GetProxySettings(out ProxyUsage pProxyUsage, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyList, [MarshalAs(UnmanagedType.LPWStr)] out string pProxyBypassList);

		/// <summary>
		/// Changes the ownership of the job to the current user
		/// </summary>
		new void TakeOwnership();

		new void SetNotifyCmdLine([In, MarshalAs(UnmanagedType.LPWStr)] string Program, [In, MarshalAs(UnmanagedType.LPWStr)] string Parameters);

		new void GetNotifyCmdLine([MarshalAs(UnmanagedType.LPWStr)] out string pProgram, [MarshalAs(UnmanagedType.LPWStr)] out string pParameters);

		new void GetReplyProgress([Out] out _BG_JOB_REPLY_PROGRESS pProgress);

		new void GetReplyData([In, Out] ref IntPtr ppBuffer, out ulong pLength);

		new void SetReplyFileName([In, MarshalAs(UnmanagedType.LPWStr)] string ReplyFileName);

		new void GetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] out string pReplyFileName);

		new void SetCredentials([In] ref _BG_AUTH_CREDENTIALS Credentials);

		new void RemoveCredentials(AuthTarget Target, AuthScheme Scheme);

        new void ReplaceRemotePrefix([MarshalAs(UnmanagedType.LPWStr)] string OldPrefix, [MarshalAs(UnmanagedType.LPWStr)] string NewPrefix);

		new void AddFileWithRanges([MarshalAs(UnmanagedType.LPWStr)] string RemoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string LocalName, int RangeCount, IntPtr Ranges);

		new void SetFileACLFlags(CopyFileFlags Flags);

		new void GetFileACLFlags(out CopyFileFlags Flags);

        new void SetClientCertificateByID( 
            CertStoreLocation StoreLocation,
            [In, MarshalAs(UnmanagedType.LPWStr)] string StoreName,
            [In] IntPtr pCertHashBlob);
        
        new void SetClientCertificateByName( 
            CertStoreLocation StoreLocation,
            [In, MarshalAs(UnmanagedType.LPWStr)] string StoreName,
            [In, MarshalAs(UnmanagedType.LPWStr)] string SubjectName);
        
        new void RemoveClientCertificate();
        
        [PreserveSig]
        new int GetClientCertificate( 
            [Out] out CertStoreLocation pStoreLocation,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pStoreName,
		    [Out] out IntPtr ppCertHashBlob,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pSubjectName);
        
        new void SetCustomHeaders([In, MarshalAs(UnmanagedType.LPWStr)] string RequestHeaders);
        
        new void GetCustomHeaders([Out, MarshalAs(UnmanagedType.LPWStr)] out string pRequestHeaders);
        
        new void SetSSLFlags([In] SSL Flags);
        
        new void GetSSLFlags([Out] out SSL pFlags);

		///
		/// Starts definition of IBackgroundCopyJob5
		///
 
		// 
		// Redirect policy: defines how BITS treats HTTP redirects
		// - default = BG_HTTP_REDIRECT_POLICY_SILENT; for any other value
		//   BITS will update IBackgroundCopyFile's RemoteName with the final URL
		// - DISALLOW will cause bits to enter error state when a redirect occurs
		// - values not in BG_HTTP_REDIRECT_POLICY return E_INVALIDARG
		//
		void SetHttpRedirectPolicy( [In]  HttpRedirectPolicy Val );

		void GetHttpRedirectPolicy( [Out] out HttpRedirectPolicy pVal );

		void SetEnableHttpsToHttpRedirect( [In] bool enable );

		void GetEnableHttpsToHttpRedirect( [Out] out bool pEnable);

		//
		// control of peer-caching
		//
		void SetAllowCaching( bool allowed );

		void GetAllowCaching( [Out] out bool pAllowed );

		void SetUsePeerCache( bool allowed );

		void GetUsePeerCache( [Out] out bool pAllowed );

		//
		// inspecting token characteristics
		//
		void GetOwnerIntegrityLevel( [Out] out ulong pLevel );

		void GetOwnerElevationState( [Out] out bool pElevated );


		// Download Timeout

		void SetMaxDownloadTime( int Timeout );

		void GetMaxDownloadTime([Out] out int pTimeout );
	}

	/// <summary>
	/// Use the information in the IBackgroundCopyError interface to 
	/// determine the cause of the error and if the transfer process 
	/// can proceed
	/// </summary>
	[GuidAttribute("19C613A0-FCB8-4F28-81AE-897C3D078F81")]
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImportAttribute()]
	internal interface IBackgroundCopyError 
	{
		/// <summary>
		/// Retrieves the error code and identify the context 
		/// in which the error occurred
		/// </summary>
		void GetError(out ErrorContext pContext, [MarshalAs(UnmanagedType.Error)] out uint pCode);

		/// <summary>
		/// Retrieves an interface pointer to the file object 
		/// associated with the error
		/// </summary>
		void GetFile([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyFile pVal);

		/// <summary>
		/// Retrieves the error text associated with the error
		/// </summary>
		void GetErrorDescription(uint LanguageId, [MarshalAs(UnmanagedType.LPWStr)] out string pErrorDescription);

		/// <summary>
		/// Retrieves a description of the context in which the error occurred
		/// </summary>
		void GetErrorContextDescription(uint LanguageId, [MarshalAs(UnmanagedType.LPWStr)] out string pContextDescription);

		/// <summary>
		/// Retrieves the protocol used to transfer the file
		/// </summary>
		void GetProtocol([MarshalAs(UnmanagedType.LPWStr)] out string pProtocol);
	}

	/// <summary>
	/// Use the IEnumBackgroundCopyJobs interface to enumerate the list 
	/// of jobs in the transfer queue. To get an IEnumBackgroundCopyJobs 
	/// interface pointer, call the IBackgroundCopyManager::EnumJobs method
	/// </summary>
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	[GuidAttribute("1AF4F612-3B71-466F-8F58-7B6F73AC57AD")]
	[ComImportAttribute()]
	internal  interface IEnumBackgroundCopyJobs 
	{
		/// <summary>
		/// Retrieves a specified number of items in the enumeration sequence
		/// </summary>
		void Next(uint celt, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob rgelt, out uint pceltFetched);

		/// <summary>
		/// Skips a specified number of items in the enumeration sequence
		/// </summary>
		void Skip(uint celt);

		/// <summary>
		/// Resets the enumeration sequence to the beginning.
		/// </summary>
		void Reset();

		/// <summary>
		/// Creates another enumerator that contains the same 
		/// enumeration state as the current one
		/// </summary>
		void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyJobs ppenum);

		/// <summary>
		/// Returns the number of items in the enumeration
		/// </summary>
		void GetCount(out uint puCount);
	}

	/// <summary>
	/// Use the IEnumBackgroundCopyFiles interface to enumerate the files 
	/// that a job contains. To get an IEnumBackgroundCopyFiles interface 
	/// pointer, call the IBackgroundCopyJob::EnumFiles method
	/// </summary>
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	[GuidAttribute("CA51E165-C365-424C-8D41-24AAA4FF3C40")]
	[ComImportAttribute()]
	internal  interface IEnumBackgroundCopyFiles 
	{
		/// <summary>
		/// Retrieves a specified number of items in the enumeration sequence
		/// </summary>
		void Next(uint celt, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyFile rgelt, out uint pceltFetched);
		
		/// <summary>
		/// Skips a specified number of items in the enumeration sequence
		/// </summary>
		void Skip(uint celt);

		/// <summary>
		/// Resets the enumeration sequence to the beginning
		/// </summary>
		void Reset();

		/// <summary>
		/// Creates another enumerator that contains the same 
		/// enumeration state as the current enumerator
		/// </summary>
		void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles ppenum);

		/// <summary>
		/// Retrieves the number of items in the enumeration
		/// </summary>
		void GetCount(out uint puCount);
	}

	/// <summary>
	///  The IBackgroundCopyFile interface contains information about a file 
	///  that is part of a job. For example, you can use the interfaces methods
	///  to retrieve the local and remote names of the file and transfer progress
	///  information
	/// </summary>
	[GuidAttribute("01B7BD23-FB88-4A77-8490-5891D3E4653A")]
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImportAttribute()]
	internal interface IBackgroundCopyFile 
	{
		/// <summary>
		/// Retrieves the remote name of the file
		/// </summary>
		void GetRemoteName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Retrieves the local name of the file
		/// </summary>
		void GetLocalName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);
		
		/// <summary>
		/// Retrieves the progress of the file transfer
		/// </summary>
		void GetProgress(out _BG_FILE_PROGRESS pVal);
	}

	/// <summary>
	///  The IBackgroundCopyFile interface contains information about a file 
	///  that is part of a job. For example, you can use the interfaces methods
	///  to retrieve the local and remote names of the file and transfer progress
	///  information
	/// </summary>
	[ComImport]
	[GuidAttribute("83e81b93-0873-474d-8a8c-f2018b1a939c")]
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IBackgroundCopyFile2 : IBackgroundCopyFile
	{
		/// <summary>
		/// Retrieves the remote name of the file
		/// </summary>
		new void GetRemoteName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Retrieves the local name of the file
		/// </summary>
		new void GetLocalName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Retrieves the progress of the file transfer
		/// </summary>
		new void GetProgress(out _BG_FILE_PROGRESS pVal);

		///
		/// Starts definition of IBackgroundCopyFile2
		///
        void GetFileRanges(ref int RangeCount, out IntPtr Ranges);
        
        void SetRemoteName([MarshalAs(UnmanagedType.LPWStr)] string Val);
	}

	[ComImport]
	[GuidAttribute("659cdeaa-489e-11d9-a9cd-000d56965251")]
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IBackgroundCopyFile3 : IBackgroundCopyFile2
	{
		/// <summary>
		/// Retrieves the remote name of the file
		/// </summary>
		new void GetRemoteName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Retrieves the local name of the file
		/// </summary>
		new void GetLocalName([MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		/// <summary>
		/// Retrieves the progress of the file transfer
		/// </summary>
		new void GetProgress(out _BG_FILE_PROGRESS pVal);

        new void GetFileRanges(ref int RangeCount, out IntPtr Ranges);
        
        new void SetRemoteName([MarshalAs(UnmanagedType.LPWStr)] string Val);

		///
		/// Starts definition of IBackgroundCopyFile3
		///

		//
		// Get the name of the temporary file, allowing access to data before 
		// the job is complete.
		//
		void GetTemporaryName( [Out, MarshalAs(UnmanagedType.LPWStr)] out string pFilename );

		//
		// Calling SetValidationState(TRUE) allows the data to be shared with peers,
		// if peer-caching is otherwise enabled for this job.
		// 
		// Calling SetValidationState(FALSE) triggers another download attempt if the 
		// file was downloaded from a peer; otherwise, it puts the job in ERROR state.
		//
		void SetValidationState([In] int state);

		//
		// Retrieves the current validation state of this file.
		//
		void GetValidationState([Out] out int pState);

		//
		// *pVal is set to TRUE if any part of the file was downloaded from a peer server.
		//
		void DownloadedFromPeer( [Out] out bool pVal );
	}

	[ComImport]
	[Guid("97EA99C7-0186-4AD4-8DF9-C5B4E0ED6B22")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IBackgroundCopyCallback
	{
		// Methods
		void JobTransferred([In, MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob);

		void JobError([In, MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob, [In, MarshalAs(UnmanagedType.Interface)] IBackgroundCopyError pError);

		void JobModification([In, MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob, [In] uint dwReserved);
	}

	[ComImport]
	[Guid("659cdeac-489e-11d9-a9cd-000d56965251")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IBackgroundCopyCallback2 : IBackgroundCopyCallback
	{
		// Methods
		new void JobTransferred([In, MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob);

		new void JobError([In, MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob, [In, MarshalAs(UnmanagedType.Interface)] IBackgroundCopyError pError);

		new void JobModification([In, MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob, [In] uint dwReserved);

		///
		/// Starts definition of IBackgroundCopyCallback2
		///

		//
		// A file has been transferred.
		//
		[PreserveSig]
		void FileTransferred([In, MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob pJob, [In, MarshalAs(UnmanagedType.Interface)] IBackgroundCopyFile pFile);
	}

	[ComImport]
	[Guid("df650878-b4ab-46f8-b195-b66def0973bd")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IGatewayMgr
	{
		void GetGateways([Out] out IntPtr Info);
	}

	[ComImport]
	[Guid("659cdea0-489e-11d9-a9cd-000d56965251")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IBackgroundCopyCacheRecord
	{
		void GetOriginUrl([Out, MarshalAs(UnmanagedType.LPWStr)] out string pVal);

		void GetFileRanges([Out] out int pRangeCount, [Out] out IntPtr ppRanges );

		void GetInformation([Out] out _BG_CACHE_RECORD_INFO pInfo );
	};

	[ComImport]
	[Guid("659cdea4-489e-11d9-a9cd-000d56965251")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumBackgroundCopyCacheRecords
	{
		/// <summary>
		/// Retrieves a specified number of items in the enumeration sequence
		/// </summary>
		void Next(uint celt, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyCacheRecord rgelt, out uint pceltFetched);

		/// <summary>
		/// Skips a specified number of items in the enumeration sequence
		/// </summary>
		void Skip(uint celt);

		/// <summary>
		/// Resets the enumeration sequence to the beginning.
		/// </summary>
		void Reset();

		/// <summary>
		/// Creates another enumerator that contains the same 
		/// enumeration state as the current one
		/// </summary>
		void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyCacheRecords ppenum);

		/// <summary>
		/// Returns the number of items in the enumeration
		/// </summary>
		void GetCount(out uint puCount);
	}

	[ComImport]
	[Guid("659cde9e-489e-11d9-a9cd-000d56965251")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IBackgroundCopyCache
	{
		/**
		 * control of caching policy in general
		 */
		void GetCacheLocation( [Out, MarshalAs(UnmanagedType.LPWStr)] out string pDirectory );
		void SetCacheLocation( [MarshalAs(UnmanagedType.LPWStr)] string Directory );

		void GetCacheLimit( [Out] out ulong pBytes );
		void SetCacheLimit( ulong Bytes );

		void GetCacheExpirationTime( [Out] out ulong pSeconds );
		void SetCacheExpirationTime( ulong Seconds );

		void GetAllowCaching( [Out] out bool pAllow );
		void SetAllowCaching( bool Allow );

		void GetUsePeerCache( [Out] out bool pAllow );
		void SetUsePeerCache( bool Allow );

		/**
		 * cache record management
		 */
		void EnumRecords( [Out, MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyCacheRecords ppEnum );

		void GetRecord( [In] ref Guid id, [Out, MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyCacheRecord ppRecord );

		void ClearRecords();

		void DeleteRecord( [In] ref Guid id );

		void DeleteUrl([In, MarshalAs(UnmanagedType.LPWStr)] string url);
	}

	[ComImport]
	[Guid("659cdea2-489e-11d9-a9cd-000d56965251")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IBackgroundCopyNeighbor
	{
		void GetPrincipalName( [Out, MarshalAs(UnmanagedType.LPWStr)] out string pName);

		void IsAuthenticated( [Out] out bool pAuth );

		void IsAvailable( [Out] out bool pOnline );
	};

	[ComImport]
	[Guid("659cdea5-489e-11d9-a9cd-000d56965251")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumBackgroundCopyNeighbors
	{
		/// <summary>
		/// Retrieves a specified number of items in the enumeration sequence
		/// </summary>
		void Next(uint celt, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyNeighbor rgelt, out uint pceltFetched);

		/// <summary>
		/// Skips a specified number of items in the enumeration sequence
		/// </summary>
		void Skip(uint celt);

		/// <summary>
		/// Resets the enumeration sequence to the beginning.
		/// </summary>
		void Reset();

		/// <summary>
		/// Creates another enumerator that contains the same 
		/// enumeration state as the current one
		/// </summary>
		void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyNeighbors ppenum);

		/// <summary>
		/// Returns the number of items in the enumeration
		/// </summary>
		void GetCount(out uint puCount);
	}

	[ComImport]
	[Guid("659cde9f-489e-11d9-a9cd-000d56965251")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	interface IBackgroundCopyNeighborhood
	{
		void EnumNeighbors( [Out, MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyNeighbors ppEnum);

		void ClearNeighbors();

		void DiscoverNeighbors();
	};

	/// <summary>
	/// The JobState enumeration type defines constant values for the 
	/// different states of a job
	/// </summary>
	public enum JobState 
	{
		/// <summary>
		/// The Job instance is not in the job queue or not part of a Jobs collection
		/// </summary>
		Inactive = -1,
		/// <summary>
		/// Specifies that the job is in the queue and waiting to run. 
		/// If a user logs off while their job is transferring, the job 
		/// transitions to the queued state
		/// </summary>
		Queued = 0,

		/// <summary>
		/// Specifies that BITS is trying to connect to the server. If the 
		/// connection succeeds, the state of the job becomes 
		/// BG_JOB_STATE_TRANSFERRING; otherwise, the state becomes 
		/// BG_JOB_STATE_TRANSIENT_ERROR
		/// </summary>
		Connecting = 1,

		/// <summary>
		/// Specifies that BITS is transferring data for the job
		/// </summary>
		Transferring = 2,

		/// <summary>
		/// Specifies that the job is suspended (paused)
		/// </summary>
		Suspended = 3,

		/// <summary>
		/// Specifies that a non-recoverable error occurred (the service is 
		/// unable to transfer the file). When the error can be corrected, 
		/// such as an access-denied error, call the IBackgroundCopyJob::Resume 
		/// method after the error is fixed. However, if the error cannot be 
		/// corrected, call the IBackgroundCopyJob::Cancel method to cancel 
		/// the job, or call the IBackgroundCopyJob::Complete method to accept 
		/// the portion of a download job that transferred successfully.
		/// </summary>
		Error = 4,

		/// <summary>
		/// Specifies that a recoverable error occurred. The service tries to 
		/// recover from the transient error until the retry time value that 
		/// you specify using the IBackgroundCopyJob::SetNoProgressTimeout method 
		/// expires. If the retry time expires, the job state changes to 
		/// BG_JOB_STATE_ERROR
		/// </summary>
		TransientError = 5,

		/// <summary>
		/// Specifies that your job was successfully processed
		/// </summary>
		Transferred = 6,

		/// <summary>
		/// Specifies that you called the IBackgroundCopyJob::Complete method 
		/// to acknowledge that your job completed successfully
		/// </summary>
		Acknowledged = 7,

		/// <summary>
		/// Specifies that you called the IBackgroundCopyJob::Cancel method to 
		/// cancel the job (remove the job from the transfer queue)
		/// </summary>
		Cancelled = 8,
	}

	/// <summary>
	/// The JobType enumeration type defines constant values that you 
	/// use to specify the type of transfer job, such as download
	/// </summary>
	public enum JobType 
	{
		/// <summary>
		/// Specifies that the job downloads files to the client
		/// </summary>
		Download = 0,

		/// <summary>
		/// Specifies that the job uploads a file to the server.
		/// </summary>
		Upload = 1,

		/// <summary>
		/// BG_JOB_TYPE_UPLOAD_REPLY
		/// </summary>
		UploadReply = 2,
	}

	[Flags]
	internal enum NotificationType : uint
	{
		JobTransferred = 0x0001,
		JobError = 0x0002,
		Disable = 0x0004,
		JobModification = 0x0008,
		FileTransferred = 0x0010,
		All = JobTransferred | JobError | JobModification,
		All_3_0 = All | FileTransferred,
	}

	/// <summary>
	/// The ProxyUsage enumeration type defines constant values 
	/// that you use to specify which proxy to use for file transfers
	/// </summary>
	public enum ProxyUsage 
	{
		/// <summary>
		/// Use the proxy and proxy bypass list settings defined by each 
		/// user to transfer files
		/// </summary>
		Preconfig = 0,

		/// <summary>
		/// Do not use a proxy to transfer files
		/// </summary>
		NoProxy = 1,

		/// <summary>
		/// Use the application's proxy and proxy bypass list to transfer files
		/// </summary>
		Override = 2,
		
		/// <summary>
		/// Automatically detect proxy settings. BITS detects proxy settings for each file in the job.
		/// </summary>
		AutoDetect  = 3
	}

	/// <summary>
	/// The JobPriority enumeration type defines the constant values 
	/// that you use to specify the priority level of the job
	/// </summary>
	public enum JobPriority 
	{
		/// <summary>
		/// Transfers the job in the foreground
		/// </summary>
		Foreground = 0,

		/// <summary>
		/// Transfers the job in the background. This is the highest background 
		/// priority level. 
		/// </summary>
		High = 1,

		/// <summary>
		/// Transfers the job in the background. This is the default priority 
		/// level for a job
		/// </summary>
		Normal = 2,

		/// <summary>
		/// Transfers the job in the background. This is the lowest background 
		/// priority level
		/// </summary>
		Low = 3,
	}

	/// <summary>
	/// https://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_auth_scheme.asp
	/// </summary>
	public enum AuthScheme
	{
		/// <summary>
		/// Basic is a scheme in which the user name and password are sent in clear-text to the server or proxy.
		/// </summary>
		Basic = 1,
		/// <summary>
		/// Digest is a challenge-response scheme that uses a server-specified data string for the challenge.
		/// </summary>
		Digest = 2,
		/// <summary>
		/// Windows NT LAN Manager (NTLM) is a challenge-response scheme that uses the credentials of the user for authentication in a Windows network environment.
		/// </summary>
		NTLM = 3,
		/// <summary>
		/// Simple and Protected Negotiation protocol (Snego) is a challenge-response scheme that negotiates with the server or proxy to determine which scheme to use for authentication. Examples are the Kerberos protocol and NTLM.
		/// </summary>
		Negotiate = 4,
		/// <summary>
		/// Passport is a centralized authentication service provided by Microsoft that offers a single logon for member sites.
		/// </summary>
		Passport = 5
	}
 
	/// <summary>
	/// https://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_auth_target.asp
	/// </summary>
	public enum AuthTarget
	{
		/// <summary>
		/// Use credentials for server requests.
		/// </summary>
		Server = 1,
		/// <summary>
		/// Use credentials for proxy requests.
		/// </summary>
		Proxy = 2,
	}

	/// <summary>
	/// The BG_ERROR_CONTEXT enumeration type defines the constant values 
	/// that specify the context in which the error occurred
	/// </summary>
	public enum ErrorContext 
	{
		/// <summary>
		/// An error has not occurred
		/// </summary>
		None = 0,

		/// <summary>
		/// The error context is unknown
		/// </summary>
		Unknown = 1,

		/// <summary>
		/// The transfer queue manager generated the error
		/// </summary>
		GeneralQueueManager = 2,

		/// <summary>
		/// The error was generated while the queue manager was 
		/// notifying the client of an event
		/// </summary>
		QueueManagerNotification = 3,

		/// <summary>
		/// The error was related to the specified local file. For example, 
		/// permission was denied or the volume was unavailable
		/// </summary>
		LocalFile = 4,

		/// <summary>
		/// The error was related to the specified remote file. 
		/// For example, the URL is not accessible
		/// </summary>
		RemoteFile = 5,

		/// <summary>
		/// The transport layer generated the error. These errors are general 
		/// transport failures; errors not specific to the remote file
		/// </summary>
		GeneralTransport = 6,
	}

	/// <summary>
	/// https://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob3_getfileaclflags.asp
	/// </summary>
	[Flags]
	public enum CopyFileFlags : int
	{
		/// <summary>
		/// To indicate no ACL flags are set
		/// </summary>
		None = 0,
		/// <summary>
		/// If set, the file's owner information is maintained. Otherwise, the job's owner becomes the owner of the file.
		/// </summary>
		Owner = 1,
		/// <summary>
		/// If set, the file's group information is maintained. Otherwise, BITS uses the job owner's primary group to assign the group information to the file.
		/// </summary>
		Group = 2,
		/// <summary>
		/// If set, BITS copies the explicit ACEs from the source file and inheritable ACEs from the destination parent folder. Otherwise, BITS copies the inheritable ACEs from the destination parent folder. If the parent folder does not contain inheritable ACEs, BITS uses the default DACL from the account.
		/// </summary>
		DACL = 4,
		/// <summary>
		/// If set, BITS copies the explicit ACEs from the source file and inheritable ACEs from the destination parent folder. Otherwise, BITS copies the inheritable ACEs from the destination parent folder.
		/// </summary>
		SACL = 8,
		/// <summary>
		/// If set, BITS copies the owner and ACL information. This is the same as setting all the flags individually.
		/// </summary>
		All = 15
	}
	
	/// <summary>
	/// BITS error codes
	/// </summary>
	public enum BG_HRESULT : uint
	{
		///  <summary>
		///
		/// MessageId: BG_E_NOT_FOUND
		///
		/// MessageText:
		///
		///  The requested job was not found.
		///
		///  </summary>
		BG_E_NOT_FOUND = 0x80200001,

		///  <summary>
		///
		/// MessageId: BG_E_INVALID_STATE
		///
		/// MessageText:
		///
		///  The requested action is not allowed in the current job state. The job might have been canceled or completed transferring. It is in a read-only state now.
		///
		///  </summary>
		BG_E_INVALID_STATE = 0x80200002,

		///  <summary>
		///
		/// MessageId: BG_E_EMPTY
		///
		/// MessageText:
		///
		///  There are no files attached to this job. Attach files to the job, and then try again.
		///
		///  </summary>
		BG_E_EMPTY = 0x80200003,

		///  <summary>
		///
		/// MessageId: BG_E_FILE_NOT_AVAILABLE
		///
		/// MessageText:
		///
		///  No file is available because no URL generated an error.
		///
		///  </summary>
		BG_E_FILE_NOT_AVAILABLE = 0x80200004,

		///  <summary>
		///
		/// MessageId: BG_E_PROTOCOL_NOT_AVAILABLE
		///
		/// MessageText:
		///
		///  No protocol is available because no URL generated an error.
		///
		///  </summary>
		BG_E_PROTOCOL_NOT_AVAILABLE = 0x80200005,

		///  <summary>
		///
		/// MessageId: BG_S_ERROR_CONTEXT_NONE
		///
		/// MessageText:
		///
		///  No errors have occurred.
		///
		///  </summary>
		BG_S_ERROR_CONTEXT_NONE = 0x00200006,

		///  <summary>
		///
		/// MessageId: BG_E_ERROR_CONTEXT_UNKNOWN
		///
		/// MessageText:
		///
		///  The error occurred in an unknown location.
		///
		///  </summary>
		BG_E_ERROR_CONTEXT_UNKNOWN = 0x80200007,

		///  <summary>
		///
		/// MessageId: BG_E_ERROR_CONTEXT_GENERAL_QUEUE_MANAGER
		///
		/// MessageText:
		///
		///  The error occurred in the Background Intelligent Transfer Service (BITS) queue manager.
		///
		///  </summary>
		BG_E_ERROR_CONTEXT_GENERAL_QUEUE_MANAGER = 0x80200008,

		///  <summary>
		///
		/// MessageId: BG_E_ERROR_CONTEXT_LOCAL_FILE
		///
		/// MessageText:
		///
		///  The error occurred while the local file was being processed. Verify that the file is not in use, and then try again.
		///
		///  </summary>
		BG_E_ERROR_CONTEXT_LOCAL_FILE = 0x80200009,

		///  <summary>
		///
		/// MessageId: BG_E_ERROR_CONTEXT_REMOTE_FILE
		///
		/// MessageText:
		///
		///  The error occurred while the remote file was being processed.
		///
		///  </summary>
		BG_E_ERROR_CONTEXT_REMOTE_FILE = 0x8020000A,

		///  <summary>
		///
		/// MessageId: BG_E_ERROR_CONTEXT_GENERAL_TRANSPORT
		///
		/// MessageText:
		///
		///  The error occurred in the transport layer. The client could not connect to the server.
		///
		///  </summary>
		BG_E_ERROR_CONTEXT_GENERAL_TRANSPORT = 0x8020000B,

		///  <summary>
		///
		/// MessageId: BG_E_ERROR_CONTEXT_QUEUE_MANAGER_NOTIFICATION
		///
		/// MessageText:
		///
		///  The error occurred while the notification callback was being processed. Background Intelligent Transfer Service (BITS) will try again later.
		///
		///  </summary>
		BG_E_ERROR_CONTEXT_QUEUE_MANAGER_NOTIFICATION = 0x8020000C,

		///  <summary>
		///
		/// MessageId: BG_E_DESTINATION_LOCKED
		///
		/// MessageText:
		///
		///  The destination file system volume is not available. Verify that another program, such as CheckDisk, is not running, which would lock the volume. When the volume is available, Background Intelligent Transfer Service (BITS) will try again.
		///
		///  </summary>
		BG_E_DESTINATION_LOCKED = 0x8020000D,

		///  <summary>
		///
		/// MessageId: BG_E_VOLUME_CHANGED
		///
		/// MessageText:
		///
		///  The destination volume has changed. If the disk is removable, it might have been replaced with a different disk. Reinsert the original disk and resume the job.
		///
		///  </summary>
		BG_E_VOLUME_CHANGED = 0x8020000E,

		///  <summary>
		///
		/// MessageId: BG_E_ERROR_INFORMATION_UNAVAILABLE
		///
		/// MessageText:
		///
		///  No errors have occurred.
		///
		///  </summary>
		BG_E_ERROR_INFORMATION_UNAVAILABLE = 0x8020000F,

		///  <summary>
		///
		/// MessageId: BG_E_NETWORK_DISCONNECTED
		///
		/// MessageText:
		///
		///  There are currently no active network connections. Background Intelligent Transfer Service (BITS) will try again when an adapter is connected.
		///
		///  </summary>
		BG_E_NETWORK_DISCONNECTED = 0x80200010,

		///  <summary>
		///
		/// MessageId: BG_E_MISSING_FILE_SIZE
		///
		/// MessageText:
		///
		///  The server did not return the file size. The URL might point to dynamic content. The Content-Length header is not available in the server's HTTP reply.
		///
		///  </summary>
		BG_E_MISSING_FILE_SIZE = 0x80200011,

		///  <summary>
		///
		/// MessageId: BG_E_INSUFFICIENT_HTTP_SUPPORT
		///
		/// MessageText:
		///
		///  The server does not support HTTP 1.1.
		///
		///  </summary>
		BG_E_INSUFFICIENT_HTTP_SUPPORT = 0x80200012,

		///  <summary>
		///
		/// MessageId: BG_E_INSUFFICIENT_RANGE_SUPPORT
		///
		/// MessageText:
		///
		///  The server does not support the necessary HTTP protocol. Background Intelligent Transfer Service (BITS) requires that the server support the Range protocol header.
		///
		///  </summary>
		BG_E_INSUFFICIENT_RANGE_SUPPORT = 0x80200013,

		///  <summary>
		///
		/// MessageId: BG_E_REMOTE_NOT_SUPPORTED
		///
		/// MessageText:
		///
		///  Background Intelligent Transfer Service (BITS) cannot be used remotely.
		///
		///  </summary>
		BG_E_REMOTE_NOT_SUPPORTED = 0x80200014,

		///  <summary>
		///
		/// MessageId: BG_E_NEW_OWNER_DIFF_MAPPING
		///
		/// MessageText:
		///
		///  The drive mapping for the job is different for the current owner than for the previous owner. Use a UNC path instead.
		///
		///  </summary>
		BG_E_NEW_OWNER_DIFF_MAPPING = 0x80200015,

		///  <summary>
		///
		/// MessageId: BG_E_NEW_OWNER_NO_FILE_ACCESS
		///
		/// MessageText:
		///
		///  The new owner has insufficient access to the local files for the job. The new owner might not have permissions to access the job files. Verify that the new owner has sufficient permissions, and then try again.
		///
		///  </summary>
		BG_E_NEW_OWNER_NO_FILE_ACCESS = 0x80200016,

		///  <summary>
		///
		/// MessageId: BG_S_PARTIAL_COMPLETE
		///
		/// MessageText:
		///
		///  Some of the transferred files were deleted because they were incomplete.
		///
		///  </summary>
		BG_S_PARTIAL_COMPLETE = 0x00200017,

		///  <summary>
		///
		/// MessageId: BG_E_PROXY_LIST_TOO_LARGE
		///
		/// MessageText:
		///
		///  The HTTP proxy list cannot be longer than 32,000 characters. Try again with a shorter proxy list.
		///
		///  </summary>
		BG_E_PROXY_LIST_TOO_LARGE = 0x80200018,

		///  <summary>
		///
		/// MessageId: BG_E_PROXY_BYPASS_LIST_TOO_LARGE
		///
		/// MessageText:
		///
		///  The HTTP proxy bypass list cannot be longer than 32,000 characters. Try again with a shorter bypass proxy list.
		///
		///  </summary>
		BG_E_PROXY_BYPASS_LIST_TOO_LARGE = 0x80200019,

		///  <summary>
		///
		/// MessageId: BG_S_UNABLE_TO_DELETE_FILES
		///
		/// MessageText:
		///
		///  Some of the temporary files could not be deleted. Check the system event log for the complete list of files that could not be deleted.
		///
		///  </summary>
		BG_S_UNABLE_TO_DELETE_FILES = 0x0020001A,

		///  <summary>
		///
		/// MessageId: BG_E_INVALID_SERVER_RESPONSE
		///
		/// MessageText:
		///
		///  The server's response was not valid. The server was not following the defined protocol. Resume the job, and then Background Intelligent Transfer Service (BITS) will try again.
		///
		///  </summary>
		BG_E_INVALID_SERVER_RESPONSE = 0x8020001B,

		///  <summary>
		///
		/// MessageId: BG_E_TOO_MANY_FILES
		///
		/// MessageText:
		///
		///  No more files can be added to this job.
		///
		///  </summary>
		BG_E_TOO_MANY_FILES = 0x8020001C,

		///  <summary>
		///
		/// MessageId: BG_E_LOCAL_FILE_CHANGED
		///
		/// MessageText:
		///
		///  The local file was changed during the transfer. Recreate the job, and then try to transfer it again.
		///
		///  </summary>
		BG_E_LOCAL_FILE_CHANGED = 0x8020001D,

		///  <summary>
		///
		/// MessageId: BG_E_ERROR_CONTEXT_REMOTE_APPLICATION
		///
		/// MessageText:
		///
		///  The program on the remote server reported the error.
		///
		///  </summary>
		BG_E_ERROR_CONTEXT_REMOTE_APPLICATION = 0x8020001E,

		///  <summary>
		///
		/// MessageId: BG_E_SESSION_NOT_FOUND
		///
		/// MessageText:
		///
		///  The specified session could not be found on the server. Background Intelligent Transfer Service (BITS) will try again.
		///
		///  </summary>
		BG_E_SESSION_NOT_FOUND = 0x8020001F,

		///  <summary>
		///
		/// MessageId: BG_E_TOO_LARGE
		///
		/// MessageText:
		///
		///  The job is too large for the server to accept. This job might exceed a job size limit set by the server administrator. Reduce the size of the job, and then try again.
		///
		///  </summary>
		BG_E_TOO_LARGE = 0x80200020,

		///  <summary>
		///
		/// MessageId: BG_E_STRING_TOO_LONG
		///
		/// MessageText:
		///
		///  The specified string is too long.
		///
		///  </summary>
		BG_E_STRING_TOO_LONG = 0x80200021,

		///  <summary>
		///
		/// MessageId: BG_E_CLIENT_SERVER_PROTOCOL_MISMATCH
		///
		/// MessageText:
		///
		///  The client and server versions of Background Intelligent Transfer Service (BITS) are incompatible.
		///
		///  </summary>
		BG_E_CLIENT_SERVER_PROTOCOL_MISMATCH = 0x80200022,

		///  <summary>
		///
		/// MessageId: BG_E_SERVER_EXECUTE_ENABLE
		///
		/// MessageText:
		///
		///  Scripting OR execute permissions are enabled on the IIS virtual directory associated with the job. To upload files to the virtual directory, disable the scripting and execute permissions on the virtual directory.
		///
		///  </summary>
		BG_E_SERVER_EXECUTE_ENABLE = 0x80200023,

		///  <summary>
		///
		/// MessageId: BG_E_NO_PROGRESS
		///
		/// MessageText:
		///
		///  The job is not making headway.  The server may be misconfigured.  Background Intelligent Transfer Service (BITS) will try again later.
		///
		///  </summary>
		BG_E_NO_PROGRESS = 0x80200024,

		///  <summary>
		///
		/// MessageId: BG_E_USERNAME_TOO_LARGE
		///
		/// MessageText:
		///
		///  The user name cannot be longer than 300 characters. Try again with a shorter name.
		///
		///  </summary>
		BG_E_USERNAME_TOO_LARGE = 0x80200025,

		///  <summary>
		///
		/// MessageId: BG_E_PASSWORD_TOO_LARGE
		///
		/// MessageText:
		///
		///  The password cannot be longer than 300 characters. Try again with a shorter password.
		///
		///  </summary>
		BG_E_PASSWORD_TOO_LARGE = 0x80200026,

		///  <summary>
		///
		/// MessageId: BG_E_INVALID_AUTH_TARGET
		///
		/// MessageText:
		///
		///  The authentication target specified in the credentials is not defined.
		///
		///  </summary>
		BG_E_INVALID_AUTH_TARGET = 0x80200027,

		///  <summary>
		///
		/// MessageId: BG_E_INVALID_AUTH_SCHEME
		///
		/// MessageText:
		///
		///  The authentication scheme specified in the credentials is not defined.
		///
		///  </summary>
		BG_E_INVALID_AUTH_SCHEME = 0x80200028,

		///  <summary>
		///
		/// MessageId: BG_E_FILE_NOT_FOUND
		///
		/// MessageText:
		///
		///  The specified file name does not match any of the files in the job.
		///
		///  </summary>
		BG_E_FILE_NOT_FOUND = 0x80200029,

		///  <summary>
		///
		/// MessageId: BG_S_PROXY_CHANGED
		///
		/// MessageText:
		///
		///  The proxy server was changed.
		///
		///  </summary>
		BG_S_PROXY_CHANGED = 0x0020002A,

		///  <summary>
		///
		/// MessageId: BG_E_INVALID_RANGE
		///
		/// MessageText:
		///
		///  The requested byte range extends beyond the end of the web page.  Use byte ranges that are wholly within the page.
		///
		///  </summary>
		BG_E_INVALID_RANGE = 0x8020002B,

		///  <summary>
		///
		/// MessageId: BG_E_OVERLAPPING_RANGES
		///
		/// MessageText:
		///
		///  The list of byte ranges contains some overlapping ranges, which are not supported.
		///
		///  </summary>
		BG_E_OVERLAPPING_RANGES = 0x8020002C,

		///  <summary>
		///
		/// MessageId: BG_E_CONNECT_FAILURE
		///
		/// MessageText:
		///
		///  A connection could not be established.
		///
		///  </summary>
		BG_E_CONNECT_FAILURE = 0x8020002D,

		///  <summary>
		///
		/// MessageId: BG_E_CONNECTION_CLOSED
		///
		/// MessageText:
		///
		///  The connection was prematurely closed.
		///
		///  </summary>
		BG_E_CONNECTION_CLOSED = 0x8020002E,

		///  <summary>
		///
		/// MessageId: BG_E_KEEP_ALIVE_FAILURE
		///
		/// MessageText:
		///
		///  The connection for a request that specifies the Keep-alive header was closed unexpectedly.
		///
		///  </summary>
		BG_E_KEEP_ALIVE_FAILURE = 0x8020002F,

		///  <summary>
		///
		/// MessageId: BG_E_MESSAGE_LENGTH_LIMIT_EXCEEDED
		///
		/// MessageText:
		///
		///  A message was received that exceeded the specified limit when sending a request or receiving a response from the server.
		///
		///  </summary>
		BG_E_MESSAGE_LENGTH_LIMIT_EXCEEDED = 0x80200030,

		///  <summary>
		///
		/// MessageId: BG_E_NAME_RESOLUTION_FAILURE
		///
		/// MessageText:
		///
		///  The host name could not be found.
		///
		///  </summary>
		BG_E_NAME_RESOLUTION_FAILURE = 0x80200031,

		///  <summary>
		///
		/// MessageId: BG_E_PENDING
		///
		/// MessageText:
		///
		///  An internal asynchronous request is pending.
		///
		///  </summary>
		BG_E_PENDING = 0x80200032,

		///  <summary>
		///
		/// MessageId: BG_E_PIPELINE_FAILURE
		///
		/// MessageText:
		///
		///  BG_E_PIPELINE_FAILURE
		///
		///  </summary>
		BG_E_PIPELINE_FAILURE = 0x80200033,

		///  <summary>
		///
		/// MessageId: BG_E_PROTOCOL_ERROR
		///
		/// MessageText:
		///
		///  The response received from the server was complete but indicated a protocol-level error.
		///
		///  </summary>
		BG_E_PROTOCOL_ERROR = 0x80200034,

		///  <summary>
		///
		/// MessageId: BG_E_PROXY_NAME_RESOLUTION_FAILURE
		///
		/// MessageText:
		///
		///  The proxy name could not be found.
		///
		///  </summary>
		BG_E_PROXY_NAME_RESOLUTION_FAILURE = 0x80200035,

		///  <summary>
		///
		/// MessageId: BG_E_RECEIVE_FAILURE
		///
		/// MessageText:
		///
		///  A complete response was not received from the server.
		///
		///  </summary>
		BG_E_RECEIVE_FAILURE = 0x80200036,

		///  <summary>
		///
		/// MessageId: BG_E_REQUEST_CANCELED
		///
		/// MessageText:
		///
		///  The request was canceled.
		///
		///  </summary>
		BG_E_REQUEST_CANCELED = 0x80200037,

		///  <summary>
		///
		/// MessageId: BG_E_SECURE_CHANNEL_FAILURE
		///
		/// MessageText:
		///
		///  An error occurred while establishing a connection using SSL.
		///
		///  </summary>
		BG_E_SECURE_CHANNEL_FAILURE = 0x80200038,

		///  <summary>
		///
		/// MessageId: BG_E_SEND_FAILURE
		///
		/// MessageText:
		///
		///  A complete request could not be sent to the remote server.
		///
		///  </summary>
		BG_E_SEND_FAILURE = 0x80200039,

		///  <summary>
		///
		/// MessageId: BG_E_SERVER_PROTOCOL_VIOLATION
		///
		/// MessageText:
		///
		///  The server response was not valid.
		///
		///  </summary>
		BG_E_SERVER_PROTOCOL_VIOLATION = 0x8020003A,

		///  <summary>
		///
		/// MessageId: BG_E_TIMEOUT
		///
		/// MessageText:
		///
		///  The operation exceeded the time limit.
		///
		///  </summary>
		BG_E_TIMEOUT = 0x8020003B,

		///  <summary>
		///
		/// MessageId: BG_E_TRUST_FAILURE
		///
		/// MessageText:
		///
		///  A server certificate could not be validated.
		///
		///  </summary>
		BG_E_TRUST_FAILURE = 0x8020003C,

		///  <summary>
		///
		/// MessageId: BG_E_UNKNOWN_ERROR
		///
		/// MessageText:
		///
		///  A unknown error occured.
		///
		///  </summary>
		BG_E_UNKNOWN_ERROR = 0x8020003D,

		///  <summary>
		///
		/// MessageId: BG_E_BLOCKED_BY_POLICY
		///
		/// MessageText:
		///
		///  Group Policy settings prevent background jobs from running at this time.
		///
		///  </summary>
		BG_E_BLOCKED_BY_POLICY = 0x8020003E,

		///  <summary>
		///
		/// MessageId: BG_E_INVALID_PROXY_INFO
		///
		/// MessageText:
		///
		///  The supplied proxy server or bypass list is invalid.
		///
		///  </summary>
		BG_E_INVALID_PROXY_INFO = 0x8020003F,

		///  <summary>
		///
		/// MessageId: BG_E_INVALID_CREDENTIALS
		///
		/// MessageText:
		///
		///  The format of the supplied security credentials is invalid.
		///
		///  </summary>
		BG_E_INVALID_CREDENTIALS = 0x80200040,


		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_100
		///
		/// MessageText:
		///
		///  The request can be continued.
		///
		///  </summary>
		BG_E_HTTP_ERROR_100 = 0x80190064,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_101
		///
		/// MessageText:
		///
		///  The server switched protocols in an upgrade header.
		///
		///  </summary>
		BG_E_HTTP_ERROR_101 = 0x80190065,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_200
		///
		/// MessageText:
		///
		///  The server's response was not valid. The server was not following the defined protocol. Resume the job, and then Background Intelligent Transfer Service (BITS) will try again.
		///
		///  </summary>
		BG_E_HTTP_ERROR_200 = 0x801900C8,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_201
		///
		/// MessageText:
		///
		///  The request was fulfilled and resulted in the creation of a new resource.
		///
		///  </summary>
		BG_E_HTTP_ERROR_201 = 0x801900C9,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_202
		///
		/// MessageText:
		///
		///  The request was accepted for processing, but the processing has not been completed yet.
		///
		///  </summary>
		BG_E_HTTP_ERROR_202 = 0x801900CA,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_203
		///
		/// MessageText:
		///
		///  The returned metadata in the entity-header is not the definitive set available from the server of origin.
		///
		///  </summary>
		BG_E_HTTP_ERROR_203 = 0x801900CB,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_204
		///
		/// MessageText:
		///
		///  The server has fulfilled the request, but there is no new information to send back.
		///
		///  </summary>
		BG_E_HTTP_ERROR_204 = 0x801900CC,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_205
		///
		/// MessageText:
		///
		///  The server's response was not valid. The server was not following the defined protocol. Resume the job, and then Background Intelligent Transfer Service (BITS) will try again.
		///
		///  </summary>
		BG_E_HTTP_ERROR_205 = 0x801900CD,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_206
		///
		/// MessageText:
		///
		///  The server fulfilled the partial GET request for the resource.
		///
		///  </summary>
		BG_E_HTTP_ERROR_206 = 0x801900CE,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_300
		///
		/// MessageText:
		///
		///  The server could not return the requested data.
		///
		///  </summary>
		BG_E_HTTP_ERROR_300 = 0x8019012C,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_301
		///
		/// MessageText:
		///
		///  The requested resource was assigned to a new permanent Uniform Resource Identifier (URI), and any future references to this resource should use one of the returned URIs.
		///
		///  </summary>
		BG_E_HTTP_ERROR_301 = 0x8019012D,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_302
		///
		/// MessageText:
		///
		///  The requested resource was assigned a different Uniform Resource Identifier (URI). This change is temporary.
		///
		///  </summary>
		BG_E_HTTP_ERROR_302 = 0x8019012E,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_303
		///
		/// MessageText:
		///
		///  The response to the request is under a different Uniform Resource Identifier (URI) and must be retrieved using a GET method on that resource.
		///
		///  </summary>
		BG_E_HTTP_ERROR_303 = 0x8019012F,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_304
		///
		/// MessageText:
		///
		///  The server's response was not valid. The server was not following the defined protocol. Resume the job, and then Background Intelligent Transfer Service (BITS) will try again.
		///
		///  </summary>
		BG_E_HTTP_ERROR_304 = 0x80190130,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_305
		///
		/// MessageText:
		///
		///  The requested resource must be accessed through the proxy given by the location field.
		///
		///  </summary>
		BG_E_HTTP_ERROR_305 = 0x80190131,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_307
		///
		/// MessageText:
		///
		///  The URL has been temporarily relocated. Try again later.
		///
		///  </summary>
		BG_E_HTTP_ERROR_307 = 0x80190133,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_400
		///
		/// MessageText:
		///
		///  The server cannot process the request because the syntax is not valid.
		///
		///  </summary>
		BG_E_HTTP_ERROR_400 = 0x80190190,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_401
		///
		/// MessageText:
		///
		///  The requested resource requires user authentication.
		///
		///  </summary>
		BG_E_HTTP_ERROR_401 = 0x80190191,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_402
		///
		/// MessageText:
		///
		///  The server's response was not valid. The server was not following the defined protocol. Resume the job, and then Background Intelligent Transfer Service (BITS) will try again.
		///
		///  </summary>
		BG_E_HTTP_ERROR_402 = 0x80190192,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_403
		///
		/// MessageText:
		///
		///  The client does not have sufficient access rights to the requested server object.
		///
		///  </summary>
		BG_E_HTTP_ERROR_403 = 0x80190193,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_404
		///
		/// MessageText:
		///
		///  The requested URL does not exist on the server.
		///
		///  </summary>
		BG_E_HTTP_ERROR_404 = 0x80190194,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_405
		///
		/// MessageText:
		///
		///  The method used is not allowed.
		///
		///  </summary>
		BG_E_HTTP_ERROR_405 = 0x80190195,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_406
		///
		/// MessageText:
		///
		///  No responses acceptable to the client were found.
		///
		///  </summary>
		BG_E_HTTP_ERROR_406 = 0x80190196,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_407
		///
		/// MessageText:
		///
		///  Proxy authentication is required.
		///
		///  </summary>
		BG_E_HTTP_ERROR_407 = 0x80190197,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_408
		///
		/// MessageText:
		///
		///  The server timed out waiting for the request.
		///
		///  </summary>
		BG_E_HTTP_ERROR_408 = 0x80190198,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_409
		///
		/// MessageText:
		///
		///  The request could not be completed because of a conflict with the current state of the resource. The user should resubmit the request with more information.
		///
		///  </summary>
		BG_E_HTTP_ERROR_409 = 0x80190199,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_410
		///
		/// MessageText:
		///
		///  The requested resource is not currently available at the server, and no forwarding address is known.
		///
		///  </summary>
		BG_E_HTTP_ERROR_410 = 0x8019019A,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_411
		///
		/// MessageText:
		///
		///  The server cannot accept the request without a defined content length.
		///
		///  </summary>
		BG_E_HTTP_ERROR_411 = 0x8019019B,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_412
		///
		/// MessageText:
		///
		///  The precondition given in one or more of the request header fields evaluated to false when it was tested on the server.
		///
		///  </summary>
		BG_E_HTTP_ERROR_412 = 0x8019019C,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_413
		///
		/// MessageText:
		///
		///  The server cannot process the request because the request entity is too large.
		///
		///  </summary>
		BG_E_HTTP_ERROR_413 = 0x8019019D,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_414
		///
		/// MessageText:
		///
		///  The server cannot process the request because the request Uniform Resource Identifier (URI) is longer than the server can interpret.
		///
		///  </summary>
		BG_E_HTTP_ERROR_414 = 0x8019019E,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_415
		///
		/// MessageText:
		///
		///  The server's response was not valid. The server was not following the defined protocol. Resume the job, and then Background Intelligent Transfer Service (BITS) will try again.
		///
		///  </summary>
		BG_E_HTTP_ERROR_415 = 0x8019019F,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_416
		///
		/// MessageText:
		///
		///  The server could not satisfy the range request.
		///
		///  </summary>
		BG_E_HTTP_ERROR_416 = 0x801901A0,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_417
		///
		/// MessageText:
		///
		///  The server could not meet the expectation given in an Expect request-header field.
		///
		///  </summary>
		BG_E_HTTP_ERROR_417 = 0x801901A1,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_449
		///
		/// MessageText:
		///
		///   The server's response was not valid. The server was not following the defined protocol. Resume the job, and then Background Intelligent Transfer Service (BITS) will try again.
		///
		///  </summary>
		BG_E_HTTP_ERROR_449 = 0x801901C1,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_500
		///
		/// MessageText:
		///
		///  An unexpected condition prevented the server from fulfilling the request.
		///
		///  </summary>
		BG_E_HTTP_ERROR_500 = 0x801901F4,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_501
		///
		/// MessageText:
		///
		///  The server does not support the functionality required to fulfill the request.
		///
		///  </summary>
		BG_E_HTTP_ERROR_501 = 0x801901F5,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_502
		///
		/// MessageText:
		///
		///  The server, while acting as a gateway or proxy to fulfill the request, received an invalid response from the upstream server it accessed.
		///
		///  </summary>
		BG_E_HTTP_ERROR_502 = 0x801901F6,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_503
		///
		/// MessageText:
		///
		///  The service is temporarily overloaded.
		///
		///  </summary>
		BG_E_HTTP_ERROR_503 = 0x801901F7,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_504
		///
		/// MessageText:
		///
		///  The request was timed out waiting for a gateway.
		///
		///  </summary>
		BG_E_HTTP_ERROR_504 = 0x801901F8,

		///  <summary>
		///
		/// MessageId: BG_E_HTTP_ERROR_505
		///
		/// MessageText:
		///
		///  The server does not support the HTTP protocol version that was used in the request message.
		///
		///  </summary>
		BG_E_HTTP_ERROR_505 = 0x801901F9,

		//
		// Additional Background Intelligent Transfer Service (BITS) mc entries
		// Reserved range is 0x4000 to 0x4100
		//
		
		///  <summary>
		///
		/// MessageId: BITS_MC_JOB_CANCELLED
		///
		/// MessageText:
		///
		///  The administrator %4 canceled job "%2" on behalf of %3.  The job ID was %1.
		///
		///  </summary>
		BITS_MC_JOB_CANCELLED = 0x80194000,

		///  <summary>
		///
		/// MessageId: BITS_MC_FILE_DELETION_FAILED
		///
		/// MessageText:
		///
		///  While canceling job "%2", BITS was not able to remove the temporary files listed below.
		///  If you can delete them, then you will regain some disk space.  The job ID was %1.%\
		///
		///  %3
		///
		///  </summary>
		BITS_MC_FILE_DELETION_FAILED = 0x80194001,

		///  <summary>
		///
		/// MessageId: BITS_MC_FILE_DELETION_FAILED_MORE
		///
		/// MessageText:
		///
		///  While canceling job "%2", BITS was not able to remove the temporary files listed below.
		///  If you can delete them, then you will regain some disk space.  The job ID was %1. %\
		///
		///  %3
		///  %\
		///  Due to space limitations, not all files are listed here.  Check for additional files of the form BITxxx.TMP in the same directory.
		///
		///  </summary>
		BITS_MC_FILE_DELETION_FAILED_MORE = 0x80194002,

		///  <summary>
		///
		/// MessageId: BITS_MC_JOB_PROPERTY_CHANGE
		///
		/// MessageText:
		///
		///  The administrator %3 modified the %4 property of job "%2".  The job ID was %1.
		///
		///  </summary>
		BITS_MC_JOB_PROPERTY_CHANGE = 0x80194003,

		///  <summary>
		///
		/// MessageId: BITS_MC_JOB_TAKE_OWNERSHIP
		///
		/// MessageText:
		///
		///  The administrator %4 took ownership of job "%2" from %3.  The job ID was %1.
		///
		///  </summary>
		BITS_MC_JOB_TAKE_OWNERSHIP = 0x80194004,

		///  <summary>
		///
		/// MessageId: BITS_MC_JOB_SCAVENGED
		///
		/// MessageText:
		///
		///  Job "%2" owned by %3 was canceled after being inactive for more than %4 days.  The job ID was %1.
		///
		///  </summary>
		BITS_MC_JOB_SCAVENGED = 0x80194005,

		///  <summary>
		///
		/// MessageId: BITS_MC_JOB_NOTIFICATION_FAILURE
		///
		/// MessageText:
		///
		///  Job "%2" owned by %3 failed to notify its associated application.  BITS will retry in %4 minutes.  The job ID was %1.
		///
		///  </summary>
		BITS_MC_JOB_NOTIFICATION_FAILURE = 0x80194006,

		///  <summary>
		///
		/// MessageId: BITS_MC_STATE_FILE_CORRUPT
		///
		/// MessageText:
		///
		///  The BITS job list is not in a recognized format.  It may have been created by a different version of BITS.  The job list has been cleared.
		///
		///  </summary>
		BITS_MC_STATE_FILE_CORRUPT = 0x80194007,

		///  <summary>
		///
		/// MessageId: BITS_MC_FAILED_TO_START
		///
		/// MessageText:
		///  The BITS server failed to start. Try restarting the service at a later time.
		///
		///  </summary>
		BITS_MC_FAILED_TO_START = 0x80194008,

		///  <summary>
		///
		/// MessageId: BITS_MC_FATAL_IGD_ERROR
		///
		/// MessageText:
		///
		/// BITS has hit a fatal error communicating with an Internet Gateway Device.  Please check that the device is functioning properly. BITS will not attempt to use this device until the next system reboot.
		///
		///  </summary>
		BITS_MC_FATAL_IGD_ERROR = 0x80194009,

		///  <summary>
		///
		/// MessageId: BITS_MC_PEERCACHING_PORT
		///
		/// MessageText:
		///
		/// BITS Peer-caching protocol
		///
		///  </summary>
		BITS_MC_PEERCACHING_PORT = 0x8019400A,

		///  <summary>
		///
		/// MessageId: BITS_MC_WSD_PORT
		///
		/// MessageText:
		///
		/// Web Services-Discovery protocol
		///
		///  </summary>
		BITS_MC_WSD_PORT = 0x8019400B
	}

	/// <summary>
	/// https://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_cert_store_location.asp
	/// </summary>
	public enum CertStoreLocation : int
    {
		/// <summary>
		/// To indicate no certificate has been set
		/// </summary>
		None = -1,
		/// <summary>
		/// Use the current user's certificate store.
		/// </summary>
		CurrentUser = 0,
		/// <summary>
		/// Use the local computer's certificate store.
		/// </summary>
		LocalMachine = (CurrentUser + 1),
		/// <summary>
		/// Use the current service's certificate store.
		/// </summary>
		CurrentService = (LocalMachine + 1),
		/// <summary>
		/// Use a specific service's certificate store.
		/// </summary>
		Services = (CurrentService + 1),
		/// <summary>
		/// Use a specific user's certificate store.
		/// </summary>
		Users = (Services + 1),
		/// <summary>
		/// Use the current user's group policy certificate store. In a network setting, stores in this location are downloaded to the client computer from the Group Policy Template (GPT) during computer startup or user logon. 
		/// </summary>
		CurrentUserGroupPolicy = (Users + 1),
		/// <summary>
		/// Use the local computer's certificate store. In a network setting, stores in this location are downloaded to the client computer from the Group Policy Template (GPT) during computer startup or user logon.
		/// </summary>
		LocalMachineGroupPolicy = (CurrentUserGroupPolicy + 1),
		/// <summary>
		/// Use the enterprise certificate store. The enterprise store is shared across domains in the enterprise and downloaded from the global enterprise directory.
		/// </summary>
		LocalMachineenterprise = (LocalMachineGroupPolicy + 1) 
    }

	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum SSL
	{
		/// <summary>
		/// 
		/// </summary>
		None = 0,
		/// <summary>
		/// 
		/// </summary>
		EnableCrlCheck = 0x0001,
		/// <summary>
		/// 
		/// </summary>
		IgnoreCertCnInvalid = 0x0002,
		/// <summary>
		/// 
		/// </summary>
		IgnoreCertDateInvalid = 0x0004,
		/// <summary>
		/// 
		/// </summary>
		IgnoreUnknownCa = 0x0008,
		/// <summary>
		/// 
		/// </summary>
		IgnoreCertWrongUsage = 0x0010
	}

	/// <summary>
	/// https://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_cert_store_location.asp
	/// </summary>
	public enum HttpRedirectPolicy
    {
		/// <summary>
		/// Allows redirection to occur. This is the default.
		/// </summary>
		Allowsilent,
		/// <summary>
		/// Allows redirection to occur. BITS updates the remote name with the final URL.
		/// </summary>
		Allowreport,
		/// <summary>
		/// Places the job in the fatal error state when a redirect occurs. BITS updates the remote name with the redirected URL.
		/// </summary>
		Disallow
    }

	/// <summary>
	/// https://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_auth_credentials.asp
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Size = 16, Pack = 4)]
	internal struct _BG_AUTH_CREDENTIALS
	{
		/// <summary>
		/// Identifies whether to use the credentials for a proxy or server authentication request. For a list of values, see the BG_AUTH_TARGET enumeration. You can specify only one value.
		/// </summary>
		[FieldOffset(0)]
		public AuthTarget Target;

		/// <summary>
		/// Identifies the scheme to use for authentication (for example, Basic or NTLM). For a list of values, see the BG_AUTH_SCHEME enumeration. You can specify only one value.
		/// </summary>
		[FieldOffset(4)]
		public AuthScheme Scheme;

		/// <summary>
		/// Identifies the credentials to use for the specified authentication scheme. For details, see the BG_AUTH_CREDENTIALS_UNION union.
		/// </summary>
		[FieldOffset(8)]
		public _BG_AUTH_CREDENTIALS_UNION Credentials;
	}

	/// <summary>
	/// https://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_auth_credentials_union.asp
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Size = 8, Pack = 4)]
	internal struct _BG_AUTH_CREDENTIALS_UNION
	{
		/// <summary>
		/// Identifies the user name and password of the user to authenticate. For details, see the BG_BASIC_CREDENTIALS structure.
		/// </summary>
		[FieldOffset(0)]
		public _BG_BASIC_CREDENTIALS Basic;
	}

	/// <summary>
	/// https://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_basic_credentials.asp
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Size = 8, Pack = 4)]
	internal struct _BG_BASIC_CREDENTIALS
	{
		/// <summary>
		/// String that contains the user name to authenticate. The user name is limited to 300 characters, not including the null terminator.
		/// The format of the user name depends on the authentication scheme requested.
		/// For example, for Basic, NTLM, and Negotiate authentication, the user name is of the form "domain\user name".
		/// For Passport authentication, the user name is an e-mail address. If NULL, default credentials for this session context are used.
		/// </summary>
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.LPWStr)]
		public string UserName;

		/// <summary>
		/// String that contains the password in clear-text. The password is limited to 300 characters, not including the null terminator.
		/// The password can be blank. Set to NULL if UserName is NULL.
		/// BITS encrypts the password before persisting the job if a network disconnect occurs or the user logs off.
		/// </summary>
		[FieldOffset(4)]
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Password;
	}
 
	/// <summary>
	/// The BG_FILE_INFO structure provides the local and 
	/// remote names of the file to transfer
	/// </summary>
	[StructLayoutAttribute(LayoutKind.Sequential, Pack=4, Size=0)]
	internal struct _BG_FILE_INFO 
	{
		/// <summary>
		/// Remote Name for the File
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string RemoteName;

		/// <summary>
		/// Local Name for the file
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string LocalName;
	}

	/// <summary>
	/// The BG_JOB_PROGRESS structure provides job-related progress information, 
	/// such as the number of bytes and files transferred
	/// </summary>
	[StructLayoutAttribute(LayoutKind.Sequential, Pack=8, Size=0)]
	internal struct _BG_JOB_PROGRESS 
	{
		/// <summary>
		/// Total number of bytes to transfer for the job.
		/// </summary>
		public ulong BytesTotal;

		/// <summary>
		/// Number of bytes transferred
		/// </summary>
		public ulong BytesTransferred;

		/// <summary>
		/// Total number of files to transfer for this job
		/// </summary>
		public uint FilesTotal;

		/// <summary>
		/// Number of files transferred. 
		/// </summary>
		public uint FilesTransferred;
	}

	[StructLayout(LayoutKind.Sequential, Pack=8)]
	internal struct _BG_JOB_REPLY_PROGRESS
	{
		// Fields
		public ulong BytesTotal;
		public ulong BytesTransferred;
	}

	/// <summary>
	/// The BG_JOB_TIMES structure provides job-related timestamps
	/// </summary>
	[StructLayoutAttribute(LayoutKind.Sequential, Pack=4, Size=0)]
	internal struct _BG_JOB_TIMES 
	{
		/// <summary>
		/// Time the job was created
		/// </summary>
		public _FILETIME CreationTime;

		/// <summary>
		/// Time the job was last modified or bytes were transferred
		/// </summary>
		public _FILETIME ModificationTime;

		/// <summary>
		/// Time the job entered the BG_JOB_STATE_TRANSFERRED state
		/// </summary>
		public _FILETIME TransferCompletionTime;
	}

	/// <summary>
	/// FILETIME Structure
	/// </summary>
	[StructLayoutAttribute(LayoutKind.Sequential, Pack=4, Size=0)]
	internal struct _FILETIME 
	{
		/// <summary>
		/// Description
		/// </summary>
		public uint dwLowDateTime;

		/// <summary>
		/// Description
		/// </summary>
		public uint dwHighDateTime;
	}

	/// <summary>
	/// The BG_FILE_PROGRESS structure provides file-related progress information, 
	/// such as the number of bytes transferred
	/// </summary>
	[StructLayoutAttribute(LayoutKind.Sequential, Pack=8, Size=0)]
	internal struct _BG_FILE_PROGRESS 
	{
		/// <summary>
		/// Size of the file in bytes
		/// </summary>
		public ulong BytesTotal;

		/// <summary>
		/// Number of bytes transferred. 
		/// </summary>
		public ulong BytesTransferred;

		/// <summary>
		/// For downloads, the value is TRUE if the file is available to the user; 
		/// otherwise, the value is FALSE
		/// </summary>
		public int Completed;
	}

	[StructLayoutAttribute(LayoutKind.Sequential, Pack = 4, Size = 16)]
	internal struct _BG_FILE_RANGE
	{
		public const long BG_LENGTH_TO_EOF = -1;

		public long InitialOffset;
		public long Length;
	}

	[StructLayoutAttribute(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
	struct _BG_GATEWAY_INFO
	{
		public IntPtr Next;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string UniqueDeviceName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string FriendlyName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string Type;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string PresentationURL;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string ManufacturerName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string ManufacturerURL;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string ModelName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string ModelNumber;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string Description;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string ModelURL;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string UPC;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
		public string SerialNumber;
		public Guid BITSDeviceID;
		public uint InBytes;
		public uint OutBytes;
		public int SupportsCounters;
	};

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct _BG_CACHE_RECORD_INFO
	{
		public Guid Id;                                // Generated GUID for the record
		public _FILETIME CreationTime;
		public _FILETIME ModificationTime;
		public _FILETIME AccessTime;
		public _FILETIME FileModificationTime;
		public ulong FileSize;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string FileEtag;
		public bool FileValidated;
	}
}
