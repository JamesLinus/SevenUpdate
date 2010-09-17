//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System.Collections.Generic;

#endregion

namespace Microsoft.Windows.Shell.PropertySystem
{
    internal class ShellPropertyDescriptionsCache
    {
        private static ShellPropertyDescriptionsCache cacheInstance;
        private readonly IDictionary<PropertyKey, ShellPropertyDescription> propsDictionary;

        private ShellPropertyDescriptionsCache()
        {
            propsDictionary = new Dictionary<PropertyKey, ShellPropertyDescription>();
        }

        public static ShellPropertyDescriptionsCache Cache { get { return cacheInstance ?? (cacheInstance = new ShellPropertyDescriptionsCache()); } }

        public ShellPropertyDescription GetPropertyDescription(PropertyKey key)
        {
            if (!propsDictionary.ContainsKey(key))
                propsDictionary.Add(key, new ShellPropertyDescription(key));
            return propsDictionary[key];
        }
    }
}