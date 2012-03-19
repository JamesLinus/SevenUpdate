// <copyright file="BitsFileInfo.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The file info for the <c>BitsFile</c>.</summary>
    public class BitsFileInfo
    {
        /// <summary>Initializes a new instance of the <see cref="BitsFileInfo" /> class.</summary>
        /// <param name="remoteName">Name of the remote.</param>
        /// <param name="localName">Name of the local.</param>
        protected BitsFileInfo(string remoteName, string localName)
        {
            this.BGFileInfo = new BGFileInfo { RemoteName = remoteName, LocalName = localName };
        }

        /// <summary>Gets the local file name.</summary>
        /// <value>The name of the local.</value>
        public string LocalName
        {
            get { return this.BGFileInfo.LocalName; }
        }

        /// <summary>Gets the remote file name.</summary>
        /// <value>The name of the remote.</value>
        public string RemoteName
        {
            get { return this.BGFileInfo.RemoteName; }
        }

        /// <summary>Gets the BG file info.</summary>
        /// <value>The BG file info.</value>
        internal BGFileInfo BGFileInfo { get; private set; }
    }
}