﻿#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
// 
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace SevenUpdate
{
    internal static class FileSystemLocations
    {
        public const int CSIDL_ADMINTOOLS = 0x0030; // <user name>\Start Menu\Programs\Administrative Tools
        public const int CSIDL_ALTSTARTUP = 0x001d; // non localized startup
        public const int CSIDL_APPDATA = 0x001a; // <user name>\Application Data
        public const int CSIDL_BITBUCKET = 0x000a; // <desktop>\Recycle Bin
        public const int CSIDL_CDBURN_AREA = 0x003b; // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
        public const int CSIDL_COMMON_ADMINTOOLS = 0x002f; // All Users\Start Menu\Programs\Administrative Tools
        public const int CSIDL_COMMON_ALTSTARTUP = 0x001e; // non localized common startup
        public const int CSIDL_COMMON_APPDATA = 0x0023; // All Users\Application Data
        public const int CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019; // All Users\Desktop
        public const int CSIDL_COMMON_DOCUMENTS = 0x002e; // All Users\Documents
        public const int CSIDL_COMMON_FAVORITES = 0x001f;
        public const int CSIDL_COMMON_MUSIC = 0x0035; // All Users\My Music
        public const int CSIDL_COMMON_OEM_LINKS = 0x003a; // Links to All Users OEM specific apps
        public const int CSIDL_COMMON_PICTURES = 0x0036; // All Users\My Pictures
        public const int CSIDL_COMMON_PROGRAMS = 0x0017; // All Users\Start Menu\Programs
        public const int CSIDL_COMMON_STARTMENU = 0x0016; // All Users\Start Menu
        public const int CSIDL_COMMON_STARTUP = 0x0018; // All Users\Startup
        public const int CSIDL_COMMON_TEMPLATES = 0x002d; // All Users\Templates
        public const int CSIDL_COMMON_VIDEO = 0x0037; // All Users\My Video
        public const int CSIDL_COMPUTERSNEARME = 0x003d; // Computers Near Me (computered from Workgroup membership)
        public const int CSIDL_CONNECTIONS = 0x0031; // Network and Dial-up Connections
        public const int CSIDL_CONTROLS = 0x0003; // My Computer\Control Panel
        public const int CSIDL_COOKIES = 0x0021;
        public const int CSIDL_DESKTOP = 0x0000; // <desktop>
        public const int CSIDL_DESKTOPDIRECTORY = 0x0010; // <user name>\Desktop
        public const int CSIDL_DRIVES = 0x0011; // My Computer
        public const int CSIDL_FAVORITES = 0x0006; // <user name>\Favorites
        public const int CSIDL_FLAG_CREATE = 0x8000; // combine with CSIDL_ value to force folder creation in SHGetFolderPath()
        public const int CSIDL_FLAG_DONT_UNEXPAND = 0x2000; // combine with CSIDL_ value to avoid unexpanding environment variables
        public const int CSIDL_FLAG_DONT_VERIFY = 0x4000; // combine with CSIDL_ value to return an unverified folder path
        public const int CSIDL_FLAG_NO_ALIAS = 0x1000; // combine with CSIDL_ value to insure non-alias versions of the pidl
        public const int CSIDL_FLAG_PER_USER_INIT = 0x0800; // combine with CSIDL_ value to indicate per-user init (eg. upgrade)
        public const int CSIDL_FONTS = 0x0014; // windows\fonts
        public const int CSIDL_HISTORY = 0x0022;
        public const int CSIDL_INTERNET = 0x0001; // Internet Explorer (icon on desktop)
        public const int CSIDL_INTERNET_CACHE = 0x0020;
        public const int CSIDL_LOCAL_APPDATA = 0x001c; // <user name>\Local Settings\Applicaiton Data (non roaming)
        public const int CSIDL_MYDOCUMENTS = CSIDL_PERSONAL; //  Personal was just a silly name for My Documents
        public const int CSIDL_MYMUSIC = 0x000d; // "My Music" folder
        public const int CSIDL_MYPICTURES = 0x0027; // C:\Program Files\My Pictures
        public const int CSIDL_MYVIDEO = 0x000e; // "My Videos" folder
        public const int CSIDL_NETHOOD = 0x0013; // <user name>\nethood
        public const int CSIDL_NETWORK = 0x0012; // Network Neighborhood (My Network Places)
        public const int CSIDL_PERSONAL = 0x0005; // My Documents
        public const int CSIDL_PRINTERS = 0x0004; // My Computer\Printers
        public const int CSIDL_PRINTHOOD = 0x001b; // <user name>\PrintHood
        public const int CSIDL_PROFILE = 0x0028; // USERPROFILE
        public const int CSIDL_PROGRAM_FILES = 0x0026; // C:\Program Files
        public const int CSIDL_PROGRAM_FILES_COMMON = 0x002b; // C:\Program Files\Common
        public const int CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c; // x86 Program Files\Common on RISC
        public const int CSIDL_PROGRAM_FILESX86 = 0x002a; // x86 C:\Program Files on RISC
        public const int CSIDL_PROGRAMS = 0x0002; // Start Menu\Programs
        public const int CSIDL_RECENT = 0x0008; // <user name>\Recent
        public const int CSIDL_RESOURCES = 0x0038; // Resource Direcotry
        public const int CSIDL_RESOURCES_LOCALIZED = 0x0039; // Localized Resource Direcotry
        public const int CSIDL_SENDTO = 0x0009; // <user name>\SendTo
        public const int CSIDL_STARTMENU = 0x000b; // <user name>\Start Menu
        public const int CSIDL_STARTUP = 0x0007; // Start Menu\Programs\Startup
        public const int CSIDL_SYSTEM = 0x0025; // GetSystemDirectory()
        public const int CSIDL_SYSTEMX86 = 0x0029; // x86 system directory on RISC
        public const int CSIDL_TEMPLATES = 0x0015;
        public const int CSIDL_WINDOWS = 0x0024; // GetWindowsDirectory()
    }
}