//Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System.Collections.ObjectModel;
using Microsoft.Windows.Shell;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Provides a strongly typed collection for file dialog filters.
    /// </summary>
    public class CommonFileDialogFilterCollection : Collection<CommonFileDialogFilter>
    {
        internal CommonFileDialogFilterCollection()
        {
            // Make the default constructor internal so users can't instantiate this 
            // collection by themselves.
        }

        internal ShellNativeMethods.COMDLG_FILTERSPEC[] GetAllFilterSpecs()
        {
            var filterSpecs = new ShellNativeMethods.COMDLG_FILTERSPEC[Count];

            for (int i = 0; i < Count; i++)
                filterSpecs[i] = this[i].GetFilterSpec();

            return filterSpecs;
        }
    }
}