// ***********************************************************************
// <copyright file="LocalizedObjectOperation.cs" project="WPFLocalizeExtension" assembly="WPFLocalizeExtension" solution="SevenUpdate" company="Bernhard Millauer">
//     Copyright (c) Bernhard Millauer. All rights reserved.
// </copyright>
// <author username="SeriousM">Bernhard Millauer</author>
// <license href="http://wpflocalizeextension.codeplex.com/license">Microsoft Public License</license>
// ***********************************************************************

namespace WPFLocalizeExtension.Engine
{
    using System;
    using System.Globalization;
    using System.Reflection;

    /// <summary>Implements the LocalizedObjectOperation.</summary>
    public static class LocalizedObjectOperation
    {
        #region Public Methods

        /// <summary>Gets the error message.</summary>
        /// <param name="errorNo">The error no.</param>
        /// <returns>The resolved string or a default error string.</returns>
        public static string GetErrorMessage(int errorNo)
        {
            return
                (string)
                Localize.Instance.GetLocalizedObject<object>(
                    Localize.GetAssemblyName(Assembly.GetExecutingAssembly()),
                    "ResError",
                    "ERR_" + errorNo,
                    Localize.Instance.Culture);
        }

        /// <summary>Gets the GUI string.</summary>
        /// <param name="key">The resource identifier.</param>
        /// <returns>The resolved string or a default error string.</returns>
        public static string GetGuiString(string key)
        {
            return GetGuiString(key, Localize.Instance.Culture);
        }

        /// <summary>Gets the help string.</summary>
        /// <param name="key">The resource identifier.</param>
        /// <returns>The resolved string or a default error string.</returns>
        public static string GetHelpString(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (key != null && string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is empty", "key");
            }

            return
                (string)
                Localize.Instance.GetLocalizedObject<object>(
                    Localize.GetAssemblyName(Assembly.GetExecutingAssembly()), "ResHelp", key, Localize.Instance.Culture);
        }

        /// <summary>Gets the maintenance string.</summary>
        /// <param name="key">The resource identifier.</param>
        /// <returns>The resolved string or a default error string.</returns>
        public static string GetMaintenanceString(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (key != null && string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is empty", "key");
            }

            return
                (string)
                Localize.Instance.GetLocalizedObject<object>(
                    Localize.GetAssemblyName(Assembly.GetExecutingAssembly()),
                    "ResMaintenance",
                    key,
                    Localize.Instance.Culture);
        }

        /// <summary>Gets the update agent string.</summary>
        /// <param name="key">The resource identifier.</param>
        /// <returns>The resolved string or a default error string.</returns>
        public static string GetUpdateAgentString(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (key != null && string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is empty", "key");
            }

            return
                (string)
                Localize.Instance.GetLocalizedObject<object>(
                    Localize.GetAssemblyName(Assembly.GetExecutingAssembly()),
                    "ResUpdateAgent",
                    key,
                    Localize.Instance.Culture);
        }

        #endregion

        #region Methods

        /// <summary>Gets the GUI string.</summary>
        /// <param name="key">The resource identifier.</param>
        /// <param name="language">The language.</param>
        /// <returns>The resolved string or a default error string.</returns>
        private static string GetGuiString(string key, CultureInfo language)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (key != null && string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is empty", "key");
            }

            return
                (string)
                Localize.Instance.GetLocalizedObject<object>(
                    Localize.GetAssemblyName(Assembly.GetExecutingAssembly()), "ResGui", key, language);
        }

        #endregion
    }
}
