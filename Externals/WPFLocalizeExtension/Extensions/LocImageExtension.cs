// ***********************************************************************
// <copyright file="LocImageExtension.cs" project="WPFLocalizeExtension" assembly="WPFLocalizeExtension" solution="SevenUpdate" company="Bernhard Millauer">
//     Copyright (c) Bernhard Millauer. All rights reserved.
// </copyright>
// <author username="SeriousM">Bernhard Millauer</author>
// <license href="http://wpflocalizeextension.codeplex.com/license">Microsoft Public License</license>
// ***********************************************************************

namespace WPFLocalizeExtension.Extensions
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;

    using WPFLocalizeExtension.Engine;

    /// <summary><c>BaseLocalizeExtension</c> for image objects.</summary>
    [MarkupExtensionReturnType(typeof(BitmapSource))]
    public class LocImageExtension : BaseLocalizeExtension<BitmapSource>
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="LocImageExtension" /> class.</summary>
        public LocImageExtension()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LocImageExtension" /> class.</summary>
        /// <param name="key">The resource identifier.</param>
        public LocImageExtension(string key) : base(key)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Provides the Value for the first Binding as <c>System.Windows.Media.Imaging.BitmapSource</c>.</summary>
        /// <param name="serviceProvider">The <c>System.Windows.Markup.IProvideValueTarget</c> provided from the <c>MarkupExtension</c>.</param>
        /// <returns>The found item from the .resx directory or <c>null</c> if not found.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            object obj = base.ProvideValue(serviceProvider);

            if (obj == null)
            {
                return null;
            }

            if (this.IsTypeOf(obj.GetType(), typeof(BaseLocalizeExtension<>)))
            {
                return obj;
            }

            if (obj.GetType().Equals(typeof(Bitmap)))
            {
                return this.FormatOutput(obj);
            }

            throw new NotSupportedException(
                string.Format(
                    CultureInfo.CurrentCulture, 
                    "ResourceKey '{0}' returns '{1}' which is not type of System.Drawing.Bitmap", 
                    this.Key, 
                    obj.GetType().FullName));
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Creates a <c>System.Windows.Media.Imaging.BitmapSource</c> from a <see cref="System.Drawing.Bitmap"
        ///   />.This extension does NOT support a DesignValue.
        /// </summary>
        /// <param name="input">The <c>System.Drawing.Bitmap</c> to convert.</param>
        /// <returns>The converted <c>System.Windows.Media.Imaging.BitmapSource</c>.</returns>
        protected override object FormatOutput(object input)
        {
            // allocate the memory for the bitmap
            IntPtr bmpPt = ((Bitmap)input).GetHbitmap();

            // create the bitmapSource
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bmpPt, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // freeze the bitmap to avoid hooking events to the bitmap
            bitmapSource.Freeze();

            // free memory
            NativeMethods.DeleteObject(bmpPt);

            // return bitmapSource
            return bitmapSource;
        }

        /// <summary>
        ///   This method gets the new value for the target property and call <see cref =
        ///   "BaseLocalizeExtension{TValue}.SetNewValue" />.
        /// </summary>
        protected override void HandleNewValue()
        {
            var obj = Localize.Instance.GetLocalizedObject<object>(
                this.Assembly, this.Dictionary, this.Key, this.Culture);
            this.SetNewValue(this.FormatOutput(obj));
        }

        #endregion
    }
}