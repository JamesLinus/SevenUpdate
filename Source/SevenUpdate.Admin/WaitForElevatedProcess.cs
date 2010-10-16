namespace SevenUpdate.Admin
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using SevenUpdate.Service;

    public class WaitForElevatedProcess : DuplexClientBase<IWaitForElevatedProcess>, IWaitForElevatedProcess
    {
        public WaitForElevatedProcess(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        public void ElevatedProcessStarted()
        {
            this.Channel.ElevatedProcessStarted();
        }

        public void ElevatedProcessStopped()
        {
            this.Channel.ElevatedProcessStopped();
        }

        public void OnDownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            this.Channel.OnDownloadCompleted(sender, e);
        }

        public void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Channel.OnDownloadProgressChanged(sender, e);
        }

        public void OnErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            this.Channel.OnErrorOccurred(sender, e);
        }

        public void OnInstallCompleted(object sender, InstallCompletedEventArgs e)
        {
            this.Channel.OnInstallCompleted(sender, e);
        }

        public void OnInstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            this.Channel.OnInstallProgressChanged(sender, e);
        }
    }
}