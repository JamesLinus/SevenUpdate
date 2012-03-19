// <copyright file="ElevatedProcessCallback.cs" project="SevenUpdate.Admin">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Admin
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using SevenUpdate.Service;

    /// <summary>The WCF client callback.</summary>
    public class ElevatedProcessCallback : DuplexClientBase<IElevatedProcessCallback>, IElevatedProcessCallback
    {
        /// <summary>Initializes a new instance of the <see cref="ElevatedProcessCallback" /> class.</summary>
        /// <param name="callbackInstance">The callback instance context.</param>
        /// <param name="binding">The service binding configuration.</param>
        /// <param name="remoteAddress">The url for the service.</param>
        public ElevatedProcessCallback(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        {
        }

        /// <summary>Occurs when the process starts.</summary>
        public void ElevatedProcessStarted()
        {
            this.Channel.ElevatedProcessStarted();
        }

        /// <summary>Occurs when the process as exited.</summary>
        public void ElevatedProcessStopped()
        {
            this.Channel.ElevatedProcessStopped();
        }

        /// <summary>Occurs when the download has completed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        public void OnDownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            this.Channel.OnDownloadCompleted(sender, e);
        }

        /// <summary>Occurs when the download progress has changed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        public void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Channel.OnDownloadProgressChanged(sender, e);
        }

        /// <summary>Occurs when an error occurs.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        public void OnErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            this.Channel.OnErrorOccurred(sender, e);
        }

        /// <summary>Occurs when the installation of updates has completed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        public void OnInstallCompleted(object sender, InstallCompletedEventArgs e)
        {
            this.Channel.OnInstallCompleted(sender, e);
        }

        /// <summary>Occurs when the installation progress has changed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        public void OnInstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            this.Channel.OnInstallProgressChanged(sender, e);
        }
    }
}