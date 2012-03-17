// ***********************************************************************
// <copyright file="JobState.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>The current status of the <c>BitsJob</c>.</summary>
    public enum JobState
    {
        /// <summary>The job is queued to be ran.</summary>
        Queued = 0, 

        /// <summary>Connecting to the remote server.</summary>
        Connecting = 1, 

        /// <summary>Transferring the files.</summary>
        Transferring = 2, 

        /// <summary>Transfer is paused.</summary>
        Suspended = 3, 

        /// <summary>An fatal error occurred.</summary>
        Error = 4, 

        /// <summary>A non-fatal error occurred.</summary>
        TransientError = 5, 

        /// <summary>The job has completed.</summary>
        Transferred = 6, 

        /// <summary>Ready to run the job.</summary>
        Acknowledged = 7, 

        /// <summary>The job was canceled.</summary>
        Canceled = 8, 
    }
}