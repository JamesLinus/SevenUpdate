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
    /// </summary>
    internal static class KnownFoldersIidGuid
    {
        // IID GUID strings for relevant Shell COM interfaces.
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal const string IKnownFolder = "3AA7AF7E-9B36-420c-A8E3-F77D4674A488";

        /// <summary>
        /// </summary>
        internal const string IKnownFolderManager = "8BE2D872-86AA-4d47-B776-32CCA40C7018";

        #endregion
    }

    /// <summary>
    /// </summary>
    internal static class KnownFoldersClsidGuid
    {
        // CLSID GUID strings for relevant coclasses.
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal const string KnownFolderManager = "4df0c730-df9d-4ae3-9153-aa6b82e9795a";

        #endregion
    }

    /// <summary>
    /// </summary>
    internal static class KnownFoldersKfidGuid
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal const string ComputerFolder = "0AC0837C-BBF8-452A-850D-79D08E667CA7";

        /// <summary>
        /// </summary>
        internal const string Documents = "FDD39AD0-238F-46AF-ADB4-6C85480369C7";

        /// <summary>
        /// </summary>
        internal const string Favorites = "1777F761-68AD-4D8A-87BD-30B759FA33DD";

        /// <summary>
        /// </summary>
        internal const string Profile = "5E6C858F-0E22-4760-9AFE-EA3317B67173";

        #endregion
    }
}