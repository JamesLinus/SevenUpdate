#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace Microsoft.Windows.Shell
{
    internal static class KnownFoldersIIDGuid
    {
        // IID GUID strings for relevant Shell COM interfaces.
        internal const string IKnownFolder = "3AA7AF7E-9B36-420c-A8E3-F77D4674A488";
        internal const string IKnownFolderManager = "8BE2D872-86AA-4d47-B776-32CCA40C7018";
    }

    internal static class KnownFoldersCLSIDGuid
    {
        // CLSID GUID strings for relevant coclasses.
        internal const string KnownFolderManager = "4df0c730-df9d-4ae3-9153-aa6b82e9795a";
    }

    internal static class KnownFoldersKFIDGuid
    {
        internal const string ComputerFolder = "0AC0837C-BBF8-452A-850D-79D08E667CA7";
        internal const string Favorites = "1777F761-68AD-4D8A-87BD-30B759FA33DD";
        internal const string Documents = "FDD39AD0-238F-46AF-ADB4-6C85480369C7";
        internal const string Profile = "5E6C858F-0E22-4760-9AFE-EA3317B67173";
    }
}