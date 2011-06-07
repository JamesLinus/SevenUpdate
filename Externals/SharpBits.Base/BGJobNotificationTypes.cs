// ***********************************************************************
// <copyright file="BGJobNotificationTypes.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System;

    /// <summary>
    ///   Used for the SetNotifyFlags method.
    /// </summary>
    [Flags]
    internal enum BGJobNotificationTypes
    {
        /// <summary>
        ///   All of the files in the job have been transferred.
        /// </summary>
        BGNotifyJobTransferred = 0x0001,

        /// <summary>
        ///   An error has occurred.
        /// </summary>
        BGNotifyJobError = 0x0002,

        /// <summary>
        ///   Event notification is disabled. BITS ignores the other flags.
        /// </summary>
        BGNotifyDisable = 0x0004,

        /// <summary>
        ///   The job has been modified. For example, a property value changed, the state of the job changed, or
        ///   progress is made transferring the files. This flag is ignored if command line notification is specified.
        /// </summary>
        BGNotifyJobModification = 0x0008,
    }
}