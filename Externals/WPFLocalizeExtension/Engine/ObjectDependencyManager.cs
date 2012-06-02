// <copyright file="ObjectDependencyManager.cs" project="WPFLocalizeExtension">Bernhard Millauer</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace WPFLocalizeExtension.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>This class ensures, that a specific object lives as long a associated object is alive.</summary>
    public static class ObjectDependencyManager
    {
        /// <summary>This member holds the list of all <c>WeakReference</c>s and their appropriate objects.</summary>
        private static readonly Dictionary<object, List<WeakReference>> InternalList =
            new Dictionary<object, List<WeakReference>>();

        /// <summary>Adds an object dependency</summary>
        /// <param name="weakRef">The weak reference</param>
        /// <param name="value">The object value</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddObjectDependency(WeakReference weakRef, object value)
        {
            // run the clean up to ensure that only objects are watched they are really still alive
            CleanUp();

            if (weakRef == null)
            {
                throw new ArgumentNullException("weakRef");
            }

            // if the objToHold is null, we cannot handle this afterwards.
            if (value == null)
            {
                throw new ArgumentNullException("value", "The objToHold cannot be null");
            }

            // if the objToHold is a weak reference, we cannot handle this type afterwards.
            if (value.GetType() == typeof(WeakReference))
            {
                throw new ArgumentException("value cannot be type of WeakReference", "value");
            }

            // if the target of the weak reference is the objToHold, this would be a cycling play.
            if (weakRef.Target == value)
            {
                throw new InvalidOperationException("The WeakReference.Target cannot be the same as value");
            }

            // check if the objToHold is contained in the internalList.
            if (!InternalList.ContainsKey(value))
            {
                // add the objToHold to the internal list.
                var lst = new List<WeakReference> { weakRef };

                InternalList.Add(value, lst);
            }
            else
            {
                // otherwise, check if the weakRefDp exists and add it if necessary
                List<WeakReference> lst = InternalList[value];
                if (!lst.Contains(weakRef))
                {
                    lst.Add(weakRef);
                }
            }

            // return the status of the registration
            return;
        }

        /// <summary>This method cleans up all independent (!<c>WeakReference</c>.IsAlive) objects or a single object.</summary>
        /// <param name="value">If defined, the associated object dependency will be removed instead of a full CleanUp.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void CleanUp(object value = null)
        {
            // if a particular object is passed, remove it.
            if (value != null)
            {
                // if the key wasn't found, throw an exception.
                if (!InternalList.Remove(value))
                {
                    throw new ArgumentException("Key was not found!");
                }

                // stop here
                return;
            }

            // perform an full clean up

            // this list will hold all keys they has to be removed
            var keysToRemove = new List<object>();

            // step through all object dependencies
            foreach (var kvp in InternalList)
            {
                // step recursive through all weak references
                for (int i = kvp.Value.Count - 1; i >= 0; i--)
                {
                    // if this weak reference is no more alive, remove it
                    if (!kvp.Value[i].IsAlive)
                    {
                        kvp.Value.RemoveAt(i);
                    }
                }

                // if the list of weak references is empty, remove the whole entry
                if (kvp.Value.Count == 0)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            // step recursive through all keys that have to be remove
            for (int i = keysToRemove.Count - 1; i >= 0; i--)
            {
                // remove the key from the internalList
                InternalList.Remove(keysToRemove[i]);
            }

            // clear up the keysToRemove
            keysToRemove.Clear();
        }
    }
}