// <copyright file="UpdateAction.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    /// <summary>The layout for the Info Panel.</summary>
    public enum UpdateAction
    {
        /// <summary>Canceled Updates.</summary>
        Canceled, 

        /// <summary>Check for updates.</summary>
        CheckForUpdates, 

        /// <summary>Checking for updates.</summary>
        CheckingForUpdates, 

        /// <summary>When connecting to the admin service.</summary>
        ConnectingToService, 

        /// <summary>When downloading of updates has been completed.</summary>
        DownloadCompleted, 

        /// <summary>Downloading updates.</summary>
        Downloading, 

        /// <summary>An Error Occurred when downloading/installing updates.</summary>
        ErrorOccurred, 

        /// <summary>When installation of updates have completed.</summary>
        InstallationCompleted, 

        /// <summary>Installing Updates.</summary>
        Installing, 

        /// <summary>No updates have been found.</summary>
        NoUpdates, 

        /// <summary>A reboot is needed to finish installing updates.</summary>
        RebootNeeded, 

        /// <summary>Updates have been found.</summary>
        UpdatesFound, 
    }
}