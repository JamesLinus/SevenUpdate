// ***********************************************************************
// <copyright file="ProxySettings.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SharpBits.Base
{
    using SharpBits.Base.Job;

    /// <summary>
    /// The proxy settings for the <see cref="BitsJob"/>
    /// </summary>
    public class ProxySettings
    {
        #region Constants and Fields

        /// <summary>
        ///   The job for the proxy settings
        /// </summary>
        private readonly IBackgroundCopyJob job;

        /// <summary>
        ///   The proxy bypass list
        /// </summary>
        private string proxyBypassList;

        /// <summary>
        ///   The proxy list
        /// </summary>
        private string proxyList;

        /// <summary>
        ///   The usage of proxy
        /// </summary>
        private BGJobProxyUsage proxyUsage;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxySettings"/> class.
        /// </summary>
        /// <param name="job">
        /// The job to set the proxy settings for
        /// </param>
        internal ProxySettings(IBackgroundCopyJob job)
        {
            this.job = job;
            job.GetProxySettings(out this.proxyUsage, out this.proxyList, out this.proxyBypassList);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the proxy bypass list.
        /// </summary>
        /// <value>The proxy bypass list.</value>
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
        ///   Gets or sets the proxy list.
        /// </summary>
        /// <value>The proxy list.</value>
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
        ///   Gets or sets the proxy usage.
        /// </summary>
        /// <value>The proxy usage.</value>
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
        /// Updates <see cref="BitsJob"/> with the proxy usage
        /// </summary>
        public void Update()
        {
            this.job.SetProxySettings(this.proxyUsage, this.proxyList, this.proxyBypassList);
        }

        #endregion
    }
}