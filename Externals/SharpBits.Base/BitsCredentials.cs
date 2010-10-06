// ***********************************************************************
// Assembly         : SharpBits.Base
// Author           : xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    /// The credentials for the job
    /// </summary>
    public class BitsCredentials
    {
        #region Properties

        /// <summary>
        /// Gets or sets the authentication scheme.
        /// </summary>
        /// <value>The authentication scheme.</value>
        public AuthenticationScheme AuthenticationScheme { get; set; }

        /// <summary>
        /// Gets or sets the authentication target.
        /// </summary>
        /// <value>The authentication target.</value>
        public AuthenticationTarget AuthenticationTarget { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        #endregion
    }
}