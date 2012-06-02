// <copyright file="ApplicationInstanceAwareness.cs" project="SevenSoftware.Windows">BladeWise</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace SevenSoftware.Windows
{
    /// <summary>
    ///   Enumerator used to define the awareness of an application, when dealing with subsequent instances of the
    ///   application itself.
    /// </summary>
    public enum ApplicationInstanceAwareness
    {
        /// <summary>
        ///   The awareness is global, meaning that the first application instance is aware of any other instances
        ///   running on the host.
        /// </summary>
        Host = 0x00, 

        /// <summary>
        ///   The awareness is local, meaning that the first application instance is aware only of other instances
        ///   running in the current user session.
        /// </summary>
        UserSession = 0x01
    }
}