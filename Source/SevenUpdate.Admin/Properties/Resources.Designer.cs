﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SevenUpdate.Admin.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SevenUpdate.Admin.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Checking for updates....
        /// </summary>
        public static string CheckingForUpdates {
            get {
                return ResourceManager.GetString("CheckingForUpdates", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downloading updates....
        /// </summary>
        public static string DownloadingUpdates {
            get {
                return ResourceManager.GetString("DownloadingUpdates", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downloading updates... ({0} out of {1} files complete).
        /// </summary>
        public static string DownloadProgress {
            get {
                return ResourceManager.GetString("DownloadProgress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Seven Update has completed the installation of updates.
        /// </summary>
        public static string InstallationCompleted {
            get {
                return ResourceManager.GetString("InstallationCompleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Installing updates....
        /// </summary>
        public static string InstallingUpdates {
            get {
                return ResourceManager.GetString("InstallingUpdates", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Installing updates {0}% complete....
        /// </summary>
        public static string InstallProgress {
            get {
                return ResourceManager.GetString("InstallProgress", resourceCulture);
            }
        }
        
        public static System.Drawing.Icon TrayIcon {
            get {
                object obj = ResourceManager.GetObject("TrayIcon", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updates have been downloaded.
        /// </summary>
        public static string UpdatesDownloaded {
            get {
                return ResourceManager.GetString("UpdatesDownloaded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updates have been downloaded, click here to view them.
        /// </summary>
        public static string UpdatesDownloadedViewThem {
            get {
                return ResourceManager.GetString("UpdatesDownloadedViewThem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updates have been found.
        /// </summary>
        public static string UpdatesFound {
            get {
                return ResourceManager.GetString("UpdatesFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updates have been found, click here to view them.
        /// </summary>
        public static string UpdatesFoundViewThem {
            get {
                return ResourceManager.GetString("UpdatesFoundViewThem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updates have been installed.
        /// </summary>
        public static string UpdatesInstalled {
            get {
                return ResourceManager.GetString("UpdatesInstalled", resourceCulture);
            }
        }
    }
}
