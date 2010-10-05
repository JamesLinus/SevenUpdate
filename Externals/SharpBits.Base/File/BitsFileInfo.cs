//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base.File
{
    /// <summary>
    /// </summary>
    public class BitsFileInfo
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="fileInfo">
        /// </param>
        internal BitsFileInfo(BGFileInfo fileInfo)
        {
            this.BGFileInfo = fileInfo;
        }

        /// <summary>
        /// </summary>
        /// <param name="remoteName">
        /// </param>
        /// <param name="localName">
        /// </param>
        protected BitsFileInfo(string remoteName, string localName)
        {
            this.BGFileInfo = new BGFileInfo { RemoteName = remoteName, LocalName = localName };
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string LocalName
        {
            get
            {
                return this.BGFileInfo.LocalName;
            }
        }

        /// <summary>
        /// </summary>
        public string RemoteName
        {
            get
            {
                return this.BGFileInfo.RemoteName;
            }
        }

        /// <summary>
        /// </summary>
        internal BGFileInfo BGFileInfo { get; private set; }

        #endregion
    }
}