// ***********************************************************************
// <copyright file="LocalizedObjectOperation.cs"
//            project="WPFLocalizeExtension"
//            assembly="WPFLocalizeExtension"
//            solution="SevenUpdate"
//            company="Bernhard Millauer">
//     Copyright (c) Bernhard Millauer. All rights reserved.
// </copyright>
// <author username="SeriousM">Bernhard Millauer</author>
// ***********************************************************************
namespace WPFLocalizeExtension.Engine
{
    using System;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Implements the LocalizedObjectOperation.
    /// </summary>
    public static class LocalizedObjectOperation
    {
        #region Public Methods

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <param name="errorNo">
        /// The error no.
        /// </param>
        /// <returns>
        /// The resolved string or a default error string.
        /// </returns>
        public static string GetErrorMessage(int errorNo)
        {
            try
            {
                return
                    (string)
                    Localize.Instance.GetLocalizedObject<object>(
                        Localize.Instance.GetAssemblyName(Assembly.GetExecutingAssembly()), "ResError", "ERR_" + errorNo, Localize.Instance.Culture);
            }
            catch (Exception)
            {
                return "No localized ErrorMessage founded for Error: " + errorNo;
            }
        }

        /// <summary>
        /// Gets the GUI string.
        /// </summary>
        /// <param name="key">
        /// The resource identifier.
        /// </param>
        /// <returns>
        /// The resolved string or a default error string.
        /// </returns>
        public static string GetGuiString(string key)
        {
            return GetGuiString(key, Localize.Instance.Culture);
        }

        /// <summary>
        /// Gets the GUI string.
        /// </summary>
        /// <param name="key">
        /// The resource identifier.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The resolved string or a default error string.
        /// </returns>
        public static string GetGuiString(string key, CultureInfo language)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (key != null && String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is empty", "key");
            }

            try
            {
                return (string)Localize.Instance.GetLocalizedObject<object>(Localize.Instance.GetAssemblyName(Assembly.GetExecutingAssembly()), "ResGui", key, language);
            }
            catch (Exception)
            {
                return "No localized GuiMessage founded for key '" + key + "'";
            }
        }

        /// <summary>
        /// Gets the help string.
        /// </summary>
        /// <param name="key">
        /// The resource identifier.
        /// </param>
        /// <returns>
        /// The resolved string or a default error string.
        /// </returns>
        public static string GetHelpString(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (key != null && String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is empty", "key");
            }

            try
            {
                return
                    (string)
                    Localize.Instance.GetLocalizedObject<object>(
                        Localize.Instance.GetAssemblyName(Assembly.GetExecutingAssembly()), "ResHelp", key, Localize.Instance.Culture);
            }
            catch (Exception)
            {
                return "No localized HelpMessage founded for key '" + key + "'";
            }
        }

        /// <summary>
        /// Gets the maintenance string.
        /// </summary>
        /// <param name="key">
        /// The resource identifier.
        /// </param>
        /// <returns>
        /// The resolved string or a default error string.
        /// </returns>
        public static string GetMaintenanceString(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (key != null && String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is empty", "key");
            }

            try
            {
                return
                    (string)
                    Localize.Instance.GetLocalizedObject<object>(
                        Localize.Instance.GetAssemblyName(Assembly.GetExecutingAssembly()), "ResMaintenance", key, Localize.Instance.Culture);
            }
            catch (Exception)
            {
                return "No localized MaintenanceMessage founded for key '" + key + "'";
            }
        }

        /// <summary>
        /// Gets the update agent string.
        /// </summary>
        /// <param name="key">
        /// The resource identifier.
        /// </param>
        /// <returns>
        /// The resolved string or a default error string.
        /// </returns>
        public static string GetUpdateAgentString(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (key != null && String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key is empty", "key");
            }

            try
            {
                return
                    (string)
                    Localize.Instance.GetLocalizedObject<object>(
                        Localize.Instance.GetAssemblyName(Assembly.GetExecutingAssembly()), "ResUpdateAgent", key, Localize.Instance.Culture);
            }
            catch (Exception)
            {
                return "No localized UpdateAgentMessage founded for key '" + key + "'";
            }
        }

        #endregion
    }
}