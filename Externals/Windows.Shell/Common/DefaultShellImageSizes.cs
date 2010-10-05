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
    using System.Windows;

    /// <summary>
    /// Defines the read-only properties for default shell icon sizes.
    /// </summary>
    public static class DefaultIconSize
    {
        #region Constants and Fields

        /// <summary>
        ///   The extra-large size property for a 256x256 pixel Shell Icon.
        /// </summary>
        public static Size ExtraLarge = new Size(256, 256);

        /// <summary>
        ///   The large size property for a 48x48 pixel Shell Icon.
        /// </summary>
        public static Size Large = new Size(48, 48);

        /// <summary>
        ///   The maximum size for a Shell Icon, 256x256 pixels.
        /// </summary>
        public static Size Maximum = new Size(256, 256);

        /// <summary>
        ///   The medium size property for a 32x32 pixel Shell Icon.
        /// </summary>
        public static Size Medium = new Size(32, 32);

        /// <summary>
        ///   The small size property for a 16x16 pixel Shell Icon.
        /// </summary>
        public static Size Small = new Size(16, 16);

        #endregion
    }

    /// <summary>
    /// Defines the read-only properties for default shell thumbnail sizes.
    /// </summary>
    public static class DefaultThumbnailSize
    {
        #region Constants and Fields

        /// <summary>
        ///   Gets the extra-large size property for a 1024x1024 pixel Shell Thumbnail.
        /// </summary>
        public static Size ExtraLarge = new Size(1024, 1024);

        /// <summary>
        ///   Gets the large size property for a 256x256 pixel Shell Thumbnail.
        /// </summary>
        public static Size Large = new Size(256, 256);

        /// <summary>
        ///   Maximum size for the Shell Thumbnail, 1024x1024 pixels.
        /// </summary>
        public static Size Maximum = new Size(1024, 1024);

        /// <summary>
        ///   Gets the medium size property for a 96x96 pixel Shell Thumbnail.
        /// </summary>
        public static Size Medium = new Size(96, 96);

        /// <summary>
        ///   Gets the small size property for a 32x32 pixel Shell Thumbnail.
        /// </summary>
        public static Size Small = new Size(32, 32);

        #endregion
    }
}