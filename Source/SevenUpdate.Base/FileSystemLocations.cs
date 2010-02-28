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

#region

using System;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace SevenUpdate.Base
{
    internal static class NativeMethods
    {
        /// <summary>
        /// Gets the system folder(s) of a path
        /// </summary>
        /// <returns>a string of the path with expanded system variables</returns>
        [DllImport("shell32.dll")] // ReSharper disable InconsistentNaming
        internal static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);
    }

    /// <summary>
    /// Defines constants for file system locations
    /// </summary>
    internal static class FileSystemLocations
    {
        /// <summary>
        /// ALLUSERS%\Start Menu\Programs
        /// </summary>
        internal const int CSIDL_COMMON_PROGRAMS = 0x0017;

        /// <summary>
        /// %ALLUSERS%\Start Menu
        /// </summary>
        internal const int CSIDL_COMMON_STARTMENU = 0x0016;

        ///// <summary>
        ///// %USERNAME%\Start Menu\Programs\Administrative Tools
        ///// </summary>
        //internal const int CSIDL_ADMINTOOLS = 0x0030;

        ///// <summary>
        ///// The non localized startup folder
        ///// </summary>
        //internal const int CSIDL_ALTSTARTUP = 0x001d;

        ///// <summary>
        ///// %USERNAME%\Application Data or %USERNAME%\AppData
        ///// </summary>
        //internal const int CSIDL_APPDATA = 0x001a;

        ///// <summary>
        ///// %DESKTOP%\RECYCLE BIN
        ///// </summary>
        //internal const int CSIDL_BITBUCKET = 0x000a;

        ///// <summary>
        ///// %USERPROFILE%\Local Settings\Application Data\Microsoft\CD Burning
        ///// </summary>
        //internal const int CSIDL_CDBURN_AREA = 0x003b;

        ///// <summary>
        ///// %ALLUSERS%\Start Menu\Programs\Administrative Tools
        ///// </summary>
        //internal const int CSIDL_COMMON_ADMINTOOLS = 0x002f;

        ///// <summary>
        ///// The non localized %ALLUSERS% startup folder
        ///// </summary>
        //internal const int CSIDL_COMMON_ALTSTARTUP = 0x001e;

        ///// <summary>
        ///// %ALLUSERS%\Application Data
        ///// </summary>
        //internal const int CSIDL_COMMON_APPDATA = 0x0023;

        ///// <summary>
        ///// %ALLUSERS%\Desktop
        ///// </summary>
        //internal const int CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019;

        ///// <summary>
        ///// %ALLUSERS%\Documents
        ///// </summary>
        //internal const int CSIDL_COMMON_DOCUMENTS = 0x002e;

        ///// <summary></summary>
        //internal const int CSIDL_COMMON_FAVORITES = 0x001f;

        ///// <summary>
        ///// %ALLUSERS%\My Music
        ///// </summary>
        //internal const int CSIDL_COMMON_MUSIC = 0x0035;

        ///// <summary>
        ///// %ALLUSERS%\My Pictures
        ///// </summary>
        //internal const int CSIDL_COMMON_PICTURES = 0x0036;

        ///// <summary>
        ///// %USERNAME%\My Documents
        ///// </summary>
        //internal const int CSIDL_PERSONAL = 0x0005;

        ///// <summary>
        ///// %ALLUSERS%\Startup
        ///// </summary>
        //internal const int CSIDL_COMMON_STARTUP = 0x0018;

        ///// <summary>
        ///// %ALLUSERS%\Templates
        ///// </summary>
        //internal const int CSIDL_COMMON_TEMPLATES = 0x002d;

        ///// <summary>
        ///// %ALLUSERS%\My Video
        ///// </summary>
        //internal const int CSIDL_COMMON_VIDEO = 0x0037;

        ///// <summary>
        ///// Computers Near Me (computers from Work group membership)
        ///// </summary>
        //internal const int CSIDL_COMPUTERSNEARME = 0x003d;

        ///// <summary>
        ///// Network and Dial-up Connections
        ///// </summary>
        //internal const int CSIDL_CONNECTIONS = 0x0031;

        ///// <summary>
        ///// My Computer\Control Panel
        ///// </summary>
        //internal const int CSIDL_CONTROLS = 0x0003;

        ///// <summary>%DESKTOP%</summary>
        //internal const int CSIDL_DESKTOP = 0x0000;

        ///// <summary>
        ///// %USERNAME%\Desktop
        ///// </summary>
        //internal const int CSIDL_DESKTOPDIRECTORY = 0x0010;

        ///// <summary>My Computer</summary>
        //internal const int CSIDL_DRIVES = 0x0011;

        ///// <summary>
        ///// %USERNAME%\Favorites
        ///// </summary>
        //internal const int CSIDL_FAVORITES = 0x0006;

        ///// <summary>
        ///// combine with CSIDL_ value to force folder creation in SHGetFolderPath()
        ///// </summary>
        //internal const int CSIDL_FLAG_CREATE = 0x8000;

        ///// <summary>
        ///// combine with CSIDL_ value to avoid un-expanding environment variables
        ///// </summary>
        //internal const int CSIDL_FLAG_DONT_UNEXPAND = 0x2000;

        ///// <summary>
        ///// combine with CSIDL_ value to return an unverified folder path
        ///// </summary>
        //internal const int CSIDL_FLAG_DONT_VERIFY = 0x4000;

        ///// <summary>
        ///// combine with CSIDL_ value to insure non-alias versions of the pidl
        ///// </summary>
        //internal const int CSIDL_FLAG_NO_ALIAS = 0x1000;

        ///// <summary>
        ///// combine with CSIDL_ value to indicate per-user init (eg. upgrade)
        ///// </summary>
        //internal const int CSIDL_FLAG_PER_USER_INIT = 0x0800;

        ///// <summary>windows\fonts</summary>
        //internal const int CSIDL_FONTS = 0x0014;

        ///// <summary>
        ///// Internet Explorer (icon on desktop)
        ///// </summary>
        //internal const int CSIDL_INTERNET = 0x0001;

        ///// <summary>%tmp%</summary>
        //internal const int CSIDL_INTERNET_CACHE = 0x0020;

        ///// <summary>
        ///// %USERNAME%\Local Settings\Application Data (non roaming)
        ///// </summary>
        //internal const int CSIDL_LOCAL_APPDATA = 0x001c;

        ///// <summary>
        ///// %USERNAME%\My Music
        ///// </summary>
        //internal const int CSIDL_MYMUSIC = 0x000d;

        ///// <summary>
        ///// %USERNAME%\My Pictures
        ///// </summary>
        //internal const int CSIDL_MYPICTURES = 0x0027;

        ///// <summary>
        ///// %USERNAME%\My Videos
        ///// </summary>
        //internal const int CSIDL_MYVIDEO = 0x000e;

        ///// <summary>
        ///// %USERNAME%\nethood
        ///// </summary>
        //internal const int CSIDL_NETHOOD = 0x0013;

        ///// <summary>
        ///// Network Neighborhood (My Network Places)
        ///// </summary>
        //internal const int CSIDL_NETWORK = 0x0012;

        ///// <summary>
        ///// My Computer\Printers
        ///// </summary>
        //internal const int CSIDL_PRINTERS = 0x0004;

        ///// <summary>
        ///// %USERNAME%\PrintHood
        ///// </summary>
        //internal const int CSIDL_PRINTHOOD = 0x001b;

        ///// <summary>%USERPROFILE%</summary>
        //internal const int CSIDL_PROFILE = 0x0028;

        ///// <summary>%PROGRAMFILES%</summary>
        //internal const int CSIDL_PROGRAM_FILES = 0x0026;

        ///// <summary>
        ///// %PROGRAMFILES%\Common
        ///// </summary>
        //internal const int CSIDL_PROGRAM_FILES_COMMON = 0x002b;

        ///// <summary>
        ///// %PROGRAMFILES(x86)%\Common
        ///// </summary>
        //internal const int CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c;

        ///// <summary>
        ///// /// %PROGRAMFILES(x86)%\
        ///// </summary>
        //internal const int CSIDL_PROGRAM_FILESX86 = 0x002a;

        ///// <summary>
        ///// %STARTMENU%\Programs
        ///// </summary>
        //internal const int CSIDL_PROGRAMS = 0x0002;

        ///// <summary>
        ///// %USERNAME%\Recent
        ///// </summary>
        //internal const int CSIDL_RECENT = 0x0008;

        ///// <summary>
        ///// %USERNAME%\SendTo
        ///// </summary>
        //internal const int CSIDL_SENDTO = 0x0009;

        ///// <summary>
        ///// %USERNAME%\Start Menu
        ///// </summary>
        //internal const int CSIDL_STARTMENU = 0x000b;

        ///// <summary>
        ///// %STARTMENU%\Programs\Startup
        ///// </summary>
        //internal const int CSIDL_STARTUP = 0x0007;

        ///// <summary>
        ///// GetSystemDirectory()
        ///// </summary>
        //internal const int CSIDL_SYSTEM = 0x0025;

        ///// <summary>
        ///// x86 system directory
        ///// </summary>
        //internal const int CSIDL_SYSTEMX86 = 0x0029;

        ///// <summary>
        ///// %ALLUSERS%\Templates
        ///// </summary>
        //internal const int CSIDL_TEMPLATES = 0x0015;

        ///// <summary>%WINDOWS%</summary>
        //internal const int CSIDL_WINDOWS = 0x0024;
    }
}