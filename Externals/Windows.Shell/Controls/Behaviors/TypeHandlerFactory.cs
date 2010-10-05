//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Security;

    // TypeHandlerFactory allows one to register handlers for specific Types
    // When asking for a handler of the type you will get the most specific
    // handler available (the most appropriate for the type).
    /// <summary>
    /// </summary>
    /// <typeparam name="TypeHandler">
    /// </typeparam>
    internal abstract class TypeHandlerFactory<TypeHandler>
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private Dictionary<Type, TypeHandler> handlerCache;

        /// <summary>
        /// </summary>
        private List<TypeHandler> handlers;

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public ICollection<TypeHandler> Handlers
        {
            get
            {
                this.InitializeIfNecessary();
                return this.handlers;
            }
        }

        /// <summary>
        /// </summary>
        protected bool IsInitialized
        {
            get
            {
                return this.handlers != null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Given a base type, which may be an interface, and a target type, this returns the most-base type (not an interface)
        ///   of type that implements or is equal to the base type.
        /// </summary>
        /// <param name="baseType">
        /// </param>
        /// <param name="targetType">
        /// </param>
        /// <returns>
        /// </returns>
        protected static Type GetImplementingType(Type baseType, Type targetType)
        {
            Debug.Assert(DoesTypeImplement(baseType, targetType), "GetImplementingType should be given a target type that implements the base type.");

            if (!baseType.IsInterface && baseType.IsAssignableFrom(targetType))
            {
                return baseType;
            }

            // Walk up the base type chain starting at type and return the most-base type that still implements the base type passed in.
            var implementingType = targetType;
            while (implementingType.BaseType != null && DoesTypeImplement(baseType, implementingType.BaseType))
            {
                implementingType = implementingType.BaseType;
            }

            Debug.Assert(implementingType != null && DoesTypeImplement(baseType, implementingType), "Error finding class that implements given interface.");

            return implementingType;
        }

        /// <summary>
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <param name="handler">
        /// </param>
        protected void CacheHandler(Type type, TypeHandler handler)
        {
            this.InitializeIfNecessary();
            this.handlerCache[type] = handler;
        }

        /// <summary>
        /// </summary>
        /// <param name="handler">
        /// </param>
        /// <param name="type">
        /// </param>
        /// <returns>
        /// </returns>
        protected TypeHandler DetermineBestHandler(TypeHandler handler, Type type)
        {
            this.InitializeIfNecessary();
            var bestType = typeof(object);
            var bestIntefaceImplementationType = typeof(object);

            // Look through all the handlers for one that applies to the specified type.
            // Choose the one that applies to the most derived type or interface.
            foreach (var candidate in this.handlers)
            {
                var candidateType = this.GetBaseType(candidate);
                if (!candidateType.IsAssignableFrom(type) && (!candidateType.IsGenericTypeDefinition || !IsGenericTypeDefinitionOf(candidateType, type)))
                {
                    continue;
                }

                // Compare the current best to the candidate and take the candidate if it is more derived/specific.
                if (!bestType.IsAssignableFrom(candidateType) && (!candidateType.IsInterface || candidateType.IsAssignableFrom(bestIntefaceImplementationType)))
                {
                    continue;
                }

                handler = candidate;
                bestType = candidateType;
                bestIntefaceImplementationType = GetImplementingType(candidateType, type);
            }

            return handler;
        }

        /// <summary>
        /// Return the base type for the given handler.
        /// </summary>
        /// <param name="handler">
        /// Handler
        /// </param>
        /// <returns>
        /// Base type
        /// </returns>
        protected abstract Type GetBaseType(TypeHandler handler);

        /// <summary>
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <param name="handler">
        /// </param>
        /// <returns>
        /// </returns>
        protected bool GetCachedHandler(Type type, out TypeHandler handler)
        {
            this.InitializeIfNecessary();
            return this.handlerCache.TryGetValue(type, out handler);
        }

        /// <summary>
        /// Return the default handler if any for the given type.
        /// </summary>
        /// <param name="type">
        /// Type for handler
        /// </param>
        /// <returns>
        /// Default handler if any
        /// </returns>
        protected virtual TypeHandler GetDefaultHandler(Type type)
        {
            return default(TypeHandler);
        }

        /// <summary>
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <returns>
        /// </returns>
        protected TypeHandler GetHandler(Type type)
        {
            TypeHandler handler;
            if (!this.GetCachedHandler(type, out handler))
            {
                handler = this.DetermineBestHandler(this.GetDefaultHandler(type), type);
                this.CacheHandler(type, handler);
            }

            return handler;
        }

        /// <summary>
        /// </summary>
        protected virtual void Initialize()
        {
            this.handlers = new List<TypeHandler>();
            this.handlerCache = new Dictionary<Type, TypeHandler>();
        }

        /// <summary>
        /// </summary>
        protected void InitializeIfNecessary()
        {
            if (!this.IsInitialized)
            {
                this.Initialize();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="handler">
        /// </param>
        protected void RegisterHandler(TypeHandler handler)
        {
            this.InitializeIfNecessary();
            this.handlers.Add(handler);

            // Clear the look-up cache.
            this.handlerCache.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="handler">
        /// </param>
        protected void UnregisterHandler(TypeHandler handler)
        {
            this.InitializeIfNecessary();

            // Remove the given handler from the list of all handler.
            this.handlers.Remove(handler);

            // Clear the look-up cache.
            this.handlerCache.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="baseType">
        /// </param>
        /// <param name="targetType">
        /// </param>
        /// <returns>
        /// </returns>
        private static bool DoesTypeImplement(Type baseType, Type targetType)
        {
            return baseType.IsAssignableFrom(targetType) || (baseType.IsGenericTypeDefinition && IsGenericTypeDefinitionOf(baseType, targetType));
        }

        /// <summary>
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <returns>
        /// </returns>
        private static Type GetGenericTypeDefinition(Type type)
        {
            try
            {
                if (type.IsGenericType)
                {
                    return type.GetGenericTypeDefinition();
                }
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

        /// <summary>
        /// True if the target type is assignable from a generic base class which matches
        ///   the definition
        /// </summary>
        /// <param name="baseDefinition">
        /// </param>
        /// <param name="targetType">
        /// </param>
        /// <returns>
        /// </returns>
        private static bool IsGenericTypeDefinitionOf(Type baseDefinition, Type targetType)
        {
            Debug.Assert(baseDefinition != null);
            Debug.Assert(baseDefinition.IsGenericTypeDefinition);

            while (targetType != null)
            {
                var genericDefinition = GetGenericTypeDefinition(targetType);
                if (genericDefinition != null && baseDefinition.IsAssignableFrom(genericDefinition))
                {
                    return true;
                }

                targetType = targetType.BaseType;
            }

            return false;
        }

        #endregion
    }
}