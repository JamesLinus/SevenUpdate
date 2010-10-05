//***********************************************************************
// Assembly         : SharpBits.Base
// Author           : sevenalive
// Created          : 10-05-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    /// </summary>
    public class JobErrorNotificationEventArgs : JobNotificationEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="error">
        /// </param>
        internal JobErrorNotificationEventArgs(BitsError error)
        {
            this.Error = error;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public BitsError Error { get; private set; }

        #endregion
    }
}