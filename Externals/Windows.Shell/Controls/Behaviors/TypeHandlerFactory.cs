// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;

#endregion

namespace Microsoft.Windows.Controls
{
    // TypeHandlerFactory allows one to register handlers for specific Types
    // When asking for a handler of the type you will get the most specific
    // handler available (the most appropriate for the type).
    internal abstract class TypeHandlerFactory<TypeHandler>
    {
        private Dictionary<Type, TypeHandler> handlerCache;
        private List<TypeHandler> handlers;

        public ICollection<TypeHandler> Handlers
        {
            get
            {
                InitializeIfNecessary();
                return handlers;
            }
        }

        protected bool IsInitialized { get { return handlers != null; } }

        protected TypeHandler GetHandler(Type type)
        {
            TypeHandler handler;
            if (!GetCachedHandler(type, out handler))
            {
                handler = DetermineBestHandler(GetDefaultHandler(type), type);
                CacheHandler(type, handler);
            }

            return handler;
        }

        protected bool GetCachedHandler(Type type, out TypeHandler handler)
        {
            InitializeIfNecessary();
            return handlerCache.TryGetValue(type, out handler);
        }

        protected void CacheHandler(Type type, TypeHandler handler)
        {
            InitializeIfNecessary();
            handlerCache[type] = handler;
        }

        protected TypeHandler DetermineBestHandler(TypeHandler handler, Type type)
        {
            InitializeIfNecessary();
            Type bestType = typeof (object);
            Type bestIntefaceImplementationType = typeof (object);

            // Look through all the handlers for one that applies to the specified type.
            // Choose the one that applies to the most derived type or interface.
            foreach (TypeHandler candidate in handlers)
            {
                Type candidateType = GetBaseType(candidate);
                if (candidateType.IsAssignableFrom(type) || (candidateType.IsGenericTypeDefinition && IsGenericTypeDefinitionOf(candidateType, type)))
                {
                    // Compare the current best to the candidate and take the candidate if it is more derived/specific.
                    if (bestType.IsAssignableFrom(candidateType) || (candidateType.IsInterface && !candidateType.IsAssignableFrom(bestIntefaceImplementationType)))
                    {
                        handler = candidate;
                        bestType = candidateType;
                        bestIntefaceImplementationType = GetImplementingType(candidateType, type);
                    }
                }
            }

            return handler;
        }

        /// <summary>
        ///   Given a base type, which may be an interface, and a target type, this returns the most-base type (not an interface)
        ///   of type that implements or is equal to the base type.
        /// </summary>
        /// <param name = "baseType" />
        /// <param name = "targetType" />
        /// <returns />
        protected static Type GetImplementingType(Type baseType, Type targetType)
        {
            Debug.Assert(DoesTypeImplement(baseType, targetType), "GetImplementingType should be given a target type that implements the base type.");

            if (!baseType.IsInterface && baseType.IsAssignableFrom(targetType))
                return baseType;

            // Walk up the base type chain starting at type and return the most-base type that still implements the base type passed in.
            Type implementingType = targetType;
            while (implementingType.BaseType != null && DoesTypeImplement(baseType, implementingType.BaseType))
                implementingType = implementingType.BaseType;

            Debug.Assert(implementingType != null && DoesTypeImplement(baseType, implementingType), "Error finding class that implements given interface.");

            return implementingType;
        }

        private static bool DoesTypeImplement(Type baseType, Type targetType)
        {
            return baseType.IsAssignableFrom(targetType) || (baseType.IsGenericTypeDefinition && IsGenericTypeDefinitionOf(baseType, targetType));
        }

        /// <summary>
        ///   True if the target type is assignable from a generic base class which matches
        ///   the definition
        /// </summary>
        private static bool IsGenericTypeDefinitionOf(Type baseDefinition, Type targetType)
        {
            Debug.Assert(baseDefinition != null);
            Debug.Assert(baseDefinition.IsGenericTypeDefinition);

            while (targetType != null)
            {
                Type genericDefinition = GetGenericTypeDefinition(targetType);
                if (genericDefinition != null && baseDefinition.IsAssignableFrom(genericDefinition))
                    return true;

                targetType = targetType.BaseType;
            }

            return false;
        }

        private static Type GetGenericTypeDefinition(Type type)
        {
            try
            {
                if (type.IsGenericType)
                    return type.GetGenericTypeDefinition();
            }
            catch (InvalidOperationException)
            {
                // May be thrown by GetGenericTypeDefinition
            }
            catch (NotSupportedException)
            {
                // May be thrown by GetGenericTypeDefinition
            }
            catch (InvalidCastException)
            {
                // Potentially thrown by either API
            }
            catch (NullReferenceException)
            {
                // Potentially thrown by either API
            }
            catch (SecurityException)
            {
                // Potentially thrown by either API
            }

            return null;
        }

        protected void RegisterHandler(TypeHandler handler)
        {
            InitializeIfNecessary();
            handlers.Add(handler);

            // Clear the look-up cache.
            handlerCache.Clear();
        }

        protected void UnregisterHandler(TypeHandler handler)
        {
            InitializeIfNecessary();

            // Remove the given handler from the list of all handler.
            handlers.Remove(handler);

            // Clear the look-up cache.
            handlerCache.Clear();
        }

        protected virtual void Initialize()
        {
            handlers = new List<TypeHandler>();
            handlerCache = new Dictionary<Type, TypeHandler>();
        }

        protected void InitializeIfNecessary()
        {
            if (!IsInitialized)
                Initialize();
        }

        /// <summary>
        ///   Return the default handler if any for the given type.
        /// </summary>
        /// <param name = "type">Type for handler</param>
        /// <returns>Default handler if any</returns>
        protected virtual TypeHandler GetDefaultHandler(Type type)
        {
            return default(TypeHandler);
        }

        /// <summary>
        ///   Return the base type for the given handler.
        /// </summary>
        /// <param name = "handler">Handler</param>
        /// <returns>Base type</returns>
        protected abstract Type GetBaseType(TypeHandler handler);
    }
}