// ***********************************************************************
// <copyright file="ApplicationInstanceAwareness.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) BladeWise. All rights reserved.
// </copyright>
// <author username="BladeWise">BladeWise</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://wpfinstanceawareapp.codeplex.com/license">Microsoft Public License</license>
// ***********************************************************************

namespace System.Windows
{
    /// <summary>Enumerator used to define the awareness of an application, when dealing with subsequent instances of the application itself.</summary>
    public enum ApplicationInstanceAwareness
    {
        /// <summary>The awareness is global, meaning that the first application instance is aware of any other instances running on the host.</summary>
        Host = 0x00,

        /// <summary>The awareness is local, meaning that the first application instance is aware only of other instances running in the current user session.</summary>
        UserSession = 0x01
    }
}