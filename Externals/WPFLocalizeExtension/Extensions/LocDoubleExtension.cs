// <copyright file="LocDoubleExtension.cs" project="WPFLocalizeExtension">Bernhard Millauer</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace WPFLocalizeExtension.Extensions
{
    using System;
    using System.Globalization;
    using System.Windows.Markup;

    using WPFLocalizeExtension.Engine;

    /// <summary><c>BaseLocalizeExtension</c> for double values.</summary>
    [MarkupExtensionReturnType(typeof(double))]
    public class LocDoubleExtension : BaseLocalizeExtension<double>
    {
        /// <summary>Initializes a new instance of the <see cref="LocDoubleExtension" /> class.</summary>
        public LocDoubleExtension()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LocDoubleExtension" /> class.</summary>
        /// <param name="key">The resource identifier.</param>
        public LocDoubleExtension(string key) : base(key)
        {
        }

        /// <summary>Provides the Value for the first Binding as double.</summary>
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
                    "ResourceKey '{0}' returns '{1}' which is not type of double", 
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
                return double.Parse((string)this.DesignValue, new CultureInfo("en-US"));
            }

            return double.Parse((string)input, new CultureInfo("en-US"));
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
    }
}