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