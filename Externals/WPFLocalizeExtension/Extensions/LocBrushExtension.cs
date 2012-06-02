// <copyright file="LocBrushExtension.cs" project="WPFLocalizeExtension">Bernhard Millauer</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace WPFLocalizeExtension.Extensions
{
    using System;
    using System.Globalization;
    using System.Windows.Markup;
    using System.Windows.Media;

    using WPFLocalizeExtension.Engine;

    /// <summary><c>BaseLocalizeExtension</c> for brush objects as string (uses <c>TypeConverter</c>).</summary>
    [MarkupExtensionReturnType(typeof(Brush))]
    public class LocBrushExtension : BaseLocalizeExtension<Brush>
    {
        /// <summary>Initializes a new instance of the <see cref="LocBrushExtension" /> class.</summary>
        public LocBrushExtension()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LocBrushExtension" /> class.</summary>
        /// <param name="key">The resource identifier.</param>
        public LocBrushExtension(string key) : base(key)
        {
        }

        /// <summary>Provides the Value for the first Binding as <c>System.Windows.Media.Brush</c>.</summary>
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

            if (obj.GetType().Equals(typeof(string)))
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

        /// <summary>This method is used to modify the passed object into the target format.</summary>
        /// <param name="input">The object that will be modified.</param>
        /// <returns>Returns the modified object.</returns>
        protected override object FormatOutput(object input)
        {
            if (Localize.Instance.IsInDesignMode && this.DesignValue != null)
            {
                return new BrushConverter().ConvertFromString((string)this.DesignValue);
            }

            return new BrushConverter().ConvertFromString((string)input);
        }

        /// <summary>
        ///   This method gets the new value for the target property and call <see cref =
        ///   "BaseLocalizeExtension{TValue}.SetNewValue" />.
        /// </summary>
        protected override void HandleNewValue()
        {
            var obj = Localize.Instance.GetLocalizedObject<object>(
                this.Assembly, this.Dictionary, this.Key, this.Culture);
            this.SetNewValue(new BrushConverter().ConvertFromString((string)obj));
        }
    }
}