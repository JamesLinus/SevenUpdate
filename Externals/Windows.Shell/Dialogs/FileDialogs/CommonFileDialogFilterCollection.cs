//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    using System.Collections.ObjectModel;

    using Microsoft.Windows.Shell;

    /// <summary>
    /// Provides a strongly typed collection for file dialog filters.
    /// </summary>
    public class CommonFileDialogFilterCollection : Collection<CommonFileDialogFilter>
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        internal CommonFileDialogFilterCollection()
        {
            // Make the default constructor internal so users can't instantiate this 
            // collection by themselves.
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        internal ShellNativeMethods.OmdlgFilterspec[] GetAllFilterSpecs()
        {
            var filterSpecs = new ShellNativeMethods.OmdlgFilterspec[this.Count];

            for (var i = 0; i < this.Count; i++)
            {
                filterSpecs[i] = this[i].GetFilterSpec();
            }

            return filterSpecs;
        }

        #endregion
    }
}