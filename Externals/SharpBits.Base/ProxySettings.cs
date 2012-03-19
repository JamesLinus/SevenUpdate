// <copyright file="ProxySettings.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The proxy settings for the <c>BitsJob</c>.</summary>
    public class ProxySettings
    {
        /// <summary>The job for the proxy settings.</summary>
        private readonly IBackgroundCopyJob job;

        /// <summary>The proxy bypass list.</summary>
        private string proxyBypassList;

        /// <summary>The proxy list.</summary>
        private string proxyList;

        /// <summary>The usage of proxy.</summary>
        private BGJobProxyUsage proxyUsage;

        /// <summary>Initializes a new instance of the <see cref="ProxySettings" /> class.</summary>
        /// <param name="job">The job to set the proxy settings for.</param>
        internal ProxySettings(IBackgroundCopyJob job)
        {
            this.job = job;
            job.GetProxySettings(out this.proxyUsage, out this.proxyList, out this.proxyBypassList);
        }

        /// <summary>Gets or sets the proxy bypass list.</summary>
        /// <value>The proxy bypass list.</value>
        public string ProxyBypassList
        {
            get { return this.proxyBypassList; }

            set { this.proxyBypassList = value; }
        }

        /// <summary>Gets or sets the proxy list.</summary>
        /// <value>The proxy list.</value>
        public string ProxyList
        {
            get { return this.proxyList; }

            set { this.proxyList = value; }
        }

        /// <summary>Gets or sets the proxy usage.</summary>
        /// <value>The proxy usage.</value>
        public ProxyUsage ProxyUsage
        {
            get { return (ProxyUsage)this.proxyUsage; }

            set { this.proxyUsage = (BGJobProxyUsage)value; }
        }

        /// <summary>Updates <c>BitsJob</c> with the proxy usage.</summary>
        public void Update()
        {
            this.job.SetProxySettings(this.proxyUsage, this.proxyList, this.proxyBypassList);
        }
    }
}