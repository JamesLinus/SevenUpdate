// ***********************************************************************
// Assembly         : WPFLocalizeExtension
// Author           : Bernhard Millauer
// Created          : 09-19-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Bernhard Millauer. All rights reserved.
// ***********************************************************************
namespace WPFLocalizeExtension.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// This class ensures, that a specific object lives as long a associated object is alive.
    /// </summary>
    public static class ObjectDependencyManager
    {
        #region Constants and Fields

        /// <summary>
        ///   This member holds the list of all <see cref = "WeakReference" />s and their appropriate objects.
        /// </summary>
        private static readonly Dictionary<object, List<WeakReference>> InternalList;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref = "ObjectDependencyManager" /> class. 
        ///   Static Constructor. Creates a new instance of 
        ///   Dictionary(object, <see cref = "WeakReference" />) and set it to the <see cref = "InternalList" />.
        /// </summary>
        static ObjectDependencyManager()
        {
            InternalList = new Dictionary<object, List<WeakReference>>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method adds a new object dependency
        /// </summary>
        /// <param name="weakRef">
        /// The <see cref="WeakReference"/>, which ensures the live cycle of <paramref name="objToHold"/>
        /// </param>
        /// <param name="objToHold">
        /// The object, which should stay alive as long <paramref name="weakRef"/> is alive
        /// </param>
        /// <returns>
        /// <see langword="true"/>, if the binding was successfully, otherwise <see langword="false"/>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="objToHold"/> cannot be <see langword="null"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="objToHold"/> cannot be type of <see cref="WeakReference"/>
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="WeakReference"/>.Target cannot be the same as <paramref name="objToHold"/>
        /// </exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool AddObjectDependency(WeakReference weakRef, object objToHold)
        {
            // run the clean up to ensure that only objects are watched they are realy still alive
            CleanUp();

            // if the objToHold is null, we cannot handle this afterwards.
            if (objToHold == null)
            {
                throw new ArgumentNullException("objToHold", "The objToHold cannot be null");
            }

            // if the objToHold is a weakreference, we cannot handle this type afterwards.
            if (objToHold.GetType() == typeof(WeakReference))
            {
                throw new ArgumentException("objToHold cannot be type of WeakReference", "objToHold");
            }

            // if the target of the weakreference is the objToHold, this would be a cycling play.
            if (weakRef.Target == objToHold)
            {
                throw new InvalidOperationException("The WeakReference.Target cannot be the same as objToHold");
            }

            // holds the status of registration of the object dependency
            var itemRegistered = false;

            // check if the objToHold is contained in the internalList.
            if (!InternalList.ContainsKey(objToHold))
            {
                // add the objToHold to the internal list.
                var lst = new List<WeakReference> { weakRef };

                InternalList.Add(objToHold, lst);

                itemRegistered = true;
            }
            else
            {
                // otherweise, check if the weakRefDp exists and add it if necessary
                var lst = InternalList[objToHold];
                if (!lst.Contains(weakRef))
                {
                    lst.Add(weakRef);

                    itemRegistered = true;
                }
            }

            // return the status of the registration
            return itemRegistered;
        }

        /// <summary>
        /// This method cleans up all independent (!<see cref="WeakReference"/>.IsAlive) objects.
        /// </summary>
        public static void CleanUp()
        {
            // call the overloaded method
            CleanUp(null);
        }

        /// <summary>
        /// This method cleans up all independent (!<see cref="WeakReference"/>.IsAlive) objects or a single object.
        /// </summary>
        /// <param name="objToRemove">
        /// If defined, the associated object dependency will be removed instead of a full CleanUp
        /// </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void CleanUp(object objToRemove)
        {
            // if a particular object is passed, remove it.
            if (objToRemove != null)
            {
                // if the key wasnt found, throw an exception.
                if (!InternalList.Remove(objToRemove))
                {
                    throw new Exception("Key was not found!");
                }

                // stop here
                return;
            }

            // perform an full clean up

            // this list will hold all keys they has to be removed
            var keysToRemove = new List<object>();

            // step through all object dependenies
            foreach (var kvp in InternalList)
            {
                // step recursive through all weak references
                for (var i = kvp.Value.Count - 1; i >= 0; i--)
                {
                    // if this weak reference is no more alive, remove it
                    if (!kvp.Value[i].IsAlive)
                    {
                        kvp.Value.RemoveAt(i);
                    }
                }

                // if the list of weak references is empty, temove the whole entry
                if (kvp.Value.Count == 0)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            // step recursive through all keys that have to be remove
            for (var i = keysToRemove.Count - 1; i >= 0; i--)
            {
                // remove the key from the internalList
                InternalList.Remove(keysToRemove[i]);
            }

            // clear up the keysToRemove
            keysToRemove.Clear();
        }

        #endregion
    }
}