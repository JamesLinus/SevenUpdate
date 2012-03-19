// <copyright file="JobPriority.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The <c>BitsJob</c> priority.</summary>
    public enum JobPriority
    {
        /// <summary>Downloads without bandwidth restriction.</summary>
        Foreground = 0, 

        /// <summary>Downloads with a 80% bandwidth use.</summary>
        High = 1, 

        /// <summary>Downloads using bandwidth available.</summary>
        Normal = 2, 

        /// <summary>Download slow, giving other net use priority.</summary>
        Low = 3, 
    }
}