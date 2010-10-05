//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    /// </summary>
    public class BitsCredentials
    {
        #region Properties

        /// <summary>
        /// </summary>
        public AuthenticationScheme AuthenticationScheme { get; set; }

        /// <summary>
        /// </summary>
        public AuthenticationTarget AuthenticationTarget { get; set; }

        /// <summary>
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// </summary>
        public string UserName { get; set; }

        #endregion
    }
}