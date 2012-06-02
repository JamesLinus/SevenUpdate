// <copyright file="IElevatedProcessCallback.cs" project="SevenUpdate.Service">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Service
{
    using System.ServiceModel;

    using ProtoBuf.ServiceModel;

    /// <summary>Contains callbacks/events to relay back to the server.</summary>
    [ServiceContract(Namespace = "http://sevenupdate.com", CallbackContract = typeof(IElevatedProcess))]
    public interface IElevatedProcessCallback
    {
        /// <summary>Occurs when the process starts.</summary>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void ElevatedProcessStarted();

        /// <summary>Occurs when the process as exited.</summary>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void ElevatedProcessStopped();

        /// <summary>Occurs when the download has completed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void OnDownloadCompleted(object sender, DownloadCompletedEventArgs e);

        /// <summary>Occurs when the download progress has changed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e);

        /// <summary>Occurs when an error occurs.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void OnErrorOccurred(object sender, ErrorOccurredEventArgs e);

        /// <summary>Occurs when the installation of updates has completed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void OnInstallCompleted(object sender, InstallCompletedEventArgs e);

        /// <summary>Occurs when the installation progress has changed.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void OnInstallProgressChanged(object sender, InstallProgressChangedEventArgs e);
    }
}