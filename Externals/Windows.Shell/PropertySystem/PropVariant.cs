//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Internal
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Shell.PropertySystem;

    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    /// <summary>
    /// Represents the OLE struct PROPVARIANT.
    ///   This class is intended for internal use only.
    /// </summary>
    /// <remarks>
    /// Must call Clear when finished to avoid memory leaks. If you get the value of
    ///   a VT_UNKNOWN prop, an implicit AddRef is called, thus your reference will
    ///   be active even after the PropVariant struct is cleared.
    ///   Correct usage:
    /// 
    ///   PropVariant propVar;
    ///   GetProp(out propVar);
    ///   try
    ///   {
    ///   object value = propVar.Value;
    ///   }
    ///   finally { propVar.Clear(); }
    ///   
    ///   Originally sourced from http://blogs.msdn.com/adamroot/pages/interop-with-propvariants-in-net.aspx
    ///   and modified to support additional types inculding vectors and ability to set values
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct PropVariant
    {
        #region Fields

        // The layout of these elements needs to be maintained.
        // NOTE: We could use LayoutKind.Explicit, but we want
        // to maintain that the IntPtr may be 8 bytes on
        // 64-bit architectures, so we'll let the CLR keep
        // us aligned.

        // This is actually a VarEnum value, but the VarEnum type
        // requires 4 bytes instead of the expected 2.
        /// <summary>
        /// </summary>
        private ushort valueType;

        // Reserved Fields
        /// <summary>
        /// </summary>
        private ushort wReserved1;

        /// <summary>
        /// </summary>
        private ushort wReserved2;

        /// <summary>
        /// </summary>
        private ushort wReserved3;

        // In order to allow x64 compat, we need to allow for
        // expansion of the IntPtr. However, the BLOB struct
        // uses a 4-byte int, followed by an IntPtr, so
        // although the valueData field catches most pointer values,
        // we need an additional 4-bytes to get the BLOB
        // pointer. The valueDataExt field provides this, as well as
        // the last 4-bytes of an 8-byte value on 32-bit
        // architectures.
        /// <summary>
        /// </summary>
        private IntPtr valueData;

        /// <summary>
        /// </summary>
        private int valueDataExt;

        #endregion // struct fields

        #region public Methods

        /// <summary>
        /// Creates a PropVariant from an object
        /// </summary>
        /// <param name="value">
        /// The object containing the data.
        /// </param>
        /// <returns>
        /// An initialized PropVariant
        /// </returns>
        public static PropVariant FromObject(object value)
        {
            var propVar = new PropVariant();

            if (value == null)
            {
                propVar.Clear();
                return propVar;
            }

            if (value.GetType() == typeof(string))
            {
                // Strings require special consideration, because they cannot be empty as well
                if (String.IsNullOrEmpty(value as string) || String.IsNullOrEmpty((value as string).Trim()))
                {
                    throw new ArgumentException("String argument cannot be null or empty.");
                }

                propVar.SetString(value as string);
            }
            else if (value.GetType() == typeof(bool?))
            {
                propVar.SetBool((value as bool?).Value);
            }
            else if (value.GetType() == typeof(bool))
            {
                propVar.SetBool((bool)value);
            }
            else if (value.GetType() == typeof(byte?))
            {
                propVar.SetByte((value as byte?).Value);
            }
            else if (value.GetType() == typeof(byte))
            {
                propVar.SetByte((byte)value);
            }
            else if (value.GetType() == typeof(sbyte?))
            {
                propVar.SetSByte((value as sbyte?).Value);
            }
            else if (value.GetType() == typeof(sbyte))
            {
                propVar.SetSByte((sbyte)value);
            }
            else if (value.GetType() == typeof(short?))
            {
                propVar.SetShort((value as short?).Value);
            }
            else if (value.GetType() == typeof(short))
            {
                propVar.SetShort((short)value);
            }
            else if (value.GetType() == typeof(ushort?))
            {
                propVar.SetUShort((value as ushort?).Value);
            }
            else if (value.GetType() == typeof(ushort))
            {
                propVar.SetUShort((ushort)value);
            }
            else if (value.GetType() == typeof(int?))
            {
                propVar.SetInt((value as int?).Value);
            }
            else if (value.GetType() == typeof(int))
            {
                propVar.SetInt((int)value);
            }
            else if (value.GetType() == typeof(uint?))
            {
                propVar.SetUInt((value as uint?).Value);
            }
            else if (value.GetType() == typeof(uint))
            {
                propVar.SetUInt((uint)value);
            }
            else if (value.GetType() == typeof(long?))
            {
                propVar.SetLong((value as long?).Value);
            }
            else if (value.GetType() == typeof(long))
            {
                propVar.SetLong((long)value);
            }
            else if (value.GetType() == typeof(ulong?))
            {
                propVar.SetULong((value as ulong?).Value);
            }
            else if (value.GetType() == typeof(ulong))
            {
                propVar.SetULong((ulong)value);
            }
            else if (value.GetType() == typeof(double?))
            {
                propVar.SetDouble((value as double?).Value);
            }
            else if (value.GetType() == typeof(double))
            {
                propVar.SetDouble((double)value);
            }
            else if (value.GetType() == typeof(float?))
            {
                propVar.SetDouble((value as float?).Value);
            }
            else if (value.GetType() == typeof(float))
            {
                propVar.SetDouble((float)value);
            }
            else if (value.GetType() == typeof(DateTime?))
            {
                propVar.SetDateTime((value as DateTime?).Value);
            }
            else if (value.GetType() == typeof(DateTime))
            {
                propVar.SetDateTime((DateTime)value);
            }
            else if (value.GetType() == typeof(string[]))
            {
                propVar.SetStringVector(value as string[]);
            }
            else if (value.GetType() == typeof(short[]))
            {
                propVar.SetShortVector(value as short[]);
            }
            else if (value.GetType() == typeof(ushort[]))
            {
                propVar.SetUShortVector(value as ushort[]);
            }
            else if (value.GetType() == typeof(int[]))
            {
                propVar.SetIntVector(value as int[]);
            }
            else if (value.GetType() == typeof(uint[]))
            {
                propVar.SetUIntVector(value as uint[]);
            }
            else if (value.GetType() == typeof(long[]))
            {
                propVar.SetLongVector(value as long[]);
            }
            else if (value.GetType() == typeof(ulong[]))
            {
                propVar.SetULongVector(value as ulong[]);
            }
            else if (value.GetType() == typeof(DateTime[]))
            {
                propVar.SetDateTimeVector(value as DateTime[]);
            }
            else if (value.GetType() == typeof(bool[]))
            {
                propVar.SetBoolVector(value as bool[]);
            }
            else
            {
                throw new ArgumentException("This Value type is not supported.");
            }

            return propVar;
        }

        /// <summary>
        /// Called to clear the PropVariant's referenced and local memory.
        /// </summary>
        /// <remarks>
        /// You must call Clear to avoid memory leaks.
        /// </remarks>
        public void Clear()
        {
            // Can't pass "this" by ref, so make a copy to call PropVariantClear with
            var var = this;
            PropVariantNativeMethods.PropVariantClear(ref var);

            // Since we couldn't pass "this" by ref, we need to clear the member fields manually
            // NOTE: PropVariantClear already freed heap data for us, so we are just setting
            // our references to null.
            this.valueType = (ushort)VarEnum.VT_EMPTY;
            this.wReserved1 = this.wReserved2 = this.wReserved3 = 0;
            this.valueData = IntPtr.Zero;
            this.valueDataExt = 0;
        }

        /// <summary>
        /// Set a string value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetString(string value)
        {
            this.valueType = (ushort)VarEnum.VT_LPWSTR;
            this.valueData = Marshal.StringToCoTaskMemUni(value);
        }

        /// <summary>
        /// Set a string vector
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetStringVector(string[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromStringVector(value, (uint)value.Length, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set a bool vector
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetBoolVector(bool[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromBooleanVector(value, (uint)value.Length, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set a short vector
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetShortVector(short[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromInt16Vector(value, (uint)value.Length, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set a short vector
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetUShortVector(ushort[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromUInt16Vector(value, (uint)value.Length, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set an int vector
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetIntVector(int[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromInt32Vector(value, (uint)value.Length, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set an uint vector
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetUIntVector(uint[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromUInt32Vector(value, (uint)value.Length, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set a long vector
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetLongVector(long[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromInt64Vector(value, (uint)value.Length, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set a ulong vector
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetULongVector(ulong[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromUInt64Vector(value, (uint)value.Length, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set a double vector
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetDoubleVector(double[] value)
        {
            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromDoubleVector(value, (uint)value.Length, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set a DateTime vector
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetDateTimeVector(DateTime[] value)
        {
            var fileTimeArr = new FILETIME[value.Length];

            for (var i = 0; i < value.Length; i++)
            {
                fileTimeArr[i] = DateTimeTotFileTime(value[i]);
            }

            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromFileTimeVector(fileTimeArr, (uint)fileTimeArr.Length, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set an IUnknown value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetIUnknown(object value)
        {
            this.valueType = (ushort)VarEnum.VT_UNKNOWN;
            this.valueData = Marshal.GetIUnknownForObject(value);
        }

        /// <summary>
        /// Set a bool value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetBool(bool value)
        {
            this.valueType = (ushort)VarEnum.VT_BOOL;
            this.valueData = value ? (IntPtr)(-1) : (IntPtr)0;
        }

        /// <summary>
        /// Set a DateTime value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetDateTime(DateTime value)
        {
            this.valueType = (ushort)VarEnum.VT_FILETIME;

            PropVariant propVar;
            var ft = new global::System.Runtime.InteropServices.FILETIME();
            PropVariantNativeMethods.InitPropVariantFromFileTime(ref ft, out propVar);
            this.CopyData(propVar);
        }

        /// <summary>
        /// Set a safe array value
        /// </summary>
        /// <param name="array">
        /// The new value to set.
        /// </param>
        public void SetSafeArray(Array array)
        {
            const ushort vtUnknown = 13;
            var psa = PropVariantNativeMethods.SafeArrayCreateVector(vtUnknown, 0, (uint)array.Length);

            var pvData = PropVariantNativeMethods.SafeArrayAccessData(psa);
            try
            {
                // to remember to release lock on data
                for (var i = 0; i < array.Length; ++i)
                {
                    var obj = array.GetValue(i);
                    var punk = (obj != null) ? Marshal.GetIUnknownForObject(obj) : IntPtr.Zero;
                    Marshal.WriteIntPtr(pvData, i * IntPtr.Size, punk);
                }
            }
            finally
            {
                PropVariantNativeMethods.SafeArrayUnaccessData(psa);
            }

            this.valueType = (ushort)VarEnum.VT_ARRAY | (ushort)VarEnum.VT_UNKNOWN;
            this.valueData = psa;
        }

        /// <summary>
        /// Set a byte value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetByte(byte value)
        {
            this.valueType = (ushort)VarEnum.VT_UI1;
            this.valueData = (IntPtr)value;
        }

        /// <summary>
        /// Set a sbyte value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetSByte(sbyte value)
        {
            this.valueType = (ushort)VarEnum.VT_I1;
            this.valueData = (IntPtr)value;
        }

        /// <summary>
        /// Set a short value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetShort(short value)
        {
            this.valueType = (ushort)VarEnum.VT_I2;
            this.valueData = (IntPtr)value;
        }

        /// <summary>
        /// Set an unsigned short value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetUShort(ushort value)
        {
            this.valueType = (ushort)VarEnum.VT_UI2;
            this.valueData = (IntPtr)value;
        }

        /// <summary>
        /// Set an int value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetInt(int value)
        {
            this.valueType = (ushort)VarEnum.VT_I4;
            this.valueData = (IntPtr)value;
        }

        /// <summary>
        /// Set an unsigned int value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetUInt(uint value)
        {
            this.valueType = (ushort)VarEnum.VT_UI4;
            this.valueData = (IntPtr)(int)value;
        }

        /// <summary>
        /// Set a decimal  value
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetDecimal(decimal value)
        {
            var bits = Decimal.GetBits(value);
            this.valueData = (IntPtr)bits[0];
            this.valueDataExt = bits[1];
            this.wReserved3 = (ushort)(bits[2] >> 16);
            this.wReserved2 = (ushort)(bits[2] & 0x0000FFFF);
            this.wReserved1 = (ushort)(bits[3] >> 16);
            this.valueType = (ushort)VarEnum.VT_DECIMAL;
        }

        /// <summary>
        /// Set a long
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetLong(long value)
        {
            var valueArr = new[] { value };

            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromInt64Vector(valueArr, 1, out propVar);

            this.CreatePropVariantFromVectorElement(propVar);
        }

        /// <summary>
        /// Set a ulong
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetULong(ulong value)
        {
            PropVariant propVar;
            var valueArr = new[] { value };
            PropVariantNativeMethods.InitPropVariantFromUInt64Vector(valueArr, 1, out propVar);

            this.CreatePropVariantFromVectorElement(propVar);
        }

        /// <summary>
        /// Set a double
        /// </summary>
        /// <param name="value">
        /// The new value to set.
        /// </param>
        public void SetDouble(double value)
        {
            var valueArr = new[] { value };

            PropVariant propVar;
            PropVariantNativeMethods.InitPropVariantFromDoubleVector(valueArr, 1, out propVar);

            this.CreatePropVariantFromVectorElement(propVar);
        }

        /// <summary>
        /// Sets the value type to empty
        /// </summary>
        public void SetEmptyValue()
        {
            this.valueType = (ushort)VarEnum.VT_EMPTY;
        }

        /// <summary>
        ///   Checks if this has an empty or null value
        /// </summary>
        /// <returns />
        public bool IsNullOrEmpty
        {
            get
            {
                return this.valueType == (ushort)VarEnum.VT_EMPTY || this.valueType == (ushort)VarEnum.VT_NULL;
            }
        }

        #endregion

        #region public Properties

        /// <summary>
        ///   Gets or sets the variant type.
        /// </summary>
        public VarEnum VarType
        {
            get
            {
                return (VarEnum)this.valueType;
            }

            set
            {
                this.valueType = (ushort)value;
            }
        }

        /// <summary>
        ///   Gets the variant value.
        /// </summary>
        public object Value
        {
            get
            {
                switch ((VarEnum)this.valueType)
                {
                    case VarEnum.VT_I1:
                        return this.CVal;
                    case VarEnum.VT_UI1:
                        return this.BVal;
                    case VarEnum.VT_I2:
                        return this.IVal;
                    case VarEnum.VT_UI2:
                        return this.UIVal;
                    case VarEnum.VT_I4:
                    case VarEnum.VT_INT:
                        return this.LVal;
                    case VarEnum.VT_UI4:
                    case VarEnum.VT_UINT:
                        return this.ULVal;
                    case VarEnum.VT_I8:
                        return this.HVal;
                    case VarEnum.VT_UI8:
                        return this.UHVal;
                    case VarEnum.VT_R4:
                        return this.FltVal;
                    case VarEnum.VT_R8:
                        return this.DblVal;
                    case VarEnum.VT_BOOL:
                        return this.BoolVal;
                    case VarEnum.VT_ERROR:
                        return this.Scode;
                    case VarEnum.VT_CY:
                        return this.CYVal;
                    case VarEnum.VT_DATE:
                        return this.Date;
                    case VarEnum.VT_FILETIME:
                        return DateTime.FromFileTime(this.HVal);
                    case VarEnum.VT_BSTR:
                        return Marshal.PtrToStringBSTR(this.valueData);
                    case VarEnum.VT_BLOB:
                        return this.GetBlobData();
                    case VarEnum.VT_LPSTR:
                        return Marshal.PtrToStringAnsi(this.valueData);
                    case VarEnum.VT_LPWSTR:
                        return Marshal.PtrToStringUni(this.valueData);
                    case VarEnum.VT_UNKNOWN:
                        return Marshal.GetObjectForIUnknown(this.valueData);
                    case VarEnum.VT_DISPATCH:
                        return Marshal.GetObjectForIUnknown(this.valueData);
                    case VarEnum.VT_DECIMAL:
                        return this.DecVal;
                    case VarEnum.VT_ARRAY | VarEnum.VT_UNKNOWN:
                        return CrackSingleDimSafeArray(this.valueData);
                    case VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR:
                        return this.GetStringVector();
                    case VarEnum.VT_VECTOR | VarEnum.VT_I2:
                        return this.GetVector<short>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_UI2:
                        return this.GetVector<ushort>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_I4:
                        return this.GetVector<int>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_UI4:
                        return this.GetVector<uint>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_I8:
                        return this.GetVector<long>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_UI8:
                        return this.GetVector<ulong>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_R8:
                        return this.GetVector<double>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_BOOL:
                        return this.GetVector<bool>();
                    case VarEnum.VT_VECTOR | VarEnum.VT_FILETIME:
                        return this.GetVector<DateTime>();
                    default:

                        // if the value cannot be marshaled
                        return null;
                }
            }
        }

        #endregion

        #region PropVariant Simple Data types

        /// <summary>
        /// </summary>
        private sbyte CVal
        {
            // CHAR cVal;
            get
            {
                return (sbyte)this.GetDataBytes()[0];
            }
        }

        /// <summary>
        /// </summary>
        private byte BVal
        {
            // UCHAR bVal;
            get
            {
                return this.GetDataBytes()[0];
            }
        }

        /// <summary>
        /// </summary>
        private short IVal
        {
            // SHORT iVal;
            get
            {
                return BitConverter.ToInt16(this.GetDataBytes(), 0);
            }
        }

        /// <summary>
        /// </summary>
        private ushort UIVal
        {
            // USHORT uiVal;
            get
            {
                return BitConverter.ToUInt16(this.GetDataBytes(), 0);
            }
        }

        /// <summary>
        /// </summary>
        private int LVal
        {
            // LONG lVal;
            get
            {
                return BitConverter.ToInt32(this.GetDataBytes(), 0);
            }
        }

        /// <summary>
        /// </summary>
        private uint ULVal
        {
            // ULONG ulVal;
            get
            {
                return BitConverter.ToUInt32(this.GetDataBytes(), 0);
            }
        }

        /// <summary>
        /// </summary>
        private long HVal
        {
            // LARGE_INTEGER hVal;
            get
            {
                return BitConverter.ToInt64(this.GetDataBytes(), 0);
            }
        }

        /// <summary>
        /// </summary>
        private ulong UHVal
        {
            // ULARGE_INTEGER uhVal;
            get
            {
                return BitConverter.ToUInt64(this.GetDataBytes(), 0);
            }
        }

        /// <summary>
        /// </summary>
        private float FltVal
        {
            // FLOAT fltVal;
            get
            {
                return BitConverter.ToSingle(this.GetDataBytes(), 0);
            }
        }

        /// <summary>
        /// </summary>
        private double DblVal
        {
            // DOUBLE dblVal;
            get
            {
                return BitConverter.ToDouble(this.GetDataBytes(), 0);
            }
        }

        /// <summary>
        /// </summary>
        private bool BoolVal
        {
            // VARIANT_BOOL boolVal;
            get
            {
                return this.IVal == 0 ? false : true;
            }
        }

        /// <summary>
        /// </summary>
        private int Scode
        {
            // SCODE scode;
            get
            {
                return this.LVal;
            }
        }

        /// <summary>
        /// </summary>
        private decimal CYVal
        {
            // CY cyVal;
            get
            {
                return decimal.FromOACurrency(this.HVal);
            }
        }

        /// <summary>
        /// </summary>
        private DateTime Date
        {
            // DATE date;
            get
            {
                return DateTime.FromOADate(this.DblVal);
            }
        }

        /// <summary>
        /// </summary>
        private decimal DecVal
        {
            // Decimal value
            get
            {
                var bits = new int[4];
                bits[0] = (int)this.valueData;
                bits[1] = this.valueDataExt;
                bits[2] = (this.wReserved3 << 16) | this.wReserved2;
                bits[3] = this.wReserved1 << 16;
                return new decimal(bits);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        private void CopyData(PropVariant propVar)
        {
            this.valueType = propVar.valueType;
            this.valueData = propVar.valueData;
            this.valueDataExt = propVar.valueDataExt;
        }

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        private void CreatePropVariantFromVectorElement(PropVariant propVar)
        {
            // Copy the first vector element to a new PropVariant
            this.CopyData(propVar);
            PropVariantNativeMethods.InitPropVariantFromPropVariantVectorElem(ref this, 0, out propVar);

            // Overwrite the existing data
            this.CopyData(propVar);
        }

        /// <summary>
        /// </summary>
        /// <param name="val">
        /// </param>
        /// <returns>
        /// </returns>
        private static long FileTimeToDateTime(ref FILETIME val)
        {
            return (((long)val.dwHighDateTime) << 32) + val.dwLowDateTime;
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// </returns>
        private static FILETIME DateTimeTotFileTime(DateTime value)
        {
            var hFT = value.ToFileTime();
            var ft = new FILETIME { dwLowDateTime = (int)(hFT & 0xFFFFFFFF), dwHighDateTime = (int)(hFT >> 32) };
            return ft;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        private object GetBlobData()
        {
            var blobData = new byte[this.LVal];
            IntPtr pBlobData;
            switch (IntPtr.Size)
            {
                case 4:
                    pBlobData = new IntPtr(this.valueDataExt);
                    break;
                case 8:
                    pBlobData =
                        new IntPtr(BitConverter.ToInt32(this.GetDataBytes(), sizeof(int)) + (Int64)(BitConverter.ToInt32(this.GetDataBytes(), 2 * sizeof(int)) << 32));
                    break;
                default:
                    throw new NotSupportedException();
            }

            Marshal.Copy(pBlobData, blobData, 0, this.LVal);

            return blobData;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        private Array GetVector<T>() where T : struct
        {
            var count = PropVariantNativeMethods.PropVariantGetElementCount(ref this);
            if (count <= 0)
            {
                return null;
            }

            Array arr = new T[count];

            for (uint i = 0; i < count; i++)
            {
                if (typeof(T) == typeof(Int16))
                {
                    short val;
                    PropVariantNativeMethods.PropVariantGetInt16Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(UInt16))
                {
                    ushort val;
                    PropVariantNativeMethods.PropVariantGetUInt16Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(Int32))
                {
                    int val;
                    PropVariantNativeMethods.PropVariantGetInt32Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(UInt32))
                {
                    uint val;
                    PropVariantNativeMethods.PropVariantGetUInt32Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(Int64))
                {
                    long val;
                    PropVariantNativeMethods.PropVariantGetInt64Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(UInt64))
                {
                    ulong val;
                    PropVariantNativeMethods.PropVariantGetUInt64Elem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    FILETIME val;
                    PropVariantNativeMethods.PropVariantGetFileTimeElem(ref this, i, out val);

                    var fileTime = FileTimeToDateTime(ref val);

                    arr.SetValue(DateTime.FromFileTime(fileTime), i);
                }
                else if (typeof(T) == typeof(Boolean))
                {
                    bool val;
                    PropVariantNativeMethods.PropVariantGetBooleanElem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(Double))
                {
                    double val;
                    PropVariantNativeMethods.PropVariantGetDoubleElem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
                else if (typeof(T) == typeof(String))
                {
                    string val;
                    PropVariantNativeMethods.PropVariantGetStringElem(ref this, i, out val);
                    arr.SetValue(val, i);
                }
            }

            return arr;
        }

        // A string requires a special case because it's not a struct or value type
        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private string[] GetStringVector()
        {
            var count = PropVariantNativeMethods.PropVariantGetElementCount(ref this);
            if (count <= 0)
            {
                return null;
            }

            var strArr = new string[count];
            for (uint i = 0; i < count; i++)
            {
                PropVariantNativeMethods.PropVariantGetStringElem(ref this, i, out strArr[i]);
            }

            return strArr;
        }

        /// <summary>
        /// Gets a byte array containing the data bits of the struct.
        /// </summary>
        /// <returns>
        /// A byte array that is the combined size of the data bits.
        /// </returns>
        private byte[] GetDataBytes()
        {
            var ret = new byte[IntPtr.Size + sizeof(int)];
            switch (IntPtr.Size)
            {
                case 4:
                    BitConverter.GetBytes(this.valueData.ToInt32()).CopyTo(ret, 0);
                    break;
                case 8:
                    BitConverter.GetBytes(this.valueData.ToInt64()).CopyTo(ret, 0);
                    break;
            }

            BitConverter.GetBytes(this.valueDataExt).CopyTo(ret, IntPtr.Size);
            return ret;
        }

        /// <summary>
        /// </summary>
        /// <param name="psa">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        private static Array CrackSingleDimSafeArray(IntPtr psa)
        {
            var cDims = PropVariantNativeMethods.SafeArrayGetDim(psa);
            if (cDims != 1)
            {
                throw new ArgumentException("Multi-dimensional SafeArrays not supported.");
            }

            var lBound = PropVariantNativeMethods.SafeArrayGetLBound(psa, 1U);
            var uBound = PropVariantNativeMethods.SafeArrayGetUBound(psa, 1U);

            var n = uBound - lBound + 1; // uBound is inclusive

            var array = new object[n];
            for (var i = lBound; i <= uBound; ++i)
            {
                array[i] = PropVariantNativeMethods.SafeArrayGetElement(psa, ref i);
            }

            return array;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator ==(PropVariant x, PropVariant y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator !=(PropVariant x, PropVariant y)
        {
            throw new NotImplementedException();
        }
    }
}