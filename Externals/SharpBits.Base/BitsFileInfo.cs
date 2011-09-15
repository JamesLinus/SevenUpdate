// ***********************************************************************
// <copyright file="BitsFileInfo.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>The file info for the <c>BitsFile</c>.</summary>
    public class BitsFileInfo
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BitsFileInfo" /> class.</summary>
        /// <param name="remoteName">Name of the remote.</param>
        /// <param name="localName">Name of the local.</param>
        protected BitsFileInfo(string remoteName, string localName)
        {
            this.BGFileInfo = new BGFileInfo { RemoteName = remoteName, LocalName = localName };
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the local file name.</summary>
        /// <value>The name of the local.</value>
        public string LocalName
        {
            get
            {
                return this.BGFileInfo.LocalName;
            }
        }

        /// <summary>Gets the remote file name.</summary>
        /// <value>The name of the remote.</value>
        public string RemoteName
        {
            get
            {
                return this.BGFileInfo.RemoteName;
            }
        }

        #endregion

        #region Properties

        /// <summary>Gets the BG file info.</summary>
        /// <value>The BG file info.</value>
        internal BGFileInfo BGFileInfo { get; private set; }

        #endregion
    }
}
