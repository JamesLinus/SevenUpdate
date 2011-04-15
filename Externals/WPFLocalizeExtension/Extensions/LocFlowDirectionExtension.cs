// ***********************************************************************
// <copyright file="LocFlowDirectionExtension.cs"
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
    using System.Windows;
    using System.Windows.Markup;

    using WPFLocalizeExtension.Engine;

    /// <summary><c>BaseLocalizeExtension</c> for <see cref="FlowDirection" /> values</summary>
    [MarkupExtensionReturnType(typeof(FlowDirection))]
    public class LocFlowDirectionExtension : BaseLocalizeExtension<FlowDirection>
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="LocFlowDirectionExtension" /> class.</summary>
        public LocFlowDirectionExtension()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LocFlowDirectionExtension" /> class.</summary>
        /// <param name="key">The resource identifier.</param>
        public LocFlowDirectionExtension(string key)
            : base(key)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>Provides the Value for the first Binding as <see cref="LocFlowDirectionExtension" /></summary>
        /// <param name="serviceProvider">The <see cref="System.Windows.Markup.IProvideValueTarget" /> provided from the <see cref="MarkupExtension" /></param>
        /// <returns>The found item from the .resx directory or LeftToRight if not found</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var obj = base.ProvideValue(serviceProvider) ?? "LeftToRight";

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
                    CultureInfo.CurrentCulture, "ResourceKey '{0}' returns '{1}' which is not type of FlowDirection", this.Key, obj.GetType().FullName));
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
                    return Enum.Parse(typeof(FlowDirection), (string)this.DesignValue, true);
                }
                catch (Exception)
                {
                    throw;
                    return null;
                }
            }

            return Enum.Parse(typeof(FlowDirection), (string)input, true);
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