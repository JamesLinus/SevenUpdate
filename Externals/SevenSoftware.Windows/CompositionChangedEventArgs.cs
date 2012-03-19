// <copyright file="CompositionChangedEventArgs.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.microsoft.com/en-us/openness/licenses.aspx" name="Microsoft Public License" />

namespace SevenSoftware.Windows
{
    using System;

    /// <summary>Event argument for The CompositionChanged event.</summary>
    public class CompositionChangedEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="CompositionChangedEventArgs" /> class.</summary>
        /// <param name="isGlassEnabled">If set to <c>True</c> aero glass is enabled.</param>
        internal CompositionChangedEventArgs(bool isGlassEnabled)
        {
            this.IsGlassEnabled = isGlassEnabled;
        }

        /// <summary>Gets a value indicating whether DWM/Glass is currently enabled.</summary>
        /// <value><c>True</c> if this instance is glass enabled; otherwise, <c>False</c>.</value>
        public bool IsGlassEnabled { get; private set; }
    }
}