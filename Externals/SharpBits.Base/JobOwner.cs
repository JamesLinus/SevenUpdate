// ***********************************************************************
// <copyright file="JobOwner.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

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