//Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#endregion

namespace Microsoft.Windows.Shell.PropertySystem
{
    /// <summary>
    ///   Defines a unique key for a Shell Property
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PropertyKey : IEquatable<PropertyKey>
    {
        #region Private Fields

        #endregion

        #region Public Properties

        /// <summary>
        ///   A unique GUID for the property
        /// </summary>
        public Guid FormatId { get; private set; }

        /// <summary>
        ///   Property identifier (PID)
        /// </summary>
        public int PropertyId { get; private set; }

        #endregion

        #region Public Construction

        /// <summary>
        ///   PropertyKey Constructor
        /// </summary>
        /// <param name = "formatId">A unique GUID for the property</param>
        /// <param name = "propertyId">Property identifier (PID)</param>
        public PropertyKey(Guid formatId, Int32 propertyId) : this()
        {
            FormatId = formatId;
            PropertyId = propertyId;
        }

        /// <summary>
        ///   PropertyKey Constructor
        /// </summary>
        /// <param name = "formatId">A string represenstion of a GUID for the property</param>
        /// <param name = "propertyId">Property identifier (PID)</param>
        public PropertyKey(string formatId, Int32 propertyId) : this()
        {
            FormatId = new Guid(formatId);
            PropertyId = propertyId;
        }

        #endregion

        #region IEquatable<PropertyKey> Members

        /// <summary>
        ///   Returns whether this object is equal to another. This is vital for performance of value types.
        /// </summary>
        /// <param name = "other">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public bool Equals(PropertyKey other)
        {
            return other.Equals((object) this);
        }

        #endregion

        #region equality and hashing

        /// <summary>
        ///   Returns the hash code of the object. This is vital for performance of value types.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return FormatId.GetHashCode() ^ PropertyId;
        }

        /// <summary>
        ///   Returns whether this object is equal to another. This is vital for performance of value types.
        /// </summary>
        /// <param name = "obj">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is PropertyKey))
                return false;

            var other = (PropertyKey) obj;
            return other.FormatId.Equals(FormatId) && (other.PropertyId == PropertyId);
        }

        /// <summary>
        ///   Implements the == (equality) operator.
        /// </summary>
        /// <param name = "a">Object a.</param>
        /// <param name = "b">Object b.</param>
        /// <returns>true if object a equals object b. false otherwise.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
        public static bool operator ==(PropertyKey a, PropertyKey b)
        {
            return a.Equals(b);
        }

        /// <summary>
        ///   Implements the != (inequality) operator.
        /// </summary>
        /// <param name = "a">Object a.</param>
        /// <param name = "b">Object b.</param>
        /// <returns>true if object a does not equal object b. false otherwise.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b"),
         SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
        public static bool operator !=(PropertyKey a, PropertyKey b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        ///   Override ToString() to provide a user friendly string representation
        /// </summary>
        /// <returns>String representing the property key</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public override string ToString()
        {
            return String.Format("{0}, {1}", FormatId.ToString("B"), PropertyId);
        }

        #endregion
    }
}