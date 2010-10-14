// ***********************************************************************
// <copyright file="LocBrushExtension.cs"
//            project="WPFLocalizeExtension"
//            assembly="WPFLocalizeExtension"
//            solution="SevenUpdate"
//            company="Bernhard Millauer">
//     Copyright (c) Bernhard Millauer. All rights reserved.
// </copyright>
// <author username="SeriousM">Bernhard Millauer</author>
// <license href="http://wpflocalizeextension.codeplex.com/license">Microsoft Public License</license>
// ***********************************************************************
namespace WPFLocalizeExtension.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Markup;
    using System.Windows.Media;

    using WPFLocalizeExtension.Engine;

    /// <summary><c>BaseLocalizeExtension</c> for brush objects as string (uses <see cref="TypeConverter"/>)</summary>
    [MarkupExtensionReturnType(typeof(Brush))]
    public class LocBrushExtension : BaseLocalizeExtension<Brush>
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "LocBrushExtension" /> class.</summary>
        public LocBrushExtension()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LocBrushExtension"/> class.</summary>
        /// <param name="key">The resource identifier.</param>
        public LocBrushExtension(string key) : base(key)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>Provides the Value for the first Binding as <see cref="System.Windows.Media.Brush"/></summary>
        /// <param name="serviceProvider">The <see cref="System.Windows.Markup.IProvideValueTarget"/> provided from the <see cref="MarkupExtension"/></param>
        /// <returns>The founded item from the .resx directory or <see langword="null"/> if not founded</returns>
        /// <exception cref="System.InvalidOperationException">thrown if <paramref name="serviceProvider"/> is not type of <see cref="System.Windows.Markup.IProvideValueTarget"/></exception>
        /// <exception cref="System.NotSupportedException">thrown if the founded object is not type of <see cref="System.String"/></exception>
        /// <exception cref="System.NotSupportedException">The founded resource-string cannot be converted into the appropriate object.</exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var obj = base.ProvideValue(serviceProvider);

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

            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "ResourceKey '{0}' returns '{1}' which is not type of System.Drawing.Bitmap", this.Key, obj.GetType().FullName));
        }

        #endregion

        #region Methods

        /// <summary>This method is used to modify the passed object into the target format</summary>
        /// <param name="input">The object that will be modified</param>
        /// <returns>Returns the modified object</returns>
        protected override object FormatOutput(object input)
        {
            if (Localize.Instance.IsInDesignMode && this.DesignValue != null)
            {
                try
                {
                    return new BrushConverter().ConvertFromString((string)this.DesignValue);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return new BrushConverter().ConvertFromString((string)input);
        }

        /// <summary>see <c>BaseLocalizeExtension</c></summary>
        protected override void HandleNewValue()
        {
            var obj = Localize.Instance.GetLocalizedObject<object>(this.Assembly, this.Dictionary, this.Key, this.GetForcedCultureOrDefault());
            this.SetNewValue(new BrushConverter().ConvertFromString((string)obj));
        }

        #endregion
    }
}