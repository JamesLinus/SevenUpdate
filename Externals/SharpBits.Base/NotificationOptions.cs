// <copyright file="NotificationOptions.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System;

    /// <summary>Provides method of job notification.</summary>
    [Flags]
    public enum NotificationOptions
    {
        /// <summary>All of the files in the job have been transferred.</summary>
        JobTransferred = 1, 

        /// <summary>An error has occurred.</summary>
        JobErrorOccurred = 2, 

        /// <summary>Event notification is disabled. BITS ignores the other flags.</summary>
        NotificationDisabled = 4, 

        /// <summary>
        ///   The job has been modified. For example, a property value changed, the state of the job changed, or
        ///   progress is made transferring the files.
        /// </summary>
        JobModified = 8, 
    }
}