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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.Windows.Internal;
using Size = System.Windows.Size;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Represents a thumbnail or an icon for a ShellObject.
    /// </summary>
    public class ShellThumbnail
    {
        #region Private members

        /// <summary>
        ///   Native shellItem
        /// </summary>
        private readonly IShellItem shellItemNative;

        /// <summary>
        ///   Internal member to keep track of the current size
        /// </summary>
        private Size currentSize = new Size(256, 256);

        #endregion

        #region Constructors

        /// <summary>
        ///   Internal constructor that takes in a parent ShellObject.
        /// </summary>
        /// <param name = "shellObject" />
        internal ShellThumbnail(ShellObject shellObject)
        {
            if (shellObject == null || shellObject.NativeShellItem == null)
                throw new ArgumentNullException("shellObject");

            shellItemNative = shellObject.NativeShellItem;
        }

        #endregion

        #region Public properties

        private ShellThumbnailFormatOptions formatOption = ShellThumbnailFormatOptions.Default;

        /// <summary>
        ///   Gets or sets the default size of the thumbnail or icon. The default is 32x32 pixels for icons and 
        ///   256x256 pixels for thumbnails.
        /// </summary>
        /// <remarks>
        ///   If the size specified is larger than the maximum size of 1024x1024 for thumbnails and 256x256 for icons,
        ///   an <see cref = "System.ArgumentOutOfRangeException" /> is thrown.
        /// </remarks>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        public Size CurrentSize
        {
            get { return currentSize; }
            set
            {
                // Check for 0; negative number check not required as System.Windows.Size only allows positive numbers.
                if (value.Height == 0 || value.Width == 0)
                    throw new ArgumentOutOfRangeException("value", "CurrentSize (width or height) cannot be 0");

                if (FormatOption == ShellThumbnailFormatOptions.IconOnly)
                {
                    if (value.Height > DefaultIconSize.Maximum.Height || value.Width > DefaultIconSize.Maximum.Width)
                        throw new ArgumentOutOfRangeException("value", string.Format("CurrentSize (width or height) cannot be greater than the maximum size"));
                }
                else
                {
                    // Covers the default mode (Thumbnail Or Icon) as well as ThumbnailOnly
                    if (value.Height > DefaultThumbnailSize.Maximum.Height || value.Width > DefaultThumbnailSize.Maximum.Width)
                        throw new ArgumentOutOfRangeException("value", string.Format("CurrentSize (width or height) cannot be greater than the maximum size"));
                }

                currentSize = value;
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon image in <see cref = "System.Drawing.Bitmap" /> format.
        ///   Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public Bitmap Bitmap { get { return GetBitmap(CurrentSize); } }

        /// <summary>
        ///   Gets the thumbnail or icon image in <see cref = "System.Windows.Media.Imaging.BitmapSource" /> format. 
        ///   Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public BitmapSource BitmapSource { get { return GetBitmapSource(CurrentSize); } }

        /// <summary>
        ///   Gets the thumbnail or icon image in <see cref = "System.Drawing.Icon" /> format. 
        ///   Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public Icon Icon { get { return Icon.FromHandle(Bitmap.GetHicon()); } }

        /// <summary>
        ///   Gets the thumbnail or icon in small size and <see cref = "System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap SmallBitmap { get { return FormatOption == ShellThumbnailFormatOptions.IconOnly ? GetBitmap(DefaultIconSize.Small) : GetBitmap(DefaultThumbnailSize.Small); } }

        /// <summary>
        ///   Gets the thumbnail or icon in small size and <see cref = "System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource SmallBitmapSource { get { return FormatOption == ShellThumbnailFormatOptions.IconOnly ? GetBitmapSource(DefaultIconSize.Small) : GetBitmapSource(DefaultThumbnailSize.Small); } }

        /// <summary>
        ///   Gets the thumbnail or icon in small size and <see cref = "System.Drawing.Icon" /> format.
        /// </summary>
        public Icon SmallIcon { get { return Icon.FromHandle(SmallBitmap.GetHicon()); } }

        /// <summary>
        ///   Gets the thumbnail or icon in Medium size and <see cref = "System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap MediumBitmap { get { return FormatOption == ShellThumbnailFormatOptions.IconOnly ? GetBitmap(DefaultIconSize.Medium) : GetBitmap(DefaultThumbnailSize.Medium); } }

        /// <summary>
        ///   Gets the thumbnail or icon in medium size and <see cref = "System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource MediumBitmapSource { get { return FormatOption == ShellThumbnailFormatOptions.IconOnly ? GetBitmapSource(DefaultIconSize.Medium) : GetBitmapSource(DefaultThumbnailSize.Medium); } }

        /// <summary>
        ///   Gets the thumbnail or icon in Medium size and <see cref = "System.Drawing.Icon" /> format.
        /// </summary>
        public Icon MediumIcon { get { return Icon.FromHandle(MediumBitmap.GetHicon()); } }

        /// <summary>
        ///   Gets the thumbnail or icon in large size and <see cref = "System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap LargeBitmap { get { return FormatOption == ShellThumbnailFormatOptions.IconOnly ? GetBitmap(DefaultIconSize.Large) : GetBitmap(DefaultThumbnailSize.Large); } }

        /// <summary>
        ///   Gets the thumbnail or icon in large size and <see cref = "System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource LargeBitmapSource { get { return FormatOption == ShellThumbnailFormatOptions.IconOnly ? GetBitmapSource(DefaultIconSize.Large) : GetBitmapSource(DefaultThumbnailSize.Large); } }

        /// <summary>
        ///   Gets the thumbnail or icon in Large size and <see cref = "System.Drawing.Icon" /> format.
        /// </summary>
        public Icon LargeIcon { get { return Icon.FromHandle(LargeBitmap.GetHicon()); } }

        /// <summary>
        ///   Gets the thumbnail or icon in extra large size and <see cref = "System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap ExtraLargeBitmap { get { return FormatOption == ShellThumbnailFormatOptions.IconOnly ? GetBitmap(DefaultIconSize.ExtraLarge) : GetBitmap(DefaultThumbnailSize.ExtraLarge); } }

        /// <summary>
        ///   Gets the thumbnail or icon in Extra Large size and <see cref = "System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource ExtraLargeBitmapSource { get { return FormatOption == ShellThumbnailFormatOptions.IconOnly ? GetBitmapSource(DefaultIconSize.ExtraLarge) : GetBitmapSource(DefaultThumbnailSize.ExtraLarge); } }

        /// <summary>
        ///   Gets the thumbnail or icon in Extra Large size and <see cref = "System.Drawing.Icon" /> format.
        /// </summary>
        public Icon ExtraLargeIcon { get { return Icon.FromHandle(ExtraLargeBitmap.GetHicon()); } }

        /// <summary>
        ///   Gets or sets a value that determines if the current retrieval option is cache or extract, cache only, or from memory only.
        ///   The default is cache or extract.
        /// </summary>
        public ShellThumbnailRetrievalOptions RetrievalOption { get; set; }

        /// <summary>
        ///   Gets or sets a value that determines if the current format option is thumbnail or icon, thumbnail only, or icon only.
        ///   The default is thumbnail or icon.
        /// </summary>
        public ShellThumbnailFormatOptions FormatOption
        {
            get { return formatOption; }
            set
            {
                formatOption = value;

                // Do a similar check as we did in CurrentSize property setter,
                // If our mode is IconOnly, then our max is defined by DefaultIconSize.Maximum. We should make sure 
                // our CurrentSize is within this max range
                if (FormatOption != ShellThumbnailFormatOptions.IconOnly)
                    return;
                if (CurrentSize.Height > DefaultIconSize.Maximum.Height || CurrentSize.Width > DefaultIconSize.Maximum.Width)
                    CurrentSize = DefaultIconSize.Maximum;
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines if the user can manually stretch the returned image.
        ///   The default value is false.
        /// </summary>
        /// <remarks>
        ///   For example, if the caller passes in 80x80 a 96x96 thumbnail could be returned. 
        ///   This could be used as a performance optimization if the caller will need to stretch 
        ///   the image themselves anyway. Note that the Shell implementation performs a GDI stretch blit. 
        ///   If the caller wants a higher quality image stretch, they should pass this flag and do it themselves.
        /// </remarks>
        public bool AllowBiggerSize { get; set; }

        #endregion

        #region Private Methods

        private ShellNativeMethods.SIIGBF CalculateFlags()
        {
            ShellNativeMethods.SIIGBF flags = 0x0000;

            if (AllowBiggerSize)
                flags |= ShellNativeMethods.SIIGBF.SIIGBF_BIGGERSIZEOK;

            switch (RetrievalOption)
            {
                case ShellThumbnailRetrievalOptions.CacheOnly:
                    flags |= ShellNativeMethods.SIIGBF.SIIGBF_INCACHEONLY;
                    break;
                case ShellThumbnailRetrievalOptions.MemoryOnly:
                    flags |= ShellNativeMethods.SIIGBF.SIIGBF_MEMORYONLY;
                    break;
            }

            switch (FormatOption)
            {
                case ShellThumbnailFormatOptions.IconOnly:
                    flags |= ShellNativeMethods.SIIGBF.SIIGBF_ICONONLY;
                    break;
                case ShellThumbnailFormatOptions.ThumbnailOnly:
                    flags |= ShellNativeMethods.SIIGBF.SIIGBF_THUMBNAILONLY;
                    break;
            }

            return flags;
        }

        private IntPtr GetHBitmap(Size size)
        {
            IntPtr hbitmap;

            // Create a size structure to pass to the native method
            var nativeSIZE = new CoreNativeMethods.SIZE {cx = Convert.ToInt32(size.Width), cy = Convert.ToInt32(size.Height)};

            // Use IShellItemImageFactory to get an icon
            // Options passed in: Resize to fit
            var hr = ((IShellItemImageFactory) shellItemNative).GetImage(nativeSIZE, CalculateFlags(), out hbitmap);

            if (hr == HRESULT.S_OK)
                return hbitmap;
            if ((uint) hr == 0x8004B200 && FormatOption == ShellThumbnailFormatOptions.ThumbnailOnly)
            {
                // Thumbnail was requested, but this ShellItem doesn't have a thumbnail.
                throw new InvalidOperationException("The current ShellObject does not have a thumbnail. Try using ShellThumbnailFormatOptions.Default to get the icon for this item.",
                                                    Marshal.GetExceptionForHR((int) hr));
            }
            throw (uint) hr == 0x80040154
                      ? new NotSupportedException("The current ShellObject does not have a valid thumbnail handler or there was a problem in extracting the thumbnail for this specific shell object.",
                                                  Marshal.GetExceptionForHR((int) hr))
                      : Marshal.GetExceptionForHR((int) hr);
        }

        private Bitmap GetBitmap(Size size)
        {
            var hBitmap = GetHBitmap(size);

            // return a System.Drawing.Bitmap from the hBitmap
            var returnValue = Image.FromHbitmap(hBitmap);

            // delete HBitmap to avoid memory leaks
            ShellNativeMethods.DeleteObject(hBitmap);

            return returnValue;
        }

        private BitmapSource GetBitmapSource(Size size)
        {
            var hBitmap = GetHBitmap(size);

            // return a System.Media.Imaging.BitmapSource
            // Use interop to create a BitmapSource from hBitmap.
            var returnValue = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // delete HBitmap to avoid memory leaks
            ShellNativeMethods.DeleteObject(hBitmap);

            return returnValue;
        }

        #endregion
    }
}