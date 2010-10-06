// ***********************************************************************
// Assembly         : WPFLocalizeExtension
// Author           : Bernhard Millauer
// Created          : 09-19-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Bernhard Millauer. All rights reserved.
// ***********************************************************************
namespace WPFLocalizeExtension.Extensions
{
    using System;
    using System.Globalization;
    using System.Windows.Markup;

    using WPFLocalizeExtension.BaseExtensions;
    using WPFLocalizeExtension.Engine;

    /// <summary>
    /// <c>BaseLocalizeExtension</c> for double values
    /// </summary>
    [MarkupExtensionReturnType(typeof(double))]
    public class LocDoubleExtension : BaseLocalizeExtension<double>
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LocDoubleExtension" /> class.
        /// </summary>
        public LocDoubleExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocDoubleExtension"/> class.
        /// </summary>
        /// <param name="key">
        /// The resource identifier.
        /// </param>
        public LocDoubleExtension(string key)
            : base(key)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Provides the Value for the first Binding as double
        /// </summary>
        /// <param name="serviceProvider">
        /// The <see cref="System.Windows.Markup.IProvideValueTarget"/> provided from the <see cref="MarkupExtension"/>
        /// </param>
        /// <returns>
        /// The founded item from the .resx directory or <see langword="null"/> if not founded
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// thrown if <paramref name="serviceProvider"/> is not type of <see cref="System.Windows.Markup.IProvideValueTarget"/>
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// thrown if the founded object is not type of double
        /// </exception>
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
                string.Format(CultureInfo.CurrentCulture, "ResourceKey '{0}' returns '{1}' which is not type of double", this.Key, obj.GetType().FullName));
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method is used to modify the passed object into the target format
        /// </summary>
        /// <param name="input">
        /// The object that will be modified
        /// </param>
        /// <returns>
        /// Returns the modified object
        /// </returns>
        protected override object FormatOutput(object input)
        {
            if (Localize.Instance.GetIsInDesignMode() && this.DesignValue != null)
            {
                try
                {
                    return double.Parse((string)this.DesignValue, new CultureInfo("en-US"));
                }
                catch
                {
                    return null;
                }
            }

            return double.Parse((string)input, new CultureInfo("en-US"));

            ////System.Reflection.MethodInfo method = typeof(System.ComponentModel.DoubleConverter).GetMethod("FromString", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            ////object result = method.Invoke(null, new object[] { source, new System.Globalization.CultureInfo("en-US") });

            ////return (double)result;
        }

        /// <summary>
        /// see <c>BaseLocalizeExtension</c>
        /// </summary>
        protected override void HandleNewValue()
        {
            var obj = Localize.Instance.GetLocalizedObject<object>(this.Assembly, this.Dict, this.Key, this.GetForcedCultureOrDefault());
            this.SetNewValue(this.FormatOutput(obj));
        }

        #endregion
    }
}