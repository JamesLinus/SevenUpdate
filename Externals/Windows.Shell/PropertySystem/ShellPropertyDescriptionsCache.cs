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
    using global::System.Collections.Generic;

    /// <summary>
    /// </summary>
    internal class ShellPropertyDescriptionsCache
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly IDictionary<PropertyKey, ShellPropertyDescription> propsDictionary;

        /// <summary>
        /// </summary>
        private static ShellPropertyDescriptionsCache cacheInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        private ShellPropertyDescriptionsCache()
        {
            this.propsDictionary = new Dictionary<PropertyKey, ShellPropertyDescription>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public static ShellPropertyDescriptionsCache Cache
        {
            get
            {
                return cacheInstance ?? (cacheInstance = new ShellPropertyDescriptionsCache());
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <returns>
        /// </returns>
        public ShellPropertyDescription GetPropertyDescription(PropertyKey key)
        {
            if (!this.propsDictionary.ContainsKey(key))
            {
                this.propsDictionary.Add(key, new ShellPropertyDescription(key));
            }

            return this.propsDictionary[key];
        }

        #endregion
    }
}