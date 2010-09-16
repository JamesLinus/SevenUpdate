#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using Microsoft.Windows.Internal;

#endregion

namespace Microsoft.Windows.Shell.PropertySystem
{
    /// <summary>
    ///   Defines the enumeration values for a property type.
    /// </summary>
    public class ShellPropertyEnumType
    {
        #region Private Properties

        private string displayText;
        private PropEnumType? enumType;
        private object enumerationValue;
        private object minValue, setValue;

        private IPropertyEnumType NativePropertyEnumType { set; get; }

        #endregion

        #region Internal Constructor

        internal ShellPropertyEnumType(IPropertyEnumType nativePropertyEnumType)
        {
            NativePropertyEnumType = nativePropertyEnumType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets display text from an enumeration information structure.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (displayText == null)
                    NativePropertyEnumType.GetDisplayText(out displayText);
                return displayText;
            }
        }

        /// <summary>
        ///   Gets an enumeration type from an enumeration information structure.
        /// </summary>
        public PropEnumType EnumType
        {
            get
            {
                if (!enumType.HasValue)
                {
                    PropEnumType tempEnumType;
                    NativePropertyEnumType.GetEnumType(out tempEnumType);
                    enumType = tempEnumType;
                }
                return enumType.Value;
            }
        }

        /// <summary>
        ///   Gets a minimum value from an enumeration information structure.
        /// </summary>
        public object RangeMinValue
        {
            get
            {
                if (minValue == null)
                {
                    PropVariant propVar;
                    NativePropertyEnumType.GetRangeMinValue(out propVar);
                    minValue = propVar.Value;
                }
                return minValue;
            }
        }

        /// <summary>
        ///   Gets a set value from an enumeration information structure.
        /// </summary>
        public object RangeSetValue
        {
            get
            {
                if (setValue == null)
                {
                    PropVariant propVar;
                    NativePropertyEnumType.GetRangeSetValue(out propVar);
                    setValue = propVar.Value;
                }
                return setValue;
            }
        }

        /// <summary>
        ///   Gets a value from an enumeration information structure.
        /// </summary>
        public object RangeValue
        {
            get
            {
                if (enumerationValue == null)
                {
                    PropVariant propVar;
                    NativePropertyEnumType.GetValue(out propVar);
                    enumerationValue = propVar.Value;
                }
                return enumerationValue;
            }
        }

        #endregion
    }
}