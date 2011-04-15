// ***********************************************************************
// <copyright file="LocDoubleExtension.cs"
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
    using System.Globalization;
    using System.Windows.Markup;

    using WPFLocalizeExtension.Engine;

    /// <summary><c>BaseLocalizeExtension</c> for double values</summary>
    [MarkupExtensionReturnType(typeof(double))]
    public class LocDoubleExtension : BaseLocalizeExtension<double>
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="LocDoubleExtension" /> class.</summary>
        public LocDoubleExtension()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LocDoubleExtension" /> class.</summary>
        /// <param name="key">The resource identifier.</param>
        public LocDoubleExtension(string key)
            : base(key)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>Provides the Value for the first Binding as double</summary>
        /// <param name="serviceProvider">The <see cref="System.Windows.Markup.IProvideValueTarget" /> provided from the <see cref="MarkupExtension" /></param>
        /// <returns>The found item from the .resx directory or <see langword="null" /> if not found</returns>
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

            throw new NotSupportedException(
                string.Format(
                    CultureInfo.CurrentCulture, "ResourceKey '{0}' returns '{1}' which is not type of double", this.Key, obj.GetType().FullName));
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
                    return double.Parse((string)this.DesignValue, new CultureInfo("en-US"));
                }
                catch (Exception)
                {
                    throw;
                    return null;
                }
            }

            return double.Parse((string)input, new CultureInfo("en-US"));
        }

        /// <summary>see <c>BaseLocalizeExtension</c></summary>
        protected override void HandleNewValue()
        {
            var obj = Localize.Instance.GetLocalizedObject<object>(this.Assembly, this.Dictionary, this.Key, this.Culture);
            this.SetNewValue(this.FormatOutput(obj));
        }

        #endregion
    }
}