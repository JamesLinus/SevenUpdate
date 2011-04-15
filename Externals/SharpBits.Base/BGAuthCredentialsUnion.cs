// ***********************************************************************
// <copyright file="BGAuthCredentialsUnion.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>The BG_AUTH_CREDENTIALS_UNION union identifies the credentials to use for the authentication scheme specified in the BG_AUTH_CREDENTIALS structure.</summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    internal struct BGAuthCredentialsUnion
    {
        /// <summary>Identifies the user name and password of the user to authenticate. For details, see the BG_BASIC_CREDENTIALS structure.</summary>
        public BGBasicCredentials Basic;
    }
}