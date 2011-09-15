// ***********************************************************************
// <copyright file="LocTextLowerExtension.cs" project="WPFLocalizeExtension" assembly="WPFLocalizeExtension" solution="SevenUpdate" company="Bernhard Millauer">
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

    /// <summary><c>BaseLocalizeExtension</c> for string objects.This strings will be converted to lower case.</summary>
    [MarkupExtensionReturnType(typeof(string))]
    public class LocTextLowerExtension : LocTextExtension
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="LocTextLowerExtension" /> class.</summary>
        public LocTextLowerExtension()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LocTextLowerExtension" /> class.</summary>
        /// <param name="key">The resource identifier.</param>
        public LocTextLowerExtension(string key)
            : base(key)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>Provides the Value for the first Binding as <c>System.String</c>.</summary>
        /// <param name="serviceProvider">The <c>System.Windows.Markup.IProvideValueTarget</c> provided from the <c>MarkupExtension</c>.</param>
        /// <returns>The found item from the .resx directory or <c>null</c> if not found.</returns>
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
                // don't call GetLocalizedText at this point, otherwise you will get prefix and suffix twice appended
                return obj;
            }

            throw new NotSupportedException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "ResourceKey '{0}' returns '{1}' which is not type of System.String",
                    this.Key,
                    obj.GetType().FullName));
        }

        #endregion

        #region Methods

        /// <summary>
        ///   This method formats the localized text.If the passed target text is <c>null</c>, string.empty will be
        ///   returned.
        /// </summary>
        /// <param name="target">The text to format.</param>
        /// <returns>Returns the formated text or string.empty, if the target text was <c>null</c>.</returns>
        protected override string FormatText(string target)
        {
            return target == null ? string.Empty : target.ToLower(this.Culture);
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
