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
    /// <c>BaseLocalizeExtension</c> for string objects.
    /// </summary>
    [MarkupExtensionReturnType(typeof(string))]
    public class LocTextExtension : BaseLocalizeExtension<string>
    {
        #region Constants and Fields

        /// <summary>
        ///   Holds the local format segment array
        /// </summary>
        private string[] formatSegments;

        /// <summary>
        ///   Holds the local prefix value
        /// </summary>
        private string prefix;

        /// <summary>
        ///   Holds the local suffix value
        /// </summary>
        private string suffix;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LocTextExtension" /> class.
        /// </summary>
        public LocTextExtension()
        {
            this.InitializeLocText();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocTextExtension"/> class.
        /// </summary>
        /// <param name="key">
        /// The resource identifier.
        /// </param>
        public LocTextExtension(string key)
            : base(key)
        {
            this.InitializeLocText();
        }

        #endregion

        #region Enums

        /// <summary>
        /// This enumeration is used to determine the type 
        ///   of the return value of <see cref="GetAppendText"/>
        /// </summary>
        protected enum TextAppendType
        {
            /// <summary>
            ///   The return value is used as prefix
            /// </summary>
            Prefix, 

            /// <summary>
            ///   The return value is used as suffix
            /// </summary>
            Suffix
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the format segment 1.
        ///   This will be used to replace format place holders from the localized text.
        ///   <see cref = "LocTextLowerExtension" /> and <see cref = "LocTextUpperExtension" /> will format this segment.
        /// </summary>
        /// <value>The format segment 1.</value>
        public string FormatSegment1
        {
            get
            {
                return this.formatSegments[0];
            }

            set
            {
                this.formatSegments[0] = value;
                this.HandleNewValue();
            }
        }

        /// <summary>
        ///   Gets or sets the format segment 2.
        ///   This will be used to replace format place holders from the localized text.
        ///   <see cref = "LocTextUpperExtension" /> and <see cref = "LocTextLowerExtension" /> will format this segment.
        /// </summary>
        /// <value>The format segment 2.</value>
        public string FormatSegment2
        {
            get
            {
                return this.formatSegments[1];
            }

            set
            {
                this.formatSegments[1] = value;
                this.HandleNewValue();
            }
        }

        /// <summary>
        ///   Gets or sets the format segment 3.
        ///   This will be used to replace format place holders from the localized text.
        ///   <see cref = "LocTextUpperExtension" /> and <see cref = "LocTextLowerExtension" /> will format this segment.
        /// </summary>
        /// <value>The format segment 3.</value>
        public string FormatSegment3
        {
            get
            {
                return this.formatSegments[2];
            }

            set
            {
                this.formatSegments[2] = value;
                this.HandleNewValue();
            }
        }

        /// <summary>
        ///   Gets or sets the format segment 4.
        ///   This will be used to replace format place holders from the localized text.
        ///   <see cref = "LocTextUpperExtension" /> and <see cref = "LocTextLowerExtension" /> will format this segment.
        /// </summary>
        /// <value>The format segment 4.</value>
        public string FormatSegment4
        {
            get
            {
                return this.formatSegments[3];
            }

            set
            {
                this.formatSegments[3] = value;
                this.HandleNewValue();
            }
        }

        /// <summary>
        ///   Gets or sets the format segment 5.
        ///   This will be used to replace format place holders from the localized text.
        ///   <see cref = "LocTextUpperExtension" /> and <see cref = "LocTextLowerExtension" /> will format this segment.
        /// </summary>
        /// <value>The format segment 5.</value>
        public string FormatSegment5
        {
            get
            {
                return this.formatSegments[4];
            }

            set
            {
                this.formatSegments[4] = value;
                this.HandleNewValue();
            }
        }

        /// <summary>
        ///   Gets or sets a prefix for the localized text
        /// </summary>
        public string Prefix
        {
            get
            {
                return this.prefix;
            }

            set
            {
                this.prefix = value;

                // reset the value of the target property
                this.HandleNewValue();
            }
        }

        /// <summary>
        ///   Gets or sets a suffix for the localized text
        /// </summary>
        public string Suffix
        {
            get
            {
                return this.suffix;
            }

            set
            {
                this.suffix = value;

                // reset the value of the target property
                this.HandleNewValue();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Provides the Value for the first Binding as <see cref="System.String"/>
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
        /// thrown if the founded object is not type of <see cref="System.String"/>
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
                string.Format(CultureInfo.CurrentCulture, "ResourceKey '{0}' returns '{1}' which is not type of System.String", this.Key, obj.GetType().FullName));
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method returns the finished formatted text
        /// </summary>
        /// <param name="input">
        /// If the passed string not <see langword="null"/>, it will be used, otherwise a fresh localized text will be loaded.
        /// </param>
        /// <returns>
        /// Returns the finished formatted text in format [PREFIX]LocalizedText[SUFFIX]
        /// </returns>
        protected override object FormatOutput(object input)
        {
            if (Localize.Instance.GetIsInDesignMode() && this.DesignValue != null)
            {
                input = this.DesignValue;
            }
            else
            {
                // load a fresh localized text, if the passed string is null
                input = input ?? Localize.Instance.GetLocalizedObject<object>(this.Assembly, this.Dict, this.Key, this.GetForcedCultureOrDefault());
            }

            // get the main text as string xor string.empty
            var textMain = input as string ?? string.Empty;

            try
            {
                // add some format segments, in case that the main text contains format place holders like {0}
                textMain = string.Format(
                    Localize.Instance.SpecificCulture, 
                    textMain, 
                    this.formatSegments[0] ?? string.Empty, 
                    this.formatSegments[1] ?? string.Empty, 
                    this.formatSegments[2] ?? string.Empty, 
                    this.formatSegments[3] ?? string.Empty, 
                    this.formatSegments[4] ?? string.Empty);
            }
            catch (FormatException)
            {
                // if a format exception was thrown, change the text to an error string
                textMain = "TextFormatError: Max 5 Format PlaceHolders! {0} to {4}";
            }

            // get the prefix
            var textPrefix = this.GetAppendText(TextAppendType.Prefix);

            // get the suffix
            var textSuffix = this.GetAppendText(TextAppendType.Suffix);

            // format the text with prefix and suffix to [PREFIX]LocalizedText[SUFFIX]
            input = this.FormatText(textPrefix + textMain + textSuffix);

            // return the finished formatted text
            return input;
        }

        /// <summary>
        /// This method formats the localized text.
        ///   If the passed target text is <see langword="null"/>, string.empty will be returned.
        /// </summary>
        /// <param name="target">
        /// The text to format.
        /// </param>
        /// <returns>
        /// Returns the formated text or string.empty, if the target text was <see langword="null"/>.
        /// </returns>
        protected virtual string FormatText(string target)
        {
            return target ?? string.Empty;
        }

        /// <summary>
        /// see <c>BaseLocalizeExtension</c>
        /// </summary>
        protected override void HandleNewValue()
        {
            this.SetNewValue(this.FormatOutput(null));
        }

        /// <summary>
        /// Returns the prefix or suffix text, depending on the supplied <see cref="TextAppendType"/>.
        ///   If the prefix or suffix is <see langword="null"/>, it will be returned a string.empty.
        /// </summary>
        /// <param name="at">
        /// The <see cref="TextAppendType"/> defines the format of the return value
        /// </param>
        /// <returns>
        /// Returns the formated prefix or suffix
        /// </returns>
        private string GetAppendText(TextAppendType at)
        {
            // define a return value
            var retVal = string.Empty;

            // check if it should be a prefix, the format will be [PREFIX],
            // or check if it should be a suffix, the format will be [SUFFIX]
            if (at == TextAppendType.Prefix && !string.IsNullOrEmpty(this.prefix))
            {
                retVal = this.prefix;
            }
            else if (at == TextAppendType.Suffix && !string.IsNullOrEmpty(this.suffix))
            {
                retVal = this.suffix;
            }

            // return the formated prefix or suffix
            return retVal;
        }

        /// <summary>
        /// Initializes the <see cref="LocTextExtension"/> extension.
        /// </summary>
        private void InitializeLocText()
        {
            this.formatSegments = new string[5];
            this.formatSegments.Initialize();

            // removed this call, because of the fact, 
            // if the LocTextExtension is defined with "LocTextExtension Key=abc" and not with "LocTextExtension abc".
            // the value will be set at call ProvideValue, AFTER the Key Property is set.

            ////SetNewValue(FormatOutput(null));
        }

        #endregion
    }
}