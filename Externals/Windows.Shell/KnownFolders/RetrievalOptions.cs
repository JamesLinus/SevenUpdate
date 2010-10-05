//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Shell
{
    /// <summary>
    /// Contains special retrieval options for known folders.
    /// </summary>
    internal enum RetrievalOption
    {
        /// <summary>
        /// </summary>
        None = 0, 

        /// <summary>
        /// </summary>
        Create = 0x00008000, 

        /// <summary>
        /// </summary>
        DontVerify = 0x00004000, 

        /// <summary>
        /// </summary>
        DontUnexpand = 0x00002000, 

        /// <summary>
        /// </summary>
        NoAlias = 0x00001000, 

        /// <summary>
        /// </summary>
        Init = 0x00000800, 

        /// <summary>
        /// </summary>
        DefaultPath = 0x00000400, 

        /// <summary>
        /// </summary>
        NotParentRelative = 0x00000200
    }
}