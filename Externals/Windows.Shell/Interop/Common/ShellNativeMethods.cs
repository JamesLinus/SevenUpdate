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
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    /// <summary>
    /// </summary>
    internal static class ShellNativeMethods
    {
        #region TaskDialog Definitions

        // Identify button *return values* - note that, unfortunately, these are different
        // from the inbound button values.
        /// <summary>
        /// </summary>
        internal enum TaskdialogCommonButtonReturnID
        {
            /// <summary>
            /// </summary>
            IDOK = 1, 

            /// <summary>
            /// </summary>
            IDCANCEL = 2, 

            /// <summary>
            /// </summary>
            IDABORT = 3, 

            /// <summary>
            /// </summary>
            IDRETRY = 4, 

            /// <summary>
            /// </summary>
            IDIGNORE = 5, 

            /// <summary>
            /// </summary>
            IDYES = 6, 

            /// <summary>
            /// </summary>
            IDNO = 7, 

            /// <summary>
            /// </summary>
            IDCLOSE = 8
        }

        #endregion

        #region Shell Enums

        #region Nested type: CDCONTROLSTATE

        /// <summary>
        /// </summary>
        internal enum DCONTROLSTATE : uint
        {
            /// <summary>
            /// </summary>
            CdcsInactive = 0x00000000, 

            /// <summary>
            /// </summary>
            CdcsEnabled = 0x00000001, 

            /// <summary>
            /// </summary>
            CdcsVisible = 0x00000002
        }

        #endregion

        #region Nested type: FDAP

        /// <summary>
        /// </summary>
        internal enum FDAP
        {
            /// <summary>
            /// </summary>
            Bottom = 0x00000000, 

            /// <summary>
            /// </summary>
            Top = 0x00000001, 
        }

        #endregion

        #region Nested type: FDE_OVERWRITE_RESPONSE

        /// <summary>
        /// </summary>
        internal enum FdeOverwriteResponse
        {
            /// <summary>
            /// </summary>
            FdeorDefault = 0x00000000, 

            /// <summary>
            /// </summary>
            FdeorAccept = 0x00000001, 

            /// <summary>
            /// </summary>
            FdeorRefuse = 0x00000002
        }

        #endregion

        #region Nested type: FDE_SHAREVIOLATION_RESPONSE

        /// <summary>
        /// </summary>
        internal enum FdeShareviolationResponse
        {
            /// <summary>
            /// </summary>
            FdesvrDefault = 0x00000000, 

            /// <summary>
            /// </summary>
            FdesvrAccept = 0x00000001, 

            /// <summary>
            /// </summary>
            FdesvrRefuse = 0x00000002
        }

        #endregion

        #region Nested type: FOS

        /// <summary>
        /// </summary>
        [Flags]
        internal enum FOSs : uint
        {
            /// <summary>
            /// </summary>
            FosOverwriteprompt = 0x00000002, 

            /// <summary>
            /// </summary>
            FosStrictfiletypes = 0x00000004, 

            /// <summary>
            /// </summary>
            FosNochangedir = 0x00000008, 

            /// <summary>
            /// </summary>
            FosPickfolders = 0x00000020, 

            // Ensure that items returned are filesystem items.
            /// <summary>
            /// </summary>
            FosForcefilesystem = 0x00000040, 

            // Allow choosing items that have no storage.
            /// <summary>
            /// </summary>
            FosAllnonstorageitems = 0x00000080, 

            /// <summary>
            /// </summary>
            FosNovalidate = 0x00000100, 

            /// <summary>
            /// </summary>
            FosAllowmultiselect = 0x00000200, 

            /// <summary>
            /// </summary>
            FosPathmustexist = 0x00000800, 

            /// <summary>
            /// </summary>
            FosFilemustexist = 0x00001000, 

            /// <summary>
            /// </summary>
            FosCreateprompt = 0x00002000, 

            /// <summary>
            /// </summary>
            FosShareaware = 0x00004000, 

            /// <summary>
            /// </summary>
            FosNoreadonlyreturn = 0x00008000, 

            /// <summary>
            /// </summary>
            FosNotestfilecreate = 0x00010000, 

            /// <summary>
            /// </summary>
            FosHidemruplaces = 0x00020000, 

            /// <summary>
            /// </summary>
            FosHidepinnedplaces = 0x00040000, 

            /// <summary>
            /// </summary>
            FosNodereferencelinks = 0x00100000, 

            /// <summary>
            /// </summary>
            FosDontaddtorecent = 0x02000000, 

            /// <summary>
            /// </summary>
            FosForceshowhidden = 0x10000000, 

            /// <summary>
            /// </summary>
            FosDefaultnominimode = 0x20000000
        }

        #endregion

        #region Nested type: GETPROPERTYSTOREFLAGS

        /// <summary>
        /// Indicate flags that modify the property store object retrieved by methods 
        ///   that create a property store, such as IShellItem2::GetPropertyStore or 
        ///   IPropertyStoreFactory::GetPropertyStore.
        /// </summary>
        [Flags]
        internal enum GETPROPERTYSTOREFLAGSs : uint
        {
            /// <summary>
            ///   Meaning to a calling process: Return a read-only property store that contains all 
            ///   properties. Slow items (offline files) are not opened. 
            ///   Combination with other flags: Can be overridden by other flags.
            /// </summary>
            GpsDefault = 0, 

            /// <summary>
            ///   Meaning to a calling process: Include only properties directly from the property
            ///   handler, which opens the file on the disk, network, or device. Meaning to a file 
            ///   folder: Only include properties directly from the handler.
            /// 
            ///   Meaning to other folders: When delegating to a file folder, pass this flag on 
            ///   to the file folder; do not do any multiplexing (MUX). When not delegating to a 
            ///   file folder, ignore this flag instead of returning a failure code.
            /// 
            ///   Combination with other flags: Cannot be combined with GPS_TEMPORARY, 
            ///   GPS_FASTPROPERTIESONLY, or GPS_BESTEFFORT.
            /// </summary>
            GpsHandlerpropertiesonly = 0x1, 

            /// <summary>
            ///   Meaning to a calling process: Can write properties to the item. 
            ///   Note: The store may contain fewer properties than a read-only store. 
            /// 
            ///   Meaning to a file folder: ReadWrite.
            /// 
            ///   Meaning to other folders: ReadWrite. Note: When using default MUX, 
            ///   return a single unmultiplexed store because the default MUX does not support ReadWrite.
            /// 
            ///   Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_FASTPROPERTIESONLY, 
            ///   GPS_BESTEFFORT, or GPS_DELAYCREATION. Implies GPS_HANDLERPROPERTIESONLY.
            /// </summary>
            GpsReadwrite = 0x2, 

            /// <summary>
            ///   Meaning to a calling process: Provides a writable store, with no initial properties, 
            ///   that exists for the lifetime of the Shell item instance; basically, a property bag 
            ///   attached to the item instance. 
            /// 
            ///   Meaning to a file folder: Not applicable. Handled by the Shell item.
            /// 
            ///   Meaning to other folders: Not applicable. Handled by the Shell item.
            /// 
            ///   Combination with other flags: Cannot be combined with any other flag. Implies GPS_READWRITE
            /// </summary>
            GpsTemporary = 0x4, 

            /// <summary>
            ///   Meaning to a calling process: Provides a store that does not involve reading from the 
            ///   disk or network. Note: Some values may be different, or missing, compared to a store 
            ///   without this flag. 
            /// 
            ///   Meaning to a file folder: Include the "innate" and "fallback" stores only. Do not load the handler.
            /// 
            ///   Meaning to other folders: Include only properties that are available in memory or can 
            ///   be computed very quickly (no properties from disk, network, or peripheral IO devices). 
            ///   This is normally only data sources from the IDLIST. When delegating to other folders, pass this flag on to them.
            /// 
            ///   Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_READWRITE, 
            ///   GPS_HANDLERPROPERTIESONLY, or GPS_DELAYCREATION.
            /// </summary>
            GpsFastpropertiesonly = 0x8, 

            /// <summary>
            ///   Meaning to a calling process: Open a slow item (offline file) if necessary. 
            ///   Meaning to a file folder: Retrieve a file from offline storage, if necessary. 
            ///   Note: Without this flag, the handler is not created for offline files.
            /// 
            ///   Meaning to other folders: Do not return any properties that are very slow.
            /// 
            ///   Combination with other flags: Cannot be combined with GPS_TEMPORARY or GPS_FASTPROPERTIESONLY.
            /// </summary>
            GpsOpenslowitem = 0x10, 

            /// <summary>
            ///   Meaning to a calling process: Delay memory-intensive operations, such as file access, until 
            ///   a property is requested that requires such access. 
            /// 
            ///   Meaning to a file folder: Do not create the handler until needed; for example, either 
            ///   GetCount/GetAt or GetValue, where the innate store does not satisfy the request. 
            ///   Note: GetValue might fail due to file access problems.
            /// 
            ///   Meaning to other folders: If the folder has memory-intensive properties, such as 
            ///   delegating to a file folder or network access, it can optimize performance by 
            ///   supporting IDelayedPropertyStoreFactory and splitting up its properties into a 
            ///   fast and a slow store. It can then use delayed MUX to recombine them.
            /// 
            ///   Combination with other flags: Cannot be combined with GPS_TEMPORARY or 
            ///   GPS_READWRITE
            /// </summary>
            GpsDelaycreation = 0x20, 

            /// <summary>
            ///   Meaning to a calling process: Succeed at getting the store, even if some 
            ///   properties are not returned. Note: Some values may be different, or missing,
            ///   compared to a store without this flag. 
            /// 
            ///   Meaning to a file folder: Succeed and return a store, even if the handler or 
            ///   innate store has an error during creation. Only fail if substores fail.
            /// 
            ///   Meaning to other folders: Succeed on getting the store, even if some properties 
            ///   are not returned.
            /// 
            ///   Combination with other flags: Cannot be combined with GPS_TEMPORARY, 
            ///   GPS_READWRITE, or GPS_HANDLERPROPERTIESONLY.
            /// </summary>
            GpsBesteffort = 0x40, 

            /// <summary>
            ///   Mask for valid GETPROPERTYSTOREFLAGS values.
            /// </summary>
            GpsMaskValid = 0xff, 
        }

        #endregion

        #region Nested type: SFGAO

        /// <summary>
        /// </summary>
        [Flags]
        internal enum SFGAOs : uint
        {
            /// <summary>
            ///   The specified items can be copied.
            /// </summary>
            SfgaoCancopy = 0x00000001, 

            /// <summary>
            ///   The specified items can be moved.
            /// </summary>
            SfgaoCanmove = 0x00000002, 

            /// <summary>
            ///   Shortcuts can be created for the specified items. This flag has the same value as DROPEFFECT. 
            ///   The normal use of this flag is to add a Create Shortcut item to the shortcut menu that is displayed 
            ///   during drag-and-drop operations. However, SFGAO_CANLINK also adds a Create Shortcut item to the Microsoft 
            ///   Windows Explorer's File menu and to normal shortcut menus. 
            ///   If this item is selected, your application's IContextMenu::InvokeCommand is invoked with the lpVerb 
            ///   member of the CMINVOKECOMMANDINFO structure set to "link." Your application is responsible for creating the link.
            /// </summary>
            SfgaoCanlink = 0x00000004, 

            /// <summary>
            ///   The specified items can be bound to an IStorage interface through IShellFolder::BindToObject.
            /// </summary>
            SfgaoStorage = 0x00000008, 

            /// <summary>
            ///   The specified items can be renamed.
            /// </summary>
            SfgaoCanrename = 0x00000010, 

            /// <summary>
            ///   The specified items can be deleted.
            /// </summary>
            SfgaoCandelete = 0x00000020, 

            /// <summary>
            ///   The specified items have property sheets.
            /// </summary>
            SfgaoHaspropsheet = 0x00000040, 

            /// <summary>
            ///   The specified items are drop targets.
            /// </summary>
            SfgaoDroptarget = 0x00000100, 

            /// <summary>
            ///   This flag is a mask for the capability flags.
            /// </summary>
            SfgaoCapabilitymask = 0x00000177, 

            /// <summary>
            ///   Windows 7 and later. The specified items are system items.
            /// </summary>
            SfgaoSystem = 0x00001000, 

            /// <summary>
            ///   The specified items are encrypted.
            /// </summary>
            SfgaoEncrypted = 0x00002000, 

            /// <summary>
            ///   Indicates that accessing the object = through IStream or other storage interfaces, 
            ///   is a slow operation. 
            ///   Applications should avoid accessing items flagged with SFGAO_ISSLOW.
            /// </summary>
            SfgaoIsslow = 0x00004000, 

            /// <summary>
            ///   The specified items are ghosted icons.
            /// </summary>
            SfgaoGhosted = 0x00008000, 

            /// <summary>
            ///   The specified items are shortcuts.
            /// </summary>
            SfgaoLink = 0x00010000, 

            /// <summary>
            ///   The specified folder objects are shared.
            /// </summary>
            SfgaoShare = 0x00020000, 

            /// <summary>
            ///   The specified items are read-only. In the case of folders, this means 
            ///   that new items cannot be created in those folders.
            /// </summary>
            SfgaoReadonly = 0x00040000, 

            /// <summary>
            ///   The item is hidden and should not be displayed unless the 
            ///   Show hidden files and folders option is enabled in Folder Settings.
            /// </summary>
            SfgaoHidden = 0x00080000, 

            /// <summary>
            ///   This flag is a mask for the display attributes.
            /// </summary>
            SfgaoDisplayattrmask = 0x000FC000, 

            /// <summary>
            ///   The specified folders contain one or more file system folders.
            /// </summary>
            SfgaoFilesysancestor = 0x10000000, 

            /// <summary>
            ///   The specified items are folders.
            /// </summary>
            SfgaoFolder = 0x20000000, 

            /// <summary>
            ///   The specified folders or file objects are part of the file system 
            ///   that is, they are files, directories, or root directories).
            /// </summary>
            SfgaoFilesystem = 0x40000000, 

            /// <summary>
            ///   The specified folders have subfolders = and are, therefore, 
            ///   expandable in the left pane of Windows Explorer).
            /// </summary>
            SfgaoHassubfolder = 0x80000000, 

            /// <summary>
            ///   This flag is a mask for the contents attributes.
            /// </summary>
            SfgaoContentsmask = 0x80000000, 

            /// <summary>
            ///   When specified as input, SFGAO_VALIDATE instructs the folder to validate that the items 
            ///   pointed to by the contents of apidl exist. If one or more of those items do not exist, 
            ///   IShellFolder::GetAttributesOf returns a failure code. 
            ///   When used with the file system folder, SFGAO_VALIDATE instructs the folder to discard cached 
            ///   properties retrieved by clients of IShellFolder2::GetDetailsEx that may 
            ///   have accumulated for the specified items.
            /// </summary>
            SfgaoValidate = 0x01000000, 

            /// <summary>
            ///   The specified items are on removable media or are themselves removable devices.
            /// </summary>
            SfgaoRemovable = 0x02000000, 

            /// <summary>
            ///   The specified items are compressed.
            /// </summary>
            SfgaoCompressed = 0x04000000, 

            /// <summary>
            ///   The specified items can be browsed in place.
            /// </summary>
            SfgaoBrowsable = 0x08000000, 

            /// <summary>
            ///   The items are nonenumerated items.
            /// </summary>
            SfgaoNonenumerated = 0x00100000, 

            /// <summary>
            ///   The objects contain new content.
            /// </summary>
            SfgaoNewcontent = 0x00200000, 

            /// <summary>
            ///   It is possible to create monikers for the specified file objects or folders.
            /// </summary>
            SfgaoCanmoniker = 0x00400000, 

            /// <summary>
            ///   Not supported.
            /// </summary>
            SfgaoHasstorage = 0x00400000, 

            /// <summary>
            ///   Indicates that the item has a stream associated with it that can be accessed 
            ///   by a call to IShellFolder::BindToObject with IID_IStream in the riid parameter.
            /// </summary>
            SfgaoStream = 0x00400000, 

            /// <summary>
            ///   Children of this item are accessible through IStream or IStorage. 
            ///   Those children are flagged with SFGAO_STORAGE or SFGAO_STREAM.
            /// </summary>
            SfgaoStorageancestor = 0x00800000, 

            /// <summary>
            ///   This flag is a mask for the storage capability attributes.
            /// </summary>
            SfgaoStoragecapmask = 0x70C50008, 

            /// <summary>
            ///   Mask used by PKEY_SFGAOFlags to remove certain values that are considered 
            ///   to cause slow calculations or lack context. 
            ///   Equal to SFGAO_VALIDATE | SFGAO_ISSLOW | SFGAO_HASSUBFOLDER.
            /// </summary>
            SfgaoPkeysfgaomask = 0x81044000, 
        }

        #endregion

        #region Nested type: SHCONT

        /// <summary>
        /// </summary>
        [Flags]
        internal enum SHCONTs : ushort
        {
            /// <summary>
            /// </summary>
            ShcontfCheckingForChildren = 0x0010, 

            /// <summary>
            /// </summary>
            ShcontfFolders = 0x0020, 

            /// <summary>
            /// </summary>
            ShcontfNonfolders = 0x0040, 

            /// <summary>
            /// </summary>
            ShcontfIncludehidden = 0x0080, 

            /// <summary>
            /// </summary>
            ShcontfInitONFirstNext = 0x0100, 

            /// <summary>
            /// </summary>
            ShcontfNetprintersrch = 0x0200, 

            /// <summary>
            /// </summary>
            ShcontfShareable = 0x0400, 

            /// <summary>
            /// </summary>
            ShcontfStorage = 0x0800, 

            /// <summary>
            /// </summary>
            ShcontfNavigationEnum = 0x1000, 

            /// <summary>
            /// </summary>
            ShcontfFastitems = 0x2000, 

            /// <summary>
            /// </summary>
            ShcontfFlatlist = 0x4000, 

            /// <summary>
            /// </summary>
            ShcontfEnableAsync = 0x8000
        }

        #endregion

        #region Nested type: SIATTRIBFLAGS

        /// <summary>
        /// </summary>
        internal enum SIATTRIBFLAGS
        {
            // if multiple items and the attirbutes together.
            /// <summary>
            /// </summary>
            And = 0x00000001, 

            // if multiple items or the attributes together.
            /// <summary>
            /// </summary>
            OR = 0x00000002, 

            // Call GetAttributes directly on the 
            // ShellFolder for multiple attributes.
            /// <summary>
            /// </summary>
            Appcompat = 0x00000003, 
        }

        #endregion

        #region Nested type: SIGDN

        /// <summary>
        /// </summary>
        internal enum SIGDN : uint
        {
            /// <summary>
            /// </summary>
            Normaldisplay = 0x00000000, // SHGDN_NORMAL
            /// <summary>
            /// </summary>
            Parentrelativeparsing = 0x80018001, // SHGDN_INFOLDER | SHGDN_FORPARSING
            /// <summary>
            /// </summary>
            Desktopabsoluteparsing = 0x80028000, // SHGDN_FORPARSING
            /// <summary>
            /// </summary>
            Parentrelativeediting = 0x80031001, // SHGDN_INFOLDER | SHGDN_FOREDITING
            /// <summary>
            /// </summary>
            Desktopabsoluteediting = 0x8004c000, // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            /// <summary>
            /// </summary>
            Filesyspath = 0x80058000, // SHGDN_FORPARSING
            /// <summary>
            /// </summary>
            Url = 0x80068000, // SHGDN_FORPARSING
            /// <summary>
            /// </summary>
            Parentrelativeforaddressbar = 0x8007c001, // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            /// <summary>
            /// </summary>
            Parentrelative = 0x80080001 // SHGDN_INFOLDER
        }

        #endregion

        #region Nested type: SIIGBF

        /// <summary>
        /// </summary>
        [Flags]
        internal enum SIIGBFs
        {
            /// <summary>
            /// </summary>
            SiigbfResizetofit = 0x00, 

            /// <summary>
            /// </summary>
            SiigbfBiggersizeok = 0x01, 

            /// <summary>
            /// </summary>
            SiigbfMemoryonly = 0x02, 

            /// <summary>
            /// </summary>
            SiigbfIcononly = 0x04, 

            /// <summary>
            /// </summary>
            SiigbfThumbnailonly = 0x08, 

            /// <summary>
            /// </summary>
            SiigbfIncacheonly = 0x10, 
        }

        #endregion

        #region Nested type: WTS_ALPHATYPE

        /// <summary>
        /// </summary>
        internal enum WtsAlphatype
        {
            /// <summary>
            /// </summary>
            WtsatUnknown = 0, 

            /// <summary>
            /// </summary>
            WtsatRgb = 1, 

            /// <summary>
            /// </summary>
            WtsatArgb = 2, 
        }

        #endregion

        #region Nested type: WTS_CACHEFLAGS

        /// <summary>
        /// </summary>
        [Flags]
        internal enum WtsCacheflagss
        {
            /// <summary>
            /// </summary>
            WtsDefault = 0x00000000, 

            /// <summary>
            /// </summary>
            WtsLowquality = 0x00000001, 

            /// <summary>
            /// </summary>
            WtsCached = 0x00000002, 
        }

        #endregion

        #region Nested type: WTS_FLAGS

        /// <summary>
        /// </summary>
        [Flags]
        internal enum WtsFlagss
        {
            /// <summary>
            /// </summary>
            WtsExtract = 0x00000000, 

            /// <summary>
            /// </summary>
            WtsIncacheonly = 0x00000001, 

            /// <summary>
            /// </summary>
            WtsFastextract = 0x00000002, 

            /// <summary>
            /// </summary>
            WtsForceextraction = 0x00000004, 

            /// <summary>
            /// </summary>
            WtsSlowreclaim = 0x00000008, 

            /// <summary>
            /// </summary>
            WtsExtractdonotcache = 0x00000020
        }

        #endregion

        #endregion

        #region Shell Structs

        #region Nested type: COMDLG_FILTERSPEC

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct OmdlgFilterspec
        {
            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSpec;

            /// <summary>
            /// </summary>
            /// <param name="name">
            /// </param>
            /// <param name="spec">
            /// </param>
            internal OmdlgFilterspec(string name, string spec)
            {
                this.pszName = name;
                this.pszSpec = spec;
            }

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(OmdlgFilterspec x, OmdlgFilterspec y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(OmdlgFilterspec x, OmdlgFilterspec y)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Nested type: WTS_THUMBNAILID

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct WtsThumbnailid
        {
            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 16)]
            private readonly byte rgbKey;

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(WtsThumbnailid x, WtsThumbnailid y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <param name="x">
            /// </param>
            /// <param name="y">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(WtsThumbnailid x, WtsThumbnailid y)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #endregion

        #region Shell Helper Methods

        /// <summary>
        /// </summary>
        /// <param name="pdo">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="iShellItemArray">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateShellItemArrayFromDataObject(
            IDataObject pdo, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItemArray iShellItemArray);

        /// <summary>
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <param name="pbc">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="shellItem">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path, 
            // The following parameter is not used - binding context.
            IntPtr pbc, 
            ref Guid riid, 
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 shellItem);

        /// <summary>
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <param name="pbc">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="shellItem">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path, 
            [MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, 
            ref Guid riid, 
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 shellItem);

        /// <summary>
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <param name="pbc">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="shellItem">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path, 
            [MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, 
            ref Guid riid, 
            [MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);

        /// <summary>
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <param name="pbc">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="shellItem">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path, 
            // The following parameter is not used - binding context.
            IntPtr pbc, 
            ref Guid riid, 
            [MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppenum">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateShellItemArrayFromShellItem(IShellItem psi, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

        /// <summary>
        /// </summary>
        /// <param name="pszIconFile">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int PathParseIconLocation([MarshalAs(UnmanagedType.LPWStr)] ref string pszIconFile);

        /// <summary>
        /// </summary>
        /// <param name="pidl">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromIDList( /*PCIDLIST_ABSOLUTE*/ IntPtr pidl, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItem2 ppv);

        /// <summary>
        /// </summary>
        /// <param name="pszName">
        /// </param>
        /// <param name="pbc">
        /// </param>
        /// <param name="ppidl">
        /// </param>
        /// <param name="sfgaoIn">
        /// </param>
        /// <param name="psfgaoOut">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHParseDisplayName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszName, IntPtr pbc, out IntPtr ppidl, SFGAOs sfgaoIn, out SFGAOs psfgaoOut);

        /// <summary>
        /// </summary>
        /// <param name="iUnknown">
        /// </param>
        /// <param name="ppidl">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHGetIDListFromObject(IntPtr iUnknown, out IntPtr ppidl);

        /// <summary>
        /// </summary>
        /// <param name="ppshf">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHGetDesktopFolder([MarshalAs(UnmanagedType.Interface)] out IShellFolder ppshf);

        /// <summary>
        /// </summary>
        /// <param name="pidlParent">
        /// </param>
        /// <param name="psfParent">
        /// </param>
        /// <param name="pidl">
        /// </param>
        /// <param name="ppsi">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateShellItem(
            IntPtr pidlParent, [In] [MarshalAs(UnmanagedType.Interface)] IShellFolder psfParent, IntPtr pidl, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="pidl">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint ILGetSize(IntPtr pidl);

        /// <summary>
        /// </summary>
        /// <param name="pidl">
        /// </param>
        [DllImport("shell32.dll", CharSet = CharSet.None)]
        public static extern void ILFree(IntPtr pidl);

        /// <summary>
        /// </summary>
        /// <param name="hObject">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// </summary>
        /// <param name="reserved">
        /// </param>
        /// <param name="ppbc">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        #endregion

        #region Shell Library Enums

        #region Nested type: DEFAULTSAVEFOLDERTYPE

        /// <summary>
        /// </summary>
        internal enum DEFAULTSAVEFOLDERTYPE
        {
            /// <summary>
            /// </summary>
            DsftDetect = 1, 

            /// <summary>
            /// </summary>
            DsftPrivate = DsftDetect + 1, 

            /// <summary>
            /// </summary>
            DsftPublic = DsftPrivate + 1
        } ;

        #endregion

        #region Nested type: LIBRARYFOLDERFILTER

        /// <summary>
        /// </summary>
        internal enum LIBRARYFOLDERFILTER
        {
            /// <summary>
            /// </summary>
            LffForcefilesystem = 1, 

            /// <summary>
            /// </summary>
            LffStorageitems = 2, 

            /// <summary>
            /// </summary>
            LffAllitems = 3
        } ;

        #endregion

        #region Nested type: LIBRARYMANAGEDIALOGOPTIONS

        /// <summary>
        /// </summary>
        internal enum LIBRARYMANAGEDIALOGOPTIONS
        {
            /// <summary>
            /// </summary>
            LmdDefault = 0, 

            /// <summary>
            /// </summary>
            LmdNounindexablelocationwarning = 0x1
        } ;

        #endregion

        #region Nested type: LIBRARYOPTIONFLAGS

        /// <summary>
        /// </summary>
        [Flags]
        internal enum LIBRARYOPTIONFLAGSs : uint
        {
            /// <summary>
            /// </summary>
            LofDefault = 0, 

            /// <summary>
            /// </summary>
            LofPinnedtonavpane = 0x1, 

            /// <summary>
            /// </summary>
            LofMaskAll = 0x1
        } ;

        #endregion

        #region Nested type: LIBRARYSAVEFLAGS

        /// <summary>
        /// </summary>
        internal enum LIBRARYSAVEFLAGS
        {
            /// <summary>
            /// </summary>
            LsfFailifthere = 0, 

            /// <summary>
            /// </summary>
            LsfOverrideexisting = 0x1, 

            /// <summary>
            /// </summary>
            LsfMakeuniquename = 0x2
        } ;

        #endregion

        #region Nested type: STGM

        /// <summary>
        /// The STGM constants are flags that indicate 
        ///   conditions for creating and deleting the object and access modes 
        ///   for the object. 
        /// 
        ///   You can combine these flags, but you can only choose one flag 
        ///   from each group of related flags. Typically one flag from each 
        ///   of the access and sharing groups must be specified for all 
        ///   functions and methods which use these constants.
        /// </summary>
        [Flags]
        internal enum STGMs : uint
        {
            /// <summary>
            ///   Indicates that, in direct mode, each change to a storage 
            ///   or stream element is written as it occurs.
            /// </summary>
            Direct = 0x00000000, 

            /// <summary>
            ///   Indicates that, in transacted mode, changes are buffered 
            ///   and written only if an explicit commit operation is called.
            /// </summary>
            Transacted = 0x00010000, 

            /// <summary>
            ///   Provides a faster implementation of a compound file 
            ///   in a limited, but frequently used, case.
            /// </summary>
            Simple = 0x08000000, 

            /// <summary>
            ///   Indicates that the object is read-only, 
            ///   meaning that modifications cannot be made.
            /// </summary>
            Read = 0x00000000, 

            /// <summary>
            ///   Enables you to save changes to the object, 
            ///   but does not permit access to its data.
            /// </summary>
            Write = 0x00000001, 

            /// <summary>
            ///   Enables access and modification of object data.
            /// </summary>
            ReadWrite = 0x00000002, 

            /// <summary>
            ///   Specifies that subsequent openings of the object are 
            ///   not denied read or write access.
            /// </summary>
            ShareDenyNone = 0x00000040, 

            /// <summary>
            ///   Prevents others from subsequently opening the object in Read mode.
            /// </summary>
            ShareDenyRead = 0x00000030, 

            /// <summary>
            ///   Prevents others from subsequently opening the object 
            ///   for Write or ReadWrite access.
            /// </summary>
            ShareDenyWrite = 0x00000020, 

            /// <summary>
            ///   Prevents others from subsequently opening the object in any mode.
            /// </summary>
            ShareExclusive = 0x00000010, 

            /// <summary>
            ///   Opens the storage object with exclusive access to the most 
            ///   recently committed version.
            /// </summary>
            Priority = 0x00040000, 

            /// <summary>
            ///   Indicates that the underlying file is to be automatically destroyed when the root 
            ///   storage object is released. This feature is most useful for creating temporary files.
            /// </summary>
            DeleteOnRelease = 0x04000000, 

            /// <summary>
            ///   Indicates that, in transacted mode, a temporary scratch file is usually used 
            ///   to save modifications until the Commit method is called. 
            ///   Specifying NoScratch permits the unused portion of the original file 
            ///   to be used as work space instead of creating a new file for that purpose.
            /// </summary>
            NoScratch = 0x00100000, 

            /// <summary>
            ///   Indicates that an existing storage object 
            ///   or stream should be removed before the new object replaces it.
            /// </summary>
            Create = 0x00001000, 

            /// <summary>
            ///   Creates the new object while preserving existing data in a stream named "Contents".
            /// </summary>
            Convert = 0x00020000, 

            /// <summary>
            ///   Causes the create operation to fail if an existing object with the specified name exists.
            /// </summary>
            FailIfThere = 0x00000000, 

            /// <summary>
            ///   This flag is used when opening a storage object with Transacted 
            ///   and without ShareExclusive or ShareDenyWrite. 
            ///   In this case, specifying NoSnapshot prevents the system-provided 
            ///   implementation from creating a snapshot copy of the file. 
            ///   Instead, changes to the file are written to the end of the file.
            /// </summary>
            NoSnapshot = 0x00200000, 

            /// <summary>
            ///   Supports direct mode for single-writer, multireader file operations.
            /// </summary>
            DirectSwmr = 0x00400000
        } ;

        #endregion

        #endregion

        #region Shell Library Helper Methods

        /// <summary>
        /// </summary>
        /// <param name="library">
        /// </param>
        /// <param name="hwndOwner">
        /// </param>
        /// <param name="title">
        /// </param>
        /// <param name="instruction">
        /// </param>
        /// <param name="lmdOptions">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("Shell32", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        internal static extern int SHShowManageLibraryUI(
            [In] [MarshalAs(UnmanagedType.Interface)] IShellItem library, 
            [In] IntPtr hwndOwner, 
            [In] string title, 
            [In] string instruction, 
            [In] LIBRARYMANAGEDIALOGOPTIONS lmdOptions);

        #endregion

        #region Command Link Definitions

        /// <summary>
        /// </summary>
        internal const int BSCommandlink = 0x0000000E;

        /// <summary>
        /// </summary>
        internal const uint BcmSetnote = 0x00001609;

        /// <summary>
        /// </summary>
        internal const uint BcmGetnote = 0x0000160A;

        /// <summary>
        /// </summary>
        internal const uint BcmGetnotelength = 0x0000160B;

        /// <summary>
        /// </summary>
        internal const uint BcmSetshield = 0x0000160C;

        #endregion
    }
}