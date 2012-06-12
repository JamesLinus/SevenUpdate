// <copyright file="LocalizedObjectOperation.cs" project="WPFLocalizeExtension">Bernhard Millauer</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace WPFLocalizeExtension.Engine
{
    using System;
    using System.Globalization;
    using System.Reflection;

    /// <summary>Implements the LocalizedObjectOperation.</summary>
    public static class LocalizedObjectOperation
    {
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

        /// <summary>Gets the GUI string.</summary>
        /// <param name="key">The resource identifier.</param>
        /// <param name="language">The language.</param>
        /// <returns>The resolved string or a default error string.</returns>
        static string GetGuiString(string key, CultureInfo language)
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
    }
}