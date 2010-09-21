#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using WPFLocalizeExtension.BaseExtensions;
using WPFLocalizeExtension.Engine;

#endregion

namespace WPFLocalizeExtension.Extensions
{
    /// <summary>
    ///   <c>BaseLocalizeExtension</c> for image objects
    /// </summary>
    [MarkupExtensionReturnType(typeof (BitmapSource))]
    public class LocImageExtension : BaseLocalizeExtension<BitmapSource>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "LocImageExtension" /> class.
        /// </summary>
        public LocImageExtension()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LocImageExtension" /> class.
        /// </summary>
        /// <param name = "key">The resource identifier.</param>
        public LocImageExtension(string key) : base(key)
        {
        }

        /// <summary>
        ///   Provides the Value for the first Binding as <see cref = "System.Windows.Media.Imaging.BitmapSource" />
        /// </summary>
        /// <param name = "serviceProvider">
        ///   The <see cref = "System.Windows.Markup.IProvideValueTarget" /> provided from the <see cref = "MarkupExtension" />
        /// </param>
        /// <returns>The founded item from the .resx directory or null if not founded</returns>
        /// <exception cref = "System.InvalidOperationException">
        ///   thrown if <paramref name = "serviceProvider" /> is not type of <see cref = "System.Windows.Markup.IProvideValueTarget" />
        /// </exception>
        /// <exception cref = "System.NotSupportedException">
        ///   thrown if the founded object is not type of <see cref = "System.Drawing.Bitmap" />
        /// </exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var obj = base.ProvideValue(serviceProvider);

            if (obj == null)
                return null;

            if (IsTypeOf(obj.GetType(), typeof (BaseLocalizeExtension<>)))
                return obj;

            if (obj.GetType().Equals(typeof (Bitmap)))
                return FormatOutput(obj);

            throw new NotSupportedException(string.Format("ResourceKey '{0}' returns '{1}' which is not type of System.Drawing.Bitmap", Key, obj.GetType().FullName));
        }

        /// <summary>
        ///   see <c>BaseLocalizeExtension</c>
        /// </summary>
        protected override void HandleNewValue()
        {
            var obj = LocalizeDictionary.Instance.GetLocalizedObject<object>(Assembly, Dict, Key, GetForcedCultureOrDefault());
            SetNewValue(FormatOutput(obj));
        }

        /// <summary>
        ///   Creates a <see cref = "System.Windows.Media.Imaging.BitmapSource" /> from a <see cref = "System.Drawing.Bitmap" />.
        ///   This extension does NOT support a DesignValue.
        /// </summary>
        /// <param name = "input">The <see cref = "System.Drawing.Bitmap" /> to convert</param>
        /// <returns>The converted <see cref = "System.Windows.Media.Imaging.BitmapSource" /></returns>
        protected override object FormatOutput(object input)
        {
            // allocate the memory for the bitmap
            var bmpPt = ((Bitmap) input).GetHbitmap();

            // create the bitmapSource
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bmpPt, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // freeze the bitmap to avoid hooking events to the bitmap
            bitmapSource.Freeze();

            // free memory
            DeleteObject(bmpPt);

            // return bitmapSource
            return bitmapSource;
        }

        /// <summary>
        ///   Frees memory of a pointer.
        /// </summary>
        /// <param name = "o">Object to remove from memory.</param>
        /// <returns>0 if the removing was success, otherwise another number.</returns>
        [DllImport("gdi32.dll")]
        private static extern int DeleteObject(IntPtr o);
    }
}