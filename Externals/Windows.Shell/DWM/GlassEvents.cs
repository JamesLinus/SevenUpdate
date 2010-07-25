#region

using System;

#endregion

namespace Microsoft.Windows.Dwm
{
    /// <summary>
    ///   Event argument for The GlassAvailabilityChanged event
    /// </summary>
    public class AeroGlassCompositionChangedEvenArgs : EventArgs
    {
        internal AeroGlassCompositionChangedEvenArgs(bool avilability)
        {
            GlassAvailable = avilability;
        }

        /// <summary>
        ///   The new GlassAvailable state
        /// </summary>
        public bool GlassAvailable { get; private set; }
    }

    /// <summary>
    ///   Sent when the availability of the desktop Glass effect is changed
    /// </summary>
    /// <param name = "sender">The AeroGlassWindow that is affected by this change</param>
    /// <param name = "e">The new state of the glass availability</param>
    public delegate void AeroGlassCompositionChangedEvent(object sender, AeroGlassCompositionChangedEvenArgs e);
}