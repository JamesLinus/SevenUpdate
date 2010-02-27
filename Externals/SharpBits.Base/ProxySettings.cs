namespace SharpBits.Base
{
    public class ProxySettings
    {
        private IBackgroundCopyJob job;
        private string proxyBypassList;
        private string proxyList;
        private BG_JOB_PROXY_USAGE proxyUsage;

        internal ProxySettings(IBackgroundCopyJob job)
        {
            this.job = job;
            job.GetProxySettings(out proxyUsage, out proxyList, out proxyBypassList);
        }

        public ProxyUsage ProxyUsage
        {
            get { return (ProxyUsage) proxyUsage; }
            set { proxyUsage = (BG_JOB_PROXY_USAGE) value; }
        }

        public string ProxyList
        {
            get { return proxyList; }
            set { proxyList = value; }
        }

        public string ProxyBypassList
        {
            get { return proxyBypassList; }
            set { proxyBypassList = value; }
        }

        public void Update()
        {
            job.SetProxySettings(proxyUsage, proxyList, proxyBypassList);
        }
    }
}