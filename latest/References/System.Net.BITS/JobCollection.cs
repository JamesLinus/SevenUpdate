/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

namespace System.Net.BITS
{
	/// <summary>
	/// A collection of Jobs.
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ienumbackgroundcopyjobs.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ienumbackgroundcopyjobs.asp")]
	public sealed class JobCollection : CollectionBase
	{
		private Manager manager;
		private JobCollectionFlags flags = JobCollectionFlags.CurrentUser;
		private IBackgroundCopyJob insertBCJ;
		private Guid insertGuid;

        /// <summary>
        /// Closes all Jobs
        /// </summary>
        public void CloseAllJobs()
        {
            foreach (Job job in List)
                job.Close();

            base.OnClear();
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="manager"></param>
		internal JobCollection(Manager manager)
		{
			this.manager = manager;
		}

		/// <summary>
		/// Same as Manager.EnumAllUserJobs
		/// </summary>
		public JobCollectionFlags JobCollectionFlags
		{
			get { return flags; }
			set
			{
				flags = value;
				Update();
			}
		}

		/// <summary>
		/// Update the list of Job maintained by BITS
		/// </summary>
		public void Update()
		{
			if (manager.IsInit || manager.BCM == null || flags == JobCollectionFlags.None)
				return;
			
			try
			{
				IEnumBackgroundCopyJobs jobs = null;

				manager.BCM.EnumJobs(	flags == JobCollectionFlags.AllUsers
											? BackgroundCopyManager.BG_JOB_ENUM_ALL_USERS
											: 0,
										out jobs);
				Utils.Release<IEnumBackgroundCopyJobs>(ref jobs, delegate()
				{
					List<Guid> guids = new List<Guid>();

					// add job not already in list
					for (;;)
					{
						uint fetched;

						jobs.Next(1, out insertBCJ, out fetched);

						if (fetched == 0)
							break;

						insertBCJ.GetId(out insertGuid);
						guids.Add(insertGuid);
						
						Utils.Release<IBackgroundCopyJob>(ref insertBCJ, delegate()
						{
							if (!Contains(insertGuid))
								List.Add(new Job());
						});
					}

					// remove jobs that do not belong in this list
					for (int i = 0; i < Count; )
					{
						if (this[i].BCJ == null || guids.Contains(this[i].Id))
							i++;
						else
							List.RemoveAt(i);
					}
				});
			}
			catch (Exception e)
			{
				manager.HandleException(e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="job"></param>
		/// <returns></returns>
		public int Add(Job job)
		{
			return List.Add(job);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public int Add(string name)
		{
			return Add(new Job(name));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public int Add(string name, JobType type)
		{
			return Add(new Job(name, type));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="priority"></param>
		public int Add(string name, JobType type, JobPriority priority)
		{
			return Add(new Job(name, type, priority));
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobs"></param>
		public void AddRange(Job[] jobs)
		{
			foreach (Job job in jobs)
				List.Add(job);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="job"></param>
		public void Remove(Job job)
		{
			List.Remove(job);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobGuid"></param>
		public void Remove(Guid jobGuid)
		{
			foreach (Job job in List)
			{
				if (job.Id.CompareTo(jobGuid) == 0)
				{
					List.Remove(job);
					break;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobs"></param>
		/// <param name="index"></param>
		public void CopyTo(Job[] jobs, int index)
		{
			List.CopyTo(jobs, index);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public Job this[int i]
		{
			get { return (Job)List[i]; }
			set { List[i] = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="job"></param>
		/// <returns></returns>
		public bool Contains(Job job)
		{
			return List.Contains(job);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobGuid"></param>
		/// <returns></returns>
		public bool Contains(Guid jobGuid)
		{
			foreach (Job job in List)
			{
				if (job.Id.CompareTo(jobGuid) == 0)
					return true;
			}

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="job"></param>
		/// <returns></returns>
		public int IndexOf(Job job)
		{
			return List.IndexOf(job);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobGuid"></param>
		/// <returns></returns>
		public int IndexOf(Guid jobGuid)
		{
			for (int i = 0; i < Count; i++)
			{
				Job job = (Job)List[i];
				
				if (job.Id.CompareTo(jobGuid) == 0)
					return i;
			}
			
			return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		protected override void OnValidate(object value)
		{
			if (!(value is Job))
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
			CheckInsert((Job)value);
			base.OnInsert(index, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsertComplete(int index, object value)
		{
			SetupJob((Job)value);
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
			CheckInsert((Job)newValue);
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
			((Job)oldValue).Close();
			SetupJob((Job)newValue);
			base.OnSetComplete(index, oldValue, newValue);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnRemove(int index, object value)
		{
			Job job = (Job)value;
			
			job.Close();
			base.OnRemove(index, value);
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnClear()
		{
			foreach (Job job in List)
				job.Close();
				
			base.OnClear();
		}
		
		private void CheckInsert(Job newJob)
		{
			if (newJob.Parent != null)
				throw new BITSException(Properties.Resources.JobInCollection);

			if (newJob.Id != Guid.Empty)
			{
				foreach (Job job in List)
				{
					if (Guid.Equals(job.Id, newJob.Id))
						throw new BITSException(Properties.Resources.JobInCollection);
				}
			}
		}

		private void SetupJob(Job job)
		{
			Utils.Release<IBackgroundCopyJob>(ref insertBCJ, delegate()
			{
				job.Construct(manager, insertBCJ, insertGuid);
				insertBCJ = null; // don't want insertBCJ to be released
			});
		}
	}
}
