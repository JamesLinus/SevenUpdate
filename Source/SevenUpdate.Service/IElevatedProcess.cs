// <copyright file="IElevatedProcess.cs" project="SevenUpdate.Service">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Service
{
    using System.Collections.ObjectModel;
    using System.ServiceModel;

    using ProtoBuf.ServiceModel;

    /// <summary>Contains callbacks/events to relay back to the client.</summary>
    [ServiceContract(Namespace = "http://sevenupdate.com")]
    public interface IElevatedProcess
    {
        /// <summary>Adds an application to Seven Update, so it can manage updates for it.</summary>
        /// <param name="application">The application to add.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void AddApp(Sua application);

        /// <summary>Changes the program settings.</summary>
        /// <param name="applications">The applications to enable update checking.</param>
        /// <param name="options">The Seven Update settings.</param>
        /// <param name="autoCheck">If set to <c>True</c> automatic updates will be enabled.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void ChangeSettings(Collection<Sua> applications, Config options, bool autoCheck);

        /// <summary>Hides a single update.</summary>
        /// <param name="hiddenUpdate">The update to hide.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void HideUpdate(Suh hiddenUpdate);

        /// <summary>Hides a collection of <c>Suh</c> to hide.</summary>
        /// <param name="hiddenUpdates">The collection of updates to hide.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void HideUpdates(Collection<Suh> hiddenUpdates);

        /// <summary>Gets a collection of <c>Sui</c>.</summary>
        /// <param name="applicationUpdates">The collection of applications and updates to install.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void InstallUpdates(Collection<Sui> applicationUpdates);

        /// <summary>The update to show and remove from hidden updates.</summary>
        /// <param name="hiddenUpdate">The hidden update to show.</param>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void ShowUpdate(Suh hiddenUpdate);

        /// <summary>
        ///   Requests shutdown of the admin process. App will only shutdown if it's not installing updates. To shutdown
        ///   when updates are being installed, execute the admin process with the 'Abort' argument.
        /// </summary>
        [OperationContract(IsOneWay = false)]
        [ProtoBehavior]
        void Shutdown();
    }
}