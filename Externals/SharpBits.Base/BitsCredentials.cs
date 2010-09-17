// //Copyright (c) xidar solutions
// //Modified by Robert Baker, Seven Software 2010.
namespace SharpBits.Base
{
    public abstract class BitsCredentials
    {
        public AuthenticationScheme AuthenticationScheme { get; set; }

        public AuthenticationTarget AuthenticationTarget { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}