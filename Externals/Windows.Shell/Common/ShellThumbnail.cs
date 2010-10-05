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
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    using Microsoft.Windows.Internal;

    using Size = System.Windows.Size;

    /// <summary>
    /// Represents a thumbnail or an icon for a ShellObject.
    /// </summary>
    public class ShellThumbnail
    {
        #region Constants and Fields

        /// <summary>
        ///   Native shellItem
        /// </summary>
        private readonly IShellItem shellItemNative;

        /// <summary>
        ///   Internal member to keep track of the current size
        /// </summary>
        private Size currentSize = new Size(256, 256);

        /// <summary>
        /// </summary>
        private ShellThumbnailFormatOption formatOption = ShellThumbnailFormatOption.Default;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Internal constructor that takes in a parent ShellObject.
        /// </summary>
        /// <param name="shellObject">
        /// </param>
        internal ShellThumbnail(ShellObject shellObject)
        {
            if (shellObject == null || shellObject.NativeShellItem == null)
            {
                throw new ArgumentNullException("shellObject");
            }

            this.shellItemNative = shellObject.NativeShellItem;
        }

        #endregion

        #region Properties

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

        /// <summary>
        ///   Gets the thumbnail or icon image in <see cref = "System.Drawing.Bitmap" /> format.
        ///   Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public Bitmap Bitmap
        {
            get
            {
                return this.GetBitmap(this.CurrentSize);
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon image in <see cref = "System.Windows.Media.Imaging.BitmapSource" /> format. 
        ///   Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public BitmapSource BitmapSource
        {
            get
            {
                return this.GetBitmapSource(this.CurrentSize);
            }
        }

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
            get
            {
                return this.currentSize;
            }

            set
            {
                // Check for 0; negative number check not required as System.Windows.Size only allows positive numbers.
                if (value.Height == 0 || value.Width == 0)
                {
                    throw new ArgumentOutOfRangeException("value", "CurrentSize (width or height) cannot be 0");
                }

                if (this.FormatOption == ShellThumbnailFormatOption.IconOnly)
                {
                    if (value.Height > DefaultIconSize.Maximum.Height || value.Width > DefaultIconSize.Maximum.Width)
                    {
                        throw new ArgumentOutOfRangeException(
                            "value", string.Format(CultureInfo.CurrentCulture, "CurrentSize (width or height) cannot be greater than the maximum size"));
                    }
                }
                else
                {
                    // Covers the default mode (Thumbnail Or Icon) as well as ThumbnailOnly
                    if (value.Height > DefaultThumbnailSize.Maximum.Height || value.Width > DefaultThumbnailSize.Maximum.Width)
                    {
                        throw new ArgumentOutOfRangeException(
                            "value", string.Format(CultureInfo.CurrentCulture, "CurrentSize (width or height) cannot be greater than the maximum size"));
                    }
                }

                this.currentSize = value;
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in extra large size and <see cref = "System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap ExtraLargeBitmap
        {
            get
            {
                return this.FormatOption == ShellThumbnailFormatOption.IconOnly
                           ? this.GetBitmap(DefaultIconSize.ExtraLarge)
                           : this.GetBitmap(DefaultThumbnailSize.ExtraLarge);
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in Extra Large size and <see cref = "System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource ExtraLargeBitmapSource
        {
            get
            {
                return this.FormatOption == ShellThumbnailFormatOption.IconOnly
                           ? this.GetBitmapSource(DefaultIconSize.ExtraLarge)
                           : this.GetBitmapSource(DefaultThumbnailSize.ExtraLarge);
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in Extra Large size and <see cref = "System.Drawing.Icon" /> format.
        /// </summary>
        public Icon ExtraLargeIcon
        {
            get
            {
                return Icon.FromHandle(this.ExtraLargeBitmap.GetHicon());
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines if the current format option is thumbnail or icon, thumbnail only, or icon only.
        ///   The default is thumbnail or icon.
        /// </summary>
        public ShellThumbnailFormatOption FormatOption
        {
            get
            {
                return this.formatOption;
            }

            set
            {
                this.formatOption = value;

                // Do a similar check as we did in CurrentSize property setter,
                // If our mode is IconOnly, then our max is defined by DefaultIconSize.Maximum. We should make sure 
                // our CurrentSize is within this max range
                if (this.FormatOption != ShellThumbnailFormatOption.IconOnly)
                {
                    return;
                }

                if (this.CurrentSize.Height > DefaultIconSize.Maximum.Height || this.CurrentSize.Width > DefaultIconSize.Maximum.Width)
                {
                    this.CurrentSize = DefaultIconSize.Maximum;
                }
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon image in <see cref = "System.Drawing.Icon" /> format. 
        ///   Null is returned if the ShellObject does not have a thumbnail or icon image.
        /// </summary>
        public Icon Icon
        {
            get
            {
                return Icon.FromHandle(this.Bitmap.GetHicon());
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in large size and <see cref = "System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap LargeBitmap
        {
            get
            {
                return this.FormatOption == ShellThumbnailFormatOption.IconOnly ? this.GetBitmap(DefaultIconSize.Large) : this.GetBitmap(DefaultThumbnailSize.Large);
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in large size and <see cref = "System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource LargeBitmapSource
        {
            get
            {
                return this.FormatOption == ShellThumbnailFormatOption.IconOnly
                           ? this.GetBitmapSource(DefaultIconSize.Large)
                           : this.GetBitmapSource(DefaultThumbnailSize.Large);
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in Large size and <see cref = "System.Drawing.Icon" /> format.
        /// </summary>
        public Icon LargeIcon
        {
            get
            {
                return Icon.FromHandle(this.LargeBitmap.GetHicon());
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in Medium size and <see cref = "System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap MediumBitmap
        {
            get
            {
                return this.FormatOption == ShellThumbnailFormatOption.IconOnly ? this.GetBitmap(DefaultIconSize.Medium) : this.GetBitmap(DefaultThumbnailSize.Medium);
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in medium size and <see cref = "System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource MediumBitmapSource
        {
            get
            {
                return this.FormatOption == ShellThumbnailFormatOption.IconOnly
                           ? this.GetBitmapSource(DefaultIconSize.Medium)
                           : this.GetBitmapSource(DefaultThumbnailSize.Medium);
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in Medium size and <see cref = "System.Drawing.Icon" /> format.
        /// </summary>
        public Icon MediumIcon
        {
            get
            {
                return Icon.FromHandle(this.MediumBitmap.GetHicon());
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines if the current retrieval option is cache or extract, cache only, or from memory only.
        ///   The default is cache or extract.
        /// </summary>
        public ShellThumbnailRetrievalOption RetrievalOption { get; set; }

        /// <summary>
        ///   Gets the thumbnail or icon in small size and <see cref = "System.Drawing.Bitmap" /> format.
        /// </summary>
        public Bitmap SmallBitmap
        {
            get
            {
                return this.FormatOption == ShellThumbnailFormatOption.IconOnly ? this.GetBitmap(DefaultIconSize.Small) : this.GetBitmap(DefaultThumbnailSize.Small);
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in small size and <see cref = "System.Windows.Media.Imaging.BitmapSource" /> format.
        /// </summary>
        public BitmapSource SmallBitmapSource
        {
            get
            {
                return this.FormatOption == ShellThumbnailFormatOption.IconOnly
                           ? this.GetBitmapSource(DefaultIconSize.Small)
                           : this.GetBitmapSource(DefaultThumbnailSize.Small);
            }
        }

        /// <summary>
        ///   Gets the thumbnail or icon in small size and <see cref = "System.Drawing.Icon" /> format.
        /// </summary>
        public Icon SmallIcon
        {
            get
            {
                return Icon.FromHandle(this.SmallBitmap.GetHicon());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private ShellNativeMethods.SIIGBFs CalculateFlags()
        {
            ShellNativeMethods.SIIGBFs flags = 0x0000;

            if (this.AllowBiggerSize)
            {
                flags |= ShellNativeMethods.SIIGBFs.SiigbfBiggersizeok;
            }

            switch (this.RetrievalOption)
            {
                case ShellThumbnailRetrievalOption.CacheOnly:
                    flags |= ShellNativeMethods.SIIGBFs.SiigbfIncacheonly;
                    break;
                case ShellThumbnailRetrievalOption.MemoryOnly:
                    flags |= ShellNativeMethods.SIIGBFs.SiigbfMemoryonly;
                    break;
            }

            switch (this.FormatOption)
            {
                case ShellThumbnailFormatOption.IconOnly:
                    flags |= ShellNativeMethods.SIIGBFs.SiigbfIcononly;
                    break;
                case ShellThumbnailFormatOption.ThumbnailOnly:
                    flags |= ShellNativeMethods.SIIGBFs.SiigbfThumbnailonly;
                    break;
            }

            return flags;
        }

        /// <summary>
        /// </summary>
        /// <param name="size">
        /// </param>
        /// <returns>
        /// </returns>
        private Bitmap GetBitmap(Size size)
        {
            var hBitmap = this.GetHBitmap(size);

            // return a System.Drawing.Bitmap from the hBitmap
            var returnValue = Image.FromHbitmap(hBitmap);

            // delete HBitmap to avoid memory leaks
            ShellNativeMethods.DeleteObject(hBitmap);

            return returnValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="size">
        /// </param>
        /// <returns>
        /// </returns>
        private BitmapSource GetBitmapSource(Size size)
        {
            var hBitmap = this.GetHBitmap(size);

            // return a System.Media.Imaging.BitmapSource
            // Use interop to create a BitmapSource from hBitmap.
            var returnValue = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // delete HBitmap to avoid memory leaks
            ShellNativeMethods.DeleteObject(hBitmap);

            return returnValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="size">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="Exception">
        /// </exception>
        private IntPtr GetHBitmap(Size size)
        {
            IntPtr hbitmap;

            // Create a size structure to pass to the native method
            var nativeSIZE = new CoreNativeMethods.SIZE { CX = Convert.ToInt32(size.Width), CY = Convert.ToInt32(size.Height) };

            // Use IShellItemImageFactory to get an icon
            // Options passed in: Resize to fit
            var hr = ((IShellItemImageFactory)this.shellItemNative).GetImage(nativeSIZE, this.CalculateFlags(), out hbitmap);

            if (hr == HRESULT.S_OK)
            {
                return hbitmap;
            }

            if ((uint)hr == 0x8004B200 && this.FormatOption == ShellThumbnailFormatOption.ThumbnailOnly)
            {
                // Thumbnail was requested, but this ShellItem doesn't have a thumbnail.
                throw new InvalidOperationException(
                    "The current ShellObject does not have a thumbnail. Try using ShellThumbnailFormatOptions.Default to get the icon for this item.", 
                    Marshal.GetExceptionForHR((int)hr));
            }

            throw (uint)hr == 0x80040154
                      ? new NotSupportedException(
                            "The current ShellObject does not have a valid thumbnail handler or there was a problem in extracting the thumbnail for this specific shell object.", 
                            Marshal.GetExceptionForHR((int)hr))
                      : Marshal.GetExceptionForHR((int)hr);
        }

        #endregion
    }
}