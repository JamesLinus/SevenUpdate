//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Shell.PropertySystem
{
    using Microsoft.Windows.Internal;

    /// <summary>
    /// Defines the enumeration values for a property type.
    /// </summary>
    public class ShellPropertyEnumType
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private string displayText;

        /// <summary>
        /// </summary>
        private PropEnumType? enumType;

        /// <summary>
        /// </summary>
        private object enumerationValue;

        /// <summary>
        /// </summary>
        private object minValue;

        /// <summary>
        /// </summary>
        private object setValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="nativePropertyEnumType">
        /// </param>
        internal ShellPropertyEnumType(IPropertyEnumType nativePropertyEnumType)
        {
            this.NativePropertyEnumType = nativePropertyEnumType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets display text from an enumeration information structure.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (this.displayText == null)
                {
                    this.NativePropertyEnumType.GetDisplayText(out this.displayText);
                }

                return this.displayText;
            }
        }

        /// <summary>
        ///   Gets an enumeration type from an enumeration information structure.
        /// </summary>
        public PropEnumType EnumType
        {
            get
            {
                if (!this.enumType.HasValue)
                {
                    PropEnumType tempEnumType;
                    this.NativePropertyEnumType.GetEnumType(out tempEnumType);
                    this.enumType = tempEnumType;
                }

                return this.enumType.Value;
            }
        }

        /// <summary>
        ///   Gets a minimum value from an enumeration information structure.
        /// </summary>
        public object RangeMinValue
        {
            get
            {
                if (this.minValue == null)
                {
                    PropVariant propVar;
                    this.NativePropertyEnumType.GetRangeMinValue(out propVar);
                    this.minValue = propVar.Value;
                }

                return this.minValue;
            }
        }

        /// <summary>
        ///   Gets a set value from an enumeration information structure.
        /// </summary>
        public object RangeSetValue
        {
            get
            {
                if (this.setValue == null)
                {
                    PropVariant propVar;
                    this.NativePropertyEnumType.GetRangeSetValue(out propVar);
                    this.setValue = propVar.Value;
                }

                return this.setValue;
            }
        }

        /// <summary>
        ///   Gets a value from an enumeration information structure.
        /// </summary>
        public object RangeValue
        {
            get
            {
                if (this.enumerationValue == null)
                {
                    PropVariant propVar;
                    this.NativePropertyEnumType.GetValue(out propVar);
                    this.enumerationValue = propVar.Value;
                }

                return this.enumerationValue;
            }
        }

        /// <summary>
        /// </summary>
        private IPropertyEnumType NativePropertyEnumType { get; set; }

        #endregion
    }
}