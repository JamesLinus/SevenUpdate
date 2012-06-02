// <copyright file="JobOwner.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>Specifies the owner of the current <c>BitsJob</c>.</summary>
    public enum JobOwner
    {
        /// <summary>The current logged in user.</summary>
        CurrentUser = 0, 

        /// <summary>The administrators group or system.</summary>
        AllUsers = 1, 
    }
}