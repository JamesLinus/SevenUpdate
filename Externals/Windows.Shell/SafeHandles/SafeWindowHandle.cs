//Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Windows.Internal
{
    /// <summary>
    ///   Safe Window Handle
    /// </summary>
    public class SafeWindowHandle : ZeroInvalidHandle
    {
        /// <summary>
        ///   Release the handle
        /// </summary>
        /// <returns>true if handled is release successfully, false otherwise</returns>
        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
                return true;

            return CoreNativeMethods.DestroyWindow(handle) != 0;
        }
    }
}