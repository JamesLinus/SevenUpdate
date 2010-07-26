#region GNU Public License Version 3

// Copyright 2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

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