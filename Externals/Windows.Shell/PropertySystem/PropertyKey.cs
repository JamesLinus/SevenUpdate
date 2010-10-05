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
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Globalization;
    using global::System.Runtime.InteropServices;

    /// <summary>
    /// Defines a unique key for a Shell Property
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PropertyKey : IEquatable<PropertyKey>
    {
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
        /// PropertyKey Constructor
        /// </summary>
        /// <param name="formatId">
        /// A unique GUID for the property
        /// </param>
        /// <param name="propertyId">
        /// Property identifier (PID)
        /// </param>
        public PropertyKey(Guid formatId, int propertyId)
            : this()
        {
            this.FormatId = formatId;
            this.PropertyId = propertyId;
        }

        /// <summary>
        /// PropertyKey Constructor
        /// </summary>
        /// <param name="formatId">
        /// A string represenstion of a GUID for the property
        /// </param>
        /// <param name="propertyId">
        /// Property identifier (PID)
        /// </param>
        public PropertyKey(string formatId, int propertyId)
            : this()
        {
            this.FormatId = new Guid(formatId);
            this.PropertyId = propertyId;
        }

        #endregion

        #region IEquatable<PropertyKey> Members

        /// <summary>
        /// Returns whether this object is equal to another. This is vital for performance of value types.
        /// </summary>
        /// <param name="other">
        /// The object to compare against.
        /// </param>
        /// <returns>
        /// Equality result.
        /// </returns>
        public bool Equals(PropertyKey other)
        {
            return other.Equals((object)this);
        }

        #endregion

        #region equality and hashing

        /// <summary>
        /// Returns the hash code of the object. This is vital for performance of value types.
        /// </summary>
        /// <returns>
        /// </returns>
        public override int GetHashCode()
        {
            return this.FormatId.GetHashCode() ^ this.PropertyId;
        }

        /// <summary>
        /// Returns whether this object is equal to another. This is vital for performance of value types.
        /// </summary>
        /// <param name="obj">
        /// The object to compare against.
        /// </param>
        /// <returns>
        /// Equality result.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is PropertyKey))
            {
                return false;
            }

            var other = (PropertyKey)obj;
            return other.FormatId.Equals(this.FormatId) && (other.PropertyId == this.PropertyId);
        }

        /// <summary>
        ///   Implements the == (equality) operator.
        /// </summary>
        /// <param name = "a">Object a.</param>
        /// <param name = "b">Object b.</param>
        /// <returns>true if object a equals object b. false otherwise.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
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
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
        public static bool operator !=(PropertyKey a, PropertyKey b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Override ToString() to provide a user friendly string representation
        /// </summary>
        /// <returns>
        /// String representing the property key
        /// </returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "{0}, {1}", this.FormatId.ToString("B", CultureInfo.CurrentCulture), this.PropertyId);
        }

        #endregion
    }
}