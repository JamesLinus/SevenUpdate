// ***********************************************************************
// <copyright file="ElevatedProcessCallback.cs" project="SevenUpdate.Admin" assembly="SevenUpdate.Admin" solution="SevenUpdate" company="Seven Software">
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
// ***********************************************************************

namespace SevenUpdate.Admin
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using SevenUpdate.Service;

    /// <summary>The WCF client callback.</summary>
    public class ElevatedProcessCallback : DuplexClientBase<IElevatedProcessCallback>, IElevatedProcessCallback
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ElevatedProcessCallback" /> class.</summary>
        /// <param name="callbackInstance">The callback instance context.</param>
        /// <param name="binding">The service binding configuration.</param>
        /// <param name="remoteAddress">The url for the service.</param>
        public ElevatedProcessCallback(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        {
        }

        #endregion

        #region Implemented Interfaces

        #region IElevatedProcessCallback

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

        #endregion

        #endregion
    }
}