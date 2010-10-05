//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    /// </summary>
    public class ProxySettings
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly IBackgroundCopyJob job;

        /// <summary>
        /// </summary>
        private string proxyBypassList;

        /// <summary>
        /// </summary>
        private string proxyList;

        /// <summary>
        /// </summary>
        private BGJobProxyUsage proxyUsage;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="job">
        /// </param>
        internal ProxySettings(IBackgroundCopyJob job)
        {
            this.job = job;
            job.GetProxySettings(out this.proxyUsage, out this.proxyList, out this.proxyBypassList);
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string ProxyBypassList
        {
            get
            {
                return this.proxyBypassList;
            }

            set
            {
                this.proxyBypassList = value;
            }
        }

        /// <summary>
        /// </summary>
        public string ProxyList
        {
            get
            {
                return this.proxyList;
            }

            set
            {
                this.proxyList = value;
            }
        }

        /// <summary>
        /// </summary>
        public ProxyUsage ProxyUsage
        {
            get
            {
                return (ProxyUsage)this.proxyUsage;
            }

            set
            {
                this.proxyUsage = (BGJobProxyUsage)value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        public void Update()
        {
            this.job.SetProxySettings(this.proxyUsage, this.proxyList, this.proxyBypassList);
        }

        #endregion
    }
}