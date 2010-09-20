#region

using System;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;
using WPFLocalizeExtension.BaseExtensions;
using WPFLocalizeExtension.Engine;

#endregion

namespace WPFLocalizeExtension.Extensions
{
    /// <summary>
    ///   <c>BaseLocalizeExtension</c> for brush objects as string (uses <see cref = "TypeConverter" />)
    /// </summary>
    [MarkupExtensionReturnType(typeof (Brush))]
    public class LocBrushExtension : BaseLocalizeExtension<Brush>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "LocBrushExtension" /> class.
        /// </summary>
        public LocBrushExtension()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LocBrushExtension" /> class.
        /// </summary>
        /// <param name = "key">The resource identifier.</param>
        public LocBrushExtension(string key) : base(key)
        {
        }

        /// <summary>
        ///   Provides the Value for the first Binding as <see cref = "System.Windows.Media.Brush" />
        /// </summary>
        /// <param name = "serviceProvider">
        ///   The <see cref = "System.Windows.Markup.IProvideValueTarget" /> provided from the <see cref = "MarkupExtension" />
        /// </param>
        /// <returns>The founded item from the .resx directory or null if not founded</returns>
        /// <exception cref = "System.InvalidOperationException">
        ///   thrown if <paramref name = "serviceProvider" /> is not type of <see cref = "System.Windows.Markup.IProvideValueTarget" />
        /// </exception>
        /// <exception cref = "System.NotSupportedException">
        ///   thrown if the founded object is not type of <see cref = "System.String" />
        /// </exception>
        /// <exception cref = "System.NotSupportedException">
        ///   The founded resource-string cannot be converted into the appropriate object.
        /// </exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var obj = base.ProvideValue(serviceProvider);

            if (obj == null)
                return null;

            if (IsTypeOf(obj.GetType(), typeof (BaseLocalizeExtension<>)))
                return obj;

            if (obj.GetType().Equals(typeof (string)))
                return FormatOutput(obj);

            throw new NotSupportedException(string.Format("ResourceKey '{0}' returns '{1}' which is not type of System.Drawing.Bitmap", Key, obj.GetType().FullName));
        }

        /// <summary>
        ///   see <c>BaseLocalizeExtension</c>
        /// </summary>
        protected override void HandleNewValue()
        {
            var obj = LocalizeDictionary.Instance.GetLocalizedObject<object>(Assembly, Dict, Key, GetForcedCultureOrDefault());
            SetNewValue(new BrushConverter().ConvertFromString((string) obj));
        }

        /// <summary>
        ///   This method is used to modify the passed object into the target format
        /// </summary>
        /// <param name = "input">The object that will be modified</param>
        /// <returns>Returns the modified object</returns>
        protected override object FormatOutput(object input)
        {
            if (LocalizeDictionary.Instance.GetIsInDesignMode() && DesignValue != null)
            {
                try
                {
                    return new BrushConverter().ConvertFromString((string) DesignValue);
                }
                catch
                {
                    return null;
                }
            }

            return new BrushConverter().ConvertFromString((string) input);
        }
    }
}