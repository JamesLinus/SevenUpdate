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
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Runtime.InteropServices;
    using global::System.Runtime.InteropServices.ComTypes;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// Defines the shell property description information for a property.
    /// </summary>
    public class ShellPropertyDescription : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private PropertyAggregationType? aggregationTypes;

        /// <summary>
        /// </summary>
        private string canonicalName;

        /// <summary>
        /// </summary>
        private PropertyColumnState? columnState;

        /// <summary>
        /// </summary>
        private PropertyConditionOperation? conditionOperation;

        /// <summary>
        /// </summary>
        private PropertyConditionType? conditionType;

        /// <summary>
        /// </summary>
        private uint? defaultColumWidth;

        /// <summary>
        /// </summary>
        private string displayName;

        /// <summary>
        /// </summary>
        private PropertyDisplayType? displayType;

        /// <summary>
        /// </summary>
        private string editInvitation;

        /// <summary>
        /// </summary>
        private PropertyGroupingRange? groupingRange;

        /// <summary>
        /// </summary>
        private IPropertyDescription nativePropertyDescription;

        /// <summary>
        /// </summary>
        private ReadOnlyCollection<ShellPropertyEnumType> propertyEnumTypes;

        /// <summary>
        /// </summary>
        private PropertyKey propertyKey;

        /// <summary>
        /// </summary>
        private PropertyTypeFlags? propertyTypeFlags;

        /// <summary>
        /// </summary>
        private PropertyViewFlags? propertyViewFlags;

        /// <summary>
        /// </summary>
        private PropertySortDescription? sortDescription;

        /// <summary>
        /// </summary>
        private Type valueType;

        /// <summary>
        /// </summary>
        private VarEnum? varEnumType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        internal ShellPropertyDescription(PropertyKey key)
        {
            this.propertyKey = key;
        }

        /// <summary>
        /// 
        ///   Release the native objects
        /// </summary>
        ~ShellPropertyDescription()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value that describes how the property values are displayed when 
        ///   multiple items are selected in the user interface (UI).
        /// </summary>
        public PropertyAggregationType AggregationTypes
        {
            get
            {
                if (this.NativePropertyDescription != null && this.aggregationTypes == null)
                {
                    PropertyAggregationType tempAggregationTypes;

                    var hr = this.NativePropertyDescription.GetAggregationType(out tempAggregationTypes);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        this.aggregationTypes = tempAggregationTypes;
                    }
                }

                return this.aggregationTypes.HasValue ? this.aggregationTypes.Value : default(PropertyAggregationType);
            }
        }

        /// <summary>
        ///   Gets the case-sensitive name of a property as it is known to the system, 
        ///   regardless of its localized name.
        /// </summary>
        public string CanonicalName
        {
            get
            {
                if (this.canonicalName == null)
                {
                    PropertySystemNativeMethods.PSGetNameFromPropertyKey(ref this.propertyKey, out this.canonicalName);
                }

                return this.canonicalName;
            }
        }

        /// <summary>
        ///   Gets the column state flag, which describes how the property 
        ///   should be treated by interfaces or APIs that use this flag.
        /// </summary>
        public PropertyColumnState ColumnState
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (this.NativePropertyDescription != null && this.columnState == null)
                {
                    PropertyColumnState state;

                    var hr = this.NativePropertyDescription.GetColumnState(out state);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        this.columnState = state;
                    }
                }

                return this.columnState.HasValue ? this.columnState.Value : default(PropertyColumnState);
            }
        }

        /// <summary>
        ///   Gets the default condition operation to use 
        ///   when displaying the property in the query builder user 
        ///   interface (UI). This influences the list of predicate conditions 
        ///   (for example, equals, less than, and contains) that are shown 
        ///   for this property.
        /// </summary>
        /// <remarks>
        ///   For more information, see the <c>conditionType</c> attribute of the 
        ///   <c>typeInfo</c> element in the property's .propdesc file.
        /// </remarks>
        public PropertyConditionOperation ConditionOperation
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (this.NativePropertyDescription != null && this.conditionOperation == null)
                {
                    PropertyConditionType tempConditionType;
                    PropertyConditionOperation tempConditionOperation;

                    var hr = this.NativePropertyDescription.GetConditionType(out tempConditionType, out tempConditionOperation);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        this.conditionOperation = tempConditionOperation;
                        this.conditionType = tempConditionType;
                    }
                }

                return this.conditionOperation.HasValue ? this.conditionOperation.Value : default(PropertyConditionOperation);
            }
        }

        /// <summary>
        ///   Gets the condition type to use when displaying the property in 
        ///   the query builder user interface (UI). This influences the list 
        ///   of predicate conditions (for example, equals, less than, and 
        ///   contains) that are shown for this property.
        /// </summary>
        /// <remarks>
        ///   For more information, see the <c>conditionType</c> attribute 
        ///   of the <c>typeInfo</c> element in the property's .propdesc file.
        /// </remarks>
        public PropertyConditionType ConditionType
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (this.NativePropertyDescription != null && this.conditionType == null)
                {
                    PropertyConditionType tempConditionType;
                    PropertyConditionOperation tempConditionOperation;

                    var hr = this.NativePropertyDescription.GetConditionType(out tempConditionType, out tempConditionOperation);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        this.conditionOperation = tempConditionOperation;
                        this.conditionType = tempConditionType;
                    }
                }

                return this.conditionType.HasValue ? this.conditionType.Value : default(PropertyConditionType);
            }
        }

        /// <summary>
        ///   Gets the default user interface (UI) column width for this property.
        /// </summary>
        public uint DefaultColumWidth
        {
            get
            {
                if (this.NativePropertyDescription != null && !this.defaultColumWidth.HasValue)
                {
                    uint tempDefaultColumWidth;

                    var hr = this.NativePropertyDescription.GetDefaultColumnWidth(out tempDefaultColumWidth);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        this.defaultColumWidth = tempDefaultColumWidth;
                    }
                }

                return this.defaultColumWidth.HasValue ? this.defaultColumWidth.Value : default(uint);
            }
        }

        /// <summary>
        ///   Gets the display name of the property as it is shown in any user interface (UI).
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (this.NativePropertyDescription != null && this.displayName == null)
                {
                    IntPtr dispNameptr;

                    var hr = this.NativePropertyDescription.GetDisplayName(out dispNameptr);

                    if (CoreErrorHelper.Succeeded((int)hr) && dispNameptr != IntPtr.Zero)
                    {
                        this.displayName = Marshal.PtrToStringUni(dispNameptr);

                        // Free the string
                        Marshal.FreeCoTaskMem(dispNameptr);
                    }
                }

                return this.displayName;
            }
        }

        /// <summary>
        ///   Gets the current data type used to display the property.
        /// </summary>
        public PropertyDisplayType DisplayType
        {
            get
            {
                if (this.NativePropertyDescription != null && this.displayType == null)
                {
                    PropertyDisplayType tempDisplayType;

                    var hr = this.NativePropertyDescription.GetDisplayType(out tempDisplayType);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        this.displayType = tempDisplayType;
                    }
                }

                return this.displayType.HasValue ? this.displayType.Value : default(PropertyDisplayType);
            }
        }

        /// <summary>
        ///   Gets the text used in edit controls hosted in various dialog boxes.
        /// </summary>
        public string EditInvitation
        {
            get
            {
                if (this.NativePropertyDescription != null && this.editInvitation == null)
                {
                    // EditInvitation can be empty, so ignore the HR value, but don't throw an exception
                    IntPtr ptr;

                    var hr = this.NativePropertyDescription.GetEditInvitation(out ptr);

                    if (CoreErrorHelper.Succeeded((int)hr) && ptr != IntPtr.Zero)
                    {
                        this.editInvitation = Marshal.PtrToStringUni(ptr);

                        // Free the string
                        Marshal.FreeCoTaskMem(ptr);
                    }
                }

                return this.editInvitation;
            }
        }

        /// <summary>
        ///   Gets the method used when a view is grouped by this property.
        /// </summary>
        /// <remarks>
        ///   The information retrieved by this method comes from 
        ///   the <c>groupingRange</c> attribute of the <c>typeInfo</c> element in the 
        ///   property's .propdesc file.
        /// </remarks>
        public PropertyGroupingRange GroupingRange
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (this.NativePropertyDescription != null && this.groupingRange == null)
                {
                    PropertyGroupingRange tempGroupingRange;

                    var hr = this.NativePropertyDescription.GetGroupingRange(out tempGroupingRange);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        this.groupingRange = tempGroupingRange;
                    }
                }

                return this.groupingRange.HasValue ? this.groupingRange.Value : default(PropertyGroupingRange);
            }
        }

        /// <summary>
        ///   Gets a value that determines if the native property description is present on the system.
        /// </summary>
        public bool HasSystemDescription
        {
            get
            {
                return this.NativePropertyDescription != null;
            }
        }

        /// <summary>
        ///   Gets a list of the possible values for this property.
        /// </summary>
        public ReadOnlyCollection<ShellPropertyEnumType> PropertyEnumTypes
        {
            get
            {
                if (this.NativePropertyDescription != null && this.propertyEnumTypes == null)
                {
                    var propEnumTypeList = new List<ShellPropertyEnumType>();

                    var guid = new Guid(ShellIidGuid.IPropertyEnumTypeList);
                    IPropertyEnumTypeList nativeList;
                    var hr = this.NativePropertyDescription.GetEnumTypeList(ref guid, out nativeList);

                    if (nativeList != null && CoreErrorHelper.Succeeded((int)hr))
                    {
                        uint count;
                        nativeList.GetCount(out count);
                        guid = new Guid(ShellIidGuid.IPropertyEnumType);

                        for (uint i = 0; i < count; i++)
                        {
                            IPropertyEnumType nativeEnumType;
                            nativeList.GetAt(i, ref guid, out nativeEnumType);
                            propEnumTypeList.Add(new ShellPropertyEnumType(nativeEnumType));
                        }
                    }

                    this.propertyEnumTypes = new ReadOnlyCollection<ShellPropertyEnumType>(propEnumTypeList);
                }

                return this.propertyEnumTypes;
            }
        }

        /// <summary>
        ///   Gets the property key identifying the underlying property.
        /// </summary>
        public PropertyKey PropertyKey
        {
            get
            {
                return this.propertyKey;
            }
        }

        /// <summary>
        ///   Gets the current sort description flags for the property, 
        ///   which indicate the particular wordings of sort offerings.
        /// </summary>
        /// <remarks>
        ///   The settings retrieved by this method are set 
        ///   through the <c>sortDescription</c> attribute of the <c>labelInfo</c> 
        ///   element in the property's .propdesc file.
        /// </remarks>
        public PropertySortDescription SortDescription
        {
            get
            {
                // If default/first value, try to get it again, otherwise used the cached one.
                if (this.NativePropertyDescription != null && this.sortDescription == null)
                {
                    PropertySortDescription tempSortDescription;

                    var hr = this.NativePropertyDescription.GetSortDescription(out tempSortDescription);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        this.sortDescription = tempSortDescription;
                    }
                }

                return this.sortDescription.HasValue ? this.sortDescription.Value : default(PropertySortDescription);
            }
        }

        /// <summary>
        ///   Gets a set of flags that describe the uses and capabilities of the property.
        /// </summary>
        public PropertyTypeFlags TypeFlags
        {
            get
            {
                if (this.NativePropertyDescription != null && this.propertyTypeFlags == null)
                {
                    PropertyTypeFlags tempFlags;

                    var hr = this.NativePropertyDescription.GetTypeFlags(PropertyTypeFlags.MaskAll, out tempFlags);

                    this.propertyTypeFlags = CoreErrorHelper.Succeeded((int)hr) ? tempFlags : default(PropertyTypeFlags);
                }

                return this.propertyTypeFlags.HasValue ? this.propertyTypeFlags.Value : default(PropertyTypeFlags);
            }
        }

        /// <summary>
        ///   Gets the .NET system type for a value of this property, or
        ///   null if the value is empty.
        /// </summary>
        public Type ValueType
        {
            get
            {
                return this.valueType ?? (this.valueType = VarEnumToSystemType(this.VarEnumType));
            }
        }

        /// <summary>
        ///   Gets the VarEnum OLE type for this property.
        /// </summary>
        public VarEnum VarEnumType
        {
            get
            {
                if (this.NativePropertyDescription != null && this.varEnumType == null)
                {
                    VarEnum tempType;

                    var hr = this.NativePropertyDescription.GetPropertyType(out tempType);

                    if (CoreErrorHelper.Succeeded((int)hr))
                    {
                        this.varEnumType = tempType;
                    }
                }

                return this.varEnumType.HasValue ? this.varEnumType.Value : default(VarEnum);
            }
        }

        /// <summary>
        ///   Gets the current set of flags governing the property's view.
        /// </summary>
        public PropertyViewFlags ViewFlags
        {
            get
            {
                if (this.NativePropertyDescription != null && this.propertyViewFlags == null)
                {
                    PropertyViewFlags tempFlags;
                    var hr = this.NativePropertyDescription.GetViewFlags(out tempFlags);

                    this.propertyViewFlags = CoreErrorHelper.Succeeded((int)hr) ? tempFlags : default(PropertyViewFlags);
                }

                return this.propertyViewFlags.HasValue ? this.propertyViewFlags.Value : default(PropertyViewFlags);
            }
        }

        /// <summary>
        ///   Get the native property description COM interface
        /// </summary>
        internal IPropertyDescription NativePropertyDescription
        {
            get
            {
                if (this.nativePropertyDescription == null)
                {
                    var guid = new Guid(ShellIidGuid.IPropertyDescription);
                    PropertySystemNativeMethods.PSGetPropertyDescription(ref this.propertyKey, ref guid, out this.nativePropertyDescription);
                }

                return this.nativePropertyDescription;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the localized display string that describes the current sort order.
        /// </summary>
        /// <param name="descending">
        /// Indicates the sort order should 
        ///   reference the string "Z on top"; otherwise, the sort order should reference the string "A on top".
        /// </param>
        /// <returns>
        /// The sort description for this property.
        /// </returns>
        /// <remarks>
        /// The string retrieved by this method is determined by flags set in the 
        ///   <c>sortDescription</c> attribute of the <c>labelInfo</c> element in the property's .propdesc file.
        /// </remarks>
        public string GetSortDescriptionLabel(bool descending)
        {
            var label = String.Empty;

            if (this.NativePropertyDescription != null)
            {
                IntPtr ptr;
                var hr = this.NativePropertyDescription.GetSortDescriptionLabel(descending, out ptr);

                if (CoreErrorHelper.Succeeded((int)hr) && ptr != IntPtr.Zero)
                {
                    label = Marshal.PtrToStringUni(ptr);

                    // Free the string
                    Marshal.FreeCoTaskMem(ptr);
                }
            }

            return label;
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// Release the native objects
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="varEnumType">
        /// </param>
        /// <returns>
        /// </returns>
        internal static Type VarEnumToSystemType(VarEnum varEnumType)
        {
            switch (varEnumType)
            {
                case VarEnum.VT_EMPTY:
                case VarEnum.VT_NULL:

                    return typeof(Object);

                case VarEnum.VT_UI1:

                    return typeof(Byte?);

                case VarEnum.VT_I2:

                    return typeof(Int16?);

                case VarEnum.VT_UI2:

                    return typeof(UInt16?);

                case VarEnum.VT_I4:

                    return typeof(Int32?);

                case VarEnum.VT_UI4:

                    return typeof(UInt32?);

                case VarEnum.VT_I8:

                    return typeof(Int64?);

                case VarEnum.VT_UI8:

                    return typeof(UInt64?);

                case VarEnum.VT_R8:

                    return typeof(Double?);

                case VarEnum.VT_BOOL:

                    return typeof(Boolean?);

                case VarEnum.VT_FILETIME:

                    return typeof(DateTime?);

                case VarEnum.VT_CLSID:

                    return typeof(IntPtr?);

                case VarEnum.VT_CF:

                    return typeof(IntPtr?);

                case VarEnum.VT_BLOB:

                    return typeof(Byte[]);

                case VarEnum.VT_LPWSTR:

                    return typeof(String);

                case VarEnum.VT_UNKNOWN:

                    return typeof(IntPtr?);

                case VarEnum.VT_STREAM:

                    return typeof(IStream);

                case VarEnum.VT_VECTOR | VarEnum.VT_UI1:

                    return typeof(Byte[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_I2:

                    return typeof(Int16[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_UI2:

                    return typeof(UInt16[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_I4:

                    return typeof(Int32[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_UI4:

                    return typeof(UInt32[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_I8:

                    return typeof(Int64[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_UI8:

                    return typeof(UInt64[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_R8:

                    return typeof(Double[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_BOOL:

                    return typeof(Boolean[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_FILETIME:

                    return typeof(DateTime[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_CLSID:

                    return typeof(IntPtr[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_CF:

                    return typeof(IntPtr[]);

                case VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR:

                    return typeof(String[]);

                default:

                    return typeof(Object);
            }
        }

        /// <summary>
        /// Release the native objects
        /// </summary>
        /// <param name="disposing">
        /// Indicates that this is being called from Dispose(), rather than the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.nativePropertyDescription != null)
            {
                Marshal.ReleaseComObject(this.nativePropertyDescription);
                this.nativePropertyDescription = null;
            }

            if (!disposing)
            {
                return;
            }

            // and the managed ones
            this.canonicalName = null;
            this.displayName = null;
            this.editInvitation = null;
            this.defaultColumWidth = null;
            this.valueType = null;
            this.propertyEnumTypes = null;
        }

        #endregion
    }
}