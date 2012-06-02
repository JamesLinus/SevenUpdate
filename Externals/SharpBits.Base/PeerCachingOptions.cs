// <copyright file="PeerCachingOptions.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System;

    /// <summary>
    ///   Flags that determine if the files of the job can be cached and served to peers and if the job can download
    ///   content from peers.
    /// </summary>
    [Flags]
    public enum PeerCachingOptions
    {
        /// <summary>
        ///   The job can download content from peers.The job will not download from a peer unless both the client
        ///   computer and the job allow BITS to download files from a peer.
        /// </summary>
        ClientPeerCaching = 0x0001, 

        /// <summary>
        ///   The files of the job can be cached and served to peers.BITS will not cache the files and serve them to
        ///   peers unless both the client computer and job allow BITS to cache and serve the files.
        /// </summary>
        ServerPeerCaching = 0x0002, 
    }
}