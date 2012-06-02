// <copyright file="BitsCredentials.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The credentials for the job.</summary>
    public class BitsCredentials
    {
        /// <summary>Gets or sets the authentication scheme.</summary>
        /// <value>The authentication scheme.</value>
        public AuthenticationScheme AuthenticationScheme { get; set; }

        /// <summary>Gets or sets the authentication target.</summary>
        /// <value>The authentication target.</value>
        public AuthenticationTarget AuthenticationTarget { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
    }
}