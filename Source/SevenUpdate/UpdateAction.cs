// ***********************************************************************
// <copyright file="UpdateAction.cs" project="SevenUpdate" assembly="SevenUpdate" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// <summary>
//   The layout for the Info Panel
// .</summary>
// ***********************************************************************
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